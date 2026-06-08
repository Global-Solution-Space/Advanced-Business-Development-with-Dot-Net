using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Common;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public class Repository<T>(TerraNovaContext context) : IRepository<T> where T : BaseEntity
{
    protected TerraNovaContext Context { get; } = context;
 
    private readonly DbSet<T> _set = context.Set<T>();
 
    private const string PropriedadeNome = "Nome";
 
    public virtual IReadOnlyList<T> GetAll() =>
        _set.AsNoTracking().OrderBy(e => e.Id).ToList();
 
    public virtual T? GetById(Guid id) =>
        _set.AsNoTracking().FirstOrDefault(e => e.Id == id);
 
    public T Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _set.Add(entity);
        Context.SaveChanges();
        return entity;
    }

    /// <summary>
    /// Adiciona a entidade ao contexto SEM chamar SaveChanges.
    /// Use quando quiser empilhar várias operações em uma única transação.
    /// </summary>
    public void AddNoSave(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _set.Add(entity);
    }

    public T Update(Guid id, T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        MarcarComoModificado(id, entity);
        Context.SaveChanges();
        return entity;
    }

    /// <summary>
    /// Marca a entidade como modificada SEM chamar SaveChanges.
    /// Use quando quiser empilhar várias operações em uma única transação.
    /// </summary>
    public void UpdateNoSave(Guid id, T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        MarcarComoModificado(id, entity);
    }

    private void MarcarComoModificado(Guid id, T entity)
    {
        // Garante que a entidade carrega o Id da rota
        if (entity.Id != id)
            Context.Entry(entity).Property(e => e.Id).CurrentValue = id;

        // Anexa a entidade sem marcar todas as colunas como Modified.
        // Vamos marcar apenas as propriedades escalares não-chave como Modified
        // para preservar colunas de auditoria (DataCriacao, RowVersion, etc.)
        // e evitar UPDATE desnecessário em FKs inalteradas.
        if (Context.Entry(entity).State == EntityState.Detached)
            _set.Attach(entity);

        var entry = Context.Entry(entity);

        // Marca apenas as propriedades "normais" (exclui chave primária e navegações)
        // como modificadas — o EF gera um UPDATE focado nos campos realmente alterados.
        foreach (var property in entry.Properties)
        {
            if (property.Metadata.IsPrimaryKey()) continue;
            property.IsModified = true;
        }
    }
 
    public bool Delete(Guid id)
    {
        var entity = GetById(id);
        if (entity is null) return false;
        _set.Remove(entity);
        Context.SaveChanges();
        return true;
    }
 
    public bool ExistsById(Guid id) =>
        _set.Count(e => e.Id == id) > 0;
 
    public bool ExistsByNome(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
 
        ValidarPropriedadeNome();
 
        var normalizado = valor.Trim().ToLowerInvariant();
        return _set.AsNoTracking().Count(e => EF.Property<string>(e, PropriedadeNome).ToLower() == normalizado) > 0;
    }
 
    private void ValidarPropriedadeNome()
    {
        var entityType = Context.Model.FindEntityType(typeof(T));
 
        if (entityType is null)
            throw new InvalidOperationException(
                $"ExistsByNome: o tipo '{typeof(T).Name}' não está registrado no modelo EF Core.");
 
        var prop = entityType.FindProperty(PropriedadeNome);
 
        if (prop is null || prop.ClrType != typeof(string))
            throw new InvalidOperationException(
                $"ExistsByNome: a entidade '{typeof(T).Name}' não possui a propriedade " +
                $"'{PropriedadeNome}' (string) mapeada.");
    }
}