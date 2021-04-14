using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class ErrorInformation
	{
		public ErrorInformation()
		{
		}

		public ErrorInformation(string message)
		{
			this.message = message;
		}

		public ErrorInformation(string message, string details)
		{
			this.message = message;
			this.messageDetails = details;
		}

		public ErrorInformation(string message, OwaEventHandlerErrorCode errorCode)
		{
			this.message = message;
			this.OwaEventHandlerErrorCode = errorCode;
		}

		public ErrorInformation(string message, string details, OwaEventHandlerErrorCode errorCode)
		{
			this.message = message;
			this.messageDetails = details;
			this.OwaEventHandlerErrorCode = errorCode;
		}

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

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
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

		public OwaEventHandlerErrorCode OwaEventHandlerErrorCode
		{
			get
			{
				return this.owaEventHandlerErrorCode;
			}
			set
			{
				this.owaEventHandlerErrorCode = value;
			}
		}

		public bool HideDebugInformation
		{
			get
			{
				return this.hideDebugInformation;
			}
			set
			{
				this.hideDebugInformation = value;
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

		public bool SendWatsonReport
		{
			get
			{
				return this.sendWatsonReport;
			}
			set
			{
				this.sendWatsonReport = value;
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

		private Exception exception;

		private string message;

		private string messageDetails;

		private OwaEventHandlerErrorCode owaEventHandlerErrorCode = OwaEventHandlerErrorCode.NotSet;

		private bool hideDebugInformation;

		private bool sendWatsonReport;

		private ThemeFileId icon = ThemeFileId.Error;

		private ThemeFileId background;

		private OwaUrl owaUrl = OwaUrl.ErrorPage;

		private bool isDetailedErrorMessageHtmlEncoded;

		private bool isErrorMessageHtmlEncoded;

		private string previousPageUrl;

		private string externalPageUrl;

		private bool showLogOffAndContinueBrowse = true;
	}
}
