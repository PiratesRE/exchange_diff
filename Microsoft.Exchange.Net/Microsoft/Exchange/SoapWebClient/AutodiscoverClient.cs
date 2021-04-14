using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.SoapWebClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AutodiscoverClient : IDisposable
	{
		public void Dispose()
		{
			this.binding.Dispose();
		}

		public StringList AllowedHostnames
		{
			get
			{
				return this.allowedHostnames;
			}
		}

		public SoapHttpClientAuthenticator Authenticator
		{
			get
			{
				return this.binding.Authenticator;
			}
			set
			{
				this.binding.Authenticator = value;
			}
		}

		public string UserAgent
		{
			get
			{
				return this.binding.UserAgent;
			}
			set
			{
				this.binding.UserAgent = value;
			}
		}

		public string AnchorMailbox
		{
			get
			{
				return this.binding.HttpHeaders[WellKnownHeader.AnchorMailbox];
			}
			set
			{
				this.binding.HttpHeaders[WellKnownHeader.AnchorMailbox] = value;
			}
		}

		public IWebProxy Proxy { get; set; }

		public RequestedServerVersion RequestedServerVersion
		{
			get
			{
				return this.binding.RequestedServerVersionValue;
			}
			set
			{
				this.binding.RequestedServerVersionValue = value;
			}
		}

		public AutodiscoverClient()
		{
			this.binding = new DefaultBinding_Autodiscover("AutodiscoverClient", new RemoteCertificateValidationCallback(AutodiscoverClient.InvalidCertificateHandler));
			this.binding.UserAgent = "AutodiscoverClient";
			this.binding.Timeout = 30000;
			this.binding.KeepAlive = new bool?(false);
			this.binding.AllowAutoRedirect = false;
		}

		public IEnumerable<AutodiscoverResultData> InvokeWithDiscovery(InvokeDelegate invokeDelegate, string domain)
		{
			string autodiscoverDomain = string.Format("autodiscover.{0}", domain);
			AutodiscoverClient.AutoDiscoverStep[] array = new AutodiscoverClient.AutoDiscoverStep[]
			{
				() => this.InvokeForHost(invokeDelegate, autodiscoverDomain),
				() => this.InvokeForHost(invokeDelegate, domain),
				() => this.InvokeForUnsecureRedirect(invokeDelegate, autodiscoverDomain),
				() => this.InvokeForUnsecureRedirect(invokeDelegate, domain)
			};
			Stopwatch stopwatch = Stopwatch.StartNew();
			IEnumerable<AutodiscoverResultData> result;
			try
			{
				List<AutodiscoverResultData> list = new List<AutodiscoverResultData>(4);
				foreach (AutodiscoverClient.AutoDiscoverStep autoDiscoverStep in array)
				{
					AutodiscoverResultData autodiscoverResultData = autoDiscoverStep();
					list.Add(autodiscoverResultData);
					if (autodiscoverResultData.Type == AutodiscoverResult.Success)
					{
						break;
					}
				}
				result = list;
			}
			finally
			{
				AutodiscoverClient.Tracer.TraceDebug<long>((long)this.GetHashCode(), "Discovery time: {0}ms", stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		public AutodiscoverResultData InvokeWithEndpoint(InvokeDelegate invokeDelegate, Uri url)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			AutodiscoverResultData result;
			try
			{
				result = this.InvokeForUrl(invokeDelegate, url);
			}
			finally
			{
				AutodiscoverClient.Tracer.TraceDebug<long>((long)this.GetHashCode(), "Discovery time: {0}ms", stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		private AutodiscoverResultData InvokeForHost(InvokeDelegate invokeDelegate, string host)
		{
			UriBuilder uriBuilder = new UriBuilder
			{
				Scheme = Uri.UriSchemeHttps,
				Host = host,
				Path = "/autodiscover/autodiscover.svc"
			};
			return this.InvokeForUrl(invokeDelegate, uriBuilder.Uri);
		}

		private AutodiscoverResultData InvokeForUrl(InvokeDelegate invokeDelegate, Uri url)
		{
			AutodiscoverClient.Tracer.TraceDebug<Uri>((long)this.GetHashCode(), "Sending autodiscover request to {0}", url);
			AutodiscoverClient.staticInvalidHostnames = null;
			Exception ex = null;
			AutodiscoverResultData autodiscoverResultData = null;
			StringList stringList;
			try
			{
				autodiscoverResultData = this.InvokeAndFollowSecureRedirects(invokeDelegate, url);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (WebException ex3)
			{
				ex = ex3;
			}
			catch (SoapException ex4)
			{
				ex = ex4;
			}
			catch (InvalidOperationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				stringList = AutodiscoverClient.staticInvalidHostnames;
				AutodiscoverClient.staticInvalidHostnames = null;
			}
			if (ex != null)
			{
				AutodiscoverClient.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Failed to make autodiscover request due exception {0}", ex);
				return new AutodiscoverResultData
				{
					Type = AutodiscoverResult.Failure,
					Url = url,
					Exception = ex
				};
			}
			if (stringList != null && !this.IsAllowedHostname(stringList))
			{
				AutodiscoverClient.Tracer.TraceError((long)this.GetHashCode(), "Autodiscover request had SSL certificate hostname mismatch");
				return new AutodiscoverResultData
				{
					Type = AutodiscoverResult.InvalidSslHostname,
					Url = url,
					SslCertificateHostnames = stringList,
					Alternate = autodiscoverResultData
				};
			}
			return autodiscoverResultData;
		}

		private AutodiscoverResultData InvokeAndFollowSecureRedirects(InvokeDelegate invokeDelegate, Uri url)
		{
			Uri uri = url;
			int num = 0;
			AutodiscoverResultData result;
			for (;;)
			{
				this.binding.Url = uri.ToString();
				try
				{
					AutodiscoverResponse response = null;
					this.InvokeWithWebProxy(uri.ToString(), delegate(IWebProxy webProxy)
					{
						this.binding.Proxy = webProxy;
						response = invokeDelegate(this.binding);
					});
					if (response == null)
					{
						AutodiscoverClient.Tracer.TraceError((long)this.GetHashCode(), "Response is empty");
						result = new AutodiscoverResultData
						{
							Type = AutodiscoverResult.Failure,
							Url = uri,
							Exception = new AutodiscoverClientException(NetException.EmptyResponse)
						};
					}
					else
					{
						result = new AutodiscoverResultData
						{
							Type = AutodiscoverResult.Success,
							Url = uri,
							Response = response
						};
					}
				}
				catch (WebException ex)
				{
					HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse == null)
					{
						AutodiscoverClient.Tracer.TraceError<string>((long)this.GetHashCode(), "WebException doesn't contain HttpWebResponse: {0}", (ex.Response == null) ? "<null>" : ex.Response.ToString());
						throw;
					}
					if (httpWebResponse.StatusCode != HttpStatusCode.Found)
					{
						AutodiscoverClient.Tracer.TraceError<HttpStatusCode>((long)this.GetHashCode(), "The StatusCode in WebException is not an redirect: {0}", httpWebResponse.StatusCode);
						throw;
					}
					num++;
					if (num > 5)
					{
						AutodiscoverClient.Tracer.TraceError<int>((long)this.GetHashCode(), "Stopped following redirects because it exceeded maximum {0}", 5);
						throw;
					}
					string text = httpWebResponse.Headers[HttpResponseHeader.Location];
					if (!Uri.IsWellFormedUriString(text, UriKind.Absolute))
					{
						AutodiscoverClient.Tracer.TraceError<string>((long)this.GetHashCode(), "Not a valid redirect URL: {0}", text);
						throw;
					}
					Uri uri2 = new Uri(text, UriKind.Absolute);
					if (uri2.Scheme != Uri.UriSchemeHttps)
					{
						AutodiscoverClient.Tracer.TraceError<string>((long)this.GetHashCode(), "Not a secure redirect URL: {0}", text);
						throw;
					}
					string path;
					if (!EwsWsSecurityUrl.IsWsSecurity(url.AbsoluteUri))
					{
						path = "/autodiscover/autodiscover.svc";
					}
					else
					{
						path = EwsWsSecurityUrl.Fix("/autodiscover/autodiscover.svc");
					}
					UriBuilder uriBuilder = new UriBuilder
					{
						Scheme = Uri.UriSchemeHttps,
						Host = uri2.Host,
						Path = path
					};
					AutodiscoverClient.Tracer.TraceDebug<Uri, Uri>((long)this.GetHashCode(), "Handling secure redirect from URL {0} to URL {1}", uri, uriBuilder.Uri);
					uri = uriBuilder.Uri;
					continue;
				}
				break;
			}
			return result;
		}

		private static bool InvalidCertificateHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				AutodiscoverClient.Tracer.TraceError<SslPolicyErrors>(0L, "SSL certificate errors: {0}", sslPolicyErrors);
			}
			if (SslConfiguration.AllowExternalUntrustedCerts)
			{
				AutodiscoverClient.Tracer.TraceDebug(0L, "Configuration tells to ignore certificate errors from external source");
				return true;
			}
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				AutodiscoverClient.Tracer.TraceDebug(0L, "SSL certificate has hostname mismatch");
				AutodiscoverClient.staticInvalidHostnames = AutodiscoverClient.GetCertificateHostnames(certificate);
				return true;
			}
			return false;
		}

		private static StringList GetCertificateHostnames(X509Certificate certificate)
		{
			StringList stringList = new StringList();
			if (certificate.Subject != null)
			{
				string[] array = certificate.Subject.Split(AutodiscoverClient.CertificateSubjectDelimiters, StringSplitOptions.RemoveEmptyEntries);
				if (array != null)
				{
					foreach (string line in array)
					{
						AutodiscoverClient.ParseCertificateHostnames("CN", line, stringList);
					}
				}
			}
			X509Certificate2 x509Certificate = certificate as X509Certificate2;
			if (x509Certificate != null && x509Certificate.Extensions != null)
			{
				X509Extension x509Extension = x509Certificate.Extensions["Subject Alternative Name"];
				if (x509Extension != null)
				{
					AsnEncodedData asnEncodedData = new AsnEncodedData(x509Extension.Oid, x509Extension.RawData);
					AsnEncodedDataCollection asnEncodedDataCollection = new AsnEncodedDataCollection(asnEncodedData);
					foreach (AsnEncodedData asnEncodedData2 in asnEncodedDataCollection)
					{
						AutodiscoverClient.ParseCertificateHostnames("DNS Name", asnEncodedData2.Format(false), stringList);
					}
				}
			}
			return stringList;
		}

		private static void ParseCertificateHostnames(string key, string line, StringList hostnames)
		{
			string[] array = line.Split(AutodiscoverClient.CommaDelimiter, StringSplitOptions.RemoveEmptyEntries);
			if (array != null)
			{
				foreach (string text in array)
				{
					string[] array3 = text.Split(AutodiscoverClient.EqualDelimiter, StringSplitOptions.RemoveEmptyEntries);
					if (array3 != null && array3.Length == 2)
					{
						string x = array3[0].Trim();
						if (StringComparer.OrdinalIgnoreCase.Equals(x, key))
						{
							string hostname = array3[1].Trim();
							if (!string.IsNullOrEmpty(hostname))
							{
								if (!hostnames.Exists((string item) => StringComparer.OrdinalIgnoreCase.Equals(item, hostname)))
								{
									AutodiscoverClient.Tracer.TraceDebug<string, string>(0L, "From item {0} in SSL certificate identified hostname: {1}", text, hostname);
									hostnames.Add(hostname);
								}
							}
						}
					}
				}
			}
		}

		private AutodiscoverResultData InvokeForUnsecureRedirect(InvokeDelegate invokeDelegate, string host)
		{
			AutodiscoverResultData autodiscoverResultData = this.DiscoverUnsecuredRedirect(host);
			AutodiscoverClient.Tracer.TraceDebug<AutodiscoverResultData>((long)this.GetHashCode(), "Unsecure redirect result: {0}", autodiscoverResultData);
			if (autodiscoverResultData.Type != AutodiscoverResult.Success)
			{
				return autodiscoverResultData;
			}
			AutodiscoverResultData autodiscoverResultData2 = this.InvokeForHost(invokeDelegate, autodiscoverResultData.RedirectUrl.Host);
			AutodiscoverClient.Tracer.TraceDebug<AutodiscoverResultData>((long)this.GetHashCode(), "Result retrieved from unsecure redirect: {0}", autodiscoverResultData2);
			if (autodiscoverResultData2.Type != AutodiscoverResult.Success)
			{
				return new AutodiscoverResultData
				{
					Type = AutodiscoverResult.Failure,
					Url = autodiscoverResultData.Url,
					RedirectUrl = autodiscoverResultData.RedirectUrl,
					Alternate = autodiscoverResultData2
				};
			}
			if (this.IsAllowedHostname(autodiscoverResultData.RedirectUrl.Host))
			{
				return autodiscoverResultData2;
			}
			return new AutodiscoverResultData
			{
				Type = AutodiscoverResult.UnsecuredRedirect,
				Url = autodiscoverResultData.Url,
				RedirectUrl = autodiscoverResultData.RedirectUrl,
				Alternate = autodiscoverResultData2
			};
		}

		private AutodiscoverResultData DiscoverUnsecuredRedirect(string host)
		{
			UriBuilder uriBuilder = new UriBuilder
			{
				Scheme = Uri.UriSchemeHttp,
				Host = host,
				Path = "/autodiscover/autodiscover.xml"
			};
			Uri url = uriBuilder.Uri;
			AutodiscoverClient.Tracer.TraceDebug<Uri>(0L, "Calling {0}", url);
			HttpWebResponse response = null;
			Exception ex = null;
			try
			{
				this.InvokeWithWebProxy(url.ToString(), delegate(IWebProxy webProxy)
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url.ToString());
					httpWebRequest.Proxy = webProxy;
					httpWebRequest.UserAgent = "AutodiscoverClient";
					httpWebRequest.Timeout = 30000;
					httpWebRequest.AllowAutoRedirect = false;
					httpWebRequest.ServicePoint.Expect100Continue = false;
					response = (HttpWebResponse)httpWebRequest.GetResponse();
				});
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (WebException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				AutodiscoverClient.Tracer.TraceError<Uri, Exception>(0L, "Unable to connect to {0}. Exception: {1}", url, ex);
				return new AutodiscoverResultData
				{
					Type = AutodiscoverResult.Failure,
					Url = url,
					Exception = ex
				};
			}
			if (response == null)
			{
				AutodiscoverClient.Tracer.TraceError<Uri>(0L, "No response from {0}.", url);
				return new AutodiscoverResultData
				{
					Type = AutodiscoverResult.Failure,
					Url = url,
					Exception = new InvalidDataException("No response")
				};
			}
			AutodiscoverResultData result;
			using (response)
			{
				if (response.StatusCode != HttpStatusCode.Found)
				{
					AutodiscoverClient.Tracer.TraceError<Uri, HttpStatusCode>(0L, "Request to {0} did not result in 302 Found. Status: {1}", url, response.StatusCode);
					result = new AutodiscoverResultData
					{
						Type = AutodiscoverResult.Failure,
						Url = url,
						Exception = new AutodiscoverClientException(NetException.UnexpectedStatusCode(response.StatusCode.ToString()))
					};
				}
				else
				{
					string text = response.Headers[HttpResponseHeader.Location];
					if (!Uri.IsWellFormedUriString(text, UriKind.Absolute))
					{
						AutodiscoverClient.Tracer.TraceError<Uri, string>(0L, "302 from {0} returned malformed URL for location header: {1}", url, text);
						result = new AutodiscoverResultData
						{
							Type = AutodiscoverResult.Failure,
							Url = url,
							Exception = new AutodiscoverClientException(NetException.MalformedLocationHeader(text))
						};
					}
					else
					{
						Uri uri = new Uri(text);
						if (uri.Scheme != Uri.UriSchemeHttps)
						{
							AutodiscoverClient.Tracer.TraceError<Uri, string>(0L, "302 from {0} returned non-HTTPS URL: {1}", url, text);
							result = new AutodiscoverResultData
							{
								Type = AutodiscoverResult.Failure,
								Url = url,
								Exception = new AutodiscoverClientException(NetException.NonHttpsLocationHeader(text))
							};
						}
						else if (!StringComparer.OrdinalIgnoreCase.Equals(uri.PathAndQuery, "/autodiscover/autodiscover.xml"))
						{
							AutodiscoverClient.Tracer.TraceError<Uri, string>(0L, "302 from {0} returned unexpected URL: {1}", url, text);
							result = new AutodiscoverResultData
							{
								Type = AutodiscoverResult.Failure,
								Url = url,
								Exception = new AutodiscoverClientException(NetException.UnexpectedPathLocationHeader(text))
							};
						}
						else
						{
							AutodiscoverClient.Tracer.TraceDebug<Uri, string>(0L, "302 from {0} returned URL: {1}", url, text);
							result = new AutodiscoverResultData
							{
								Type = AutodiscoverResult.Success,
								Url = url,
								RedirectUrl = uri
							};
						}
					}
				}
			}
			return result;
		}

		private void InvokeWithWebProxy(string url, AutodiscoverClient.InvokeWithWebProxyDelegate invokeWithWebProxy)
		{
			if (this.Proxy != null)
			{
				AutodiscoverClient.Tracer.TraceDebug<string, IWebProxy>((long)this.GetHashCode(), "Trying to connect to {0} endpoint via web proxy {1}", url, this.Proxy);
				try
				{
					invokeWithWebProxy(this.Proxy);
					return;
				}
				catch (WebException ex)
				{
					AutodiscoverClient.Tracer.TraceDebug<string, IWebProxy, WebException>((long)this.GetHashCode(), "Failed to connect to {0} endpoint via web proxy {1} due exception: {2}", url, this.Proxy, ex);
					Exception ex2 = null;
					try
					{
						AutodiscoverClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Trying to connect to {0} endpoint without web proxy", url);
						invokeWithWebProxy(AutodiscoverClient.NoProxy);
					}
					catch (WebException ex3)
					{
						ex2 = ex3;
					}
					catch (IOException ex4)
					{
						ex2 = ex4;
					}
					if (ex2 != null)
					{
						AutodiscoverClient.Tracer.TraceError<string, WebException>((long)this.GetHashCode(), "Failed to connect to {0} endpoint without web proxy due exception: {1}", url, ex);
						throw;
					}
					return;
				}
			}
			AutodiscoverClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Trying to connect to {0} endpoint without web proxy", url);
			invokeWithWebProxy(null);
		}

		private bool IsAllowedHostname(IEnumerable<string> hostnames)
		{
			foreach (string hostname in hostnames)
			{
				if (this.IsAllowedHostname(hostname))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsAllowedHostname(string hostname)
		{
			foreach (string allowedHostname in this.allowedHostnames)
			{
				if (this.IsHostnameMatch(allowedHostname, hostname))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsHostnameMatch(string allowedHostname, string hostname)
		{
			if (!allowedHostname.StartsWith("*.", StringComparison.OrdinalIgnoreCase))
			{
				return StringComparer.OrdinalIgnoreCase.Equals(allowedHostname, hostname);
			}
			string x = allowedHostname.Substring(2);
			if (StringComparer.OrdinalIgnoreCase.Equals(x, hostname))
			{
				return true;
			}
			string value = allowedHostname.Substring(1);
			return hostname.EndsWith(value, StringComparison.OrdinalIgnoreCase);
		}

		private const int WebRequestTimeout = 30000;

		private const string CertificateSubjectKey = "CN";

		private const string CertificateSubjectAlternativeNameKey = "DNS Name";

		private const string CertificateSubjectAlternativeName = "Subject Alternative Name";

		private const string AgentName = "AutodiscoverClient";

		private const string AutodiscoverPath = "/autodiscover/";

		private const string SoapAutodiscoverPath = "/autodiscover/autodiscover.svc";

		private const string XmlAutodiscoverPath = "/autodiscover/autodiscover.xml";

		private const string AutoDiscoverHostnameFormat = "autodiscover.{0}";

		private const int MaximumRedirects = 5;

		private static readonly WebProxy NoProxy = new WebProxy();

		[ThreadStatic]
		private static StringList staticInvalidHostnames;

		private DefaultBinding_Autodiscover binding;

		private StringList allowedHostnames = new StringList();

		private static readonly char[] EqualDelimiter = new char[]
		{
			'='
		};

		private static readonly char[] CommaDelimiter = new char[]
		{
			','
		};

		private static readonly char[] CertificateSubjectDelimiters = new char[]
		{
			'\n',
			'\r'
		};

		private static Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.EwsClientTracer;

		private delegate AutodiscoverResultData AutoDiscoverStep();

		private delegate void InvokeWithWebProxyDelegate(IWebProxy webProxy);
	}
}
