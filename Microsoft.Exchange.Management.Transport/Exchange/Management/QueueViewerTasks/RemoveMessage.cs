using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[Cmdlet("Remove", "Message", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	[LocDescription(QueueViewerStrings.IDs.RemoveMessageTask)]
	public sealed class RemoveMessage : MessageActionWithFilter
	{
		public RemoveMessage()
		{
			this.WithNDR = true;
		}

		[Parameter(Mandatory = false)]
		public bool WithNDR
		{
			get
			{
				return (bool)base.Fields["WithNDR"];
			}
			set
			{
				base.Fields["WithNDR"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Filter" == base.ParameterSetName)
				{
					return QueueViewerStrings.ConfirmationMessageRemoveMessageFilter(base.Filter.ToString());
				}
				return QueueViewerStrings.ConfirmationMessageRemoveMessageIdentity(base.Identity.ToString());
			}
		}

		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleMessageInfo> queueViewerClient = new QueueViewerClient<ExtensibleMessageInfo>((string)base.Server))
			{
				queueViewerClient.DeleteMessage(base.Identity, this.innerFilter, this.WithNDR);
			}
		}
	}
}
