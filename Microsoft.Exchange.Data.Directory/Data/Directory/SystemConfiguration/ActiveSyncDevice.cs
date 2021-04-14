using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ActiveSyncDevice : MobileDevice
	{
		public ActiveSyncDevice()
		{
			base.ClientType = MobileClientType.EAS;
		}

		public ActiveSyncDevice(MobileDevice mobileDevice)
		{
			if (mobileDevice.ClientType != MobileClientType.EAS)
			{
				throw new ArgumentException("mobileDevice's ClientType is not EAS.");
			}
			this.propertyBag = mobileDevice.propertyBag;
		}

		public string DeviceActiveSyncVersion
		{
			get
			{
				return base.ClientVersion;
			}
			internal set
			{
				base.ClientVersion = value;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new AndFilter(new QueryFilter[]
				{
					base.ImplicitFilter,
					new ComparisonFilter(ComparisonOperator.Equal, MobileDeviceSchema.ClientType, MobileClientType.EAS)
				});
			}
		}

		private new string ClientVersion { get; set; }
	}
}
