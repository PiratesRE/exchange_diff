using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	internal class ADUserSetting : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return base.Id;
			}
		}

		public ObjectId ConfigurationId
		{
			get
			{
				return (ObjectId)this[ADUserSettingSchema.ConfigurationIdProp];
			}
			set
			{
				this[ADUserSettingSchema.ConfigurationIdProp] = value;
			}
		}

		public UserSettingFlags Flags
		{
			get
			{
				return (UserSettingFlags)this[ADUserSettingSchema.FlagsProp];
			}
			set
			{
				this[ADUserSettingSchema.FlagsProp] = value;
			}
		}

		public byte[] SafeSenders
		{
			get
			{
				return (byte[])this[ADUserSettingSchema.SafeSendersProp];
			}
			set
			{
				this[ADUserSettingSchema.SafeSendersProp] = value;
			}
		}

		public byte[] BlockedSenders
		{
			get
			{
				return (byte[])this[ADUserSettingSchema.BlockedSendersProp];
			}
			set
			{
				this[ADUserSettingSchema.BlockedSendersProp] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ADUserSettingSchema.DisplayNameProp];
			}
			set
			{
				this[ADUserSettingSchema.DisplayNameProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADUserSetting.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADUserSetting.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly ADUserSettingSchema schema = ObjectSchema.GetInstance<ADUserSettingSchema>();

		private static string mostDerivedClass = "ADUserSetting";
	}
}
