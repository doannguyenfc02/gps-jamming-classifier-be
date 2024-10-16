﻿namespace gps_jamming_classifier_be.Models
{
    public class SignalData
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public ICollection<Spectrogram> Spectrograms { get; set; } = new List<Spectrogram>();
    }
}
