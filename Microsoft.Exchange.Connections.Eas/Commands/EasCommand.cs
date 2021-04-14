using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas.WBXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EasCommand<TRequest, TResponse> where TResponse : IHaveAnHttpStatus, new()
	{
		protected internal EasCommand(Command command, EasConnectionSettings easConnectionSettings)
		{
			this.Command = command;
			this.EasConnectionSettings = easConnectionSettings;
			this.ProtocolVersion = EasCommand<TRequest, TResponse>.asVersionToStringDict[this.EasConnectionSettings.EasProtocolVersion];
		}

		internal EasConnectionSettings EasConnectionSettings { get; private set; }

		internal virtual bool UseSsl
		{
			get
			{
				return true;
			}
		}

		internal string ProtocolVersion { get; private set; }

		internal Command Command { get; set; }

		internal string CommandName
		{
			get
			{
				return this.Command.ToString();
			}
		}

		internal string UriString { get; set; }

		internal HttpWebRequest HttpWebRequest { get; set; }

		protected virtual string RequestMethodName
		{
			get
			{
				return "POST";
			}
		}

		protected List<int> ExpectedHttpStatusCodes { get; set; }

		internal virtual TResponse Execute(TRequest easRequest)
		{
			TResponse result;
			try
			{
				HttpWebRequest httpWebRequest = this.CreateWebRequest(this.GetUriString());
				this.AddWebRequestHeaders(httpWebRequest);
				this.LogInfoHeaders(httpWebRequest.Headers);
				this.AddWebRequestBody(httpWebRequest, easRequest);
				result = this.SendWebRequest(httpWebRequest, easRequest);
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
				if (httpWebResponse != null)
				{
					throw new EasWebException(httpWebResponse.StatusCode.ToString(), ex);
				}
				throw new EasWebException(ex.Status.ToString(), ex);
			}
			return result;
		}

		protected static XmlDocument GetRequestXmlDocument(TRequest easRequest)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TRequest));
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8)
				{
					Formatting = Formatting.Indented
				})
				{
					xmlSerializer.Serialize(xmlTextWriter, easRequest);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					xmlDocument.Load(memoryStream);
				}
			}
			return xmlDocument;
		}

		protected virtual string GetUriString()
		{
			string local = this.EasConnectionSettings.EasEndpointSettings.Local;
			string domain = this.EasConnectionSettings.EasEndpointSettings.Domain;
			return string.Format("{0}//{1}/Microsoft-Server-ActiveSync?Cmd={2}&User={3}&DeviceId={4}&DeviceType={5}{6}", new object[]
			{
				this.UseSsl ? "https:" : "http:",
				domain,
				this.CommandName,
				local,
				this.EasConnectionSettings.DeviceId,
				this.EasConnectionSettings.DeviceType,
				string.Empty
			});
		}

		protected virtual void AddWebRequestHeaders(HttpWebRequest request)
		{
			request.Headers.Add("MS-ASProtocolVersion", this.ProtocolVersion);
			if (this.EasConnectionSettings.RequestCompression)
			{
				request.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
			}
			if (this.EasConnectionSettings.AcceptMultipart)
			{
				request.Headers.Add("MS-ASAcceptMultiPart: T");
			}
			if (!string.IsNullOrWhiteSpace(this.EasConnectionSettings.ClientLanguage))
			{
				request.Headers.Add("Accept-Language: " + this.EasConnectionSettings.ClientLanguage);
			}
		}

		protected virtual void AddWebRequestBody(HttpWebRequest webRequest, TRequest easRequest)
		{
			XmlDocument requestXmlDocument = EasCommand<TRequest, TResponse>.GetRequestXmlDocument(easRequest);
			this.LogInfoXml(requestXmlDocument);
			using (Stream requestStream = webRequest.GetRequestStream())
			{
				new WBXmlWriter(requestStream).WriteXmlDocument(requestXmlDocument);
			}
			webRequest.ContentType = "application/vnd.ms-sync.wbxml";
		}

		protected virtual TResponse ExtractResponse(HttpWebResponse webResponse)
		{
			HttpStatus statusCode;
			XmlDocument xmlDocument;
			using (Stream responseStream = webResponse.GetResponseStream())
			{
				statusCode = (HttpStatus)webResponse.StatusCode;
				if (responseStream == null || webResponse.ContentLength == 0L)
				{
					TResponse result = (default(TResponse) == null) ? Activator.CreateInstance<TResponse>() : default(TResponse);
					result.HttpStatus = statusCode;
					return result;
				}
				using (MemoryStream memoryStream = new MemoryStream())
				{
					responseStream.CopyTo(memoryStream);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using (WBXmlReader wbxmlReader = new WBXmlReader(memoryStream))
					{
						xmlDocument = wbxmlReader.ReadXmlDocument();
					}
				}
			}
			this.LogInfoXml(xmlDocument);
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				NewLineChars = Environment.NewLine,
				NewLineHandling = NewLineHandling.Entitize
			};
			TResponse result2;
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream2, settings))
				{
					xmlDocument.WriteTo(xmlWriter);
					xmlWriter.Flush();
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(TResponse));
					memoryStream2.Seek(0L, SeekOrigin.Begin);
					TResponse tresponse = (TResponse)((object)xmlSerializer.Deserialize(memoryStream2));
					tresponse.HttpStatus = statusCode;
					result2 = tresponse;
				}
			}
			return result2;
		}

		protected virtual TResponse SendWebRequest(HttpWebRequest webRequest, TRequest easRequest)
		{
			TResponse result;
			using (HttpWebResponse httpResponse = webRequest.GetHttpResponse(this.ExpectedHttpStatusCodes))
			{
				int statusCode = (int)httpResponse.StatusCode;
				this.EasConnectionSettings.Log.Info("{0} {1}", new object[]
				{
					statusCode,
					httpResponse.StatusDescription
				});
				int num = statusCode;
				if (num <= 301)
				{
					if (num == 200)
					{
						goto IL_D7;
					}
					if (num == 301)
					{
						return this.FollowRedirect(easRequest, httpResponse.Headers["Location"], statusCode);
					}
				}
				else
				{
					if (num == 451)
					{
						return this.FollowRedirect(easRequest, httpResponse.Headers["X-MS-Location"], statusCode);
					}
					if (num == 503)
					{
						this.ThrowRetryAfterException(httpResponse);
						goto IL_D7;
					}
				}
				string msg = string.Format("HTTP status code = '{0}'", httpResponse.StatusCode);
				throw new EasUnexpectedHttpStatusException(msg);
				IL_D7:
				result = this.ExtractResponse(httpResponse);
			}
			return result;
		}

		protected void ThrowRetryAfterException(HttpWebResponse webResponse)
		{
			string s = webResponse.Headers["Retry-After"];
			int num;
			if (!int.TryParse(s, out num))
			{
				num = 3600;
			}
			throw new EasRetryAfterException(TimeSpan.FromSeconds((double)num), webResponse.StatusDescription);
		}

		protected HttpWebRequest CreateWebRequest(string uriString)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uriString);
			httpWebRequest = this.ConditionWebRequest(httpWebRequest);
			this.EasConnectionSettings.Log.Info("{0} {1}", new object[]
			{
				httpWebRequest.Method,
				httpWebRequest.RequestUri
			});
			return httpWebRequest;
		}

		protected virtual HttpWebRequest ConditionWebRequest(HttpWebRequest webRequest)
		{
			webRequest.Method = this.RequestMethodName;
			webRequest.Credentials = this.EasConnectionSettings.EasEndpointSettings.NetworkCredential;
			webRequest.UserAgent = this.EasConnectionSettings.UserAgent;
			webRequest.AllowAutoRedirect = false;
			webRequest.PreAuthenticate = true;
			return webRequest;
		}

		protected void InitializeExpectedHttpStatusCodes(Type statusType)
		{
			Array values = Enum.GetValues(statusType);
			this.ExpectedHttpStatusCodes = values.Cast<int>().ToList<int>();
		}

		protected void LogInfoXml(XmlDocument xmlDocument)
		{
			if (this.EasConnectionSettings.Log.IsEnabled(LogLevel.LogInfo))
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						xmlTextWriter.Formatting = Formatting.Indented;
						xmlTextWriter.Indentation = 1;
						xmlDocument.WriteTo(xmlTextWriter);
						xmlTextWriter.Flush();
						string text = stringWriter.GetStringBuilder().ToString();
						this.EasConnectionSettings.Log.Info("{0}", new object[]
						{
							text
						});
					}
				}
			}
		}

		protected void LogInfoHeaders(WebHeaderCollection headers)
		{
			if (this.EasConnectionSettings.Log.IsEnabled(LogLevel.LogInfo))
			{
				foreach (string text in headers.AllKeys)
				{
					this.EasConnectionSettings.Log.Info("{0}: {1}", new object[]
					{
						text,
						string.Join("|", headers.GetValues(text))
					});
				}
			}
		}

		private TResponse FollowRedirect(TRequest easRequest, string newLocationUrl, int statusCode)
		{
			if (string.IsNullOrWhiteSpace(newLocationUrl))
			{
				throw new EasMissingOrBadUrlOnRedirectException();
			}
			EasEndpointSettings easEndpointSettings = this.EasConnectionSettings.EasEndpointSettings;
			Uri uri = new Uri(newLocationUrl);
			easEndpointSettings.MostRecentDomain = uri.Host;
			if (statusCode == 301 || (statusCode != 307 && statusCode == 451))
			{
				easEndpointSettings.MostRecentEndpoint.ExplicitExpiration = new DateTime?(DateTime.MaxValue);
			}
			return this.Execute(easRequest);
		}

		protected const string UriStringFormat = "{0}//{1}/Microsoft-Server-ActiveSync?Cmd={2}&User={3}&DeviceId={4}&DeviceType={5}{6}";

		protected const string HeaderVersionString = "MS-ASProtocolVersion";

		protected const string XmlContentType = "text/xml";

		protected const string WBXmlContentType = "application/vnd.ms-sync.wbxml";

		private static Dictionary<EasProtocolVersion, string> asVersionToStringDict = new Dictionary<EasProtocolVersion, string>
		{
			{
				EasProtocolVersion.Version140,
				"14.0"
			}
		};
	}
}
