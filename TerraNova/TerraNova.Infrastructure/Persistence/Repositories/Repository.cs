using Microsoft.EntityFrameworkCore;
using TerraNova.Application.Repositories;
using TerraNova.Domain.Common;

namespace TerraNova.Infrastructure.Persistence.Repositories;

public class Repository<T>(TerraNovaContext context) : IRepository<T> where T : BaseEntity
{
    protected TerraNovaContext Context { get; } = context;
 
    private readonly DbSet<T> _set = context.Set<T>();
 
    private const string PropriedadeNome = "Nome";
 
    public IReadOnlyList<T> GetAll() =>
        _set.AsNoTracking().OrderBy(e => e.Id).ToList();
 
    public virtual T? GetById(Guid id) =>
        _set.Find(id);
 
    public T Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _set.Add(entity);
        Context.SaveChanges();
        return entity;
    }
 
    public T Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _set.Update(entity);
        Context.SaveChanges();
        return entity;
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