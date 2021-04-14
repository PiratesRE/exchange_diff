using System;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class ErrorInformation
	{
		public ErrorInformation()
		{
		}

		public ErrorInformation(int httpCode)
		{
			this.httpCode = httpCode;
		}

		public ErrorInformation(int httpCode, string details)
		{
			this.httpCode = httpCode;
			this.messageDetails = details;
		}

		public ErrorInformation(int httpCode, string details, bool sharePointApp)
		{
			this.httpCode = httpCode;
			this.messageDetails = details;
			this.SharePointApp = sharePointApp;
		}

		public ErrorMode? ErrorMode { get; set; }

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		public int HttpCode
		{
			get
			{
				return this.httpCode;
			}
			set
			{
				this.httpCode = value;
			}
		}

		public bool IsErrorMessageHtmlEncoded
		{
			get
			{
				return this.isErrorMessageHtmlEncoded;
			}
			set
			{
				this.isErrorMessageHtmlEncoded = value;
			}
		}

		public string MessageDetails
		{
			get
			{
				return this.messageDetails;
			}
			set
			{
				this.messageDetails = value;
			}
		}

		public bool IsDetailedErrorHtmlEncoded
		{
			get
			{
				return this.isDetailedErrorMessageHtmlEncoded;
			}
			set
			{
				this.isDetailedErrorMessageHtmlEncoded = value;
			}
		}

		public ThemeFileId Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;
			}
		}

		public ThemeFileId Background
		{
			get
			{
				return this.background;
			}
			set
			{
				this.background = value;
			}
		}

		public OwaUrl OwaUrl
		{
			get
			{
				return this.owaUrl;
			}
			set
			{
				this.owaUrl = value;
			}
		}

		public string PreviousPageUrl
		{
			get
			{
				return this.previousPageUrl;
			}
			set
			{
				this.previousPageUrl = value;
			}
		}

		public string ExternalPageLink
		{
			get
			{
				return this.externalPageUrl;
			}
			set
			{
				this.externalPageUrl = value;
			}
		}

		public bool ShowLogoffAndWorkButton
		{
			get
			{
				return this.showLogOffAndContinueBrowse;
			}
			set
			{
				this.showLogOffAndContinueBrowse = value;
			}
		}

		public bool SharePointApp
		{
			get
			{
				return this.sharePointApp;
			}
			set
			{
				this.sharePointApp = value;
			}
		}

		public bool SiteMailbox
		{
			get
			{
				return this.siteMailbox;
			}
			set
			{
				this.siteMailbox = value;
			}
		}

		public bool GroupMailbox
		{
			get
			{
				return !string.IsNullOrEmpty(this.groupMailboxDestination);
			}
		}

		public string GroupMailboxDestination
		{
			get
			{
				return this.groupMailboxDestination;
			}
			set
			{
				this.groupMailboxDestination = value;
			}
		}

		public string RedirectionUrl { get; set; }

		private Exception exception;

		private int httpCode;

		private string messageDetails;

		private ThemeFileId icon = ThemeFileId.Error;

		private ThemeFileId background;

		private OwaUrl owaUrl = OwaUrl.ErrorPage;

		private bool isDetailedErrorMessageHtmlEncoded;

		private bool isErrorMessageHtmlEncoded;

		private string previousPageUrl;

		private string externalPageUrl;

		private bool showLogOffAndContinueBrowse = true;

		private bool sharePointApp;

		private bool siteMailbox;

		private string groupMailboxDestination;
	}
}
