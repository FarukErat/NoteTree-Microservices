namespace Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    // public Guid TenantId { get; set; }
    // public DateTime CreatedAt { get; set; }
    // public DateTime UpdatedAt { get; set; }
    // public DateTime? DeletedAt { get; set; }
}

// public class SoftDeleteInterceptor : SaveChangesInterceptor
// {
//     public override InterceptionResult<int> SavingChanges(
//         DbContextEventData eventData,
//         InterceptionResult<int> result)
//     {
//         foreach (var entry in eventData.Context.ChangeTracker.Entries())
//         {
//             if (entry.Entity is BaseEntity entity)
//             {
//                 switch (entry.State)
//                 {
//                     case EntityState.Deleted:
//                         entry.State = EntityState.Modified;
//                         entity.DeletedAt = DateTime.UtcNow;
//                         break;
//                 }
//             }
//         }

//         return base.SavingChanges(eventData, result);
//     }
// }
