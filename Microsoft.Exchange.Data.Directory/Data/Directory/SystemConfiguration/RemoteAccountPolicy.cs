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
	public class RemoteAccountPolicy : ADConfigurationObject
	{
		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan PollingInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[RemoteAccountPolicySchema.PollingInterval];
			}
			set
			{
				this[RemoteAccountPolicySchema.PollingInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TimeBeforeInactive
		{
			get
			{
				return (EnhancedTimeSpan)this[RemoteAccountPolicySchema.TimeBeforeInactive];
			}
			set
			{
				this[RemoteAccountPolicySchema.TimeBeforeInactive] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TimeBeforeDormant
		{
			get
			{
				return (EnhancedTimeSpan)this[RemoteAccountPolicySchema.TimeBeforeDormant];
			}
			set
			{
				this[RemoteAccountPolicySchema.TimeBeforeDormant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxSyncAccounts
		{
			get
			{
				return (int)this[RemoteAccountPolicySchema.MaxSyncAccounts];
			}
			set
			{
				this[RemoteAccountPolicySchema.MaxSyncAccounts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SyncNowAllowed
		{
			get
			{
				return (bool)this[RemoteAccountPolicySchema.SyncNowAllowed];
			}
			set
			{
				this[RemoteAccountPolicySchema.SyncNowAllowed] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RemoteAccountPolicy.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchSyncAccountsPolicy";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return RemoteAccountPolicy.RemoteAccountPolicysContainer;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal const string TaskNoun = "RemoteAccountPolicy";

		internal const string LdapName = "msExchSyncAccountsPolicy";

		private static readonly ADObjectId RemoteAccountPolicysContainer = new ADObjectId("CN=Remote Accounts Policies Container");

		private static readonly RemoteAccountPolicySchema SchemaObject = ObjectSchema.GetInstance<RemoteAccountPolicySchema>();
	}
}
