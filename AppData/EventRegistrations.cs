namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class EventRegistrations
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EventId { get; set; }

        public virtual Clients Clients { get; set; }
        public virtual Events Events { get; set; }
    }
}
