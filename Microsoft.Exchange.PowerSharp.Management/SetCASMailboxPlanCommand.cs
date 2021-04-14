using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetCASMailboxPlanCommand : SyntheticCommandWithPipelineInputNoOutput<CASMailboxPlan>
	{
		private SetCASMailboxPlanCommand() : base("Set-CASMailboxPlan")
		{
		}

		public SetCASMailboxPlanCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetCASMailboxPlanCommand SetParameters(SetCASMailboxPlanCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetCASMailboxPlanCommand SetParameters(SetCASMailboxPlanCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ActiveSyncMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual string OwaMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["OwaMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool ActiveSyncDebugLogging
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncDebugLogging"] = value;
				}
			}

			public virtual bool ActiveSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncEnabled"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool ECPEnabled
			{
				set
				{
					base.PowerSharpParameters["ECPEnabled"] = value;
				}
			}

			public virtual bool ImapEnabled
			{
				set
				{
					base.PowerSharpParameters["ImapEnabled"] = value;
				}
			}

			public virtual bool ImapUseProtocolDefaults
			{
				set
				{
					base.PowerSharpParameters["ImapUseProtocolDefaults"] = value;
				}
			}

			public virtual MimeTextFormat ImapMessagesRetrievalMimeFormat
			{
				set
				{
					base.PowerSharpParameters["ImapMessagesRetrievalMimeFormat"] = value;
				}
			}

			public virtual bool ImapEnableExactRFC822Size
			{
				set
				{
					base.PowerSharpParameters["ImapEnableExactRFC822Size"] = value;
				}
			}

			public virtual bool ImapProtocolLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["ImapProtocolLoggingEnabled"] = value;
				}
			}

			public virtual bool ImapSuppressReadReceipt
			{
				set
				{
					base.PowerSharpParameters["ImapSuppressReadReceipt"] = value;
				}
			}

			public virtual bool ImapForceICalForCalendarRetrievalOption
			{
				set
				{
					base.PowerSharpParameters["ImapForceICalForCalendarRetrievalOption"] = value;
				}
			}

			public virtual bool MAPIEnabled
			{
				set
				{
					base.PowerSharpParameters["MAPIEnabled"] = value;
				}
			}

			public virtual bool? MapiHttpEnabled
			{
				set
				{
					base.PowerSharpParameters["MapiHttpEnabled"] = value;
				}
			}

			public virtual bool MAPIBlockOutlookNonCachedMode
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookNonCachedMode"] = value;
				}
			}

			public virtual string MAPIBlockOutlookVersions
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookVersions"] = value;
				}
			}

			public virtual bool MAPIBlockOutlookRpcHttp
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookRpcHttp"] = value;
				}
			}

			public virtual bool MAPIBlockOutlookExternalConnectivity
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookExternalConnectivity"] = value;
				}
			}

			public virtual bool OWAEnabled
			{
				set
				{
					base.PowerSharpParameters["OWAEnabled"] = value;
				}
			}

			public virtual bool OWAforDevicesEnabled
			{
				set
				{
					base.PowerSharpParameters["OWAforDevicesEnabled"] = value;
				}
			}

			public virtual bool PopEnabled
			{
				set
				{
					base.PowerSharpParameters["PopEnabled"] = value;
				}
			}

			public virtual bool PopUseProtocolDefaults
			{
				set
				{
					base.PowerSharpParameters["PopUseProtocolDefaults"] = value;
				}
			}

			public virtual MimeTextFormat PopMessagesRetrievalMimeFormat
			{
				set
				{
					base.PowerSharpParameters["PopMessagesRetrievalMimeFormat"] = value;
				}
			}

			public virtual bool PopEnableExactRFC822Size
			{
				set
				{
					base.PowerSharpParameters["PopEnableExactRFC822Size"] = value;
				}
			}

			public virtual bool PopProtocolLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["PopProtocolLoggingEnabled"] = value;
				}
			}

			public virtual bool PopSuppressReadReceipt
			{
				set
				{
					base.PowerSharpParameters["PopSuppressReadReceipt"] = value;
				}
			}

			public virtual bool PopForceICalForCalendarRetrievalOption
			{
				set
				{
					base.PowerSharpParameters["PopForceICalForCalendarRetrievalOption"] = value;
				}
			}

			public virtual bool RemotePowerShellEnabled
			{
				set
				{
					base.PowerSharpParameters["RemotePowerShellEnabled"] = value;
				}
			}

			public virtual bool? EwsEnabled
			{
				set
				{
					base.PowerSharpParameters["EwsEnabled"] = value;
				}
			}

			public virtual bool? EwsAllowOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowMacOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowMacOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowEntourage
			{
				set
				{
					base.PowerSharpParameters["EwsAllowEntourage"] = value;
				}
			}

			public virtual EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
			{
				set
				{
					base.PowerSharpParameters["EwsApplicationAccessPolicy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsAllowList
			{
				set
				{
					base.PowerSharpParameters["EwsAllowList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsBlockList
			{
				set
				{
					base.PowerSharpParameters["EwsBlockList"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxPlanIdParameter(value) : null);
				}
			}

			public virtual string ActiveSyncMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual string OwaMailboxPolicy
			{
				set
				{
					base.PowerSharpParameters["OwaMailboxPolicy"] = ((value != null) ? new MailboxPolicyIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool ActiveSyncDebugLogging
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncDebugLogging"] = value;
				}
			}

			public virtual bool ActiveSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncEnabled"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool ECPEnabled
			{
				set
				{
					base.PowerSharpParameters["ECPEnabled"] = value;
				}
			}

			public virtual bool ImapEnabled
			{
				set
				{
					base.PowerSharpParameters["ImapEnabled"] = value;
				}
			}

			public virtual bool ImapUseProtocolDefaults
			{
				set
				{
					base.PowerSharpParameters["ImapUseProtocolDefaults"] = value;
				}
			}

			public virtual MimeTextFormat ImapMessagesRetrievalMimeFormat
			{
				set
				{
					base.PowerSharpParameters["ImapMessagesRetrievalMimeFormat"] = value;
				}
			}

			public virtual bool ImapEnableExactRFC822Size
			{
				set
				{
					base.PowerSharpParameters["ImapEnableExactRFC822Size"] = value;
				}
			}

			public virtual bool ImapProtocolLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["ImapProtocolLoggingEnabled"] = value;
				}
			}

			public virtual bool ImapSuppressReadReceipt
			{
				set
				{
					base.PowerSharpParameters["ImapSuppressReadReceipt"] = value;
				}
			}

			public virtual bool ImapForceICalForCalendarRetrievalOption
			{
				set
				{
					base.PowerSharpParameters["ImapForceICalForCalendarRetrievalOption"] = value;
				}
			}

			public virtual bool MAPIEnabled
			{
				set
				{
					base.PowerSharpParameters["MAPIEnabled"] = value;
				}
			}

			public virtual bool? MapiHttpEnabled
			{
				set
				{
					base.PowerSharpParameters["MapiHttpEnabled"] = value;
				}
			}

			public virtual bool MAPIBlockOutlookNonCachedMode
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookNonCachedMode"] = value;
				}
			}

			public virtual string MAPIBlockOutlookVersions
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookVersions"] = value;
				}
			}

			public virtual bool MAPIBlockOutlookRpcHttp
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookRpcHttp"] = value;
				}
			}

			public virtual bool MAPIBlockOutlookExternalConnectivity
			{
				set
				{
					base.PowerSharpParameters["MAPIBlockOutlookExternalConnectivity"] = value;
				}
			}

			public virtual bool OWAEnabled
			{
				set
				{
					base.PowerSharpParameters["OWAEnabled"] = value;
				}
			}

			public virtual bool OWAforDevicesEnabled
			{
				set
				{
					base.PowerSharpParameters["OWAforDevicesEnabled"] = value;
				}
			}

			public virtual bool PopEnabled
			{
				set
				{
					base.PowerSharpParameters["PopEnabled"] = value;
				}
			}

			public virtual bool PopUseProtocolDefaults
			{
				set
				{
					base.PowerSharpParameters["PopUseProtocolDefaults"] = value;
				}
			}

			public virtual MimeTextFormat PopMessagesRetrievalMimeFormat
			{
				set
				{
					base.PowerSharpParameters["PopMessagesRetrievalMimeFormat"] = value;
				}
			}

			public virtual bool PopEnableExactRFC822Size
			{
				set
				{
					base.PowerSharpParameters["PopEnableExactRFC822Size"] = value;
				}
			}

			public virtual bool PopProtocolLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["PopProtocolLoggingEnabled"] = value;
				}
			}

			public virtual bool PopSuppressReadReceipt
			{
				set
				{
					base.PowerSharpParameters["PopSuppressReadReceipt"] = value;
				}
			}

			public virtual bool PopForceICalForCalendarRetrievalOption
			{
				set
				{
					base.PowerSharpParameters["PopForceICalForCalendarRetrievalOption"] = value;
				}
			}

			public virtual bool RemotePowerShellEnabled
			{
				set
				{
					base.PowerSharpParameters["RemotePowerShellEnabled"] = value;
				}
			}

			public virtual bool? EwsEnabled
			{
				set
				{
					base.PowerSharpParameters["EwsEnabled"] = value;
				}
			}

			public virtual bool? EwsAllowOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowMacOutlook
			{
				set
				{
					base.PowerSharpParameters["EwsAllowMacOutlook"] = value;
				}
			}

			public virtual bool? EwsAllowEntourage
			{
				set
				{
					base.PowerSharpParameters["EwsAllowEntourage"] = value;
				}
			}

			public virtual EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
			{
				set
				{
					base.PowerSharpParameters["EwsApplicationAccessPolicy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsAllowList
			{
				set
				{
					base.PowerSharpParameters["EwsAllowList"] = value;
				}
			}

			public virtual MultiValuedProperty<string> EwsBlockList
			{
				set
				{
					base.PowerSharpParameters["EwsBlockList"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
