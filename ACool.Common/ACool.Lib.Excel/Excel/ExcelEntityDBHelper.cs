using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class ExcelEntityDataBase : ExcelEntityDBHelper
    {
        public ExcelEntityDataBase(string filepath) : base(filepath)
        { }
        public override T Insert<T>(T entity)
        {
            if (entity is BaseExcelEntity)
            {
                BaseExcelEntity excelEntity = entity as BaseExcelEntity;

                excelEntity.Id = Guid.NewGuid();
                excelEntity.CreateTime = DateTime.UtcNow;
                excelEntity.UpdateTime = excelEntity.CreateTime;
                excelEntity.isDelete = false;
            }

            return base.Insert<T>(entity);
        }
        public override T Update<T>(T entity)
        {
            if (entity is BaseExcelEntity)
            {
                BaseExcelEntity excelEntity = entity as BaseExcelEntity;

                excelEntity.UpdateTime = DateTime.UtcNow;
            }

            return base.Update<T>(entity);
        }
        public override void Delete<T>(T entity)
        {
            if (entity is BaseExcelEntity)
            {
                BaseExcelEntity excelEntity = entity as BaseExcelEntity;

                excelEntity.UpdateTime = DateTime.UtcNow;
                excelEntity.isDelete = true;
            }

            base.Update<T>(entity);
        }
        public override void Delete<T>(Guid id)
        {
            T entity = Get<T>(id);

            this.Delete<T>(entity);
        }
        public override  List<T> QueryAll<T>()
        {
            List<T> entities = base.QueryAll<T>();

            if (typeof(T).IsSubclassOf(typeof(BaseExcelEntity)))
            {
                return entities.Where(x => !(x as BaseExcelEntity).isDelete).ToList();
            }
            else
            {
                return entities;
            }
        }
        public override T Get<T>(Guid id)
        {
            T entity = base.Get<T>(id);

            if (entity is BaseExcelEntity && (entity as BaseExcelEntity).isDelete)
            {
                return default(T);
            }
            else
            {
                return entity;
            }
        }
    }
}
