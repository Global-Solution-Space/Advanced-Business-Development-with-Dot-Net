namespace TerraNova.Application.Repositories;

/// <summary>
/// Abstração de Unit of Work: garante que múltiplas operações de repositório
/// sejam commitadas em uma única transação atômica, preservando os princípios
/// da Clean Architecture (Application não depende de Infrastructure).
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Executa <paramref name="op"/> dentro de uma transação.
    /// Se algo dentro de <paramref name="op"/> falhar, toda a transação sofre rollback.
    /// </summary>
    void ExecuteInTransaction(Action op);

    /// <summary>
    /// Versão assíncrona de <see cref="ExecuteInTransaction(Action)"/>.
    /// </summary>
    Task ExecuteInTransactionAsync(Func<Task> op, CancellationToken ct = default);
}
