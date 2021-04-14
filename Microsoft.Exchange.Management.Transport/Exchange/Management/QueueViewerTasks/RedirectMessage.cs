using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[Cmdlet("Redirect", "Message", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	[LocDescription(QueueViewerStrings.IDs.RedirectMessageTask)]
	public sealed class RedirectMessage : MessageAction
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<Fqdn> Target
		{
			get
			{
				return (MultiValuedProperty<Fqdn>)base.Fields["Target"];
			}
			set
			{
				base.Fields["Target"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return QueueViewerStrings.ConfirmationMessageRedirectMessage(string.Join<Fqdn>(",", this.Target));
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 796, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\Queueviewer\\MessageTasks.cs");
			foreach (Fqdn fqdn in this.Target)
			{
				Server server = topologyConfigurationSession.FindServerByFqdn(fqdn);
				if (server == null)
				{
					base.WriteError(new LocalizedException(QueueViewerStrings.UnknownServer(fqdn)), ErrorCategory.InvalidArgument, null);
				}
				else if (!server.IsHubTransportServer)
				{
					base.WriteError(new LocalizedException(QueueViewerStrings.NotTransportHubServer(fqdn)), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleMessageInfo> queueViewerClient = new QueueViewerClient<ExtensibleMessageInfo>((string)this.Server))
			{
				queueViewerClient.RedirectMessage(this.Target);
				base.WriteVerbose(QueueViewerStrings.SuccessMessageRedirectMessageRequestCompleted);
			}
		}

		protected override LocalizedException GetLocalizedException(Exception ex)
		{
			if (ex is QueueViewerException)
			{
				return ErrorMapper.GetLocalizedException((ex as QueueViewerException).ErrorCode, null, this.Server);
			}
			if (ex is RpcException)
			{
				return ErrorMapper.GetLocalizedException((ex as RpcException).ErrorCode, null, this.Server);
			}
			return null;
		}
	}
}
