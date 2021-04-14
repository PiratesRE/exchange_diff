using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class SecurityTokenService : SoapClient
	{
		internal Uri TokenIssuerUri
		{
			get
			{
				return this.stsUri;
			}
		}

		internal Uri TokenIssuerEndpoint
		{
			get
			{
				return this.endpoint;
			}
		}

		internal X509Certificate2 Certificate
		{
			get
			{
				return this.certificate;
			}
		}

		internal string Requestor
		{
			get
			{
				return this.requestor;
			}
		}

		internal string STSClientIdentity
		{
			get
			{
				return this.stsClientIdentity;
			}
		}

		public SecurityTokenService(Uri endpoint, WebProxy webProxy, X509Certificate2 certificate, Uri stsUri, string policy, string requestor) : base(endpoint, webProxy)
		{
			SecurityTokenService.Tracer.TraceDebug((long)this.GetHashCode(), "SecurityTokenService(endpoint={0},webProxy={1},stsUri={2},policy={3},requestor={4},certificate={5})", new object[]
			{
				endpoint,
				webProxy,
				stsUri,
				policy,
				requestor,
				certificate
			});
			this.endpoint = endpoint;
			this.certificate = certificate;
			this.requestor = requestor;
			this.policy = policy;
			this.stsUri = stsUri;
			this.stsClientIdentity = ((this.stsUri != null) ? this.stsUri.ToString() : "UnknownSTS") + "/" + ((this.requestor != null) ? this.requestor : "UnknownRequestor");
			using (X509SecurityToken x509SecurityToken = new X509SecurityToken(this.certificate))
			{
				this.x509SubjectKeyIdentifierClause = x509SecurityToken.CreateKeyIdentifierClause<X509SubjectKeyIdentifierClause>();
			}
		}

		public RequestedToken IssueToken(DelegationTokenRequest request)
		{
			SecurityTokenService.Tracer.TraceDebug<DelegationTokenRequest>((long)this.GetHashCode(), "IssueToken:request={0}", request);
			this.EnforceCommonTokenIssuer(request.Target);
			XmlDocument xmlDocument = SecurityTokenService.CreateXmlDocument();
			IEnumerable<XmlElement> headers = this.CreateSoapHeaders(xmlDocument);
			XmlElement bodyContent = this.CreateRequestSecurityToken(xmlDocument, request);
			XmlElement requestedSecurityTokenResponseElement = base.Invoke(headers, bodyContent);
			return this.ParseRequestSecurityTokenResponse(requestedSecurityTokenResponseElement, request.Target);
		}

		public RequestedToken IssueToken(DelegationTokenRequest request, XmlTextWriter debugStream)
		{
			if (request == null)
			{
				throw new ArgumentException("request");
			}
			if (debugStream == null)
			{
				throw new ArgumentException("debugStream");
			}
			SecurityTokenService.Tracer.TraceDebug<DelegationTokenRequest>((long)this.GetHashCode(), "IssueToken:request={0}", request);
			debugStream.WriteStartElement("DelegationTokenRequest");
			debugStream.WriteElementString("SigningCertificateThumpbrint", this.Certificate.Thumbprint);
			debugStream.WriteElementString("SigningCertificateSKI", Convert.ToBase64String(this.x509SubjectKeyIdentifierClause.GetX509SubjectKeyIdentifier()));
			debugStream.WriteElementString("Requestor", this.Requestor);
			debugStream.WriteElementString("TokenIssuerEndpoint", this.TokenIssuerEndpoint.ToString());
			debugStream.WriteElementString("TokenIssuerUri", this.TokenIssuerUri.ToString());
			debugStream.WriteElementString("Policy", this.policy);
			debugStream.WriteElementString("CallerRequestParameters", request.ToString());
			this.EnforceCommonTokenIssuer(request.Target);
			XmlDocument xmlDocument = SecurityTokenService.CreateXmlDocument();
			IEnumerable<XmlElement> enumerable = this.CreateSoapHeaders(xmlDocument);
			debugStream.WriteStartElement("SoapHeaders");
			foreach (XmlElement xmlElement in enumerable)
			{
				xmlElement.WriteTo(debugStream);
			}
			debugStream.WriteEndElement();
			XmlElement xmlElement2 = this.CreateRequestSecurityToken(xmlDocument, request);
			debugStream.WriteStartElement("RST");
			xmlElement2.WriteTo(debugStream);
			debugStream.WriteEndElement();
			debugStream.Flush();
			XmlElement xmlElement3 = null;
			try
			{
				xmlElement3 = base.Invoke(enumerable, xmlElement2);
			}
			catch (Exception ex)
			{
				debugStream.WriteElementString("Exception", this.GetAllExceptionDetails(ex));
				debugStream.WriteEndElement();
				debugStream.Flush();
				throw ex;
			}
			debugStream.WriteStartElement("RSTR");
			if (xmlElement3 != null)
			{
				xmlElement3.WriteTo(debugStream);
			}
			debugStream.WriteEndElement();
			debugStream.Flush();
			RequestedToken result = this.ParseRequestSecurityTokenResponse(xmlElement3, request.Target);
			debugStream.WriteEndElement();
			debugStream.Flush();
			return result;
		}

		public IAsyncResult BeginIssueToken(DelegationTokenRequest request, AsyncCallback callback, object state)
		{
			SecurityTokenService.Tracer.TraceDebug<DelegationTokenRequest>((long)this.GetHashCode(), "BeginIssueToken:request={0}", request);
			this.EnforceCommonTokenIssuer(request.Target);
			XmlDocument xmlDocument = SecurityTokenService.CreateXmlDocument();
			IEnumerable<XmlElement> headers = this.CreateSoapHeaders(xmlDocument);
			XmlElement bodyContent = this.CreateRequestSecurityToken(xmlDocument, request);
			CustomContextAsyncResult customContextAsyncResult = new CustomContextAsyncResult(callback, state, request.Target);
			try
			{
				customContextAsyncResult.InnerAsyncResult = base.BeginInvoke(headers, bodyContent, new AsyncCallback(customContextAsyncResult.CustomCallback), customContextAsyncResult);
			}
			catch (WebException innerException)
			{
				throw new FailedToIssueTokenException(innerException);
			}
			SecurityTokenService.Tracer.TraceDebug<CustomContextAsyncResult>((long)this.GetHashCode(), "BeginIssueToken:asyncResult={0}", customContextAsyncResult);
			return customContextAsyncResult;
		}

		public RequestedToken EndIssueToken(IAsyncResult asyncResult)
		{
			SecurityTokenService.Tracer.TraceDebug<IAsyncResult>((long)this.GetHashCode(), "EndIssueToken:asyncResult={0}", asyncResult);
			CustomContextAsyncResult customContextAsyncResult = (CustomContextAsyncResult)asyncResult;
			XmlElement requestedSecurityTokenResponseElement = base.EndInvoke(customContextAsyncResult.InnerAsyncResult);
			return this.ParseRequestSecurityTokenResponse(requestedSecurityTokenResponseElement, (TokenTarget)customContextAsyncResult.CustomState);
		}

		public void AbortIssueToken(IAsyncResult asyncResult)
		{
			SecurityTokenService.Tracer.TraceDebug<IAsyncResult>((long)this.GetHashCode(), "AbortIssueToken:asyncResult={0}", asyncResult);
			CustomContextAsyncResult customContextAsyncResult = (CustomContextAsyncResult)asyncResult;
			base.AbortInvoke(customContextAsyncResult.InnerAsyncResult);
		}

		internal static XmlDocument CreateXmlDocument()
		{
			return new SafeXmlDocument
			{
				PreserveWhitespace = true
			};
		}

		private IEnumerable<XmlElement> CreateSoapHeaders(XmlDocument xmlDocument)
		{
			List<XmlElement> list = new List<XmlElement>();
			XmlAttribute xmlAttribute = Soap.MustUnderstand.CreateAttribute(xmlDocument, "1");
			XmlAttribute node = WSSecurityUtility.Id.CreateAttribute(xmlDocument, "_1");
			XmlElement xmlElement = WSAddressing.To.CreateElement(xmlDocument, this.endpoint.ToString());
			xmlElement.Attributes.Append((XmlAttribute)xmlAttribute.Clone());
			xmlElement.Attributes.Append(node);
			list.Add(xmlElement);
			XmlElement xmlElement2 = WSAddressing.Action.CreateElement(xmlDocument, "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue");
			xmlElement2.Attributes.Append((XmlAttribute)xmlAttribute.Clone());
			list.Add(xmlElement2);
			XmlElement item = WSAddressing.MessageId.CreateElement(xmlDocument, "urn:uuid:" + Guid.NewGuid().ToString());
			list.Add(item);
			XmlElement xmlElement3 = WSAddressing.Address.CreateElement(xmlDocument, "http://www.w3.org/2005/08/addressing/anonymous");
			XmlElement item2 = WSAddressing.ReplyTo.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement3
			});
			list.Add(item2);
			Timestamp timestamp = new Timestamp("_0", DateTime.UtcNow, DateTime.UtcNow + SecurityTokenService.requestDuration);
			XmlElement xml = timestamp.GetXml(xmlDocument);
			XmlElement securityTokenReference = this.CreateExternalSecurityTokenReference(xmlDocument);
			XmlElement xmlElement4 = this.CreatedSignature(xmlDocument, new XmlElement[]
			{
				xmlElement,
				xml
			}, securityTokenReference);
			XmlElement item3 = WSSecurityExtensions.Security.CreateElement(xmlDocument, new XmlAttribute[]
			{
				(XmlAttribute)xmlAttribute.Clone()
			}, new XmlElement[]
			{
				xml,
				xmlElement4
			});
			list.Add(item3);
			return list;
		}

		private XmlElement CreateSecurityTokenReference(XmlDocument xmlDocument, string certificateUri)
		{
			XmlAttribute xmlAttribute = Unqualified.URI.CreateAttribute(xmlDocument, "#" + certificateUri);
			XmlElement xmlElement = WSSecurityExtensions.Reference.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute
			});
			return WSSecurityExtensions.SecurityTokenReference.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement
			});
		}

		private XmlElement CreateExternalSecurityTokenReference(XmlDocument xmlDocument)
		{
			XmlElement xmlElement = WSSecurityExtensions.KeyIdentifier.CreateElement(xmlDocument, new XmlAttribute[]
			{
				Unqualified.ValueType.CreateAttribute(xmlDocument, "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509SubjectKeyIdentifier")
			}, Convert.ToBase64String(this.x509SubjectKeyIdentifierClause.GetX509SubjectKeyIdentifier()));
			return WSSecurityExtensions.SecurityTokenReference.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement
			});
		}

		internal static XmlElement CreateSamlAssertionSecurityTokenReference(XmlDocument xmlDocument, string assertionId)
		{
			XmlElement xmlElement = WSSecurityExtensions.KeyIdentifier.CreateElement(xmlDocument, new XmlAttribute[]
			{
				Unqualified.ValueType.CreateAttribute(xmlDocument, "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0#SAMLAssertionID")
			}, assertionId);
			return WSSecurityExtensions.SecurityTokenReference.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement
			});
		}

		private XmlElement CreatedSignature(XmlDocument xmlDocument, XmlElement[] elementsToSign, XmlElement securityTokenReference)
		{
			SecuritySignedXml securitySignedXml = new SecuritySignedXml(xmlDocument, elementsToSign);
			securitySignedXml.SigningKey = this.certificate.PrivateKey;
			XmlDsigExcC14NTransform transform = new XmlDsigExcC14NTransform();
			Signature signature = securitySignedXml.Signature;
			foreach (XmlElement xmlElement in elementsToSign)
			{
				string attributeValue = WSSecurityUtility.Id.GetAttributeValue(xmlElement);
				Reference reference = new Reference("#" + attributeValue);
				reference.AddTransform(transform);
				signature.SignedInfo.AddReference(reference);
			}
			KeyInfo keyInfo = new KeyInfo();
			keyInfo.AddClause(new KeyInfoNode(securityTokenReference));
			signature.KeyInfo = keyInfo;
			securitySignedXml.ComputeSignature();
			return (XmlElement)xmlDocument.ImportNode(securitySignedXml.GetXml(), true);
		}

		private string CreateNewUuid()
		{
			return "uuid-" + Guid.NewGuid().ToString();
		}

		private void EnforceCommonTokenIssuer(TokenTarget tokenTarget)
		{
			if (tokenTarget.TokenIssuerUris != null)
			{
				foreach (Uri uri in tokenTarget.TokenIssuerUris)
				{
					if (uri.Equals(this.stsUri))
					{
						return;
					}
				}
				throw new UnknownTokenIssuerException(this.stsUri.ToString(), tokenTarget.ToString());
			}
		}

		private XmlElement CreateRequestSecurityToken(XmlDocument xmlDocument, DelegationTokenRequest request)
		{
			XmlAttribute xmlAttribute = Unqualified.Id.CreateAttribute(xmlDocument, this.CreateNewUuid());
			XmlElement xmlElement = WSTrust.RequestSecurityToken.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute
			});
			XmlElement newChild = WSTrust.RequestType.CreateElement(xmlDocument, "http://schemas.xmlsoap.org/ws/2005/02/trust/Issue");
			xmlElement.AppendChild(newChild);
			XmlElement newChild2 = WSTrust.TokenType.CreateElement(xmlDocument, "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1");
			xmlElement.AppendChild(newChild2);
			XmlElement newChild3 = WSTrust.KeyType.CreateElement(xmlDocument, "http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey");
			xmlElement.AppendChild(newChild3);
			XmlElement newChild4 = WSTrust.KeySize.CreateElement(xmlDocument, "256");
			xmlElement.AppendChild(newChild4);
			XmlElement newChild5 = WSTrust.CanonicalizationAlgorithm.CreateElement(xmlDocument, "http://www.w3.org/2001/10/xml-exc-c14n#");
			xmlElement.AppendChild(newChild5);
			XmlElement newChild6 = WSTrust.EncryptionAlgorithm.CreateElement(xmlDocument, "http://www.w3.org/2001/04/xmlenc#aes256-cbc");
			xmlElement.AppendChild(newChild6);
			XmlElement newChild7 = WSTrust.EncryptWith.CreateElement(xmlDocument, "http://www.w3.org/2001/04/xmlenc#aes256-cbc");
			xmlElement.AppendChild(newChild7);
			XmlElement newChild8 = WSTrust.SignWith.CreateElement(xmlDocument, "http://www.w3.org/2000/09/xmldsig#hmac-sha1");
			xmlElement.AppendChild(newChild8);
			XmlElement newChild9 = WSTrust.ComputedKeyAlgorithm.CreateElement(xmlDocument, "http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1");
			xmlElement.AppendChild(newChild9);
			XmlElement newChild10 = this.CreateAppliesTo(xmlDocument, request.Target);
			xmlElement.AppendChild(newChild10);
			XmlElement newChild11 = this.CreateOnBehalfOf(xmlDocument, request);
			xmlElement.AppendChild(newChild11);
			XmlElement newChild12 = this.CreateAdditionalContext(xmlDocument);
			xmlElement.AppendChild(newChild12);
			XmlElement newChild13 = this.CreateClaims(xmlDocument, request.Offer.Name);
			xmlElement.AppendChild(newChild13);
			XmlAttribute xmlAttribute2 = Unqualified.URI.CreateAttribute(xmlDocument, string.IsNullOrEmpty(request.Policy) ? this.policy : request.Policy);
			XmlElement newChild14 = WSPolicy.PolicyReference.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute2
			});
			xmlElement.AppendChild(newChild14);
			return xmlElement;
		}

		private XmlElement CreateEntropy(XmlDocument xmlDocument)
		{
			byte[] array = new byte[32];
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(array);
			XmlAttribute xmlAttribute = Unqualified.Type.CreateAttribute(xmlDocument, "http://schemas.xmlsoap.org/ws/2005/02/trust/Nonce");
			XmlElement xmlElement = WSTrust.BinarySecret.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute
			}, Convert.ToBase64String(array));
			return WSTrust.Entropy.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement
			});
		}

		private XmlElement CreateAppliesTo(XmlDocument xmlDocument, TokenTarget target)
		{
			XmlElement xmlElement = WSAddressing.Address.CreateElement(xmlDocument, target.Uri.IsAbsoluteUri ? target.Uri.OriginalString : ("http://" + target.Uri.OriginalString));
			XmlElement xmlElement2 = WSAddressing.EndpointReference.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement
			});
			return WSPolicy.AppliesTo.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement2
			});
		}

		private XmlElement CreateOnBehalfOf(XmlDocument xmlDocument, DelegationTokenRequest request)
		{
			XmlElement xmlElement = this.CreateSamlAssertion(xmlDocument, request);
			return WSTrust.OnBehalfOf.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement
			});
		}

		private XmlElement CreateSamlAssertion(XmlDocument xmlDocument, DelegationTokenRequest request)
		{
			SamlAudienceRestrictionCondition samlAudienceRestrictionCondition = new SamlAudienceRestrictionCondition(new Uri[]
			{
				this.stsUri
			});
			SamlConditions samlConditions = new SamlConditions(DateTime.UtcNow, DateTime.UtcNow + request.Offer.Duration, new SamlCondition[]
			{
				samlAudienceRestrictionCondition
			});
			SamlSubject samlSubject = new SamlSubject(SecurityTokenService.GetIdentityNameFormat(request.FederatedIdentity.Type), null, request.FederatedIdentity.Identity, new string[]
			{
				"urn:oasis:names:tc:SAML:1.0:cm:sender-vouches"
			}, null, null);
			SamlAuthenticationStatement samlAuthenticationStatement = new SamlAuthenticationStatement(samlSubject, "urn:oasis:names:tc:SAML:1.0:am:password", DateTime.UtcNow, null, null, null);
			List<SamlAttribute> list = new List<SamlAttribute>();
			SamlAttribute item = new SamlAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims", "EmailAddress", new string[]
			{
				request.EmailAddress
			});
			list.Add(item);
			if (request.EmailAddresses != null && request.EmailAddresses.Count != 0)
			{
				foreach (string text in request.EmailAddresses)
				{
					if (!string.IsNullOrEmpty(text))
					{
						SamlAttribute item2 = new SamlAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims", "EmailAddressList", new string[]
						{
							text
						});
						list.Add(item2);
					}
				}
			}
			SamlAttributeStatement samlAttributeStatement = new SamlAttributeStatement(samlSubject, list.ToArray());
			SamlAssertion samlAssertion = new SamlAssertion(SecurityTokenService.CreateSamlAssertionId(), this.requestor, DateTime.UtcNow, samlConditions, null, new SamlStatement[]
			{
				samlAttributeStatement,
				samlAuthenticationStatement
			});
			XmlElement result;
			using (X509SecurityToken x509SecurityToken = new X509SecurityToken(this.certificate))
			{
				SecurityKeyIdentifier signingKeyIdentifier = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[]
				{
					x509SecurityToken.CreateKeyIdentifierClause<X509SubjectKeyIdentifierClause>()
				});
				samlAssertion.SigningCredentials = new SigningCredentials(x509SecurityToken.SecurityKeys[0], "http://www.w3.org/2000/09/xmldsig#rsa-sha1", "http://www.w3.org/2000/09/xmldsig#sha1", signingKeyIdentifier);
				SamlSecurityToken samlSecurityToken = new SamlSecurityToken(samlAssertion);
				XmlElement xmlFromSecurityToken = SecurityTokenService.GetXmlFromSecurityToken(samlSecurityToken);
				XmlElement xmlElement = (XmlElement)xmlDocument.ImportNode(xmlFromSecurityToken, true);
				result = xmlElement;
			}
			return result;
		}

		internal static XmlElement GetXmlFromSecurityToken(SamlSecurityToken samlSecurityToken)
		{
			XmlDocument xmlDocument = SecurityTokenService.CreateXmlDocument();
			using (Stream stream = new MemoryStream())
			{
				using (XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(stream))
				{
					try
					{
						WSSecurityTokenSerializer.DefaultInstance.WriteToken(xmlDictionaryWriter, samlSecurityToken);
					}
					catch (XmlException innerException)
					{
						throw new TokenSerializationException(innerException);
					}
					stream.Seek(0L, SeekOrigin.Begin);
					xmlDocument.Load(stream);
				}
			}
			return xmlDocument.DocumentElement;
		}

		internal static string CreateSamlAssertionId()
		{
			return "saml-" + Guid.NewGuid().ToString();
		}

		private XmlElement CreateAdditionalContext(XmlDocument xmlDocument)
		{
			XmlElement xmlElement = WSAuthorization.Value.CreateElement(xmlDocument, this.requestor);
			XmlAttribute xmlAttribute = Unqualified.Scope.CreateAttribute(xmlDocument, "http://schemas.xmlsoap.org/ws/2006/12/authorization/ctx/requestor");
			XmlAttribute xmlAttribute2 = Unqualified.Name.CreateAttribute(xmlDocument, "http://schemas.microsoft.com/wlid/requestor");
			XmlElement xmlElement2 = WSAuthorization.ContextItem.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute,
				xmlAttribute2
			}, new XmlElement[]
			{
				xmlElement
			});
			return WSAuthorization.AdditionalContext.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement2
			});
		}

		private XmlElement CreateClaims(XmlDocument xmlDocument, string authorizationClaim)
		{
			XmlElement xmlElement = WSAuthorization.Value.CreateElement(xmlDocument, authorizationClaim);
			XmlAttribute xmlAttribute = Unqualified.Uri.CreateAttribute(xmlDocument, "http://schemas.xmlsoap.org/ws/2006/12/authorization/claims/action");
			XmlElement xmlElement2 = WSAuthorization.ClaimType.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute
			}, new XmlElement[]
			{
				xmlElement
			});
			XmlAttribute xmlAttribute2 = Unqualified.Dialect.CreateAttribute(xmlDocument, "http://schemas.xmlsoap.org/ws/2006/12/authorization/authclaims");
			return WSTrust.Claims.CreateElement(xmlDocument, new XmlAttribute[]
			{
				xmlAttribute2
			}, new XmlElement[]
			{
				xmlElement2
			});
		}

		private RequestedToken ParseRequestSecurityTokenResponse(XmlElement requestedSecurityTokenResponseElement, TokenTarget target)
		{
			if (!WSTrust.RequestSecurityTokenResponse.IsMatch(requestedSecurityTokenResponseElement))
			{
				SecurityTokenService.Tracer.TraceError<SecurityTokenService, string>((long)this.GetHashCode(), "{0}: RequestSecurityTokenResponse XML is malformed: {1}", this, requestedSecurityTokenResponseElement.OuterXml);
				throw new SoapXmlMalformedException(requestedSecurityTokenResponseElement, WSTrust.RequestSecurityTokenResponse);
			}
			XmlElement requiredChildElement = base.GetRequiredChildElement(requestedSecurityTokenResponseElement, WSTrust.Lifetime);
			Timestamp lifetime = Timestamp.Parse(requiredChildElement);
			XmlElement requiredChildElement2 = base.GetRequiredChildElement(requestedSecurityTokenResponseElement, WSTrust.RequestedSecurityToken);
			XmlElement optionalChildElement = base.GetOptionalChildElement(requiredChildElement2, WSSecurityExtensions.BinarySecurityToken);
			if (optionalChildElement != null)
			{
				return new RequestedToken(optionalChildElement, null, null, null, lifetime);
			}
			XmlElement singleChildElement = base.GetSingleChildElement(requiredChildElement2);
			XmlElement requiredChildElement3 = base.GetRequiredChildElement(requestedSecurityTokenResponseElement, WSTrust.RequestedProofToken);
			XmlElement requiredChildElement4 = base.GetRequiredChildElement(requiredChildElement3, WSTrust.BinarySecret);
			byte[] symmetricKey = Convert.FromBase64String(requiredChildElement4.InnerText);
			SymmetricSecurityKey proofToken = new InMemorySymmetricSecurityKey(symmetricKey);
			XmlElement requiredChildElement5 = base.GetRequiredChildElement(requestedSecurityTokenResponseElement, WSTrust.RequestedAttachedReference);
			XmlElement singleChildElement2 = base.GetSingleChildElement(requiredChildElement5);
			XmlElement requiredChildElement6 = base.GetRequiredChildElement(requestedSecurityTokenResponseElement, WSTrust.RequestedUnattachedReference);
			XmlElement singleChildElement3 = base.GetSingleChildElement(requiredChildElement6);
			SecurityTokenService.Tracer.TraceDebug<SecurityTokenService, string>((long)this.GetHashCode(), "{0}: found encrypted security token in response: {1}", this, singleChildElement.OuterXml);
			return new RequestedToken(singleChildElement, singleChildElement2, singleChildElement3, proofToken, lifetime);
		}

		private static string GetIdentityNameFormat(IdentityType identityType)
		{
			switch (identityType)
			{
			case IdentityType.UPN:
				return "http://schemas.xmlsoap.org/claims/UPN";
			case IdentityType.ImmutableId:
				return "http://schemas.microsoft.com/LiveID/Federation/2008/05/ImmutableID";
			default:
				throw new ArgumentException("identityType");
			}
		}

		private string GetAllExceptionDetails(Exception e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				string value = ex.Message;
				SoapFaultException ex2 = ex as SoapFaultException;
				if (ex2 != null)
				{
					value = ex2.ToString();
				}
				else
				{
					SoapXmlMalformedException ex3 = ex as SoapXmlMalformedException;
					if (ex3 != null)
					{
						value = ex3.ToString();
					}
				}
				stringBuilder.AppendLine(value);
			}
			return stringBuilder.ToString();
		}

		private const int KeySize = 256;

		private const string KeySizeString = "256";

		private string requestor;

		private string policy;

		private readonly string stsClientIdentity;

		private Uri stsUri;

		private Uri endpoint;

		private X509Certificate2 certificate;

		private X509SubjectKeyIdentifierClause x509SubjectKeyIdentifierClause;

		private static readonly TimeSpan requestDuration = TimeSpan.FromMinutes(5.0);

		private static readonly Trace Tracer = ExTraceGlobals.WSTrustTracer;
	}
}
