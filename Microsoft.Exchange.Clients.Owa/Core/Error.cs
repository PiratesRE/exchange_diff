using System;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class Error : OwaPage
	{
		public bool IsUserRtl
		{
			get
			{
				if (base.UserContext != null)
				{
					return base.UserContext.IsRtl;
				}
				return base.OwaContext.Culture != null && base.OwaContext.Culture.TextInfo.IsRightToLeft;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.errorInformation = base.OwaContext.ErrorInformation;
			if (this.errorInformation == null)
			{
				this.errorInformation = new ErrorInformation();
				this.errorInformation.Message = LocalizedStrings.GetNonEncoded(641346049);
			}
			else if (this.errorInformation.Exception != null)
			{
				Exception ex = this.errorInformation.Exception;
				if (ex is AsyncLocalizedExceptionWrapper)
				{
					ex = AsyncExceptionWrapperHelper.GetRootException(ex);
				}
				try
				{
					base.OwaContext.HttpContext.Response.Headers.Add("X-OWA-Error", ex.GetType().FullName);
				}
				catch (HttpException arg)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Exception happened while trying to append error headers. Exception will be ignored: {0}", arg);
				}
			}
			this.OnInit(e);
		}

		protected bool HasErrorDetails
		{
			get
			{
				return this.errorInformation.MessageDetails != null;
			}
		}

		protected void RenderIcon()
		{
			ThemeManager.RenderBaseThemeFileUrl(base.Response.Output, this.errorInformation.Icon, false);
		}

		protected void RenderError()
		{
			if (this.errorInformation.IsErrorMessageHtmlEncoded)
			{
				base.Response.Write(this.errorInformation.Message);
			}
			else
			{
				Utilities.HtmlEncode(this.errorInformation.Message, base.Response.Output);
			}
			if (this.IsPreviousPageLinkEnabled)
			{
				this.RenderBackLink();
			}
		}

		protected void RenderErrorDetails()
		{
			if (!this.errorInformation.IsDetailedErrorHtmlEncoded)
			{
				Utilities.HtmlEncode(this.errorInformation.MessageDetails, base.Response.Output);
				return;
			}
			base.Response.Write(this.errorInformation.MessageDetails);
		}

		protected void RenderDebugInformation()
		{
			if (!Globals.ShowDebugInformation)
			{
				return;
			}
			UserContext userContext = null;
			bool flag = false;
			try
			{
				userContext = base.OwaContext.TryGetUserContext();
				if (userContext != null)
				{
					userContext.Lock();
					flag = true;
				}
				Exception ex = this.errorInformation.Exception;
				if (ex == null)
				{
					ex = Globals.InitializationError;
				}
				Utilities.RenderDebugInformation(base.Response.Output, base.OwaContext, ex);
			}
			finally
			{
				if (userContext != null && flag)
				{
					userContext.Unlock();
				}
			}
		}

		protected bool ShowDebugInformation
		{
			get
			{
				return Globals.ShowDebugInformation && !this.errorInformation.HideDebugInformation;
			}
		}

		protected bool ShowSendReport
		{
			get
			{
				return this.ShowDebugInformation && Globals.EnableEmailReports && !base.IsDownLevelClient && base.SessionContext != null && this.errorInformation.Exception != null;
			}
		}

		protected bool IsPreviousPageLinkEnabled
		{
			get
			{
				return !string.IsNullOrEmpty(this.errorInformation.PreviousPageUrl);
			}
		}

		protected bool IsExternalLinkPresent
		{
			get
			{
				return !string.IsNullOrEmpty(this.errorInformation.ExternalPageLink);
			}
		}

		protected ErrorInformation ErrorInformation
		{
			get
			{
				return this.errorInformation;
			}
		}

		protected void RenderExternalLink()
		{
			base.Response.Write(this.errorInformation.ExternalPageLink);
		}

		protected void RenderBackLink()
		{
			base.Response.Write(string.Format(LocalizedStrings.GetHtmlEncoded(161749640), "<a href=\"" + this.errorInformation.PreviousPageUrl + "\">", "</a>"));
		}

		protected void RenderBackground()
		{
			ThemeManager.RenderBaseThemeFileUrl(base.Response.Output, this.errorInformation.Background, false);
		}

		protected bool RedirectForFailover
		{
			get
			{
				return this.ErrorInformation.OwaEventHandlerErrorCode == OwaEventHandlerErrorCode.MailboxFailoverWithRedirection;
			}
		}

		protected string ResourcePath
		{
			get
			{
				if (this.resourcePath == null)
				{
					if (Globals.OwaVDirType == OWAVDirType.Calendar)
					{
						PublishingUrl publishingUrl = (PublishingUrl)base.OwaContext.HttpContext.Items["AnonymousUserContextPublishedUrl"];
						this.resourcePath = AnonymousSessionContext.GetEscapedPathFromUri(publishingUrl.Uri) + "/";
					}
					else
					{
						this.resourcePath = OwaUrl.ApplicationRoot.ImplicitUrl;
					}
				}
				return this.resourcePath;
			}
		}

		private ErrorInformation errorInformation;

		private string resourcePath;
	}
}
