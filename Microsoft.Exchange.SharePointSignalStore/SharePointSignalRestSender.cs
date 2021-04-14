using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharePointSignalStore
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SharePointSignalRestSender : ISharePointSender<string>
	{
		public SharePointSignalRestSender(ICredentials credentials, string server, ILogger logger)
		{
			this.Timeout = 100000;
			this.credentials = credentials;
			this.server = server;
			this.logger = logger;
			this.UserAgentString = "OfficeGraphSignals-EXO-SystemUsage";
		}

		public int Timeout { get; set; }

		public string UserAgentString { get; set; }

		public static string GetFormDigestResponse(HttpWebRequest requestPost, ILogger logger)
		{
			HttpWebResponse httpWebResponse = null;
			try
			{
				httpWebResponse = (HttpWebResponse)requestPost.GetResponse();
				XDocument xdocument = XDocument.Load(httpWebResponse.GetResponseStream());
				XNamespace ns = "http://schemas.microsoft.com/ado/2007/08/dataservices";
				return xdocument.Descendants(ns + "FormDigestValue").First<XElement>().Value;
			}
			catch (WebException webException)
			{
				SharePointSignalRestSender.ThrowDetailedWebException(webException, logger);
			}
			finally
			{
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
			}
			return null;
		}

		public static void CheckPostResponse(HttpWebRequest request, ILogger logger)
		{
			WebResponse webResponse = null;
			try
			{
				webResponse = request.GetResponse();
			}
			catch (WebException webException)
			{
				SharePointSignalRestSender.ThrowDetailedWebException(webException, logger);
			}
			finally
			{
				if (webResponse != null)
				{
					webResponse.Close();
				}
			}
		}

		public static void WriteBodyToRequestStream(HttpWebRequest request, string body)
		{
			using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(body);
				streamWriter.Flush();
			}
		}

		public static void ThrowDetailedWebException(WebException webException, ILogger logger)
		{
			string text = null;
			try
			{
				if (webException.Response != null)
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)webException.Response;
					logger.LogWarning("Http error code: {0}", new object[]
					{
						httpWebResponse.StatusCode
					});
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							text = streamReader.ReadToEnd();
						}
						goto IL_78;
					}
				}
				logger.LogWarning("Failed to retrieve detailed error message: HttpWebResponse not available", new object[0]);
				IL_78:;
			}
			catch (Exception ex)
			{
				logger.LogWarning("Failed to retrieve detailed error message: {0}", new object[]
				{
					ex
				});
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				logger.LogWarning("Http response: {0}", new object[]
				{
					text
				});
				throw new WebException(webException.Message + " " + text, webException);
			}
			throw webException;
		}

		public HttpWebRequest CreateDigestHttpRequest(string server, ICredentials credentials, int timeout)
		{
			Uri requestUri = new Uri(new Uri(server), "/_api/contextinfo");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.Credentials = credentials;
			httpWebRequest.Method = "POST";
			httpWebRequest.UserAgent = this.UserAgentString;
			httpWebRequest.ContentLength = 0L;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
			httpWebRequest.Timeout = timeout;
			return httpWebRequest;
		}

		public HttpWebRequest CreatePostHttpRequest(string server, string digest, string body, ICredentials credentials, int timeout)
		{
			Uri requestUri = new Uri(new Uri(server), "/_api/signalstore/signals");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.Method = "POST";
			httpWebRequest.Credentials = credentials;
			httpWebRequest.Accept = "application/json;odata=verbose";
			httpWebRequest.Headers.Add("x-requestdigest", digest);
			httpWebRequest.ContentType = "application/json;odata=verbose";
			httpWebRequest.UserAgent = this.UserAgentString;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
			httpWebRequest.ContentLength = (long)Encoding.UTF8.GetByteCount(body);
			httpWebRequest.Timeout = timeout;
			return httpWebRequest;
		}

		public void SetData(string data)
		{
			this.data = data;
		}

		public void Send()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			string digest = this.GetDigest(this.server, this.credentials);
			stopwatch.Stop();
			this.logger.LogInfo("Retrieved form digest {0} (used {1} seconds)", new object[]
			{
				digest,
				stopwatch.Elapsed.TotalSeconds
			});
			stopwatch.Restart();
			this.Post(this.server, digest, this.data, this.credentials);
			stopwatch.Stop();
			this.logger.LogInfo("Signals posted (used {0} seconds)", new object[]
			{
				stopwatch.Elapsed.TotalSeconds
			});
		}

		public string GetDigest(string server, ICredentials credentials)
		{
			HttpWebRequest requestPost = this.CreateDigestHttpRequest(server, credentials, this.Timeout);
			return SharePointSignalRestSender.GetFormDigestResponse(requestPost, this.logger);
		}

		public void Post(string server, string digest, string signals, ICredentials credentials)
		{
			HttpWebRequest request = this.CreatePostHttpRequest(server, digest, signals, credentials, this.Timeout);
			SharePointSignalRestSender.WriteBodyToRequestStream(request, signals);
			SharePointSignalRestSender.CheckPostResponse(request, this.logger);
		}

		public const int DefaultTimeout = 100000;

		private const string DefaultUserAgentString = "OfficeGraphSignals-EXO-SystemUsage";

		private readonly string server;

		private readonly ILogger logger;

		private string data;

		private ICredentials credentials;
	}
}
