using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[Cmdlet("Retry", "Queue", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	[LocDescription(QueueViewerStrings.IDs.RetryQueueTask)]
	public sealed class RetryQueue : QueueAction
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Filter" == base.ParameterSetName)
				{
					return QueueViewerStrings.ConfirmationMessageRetryQueueFilter(base.Filter.ToString());
				}
				return QueueViewerStrings.ConfirmationMessageRetryQueueIdentity(base.Identity.ToString());
			}
		}

		public RetryQueue()
		{
			this.Resubmit = false;
		}

		[Parameter(Mandatory = false)]
		public bool Resubmit
		{
			get
			{
				return (bool)base.Fields["Resubmit"];
			}
			set
			{
				base.Fields["Resubmit"] = value;
			}
		}

		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleQueueInfo> queueViewerClient = new QueueViewerClient<ExtensibleQueueInfo>((string)base.Server))
			{
				queueViewerClient.RetryQueue(base.Identity, this.innerFilter, this.Resubmit);
			}
		}
	}
}
