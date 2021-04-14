using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe
{
	[Cmdlet("Send", "MapiSubmitSystemProbe", SupportsShouldProcess = true)]
	[OutputType(new Type[]
	{
		typeof(Guid)
	})]
	public sealed class MapiSubmitSystemProbe : Task
	{
		[Parameter(Mandatory = true)]
		public string SenderEmailAddress { get; set; }

		[Parameter(Mandatory = false)]
		public string RecipientEmailAddress { get; set; }

		[Parameter(Mandatory = false)]
		public string InternetMessageIdOfTheMessageToDeleteFromOutbox { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSendMapiSubmitSystemProbe;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (string.IsNullOrEmpty(this.InternetMessageIdOfTheMessageToDeleteFromOutbox))
			{
				string text = null;
				Guid guid = this.SendMapiSubmitSystemProbe(this.SenderEmailAddress, this.RecipientEmailAddress, out text);
				base.WriteObject(string.Format("{0}:{1}\n{2}:{3}", new object[]
				{
					Strings.MapiSubmitSystemProbeId,
					guid,
					Strings.MapiSubmitSystemProbeInternetMessageId,
					text
				}));
				return;
			}
			switch (this.DeleteMessageFromOutbox(this.SenderEmailAddress, this.InternetMessageIdOfTheMessageToDeleteFromOutbox))
			{
			case DeletionResult.Fail:
				base.WriteError(new ApplicationException(Strings.MapiSubmitSystemProbeFail.ToString()), ErrorCategory.InvalidResult, null);
				return;
			case DeletionResult.NoMatchingMessage:
				base.WriteWarning(Strings.MapiSubmitSystemProbeNoMessageFound.ToString());
				return;
			case DeletionResult.Success:
				base.WriteVerbose(Strings.MapiSubmitSystemProbeSuccesfullyDeleted);
				return;
			default:
				return;
			}
		}

		private Guid SendMapiSubmitSystemProbe(string senderEmail, string recipientEmail, out string internetMessageId)
		{
			return MapiSubmitSystemProbeHandler.GetInstance().SendMapiSubmitSystemProbe(senderEmail, recipientEmail, out internetMessageId);
		}

		private DeletionResult DeleteMessageFromOutbox(string senderEmail, string internetMessageId)
		{
			return MapiSubmitSystemProbeHandler.GetInstance().DeleteMessageFromOutbox(senderEmail, internetMessageId);
		}
	}
}
