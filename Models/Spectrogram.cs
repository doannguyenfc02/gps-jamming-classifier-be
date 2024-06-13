namespace gps_jamming_classifier_be.Models
{
    public class Spectrogram
    {
        public int Id { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public string DataBase64 { get; set; } = string.Empty;
        public string Class { get; set; }
        public int SignalDataId { get; set; }
        public SignalData SignalData { get; set; }
    }
}
