using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.DxStore;

namespace Microsoft.Exchange.DxStore.Common
{
	internal class HttpStoreAccessClient : IDxStoreAccessClient
	{
		public HttpStoreAccessClient(string self, HttpClient.TargetInfo targetInfo, int timeoutInMsec)
		{
			this.TargetInfo = targetInfo;
			this.Self = self;
			this.TimeoutInMsec = timeoutInMsec;
		}

		public HttpClient.TargetInfo TargetInfo { get; private set; }

		public int TimeoutInMsec { get; set; }

		private string Self { get; set; }

		public void SetTimeout(TimeSpan timeout)
		{
			this.TimeoutInMsec = (int)timeout.TotalMilliseconds;
		}

		public DxStoreAccessReply.CheckKey CheckKey(DxStoreAccessRequest.CheckKey request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.CheckKey, DxStoreAccessReply.CheckKey>(request, timeout);
		}

		public DxStoreAccessReply.DeleteKey DeleteKey(DxStoreAccessRequest.DeleteKey request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.DeleteKey, DxStoreAccessReply.DeleteKey>(request, timeout);
		}

		public DxStoreAccessReply.SetProperty SetProperty(DxStoreAccessRequest.SetProperty request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.SetProperty, DxStoreAccessReply.SetProperty>(request, timeout);
		}

		public DxStoreAccessReply.DeleteProperty DeleteProperty(DxStoreAccessRequest.DeleteProperty request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.DeleteProperty, DxStoreAccessReply.DeleteProperty>(request, timeout);
		}

		public DxStoreAccessReply.ExecuteBatch ExecuteBatch(DxStoreAccessRequest.ExecuteBatch request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.ExecuteBatch, DxStoreAccessReply.ExecuteBatch>(request, timeout);
		}

		public DxStoreAccessReply.GetProperty GetProperty(DxStoreAccessRequest.GetProperty request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.GetProperty, DxStoreAccessReply.GetProperty>(request, timeout);
		}

		public DxStoreAccessReply.GetAllProperties GetAllProperties(DxStoreAccessRequest.GetAllProperties request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.GetAllProperties, DxStoreAccessReply.GetAllProperties>(request, timeout);
		}

		public DxStoreAccessReply.GetPropertyNames GetPropertyNames(DxStoreAccessRequest.GetPropertyNames request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.GetPropertyNames, DxStoreAccessReply.GetPropertyNames>(request, timeout);
		}

		public DxStoreAccessReply.GetSubkeyNames GetSubkeyNames(DxStoreAccessRequest.GetSubkeyNames request, TimeSpan? timeout = null)
		{
			return this.Execute<DxStoreAccessRequest.GetSubkeyNames, DxStoreAccessReply.GetSubkeyNames>(request, timeout);
		}

		private TReply Execute<TRequest, TReply>(TRequest request, TimeSpan? timeout = null) where TRequest : DxStoreRequestBase where TReply : DxStoreReplyBase
		{
			string text = null;
			TReply result;
			try
			{
				text = HttpConfiguration.FormClientUriPrefix(this.TargetInfo.TargetHost, this.TargetInfo.TargetNode, this.TargetInfo.GroupName);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
				if (timeout != null)
				{
					httpWebRequest.Timeout = (int)timeout.Value.TotalMilliseconds;
				}
				else
				{
					httpWebRequest.Timeout = this.TimeoutInMsec;
				}
				httpWebRequest.Method = "PUT";
				httpWebRequest.ContentType = "application/octet-stream";
				HttpRequest.DxStoreRequest msg = new HttpRequest.DxStoreRequest(this.Self, request);
				MemoryStream memoryStream = DxSerializationUtil.SerializeMessage(msg);
				httpWebRequest.ContentLength = memoryStream.Length;
				memoryStream.Position = 0L;
				Stream requestStream = httpWebRequest.GetRequestStream();
				using (requestStream)
				{
					memoryStream.CopyTo(requestStream);
				}
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						HttpReply httpReply = DxSerializationUtil.Deserialize<HttpReply>(responseStream);
						HttpReply.DxStoreReply dxStoreReply = httpReply as HttpReply.DxStoreReply;
						if (dxStoreReply != null)
						{
							TReply treply = dxStoreReply.Reply as TReply;
							if (treply == null)
							{
								throw new DxStoreAccessClientException(string.Format("Unexpected DxStoreReply {0}", dxStoreReply.Reply.GetType().FullName));
							}
							result = treply;
						}
						else
						{
							HttpReply.ExceptionReply exceptionReply = httpReply as HttpReply.ExceptionReply;
							if (exceptionReply != null)
							{
								Exception exception = exceptionReply.Exception;
								throw new DxStoreServerException(exception.Message, exception);
							}
							throw new DxStoreServerException(string.Format("unexpected reply: {0}", httpReply.GetType().FullName));
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.AccessClientTracer.TraceError<string, string>(0L, "HttpSend failed. Uri={0} Req={1} Ex={2}", text, request.GetType().FullName);
				if (ex is DxStoreAccessClientException || ex is DxStoreServerException)
				{
					throw;
				}
				throw new DxStoreAccessClientException(ex.Message, ex);
			}
			return result;
		}
	}
}
