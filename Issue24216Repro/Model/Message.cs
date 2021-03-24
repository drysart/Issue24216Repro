using System;

namespace Issue24216Repro.Model
{
    public class Message
    {
        public long Id { get; set; }

        public long PersonId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
