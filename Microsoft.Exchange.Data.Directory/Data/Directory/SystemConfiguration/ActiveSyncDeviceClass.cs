using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ActiveSyncDeviceClass : ADConfigurationObject
	{
		internal static ActiveSyncDeviceClassSchema StaticSchema
		{
			get
			{
				return ActiveSyncDeviceClass.schema;
			}
		}

		public string DeviceType
		{
			get
			{
				return (string)this[ActiveSyncDeviceClassSchema.DeviceType];
			}
			set
			{
				this[ActiveSyncDeviceClassSchema.DeviceType] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceType, value);
			}
		}

		public string DeviceModel
		{
			get
			{
				return (string)this[ActiveSyncDeviceClassSchema.DeviceModel];
			}
			set
			{
				this[ActiveSyncDeviceClassSchema.DeviceModel] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceModel, value);
			}
		}

		public DateTime? LastUpdateTime
		{
			get
			{
				DateTime? result = this[ActiveSyncDeviceClassSchema.LastUpdateTime] as DateTime?;
				if (result != null)
				{
					return new DateTime?(result.Value.ToUniversalTime());
				}
				return result;
			}
			set
			{
				this[ActiveSyncDeviceClassSchema.LastUpdateTime] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ActiveSyncDeviceClass.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchActiveSyncDevice";
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return ActiveSyncDeviceClass.parentPath;
			}
		}

		internal static string GetCommonName(string deviceType, string deviceModel)
		{
			return deviceType + '§' + deviceModel;
		}

		internal string GetCommonName()
		{
			return ActiveSyncDeviceClass.GetCommonName(this.DeviceType, this.DeviceModel);
		}

		public const char SeparatorChar = '§';

		private const string MostDerivedClass = "msExchActiveSyncDevice";

		private const string RelativeParentPath = "CN=ExchangeDeviceClasses,CN=Mobile Mailbox Settings";

		private static ADObjectId parentPath = new ADObjectId("CN=ExchangeDeviceClasses,CN=Mobile Mailbox Settings");

		private static ActiveSyncDeviceClassSchema schema = ObjectSchema.GetInstance<ActiveSyncDeviceClassSchema>();
	}
}
