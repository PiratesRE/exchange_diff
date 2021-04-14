using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class CASMailboxPlan : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return CASMailboxPlan.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public CASMailboxPlan()
		{
		}

		public CASMailboxPlan(ADUser dataObject) : base(dataObject)
		{
		}

		internal static CASMailboxPlan FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new CASMailboxPlan(dataObject);
		}

		public ADObjectId ActiveSyncMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[CASMailboxPlanSchema.ActiveSyncMailboxPolicy];
			}
			set
			{
				this[CASMailboxPlanSchema.ActiveSyncMailboxPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ActiveSyncDebugLogging
		{
			get
			{
				return CASMailbox.IsMailboxProtocolLoggingEnabled(this, CASMailboxSchema.ActiveSyncDebugLogging);
			}
			set
			{
				CASMailbox.SetMailboxProtocolLoggingEnabled(this, CASMailboxSchema.ActiveSyncDebugLogging, value);
			}
		}

		[Parameter]
		public bool ActiveSyncEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.ActiveSyncEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.ActiveSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[CASMailboxPlanSchema.DisplayName];
			}
			set
			{
				this[CASMailboxPlanSchema.DisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ECPEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.ECPEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.ECPEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.ImapEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.ImapEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapUseProtocolDefaults
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.ImapUseProtocolDefaults];
			}
			set
			{
				this[CASMailboxPlanSchema.ImapUseProtocolDefaults] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MimeTextFormat ImapMessagesRetrievalMimeFormat
		{
			get
			{
				return (MimeTextFormat)this[CASMailboxPlanSchema.ImapMessagesRetrievalMimeFormat];
			}
			set
			{
				this[CASMailboxPlanSchema.ImapMessagesRetrievalMimeFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapEnableExactRFC822Size
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.ImapEnableExactRFC822Size];
			}
			set
			{
				this[CASMailboxPlanSchema.ImapEnableExactRFC822Size] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapProtocolLoggingEnabled
		{
			get
			{
				return CASMailbox.IsMailboxProtocolLoggingEnabled(this, CASMailboxPlanSchema.ImapProtocolLoggingEnabled);
			}
			set
			{
				CASMailbox.SetMailboxProtocolLoggingEnabled(this, CASMailboxPlanSchema.ImapProtocolLoggingEnabled, value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapSuppressReadReceipt
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.ImapSuppressReadReceipt];
			}
			set
			{
				this[CASMailboxPlanSchema.ImapSuppressReadReceipt] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapForceICalForCalendarRetrievalOption
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.ImapForceICalForCalendarRetrievalOption];
			}
			set
			{
				this[CASMailboxPlanSchema.ImapForceICalForCalendarRetrievalOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MAPIEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.MAPIEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.MAPIEnabled] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public bool? MapiHttpEnabled
		{
			get
			{
				return (bool?)this[CASMailboxSchema.MapiHttpEnabled];
			}
			set
			{
				this[CASMailboxSchema.MapiHttpEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MAPIBlockOutlookNonCachedMode
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.MAPIBlockOutlookNonCachedMode];
			}
			set
			{
				this[CASMailboxPlanSchema.MAPIBlockOutlookNonCachedMode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MAPIBlockOutlookVersions
		{
			get
			{
				return (string)this[CASMailboxPlanSchema.MAPIBlockOutlookVersions];
			}
			set
			{
				this[CASMailboxPlanSchema.MAPIBlockOutlookVersions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MAPIBlockOutlookRpcHttp
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.MAPIBlockOutlookRpcHttp];
			}
			set
			{
				this[CASMailboxPlanSchema.MAPIBlockOutlookRpcHttp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MAPIBlockOutlookExternalConnectivity
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.MAPIBlockOutlookExternalConnectivity];
			}
			set
			{
				this[CASMailboxPlanSchema.MAPIBlockOutlookExternalConnectivity] = value;
			}
		}

		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[CASMailboxPlanSchema.OwaMailboxPolicy];
			}
			set
			{
				this[CASMailboxPlanSchema.OwaMailboxPolicy] = value;
			}
		}

		[Parameter]
		public bool OWAEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.OWAEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.OWAEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWAforDevicesEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.OWAforDevicesEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.OWAforDevicesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.PopEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.PopEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopUseProtocolDefaults
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.PopUseProtocolDefaults];
			}
			set
			{
				this[CASMailboxPlanSchema.PopUseProtocolDefaults] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MimeTextFormat PopMessagesRetrievalMimeFormat
		{
			get
			{
				return (MimeTextFormat)this[CASMailboxPlanSchema.PopMessagesRetrievalMimeFormat];
			}
			set
			{
				this[CASMailboxPlanSchema.PopMessagesRetrievalMimeFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopEnableExactRFC822Size
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.PopEnableExactRFC822Size];
			}
			set
			{
				this[CASMailboxPlanSchema.PopEnableExactRFC822Size] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopProtocolLoggingEnabled
		{
			get
			{
				return CASMailbox.IsMailboxProtocolLoggingEnabled(this, CASMailboxPlanSchema.PopProtocolLoggingEnabled);
			}
			set
			{
				CASMailbox.SetMailboxProtocolLoggingEnabled(this, CASMailboxPlanSchema.PopProtocolLoggingEnabled, value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopSuppressReadReceipt
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.PopSuppressReadReceipt];
			}
			set
			{
				this[CASMailboxPlanSchema.PopSuppressReadReceipt] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopForceICalForCalendarRetrievalOption
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.PopForceICalForCalendarRetrievalOption];
			}
			set
			{
				this[CASMailboxPlanSchema.PopForceICalForCalendarRetrievalOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemotePowerShellEnabled
		{
			get
			{
				return (bool)this[CASMailboxPlanSchema.RemotePowerShellEnabled];
			}
			set
			{
				this[CASMailboxPlanSchema.RemotePowerShellEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EwsEnabled
		{
			get
			{
				return CASMailboxHelper.ToBooleanNullable((int?)this[CASMailboxPlanSchema.EwsEnabled]);
			}
			set
			{
				this[CASMailboxPlanSchema.EwsEnabled] = CASMailboxHelper.ToInt32Nullable(value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EwsAllowOutlook
		{
			get
			{
				return (bool?)this[CASMailboxPlanSchema.EwsAllowOutlook];
			}
			set
			{
				this[CASMailboxPlanSchema.EwsAllowOutlook] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EwsAllowMacOutlook
		{
			get
			{
				return (bool?)this[CASMailboxPlanSchema.EwsAllowMacOutlook];
			}
			set
			{
				this[CASMailboxPlanSchema.EwsAllowMacOutlook] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EwsAllowEntourage
		{
			get
			{
				return (bool?)this[CASMailboxPlanSchema.EwsAllowEntourage];
			}
			set
			{
				this[CASMailboxPlanSchema.EwsAllowEntourage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
		{
			get
			{
				return (EwsApplicationAccessPolicy?)this[CASMailboxPlanSchema.EwsApplicationAccessPolicy];
			}
			set
			{
				this[CASMailboxPlanSchema.EwsApplicationAccessPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EwsAllowList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[CASMailboxPlanSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceAllowList)
				{
					return (MultiValuedProperty<string>)this[CASMailboxPlanSchema.EwsExceptions];
				}
				return null;
			}
			set
			{
				this[CASMailboxPlanSchema.EwsExceptions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EwsBlockList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[CASMailboxPlanSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceBlockList)
				{
					return (MultiValuedProperty<string>)this[CASMailboxPlanSchema.EwsExceptions];
				}
				return null;
			}
			set
			{
				this[CASMailboxPlanSchema.EwsExceptions] = value;
			}
		}

		private static CASMailboxPlanSchema schema = ObjectSchema.GetInstance<CASMailboxPlanSchema>();
	}
}
