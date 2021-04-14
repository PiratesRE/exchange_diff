using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.DataProvider
{
	internal class EnvironmentStrategy
	{
		public virtual bool IsForefrontForOffice()
		{
			return DatacenterRegistry.IsForefrontForOffice();
		}

		public virtual bool IsForefrontDALOverrideUseSQL()
		{
			return Convert.ToBoolean(ConfigurationManager.AppSettings["DALUseLocalDB"]);
		}
	}
}
