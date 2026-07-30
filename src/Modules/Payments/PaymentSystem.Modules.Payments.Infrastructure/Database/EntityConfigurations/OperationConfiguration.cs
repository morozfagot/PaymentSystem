using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentSystem.Modules.Payments.Domain.Operations;

namespace PaymentSystem.Modules.Payments.Infrastructure.Database.EntityConfigurations;

/// <summary>
/// EF Core конфигурация для агрегата <see cref="Operation"/>.
/// OperationId — string PK.
/// OperationTransition — owned value object (OwnsMany).
/// </summary>
internal sealed class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        // === Operation (Aggregate Root) ===

        // PK — бизнес-идентификатор
        builder.HasKey(o => o.OperationId);
        builder.Property(o => o.OperationId)
            .HasMaxLength(100)
            .ValueGeneratedNever();

        // Amount — decimal с точностью 2 знака
        builder.Property(o => o.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(500);

        // Status — enum как string
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.ProviderPaymentId)
            .HasMaxLength(100);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired();

        // Игнорируем технический Id из Entity base class
        builder.Ignore(o => o.Id);
        builder.Ignore(o => o.DomainEvents);

        // === Owned collection: OperationTransition (Value Object) ===
        builder.OwnsMany(o => o.Transitions, transitionBuilder =>
        {
            // FK на Operation (автоматически)
            transitionBuilder.WithOwner()
                .HasForeignKey("OperationId");

            // EventId — auto-increment PK внутри owned-таблицы
            transitionBuilder.Property(t => t.EventId)
                .ValueGeneratedOnAdd();

            transitionBuilder.HasKey("EventId", "OperationId");

            transitionBuilder.Property(t => t.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            transitionBuilder.Property(t => t.FromStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            transitionBuilder.Property(t => t.ToStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            transitionBuilder.Property(t => t.Message)
                .HasMaxLength(500)
                .IsRequired();

            transitionBuilder.Property(t => t.OccurredAt)
                .IsRequired();

            transitionBuilder.Property(t => t.StateChanged)
                .IsRequired();
        });

        // Navigation для Transitions использует backing field _transitions
        builder.Navigation(o => o.Transitions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}