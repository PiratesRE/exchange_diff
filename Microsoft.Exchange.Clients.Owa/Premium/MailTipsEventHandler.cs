using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("MailTips")]
	internal sealed class MailTipsEventHandler : OwaEventHandlerBase
	{
		[OwaEventParameter("From", typeof(RecipientInfo), false, true)]
		[OwaEvent("Get")]
		[OwaEventParameter("Recips", typeof(RecipientInfo), true, false)]
		[OwaEventParameter("DoesNeedConfig", typeof(bool), false, true)]
		[OwaEventParameter("HideMailTipsByDefault", typeof(bool), false, true)]
		public void GetMailTips()
		{
			RecipientInfo[] array = (RecipientInfo[])base.GetParameter("Recips");
			if (array != null && 0 < array.Length)
			{
				RecipientInfo senderInfo = (RecipientInfo)base.GetParameter("From");
				object parameter = base.GetParameter("DoesNeedConfig");
				bool doesNeedConfig = parameter != null && (bool)parameter;
				base.SaveHideMailTipsByDefault();
				base.UserContext.MailTipsNotificationHandler.BeginGetMailTipsInBatches(array, senderInfo, doesNeedConfig, null, null);
			}
		}

		public const string EventNamespace = "MailTips";

		public const string MethodGet = "Get";

		public const string From = "From";

		public const string Recipients = "Recips";

		public const string DoesNeedConfig = "DoesNeedConfig";
	}
}
