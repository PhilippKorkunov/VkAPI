using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class VkApiContext : DbContext, ICloneable
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserGroup> UserGroups { get; set; } = null!;
        public DbSet<UserState> UserStates { get; set; } = null!;
        public DbSet<UserWithFullInfo> UsersWithFullInfos{ get; set; } = null!;

        private readonly string viewName = "View_UserFullInfo";

        public VkApiContext()
        {
            Database.EnsureCreated();
            //viewName = "View_UserFullInfo";
            Database.ExecuteSqlRaw($@"DROP VIEW IF EXISTS public.{viewName};
                                        CREATE VIEW {viewName} AS
                                        SELECT u.id AS ""Id"", u.login AS ""Login"", u.password AS ""Password"", 
                                        u.created_date AS ""Created Date"", s.code AS ""State"", g.code As ""User Group""
                                        FROM public.user as u
                                        INNER JOIN public.user_state s on u.user_state_id = s.id
                                        INNER JOIN public.user_group g on u.user_group_id = g.id
                                        Order By u.id"); // формальное создание представления с необходимыми столбцами из всех 3 таблиц
        }

        public VkApiContext(DbContextOptions<VkApiContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=VkDB;Username=postgres;Password=qwe123@");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(user => user.Login).IsUnique(); //Добавление ограничение UNIQUE на столбец с логином
            modelBuilder.Entity<UserWithFullInfo>(user =>
            {
                user.HasNoKey();
                user.ToView(viewName.ToLower());
                user.Property("CreatedDate").HasColumnName("Created Date");
                user.Property("UserStateCode").HasColumnName("State");
                user.Property("UserGroupCode").HasColumnName("User Group");
            }); // Привязка прдеставления в БД к UserWithFullInfo
        }

        public object Clone()
        {
            return new VkApiContext();
        }
    }
}
