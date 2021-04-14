using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SingleComponentThrottlingPolicy
	{
		internal SingleComponentThrottlingPolicy(BudgetType budgetType, IThrottlingPolicy fullPolicy)
		{
			this.budgetType = budgetType;
			if (fullPolicy == null)
			{
				throw new ArgumentNullException("fullPolicy");
			}
			this.fullPolicy = fullPolicy;
		}

		public static uint BulkOperationConcurrencyCap
		{
			get
			{
				return SingleComponentThrottlingPolicy.bulkOperationConcurrencyCap;
			}
			set
			{
				SingleComponentThrottlingPolicy.bulkOperationConcurrencyCap = value;
			}
		}

		public static uint NonInteractiveOperationConcurrencyCap
		{
			get
			{
				return SingleComponentThrottlingPolicy.nonInteractiveOperationConcurrencyCap;
			}
			set
			{
				SingleComponentThrottlingPolicy.nonInteractiveOperationConcurrencyCap = value;
			}
		}

		public virtual Unlimited<uint> MaxConcurrency
		{
			get
			{
				switch (this.budgetType)
				{
				case BudgetType.Owa:
					return this.fullPolicy.OwaMaxConcurrency;
				case BudgetType.Ews:
					return this.fullPolicy.EwsMaxConcurrency;
				case BudgetType.Eas:
					return this.fullPolicy.EasMaxConcurrency;
				case BudgetType.Pop:
					return this.fullPolicy.PopMaxConcurrency;
				case BudgetType.Imap:
					return this.fullPolicy.ImapMaxConcurrency;
				case BudgetType.PowerShell:
				case BudgetType.WSMan:
					return this.fullPolicy.PowerShellMaxConcurrency;
				case BudgetType.Rca:
					return this.fullPolicy.RcaMaxConcurrency;
				case BudgetType.Cpa:
					return this.fullPolicy.CpaMaxConcurrency;
				case BudgetType.Anonymous:
					return this.fullPolicy.AnonymousMaxConcurrency;
				case BudgetType.ResourceTracking:
					return Unlimited<uint>.UnlimitedValue;
				case BudgetType.WSManTenant:
					return this.fullPolicy.PowerShellMaxTenantConcurrency;
				case BudgetType.OwaVoice:
					return this.fullPolicy.OwaVoiceMaxConcurrency;
				case BudgetType.PushNotificationTenant:
					return this.fullPolicy.PushNotificationMaxConcurrency;
				case BudgetType.EwsBulkOperation:
					return SingleComponentThrottlingPolicy.BulkOperationConcurrencyCap;
				case BudgetType.OwaBulkOperation:
					return SingleComponentThrottlingPolicy.BulkOperationConcurrencyCap;
				case BudgetType.OwaNonInteractiveOperation:
					return SingleComponentThrottlingPolicy.NonInteractiveOperationConcurrencyCap;
				case BudgetType.E4eSender:
					return this.fullPolicy.EncryptionSenderMaxConcurrency;
				case BudgetType.E4eRecipient:
					return this.fullPolicy.EncryptionRecipientMaxConcurrency;
				case BudgetType.OutlookService:
					return this.fullPolicy.OutlookServiceMaxConcurrency;
				default:
					return Unlimited<uint>.UnlimitedValue;
				}
			}
		}

		public virtual Unlimited<uint> MaxBurst
		{
			get
			{
				switch (this.budgetType)
				{
				case BudgetType.Owa:
				case BudgetType.OwaBulkOperation:
				case BudgetType.OwaNonInteractiveOperation:
					return this.fullPolicy.OwaMaxBurst;
				case BudgetType.Ews:
				case BudgetType.EwsBulkOperation:
					return this.fullPolicy.EwsMaxBurst;
				case BudgetType.Eas:
					return this.fullPolicy.EasMaxBurst;
				case BudgetType.Pop:
					return this.fullPolicy.PopMaxBurst;
				case BudgetType.Imap:
					return this.fullPolicy.ImapMaxBurst;
				case BudgetType.PowerShell:
				case BudgetType.WSMan:
				case BudgetType.WSManTenant:
					return this.fullPolicy.PowerShellMaxBurst;
				case BudgetType.Rca:
					return this.fullPolicy.RcaMaxBurst;
				case BudgetType.Cpa:
					return this.fullPolicy.CpaMaxBurst;
				case BudgetType.Anonymous:
					return this.fullPolicy.AnonymousMaxBurst;
				case BudgetType.ResourceTracking:
					return Unlimited<uint>.UnlimitedValue;
				case BudgetType.OwaVoice:
					return this.fullPolicy.OwaVoiceMaxBurst;
				case BudgetType.PushNotificationTenant:
					return this.fullPolicy.PushNotificationMaxBurst;
				case BudgetType.E4eSender:
					return this.fullPolicy.EncryptionSenderMaxBurst;
				case BudgetType.E4eRecipient:
					return this.fullPolicy.EncryptionRecipientMaxBurst;
				case BudgetType.OutlookService:
					return this.fullPolicy.OutlookServiceMaxBurst;
				default:
					return Unlimited<uint>.UnlimitedValue;
				}
			}
		}

		public virtual Unlimited<uint> RechargeRate
		{
			get
			{
				switch (this.budgetType)
				{
				case BudgetType.Owa:
				case BudgetType.OwaBulkOperation:
				case BudgetType.OwaNonInteractiveOperation:
					return this.fullPolicy.OwaRechargeRate;
				case BudgetType.Ews:
				case BudgetType.EwsBulkOperation:
					return this.fullPolicy.EwsRechargeRate;
				case BudgetType.Eas:
					return this.fullPolicy.EasRechargeRate;
				case BudgetType.Pop:
					return this.fullPolicy.PopRechargeRate;
				case BudgetType.Imap:
					return this.fullPolicy.ImapRechargeRate;
				case BudgetType.PowerShell:
				case BudgetType.WSMan:
				case BudgetType.WSManTenant:
					return this.fullPolicy.PowerShellRechargeRate;
				case BudgetType.Rca:
					return this.fullPolicy.RcaRechargeRate;
				case BudgetType.Cpa:
					return this.fullPolicy.CpaRechargeRate;
				case BudgetType.Anonymous:
					return this.fullPolicy.AnonymousRechargeRate;
				case BudgetType.ResourceTracking:
					return Unlimited<uint>.UnlimitedValue;
				case BudgetType.OwaVoice:
					return this.fullPolicy.OwaVoiceRechargeRate;
				case BudgetType.PushNotificationTenant:
					return this.fullPolicy.PushNotificationRechargeRate;
				case BudgetType.E4eSender:
					return this.fullPolicy.EncryptionSenderRechargeRate;
				case BudgetType.E4eRecipient:
					return this.fullPolicy.EncryptionRecipientRechargeRate;
				case BudgetType.OutlookService:
					return this.fullPolicy.OutlookServiceRechargeRate;
				default:
					return Unlimited<uint>.UnlimitedValue;
				}
			}
		}

		public virtual Unlimited<uint> CutoffBalance
		{
			get
			{
				switch (this.budgetType)
				{
				case BudgetType.Owa:
				case BudgetType.OwaBulkOperation:
				case BudgetType.OwaNonInteractiveOperation:
					return this.fullPolicy.OwaCutoffBalance;
				case BudgetType.Ews:
				case BudgetType.EwsBulkOperation:
					return this.fullPolicy.EwsCutoffBalance;
				case BudgetType.Eas:
					return this.fullPolicy.EasCutoffBalance;
				case BudgetType.Pop:
					return this.fullPolicy.PopCutoffBalance;
				case BudgetType.Imap:
					return this.fullPolicy.ImapCutoffBalance;
				case BudgetType.PowerShell:
				case BudgetType.WSMan:
				case BudgetType.WSManTenant:
					return this.fullPolicy.PowerShellCutoffBalance;
				case BudgetType.Rca:
					return this.fullPolicy.RcaCutoffBalance;
				case BudgetType.Cpa:
					return this.fullPolicy.CpaCutoffBalance;
				case BudgetType.Anonymous:
					return this.fullPolicy.AnonymousCutoffBalance;
				case BudgetType.ResourceTracking:
					return Unlimited<uint>.UnlimitedValue;
				case BudgetType.OwaVoice:
					return this.fullPolicy.OwaVoiceCutoffBalance;
				case BudgetType.PushNotificationTenant:
					return this.fullPolicy.PushNotificationCutoffBalance;
				case BudgetType.E4eSender:
					return this.fullPolicy.EncryptionSenderCutoffBalance;
				case BudgetType.E4eRecipient:
					return this.fullPolicy.EncryptionRecipientCutoffBalance;
				case BudgetType.OutlookService:
					return this.fullPolicy.OutlookServiceCutoffBalance;
				default:
					return Unlimited<uint>.UnlimitedValue;
				}
			}
		}

		public Unlimited<uint> PowerShellMaxTenantConcurrency
		{
			get
			{
				return this.fullPolicy.PowerShellMaxTenantConcurrency;
			}
		}

		public Unlimited<uint> PowerShellMaxOperations
		{
			get
			{
				return this.fullPolicy.PowerShellMaxOperations;
			}
		}

		public Unlimited<uint> PowerShellMaxBurst
		{
			get
			{
				return this.fullPolicy.PowerShellMaxBurst;
			}
		}

		public Unlimited<uint> PowerShellRechargeRate
		{
			get
			{
				return this.fullPolicy.PowerShellRechargeRate;
			}
		}

		public Unlimited<uint> PowerShellCutoffBalance
		{
			get
			{
				return this.fullPolicy.PowerShellCutoffBalance;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletsTimePeriod
		{
			get
			{
				return this.fullPolicy.PowerShellMaxCmdletsTimePeriod;
			}
		}

		public Unlimited<uint> ExchangeMaxCmdlets
		{
			get
			{
				return this.fullPolicy.ExchangeMaxCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdletQueueDepth
		{
			get
			{
				return this.fullPolicy.PowerShellMaxCmdletQueueDepth;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdlets
		{
			get
			{
				return this.fullPolicy.PowerShellMaxDestructiveCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxDestructiveCmdletsTimePeriod
		{
			get
			{
				return this.fullPolicy.PowerShellMaxDestructiveCmdletsTimePeriod;
			}
		}

		public Unlimited<uint> PowerShellMaxCmdlets
		{
			get
			{
				return this.fullPolicy.PowerShellMaxCmdlets;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspaces
		{
			get
			{
				return this.fullPolicy.PowerShellMaxRunspaces;
			}
		}

		public Unlimited<uint> PowerShellMaxTenantRunspaces
		{
			get
			{
				return this.fullPolicy.PowerShellMaxTenantRunspaces;
			}
		}

		public Unlimited<uint> PowerShellMaxRunspacesTimePeriod
		{
			get
			{
				return this.fullPolicy.PowerShellMaxRunspacesTimePeriod;
			}
		}

		public Unlimited<uint> PswsMaxConcurrency
		{
			get
			{
				return this.fullPolicy.PswsMaxConcurrency;
			}
		}

		public Unlimited<uint> PswsMaxRequest
		{
			get
			{
				return this.fullPolicy.PswsMaxRequest;
			}
		}

		public Unlimited<uint> PswsMaxRequestTimePeriod
		{
			get
			{
				return this.fullPolicy.PswsMaxRequestTimePeriod;
			}
		}

		public Unlimited<uint> EasMaxDevices
		{
			get
			{
				return this.fullPolicy.EasMaxDevices;
			}
		}

		public Unlimited<uint> EasMaxDeviceDeletesPerMonth
		{
			get
			{
				return this.fullPolicy.EasMaxDeviceDeletesPerMonth;
			}
		}

		public Unlimited<uint> EasMaxInactivityForDeviceCleanup
		{
			get
			{
				return this.fullPolicy.EasMaxInactivityForDeviceCleanup;
			}
		}

		public Unlimited<uint> DiscoveryMaxConcurrency
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxConcurrency;
			}
		}

		public Unlimited<uint> DiscoveryMaxMailboxes
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywords
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxKeywords;
			}
		}

		public Unlimited<uint> DiscoveryMaxPreviewSearchMailboxes
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxPreviewSearchMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryMaxStatsSearchMailboxes
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxStatsSearchMailboxes;
			}
		}

		public Unlimited<uint> DiscoveryPreviewSearchResultsPageSize
		{
			get
			{
				return this.fullPolicy.DiscoveryPreviewSearchResultsPageSize;
			}
		}

		public Unlimited<uint> DiscoveryMaxKeywordsPerPage
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxKeywordsPerPage;
			}
		}

		public Unlimited<uint> DiscoveryMaxRefinerResults
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxRefinerResults;
			}
		}

		public Unlimited<uint> DiscoveryMaxSearchQueueDepth
		{
			get
			{
				return this.fullPolicy.DiscoveryMaxSearchQueueDepth;
			}
		}

		public Unlimited<uint> DiscoverySearchTimeoutPeriod
		{
			get
			{
				return this.fullPolicy.DiscoverySearchTimeoutPeriod;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionDGRecipients
		{
			get
			{
				return this.fullPolicy.ComplianceMaxExpansionDGRecipients;
			}
		}

		public Unlimited<uint> ComplianceMaxExpansionNestedDGs
		{
			get
			{
				return this.fullPolicy.ComplianceMaxExpansionNestedDGs;
			}
		}

		public Unlimited<uint> PushNotificationMaxConcurrency
		{
			get
			{
				return this.MaxConcurrency;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSubscriptions
		{
			get
			{
				return this.fullPolicy.OutlookServiceMaxSubscriptions;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return this.fullPolicy.OutlookServiceMaxSocketConnectionsPerDevice;
			}
		}

		public Unlimited<uint> OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return this.fullPolicy.OutlookServiceMaxSocketConnectionsPerUser;
			}
		}

		internal IThrottlingPolicy FullPolicy
		{
			get
			{
				return this.fullPolicy;
			}
		}

		private BudgetType budgetType;

		private IThrottlingPolicy fullPolicy;

		private static uint bulkOperationConcurrencyCap = 2U;

		private static uint nonInteractiveOperationConcurrencyCap = 2U;
	}
}
