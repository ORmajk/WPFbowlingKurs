namespace WPFbowlingKurs.AppData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class BowlingClubEntities : DbContext
    {
        public BowlingClubEntities()
            : base("name=BowlingClubEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Awards> Awards { get; set; }
        public virtual DbSet<BookingItems> BookingItems { get; set; }
        public virtual DbSet<Bookings> Bookings { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<EventRegistrations> EventRegistrations { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<InventoryItems> InventoryItems { get; set; }
        public virtual DbSet<InventoryStatuses> InventoryStatuses { get; set; }
        public virtual DbSet<Lanes> Lanes { get; set; }
        public virtual DbSet<LaneStatuses> LaneStatuses { get; set; }
        public virtual DbSet<LaneTypes> LaneTypes { get; set; }
        public virtual DbSet<UserActions> UserActions { get; set; }
        public virtual DbSet<UserAwards> UserAwards { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}
