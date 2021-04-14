using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Add", "DelegatedGroupsToLocalAdminGroup")]
	public class AddDelegatedGroupsToLocalAdminGroup : SetupTaskBase
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

		protected ADGroup ExchangeOrgAdminGroup
		{
			get
			{
				return this.eoa;
			}
		}

		protected Server ServerObject
		{
			get
			{
				return this.server;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.eoa = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EoaWkGuid);
			if (this.eoa == null)
			{
				base.ThrowTerminatingError(new ExOrgAdminSGroupNotFoundException(WellKnownGuid.EoaWkGuid), ErrorCategory.ObjectNotFound, null);
			}
			base.LogReadObject(this.eoa);
			this.ets = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EtsWkGuid);
			if (this.ets == null)
			{
				base.ThrowTerminatingError(new ExOrgAdminSGroupNotFoundException(WellKnownGuid.EtsWkGuid), ErrorCategory.ObjectNotFound, null);
			}
			base.LogReadObject(this.ets);
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.ServerName != null)
			{
				this.server = ((ITopologyConfigurationSession)this.configurationSession).FindServerByName(this.ServerName);
			}
			else
			{
				try
				{
					this.server = ((ITopologyConfigurationSession)this.configurationSession).FindLocalServer();
				}
				catch (LocalServerNotFoundException)
				{
					this.server = null;
				}
			}
			if (this.server == null)
			{
				base.WriteError(new DirectoryObjectNotFoundException(string.IsNullOrEmpty(this.ServerName) ? NativeHelpers.GetLocalComputerFqdn(false) : this.ServerName), ErrorCategory.ObjectNotFound, null);
			}
			else
			{
				base.LogReadObject(this.ServerObject);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			DirectoryEntry localAdminGroup = AddDelegatedGroupsToLocalAdminGroup.GetLocalAdminGroup(Environment.MachineName);
			if (localAdminGroup != null)
			{
				base.WriteVerbose(Strings.AboutToAddLocalOrgUSGToLocalAdminGroup);
				AddDelegatedGroupsToLocalAdminGroup.AddToLocalAdminGroup(this.eoa.Sid, localAdminGroup, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null);
				base.WriteVerbose(Strings.AboutToAddLocalEtsUSGToLocalAdminGroup);
				AddDelegatedGroupsToLocalAdminGroup.AddToLocalAdminGroup(this.ets.Sid, localAdminGroup, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), null);
			}
			else
			{
				base.WriteError(new CannotFindLocalAdminGroupException(Environment.MachineName), ErrorCategory.ObjectNotFound, null);
			}
			TaskLogger.LogExit();
		}

		internal static DirectoryEntry GetLocalAdminGroup(string serverName)
		{
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			NTAccount ntaccount = (NTAccount)securityIdentifier.Translate(typeof(NTAccount));
			string[] array = ntaccount.Value.Split(new char[]
			{
				'\\'
			}, 2);
			string name = array[1];
			DirectoryEntry result;
			using (DirectoryEntry directoryEntry = new DirectoryEntry("WinNT://" + serverName + ",computer"))
			{
				DirectoryEntry directoryEntry2 = directoryEntry.Children.Find(name, "group");
				result = directoryEntry2;
			}
			return result;
		}

		internal static void AddToLocalAdminGroup(SecurityIdentifier sid, DirectoryEntry localAdminGroup, Task.TaskVerboseLoggingDelegate logVerbose, string user)
		{
			string user2 = string.IsNullOrEmpty(user) ? sid.ToString() : user;
			try
			{
				localAdminGroup.Invoke("Add", new object[]
				{
					"WinNT://" + sid.ToString()
				});
				localAdminGroup.CommitChanges();
			}
			catch (TargetInvocationException ex)
			{
				logVerbose(Strings.FailToAddServerAdminToLocalAdminGroup(user2, ex.ToString()));
			}
		}

		private ADGroup eoa;

		private ADGroup ets;

		private Server server;
	}
}
