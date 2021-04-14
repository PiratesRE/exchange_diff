using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[LocDescription(QueueViewerStrings.IDs.FreezeMessageTask)]
	[Cmdlet("Suspend", "Message", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class FreezeMessage : MessageActionWithFilter
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Filter" == base.ParameterSetName)
				{
					return QueueViewerStrings.ConfirmationMessageSuspendMessageFilter(base.Filter.ToString());
				}
				return QueueViewerStrings.ConfirmationMessageSuspendMessageIdentity(base.Identity.ToString());
			}
		}

		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleMessageInfo> queueViewerClient = new QueueViewerClient<ExtensibleMessageInfo>((string)base.Server))
			{
				queueViewerClient.FreezeMessage(base.Identity, this.innerFilter);
			}
		}
	}
}
