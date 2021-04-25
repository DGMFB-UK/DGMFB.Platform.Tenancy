using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Platform.Tenant.Management
{
	internal class TenantManager : ITenantManager
    {
        public static Func<IEnumerable<ITenantResolver<TenantId<Guid>>>> TenantResolvers;
        
        private readonly ITenantFilterBuilder _tenantFilterBuilder;
        private readonly Func<ICatalogContext> _catalogFactory;
        public TenantManager(ITenantFilterBuilder tenantFilterBuilder, Func<ICatalogContext> catalogFactory)
        {
            this._tenantFilterBuilder = tenantFilterBuilder;
            this._catalogFactory = catalogFactory;
        }

        public string TenantConnectionString => throw new NotImplementedException();

        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query) where T : BaseTenantModel
        {
            var tenant = this.ResolveTenant();
            return this._tenantFilterBuilder.ApplyFilter(query, tenant);
        }

        public T AssignTenantId<T>(T item) where T : BaseTenantModel
        {
            var tenant = this.ResolveTenant();
            return this._tenantFilterBuilder.AssignTenantId(item, tenant);
        }

        public Guid ResolveTenant()
        {
            var resolvers = TenantManager.TenantResolvers();
            var seed = new Func<TenantId<Guid>>(() => null);
            var del = resolvers.Aggregate(seed, (x, next) => new Func<TenantId<Guid>>(() => next.ResolveTenant(x)));
            var tenant = del();
            if (tenant == null)
                throw new InvalidOperationException("Cannot resolver the tenant Id");
            return tenant.Id;
        }

        public NameValueCollection ResolveTenantConnectionString()
        {
            var nameCollection = new NameValueCollection
            {
                { "DataSource", @"localhost\SQLEXPRESS"},
                { "DataBase","HubDatabase"},
                { "UserName", "ManagementService"},
                { "Password", "password1!"}
            };
            return nameCollection;
        }
    }
}