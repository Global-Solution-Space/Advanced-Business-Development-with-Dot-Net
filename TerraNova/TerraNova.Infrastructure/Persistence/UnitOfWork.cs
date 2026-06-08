using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;

namespace TerraNova.Infrastructure.Persistence;

/// <summary>
/// Implementação de <see cref="IUnitOfWork"/> baseada no <see cref="TerraNovaContext"/>.
/// Garante que múltiplas operações de repositório executem em uma única transação atômica,
/// chamando <c>SaveChanges</c> automaticamente ao final da operação.
/// </summary>
public sealed class UnitOfWork(TerraNovaContext context) : IUnitOfWork
{
    public void ExecuteInTransaction(Action op)
    {
        ArgumentNullException.ThrowIfNull(op);

        // Se já houver uma transação em andamento (aninhamento), reaproveita.
        if (context.Database.CurrentTransaction is not null)
        {
            op();
            context.SaveChanges();
            return;
        }

        using var transaction = context.Database.BeginTransaction();
        try
        {
            op();
            context.SaveChanges();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> op, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(op);

        if (context.Database.CurrentTransaction is not null)
        {
            await op();
            await context.SaveChangesAsync(ct);
            return;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        try
        {
            await op();
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
