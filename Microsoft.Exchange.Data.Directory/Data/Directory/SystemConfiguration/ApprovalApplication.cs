using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ApprovalApplication : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ApprovalApplication.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ApprovalApplication.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return ApprovalApplication.ParentPathInternal;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal ADObjectId ELCRetentionPolicyTag
		{
			get
			{
				return (ADObjectId)this[ApprovalApplicationSchema.ELCRetentionPolicyTag];
			}
			set
			{
				this[ApprovalApplicationSchema.ELCRetentionPolicyTag] = value;
			}
		}

		private static ApprovalApplicationSchema schema = ObjectSchema.GetInstance<ApprovalApplicationSchema>();

		private static string mostDerivedClass = "msExchApprovalApplication";

		public static readonly ADObjectId ParentPathInternal = new ADObjectId("CN=Approval Applications");
	}
}
