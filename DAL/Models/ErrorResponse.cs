﻿namespace DAL.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public string Path { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
