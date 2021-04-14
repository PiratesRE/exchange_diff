using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("initialize", "ServerPermissions", SupportsShouldProcess = true)]
	public sealed class InitializeServerPermissions : SetupTaskBase
	{
		[Parameter(Mandatory = false)]
		public string ServerName
		{
			get
			{
				return (string)base.Fields["ServerName"];
			}
			set
			{
				base.Fields["ServerName"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			List<Server> list = new List<Server>();
			List<SecurityIdentifier> list2 = new List<SecurityIdentifier>();
			Server server = null;
			try
			{
				server = ((ITopologyConfigurationSession)this.configurationSession).FindLocalServer();
			}
			catch (LocalServerNotFoundException ex)
			{
				base.WriteError(new CouldNotFindExchangeServerDirectoryEntryException(ex.Fqdn), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(server);
			list.Add(server);
			ADComputer adcomputer = ((ITopologyConfigurationSession)this.domainConfigurationSession).FindLocalComputer();
			if (adcomputer == null)
			{
				base.WriteError(new CouldNotFindLocalhostDirectoryEntryException(), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(adcomputer);
			list2.Add(adcomputer.Sid);
			if (list.Count > 0)
			{
				List<ActiveDirectoryAccessRule> list3 = list2.ConvertAll<ActiveDirectoryAccessRule>((SecurityIdentifier machineSid) => new ActiveDirectoryAccessRule(machineSid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All));
				using (IEnumerator<string> enumerator = DirectoryCommon.ServerWriteAttrs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string attr = enumerator.Current;
						list3.AddRange(list2.ConvertAll<ActiveDirectoryAccessRule>((SecurityIdentifier machineSid) => new ActiveDirectoryAccessRule(machineSid, ActiveDirectoryRights.WriteProperty, AccessControlType.Allow, DirectoryCommon.GetSchemaPropertyGuid(this.configurationSession, attr), ActiveDirectorySecurityInheritance.None)));
					}
				}
				foreach (Server obj in list)
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, obj, list3.ToArray());
				}
				SecurityIdentifier sid = this.exs.Sid;
				List<ActiveDirectoryAccessRule> list4 = new List<ActiveDirectoryAccessRule>();
				list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreTransportAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreConstrainedDelegationExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreReadAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				list4.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreReadWriteAccessExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
				foreach (Server obj2 in list)
				{
					DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null, obj2, list4.ToArray());
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.exs = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.ExSWkGuid);
			if (this.exs == null)
			{
				base.ThrowTerminatingError(new ExSGroupNotFoundException(WellKnownGuid.ExSWkGuid), ErrorCategory.InvalidData, null);
			}
			base.LogReadObject(this.exs);
			TaskLogger.LogExit();
		}

		private ADGroup exs;
	}
}
