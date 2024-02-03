﻿namespace api.Models
{
    public class RequestOrder
    {
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public int? Freight { get; set; }

    }
}
