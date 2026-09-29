namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class LaneStatuses
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LaneStatuses()
        {
            this.Lanes = new HashSet<Lanes>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lanes> Lanes { get; set; }
    }
}
