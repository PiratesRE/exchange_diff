using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[LocDescription(QueueViewerStrings.IDs.UnfreezeMessageTask)]
	[Cmdlet("Resume", "Message", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class UnfreezeMessage : MessageActionWithFilter
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Filter" == base.ParameterSetName)
				{
					return QueueViewerStrings.ConfirmationMessageResumeMessageFilter(base.Filter.ToString());
				}
				return QueueViewerStrings.ConfirmationMessageResumeMessageIdentity(base.Identity.ToString());
			}
		}

		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleMessageInfo> queueViewerClient = new QueueViewerClient<ExtensibleMessageInfo>((string)base.Server))
			{
				queueViewerClient.UnfreezeMessage(base.Identity, this.innerFilter);
			}
		}
	}
}
