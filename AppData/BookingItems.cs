namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class BookingItems
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }

        public virtual Bookings Bookings { get; set; }
    }
}
