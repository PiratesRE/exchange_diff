using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class EmailAddressPolicyContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return EmailAddressPolicyContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return EmailAddressPolicyContainer.mostDerivedClass;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = SystemFlagsEnum.None;
			}
			base.StampPersistableDefaultValues();
		}

		public const string DefaultName = "Recipient Policies";

		private static EmailAddressPolicyContainerSchema schema = ObjectSchema.GetInstance<EmailAddressPolicyContainerSchema>();

		private static string mostDerivedClass = "msExchRecipientPolicyContainer";
	}
}
