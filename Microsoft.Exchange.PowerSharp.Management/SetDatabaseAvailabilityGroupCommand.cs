using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetDatabaseAvailabilityGroupCommand : SyntheticCommandWithPipelineInputNoOutput<DatabaseAvailabilityGroup>
	{
		private SetDatabaseAvailabilityGroupCommand() : base("Set-DatabaseAvailabilityGroup")
		{
		}

		public SetDatabaseAvailabilityGroupCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetDatabaseAvailabilityGroupCommand SetParameters(SetDatabaseAvailabilityGroupCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetDatabaseAvailabilityGroupCommand SetParameters(SetDatabaseAvailabilityGroupCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual FileShareWitnessServerName WitnessServer
			{
				set
				{
					base.PowerSharpParameters["WitnessServer"] = value;
				}
			}

			public virtual ushort ReplicationPort
			{
				set
				{
					base.PowerSharpParameters["ReplicationPort"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroup.NetworkOption NetworkCompression
			{
				set
				{
					base.PowerSharpParameters["NetworkCompression"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroup.NetworkOption NetworkEncryption
			{
				set
				{
					base.PowerSharpParameters["NetworkEncryption"] = value;
				}
			}

			public virtual bool ManualDagNetworkConfiguration
			{
				set
				{
					base.PowerSharpParameters["ManualDagNetworkConfiguration"] = value;
				}
			}

			public virtual SwitchParameter DiscoverNetworks
			{
				set
				{
					base.PowerSharpParameters["DiscoverNetworks"] = value;
				}
			}

			public virtual SwitchParameter AllowCrossSiteRpcClientAccess
			{
				set
				{
					base.PowerSharpParameters["AllowCrossSiteRpcClientAccess"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath WitnessDirectory
			{
				set
				{
					base.PowerSharpParameters["WitnessDirectory"] = value;
				}
			}

			public virtual FileShareWitnessServerName AlternateWitnessServer
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessServer"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AlternateWitnessDirectory
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessDirectory"] = value;
				}
			}

			public virtual DatacenterActivationModeOption DatacenterActivationMode
			{
				set
				{
					base.PowerSharpParameters["DatacenterActivationMode"] = value;
				}
			}

			public virtual IPAddress DatabaseAvailabilityGroupIpAddresses
			{
				set
				{
					base.PowerSharpParameters["DatabaseAvailabilityGroupIpAddresses"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroupConfigurationIdParameter DagConfiguration
			{
				set
				{
					base.PowerSharpParameters["DagConfiguration"] = value;
				}
			}

			public virtual SwitchParameter SkipDagValidation
			{
				set
				{
					base.PowerSharpParameters["SkipDagValidation"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int AutoDagDatabaseCopiesPerDatabase
			{
				set
				{
					base.PowerSharpParameters["AutoDagDatabaseCopiesPerDatabase"] = value;
				}
			}

			public virtual int AutoDagDatabaseCopiesPerVolume
			{
				set
				{
					base.PowerSharpParameters["AutoDagDatabaseCopiesPerVolume"] = value;
				}
			}

			public virtual int AutoDagTotalNumberOfDatabases
			{
				set
				{
					base.PowerSharpParameters["AutoDagTotalNumberOfDatabases"] = value;
				}
			}

			public virtual int AutoDagTotalNumberOfServers
			{
				set
				{
					base.PowerSharpParameters["AutoDagTotalNumberOfServers"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AutoDagDatabasesRootFolderPath
			{
				set
				{
					base.PowerSharpParameters["AutoDagDatabasesRootFolderPath"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AutoDagVolumesRootFolderPath
			{
				set
				{
					base.PowerSharpParameters["AutoDagVolumesRootFolderPath"] = value;
				}
			}

			public virtual bool AutoDagAllServersInstalled
			{
				set
				{
					base.PowerSharpParameters["AutoDagAllServersInstalled"] = value;
				}
			}

			public virtual bool AutoDagAutoReseedEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoDagAutoReseedEnabled"] = value;
				}
			}

			public virtual bool AutoDagDiskReclaimerEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoDagDiskReclaimerEnabled"] = value;
				}
			}

			public virtual bool AutoDagBitlockerEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoDagBitlockerEnabled"] = value;
				}
			}

			public virtual bool AutoDagFIPSCompliant
			{
				set
				{
					base.PowerSharpParameters["AutoDagFIPSCompliant"] = value;
				}
			}

			public virtual bool ReplayLagManagerEnabled
			{
				set
				{
					base.PowerSharpParameters["ReplayLagManagerEnabled"] = value;
				}
			}

			public virtual ByteQuantifiedSize? MailboxLoadBalanceMaximumEdbFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceMaximumEdbFileSize"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceRelativeLoadCapacity
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceRelativeLoadCapacity"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceOverloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceOverloadedThreshold"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceUnderloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceUnderloadedThreshold"] = value;
				}
			}

			public virtual bool MailboxLoadBalanceEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceEnabled"] = value;
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
			public virtual DatabaseAvailabilityGroupIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual FileShareWitnessServerName WitnessServer
			{
				set
				{
					base.PowerSharpParameters["WitnessServer"] = value;
				}
			}

			public virtual ushort ReplicationPort
			{
				set
				{
					base.PowerSharpParameters["ReplicationPort"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroup.NetworkOption NetworkCompression
			{
				set
				{
					base.PowerSharpParameters["NetworkCompression"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroup.NetworkOption NetworkEncryption
			{
				set
				{
					base.PowerSharpParameters["NetworkEncryption"] = value;
				}
			}

			public virtual bool ManualDagNetworkConfiguration
			{
				set
				{
					base.PowerSharpParameters["ManualDagNetworkConfiguration"] = value;
				}
			}

			public virtual SwitchParameter DiscoverNetworks
			{
				set
				{
					base.PowerSharpParameters["DiscoverNetworks"] = value;
				}
			}

			public virtual SwitchParameter AllowCrossSiteRpcClientAccess
			{
				set
				{
					base.PowerSharpParameters["AllowCrossSiteRpcClientAccess"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath WitnessDirectory
			{
				set
				{
					base.PowerSharpParameters["WitnessDirectory"] = value;
				}
			}

			public virtual FileShareWitnessServerName AlternateWitnessServer
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessServer"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AlternateWitnessDirectory
			{
				set
				{
					base.PowerSharpParameters["AlternateWitnessDirectory"] = value;
				}
			}

			public virtual DatacenterActivationModeOption DatacenterActivationMode
			{
				set
				{
					base.PowerSharpParameters["DatacenterActivationMode"] = value;
				}
			}

			public virtual IPAddress DatabaseAvailabilityGroupIpAddresses
			{
				set
				{
					base.PowerSharpParameters["DatabaseAvailabilityGroupIpAddresses"] = value;
				}
			}

			public virtual DatabaseAvailabilityGroupConfigurationIdParameter DagConfiguration
			{
				set
				{
					base.PowerSharpParameters["DagConfiguration"] = value;
				}
			}

			public virtual SwitchParameter SkipDagValidation
			{
				set
				{
					base.PowerSharpParameters["SkipDagValidation"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int AutoDagDatabaseCopiesPerDatabase
			{
				set
				{
					base.PowerSharpParameters["AutoDagDatabaseCopiesPerDatabase"] = value;
				}
			}

			public virtual int AutoDagDatabaseCopiesPerVolume
			{
				set
				{
					base.PowerSharpParameters["AutoDagDatabaseCopiesPerVolume"] = value;
				}
			}

			public virtual int AutoDagTotalNumberOfDatabases
			{
				set
				{
					base.PowerSharpParameters["AutoDagTotalNumberOfDatabases"] = value;
				}
			}

			public virtual int AutoDagTotalNumberOfServers
			{
				set
				{
					base.PowerSharpParameters["AutoDagTotalNumberOfServers"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AutoDagDatabasesRootFolderPath
			{
				set
				{
					base.PowerSharpParameters["AutoDagDatabasesRootFolderPath"] = value;
				}
			}

			public virtual NonRootLocalLongFullPath AutoDagVolumesRootFolderPath
			{
				set
				{
					base.PowerSharpParameters["AutoDagVolumesRootFolderPath"] = value;
				}
			}

			public virtual bool AutoDagAllServersInstalled
			{
				set
				{
					base.PowerSharpParameters["AutoDagAllServersInstalled"] = value;
				}
			}

			public virtual bool AutoDagAutoReseedEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoDagAutoReseedEnabled"] = value;
				}
			}

			public virtual bool AutoDagDiskReclaimerEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoDagDiskReclaimerEnabled"] = value;
				}
			}

			public virtual bool AutoDagBitlockerEnabled
			{
				set
				{
					base.PowerSharpParameters["AutoDagBitlockerEnabled"] = value;
				}
			}

			public virtual bool AutoDagFIPSCompliant
			{
				set
				{
					base.PowerSharpParameters["AutoDagFIPSCompliant"] = value;
				}
			}

			public virtual bool ReplayLagManagerEnabled
			{
				set
				{
					base.PowerSharpParameters["ReplayLagManagerEnabled"] = value;
				}
			}

			public virtual ByteQuantifiedSize? MailboxLoadBalanceMaximumEdbFileSize
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceMaximumEdbFileSize"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceRelativeLoadCapacity
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceRelativeLoadCapacity"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceOverloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceOverloadedThreshold"] = value;
				}
			}

			public virtual int? MailboxLoadBalanceUnderloadedThreshold
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceUnderloadedThreshold"] = value;
				}
			}

			public virtual bool MailboxLoadBalanceEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxLoadBalanceEnabled"] = value;
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
