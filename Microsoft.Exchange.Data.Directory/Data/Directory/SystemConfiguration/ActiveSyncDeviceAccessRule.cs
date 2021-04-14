using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ActiveSyncDeviceAccessRule : ADConfigurationObject
	{
		public string QueryString
		{
			get
			{
				return (string)this[ActiveSyncDeviceAccessRuleSchema.QueryString];
			}
			set
			{
				this[ActiveSyncDeviceAccessRuleSchema.QueryString] = value;
			}
		}

		public DeviceAccessCharacteristic Characteristic
		{
			get
			{
				return (DeviceAccessCharacteristic)this[ActiveSyncDeviceAccessRuleSchema.Characteristic];
			}
			set
			{
				this[ActiveSyncDeviceAccessRuleSchema.Characteristic] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DeviceAccessLevel AccessLevel
		{
			get
			{
				return (DeviceAccessLevel)this[ActiveSyncDeviceAccessRuleSchema.AccessLevel];
			}
			set
			{
				this[ActiveSyncDeviceAccessRuleSchema.AccessLevel] = value;
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
				return ActiveSyncDeviceAccessRule.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchDeviceAccessRule";
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
				return ActiveSyncDeviceAccessRule.parentPath;
			}
		}

		private const string MostDerivedClass = "msExchDeviceAccessRule";

		private static ADObjectId parentPath = new ADObjectId("CN=Mobile Mailbox Settings");

		private static ActiveSyncDeviceAccessRuleSchema schema = ObjectSchema.GetInstance<ActiveSyncDeviceAccessRuleSchema>();
	}
}
