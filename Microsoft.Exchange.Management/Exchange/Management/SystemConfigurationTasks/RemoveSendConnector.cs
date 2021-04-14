using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "SendConnector", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSendConnector : RemoveSystemConfigurationObjectTask<SendConnectorIdParameter, SmtpSendConnectorConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (base.DataObject.IsInitialSendConnector())
				{
					return Strings.ConfirmationMessageRemoveEdgeSyncSendConnector(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageRemoveSendConnector(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			Server server = null;
			try
			{
				server = ((ITopologyConfigurationSession)base.DataSession).FindLocalServer();
			}
			catch (ComputerNameNotCurrentlyAvailableException)
			{
			}
			catch (LocalServerNotFoundException)
			{
			}
			if (server == null || !server.IsEdgeServer)
			{
				ManageSendConnectors.UpdateGwartLastModified((ITopologyConfigurationSession)base.DataSession, base.DataObject.SourceRoutingGroup, new ManageSendConnectors.ThrowTerminatingErrorDelegate(base.ThrowTerminatingError));
			}
		}

		protected override void InternalValidate()
		{
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
		}
	}
}
