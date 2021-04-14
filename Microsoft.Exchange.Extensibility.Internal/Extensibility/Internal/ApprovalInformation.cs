using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal class ApprovalInformation
	{
		public ApprovalInformation(Charset messageCharset, CultureInfo culture, string subject, string topText, string topTextFont, IEnumerable<string> details, string finalText, string finalTextFont, string bodyTextFont, IEnumerable<int> codepages)
		{
			this.Culture = culture;
			this.Details = details;
			this.FinalText = finalText;
			this.FinalTextFont = finalTextFont;
			this.BodyTextFont = bodyTextFont;
			this.MessageCharset = messageCharset;
			this.Subject = subject;
			this.Codepages = codepages;
			this.TopText = topText;
			this.TopTextFont = topTextFont;
		}

		public readonly string Subject;

		public readonly string TopText;

		public readonly string TopTextFont;

		public readonly string FinalText;

		public readonly IEnumerable<string> Details;

		public readonly string FinalTextFont;

		public readonly string BodyTextFont;

		public readonly Charset MessageCharset;

		public readonly CultureInfo Culture;

		public readonly IEnumerable<int> Codepages;

		internal enum ApprovalNotificationType
		{
			DecisionConflict,
			ApprovalRequest,
			ModeratedTransportReject,
			ModeratedTransportRejectWithComments,
			DecisionUpdate,
			ApprovalRequestExpiry,
			ExpiryNotification,
			ModeratorsNdrNotification,
			ModeratorsOofNotification,
			ModeratorExpiryNotification
		}
	}
}
