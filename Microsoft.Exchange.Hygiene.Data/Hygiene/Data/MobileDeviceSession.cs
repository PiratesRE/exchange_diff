using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class MobileDeviceSession
	{
		public MobileDeviceSession()
		{
			this.dataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Mtrt);
		}

		public IEnumerable<T> GetDashboardSummary<T>(QueryFilter filter) where T : DeviceSnapshot, new()
		{
			return this.dataProvider.Find<T>(filter, null, false, null).Cast<T>();
		}

		private readonly IConfigDataProvider dataProvider;
	}
}
