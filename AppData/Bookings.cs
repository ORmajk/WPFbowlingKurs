namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class Bookings
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bookings()
        {
            this.BookingItems = new HashSet<BookingItems>();
        }

        public int Id { get; set; }
        public string BookingNumber { get; set; }
        public int ClientId { get; set; }
        public int LaneId { get; set; }
        public System.DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingItems> BookingItems { get; set; }
        public virtual Clients Clients { get; set; }
        public virtual Lanes Lanes { get; set; }
    }
}
