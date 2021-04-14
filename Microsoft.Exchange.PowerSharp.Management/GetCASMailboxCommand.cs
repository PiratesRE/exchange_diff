using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetCASMailboxCommand : SyntheticCommandWithPipelineInput<MailboxIdParameter, MailboxIdParameter>
	{
		private GetCASMailboxCommand() : base("Get-CASMailbox")
		{
		}

		public GetCASMailboxCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetCASMailboxCommand SetParameters(GetCASMailboxCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCASMailboxCommand SetParameters(GetCASMailboxCommand.AnrSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetCASMailboxCommand SetParameters(GetCASMailboxCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter GetPopProtocolLog
			{
				set
				{
					base.PowerSharpParameters["GetPopProtocolLog"] = value;
				}
			}

			public virtual SwitchParameter GetImapProtocolLog
			{
				set
				{
					base.PowerSharpParameters["GetImapProtocolLog"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> SendLogsTo
			{
				set
				{
					base.PowerSharpParameters["SendLogsTo"] = value;
				}
			}

			public virtual SwitchParameter ProtocolSettings
			{
				set
				{
					base.PowerSharpParameters["ProtocolSettings"] = value;
				}
			}

			public virtual SwitchParameter ActiveSyncDebugLogging
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncDebugLogging"] = value;
				}
			}

			public virtual SwitchParameter RecalculateHasActiveSyncDevicePartnership
			{
				set
				{
					base.PowerSharpParameters["RecalculateHasActiveSyncDevicePartnership"] = value;
				}
			}

			public virtual SwitchParameter Monitoring
			{
				set
				{
					base.PowerSharpParameters["Monitoring"] = value;
				}
			}

			public virtual string Filter
			{
				set
				{
					base.PowerSharpParameters["Filter"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual string SortBy
			{
				set
				{
					base.PowerSharpParameters["SortBy"] = value;
				}
			}

			public virtual string OrganizationalUnit
			{
				set
				{
					base.PowerSharpParameters["OrganizationalUnit"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual SwitchParameter ReadFromDomainController
			{
				set
				{
					base.PowerSharpParameters["ReadFromDomainController"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
		}

		public class AnrSetParameters : ParametersBase
		{
			public virtual string Anr
			{
				set
				{
					base.PowerSharpParameters["Anr"] = value;
				}
			}

			public virtual SwitchParameter GetPopProtocolLog
			{
				set
				{
					base.PowerSharpParameters["GetPopProtocolLog"] = value;
				}
			}

			public virtual SwitchParameter GetImapProtocolLog
			{
				set
				{
					base.PowerSharpParameters["GetImapProtocolLog"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> SendLogsTo
			{
				set
				{
					base.PowerSharpParameters["SendLogsTo"] = value;
				}
			}

			public virtual SwitchParameter ProtocolSettings
			{
				set
				{
					base.PowerSharpParameters["ProtocolSettings"] = value;
				}
			}

			public virtual SwitchParameter ActiveSyncDebugLogging
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncDebugLogging"] = value;
				}
			}

			public virtual SwitchParameter RecalculateHasActiveSyncDevicePartnership
			{
				set
				{
					base.PowerSharpParameters["RecalculateHasActiveSyncDevicePartnership"] = value;
				}
			}

			public virtual SwitchParameter Monitoring
			{
				set
				{
					base.PowerSharpParameters["Monitoring"] = value;
				}
			}

			public virtual string Filter
			{
				set
				{
					base.PowerSharpParameters["Filter"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual string SortBy
			{
				set
				{
					base.PowerSharpParameters["SortBy"] = value;
				}
			}

			public virtual string OrganizationalUnit
			{
				set
				{
					base.PowerSharpParameters["OrganizationalUnit"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual SwitchParameter ReadFromDomainController
			{
				set
				{
					base.PowerSharpParameters["ReadFromDomainController"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter GetPopProtocolLog
			{
				set
				{
					base.PowerSharpParameters["GetPopProtocolLog"] = value;
				}
			}

			public virtual SwitchParameter GetImapProtocolLog
			{
				set
				{
					base.PowerSharpParameters["GetImapProtocolLog"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> SendLogsTo
			{
				set
				{
					base.PowerSharpParameters["SendLogsTo"] = value;
				}
			}

			public virtual SwitchParameter ProtocolSettings
			{
				set
				{
					base.PowerSharpParameters["ProtocolSettings"] = value;
				}
			}

			public virtual SwitchParameter ActiveSyncDebugLogging
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncDebugLogging"] = value;
				}
			}

			public virtual SwitchParameter RecalculateHasActiveSyncDevicePartnership
			{
				set
				{
					base.PowerSharpParameters["RecalculateHasActiveSyncDevicePartnership"] = value;
				}
			}

			public virtual SwitchParameter Monitoring
			{
				set
				{
					base.PowerSharpParameters["Monitoring"] = value;
				}
			}

			public virtual string Filter
			{
				set
				{
					base.PowerSharpParameters["Filter"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual AccountPartitionIdParameter AccountPartition
			{
				set
				{
					base.PowerSharpParameters["AccountPartition"] = value;
				}
			}

			public virtual string SortBy
			{
				set
				{
					base.PowerSharpParameters["SortBy"] = value;
				}
			}

			public virtual string OrganizationalUnit
			{
				set
				{
					base.PowerSharpParameters["OrganizationalUnit"] = ((value != null) ? new OrganizationalUnitIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual Unlimited<uint> ResultSize
			{
				set
				{
					base.PowerSharpParameters["ResultSize"] = value;
				}
			}

			public virtual SwitchParameter ReadFromDomainController
			{
				set
				{
					base.PowerSharpParameters["ReadFromDomainController"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
		}
	}
}
