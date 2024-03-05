﻿using System.Text.Json.Serialization;

namespace HomeBanking.DTOs
{
    public class TransactionDTO
    {
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
