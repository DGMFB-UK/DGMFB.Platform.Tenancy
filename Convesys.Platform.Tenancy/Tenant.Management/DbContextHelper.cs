using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Platform.Tenant.Management
{
	internal class DbContextHelper
    {
        public const string TenantId = "TenantId";
        private static ConcurrentDictionary<Type, Action<object, object>> _delegateCache = new ConcurrentDictionary<Type, Action<object, object>>();
        internal static IQueryable<T> BuildFilterQuery<T>(IQueryable<T> query, Guid id)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            var pe = Expression.Parameter(typeof(T));
            var pinfo = typeof(T).GetProperty(DbContextHelper.TenantId);
            if (pinfo == null)
                throw new MissingMemberException(typeof(T).FullName, DbContextHelper.TenantId);
            var left = Expression.Property(pe, pinfo);
            var right = Expression.Constant(id, typeof(Guid));
            var predicateBody = Expression.Equal(left, right);
            
            var whereCallExpression = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Where),
                new Type[] { query.ElementType },
                query.Expression,
                Expression.Lambda<Func<T, bool>>(predicateBody, new ParameterExpression[] { pe }));
            return query.Provider.CreateQuery<T>(whereCallExpression);
        }

        internal static Action<object, object> GetAssignDelegate(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            var del = DbContextHelper._delegateCache.GetOrAdd(type, t => TypeExtensions.GetAssignPropertyDelegate(t, DbContextHelper.TenantId));
            return del;
        }
    }
}