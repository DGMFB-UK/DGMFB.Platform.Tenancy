using System;
using System.Linq;

namespace Platform.Tenant.Management
{
	internal class TenantFilterBuilder : ITenantFilterBuilder
    {
        public TenantFilterBuilder()
        {
        }

        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, ITenantManager manager)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            var tenantId = manager.ResolveTenant();
            return QueryHelper.BuildFilterQuery(query, tenantId);
        }

        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, Guid tenantId) where T : BaseTenantModel
        {
            throw new NotImplementedException();
        }

        public T AssignTenantId<T>(T item, ITenantManager manager)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            var tenantId = manager.ResolveTenant();
            var propertyDelegate = QueryHelper.GetAssignDelegate(typeof(T));
            propertyDelegate(item, tenantId);
            return item;
        }

        public T AssignTenantId<T>(T item, Guid tenantId) where T : BaseTenantModel
        {
            throw new NotImplementedException();
        }
    }
}