using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Net.WSSecurity;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.SoapWebClient
{
	internal abstract class SoapAuthenticator
	{
		protected virtual XmlNamespaceDefinition[] PredefinedNamespaces
		{
			get
			{
				return SoapAuthenticator.NoPredefinedNamespaces;
			}
		}

		public SoapHeaderCollection AdditionalSoapHeaders
		{
			get
			{
				if (this.additionalSoapHeaders == null)
				{
					this.additionalSoapHeaders = new SoapHeaderCollection();
				}
				return this.additionalSoapHeaders;
			}
		}

		protected virtual void AddMessageSoapHeaders(SoapClientMessage message)
		{
			if (this.AdditionalSoapHeaders != null && this.AdditionalSoapHeaders.Count > 0)
			{
				foreach (object obj in this.AdditionalSoapHeaders)
				{
					SoapHeader header = (SoapHeader)obj;
					message.Headers.Add(header);
				}
			}
		}

		public XmlReader GetReaderForMessage(XmlReader xmlReader, SoapClientMessage message)
		{
			return new SoapHttpClientXmlReader(xmlReader);
		}

		public XmlWriter GetWriterForMessage(XmlNamespaceDefinition[] otherPredefinedNamespaces, XmlWriter xmlWriter, SoapClientMessage message)
		{
			this.AddMessageSoapHeaders(message);
			XmlNamespaceDefinition[] namespaceDefinitions;
			if (otherPredefinedNamespaces.Length > 0 && this.PredefinedNamespaces.Length > 0)
			{
				List<XmlNamespaceDefinition> list = new List<XmlNamespaceDefinition>(otherPredefinedNamespaces.Length + this.PredefinedNamespaces.Length);
				list.AddRange(otherPredefinedNamespaces);
				list.AddRange(this.PredefinedNamespaces);
				namespaceDefinitions = list.ToArray();
			}
			else if (otherPredefinedNamespaces.Length > 0)
			{
				namespaceDefinitions = otherPredefinedNamespaces;
			}
			else
			{
				namespaceDefinitions = this.PredefinedNamespaces;
			}
			return new SoapHttpClientXmlWriter(xmlWriter, namespaceDefinitions);
		}

		public static SoapAuthenticator CreateNone()
		{
			return new SoapAuthenticator.NoSoapAuthenticator();
		}

		public static SoapAuthenticator CreateAnonymous()
		{
			return new SoapAuthenticator.AnonymousSoapAuthenticator();
		}

		public static SoapAuthenticator Create(X509Certificate2 certificate)
		{
			return SoapAuthenticator.Create(WSSecurityHeader.Create(certificate));
		}

		public static SoapAuthenticator Create(RequestedToken token)
		{
			return SoapAuthenticator.Create(WSSecurityHeader.Create(token));
		}

		public static SoapAuthenticator Create(WSSecurityHeader header)
		{
			return new SoapAuthenticator.WSSecurityAuthenticator(header);
		}

		private SoapHeaderCollection additionalSoapHeaders;

		private static XmlNamespaceDefinition[] NoPredefinedNamespaces = new XmlNamespaceDefinition[0];

		private sealed class NoSoapAuthenticator : SoapAuthenticator
		{
		}

		private class AnonymousSoapAuthenticator : SoapAuthenticator
		{
			protected override XmlNamespaceDefinition[] PredefinedNamespaces
			{
				get
				{
					return SoapAuthenticator.AnonymousSoapAuthenticator.wsAddressingPredefinedNamespaces;
				}
			}

			protected override void AddMessageSoapHeaders(SoapClientMessage message)
			{
				base.AddMessageSoapHeaders(message);
				message.Headers.Add(new WSAddressingActionHeader(message.Action));
				message.Headers.Add(new WSAddressingToHeader(message.Url));
				message.Headers.Add(WSAddressingReplyToHeader.Anonymous);
			}

			private static XmlNamespaceDefinition[] wsAddressingPredefinedNamespaces = new XmlNamespaceDefinition[]
			{
				WSAddressing.Namespace
			};
		}

		private sealed class WSSecurityAuthenticator : SoapAuthenticator.AnonymousSoapAuthenticator
		{
			protected override XmlNamespaceDefinition[] PredefinedNamespaces
			{
				get
				{
					return SoapAuthenticator.WSSecurityAuthenticator.wsSecurityPredefinedNamespaces;
				}
			}

			public WSSecurityAuthenticator(WSSecurityHeader wsSecurityHeader)
			{
				this.wsSecurityHeader = wsSecurityHeader;
			}

			protected override void AddMessageSoapHeaders(SoapClientMessage message)
			{
				base.AddMessageSoapHeaders(message);
				message.Headers.Add(this.wsSecurityHeader);
			}

			private WSSecurityHeader wsSecurityHeader;

			private static XmlNamespaceDefinition[] wsSecurityPredefinedNamespaces = new XmlNamespaceDefinition[]
			{
				WSAddressing.Namespace,
				WSSecurityExtensions.Namespace,
				XmlDigitalSignature.Namespace,
				XmlEncryption.Namespace
			};
		}
	}
}
