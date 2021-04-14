using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[LocDescription(QueueViewerStrings.IDs.UnfreezeQueueTask)]
	[Cmdlet("Resume", "Queue", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class UnfreezeQueue : QueueAction
	{
		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleQueueInfo> queueViewerClient = new QueueViewerClient<ExtensibleQueueInfo>((string)base.Server))
			{
				queueViewerClient.UnfreezeQueue(base.Identity, this.innerFilter);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Filter" == base.ParameterSetName)
				{
					return QueueViewerStrings.ConfirmationMessageResumeQueueFilter(base.Filter.ToString());
				}
				return QueueViewerStrings.ConfirmationMessageResumeQueueIdentity(base.Identity.ToString());
			}
		}
	}
}
