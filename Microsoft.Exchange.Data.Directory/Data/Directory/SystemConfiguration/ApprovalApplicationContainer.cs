using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ApprovalApplicationContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ApprovalApplicationContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ApprovalApplicationContainer.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal ADObjectId RetentionPolicy
		{
			get
			{
				return (ADObjectId)this[ApprovalApplicationContainerSchema.RetentionPolicy];
			}
			set
			{
				this[ApprovalApplicationContainerSchema.RetentionPolicy] = value;
			}
		}

		private static ApprovalApplicationContainerSchema schema = ObjectSchema.GetInstance<ApprovalApplicationContainerSchema>();

		private static string mostDerivedClass = "msExchApprovalApplicationContainer";

		public static readonly string DefaultName = "Approval Applications";
	}
}
