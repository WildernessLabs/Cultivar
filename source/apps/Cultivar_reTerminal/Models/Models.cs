﻿using System;
using System.Collections.Generic;

namespace Cultivar_reTerminal.Models
{
    public class MeasurementData
    {
        public double TemperatureCelsius { get; set; }
        public double HumidityPercentage { get; set; }
        public double PressureMillibar { get; set; }
    }

    public class Record
    {
        public int eventId { get; set; }
        public string description { get; set; }
        public DateTime receivedAt { get; set; }
        public string deviceId { get; set; }
        public string orgId { get; set; }
        public MeasurementData measurements { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class QueryResponse
    {
        public string id { get; set; }
        public Record record { get; set; }
    }

    public class Data
    {
        public int totalRecords { get; set; }
        public int queryTimeInMilliseconds { get; set; }
        public List<QueryResponse> queryResponses { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public bool isSuccessful { get; set; }
        public object errorMessage { get; set; }
    }
}