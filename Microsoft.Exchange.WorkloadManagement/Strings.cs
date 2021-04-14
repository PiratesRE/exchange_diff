using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal static class Strings
	{
		public static LocalizedString NonOperationalAdmissionControl(ResourceKey resource)
		{
			return new LocalizedString("NonOperationalAdmissionControl", Strings.ResourceManager, new object[]
			{
				resource
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.WorkloadManagement.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			NonOperationalAdmissionControl
		}
	}
}
