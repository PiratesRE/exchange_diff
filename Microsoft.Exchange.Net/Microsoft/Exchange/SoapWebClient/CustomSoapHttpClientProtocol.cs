using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.SoapWebClient
{
	public abstract class CustomSoapHttpClientProtocol : SoapHttpClientProtocol
	{
		protected CustomSoapHttpClientProtocol()
		{
		}

		protected virtual bool SuppressMustUnderstand
		{
			get
			{
				return false;
			}
		}

		internal virtual XmlNamespaceDefinition[] PredefinedNamespaces
		{
			get
			{
				return CustomSoapHttpClientProtocol.NoPredefinedNamespaces;
			}
		}

		protected CustomSoapHttpClientProtocol(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool normalization) : this(componentId, remoteCertificateValidationCallback, normalization, true)
		{
		}

		protected CustomSoapHttpClientProtocol(RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool normalization)
		{
			this.normalization = normalization;
			this.remoteCertificateValidationCallback = remoteCertificateValidationCallback;
			this.checkInvalidCharacters = true;
		}

		protected CustomSoapHttpClientProtocol(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool normalization, bool checkInvalidCharacters)
		{
			this.normalization = normalization;
			this.remoteCertificateValidationCallback = remoteCertificateValidationCallback;
			this.checkInvalidCharacters = checkInvalidCharacters;
		}

		protected override WebRequest GetWebRequest(Uri uri)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)base.GetWebRequest(uri);
			httpWebRequest.ServerCertificateValidationCallback = this.remoteCertificateValidationCallback;
			if (this.keepAlive != null)
			{
				httpWebRequest.KeepAlive = this.keepAlive.Value;
			}
			foreach (KeyValuePair<string, string> keyValuePair in this.httpHeaders)
			{
				httpWebRequest.Headers.Add(keyValuePair.Key, keyValuePair.Value);
			}
			httpWebRequest.ServicePoint.Expect100Continue = false;
			return httpWebRequest;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse webResponse = base.GetWebResponse(request);
			this.TraceRequestAndResponse(request, webResponse);
			this.PopulateResponseHttpHeaders(webResponse);
			return webResponse;
		}

		protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
		{
			WebResponse webResponse = base.GetWebResponse(request, result);
			this.TraceRequestAndResponse(request, webResponse);
			this.PopulateResponseHttpHeaders(webResponse);
			return webResponse;
		}

		private void PopulateResponseHttpHeaders(WebResponse webResponse)
		{
			this.responseHttpHeaders.Clear();
			foreach (string text in webResponse.Headers.AllKeys)
			{
				this.responseHttpHeaders[text] = webResponse.Headers[text];
			}
		}

		private void TraceRequestAndResponse(WebRequest request, WebResponse response)
		{
			if (CustomSoapHttpClientProtocol.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (request != null)
				{
					HttpWebRequest httpWebRequest = request as HttpWebRequest;
					if (httpWebRequest != null)
					{
						CustomSoapHttpClientProtocol.Tracer.TraceDebug<Uri>((long)this.GetHashCode(), "Request HTTP URI: {0}", httpWebRequest.RequestUri);
					}
					this.TraceHttpHeaderCollection("Request", request.Headers);
				}
				if (response != null)
				{
					this.TraceHttpHeaderCollection("Response", response.Headers);
					HttpWebResponse httpWebResponse = response as HttpWebResponse;
					if (httpWebResponse != null)
					{
						CustomSoapHttpClientProtocol.Tracer.TraceDebug<HttpStatusCode, string>((long)this.GetHashCode(), "Response HTTP status: {0} - {1}", httpWebResponse.StatusCode, httpWebResponse.StatusDescription);
					}
				}
			}
		}

		private void TraceHttpHeaderCollection(string name, WebHeaderCollection headers)
		{
			StringBuilder stringBuilder = new StringBuilder(headers.Count * 40);
			foreach (string text in headers.AllKeys)
			{
				stringBuilder.Append(text + "=" + headers[text] + ";");
			}
			CustomSoapHttpClientProtocol.Tracer.TraceDebug<string, StringBuilder>((long)this.GetHashCode(), "{0} HTTP headers: {1}", name, stringBuilder);
		}

		internal bool? KeepAlive
		{
			get
			{
				return this.keepAlive;
			}
			set
			{
				this.keepAlive = value;
			}
		}

		internal SoapHttpClientAuthenticator Authenticator
		{
			get
			{
				return this.authenticator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Authenticator");
				}
				this.authenticator = value;
			}
		}

		public Dictionary<string, string> HttpHeaders
		{
			get
			{
				return this.httpHeaders;
			}
		}

		public Dictionary<string, string> ResponseHttpHeaders
		{
			get
			{
				return this.responseHttpHeaders;
			}
		}

		protected override XmlReader GetReaderForMessage(SoapClientMessage message, int bufferSize)
		{
			if (!message.Stream.CanRead)
			{
				throw new ChannelTerminatedException();
			}
			XmlReader reader;
			try
			{
				if (!this.checkInvalidCharacters)
				{
					XmlReaderSettings settings = new XmlReaderSettings
					{
						CheckCharacters = false
					};
					reader = XmlReader.Create(message.Stream, settings);
				}
				else
				{
					reader = base.GetReaderForMessage(message, bufferSize);
				}
			}
			catch (ArgumentException ex)
			{
				throw new ChannelTerminatedException(ex.Message, ex);
			}
			XmlReader readerForMessage = this.authenticator.GetReaderForMessage(reader, message);
			SoapHttpClientXmlReader soapHttpClientXmlReader = readerForMessage as SoapHttpClientXmlReader;
			if (soapHttpClientXmlReader != null && soapHttpClientXmlReader.SupportsNormalization)
			{
				soapHttpClientXmlReader.Normalization = this.normalization;
			}
			return readerForMessage;
		}

		protected override XmlWriter GetWriterForMessage(SoapClientMessage message, int bufferSize)
		{
			XmlWriter writerForMessage = this.authenticator.GetWriterForMessage(this.PredefinedNamespaces, base.GetWriterForMessage(message, bufferSize), message);
			if (this.SuppressMustUnderstand)
			{
				foreach (object obj in message.Headers)
				{
					SoapHeader soapHeader = (SoapHeader)obj;
					soapHeader.MustUnderstand = false;
				}
			}
			return writerForMessage;
		}

		protected new IAsyncResult BeginInvoke(string methodName, object[] parameters, AsyncCallback callback, object asyncState)
		{
			return this.authenticator.AuthenticateAndExecute<IAsyncResult>(this, () => this.BeginInvoke(methodName, parameters, callback, asyncState));
		}

		protected new virtual object[] Invoke(string methodName, object[] parameters)
		{
			return this.authenticator.AuthenticateAndExecute<object[]>(this, () => this.Invoke(methodName, parameters));
		}

		private static readonly XmlNamespaceDefinition[] NoPredefinedNamespaces = new XmlNamespaceDefinition[0];

		private SoapHttpClientAuthenticator authenticator = SoapHttpClientAuthenticator.CreateNone();

		private bool? keepAlive;

		private readonly Dictionary<string, string> httpHeaders = new Dictionary<string, string>();

		private readonly RemoteCertificateValidationCallback remoteCertificateValidationCallback;

		private readonly bool normalization;

		private readonly Dictionary<string, string> responseHttpHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private readonly bool checkInvalidCharacters;

		private static readonly Trace Tracer = ExTraceGlobals.EwsClientTracer;
	}
}
