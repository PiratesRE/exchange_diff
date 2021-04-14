using System;
using System.Collections;
using System.IO;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
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

		public virtual HttpContext HttpContext
		{
			get
			{
				return HttpContext.Current;
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
				return HttpContext.Current.Response.Output;
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

		internal static bool ShouldIgnoreRequest(RequestContext requestContext, IMailboxContext userContext)
		{
			return PendingRequestEventHandler.IsObsoleteRequest(requestContext, userContext);
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

		protected override void InternalDispose(bool isDisposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaEventHandlerBase>(this);
		}

		private const int InitialTableCapacity = 4;

		private const string JavascriptContentType = "application/x-javascript";

		private const string HtmlContentType = "text/html";

		private OwaEventAttribute eventInfo;

		private Hashtable parameterTable;

		private OwaEventContentType responseContentType = OwaEventContentType.Html;

		private OwaEventVerb verb;

		private bool dontWriteHeaders;

		private bool showErrorInPage;
	}
}
