using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace surgingDemo.Data.Repositories.Implementation
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private DbContext _context;
        private DbSet<T> _entities;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EFRepository(DbContext context)
        {
        }

        public EFRepository()
        {
            // this._context = CommonEntities.Instance();
        }

        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public T GetModel(Expression<Func<T, bool>> whereLambda)
        {
            return this.Entities.AsNoTracking().SingleOrDefault(whereLambda);
        }

        public IRepository<T> Instance(DbContext context)
        {
            _context = context;
            return this;
        }

        public bool Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this.Entities.Add(entity);
                return this._context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增 实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int InsertBatch(List<T> list)
        {
            try
            {

                foreach (T t in list)
                {
                    this.Entities.Add(t);
                }
                return this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Modify(T model, params string[] proNames)
        {
            int botInt = 0;
            try
            {

                //4.1将 对象 添加到 EF中
                var entry = this._context.Entry<T>(model);
                //4.2先设置 对象的包装 状态为 Unchanged
                entry.State = EntityState.Unchanged;
                //4.3循环 被修改的属性名 数组
                foreach (string proName in proNames)
                {
                    if (!string.IsNullOrEmpty(proName))
                        //4.4将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                        entry.Property(proName).IsModified = true;
                }
                botInt = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //4.4一次性 生成sql语句到数据库执行
            return botInt;
        }
        public int Modify(List<T> list, params string[] proNames)
        {
            int botInt = 0;
            try
            {

                list.ForEach(model =>
                {
                    //4.1将 对象 添加到 EF中
                    var entry = this._context.Entry<T>(model);
                    //4.2先设置 对象的包装 状态为 Unchanged
                    entry.State = EntityState.Unchanged;
                    //4.3循环 被修改的属性名 数组
                    foreach (string proName in proNames)
                    {
                        if (!string.IsNullOrEmpty(proName))
                            //4.4将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                            entry.Property(proName).IsModified = true;
                    }
                });
                botInt = this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //4.4一次性 生成sql语句到数据库执行
            return botInt;
        }

        /// <summary>
        /// 4.0 批量修改
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="proNames">要修改的 属性 名称</param>
        /// <returns></returns>
        public virtual int ModifyBy(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        {
            try
            {
                //4.1查询要修改的数据
                List<T> listModifing = this.Entities.Where(whereLambda).ToList();

                //获取 实体类 类型对象
                Type t = typeof(T); // model.GetType();
                //获取 实体类 所有的 公有属性
                List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                //创建 实体属性 字典集合
                Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
                //将 实体属性 中要修改的属性名 添加到 字典集合中 键：属性名  值：属性对象
                proInfos.ForEach(p =>
                {
                    if (modifiedProNames.Contains(p.Name))
                    {
                        dictPros.Add(p.Name, p);
                    }
                });

                //4.3循环 要修改的属性名
                foreach (string proName in modifiedProNames)
                {
                    //判断 要修改的属性名是否在 实体类的属性集合中存在
                    if (!string.IsNullOrEmpty(proName) && dictPros.ContainsKey(proName))
                    {
                        //如果存在，则取出要修改的 属性对象
                        PropertyInfo proInfo = dictPros[proName];
                        //取出 要修改的值

                        object newValue = proInfo.GetValue(model, null); //object newValue = model.uName;

                        //4.4批量设置 要修改 对象的 属性
                        foreach (T usrO in listModifing)
                        {
                            var entry = this._context.Entry<T>(usrO);
                            entry.Property(proName).IsModified = true;
                            //为 要修改的对象 的 要修改的属性 设置新的值
                            proInfo.SetValue(usrO, newValue, null); //usrO.uName = newValue;
                        }
                    }
                }
                //4.4一次性 生成sql语句到数据库执行
                return this._context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }





        /// <summary>
        /// 5.0 根据条件查询 +List<T> GetListBy(Expression<Func<T,bool>> whereLambda)
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public virtual List<T> GetListBy(Expression<Func<T, bool>> whereLambda)
        {
            return this.Entities.AsNoTracking().Where(whereLambda).ToList();
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> whereLambda)
        {
            return this.Entities.FirstOrDefault(whereLambda);
        }

        /// <summary>
        /// 5.1 根据条件 排序 和查询
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件 lambda表达式</param>
        /// <param name="orderLambda">排序条件 lambda表达式</param>
        /// <returns></returns>
        public virtual List<T> GetListBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda)
        {
            //List<int> listIds = new List<int>() { 1, 2, 3 };
            //new MODEL.OuOAEntities().Ou_UserInfo.Where(u => listIds.Contains(u.uId));
            return this.Entities.Where(whereLambda).OrderBy(orderLambda).ToList();
        }

        public List<T> SqlQuery(string sql)
        {
            return this._context.Set<T>().FromSql(sql).ToList();
        }

        /// <summary>
        /// 6.0 分页查询 + List<T> GetPagedList<TKey>
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public virtual List<T> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            return this.Entities.Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 6.1分页查询 带输出
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="rowCount"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderBy"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public virtual List<T> GetPagedList<TKey>(int pageIndex, int pageSize, ref int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, bool isAsc = true)
        {
            //查询总行数
            rowCount = this.Entities.Where(whereLambda).Count();
            //查询分页数据
            if (isAsc)
            {
                return this.Entities.OrderBy(orderBy).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return this.Entities.OrderByDescending(orderBy).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 7.0 执行sql语句 +int ExcuteSql(string strSql, params object[] paras)
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public virtual int ExcuteSql(string strSql, params object[] paras)
        {
            return this._context.Database.ExecuteSqlCommand(strSql, paras);
        }

        /// <summary>
        /// 8.0 根据条件获取一个 不被ef跟踪的 对象
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public virtual T GetModelWithOutTrace(Expression<Func<T, bool>> whereLambda)
        {
            return this._context.Set<T>().AsNoTracking().Single(whereLambda);
        }


        public virtual DbQuery<T> GetDbQuery(Expression<Func<T, bool>> whereLambda)
        {
            return this._context.Set<T>().Where(whereLambda) as DbQuery<T>;
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        private DbSet<T> Entities
        {
            get
            {
                _entities = _context.Set<T>();
                return _entities;
            }
        }
    }
}