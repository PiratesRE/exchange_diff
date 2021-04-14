using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ChunkedHttpResponse : DisposeTrackableBase
	{
		internal ChunkedHttpResponse(HttpContext context)
		{
			this.Response = context.Response;
			HttpUtilities.MakePageNoCacheNoStore(this.Response);
			this.evalChunksNotSupportedByXmlhttpRequest = (HttpUtilities.GetQueryStringParameter(context.Request, "ecnsq", false) == "1");
			this.browserNameQueryParamValue = HttpUtilities.GetQueryStringParameter(context.Request, "brwnm", false);
			this.userAgent = new UserAgent(context.Request.UserAgent, UserContextManager.GetUserContext(context).FeaturesManager.ClientServerSettings.ChangeLayout.Enabled, context.Request.Cookies);
			this.accountValidationContext = (context.Items["AccountValidationContext"] as IAccountValidationContext);
			this.Response.BufferOutput = false;
			this.Response.Buffer = false;
			this.Response.ContentType = "text/html; charset=UTF-8";
			this.Response.AddHeader("Transfer-Encoding", "chunked");
			if ((string.Equals("iPhone", this.userAgent.Platform) || string.Equals("iPad", this.userAgent.Platform)) && ((this.userAgent.Browser == "Safari" && this.userAgent.BrowserVersion.Build > 5) || this.browserNameQueryParamValue == "safari"))
			{
				this.Response.AddHeader("X-FromBackEnd-ClientConnection", "close");
			}
			if (!this.evalChunksNotSupportedByXmlhttpRequest)
			{
				this.Response.TrySkipIisCustomErrors = true;
			}
			this.streamWriter = PendingRequestUtilities.CreateStreamWriter(this.Response.OutputStream);
			this.WriteFirstChunk();
		}

		public IAccountValidationContext AccountValidationContext
		{
			get
			{
				return this.accountValidationContext;
			}
		}

		public bool IsClientConnected
		{
			get
			{
				return this.response.IsClientConnected;
			}
		}

		private HttpResponse Response
		{
			get
			{
				return this.response;
			}
			set
			{
				this.response = value;
			}
		}

		public void WriteIsRequestAlive(bool isAlive, long notificationMark)
		{
			if (isAlive)
			{
				this.Write(string.Format("{{id:'pg',data:'alive1',mark:'{0}'}}", notificationMark));
				this.WritePendingGetNotification("noerr");
				return;
			}
			this.WritePendingGetNotification("alive0");
		}

		public void WriteReinitializeSubscriptions()
		{
			this.WritePendingGetNotification("reinitSubscription");
		}

		public void WritePendingGeMark(long notificationMark)
		{
			this.Write(string.Format("{{id:'pg',data:'update',mark:'{0}'}}", notificationMark));
		}

		public void WriteError(string s)
		{
			this.Write(string.Format("{{id:'pg',data:'err',ex:'{0}'}}", s));
		}

		public void Write(string notificationString)
		{
			if (notificationString == null)
			{
				throw new ArgumentNullException();
			}
			if (this.evalChunksNotSupportedByXmlhttpRequest)
			{
				this.ChunkWrite(string.Format(CultureInfo.InvariantCulture, "<script>var y=parent;if(y){{var x=y.pR;if(x) x(\"{0}\");}}</script>\r\n", new object[]
				{
					PendingRequestUtilities.JavascriptEncode(notificationString)
				}), NotificationChunkOrder.Mid);
				return;
			}
			this.ChunkWrite(string.Format(CultureInfo.InvariantCulture, "<script>{0}</script>\r\n", new object[]
			{
				PendingRequestUtilities.JavascriptEscapeNonAscii(notificationString)
			}), NotificationChunkOrder.Mid);
		}

		internal void WriteEmptyNotification()
		{
			this.WritePendingGetNotification("update");
		}

		internal void WriteLastChunk()
		{
			try
			{
				this.ChunkWrite(string.Empty, NotificationChunkOrder.Last);
			}
			catch (OwaNotificationPipeWriteException ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>((long)this.GetHashCode(), "Exception when writing the last data chunk. Exception message:{0};", (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
			}
			this.streamWriter.Close();
			this.response.End();
		}

		internal void RestartRequest()
		{
			this.WritePendingGetNotification("restart");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ChunkedHttpResponse>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (!this.disposed)
			{
				if (isDisposing && this.streamWriter != null)
				{
					this.streamWriter.Dispose();
				}
				this.disposed = true;
			}
		}

		private void WritePendingGetNotification(string notification)
		{
			this.Write(string.Format("{{id:'pg',data:'{0}'}}", notification));
		}

		private void WriteFirstChunk()
		{
			this.ChunkWrite("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678 01234 567 89012 345\r\n" + ((this.browserNameQueryParamValue == "chrome") ? "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678 01234 567 89012 345\r\n" : string.Empty), NotificationChunkOrder.First);
		}

		private void ChunkWrite(string s, NotificationChunkOrder order)
		{
			Exception ex = null;
			try
			{
				if (order == NotificationChunkOrder.First)
				{
					this.streamWriter.Write("{0:x}\r\n{1}{2}", s.Length + "<script></script>\r\n<script></script>\r\n".Length, s, "<script></script>\r\n");
				}
				else if (order == NotificationChunkOrder.Last)
				{
					this.streamWriter.Write("{0}\r\n{1:x}\r\n{2}\r\n", "<script></script>\r\n", s.Length, s);
				}
				else
				{
					this.streamWriter.Write("{0}\r\n{1:x}\r\n{2}{3}", new object[]
					{
						"<script></script>\r\n",
						s.Length + "<script></script>\r\n<script></script>\r\n".Length,
						s,
						"<script></script>\r\n"
					});
				}
				this.streamWriter.Flush();
			}
			catch (ObjectDisposedException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (HttpException ex4)
			{
				ex = ex4;
			}
			catch (COMException ex5)
			{
				ex = ex5;
			}
			catch (ArgumentException ex6)
			{
				ex = ex6;
			}
			finally
			{
				if (ex != null)
				{
					throw new OwaNotificationPipeWriteException("Some exception was raised while trying to write in the response", ex);
				}
			}
		}

		internal const string EvalChunkNotSupportedQueryParamName = "ecnsq";

		internal const string BrowserNameQueryParam = "brwnm";

		private const string EvalChunkNotSupportedQueryParamValue = "1";

		private const string SafariBrowserNameQueryParamValue = "safari";

		private const string ChromeBrowserNameQueryParamValue = "chrome";

		private const string PendingGetFormat = "{{id:'pg',data:'{0}'}}";

		private const string PendingGetErrorFormat = "{{id:'pg',data:'err',ex:'{0}'}}";

		private const string PendingGetMarkFormat = "{{id:'pg',data:'update',mark:'{0}'}}";

		private const string IsRequestAliveFormat = "{{id:'pg',data:'alive1',mark:'{0}'}}";

		private const string IsRequestNotAliveFormat = "alive0";

		private const string ReinitializeSubscription = "reinitSubscription";

		private const string ClearErrorFlag = "noerr";

		private const string ChunkedStringFormat = "{0:x}\r\n{1}\r\n";

		private const string ChunkedWrapperFormatIE = "<script>var y=parent;if(y){{var x=y.pR;if(x) x(\"{0}\");}}</script>\r\n";

		private const string ChunkedWrapperFormat = "<script>{0}</script>\r\n";

		private const string CommonFirstChunkResponseFormat = "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678 01234 567 89012 345\r\n";

		private const string RestartRequestScript = "restart";

		private const string EmptyNotificationScript = "update";

		private const string HtmlContentType = "text/html; charset=UTF-8";

		private const string FullDummyResponse = "<script></script>\r\n<script></script>\r\n";

		private const string PartialDummyResponse = "<script></script>\r\n";

		private const string PrefixDummyStringFormat = "{0}\r\n";

		private const string PostfixDummyStringFormat = "{0:x}\r\n{1}";

		private const string FirstChunkStringFormat = "{0:x}\r\n{1}{2}";

		private const string MidChunkStringFormat = "{0}\r\n{1:x}\r\n{2}{3}";

		private const string LastChunkStringFormat = "{0}\r\n{1:x}\r\n{2}\r\n";

		private HttpResponse response;

		private StreamWriter streamWriter;

		private UserAgent userAgent;

		private bool disposed;

		private readonly string browserNameQueryParamValue;

		private readonly bool evalChunksNotSupportedByXmlhttpRequest;

		private readonly IAccountValidationContext accountValidationContext;
	}
}
