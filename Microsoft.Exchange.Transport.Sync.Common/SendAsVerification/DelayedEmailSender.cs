using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.SendAsVerification
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DelayedEmailSender : IEmailSender
	{
		public DelayedEmailSender(IEmailSender toWrap)
		{
			SyncUtilities.ThrowIfArgumentNull("toWrap", toWrap);
			this.wrappedEmailSender = toWrap;
		}

		public bool SendAttempted
		{
			get
			{
				return this.sendAttempted;
			}
		}

		public bool SendSuccessful
		{
			get
			{
				return this.SendAttempted;
			}
		}

		public string MessageId
		{
			get
			{
				return string.Empty;
			}
		}

		public void SendWith(Guid sharedSecret)
		{
			if (this.wrappedEmailSender != EmailSender.NullEmailSender)
			{
				SyncUtilities.ThrowIfGuidEmpty("sharedSecret", sharedSecret);
			}
			this.sendAttempted = true;
			this.sharedSecret = sharedSecret;
		}

		public IEmailSender TriggerDelayedSend()
		{
			if (!this.SendAttempted)
			{
				return null;
			}
			this.wrappedEmailSender.SendWith(this.sharedSecret);
			return this.wrappedEmailSender;
		}

		private IEmailSender wrappedEmailSender;

		private bool sendAttempted;

		private Guid sharedSecret;
	}
}
