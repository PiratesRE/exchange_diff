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
	public class PolicyTipMessageConfig : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return PolicyTipMessageConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return PolicyTipMessageConfig.mostDerivedClass;
			}
		}

		public string Locale
		{
			get
			{
				return (string)this[PolicyTipMessageConfigSchema.Locale];
			}
			set
			{
				this[PolicyTipMessageConfigSchema.Locale] = value;
			}
		}

		public PolicyTipMessageConfigAction Action
		{
			get
			{
				return (PolicyTipMessageConfigAction)this[PolicyTipMessageConfigSchema.Action];
			}
			set
			{
				this[PolicyTipMessageConfigSchema.Action] = value;
			}
		}

		[Parameter]
		public string Value
		{
			get
			{
				return (string)this[PolicyTipMessageConfigSchema.Value];
			}
			set
			{
				this[PolicyTipMessageConfigSchema.Value] = value;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return PolicyTipMessageConfig.PolicyTipMessageConfigContainer;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal static readonly ADObjectId PolicyTipMessageConfigContainer = new ADObjectId("CN=PolicyTipMessageConfigs,CN=Rules,CN=Transport Settings");

		private static PolicyTipMessageConfigSchema schema = ObjectSchema.GetInstance<PolicyTipMessageConfigSchema>();

		private static string mostDerivedClass = "msExchPolicyTipMessageConfig";
	}
}
