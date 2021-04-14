using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ChunkedHttpResponse
	{
		internal ChunkedHttpResponse(HttpContext context)
		{
			this.Response = context.Response;
			Utilities.MakePageNoCacheNoStore(this.Response);
			this.browserType = Utilities.GetBrowserType(context.Request.UserAgent);
			this.Response.BufferOutput = false;
			this.Response.Buffer = false;
			this.Response.ContentType = "text/html; charset=UTF-8";
			this.Response.AddHeader("Transfer-Encoding", "chunked");
			this.streamWriter = Utilities.CreateStreamWriter(this.Response.OutputStream);
			this.ChunkWrite(((this.browserType == BrowserType.IE) ? "<script>try{{document.domain=document.domain;}}catch(e){{}}</script>" : string.Empty) + "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678 01234 567 89012 345\r\n" + ((this.browserType == BrowserType.Chrome) ? "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678 01234 567 89012 345\r\n" : string.Empty));
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

		public bool IsClientConnected
		{
			get
			{
				return this.response.IsClientConnected;
			}
		}

		public void WriteIsRequestAlive(bool isAlive)
		{
			this.Write(string.Format(CultureInfo.InvariantCulture, "a_oPndGt.pndGtMgr.fRqAlive = {0};", new object[]
			{
				isAlive ? "1" : "0"
			}));
			if (isAlive)
			{
				this.Write("a_oPndGt.pndGtMgr.fErrLstPndGt = 0;");
			}
		}

		public void WriteReInitializeOWA()
		{
			this.Write("Owa.Utility.ReInitializeOWA();");
		}

		public void WriteError(string s)
		{
			this.ChunkWrite(string.Format(CultureInfo.InvariantCulture, (this.browserType == BrowserType.IE) ? "<script>var y=parent;if(y){{var x=y.pdnRsp;if(x) x(\"{0}\");}}</script>\r\n" : "<script>window.evlRsp(\"{0}\");</script>\r\n", new object[]
			{
				Utilities.JavascriptEncode("a_oPndGt.pndGtMgr.errorDiv='" + s + "';", true)
			}));
			this.Write("a_oPndGt.pndGtMgr.hndErrPndGt(1);");
		}

		public void Write(string notificationString)
		{
			if (notificationString == null)
			{
				throw new ArgumentNullException();
			}
			this.ChunkWrite(string.Format(CultureInfo.InvariantCulture, (this.browserType == BrowserType.IE) ? "<script>var y=parent;if(y){{var x=y.pdnRsp;if(x) x(\"{0}\");}}</script>\r\n" : "<script>window.evlRsp(\"{0}\");</script>\r\n", new object[]
			{
				Utilities.JavascriptEncode(notificationString, true)
			}));
		}

		public void Log(RequestLogger requestLogger)
		{
			if (!this.hasEnded)
			{
				try
				{
					requestLogger.LogToResponse(this.response);
				}
				catch (Exception ex)
				{
					ExTraceGlobals.CoreTracer.TraceError((long)this.GetHashCode(), "Exception caught while logging. Log data:{0}{1}. Exception:{2}{3}", new object[]
					{
						Environment.NewLine,
						requestLogger.LogData,
						Environment.NewLine,
						ex
					});
				}
			}
		}

		internal void WriteEmptyNotification()
		{
			this.Write("a_oPndGt.pndGtMgr.updTmStmp();");
		}

		internal void WriteLastChunk()
		{
			try
			{
				this.ChunkWrite(string.Empty);
			}
			catch (OwaNotificationPipeWriteException ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>((long)this.GetHashCode(), "Exception when writing the last data chunk. Exception message:{0};", (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
			}
			this.hasEnded = true;
			this.streamWriter.Close();
			this.response.End();
		}

		internal void RestartRequest()
		{
			this.Write("rstPndRq();");
		}

		private void ChunkWrite(string s)
		{
			Exception ex = null;
			try
			{
				this.streamWriter.Write("{0:x}\r\n{1}\r\n", s.Length, s);
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

		private const string IsRequestAliveFormat = "a_oPndGt.pndGtMgr.fRqAlive = {0};";

		private const string ClearErrorFlag = "a_oPndGt.pndGtMgr.fErrLstPndGt = 0;";

		private const string ChunkedStringFormat = "{0:x}\r\n{1}\r\n";

		private const string ChunkedWrapperFormatIE = "<script>var y=parent;if(y){{var x=y.pdnRsp;if(x) x(\"{0}\");}}</script>\r\n";

		private const string ChunkedWrapperFormat = "<script>window.evlRsp(\"{0}\");</script>\r\n";

		private const string IEFirstChunkResponseFormat = "<script>try{{document.domain=document.domain;}}catch(e){{}}</script>";

		private const string CommonFirstChunkResponseFormat = "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678 01234 567 89012 345\r\n";

		private const string RestartRequestScript = "rstPndRq();";

		private const string EmptyNotificationScript = "a_oPndGt.pndGtMgr.updTmStmp();";

		private const string HandleErrorScript = "a_oPndGt.pndGtMgr.hndErrPndGt(1);";

		private const string HtmlContentType = "text/html; charset=UTF-8";

		private HttpResponse response;

		private StreamWriter streamWriter;

		private BrowserType browserType;

		private bool hasEnded;
	}
}
