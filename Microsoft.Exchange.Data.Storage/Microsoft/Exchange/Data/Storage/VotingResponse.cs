using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class VotingResponse : ReplyForwardCommon
	{
		internal VotingResponse(MessageItem originalItem, MessageItem newItem, ReplyForwardConfiguration configuration, string votingResponse) : base(originalItem, newItem, configuration, true)
		{
			this.votingResponse = votingResponse;
			this.newItem.SafeSetProperty(InternalSchema.IsVotingResponse, 1);
			int lastAction = originalItem.VotingInfo.GetOptionsList().IndexOf(votingResponse) + 1;
			this.newItem.SafeSetProperty(InternalSchema.ReplyForwardStatus, ReplyForwardUtils.EncodeReplyForwardStatus((LastAction)lastAction, IconIndex.MailReplied, originalItem.Id));
		}

		protected override void BuildSubject()
		{
			this.newItem[InternalSchema.SubjectPrefix] = this.votingResponse + ": ";
			this.newItem[InternalSchema.NormalizedSubject] = this.originalItem.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
		}

		protected override void UpdateNewItemProperties()
		{
			base.UpdateNewItemProperties();
			ReplyForwardCommon.BuildReplyRecipientsFromMessage(this.newItem as MessageItem, this.originalItem as MessageItem, false, true, true);
			this.newItem.SafeSetProperty(InternalSchema.Importance, Importance.Normal);
			this.newItem.SafeSetProperty(InternalSchema.IsVotingResponse, 1);
			this.newItem.SafeSetProperty(InternalSchema.ReportTag, this.originalItem.PropertyBag.GetValueOrDefault<byte[]>(InternalSchema.ReportTag));
			this.newItem.SafeSetProperty(InternalSchema.VotingResponse, this.votingResponse);
			if (this.originalItem.ClassName.Equals("IPM.Note.Microsoft.Approval.Request", StringComparison.OrdinalIgnoreCase))
			{
				MessageItem messageItem = this.originalItem as MessageItem;
				string[] array = (string[])messageItem.VotingInfo.GetOptionsList();
				int num = Array.IndexOf<string>(array, this.votingResponse);
				if (num == 0)
				{
					this.newItem.SafeSetProperty(InternalSchema.ItemClass, "IPM.Note.Microsoft.Approval.Reply.Approve");
					return;
				}
				if (num == 1)
				{
					this.newItem.SafeSetProperty(InternalSchema.ItemClass, "IPM.Note.Microsoft.Approval.Reply.Reject");
				}
			}
		}

		protected override void BuildBody(BodyConversionCallbacks callbacks)
		{
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(base.Format);
			bodyWriteConfiguration.SetTargetFormat(this.parameters.TargetFormat, this.newItem.Body.Charset);
			if (!string.IsNullOrEmpty(this.parameters.BodyPrefix))
			{
				bodyWriteConfiguration.AddInjectedText(this.parameters.BodyPrefix, null, this.parameters.BodyPrefixFormat);
			}
			using (this.newItem.Body.OpenTextWriter(bodyWriteConfiguration))
			{
			}
		}

		protected override void BuildAttachments(BodyConversionCallbacks callbacks, InboundConversionOptions optionsForSmime)
		{
		}

		private string votingResponse;
	}
}
