using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.VariantConfiguration.DataLoad
{
	internal class ResourcesDataSourceReader : IDataSourceReader
	{
		internal ResourcesDataSourceReader(Assembly resourcesAssembly)
		{
			if (resourcesAssembly == null)
			{
				throw new ArgumentNullException("resourcesAssembly");
			}
			this.resourcesAssembly = resourcesAssembly;
			this.ResourceNames = resourcesAssembly.GetManifestResourceNames();
		}

		internal IEnumerable<string> ResourceNames { get; private set; }

		public Func<TextReader> GetContentReader(string dataSource)
		{
			if (string.IsNullOrEmpty(dataSource))
			{
				throw new ArgumentNullException("dataSource");
			}
			string adjustedDataSource = this.ResourceNames.FirstOrDefault((string name) => string.Equals(name, dataSource, StringComparison.OrdinalIgnoreCase));
			if (string.IsNullOrEmpty(adjustedDataSource))
			{
				throw new ArgumentException(string.Format("Data source '{0}' could not be read from resources", dataSource));
			}
			return () => new StreamReader(this.resourcesAssembly.GetManifestResourceStream(adjustedDataSource));
		}

		public bool CanGetContentReader(string dataSource)
		{
			return this.ResourceNames.Contains(dataSource, StringComparer.OrdinalIgnoreCase);
		}

		private readonly Assembly resourcesAssembly;
	}
}
