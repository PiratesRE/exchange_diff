using System;
using Microsoft.Exchange.Clients.Owa.Core.Transcoding;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[OwaEventNamespace("WebReady")]
	internal sealed class WebReadyFileEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(WebReadyFileEventHandler));
		}

		[OwaEventParameter("d", typeof(string))]
		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("GetFile")]
		[OwaEventParameter("X-OWA-CANARY", typeof(string))]
		[OwaEventParameter("fileName", typeof(string))]
		public void GetFile()
		{
			string text = (string)base.GetParameter("d");
			string text2 = (string)base.GetParameter("fileName");
			if (text == null || text2 == null)
			{
				throw new OwaInvalidRequestException("DocumentId or fileName does not exist");
			}
			base.DontWriteHeaders = true;
			Utilities.MakePageNoCacheNoStore(this.HttpContext.Response);
			if (text2.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
			{
				this.HttpContext.Response.ContentType = Utilities.GetContentTypeString(OwaEventContentType.Css);
			}
			else
			{
				if (!text2.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
				{
					throw new OwaInvalidRequestException("Unsupported file type");
				}
				this.HttpContext.Response.ContentType = Utilities.GetContentTypeString(OwaEventContentType.Jpeg);
			}
			try
			{
				TranscodingTaskManager.TransmitFile(base.UserContext.Key.UserContextId, text, text2, this.HttpContext.Response);
			}
			catch (TranscodingFatalFaultException innerException)
			{
				throw new OwaInvalidRequestException("The TransmitFile function fails", innerException);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WebReadyFileEventHandler>(this);
		}

		public const string EventNamespace = "WebReady";

		public const string DocumentID = "d";

		public const string FileName = "fileName";

		public const string MethodGetFile = "GetFile";
	}
}
