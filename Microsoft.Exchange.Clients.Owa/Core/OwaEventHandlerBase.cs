using System;
using System.Collections;
using System.IO;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class OwaEventHandlerBase : DisposeTrackableBase
	{
		public static bool IsReusable
		{
			get
			{
				return false;
			}
		}

		internal static bool ShouldIgnoreRequest(OwaContext owaContext, UserContext userContext)
		{
			return PendingRequestEventHandler.IsObsoleteRequest(owaContext, userContext);
		}

		public OwaEventAttribute EventInfo
		{
			get
			{
				return this.eventInfo;
			}
			internal set
			{
				this.eventInfo = value;
			}
		}

		public OwaContext OwaContext
		{
			get
			{
				return this.owaContext;
			}
			internal set
			{
				this.owaContext = value;
			}
		}

		public UserContext UserContext
		{
			get
			{
				return this.OwaContext.UserContext;
			}
			set
			{
				this.OwaContext.UserContext = value;
			}
		}

		public ISessionContext SessionContext
		{
			get
			{
				return this.OwaContext.SessionContext;
			}
		}

		public virtual HttpContext HttpContext
		{
			get
			{
				return this.OwaContext.HttpContext;
			}
		}

		public OwaEventVerb Verb
		{
			get
			{
				return this.verb;
			}
			internal set
			{
				this.verb = value;
			}
		}

		public OwaEventContentType ResponseContentType
		{
			get
			{
				return this.responseContentType;
			}
			set
			{
				this.responseContentType = value;
			}
		}

		public virtual TextWriter Writer
		{
			get
			{
				return this.owaContext.HttpContext.Response.Output;
			}
		}

		public virtual TextWriter SanitizingWriter
		{
			get
			{
				return this.owaContext.SanitizingResponseWriter;
			}
		}

		public bool DontWriteHeaders
		{
			get
			{
				return this.dontWriteHeaders;
			}
			set
			{
				this.dontWriteHeaders = value;
			}
		}

		public bool ShowErrorInPage
		{
			get
			{
				return this.showErrorInPage;
			}
			set
			{
				this.showErrorInPage = value;
			}
		}

		internal void SetParameterTable(Hashtable parameterTable)
		{
			this.parameterTable = parameterTable;
		}

		internal object GetParameter(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.parameterTable[name];
		}

		protected bool IsParameterSet(string name)
		{
			return null != this.GetParameter(name);
		}

		protected void RenderPartialFailure(Strings.IDs messageString)
		{
			this.RenderPartialFailure(messageString, OwaEventHandlerErrorCode.NotSet);
		}

		protected void RenderPartialFailure(Strings.IDs messageString, OwaEventHandlerErrorCode errorCode)
		{
			this.RenderPartialFailure(messageString, null, ButtonDialogIcon.NotSet, errorCode);
		}

		protected void RenderPartialFailure(Strings.IDs messageString, Strings.IDs? titleString, ButtonDialogIcon icon)
		{
			this.RenderPartialFailure(messageString, titleString, icon, OwaEventHandlerErrorCode.NotSet);
		}

		protected void RenderPartialFailure(Strings.IDs messageString, Strings.IDs? titleString, ButtonDialogIcon icon, OwaEventHandlerErrorCode errorCode)
		{
			this.RenderPartialFailure(LocalizedStrings.GetHtmlEncoded(messageString), (titleString != null) ? LocalizedStrings.GetHtmlEncoded(titleString.Value) : null, icon, errorCode);
		}

		protected void RenderPartialFailure(string messageHtml, string titleHtml, ButtonDialogIcon icon, OwaEventHandlerErrorCode errorCode)
		{
			if (messageHtml == null)
			{
				throw new ArgumentNullException("message");
			}
			this.Writer.Write("<div id=err _msg=\"");
			this.Writer.Write(messageHtml);
			this.Writer.Write("\"");
			if (errorCode != OwaEventHandlerErrorCode.NotSet)
			{
				this.Writer.Write(" _cd=");
				this.Writer.Write((int)errorCode);
			}
			if (titleHtml != null)
			{
				this.Writer.Write(" _ttl=\"");
				this.Writer.Write(titleHtml);
				this.Writer.Write("\"");
			}
			if (icon != ButtonDialogIcon.NotSet)
			{
				this.Writer.Write(" _icn=\"");
				this.Writer.Write((int)icon);
				this.Writer.Write("\"");
			}
			this.Writer.Write("></div>");
		}

		protected override void InternalDispose(bool isDisposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaEventHandlerBase>(this);
		}

		protected void ThrowIfCannotActAsOwner()
		{
			if (!this.UserContext.CanActAsOwner)
			{
				throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(1622692336), true);
			}
		}

		protected void SaveHideMailTipsByDefault()
		{
			if (this.IsParameterSet("HideMailTipsByDefault"))
			{
				bool flag = (bool)this.GetParameter("HideMailTipsByDefault");
				if (flag != this.UserContext.UserOptions.HideMailTipsByDefault)
				{
					this.UserContext.UserOptions.HideMailTipsByDefault = flag;
					this.UserContext.UserOptions.CommitChanges();
				}
			}
		}

		protected void WriteNewItemId(Item item)
		{
			this.SanitizingWriter.Write("<div id=");
			this.SanitizingWriter.Write("itemId");
			this.SanitizingWriter.Write(">");
			this.SanitizingWriter.Write(item.Id.ObjectId.ToBase64String());
			this.SanitizingWriter.Write("</div>");
		}

		protected void WriteChangeKey(Item item)
		{
			this.SanitizingWriter.Write("<div id=");
			this.SanitizingWriter.Write("ck");
			this.SanitizingWriter.Write(">");
			this.SanitizingWriter.Write(item.Id.ChangeKeyAsBase64String());
			this.SanitizingWriter.Write("</div>");
		}

		protected void WriteIdAndChangeKey(Item item, bool existingItem)
		{
			item.Load();
			if (!existingItem)
			{
				this.WriteNewItemId(item);
			}
			this.WriteChangeKey(item);
		}

		private const int InitialTableCapacity = 4;

		private const string JavascriptContentType = "application/x-javascript";

		private const string HtmlContentType = "text/html";

		public const string HideMailTipsByDefault = "HideMailTipsByDefault";

		protected const string ItemIdKey = "itemId";

		protected const string ChangeKeyKey = "ck";

		private OwaContext owaContext;

		private OwaEventAttribute eventInfo;

		private Hashtable parameterTable;

		private OwaEventContentType responseContentType = OwaEventContentType.Html;

		private OwaEventVerb verb;

		private bool dontWriteHeaders;

		private bool showErrorInPage;
	}
}
