using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.OData
{
	internal static class ODataConfig
	{
		public static void Initialize()
		{
			if (ODataConfig.ShouldEnabledOData())
			{
				HandlerInstaller.Initialize();
			}
		}

		private static bool ShouldEnabledOData()
		{
			return ODataConfig.ODataEnabled.Member;
		}

		private static LazyMember<bool> ODataEnabled = new LazyMember<bool>(() => VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Ews.OData.Enabled);
	}
}
