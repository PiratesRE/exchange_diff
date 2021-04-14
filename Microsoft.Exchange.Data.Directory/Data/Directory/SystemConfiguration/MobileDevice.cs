using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MobileDevice : ADConfigurationObject
	{
		public string FriendlyName
		{
			get
			{
				return (string)this[MobileDeviceSchema.FriendlyName];
			}
			internal set
			{
				this[MobileDeviceSchema.FriendlyName] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.FriendlyName, value);
			}
		}

		public string DeviceId
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceId];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceId] = value;
			}
		}

		public string DeviceImei
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceImei];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceImei] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceImei, value);
			}
		}

		public string DeviceMobileOperator
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceMobileOperator];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceMobileOperator] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceMobileOperator, value);
			}
		}

		public string DeviceOS
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceOS];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceOS] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceOS, value);
			}
		}

		public string DeviceOSLanguage
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceOSLanguage];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceOSLanguage] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceOSLanguage, value);
			}
		}

		public string DeviceTelephoneNumber
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceTelephoneNumber];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceTelephoneNumber] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceTelephoneNumber, value);
			}
		}

		public string DeviceType
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceType];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceType] = value;
			}
		}

		public string DeviceUserAgent
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceUserAgent];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceUserAgent] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceUserAgent, value);
			}
		}

		public string DeviceModel
		{
			get
			{
				return (string)this[MobileDeviceSchema.DeviceModel];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceModel] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.DeviceModel, value);
			}
		}

		public DateTime? FirstSyncTime
		{
			get
			{
				DateTime? result = this[MobileDeviceSchema.FirstSyncTime] as DateTime?;
				if (result != null)
				{
					return new DateTime?(result.Value.ToUniversalTime());
				}
				return result;
			}
			internal set
			{
				this[MobileDeviceSchema.FirstSyncTime] = value;
			}
		}

		public string UserDisplayName
		{
			get
			{
				return (string)this[MobileDeviceSchema.UserDisplayName];
			}
			internal set
			{
				this[MobileDeviceSchema.UserDisplayName] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.UserDisplayName, value);
			}
		}

		public DeviceAccessState DeviceAccessState
		{
			get
			{
				return (DeviceAccessState)this[MobileDeviceSchema.DeviceAccessState];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceAccessState] = value;
			}
		}

		public DeviceAccessStateReason DeviceAccessStateReason
		{
			get
			{
				return (DeviceAccessStateReason)this[MobileDeviceSchema.DeviceAccessStateReason];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceAccessStateReason] = value;
			}
		}

		public ADObjectId DeviceAccessControlRule
		{
			get
			{
				return (ADObjectId)this[MobileDeviceSchema.DeviceAccessControlRule];
			}
			internal set
			{
				this[MobileDeviceSchema.DeviceAccessControlRule] = value;
			}
		}

		public string ClientVersion
		{
			get
			{
				return (string)this[MobileDeviceSchema.ClientVersion];
			}
			internal set
			{
				this[MobileDeviceSchema.ClientVersion] = MobileDevice.TrimStringValue(this.Schema, MobileDeviceSchema.ClientVersion, value);
			}
		}

		public MobileClientType ClientType
		{
			get
			{
				return (MobileClientType)this[MobileDeviceSchema.ClientType];
			}
			internal set
			{
				this[MobileDeviceSchema.ClientType] = value;
			}
		}

		public bool IsManaged
		{
			get
			{
				return (bool)this[MobileDeviceSchema.IsManaged];
			}
			internal set
			{
				this[MobileDeviceSchema.IsManaged] = value;
			}
		}

		public bool IsCompliant
		{
			get
			{
				return (bool)this[MobileDeviceSchema.IsCompliant];
			}
			internal set
			{
				this[MobileDeviceSchema.IsCompliant] = value;
			}
		}

		public bool IsDisabled
		{
			get
			{
				return (bool)this[MobileDeviceSchema.IsDisabled];
			}
			internal set
			{
				this[MobileDeviceSchema.IsDisabled] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MobileDevice.schema;
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
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal static ADObjectId GetRootId(ADUser adUser)
		{
			return adUser.Id.GetChildId("ExchangeActiveSyncDevices");
		}

		internal static ADObjectId GetRootId(ADObjectId adUserId)
		{
			return adUserId.GetChildId("ExchangeActiveSyncDevices");
		}

		internal static int GetStringConstraintLength(ADObjectSchema schema, ADPropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			schema.InitializeAutogeneratedConstraints();
			int num = int.MaxValue;
			foreach (PropertyDefinitionConstraint propertyDefinitionConstraint in propertyDefinition.AllConstraints)
			{
				StringLengthConstraint stringLengthConstraint = propertyDefinitionConstraint as StringLengthConstraint;
				if (stringLengthConstraint != null && stringLengthConstraint.MaxLength < num)
				{
					num = stringLengthConstraint.MaxLength;
				}
			}
			return num;
		}

		internal static string TrimStringValue(ADObjectSchema schema, ADPropertyDefinition propertyDefinition, string value)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			int stringConstraintLength = MobileDevice.GetStringConstraintLength(schema, propertyDefinition);
			if (value.Length > stringConstraintLength)
			{
				return value.Remove(stringConstraintLength);
			}
			return value;
		}

		internal const string MostDerivedClass = "msExchActiveSyncDevice";

		private static MobileDeviceSchema schema = ObjectSchema.GetInstance<MobileDeviceSchema>();
	}
}
