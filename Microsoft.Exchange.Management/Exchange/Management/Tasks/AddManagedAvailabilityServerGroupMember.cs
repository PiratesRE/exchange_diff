using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("add", "ManagedAvailabilityServerGroupMember", SupportsShouldProcess = true)]
	public sealed class AddManagedAvailabilityServerGroupMember : ManageManagedAvailabilityServerGroupMember
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.server == null)
			{
				base.WriteError(new CouldNotFindServerDirectoryEntryException(string.IsNullOrEmpty(base.ServerName) ? NativeHelpers.GetLocalComputerFqdn(false) : base.ServerName), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			if (this.mas == null)
			{
				base.WriteError(new MaSGroupNotFoundException(WellKnownGuid.MaSWkGuid), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			this.AddServerToGroup(this.mas, this.rootDomainRecipientSession);
			this.server.MonitoringInstalled = 1;
			this.gcSession.Save(this.server);
			TaskLogger.LogExit();
		}

		private void AddServerToGroup(ADGroup group, IRecipientSession session)
		{
			if (group.Members.Contains(this.server.Id))
			{
				this.WriteWarning(Strings.InfoAlreadyIsMemberOfGroup(this.server.DistinguishedName, group.DistinguishedName));
				return;
			}
			group.Members.Add(this.server.Id);
			if (base.ShouldProcess(group.DistinguishedName, Strings.InfoProcessAddMember(this.server.DistinguishedName), null))
			{
				try
				{
					SetupTaskBase.Save(group, session);
					base.LogWriteObject(group);
				}
				catch (ADOperationException ex)
				{
					this.WriteWarning(Strings.ErrorCouldNotAddGroupMember(group.Id.DistinguishedName, this.server.Id.DistinguishedName, ex.Message));
				}
			}
		}
	}
}
