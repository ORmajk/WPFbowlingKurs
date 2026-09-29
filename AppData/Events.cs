namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class Events
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Events()
        {
            this.EventRegistrations = new HashSet<EventRegistrations>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime EventDate { get; set; }
        public string Description { get; set; }
        public Nullable<int> LaneId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventRegistrations> EventRegistrations { get; set; }
        public virtual Lanes Lanes { get; set; }
    }
}
