using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Error")]
	internal sealed class ErrorEventHandler : OwaEventHandlerBase
	{
		[OwaEvent("SendReport")]
		[OwaEventParameter("b", typeof(string))]
		[OwaEventParameter("s", typeof(string))]
		public void SendReport()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "ErrorEventHandler.SendReport");
			this.SendEmailReport((string)base.GetParameter("b"), (string)base.GetParameter("s"));
		}

		private void SendEmailReport(string body, string subject)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), "ErrorEventHandler.SendEmailReport");
			if (!Globals.EnableEmailReports)
			{
				return;
			}
			body = string.Format("{0}\r\n--------------------------------------------------\r\n{1}", body, Utilities.GetExtraWatsonData(base.OwaContext));
			MessageItem messageItem = MessageItem.Create(base.UserContext.MailboxSession, base.UserContext.DraftsFolderId);
			messageItem.Subject = subject;
			ItemUtility.SetItemBody(messageItem, BodyFormat.TextPlain, body);
			messageItem.Recipients.Add(new Participant(null, Globals.ErrorReportAddress, "SMTP"), RecipientItemType.To);
			messageItem[ItemSchema.ConversationIndexTracking] = true;
			messageItem.Send();
		}

		public const string EventNamespace = "Error";

		public const string MethodSendReport = "SendReport";

		public const string MethodSendClientError = "ClientError";

		public const string Subject = "s";

		public const string Body = "b";

		public const string LineNumber = "ln";

		public const string Function = "fn";

		public const string ExceptionMessage = "msg";

		public const string CallStack = "cs";
	}
}
