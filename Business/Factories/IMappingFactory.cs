using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories
{
    public interface IMappingFactory<TEntity, TModel>
    {
        TModel MapToModel(TEntity entity);
        TEntity MapToEntity(TModel model);
    }
}
