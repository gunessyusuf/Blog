using AppCore.DataAccess.EntityFramework.Bases;
using AppCore.Records.Bases;

namespace DataAccess;

public class Repo<TEntity> : RepoBase<TEntity> where TEntity : RecordBase, new()
{
    public Repo(Db dbContext) : base(dbContext)
    {
    }
}
