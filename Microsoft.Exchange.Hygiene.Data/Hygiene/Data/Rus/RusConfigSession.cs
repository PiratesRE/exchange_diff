using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Rus
{
	internal class RusConfigSession
	{
		public RusConfigSession()
		{
			this.DataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Directory);
		}

		private IConfigDataProvider DataProvider { get; set; }

		public RusConfig FindRusConfigProperty()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "RUS_Default");
			return this.DataProvider.Find<RusConfig>(filter, null, false, null).Cast<RusConfig>().FirstOrDefault<RusConfig>();
		}

		public void UpdateRusConfigUniversalManifestVersion(string newVersion)
		{
			this.UpdateRusConfigUM(newVersion, false);
		}

		public void UpdateRusConfigUniversalManifestVersionV2(string newVersion)
		{
			this.UpdateRusConfigUM(newVersion, true);
		}

		private void UpdateRusConfigUM(string value, bool isV2 = false)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("UMVersionValue");
			}
			RusConfig rusConfig = this.FindRusConfigProperty();
			if (rusConfig == null)
			{
				rusConfig = new RusConfig();
			}
			if (isV2)
			{
				rusConfig.UniversalManifestVersionV2 = value;
			}
			else
			{
				rusConfig.UniversalManifestVersion = value;
			}
			this.DataProvider.Save(rusConfig);
		}
	}
}
