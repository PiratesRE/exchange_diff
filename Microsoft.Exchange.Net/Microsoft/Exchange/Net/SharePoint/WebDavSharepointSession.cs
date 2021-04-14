using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Net.SharePoint
{
	public class WebDavSharepointSession : ISharePointSession
	{
		public WebDavSharepointSession(ICredentials webServiceCreds, bool enableHttpDebugProxy, TimeSpan requestTimeout)
		{
			if (webServiceCreds == null)
			{
				throw new ArgumentNullException("webServiceCreds");
			}
			this.webServiceCreds = webServiceCreds;
			this.enableHttpDebugProxy = enableHttpDebugProxy;
			this.requestTimeout = requestTimeout;
		}

		public WebDavSharepointSession(ICredentials webServiceCreds, bool enableHttpDebugProxy, TimeSpan requestTimeout, int heartBeatInterval, int copyStreamBufferSize, int maxFilesToListPerFolder) : this(webServiceCreds, enableHttpDebugProxy, requestTimeout)
		{
			this.heartBeatInterval = heartBeatInterval;
			this.copyStreamBufferSize = copyStreamBufferSize;
			this.maxFilesToListPerFolder = maxFilesToListPerFolder;
		}

		public static string GetDetailedWebExceptionMessage(WebException e, string reqUrl)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			HttpWebResponse httpWebResponse = e.Response as HttpWebResponse;
			return string.Format("ReqUrl:{0}; Status{1}; Message:{2}; HttpResponse.StatusCode:{3}; HttpResponse.Method:{4}; HttpResponse.ResponseUri:{5}", new object[]
			{
				reqUrl,
				e.Status,
				e.Message,
				(httpWebResponse != null) ? httpWebResponse.StatusCode.ToString() : string.Empty,
				(httpWebResponse != null) ? httpWebResponse.Method : string.Empty,
				(httpWebResponse != null) ? httpWebResponse.ResponseUri.ToString() : string.Empty
			});
		}

		public bool DoesFileExist(string fileUrl)
		{
			if (string.IsNullOrEmpty(fileUrl))
			{
				throw new ArgumentNullException("fileUrl");
			}
			WebRequest webRequest = this.CreateWebRequest(fileUrl, "HEAD");
			bool result;
			try
			{
				WebResponse response = webRequest.GetResponse();
				response.Close();
				result = true;
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
				if (httpWebResponse == null || httpWebResponse.StatusCode != HttpStatusCode.NotFound)
				{
					throw;
				}
				httpWebResponse.Close();
				result = false;
			}
			return result;
		}

		public void DeleteFile(string fileUrl)
		{
			if (string.IsNullOrEmpty(fileUrl))
			{
				throw new ArgumentNullException("fileUrl");
			}
			WebRequest webRequest = this.CreateWebRequest(fileUrl, "DELETE");
			WebResponse response = webRequest.GetResponse();
			response.Close();
		}

		public string UploadFile(string fileUrl, Stream inStream, Action heartbeat, out NameValueCollection propertyBag)
		{
			if (string.IsNullOrEmpty(fileUrl))
			{
				throw new ArgumentNullException("fileUrl");
			}
			if (inStream == null)
			{
				throw new ArgumentNullException("inStream");
			}
			WebRequest webRequest = this.CreateWebRequest(fileUrl, "PUT");
			using (Stream requestStream = webRequest.GetRequestStream())
			{
				this.CopyStream(inStream, requestStream, heartbeat);
			}
			WebResponse response = webRequest.GetResponse();
			propertyBag = response.Headers;
			string result = response.ResponseUri.ToString();
			response.Close();
			return result;
		}

		public void DownloadFile(string fileUrl, SharepointFileDownloadHelper writeStream)
		{
			if (string.IsNullOrEmpty(fileUrl))
			{
				throw new ArgumentNullException("fileUrl");
			}
			if (writeStream == null)
			{
				throw new ArgumentNullException("writeStream");
			}
			WebRequest webRequest = this.CreateWebRequest(fileUrl, "GET");
			using (WebResponse response = webRequest.GetResponse())
			{
				int num = (response.ContentLength >= 2147483647L) ? int.MaxValue : ((int)response.ContentLength);
				if (num == 2147483647)
				{
					num = int.MaxValue;
				}
				using (Stream responseStream = response.GetResponseStream())
				{
					writeStream(fileUrl, responseStream, num, response.ContentType);
				}
			}
		}

		public ICollection<SharepointFileInfo> ListFolderContents(string folderUrl)
		{
			if (string.IsNullOrEmpty(folderUrl))
			{
				throw new ArgumentNullException("folderUrl");
			}
			WebRequest webRequest = this.CreateWebRequest(folderUrl, "PROPFIND");
			webRequest.Headers.Add("Depth", "1");
			webRequest.Headers.Add(HttpRequestHeader.Translate, "f");
			SafeXmlDocument safeXmlDocument = null;
			using (WebResponse response = webRequest.GetResponse())
			{
				safeXmlDocument = new SafeXmlDocument();
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						safeXmlDocument.Load(streamReader);
					}
				}
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("D", "DAV:");
			List<SharepointFileInfo> list = new List<SharepointFileInfo>(100);
			int num = 0;
			bool flag = false;
			foreach (object obj in safeXmlDocument.SelectNodes("/D:multistatus/D:response", xmlNamespaceManager))
			{
				XmlNode node = (XmlNode)obj;
				if (num++ > this.maxFilesToListPerFolder)
				{
					break;
				}
				SharepointFileInfo sharepointFileInfo = SharepointFileInfo.ParseNode(node, xmlNamespaceManager);
				if (!flag && folderUrl.EndsWith(sharepointFileInfo.DisplayName))
				{
					flag = true;
				}
				else
				{
					list.Add(sharepointFileInfo);
				}
			}
			return list;
		}

		private WebRequest CreateWebRequest(string requestUrl, string method)
		{
			WebRequest webRequest = WebRequest.Create(requestUrl);
			webRequest.Method = method;
			if (this.requestTimeout > TimeSpan.Zero)
			{
				webRequest.Timeout = (int)this.requestTimeout.TotalMilliseconds;
			}
			if (this.enableHttpDebugProxy)
			{
				webRequest.Proxy = new WebProxy("127.0.0.1", 8888);
			}
			webRequest.Credentials = this.webServiceCreds;
			return webRequest;
		}

		private void CopyStream(Stream inStream, Stream outStream, Action heartbeat)
		{
			byte[] array = new byte[this.copyStreamBufferSize];
			long num = 0L;
			for (;;)
			{
				int num2 = inStream.Read(array, 0, array.Length);
				if (num2 <= 0)
				{
					break;
				}
				num += (long)num2;
				outStream.Write(array, 0, num2);
				if (heartbeat != null && num % (long)this.heartBeatInterval == 0L)
				{
					heartbeat();
				}
			}
		}

		private readonly ICredentials webServiceCreds;

		private readonly int heartBeatInterval = 100;

		private readonly int copyStreamBufferSize = 1024;

		private readonly int maxFilesToListPerFolder = 10000;

		private readonly bool enableHttpDebugProxy;

		private readonly TimeSpan requestTimeout;
	}
}
