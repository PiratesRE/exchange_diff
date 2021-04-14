using System;
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
	[Cmdlet("initialize", "SystemAttendantPermissions", SupportsShouldProcess = true)]
	public sealed class InitializeSystemAttendantPermissions : SetupTaskBase
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
			ADSystemAttendantMailbox adsystemAttendantMailbox = null;
			Server server = null;
			SecurityIdentifier sid = this.exs.Sid;
			SecurityIdentifier securityIdentifier = new SecurityIdentifier("SY");
			try
			{
				server = ((ITopologyConfigurationSession)this.configurationSession).FindLocalServer();
			}
			catch (LocalServerNotFoundException ex)
			{
				base.WriteError(new CouldNotFindExchangeServerDirectoryEntryException(ex.Fqdn), ErrorCategory.InvalidData, null);
			}
			if (server != null)
			{
				base.LogReadObject(server);
				this.recipientSession.DomainController = server.OriginatingServer;
				ADRecipient[] array = this.recipientSession.Find(server.Id.GetChildId("Microsoft System Attendant"), QueryScope.Base, null, null, 1);
				if (array.Length > 0)
				{
					adsystemAttendantMailbox = (array[0] as ADSystemAttendantMailbox);
				}
			}
			if (adsystemAttendantMailbox != null)
			{
				base.LogReadObject(adsystemAttendantMailbox);
				GenericAce[] aces = new GenericAce[]
				{
					new CommonAce(AceFlags.None, AceQualifier.AccessAllowed, 131073, securityIdentifier, false, null)
				};
				DirectoryCommon.SetAclOnAlternateProperty(adsystemAttendantMailbox, aces, ADSystemAttendantMailboxSchema.ExchangeSecurityDescriptor);
				if (base.ShouldProcess(adsystemAttendantMailbox.DistinguishedName, Strings.InfoProcessAction(securityIdentifier.ToString()), null))
				{
					this.recipientSession.Save(adsystemAttendantMailbox);
				}
			}
			TaskLogger.LogExit();
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
