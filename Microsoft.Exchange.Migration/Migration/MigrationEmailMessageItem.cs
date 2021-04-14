using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationEmailMessageItem : DisposeTrackableBase, IMigrationEmailMessageItem, IMigrationAttachmentMessage, IDisposable
	{
		internal MigrationEmailMessageItem(MigrationDataProvider dataProvider, MessageItem message)
		{
			this.DataProvider = dataProvider;
			this.Message = message;
		}

		private MigrationDataProvider DataProvider { get; set; }

		private MessageItem Message { get; set; }

		public void Send(IEnumerable<SmtpAddress> recipientAddresses, string subject, string body)
		{
			MigrationUtil.ThrowOnCollectionEmptyArgument(recipientAddresses, "recipientAddresses");
			MigrationUtil.ThrowOnNullOrEmptyArgument(subject, "subject");
			MigrationUtil.ThrowOnNullOrEmptyArgument(body, "body");
			Participant participant = new Participant(this.DataProvider.ADProvider.PrimaryExchangeRecipient);
			this.Message.Sender = participant;
			this.Message.From = participant;
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
			MigrationLogger.Log(MigrationEventType.Information, "Sending report email to {0}, subject {1}", new object[]
			{
				stringBuilder,
				subject
			});
			this.Message.SendWithoutSavingMessage();
		}

		public IMigrationAttachment CreateAttachment(string name)
		{
			base.CheckDisposed();
			return MigrationMessageHelper.CreateAttachment(this.Message, name);
		}

		public bool TryGetAttachment(string name, PropertyOpenMode openMode, out IMigrationAttachment attachment)
		{
			base.CheckDisposed();
			return MigrationMessageHelper.TryGetAttachment(this.Message, name, openMode, out attachment);
		}

		public IMigrationAttachment GetAttachment(string name, PropertyOpenMode openMode)
		{
			base.CheckDisposed();
			return MigrationMessageHelper.GetAttachment(this.Message, name, openMode);
		}

		public void DeleteAttachment(string name)
		{
			base.CheckDisposed();
			MigrationMessageHelper.DeleteAttachment(this.Message, name);
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
			return DisposeTracker.Get<MigrationEmailMessageItem>(this);
		}

		private Participant CreateRecipient(string emailAddress)
		{
			ADRecipient adrecipientByProxyAddress = this.DataProvider.ADProvider.GetADRecipientByProxyAddress(emailAddress);
			if (adrecipientByProxyAddress != null)
			{
				return new Participant(adrecipientByProxyAddress);
			}
			return new Participant(emailAddress, emailAddress, "SMTP");
		}
	}
}
