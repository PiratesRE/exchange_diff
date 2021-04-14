using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADGlobalConfigSettings
	{
		internal static bool WriteOriginatingChangeTimestamp
		{
			get
			{
				return ADGlobalConfigSettings.writeOriginatingChangeTimestamp;
			}
			set
			{
				ADGlobalConfigSettings.writeOriginatingChangeTimestamp = value;
			}
		}

		internal static bool WriteShadowProperties
		{
			get
			{
				return ADGlobalConfigSettings.writeShadowProperties;
			}
			set
			{
				ADGlobalConfigSettings.writeShadowProperties = value;
			}
		}

		internal static bool SoftLinkEnabled
		{
			get
			{
				return ADGlobalConfigSettings.softLinkEnabled;
			}
			set
			{
				ADGlobalConfigSettings.softLinkEnabled = value;
			}
		}

		private static bool writeOriginatingChangeTimestamp = Datacenter.IsMicrosoftHostedOnly(true);

		private static bool writeShadowProperties = Datacenter.IsMicrosoftHostedOnly(true);

		private static bool softLinkEnabled = Datacenter.IsMicrosoftHostedOnly(true);
	}
}
