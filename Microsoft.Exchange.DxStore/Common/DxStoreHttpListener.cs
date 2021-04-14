using System;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.DxStore;

namespace Microsoft.Exchange.DxStore.Common
{
	internal class DxStoreHttpListener
	{
		public DxStoreHttpListener(Func<HttpRequest, HttpReply> callback)
		{
			this.msgHandler = callback;
		}

		public bool StartListening(string self, string groupName, string defaultGroupName, out Exception ex)
		{
			ex = null;
			lock (this.lockObj)
			{
				if (this.initialized)
				{
					return true;
				}
				try
				{
					this.listener = new HttpListener();
					this.listener.Prefixes.Add(HttpConfiguration.FormServerUriPrefix(self, groupName));
					if (defaultGroupName != null)
					{
						this.listener.Prefixes.Add(HttpConfiguration.FormServerUriPrefix(self, defaultGroupName));
					}
					this.listener.Start();
					this.initialized = true;
				}
				catch (HttpListenerException ex2)
				{
					ex = ex2;
				}
				finally
				{
					if (!this.initialized)
					{
						this.listener.Abort();
						this.listener = null;
					}
				}
			}
			if (this.initialized)
			{
				this.listener.BeginGetContext(new AsyncCallback(this.ListenerCallback), null);
				return true;
			}
			return false;
		}

		private void ListenForAnotherRequest(object unused)
		{
			this.listener.BeginGetContext(new AsyncCallback(this.ListenerCallback), null);
		}

		private void ListenerCallback(IAsyncResult asyncContext)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ListenForAnotherRequest));
			Exception arg = null;
			try
			{
				HttpListenerContext httpListenerContext = this.listener.EndGetContext(asyncContext);
				HttpListenerRequest request = httpListenerContext.Request;
				using (HttpListenerResponse response = httpListenerContext.Response)
				{
					using (Stream inputStream = request.InputStream)
					{
						int num = (int)request.ContentLength64;
						MemoryStream memoryStream = new MemoryStream(num);
						inputStream.CopyTo(memoryStream);
						memoryStream.Position = 0L;
						HttpRequest httpRequest = DxSerializationUtil.TryDeserialize<HttpRequest>(memoryStream, out arg);
						if (httpRequest != null)
						{
							ExTraceGlobals.InstanceTracer.TraceDebug<string, int>(0L, "Listener got {0} of size {1}", httpRequest.GetType().FullName, num);
							HttpReply httpReply = null;
							try
							{
								httpReply = this.msgHandler(httpRequest);
							}
							catch (Exception ex)
							{
								ExTraceGlobals.InstanceTracer.TraceError<Exception>(0L, "Listener handler threw {0}", ex);
								httpReply = new HttpReply.ExceptionReply(ex);
							}
							if (httpReply == null)
							{
								response.ContentLength64 = 0L;
								response.Close();
								return;
							}
							using (MemoryStream memoryStream2 = DxSerializationUtil.Serialize<HttpReply>(httpReply))
							{
								response.ContentLength64 = memoryStream2.Length;
								memoryStream2.Position = 0L;
								using (Stream outputStream = response.OutputStream)
								{
									memoryStream2.CopyTo(outputStream);
								}
								ExTraceGlobals.InstanceTracer.TraceDebug<string, long>(0L, "Listener returns {0} of size {1}", httpReply.GetType().FullName, memoryStream2.Length);
								response.Close();
								return;
							}
						}
						response.StatusCode = 400;
						response.ContentLength64 = 0L;
						response.Close();
						EventLogger.LogErr("msg unhandled: {0}", new object[]
						{
							(httpRequest == null) ? "UnknownMsg" : httpRequest.GetType().FullName
						});
					}
				}
			}
			catch (Exception ex2)
			{
				arg = ex2;
				ExTraceGlobals.InstanceTracer.TraceError<Exception>(0L, "Listener caught {0}", arg);
			}
		}

		private object lockObj = new object();

		private bool initialized;

		private HttpListener listener;

		private Func<HttpRequest, HttpReply> msgHandler;
	}
}
