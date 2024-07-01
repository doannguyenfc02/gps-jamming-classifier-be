using gps_jamming_classifier_be.Data;
using gps_jamming_classifier_be.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Diagnostics;

namespace gps_jamming_classifier_be.Services
{
    public class SignalProcessingService
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SignalProcessingService> _logger;

        public SignalProcessingService(IServiceScopeFactory scopeFactory, ILogger<SignalProcessingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<object> ProcessFile(IFormFile file, int numImages, double fs, double time, string fileName)
        {
            await _semaphore.WaitAsync();

            try
            {
                const int chunkSize = 5 * 1024 * 1024; // 5 MB
                long fileLength = file.Length;
                int totalChunks = (int)Math.Ceiling((double)fileLength / chunkSize);

                using (var client = new HttpClient { Timeout = TimeSpan.FromMinutes(15) }) // Set timeout to 15 minutes
                {
                    for (int i = 0; i < totalChunks; i++)
                    {
                        int currentChunkSize = chunkSize;

                        if (i * chunkSize + chunkSize > fileLength)
                        {
                            currentChunkSize = (int)(fileLength - i * chunkSize);
                        }

                        byte[] buffer = new byte[currentChunkSize];
                        using (var stream = file.OpenReadStream())
                        {
                            stream.Seek(i * (long)chunkSize, SeekOrigin.Begin); // Ensure Seek uses long
                            int bytesRead = await stream.ReadAsync(buffer, 0, currentChunkSize);

                            if (bytesRead < buffer.Length)
                            {
                                Array.Resize(ref buffer, bytesRead);
                            }

                            string base64Chunk = Convert.ToBase64String(buffer);

                            var payload = new
                            {
                                fileData = base64Chunk,
                                chunkIndex = i,
                                totalChunks = totalChunks,
                                numImages = numImages,
                                fs = fs,
                                time = time
                            };

                            var response = await client.PostAsJsonAsync("http://127.0.0.1:5000/upload", payload);
                            response.EnsureSuccessStatusCode();
                        }
                    }

                    var processPayload = new
                    {
                        numImages = numImages,
                        fs = fs,
                        time = time
                    };

                    var finalResponse = await client.PostAsJsonAsync("http://127.0.0.1:5000/upload/completed", processPayload);
                    finalResponse.EnsureSuccessStatusCode();

                    var responseData = JsonConvert.DeserializeObject<dynamic>(await finalResponse.Content.ReadAsStringAsync());

                    var predictionPayload = new { };
                    var predictionResponse = await client.PostAsJsonAsync("http://127.0.0.1:5000/predict", predictionPayload);
                    predictionResponse.EnsureSuccessStatusCode();

                    var predictionsJson = await predictionResponse.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Predictions JSON: {predictionsJson}");

                    var predictions = JsonConvert.DeserializeObject<List<PredictionResult>>(predictionsJson);

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var signalData = new SignalData
                        {
                            FileName = fileName,
                            Timestamp = DateTime.UtcNow,
                            Spectrograms = new List<Spectrogram>()
                        };

                        foreach (var prediction in predictions)
                        {

                            signalData.Spectrograms.Add(new Spectrogram
                            {
                                ImageName = prediction.Image,
                                DataBase64 = prediction.Base64,
                                Class = prediction.Class
                            });
                        }

                        context.SignalDatas.Add(signalData);
                        await context.SaveChangesAsync();

                        return new
                        {
                            message = "File processed and saved to database",
                            signalDataId = signalData.Id // Trả về SignalDataId
                        };
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    public class PredictionResult
    {
        public string Image { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
    }
}
