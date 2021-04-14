using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorEmailMessageItem : DisposeTrackableBase, IAnchorEmailMessageItem, IAnchorAttachmentMessage, IDisposable
	{
		internal AnchorEmailMessageItem(AnchorContext context, IAnchorADProvider adProvider, MessageItem message)
		{
			this.context = context;
			this.ADProvider = adProvider;
			this.Message = message;
		}

		private IAnchorADProvider ADProvider { get; set; }

		private MessageItem Message { get; set; }

		public void Send(IEnumerable<SmtpAddress> recipientAddresses, string subject, string body)
		{
			AnchorUtil.ThrowOnCollectionEmptyArgument(recipientAddresses, "recipientAddresses");
			AnchorUtil.ThrowOnNullOrEmptyArgument(subject, "subject");
			AnchorUtil.ThrowOnNullOrEmptyArgument(body, "body");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SmtpAddress smtpAddress in recipientAddresses)
			{
				this.Message.Recipients.Add(this.CreateRecipient(smtpAddress.ToString()), RecipientItemType.To);
				stringBuilder.Append(smtpAddress.ToString());
				stringBuilder.Append(';');
			}
			this.Message.AutoResponseSuppress = AutoResponseSuppress.All;
			this.Message.Subject = subject;
			using (TextWriter textWriter = this.Message.Body.OpenTextWriter(BodyFormat.TextHtml))
			{
				textWriter.Write(body);
			}
			this.context.Logger.Log(MigrationEventType.Information, "Sending report email to {0}, subject {1}", new object[]
			{
				stringBuilder,
				subject
			});
			this.Message.SendWithoutSavingMessage();
		}

		public AnchorAttachment CreateAttachment(string name)
		{
			base.CheckDisposed();
			return AnchorMessageHelper.CreateAttachment(this.context, this.Message, name);
		}

		public AnchorAttachment GetAttachment(string name, PropertyOpenMode openMode)
		{
			base.CheckDisposed();
			return AnchorMessageHelper.GetAttachment(this.context, this.Message, name, openMode);
		}

		public void DeleteAttachment(string name)
		{
			base.CheckDisposed();
			AnchorMessageHelper.DeleteAttachment(this.Message, name);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Message != null)
			{
				this.Message.Dispose();
				this.Message = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AnchorEmailMessageItem>(this);
		}

		private Participant CreateRecipient(string emailAddress)
		{
			ADRecipient adrecipientByProxyAddress = this.ADProvider.GetADRecipientByProxyAddress(emailAddress);
			if (adrecipientByProxyAddress != null)
			{
				return new Participant(adrecipientByProxyAddress);
			}
			return new Participant(emailAddress, emailAddress, "SMTP");
		}

		private AnchorContext context;
	}
}
