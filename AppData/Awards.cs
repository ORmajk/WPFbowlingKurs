namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class Awards
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Awards()
        {
            this.UserAwards = new HashSet<UserAwards>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAwards> UserAwards { get; set; }
    }
}
