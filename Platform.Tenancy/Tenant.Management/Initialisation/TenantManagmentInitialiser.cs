using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Platform.Tenant.Management.Initialisation
{
	public class TenantManagmentInitialiser : IInitialiser
    {
        private const string TenantCatalogKey = "Tenant_Catalog_Context";
        
        public Type Type => throw new NotImplementedException();

        public bool AutoDiscoverable => throw new NotImplementedException();

        public byte Order => throw new NotImplementedException();

        

        public Task Initialise(IDependencyResolver dependencyResolver)
        {

            var models = ReflectionHelper.GetAllTypes(t => !t.IsInterface && !t.IsAbstract && typeof(BaseModel).IsAssignableFrom(t));
            TenantManager.TenantResolvers = () => dependencyResolver.ResolveAll<ITenantResolver<TenantId<Guid>>>();
            dependencyResolver.RegisterType<TenantManager>(Lifetime.Transient);
            dependencyResolver.RegisterType<TenantFilterBuilder>(Lifetime.Transient);
            dependencyResolver.RegisterFactory<Func<NameValueCollection>>(() =>
            () =>
            {
                //return this.ResolveMasterConnectionString();
                var tenantManger = dependencyResolver.Resolve<ITenantManager>();
                return tenantManger.ResolveTenantConnectionString();
            }
            , Lifetime.Transient);
            dependencyResolver.RegisterFactory<Func<ICatalogContext>>(() =>
            {
                return () =>
                {
                    var seeders = dependencyResolver.ResolveAll<ISeeder>();
                    var builder = dependencyResolver.Resolve<ITenantManager>();
                    IEnumerable<IDbMapper> MapperFactory() => dependencyResolver.ResolveAll<IDbMapper>();
                    var configuration = new DbCustomConfiguration(() => models, MapperFactory, TenantManagmentInitialiser.TenantCatalogKey);
                    seeders.Aggregate(configuration.Seeders, (c, next) => { c.Add(next); return c; });
                    var context = new ConvesysDbContext(null, configuration);
                    return context;
                };
            }, Lifetime.Singleton);
            return Task.CompletedTask;
        }

        //private NameValueCollection ResolveMasterConnectionString()
        //{
        //    var nameCollection = new NameValueCollection
        //    {
        //        { "DataSource","GW-WS-00383\\SQLEXPRESS"},
        //        { "DataBase","TenantCatalog"},
        //        { "UserName", "sa" },
        //        { "Password", "Gla$$wall1234"}
        //    };
        //    return nameCollection;
        //}
    }
}