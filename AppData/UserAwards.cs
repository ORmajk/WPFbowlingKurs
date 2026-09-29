namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class UserAwards
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AwardId { get; set; }
        public System.DateTime AwardedAt { get; set; }

        public virtual Awards Awards { get; set; }
        public virtual Users Users { get; set; }
    }
}
