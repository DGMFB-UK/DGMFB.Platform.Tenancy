using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Platform.Platform.Tenancy.Convesys.Tenant.Management
{
	public class DbCustomConfiguration : IDbCustomConfiguration
    {
        public DbCustomConfiguration(Func<IEnumerable<Type>> modelsFactory, Func<IEnumerable<IDbMapper>> mapperFactory, string modelKey)
        {
            this.ModelsFactory = modelsFactory;
            this.Seeders = new List<ISeeder>();
            this.ModelMappers = mapperFactory;
            this.ModelKey = modelKey;
        }
        public ICollection<ISeeder> Seeders { get; }

        public Func<IEnumerable<Type>> ModelsFactory { get; }

        public string Schema => throw new NotImplementedException();

        public string ModelKey { get; }

        public Func<IEnumerable<IDbMapper>> ModelMappers { get; private set; }

        public Func<ITenantManager> TenantManager => throw new NotImplementedException();

        public Task ConfigureOptions<T>(T options)
        {
            throw new NotImplementedException();
        }
    }
}
