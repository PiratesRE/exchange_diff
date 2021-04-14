using System;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Net.WSSecurity;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ProxyAuthenticator
	{
		public AuthenticatorType AuthenticatorType { get; private set; }

		public static ProxyAuthenticator Create(NetworkCredential credentials, SerializedSecurityContext serializedContext, string messageId)
		{
			SoapHttpClientAuthenticator soapHttpClientAuthenticator;
			if (credentials == null)
			{
				ProxyAuthenticator.SecurityTracer.TraceDebug(0L, "{0}: creating ProxyAuthenticator for network service", new object[]
				{
					TraceContext.Get()
				});
				soapHttpClientAuthenticator = SoapHttpClientAuthenticator.CreateNetworkService();
			}
			else
			{
				ProxyAuthenticator.SecurityTracer.TraceDebug<object, string, string>(0L, "{0}: creating ProxyAuthenticator for credentials: {1}\\{2}", TraceContext.Get(), credentials.Domain, credentials.UserName);
				soapHttpClientAuthenticator = SoapHttpClientAuthenticator.Create(credentials);
			}
			if (serializedContext != null)
			{
				soapHttpClientAuthenticator.AdditionalSoapHeaders.Add(serializedContext);
			}
			ProxyAuthenticator.SetMessageId(soapHttpClientAuthenticator, messageId);
			return new ProxyAuthenticator(soapHttpClientAuthenticator, AuthenticatorType.NetworkCredentials);
		}

		public static ProxyAuthenticator Create(CredentialCache cache, SerializedSecurityContext serializedContext, string messageId)
		{
			SoapHttpClientAuthenticator soapHttpClientAuthenticator;
			if (cache == null)
			{
				ProxyAuthenticator.SecurityTracer.TraceDebug(0L, "{0}: creating ProxyAuthenticator for network service", new object[]
				{
					TraceContext.Get()
				});
				soapHttpClientAuthenticator = SoapHttpClientAuthenticator.CreateNetworkService();
			}
			else
			{
				ProxyAuthenticator.SecurityTracer.TraceDebug<object, CredentialCache>(0L, "{0}: creating ProxyAuthenticator for credential cache: {1}", TraceContext.Get(), cache);
				soapHttpClientAuthenticator = SoapHttpClientAuthenticator.Create(cache);
			}
			if (serializedContext != null)
			{
				soapHttpClientAuthenticator.AdditionalSoapHeaders.Add(serializedContext);
			}
			ProxyAuthenticator.SetMessageId(soapHttpClientAuthenticator, messageId);
			return new ProxyAuthenticator(soapHttpClientAuthenticator, AuthenticatorType.NetworkCredentials);
		}

		public static ProxyAuthenticator Create(OAuthCredentials credentials, string messageId, bool isAutodiscoverRequest)
		{
			ProxyAuthenticator.SecurityTracer.TraceDebug<object, OAuthCredentials>(0L, "{0}: creating ProxyAuthenticator for OAuthCredentials: {1}", TraceContext.Get(), credentials);
			SoapHttpClientAuthenticator soapHttpClientAuthenticator = isAutodiscoverRequest ? SoapHttpClientAuthenticator.CreateForSoap(credentials) : SoapHttpClientAuthenticator.Create(credentials);
			ProxyAuthenticator.SetMessageId(soapHttpClientAuthenticator, messageId);
			return new ProxyAuthenticator(soapHttpClientAuthenticator, AuthenticatorType.OAuth);
		}

		public static ProxyAuthenticator Create(CommonAccessToken commonAccessToken, string messageId)
		{
			ProxyAuthenticator.SecurityTracer.TraceDebug<object, CommonAccessToken>(0L, "{0}: creating ProxyAuthenticator for CommonAccessToken: {1}", TraceContext.Get(), commonAccessToken);
			SoapHttpClientAuthenticator soapHttpClientAuthenticator = SoapHttpClientAuthenticator.Create(commonAccessToken);
			ProxyAuthenticator.SetMessageId(soapHttpClientAuthenticator, messageId);
			return new ProxyAuthenticator(soapHttpClientAuthenticator, AuthenticatorType.OAuth);
		}

		public static ProxyAuthenticator Create(RequestedToken token, SmtpAddress sharingKey, string messageId)
		{
			ProxyAuthenticator.SecurityTracer.TraceDebug(0L, "{0}: creating ProxyAuthenticator for WS-Security", new object[]
			{
				TraceContext.Get()
			});
			XmlElement xmlElement = null;
			if (sharingKey != SmtpAddress.Empty)
			{
				xmlElement = SharingKeyHandler.Encrypt(sharingKey, token.ProofToken);
			}
			SoapHttpClientAuthenticator soapHttpClientAuthenticator = SoapHttpClientAuthenticator.Create(token);
			if (xmlElement != null)
			{
				soapHttpClientAuthenticator.AdditionalSoapHeaders.Add(new SharingSecurityHeader(xmlElement));
			}
			ProxyAuthenticator.SetMessageId(soapHttpClientAuthenticator, messageId);
			return new ProxyAuthenticator(soapHttpClientAuthenticator, AuthenticatorType.WSSecurity);
		}

		public static ProxyAuthenticator Create(WSSecurityHeader wsSecurityHeader, SharingSecurityHeader sharingSecurityHeader, string messageId)
		{
			ProxyAuthenticator.SecurityTracer.TraceDebug(0L, "{0}: creating ProxyAuthenticator for WS-Security", new object[]
			{
				TraceContext.Get()
			});
			SoapHttpClientAuthenticator soapHttpClientAuthenticator = SoapHttpClientAuthenticator.Create(wsSecurityHeader);
			if (sharingSecurityHeader != null)
			{
				soapHttpClientAuthenticator.AdditionalSoapHeaders.Add(sharingSecurityHeader);
			}
			ProxyAuthenticator.SetMessageId(soapHttpClientAuthenticator, messageId);
			return new ProxyAuthenticator(soapHttpClientAuthenticator, AuthenticatorType.WSSecurity);
		}

		public static ProxyAuthenticator CreateForSoap(string messageId)
		{
			ProxyAuthenticator.SecurityTracer.TraceDebug(0L, "{0}: creating ProxyAuthenticator for network service", new object[]
			{
				TraceContext.Get()
			});
			SoapHttpClientAuthenticator soapHttpClientAuthenticator = SoapHttpClientAuthenticator.CreateNetworkServiceForSoap();
			ProxyAuthenticator.SetMessageId(soapHttpClientAuthenticator, messageId);
			return new ProxyAuthenticator(soapHttpClientAuthenticator, AuthenticatorType.NetworkCredentials);
		}

		public void Authenticate(CustomSoapHttpClientProtocol client)
		{
			ProxyAuthenticator.SecurityTracer.TraceDebug<object, AuthenticatorType>((long)this.GetHashCode(), "{0}: Authenticating client with {1}", TraceContext.Get(), this.AuthenticatorType);
			client.Authenticator = this.authenticator;
			if (this.AuthenticatorType == AuthenticatorType.WSSecurity)
			{
				client.Url = EwsWsSecurityUrl.Fix(client.Url);
				client.ConnectionGroupName = "WS>";
				return;
			}
			client.Url = EwsWsSecurityUrl.FixForAnonymous(client.Url);
			client.UnsafeAuthenticatedConnectionSharing = Configuration.UnsafeAuthenticatedConnectionSharing.Value;
			client.ConnectionGroupName = "NC>";
		}

		public override string ToString()
		{
			return this.AuthenticatorType.ToString();
		}

		private ProxyAuthenticator(SoapHttpClientAuthenticator authenticator, AuthenticatorType authenticatorType)
		{
			this.authenticator = authenticator;
			this.AuthenticatorType = authenticatorType;
		}

		private static void SetMessageId(SoapHttpClientAuthenticator authenticator, string messageId)
		{
			if (messageId != null)
			{
				authenticator.AdditionalSoapHeaders.Add(WSAddressingMessageIDHeader.Create(messageId));
			}
		}

		private SoapHttpClientAuthenticator authenticator;

		private static readonly Trace SecurityTracer = ExTraceGlobals.SecurityTracer;
	}
}
