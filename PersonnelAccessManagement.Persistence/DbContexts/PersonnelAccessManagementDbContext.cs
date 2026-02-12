using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Persistence.DbContexts;

public class PersonnelAccessManagementDbContext: DbContext
{
    public PersonnelAccessManagementDbContext(DbContextOptions<PersonnelAccessManagementDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Personnel> Personnels => Set<Personnel>();
    public DbSet<Rule> Rules => Set<Rule>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventLog> EventLogs => Set<EventLog>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobLog> JobLogs => Set<JobLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonnelAccessManagementDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}