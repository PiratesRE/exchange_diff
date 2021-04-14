using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class Info : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "Msg", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				if (string.Compare(queryStringParameter, "1", StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.message = Info.InfoMessage.FailedToSaveUserCulture;
					return;
				}
			}
			else
			{
				this.message = Info.InfoMessage.Unknown;
			}
		}

		protected Info.InfoMessage Message
		{
			get
			{
				return this.message;
			}
		}

		protected string UrlTitle
		{
			get
			{
				string arg = Utilities.HtmlEncode(base.OwaContext.MailboxIdentity.GetOWAMiniRecipient().DisplayName);
				return string.Format(LocalizedStrings.GetHtmlEncoded(-456269480), arg);
			}
		}

		private Info.InfoMessage message;

		protected enum InfoMessage
		{
			Unknown,
			FailedToSaveUserCulture
		}
	}
}
