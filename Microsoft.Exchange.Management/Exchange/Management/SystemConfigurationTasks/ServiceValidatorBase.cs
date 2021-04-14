using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal abstract class ServiceValidatorBase
	{
		public ServiceValidatorBase(string uri, NetworkCredential credentials)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (credentials == null)
			{
				throw new ArgumentNullException("credentials");
			}
			this.Uri = uri;
			this.Credentials = credentials;
			this.TraceResponseBody = true;
		}

		public Task.TaskVerboseLoggingDelegate VerboseDelegate { get; set; }

		public bool IgnoreSslCertError { get; set; }

		public string UserAgent { get; set; }

		public bool TraceResponseBody { get; set; }

		public string Uri { get; private set; }

		public NetworkCredential Credentials { get; private set; }

		public Exception Error { get; private set; }

		public long Latency { get; private set; }

		public string Verbose
		{
			get
			{
				return string.Join("\n", this.verboseEntries.ToArray());
			}
		}

		protected abstract string Name { get; }

		public bool Invoke()
		{
			try
			{
				this.PreCreateRequest();
				this.Error = this.InternalInvoke();
			}
			catch (WebException ex)
			{
				this.Error = ex;
				if (ex.Response != null && ex.Response.Headers != null)
				{
					this.TraceResponse(ServiceValidatorBase.FormatHeaders(ex.Response.Headers));
				}
			}
			catch (InvalidOperationException error)
			{
				this.Error = error;
			}
			if (this.Error != null)
			{
				this.TraceResponse(this.Error.ToString());
			}
			return this.Error == null;
		}

		protected virtual void PreCreateRequest()
		{
			this.Error = null;
			this.Latency = 0L;
			this.verboseEntries.Clear();
		}

		protected virtual void FillRequestProperties(HttpWebRequest request)
		{
			request.ContentType = "text/xml;charset=utf-8";
			request.UserAgent = this.UserAgent;
			request.PreAuthenticate = true;
			request.CookieContainer = new CookieContainer();
			if (this.IgnoreSslCertError)
			{
				CertificateValidationManager.RegisterCallback(ServiceValidatorBase.ComponentId, new RemoteCertificateValidationCallback(ServiceValidatorBase.TrustAllCertValidationCallback));
				CertificateValidationManager.SetComponentId(request, ServiceValidatorBase.ComponentId);
			}
		}

		protected virtual bool FillRequestStream(Stream requestStream)
		{
			return false;
		}

		protected virtual Exception ValidateResponse(Stream responseStream)
		{
			return null;
		}

		private Exception InternalInvoke()
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.Uri);
			httpWebRequest.Credentials = this.Credentials;
			this.FillRequestProperties(httpWebRequest);
			this.WriteVerbose(Strings.VerboseServiceValidatorUrl(this.Name, this.Uri));
			if (Datacenter.IsMultiTenancyEnabled())
			{
				this.WriteVerbose(Strings.VerboseServiceValidatorCredential(this.Credentials.UserName, this.Credentials.Password));
			}
			else
			{
				this.WriteVerbose(Strings.VerboseServiceValidatorCredential(this.Credentials.UserName, "******"));
			}
			string message = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (this.FillRequestStream(memoryStream))
				{
					byte[] array = memoryStream.ToArray();
					message = Encoding.UTF8.GetString(array);
					using (Stream requestStream = httpWebRequest.GetRequestStream())
					{
						requestStream.Write(array, 0, array.Length);
					}
				}
			}
			WebResponse webResponse = null;
			Exception result;
			try
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					webResponse = httpWebRequest.GetResponse();
				}
				finally
				{
					this.Latency = stopwatch.ElapsedMilliseconds;
					this.TraceRequest(ServiceValidatorBase.FormatHeaders(httpWebRequest.Headers));
					this.TraceRequest(message);
				}
				this.TraceResponse(ServiceValidatorBase.FormatHeaders(webResponse.Headers));
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						string text = streamReader.ReadToEnd();
						if (this.TraceResponseBody)
						{
							this.TraceResponse(text);
						}
						byte[] bytes = Encoding.UTF8.GetBytes(text);
						using (MemoryStream memoryStream2 = new MemoryStream(bytes))
						{
							result = this.ValidateResponse(memoryStream2);
						}
					}
				}
			}
			finally
			{
				if (webResponse != null)
				{
					((IDisposable)webResponse).Dispose();
				}
			}
			return result;
		}

		private static string FormatHeaders(WebHeaderCollection headers)
		{
			string[] value = Array.ConvertAll<string, string>(headers.AllKeys, (string x) => string.Format("{0}: {1}", x, headers[x]));
			return string.Join("\n", value);
		}

		private void TraceRequest(string message)
		{
			this.WriteVerbose(Strings.VerboseServiceValidatorRequestTrace(this.Name, message));
		}

		private void TraceResponse(string message)
		{
			this.WriteVerbose(Strings.VerboseServiceValidatorResponseTrace(this.Name, this.TryFormatXml(message)));
		}

		private void WriteVerbose(LocalizedString locString)
		{
			if (this.VerboseDelegate != null)
			{
				this.VerboseDelegate(locString);
			}
			this.verboseEntries.Add(string.Format("[{0}] {1}", ExDateTime.UtcNow.ToString("u"), locString));
		}

		private string TryFormatXml(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return content;
			}
			string result;
			try
			{
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(content);
				StringBuilder stringBuilder = new StringBuilder();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
				{
					Indent = true,
					Encoding = Encoding.UTF8
				}))
				{
					xmlDocument.WriteTo(xmlWriter);
				}
				result = stringBuilder.ToString();
			}
			catch (XmlException)
			{
				result = content;
			}
			return result;
		}

		private static bool TrustAllCertValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public static readonly string ComponentId = "Management.ServiceValidator";

		private List<string> verboseEntries = new List<string>();
	}
}
