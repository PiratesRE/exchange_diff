using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.SendAsVerification
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SendAsVerificationEmail : DisposeTrackableBase
	{
		internal SendAsVerificationEmail(ExchangePrincipal subscriptionExchangePrincipal, string senderAddress, PimAggregationSubscription subscription, Guid sharedSecret, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionExchangePrincipal", subscriptionExchangePrincipal);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("senderAddress", senderAddress);
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			if (!subscription.SendAsNeedsVerification)
			{
				throw new ArgumentException("subscription is not SendAs verified.  Type: " + subscription.SubscriptionType.ToString(), "subscription");
			}
			SyncUtilities.ThrowIfGuidEmpty("sharedSecret", sharedSecret);
			this.messageId = SendAsVerificationEmail.GenerateMessageId();
			this.subscriptionAddress = subscription.UserEmailAddress.ToString();
			string emailSubject = Strings.SendAsVerificationSubject;
			this.syncLogSession = syncLogSession;
			string htmlBodyContent = this.MakeHtmlContent(subscriptionExchangePrincipal, this.subscriptionAddress, subscription.SubscriptionType, subscription.SubscriptionGuid, sharedSecret);
			this.messageData = EmailGenerationUtilities.GenerateEmailMimeStream(this.messageId, Strings.SendAsVerificationSender, senderAddress, this.subscriptionAddress, emailSubject, htmlBodyContent, this.syncLogSession);
		}

		internal string MessageId
		{
			get
			{
				base.CheckDisposed();
				return this.messageId;
			}
		}

		internal string SubscriptionAddress
		{
			get
			{
				base.CheckDisposed();
				return this.subscriptionAddress;
			}
		}

		internal MemoryStream MessageData
		{
			get
			{
				base.CheckDisposed();
				return this.messageData;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.messageData.Close();
				this.messageData = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SendAsVerificationEmail>(this);
		}

		private static string GenerateMessageId()
		{
			return SyncUtilities.GenerateMessageId(string.Format(CultureInfo.InvariantCulture, "{0}-SAV", new object[]
			{
				Guid.NewGuid().ToString()
			}));
		}

		private string GenerateVerificationURLFor(ExchangePrincipal subscriptionExchangePrincipal, AggregationSubscriptionType subscriptionType, Guid subscriptionGuid, Guid sharedSecret)
		{
			SendAsVerificationUrlGenerator sendAsVerificationUrlGenerator = new SendAsVerificationUrlGenerator();
			return sendAsVerificationUrlGenerator.GenerateURLFor(subscriptionExchangePrincipal, subscriptionType, subscriptionGuid, sharedSecret, this.syncLogSession);
		}

		private string MakeHtmlContent(ExchangePrincipal subscriptionExchangePrincipal, string subscriptionAddress, AggregationSubscriptionType subscriptionType, Guid subscriptionGuid, Guid sharedSecret)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<html>").AppendLine();
			stringBuilder.Append("<body>").AppendLine();
			stringBuilder.Append(SystemMessages.BodyBlockFontTag);
			stringBuilder.Append("<p>");
			stringBuilder.Append(AntiXssEncoder.HtmlEncode(Strings.SendAsVerificationSalutation(subscriptionExchangePrincipal.MailboxInfo.DisplayName), false));
			stringBuilder.Append("</p>").AppendLine().AppendLine();
			string text = string.Format(CultureInfo.InvariantCulture, "<a href=\"mailto:{0}\">{0}</a>", new object[]
			{
				HttpUtility.HtmlEncode(subscriptionAddress)
			});
			stringBuilder.Append("<p>");
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, AntiXssEncoder.HtmlEncode(Strings.SendAsVerificationTopBlock, false), new object[]
			{
				text
			}));
			stringBuilder.Append("</p>").AppendLine().AppendLine();
			string text2 = this.GenerateVerificationURLFor(subscriptionExchangePrincipal, subscriptionType, subscriptionGuid, sharedSecret);
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<p><a href=\"{0}\">{0}</a></p>", new object[]
			{
				text2
			})).AppendLine().AppendLine();
			stringBuilder.Append("<p>");
			stringBuilder.Append(AntiXssEncoder.HtmlEncode(Strings.SendAsVerificationBottomBlock, false));
			stringBuilder.Append("</p>").AppendLine().AppendLine();
			stringBuilder.Append("<p>");
			stringBuilder.Append(AntiXssEncoder.HtmlEncode(Strings.SendAsVerificationSignatureTopPart, false));
			stringBuilder.Append("<br>").AppendLine();
			stringBuilder.Append(AntiXssEncoder.HtmlEncode(Strings.SendAsVerificationSender, false));
			stringBuilder.Append("</p>");
			stringBuilder.Append("</font>").AppendLine();
			stringBuilder.Append("</body>").AppendLine();
			stringBuilder.Append("</html>");
			return stringBuilder.ToString();
		}

		private static readonly Trace Tracer = ExTraceGlobals.SendAsTracer;

		private string messageId;

		private string subscriptionAddress;

		private MemoryStream messageData;

		private SyncLogSession syncLogSession;
	}
}
