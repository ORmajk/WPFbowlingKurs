namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Collections.Generic;

    public partial class UserActions
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ActionType { get; set; }
        public string Details { get; set; }
        public System.DateTime Timestamp { get; set; }

        public virtual Users Users { get; set; }
    }
}
