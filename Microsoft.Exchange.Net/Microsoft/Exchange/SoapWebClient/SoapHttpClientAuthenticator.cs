using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Net.WSSecurity;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.SoapWebClient
{
	internal sealed class SoapHttpClientAuthenticator
	{
		public SoapHeaderCollection AdditionalSoapHeaders
		{
			get
			{
				return this.soapAuthenticator.AdditionalSoapHeaders;
			}
		}

		public static SoapHttpClientAuthenticator CreateNone()
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.None, SoapAuthenticator.CreateNone());
		}

		public static SoapHttpClientAuthenticator CreateAnonymous()
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.None, SoapAuthenticator.CreateAnonymous());
		}

		public static SoapHttpClientAuthenticator CreateNetworkService()
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.NetworkService, SoapAuthenticator.CreateNone());
		}

		public static SoapHttpClientAuthenticator CreateNetworkServiceForSoap()
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.NetworkService, SoapAuthenticator.CreateAnonymous());
		}

		public static SoapHttpClientAuthenticator Create(ICredentials credentials)
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.Create(credentials), SoapAuthenticator.CreateNone());
		}

		public static SoapHttpClientAuthenticator CreateForSoap(ICredentials credentials)
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.Create(credentials), SoapAuthenticator.CreateAnonymous());
		}

		public static SoapHttpClientAuthenticator Create(CommonAccessToken commonAccessToken)
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.Create(commonAccessToken), SoapAuthenticator.CreateNone());
		}

		public static SoapHttpClientAuthenticator Create(RequestedToken token)
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.None, SoapAuthenticator.Create(token));
		}

		public static SoapHttpClientAuthenticator Create(X509Certificate2 certificate)
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.None, SoapAuthenticator.Create(certificate));
		}

		public static SoapHttpClientAuthenticator Create(WSSecurityHeader header)
		{
			return new SoapHttpClientAuthenticator(HttpAuthenticator.None, SoapAuthenticator.Create(header));
		}

		public XmlReader GetReaderForMessage(XmlReader reader, SoapClientMessage message)
		{
			return this.soapAuthenticator.GetReaderForMessage(reader, message);
		}

		public XmlWriter GetWriterForMessage(XmlNamespaceDefinition[] predefinedNamespaces, XmlWriter writer, SoapClientMessage message)
		{
			return this.soapAuthenticator.GetWriterForMessage(predefinedNamespaces, writer, message);
		}

		public T AuthenticateAndExecute<T>(CustomSoapHttpClientProtocol client, AuthenticateAndExecuteHandler<T> handler)
		{
			return this.httpAuthenticator.AuthenticateAndExecute<T>(client, handler);
		}

		private SoapHttpClientAuthenticator(HttpAuthenticator httpAuthenticator, SoapAuthenticator soapAuthenticator)
		{
			this.httpAuthenticator = httpAuthenticator;
			this.soapAuthenticator = soapAuthenticator;
		}

		private SoapAuthenticator soapAuthenticator;

		private HttpAuthenticator httpAuthenticator;
	}
}
