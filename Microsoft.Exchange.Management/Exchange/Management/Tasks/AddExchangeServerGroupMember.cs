using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("add", "ExchangeServerGroupMember", SupportsShouldProcess = true)]
	public sealed class AddExchangeServerGroupMember : ManageExchangeServerGroupMember
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter SkipEtsGroup
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipEtsGroup"] ?? false);
			}
			set
			{
				base.Fields["SkipEtsGroup"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.server == null)
			{
				base.WriteError(new CouldNotFindServerDirectoryEntryException(string.IsNullOrEmpty(base.ServerName) ? NativeHelpers.GetLocalComputerFqdn(false) : base.ServerName), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			if (this.serverDomain == null)
			{
				base.WriteError(new DomainNotReachableException(this.server.Id.DomainId.DistinguishedName, "PrepareDomain"), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			if (this.meso == null)
			{
				base.WriteError(new MesoContainerNotFoundException(this.serverDomain.Name), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			if (this.e12ds == null)
			{
				base.WriteError(new E12DomainServersGroupNotFoundException(this.meso.Id.GetChildId("Exchange Install Domain Servers").DistinguishedName, this.meso.OriginatingServer), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			if (this.exs == null)
			{
				base.WriteError(new ExSGroupNotFoundException(WellKnownGuid.ExSWkGuid), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			if (this.ets == null)
			{
				base.WriteError(new ExSGroupNotFoundException(WellKnownGuid.EtsWkGuid), ErrorCategory.ObjectNotFound, null);
				TaskLogger.LogExit();
				return;
			}
			this.AddServerToGroup(this.e12ds, this.recipientSession);
			this.AddServerToGroup(this.exs, this.rootDomainRecipientSession);
			if (!this.SkipEtsGroup)
			{
				this.AddServerToGroup(this.ets, this.rootDomainRecipientSession);
			}
			base.WriteObject(this.exs.Sid.ToString());
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
