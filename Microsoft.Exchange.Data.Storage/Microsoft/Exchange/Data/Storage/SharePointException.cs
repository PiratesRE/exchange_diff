using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class SharePointException : LocalizedException
	{
		public string RequestUrl { get; private set; }

		public string DiagnosticInfo { get; private set; }

		private static string MachineName
		{
			get
			{
				if (SharePointException.machineName == null)
				{
					SharePointException.machineName = "Not available";
					try
					{
						SharePointException.machineName = Environment.MachineName;
					}
					catch (InvalidOperationException)
					{
					}
				}
				return SharePointException.machineName;
			}
		}

		public SharePointException(string requestUrl, LocalizedString errorMessage) : base(errorMessage, null)
		{
			this.RequestUrl = requestUrl;
			this.DiagnosticInfo = errorMessage.ToString();
			this.DiagnosticInfo = string.Format("ErrorMessage:{0}, ClientMachine:{1}", errorMessage.ToString(), SharePointException.MachineName);
		}

		public SharePointException(string requestUrl, ClientRequestException e) : base(new LocalizedString(e.Message), e)
		{
			this.RequestUrl = requestUrl;
			this.DiagnosticInfo = string.Format("SharePoint ClientRequestException - ErrorMessage:{0}, ClientMachine:{1}", e.Message, SharePointException.MachineName);
		}

		public SharePointException(string requestUrl, ServerException e) : base(new LocalizedString(e.Message), e)
		{
			this.RequestUrl = requestUrl;
			this.DiagnosticInfo = string.Format("SharePoint ServerException - ErrorType:{0}; ErrorMessage:{1}; ErrorCode:{2}, ClientMachine:{3}", new object[]
			{
				e.ServerErrorTypeName,
				e.Message,
				e.ServerErrorCode,
				SharePointException.MachineName
			});
		}

		public SharePointException(string requestUrl, WebException e, bool includeResponseStream = true) : base(new LocalizedString(e.Message), e)
		{
			this.RequestUrl = requestUrl;
			HttpWebResponse httpWebResponse = e.Response as HttpWebResponse;
			this.DiagnosticInfo = string.Format("WebException - Status:{0}; Message:{1};HttpStatusCode:{2};HttpStatusDescription:{3};HttpResponseUri:{4};Server{5};ClientMachine:{6};ResponseHeaders:{7}", new object[]
			{
				e.Status,
				e.Message,
				(httpWebResponse != null) ? httpWebResponse.StatusCode.ToString() : "N/A",
				(httpWebResponse != null) ? httpWebResponse.StatusDescription : "N/A",
				(httpWebResponse != null && httpWebResponse.ResponseUri != null) ? httpWebResponse.ResponseUri.ToString() : "N/A",
				(httpWebResponse != null) ? httpWebResponse.Server : "N/A",
				SharePointException.MachineName,
				this.GetResponseHeaderString(httpWebResponse)
			}) + (includeResponseStream ? (";ResponseStream:" + this.GetResponseString(httpWebResponse)) : string.Empty);
		}

		protected SharePointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private string GetResponseHeaderString(HttpWebResponse httpResponse)
		{
			if (httpResponse == null || httpResponse.Headers == null)
			{
				return "N/A";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < httpResponse.Headers.Count; i++)
			{
				string key = httpResponse.Headers.GetKey(i);
				string value = httpResponse.Headers[key];
				stringBuilder.Append("{");
				stringBuilder.Append(key);
				stringBuilder.Append(":");
				stringBuilder.Append(value);
				stringBuilder.Append("}");
			}
			return stringBuilder.ToString();
		}

		private string GetResponseString(HttpWebResponse httpResponse)
		{
			if (httpResponse != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				try
				{
					using (Stream responseStream = httpResponse.GetResponseStream())
					{
						Encoding encoding = Encoding.GetEncoding("utf-8");
						using (StreamReader streamReader = new StreamReader(responseStream, encoding))
						{
							char[] array = new char[256];
							for (;;)
							{
								int num = streamReader.Read(array, 0, array.Length);
								if (num <= 0)
								{
									break;
								}
								stringBuilder.Append(array, 0, num);
							}
						}
					}
				}
				catch (ProtocolViolationException)
				{
					return string.Empty;
				}
				catch (IOException)
				{
					return string.Empty;
				}
				return stringBuilder.ToString();
			}
			return "N/A";
		}

		private static volatile string machineName;
	}
}
