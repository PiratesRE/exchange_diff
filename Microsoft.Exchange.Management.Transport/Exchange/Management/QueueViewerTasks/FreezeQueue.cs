using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[LocDescription(QueueViewerStrings.IDs.FreezeQueueTask)]
	[Cmdlet("Suspend", "Queue", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class FreezeQueue : QueueAction
	{
		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleQueueInfo> queueViewerClient = new QueueViewerClient<ExtensibleQueueInfo>((string)base.Server))
			{
				queueViewerClient.FreezeQueue(base.Identity, this.innerFilter);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Filter" == base.ParameterSetName)
				{
					return QueueViewerStrings.ConfirmationMessageSuspendQueueFilter(base.Filter.ToString());
				}
				return QueueViewerStrings.ConfirmationMessageSuspendQueueIdentity(base.Identity.ToString());
			}
		}
	}
}
