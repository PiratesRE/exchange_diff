using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("perf")]
	internal sealed class PerformanceConsoleEventHandler : ItemEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(PerformanceConsoleEventHandler));
		}

		[OwaEvent("strt")]
		public void StartPerfConsole()
		{
			base.ResponseContentType = OwaEventContentType.Javascript;
			OwaContext.Current.UserContext.IsPerformanceConsoleOn = true;
		}

		[OwaEvent("stop")]
		public void StopPerfConsole()
		{
			base.ResponseContentType = OwaEventContentType.Javascript;
			OwaContext.Current.UserContext.IsPerformanceConsoleOn = false;
		}

		[OwaEvent("reportPerf")]
		[OwaEventParameter("b", typeof(string))]
		public void ReportPerformanceExperience()
		{
			base.ResponseContentType = OwaEventContentType.Javascript;
			string text = (string)base.GetParameter("s");
			MessageItem messageItem = MessageItem.Create(base.UserContext.MailboxSession, base.UserContext.DraftsFolderId);
			messageItem[ItemSchema.ConversationIndexTracking] = true;
			Markup markup = Markup.Html;
			BodyConversionUtilities.SetBody(messageItem, (string)base.GetParameter("b"), markup, StoreObjectType.Message, base.UserContext);
			messageItem.Recipients.Add(new Participant(null, Globals.ErrorReportAddress, "SMTP"), RecipientItemType.To);
			messageItem.Subject = string.Concat(new string[]
			{
				"Performance Report on ",
				DateTime.UtcNow.ToLocalTime().ToShortDateString(),
				" ",
				DateTime.UtcNow.ToLocalTime().ToShortTimeString(),
				" for ",
				base.OwaContext.UserContext.ExchangePrincipal.MailboxInfo.DisplayName
			});
			messageItem.Save(SaveMode.ResolveConflicts);
			messageItem.Load();
			this.Writer.Write(messageItem.Id.ObjectId.ToBase64String());
		}

		public const string EventNamespace = "perf";

		public const string MethodReportExperience = "reportPerf";

		public const string MethodStart = "strt";

		public const string MethodStop = "stop";

		public const string Body = "b";

		public const string Subject = "s";
	}
}
