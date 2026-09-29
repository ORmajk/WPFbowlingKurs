namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class InventoryStatuses
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InventoryStatuses()
        {
            this.InventoryItems = new HashSet<InventoryItems>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryItems> InventoryItems { get; set; }
    }
}
