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
	public class HostedConnectionFilterPolicy : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return HostedConnectionFilterPolicy.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return HostedConnectionFilterPolicy.parentPath;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return HostedConnectionFilterPolicy.ldapName;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		[Parameter]
		public new string AdminDisplayName
		{
			get
			{
				return (string)this[ADConfigurationObjectSchema.AdminDisplayName];
			}
			set
			{
				this[ADConfigurationObjectSchema.AdminDisplayName] = value;
			}
		}

		public bool IsDefault
		{
			get
			{
				return (bool)this[HostedConnectionFilterPolicySchema.IsDefault];
			}
			internal set
			{
				this[HostedConnectionFilterPolicySchema.IsDefault] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPRange> IPAllowList
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[HostedConnectionFilterPolicySchema.IPAllowList];
			}
			set
			{
				this[HostedConnectionFilterPolicySchema.IPAllowList] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<IPRange> IPBlockList
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[HostedConnectionFilterPolicySchema.IPBlockList];
			}
			set
			{
				this[HostedConnectionFilterPolicySchema.IPBlockList] = value;
			}
		}

		[Parameter]
		public bool EnableSafeList
		{
			get
			{
				return (bool)this[HostedConnectionFilterPolicySchema.EnableSafeList];
			}
			set
			{
				this[HostedConnectionFilterPolicySchema.EnableSafeList] = value;
			}
		}

		[Parameter]
		public DirectoryBasedEdgeBlockMode DirectoryBasedEdgeBlockMode
		{
			get
			{
				return (DirectoryBasedEdgeBlockMode)this[HostedConnectionFilterPolicySchema.DirectoryBasedEdgeBlockMode];
			}
			set
			{
				this[HostedConnectionFilterPolicySchema.DirectoryBasedEdgeBlockMode] = (int)value;
			}
		}

		private static readonly string ldapName = "msExchHostedConnectionFilterPolicy";

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Hosted Connection Filter,CN=Transport Settings");

		private static readonly HostedConnectionFilterPolicySchema schema = ObjectSchema.GetInstance<HostedConnectionFilterPolicySchema>();
	}
}
