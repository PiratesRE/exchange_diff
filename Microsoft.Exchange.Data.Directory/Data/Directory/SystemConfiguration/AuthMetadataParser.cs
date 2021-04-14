using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel.Security;
using System.Web.Script.Serialization;
using System.Xml;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class AuthMetadataParser
	{
		public static AuthMetadataParser.MetadataDocType DecideMetadataDocumentType(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentException("Metadata endpoint url cannot be null");
			}
			if (url.IndexOf("federationmetadata", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return AuthMetadataParser.MetadataDocType.WSFedMetadata;
			}
			if (url.IndexOf("metadata/json/1", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return AuthMetadataParser.MetadataDocType.OAuthS2SV1Metadata;
			}
			if (url.IndexOf("openid-configuration", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return AuthMetadataParser.MetadataDocType.OAuthOpenIdConnectMetadata;
			}
			return AuthMetadataParser.MetadataDocType.Unknown;
		}

		public static T GetDocument<T>(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetaDataContentEmpty);
			}
			T result;
			try
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				T t = javaScriptSerializer.Deserialize<T>(content);
				result = t;
			}
			catch (ArgumentException innerException)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorCannotParseAuthMetadata, innerException);
			}
			catch (InvalidOperationException innerException2)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorCannotParseAuthMetadata, innerException2);
			}
			return result;
		}

		public static AuthMetadata GetAuthMetadata(string content, bool requireIssuingEndpoint)
		{
			JsonMetadataDocument document = AuthMetadataParser.GetDocument<JsonMetadataDocument>(content);
			AuthMetadata authMetadata = new AuthMetadata();
			AuthMetadataParser.ExtractServiceNameRealmAndIssuer(document, authMetadata);
			if (document.keys == null)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoSigningKey);
			}
			string[] array = (from k in document.keys
			where string.Equals(k.usage, AuthMetadataConstants.KeyUsage, StringComparison.OrdinalIgnoreCase) && k.keyvalue != null && string.Equals(k.keyvalue.type, AuthMetadataConstants.SigningKeyType, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(k.keyvalue.value)
			select k.keyvalue.value).ToArray<string>();
			if (array.Length == 0)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoSigningKey);
			}
			authMetadata.CertificateStrings = array;
			if (document.endpoints != null)
			{
				string[] array2 = (from e in document.endpoints
				where string.Equals(e.protocol, AuthMetadataConstants.Protocol, StringComparison.OrdinalIgnoreCase) && string.Equals(e.usage, AuthMetadataConstants.IssuingEndpointUsage, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(e.location)
				select e.location).ToArray<string>();
				if (array2.Length > 0)
				{
					authMetadata.IssuingEndpoint = array2[0];
				}
			}
			if (requireIssuingEndpoint && string.IsNullOrEmpty(authMetadata.IssuingEndpoint))
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoIssuingEndpoint);
			}
			return authMetadata;
		}

		public static AuthMetadata GetOpenIdConnectAuthMetadata(string content, bool requireIssuingEndpoint)
		{
			OpenIdConnectJsonMetadataDocument document = AuthMetadataParser.GetDocument<OpenIdConnectJsonMetadataDocument>(content);
			AuthMetadata authMetadata = new AuthMetadata();
			if (!string.IsNullOrEmpty(document.token_endpoint))
			{
				authMetadata.IssuingEndpoint = document.token_endpoint.Trim();
			}
			if (requireIssuingEndpoint && string.IsNullOrEmpty(authMetadata.IssuingEndpoint))
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoIssuingEndpoint);
			}
			string serviceName;
			string realm;
			if (AuthMetadataParser.TryExtractUrlFormatServiceNameAndRealm(document.issuer, out serviceName, out realm))
			{
				authMetadata.ServiceName = serviceName;
				authMetadata.Realm = realm;
			}
			if (string.IsNullOrEmpty(authMetadata.ServiceName))
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataCannotResolveServiceName);
			}
			if (!string.IsNullOrEmpty(document.jwks_uri))
			{
				authMetadata.KeysEndpoint = document.jwks_uri.Trim();
			}
			if (string.IsNullOrEmpty(authMetadata.KeysEndpoint))
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoSigningKey);
			}
			if (!string.IsNullOrEmpty(document.authorization_endpoint))
			{
				authMetadata.AuthorizationEndpoint = document.authorization_endpoint.Trim();
			}
			if (string.IsNullOrEmpty(authMetadata.AuthorizationEndpoint))
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoIssuingEndpoint);
			}
			return authMetadata;
		}

		public static AuthMetadata GetOpenIdConnectKeys(string content, AuthMetadata authData)
		{
			OpenIdConnectKeysJsonMetadataDocument document = AuthMetadataParser.GetDocument<OpenIdConnectKeysJsonMetadataDocument>(content);
			if (document.keys == null)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoSigningKey);
			}
			if (authData == null)
			{
				authData = new AuthMetadata();
			}
			string[] array = (from k in document.keys
			where !string.IsNullOrEmpty(k.use) && k.use.Equals(AuthMetadataConstants.OpenIdConnectSigningKeyUsage, StringComparison.OrdinalIgnoreCase) && k.x5c != null && k.x5c.Length > 0
			select k.x5c[0]).ToArray<string>();
			if (array.Length == 0)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataNoSigningKey);
			}
			authData.CertificateStrings = array;
			return authData;
		}

		public static AuthMetadata GetWSFederationMetadata(string content)
		{
			AuthMetadata result;
			try
			{
				using (TextReader textReader = new StringReader(content))
				{
					using (XmlReader xmlReader = XmlReader.Create(textReader))
					{
						MetadataSerializer metadataSerializer = new MetadataSerializer
						{
							CertificateValidationMode = X509CertificateValidationMode.None
						};
						EntityDescriptor entityDescriptor = metadataSerializer.ReadMetadata(xmlReader) as EntityDescriptor;
						SecurityTokenServiceDescriptor securityTokenServiceDescriptor = entityDescriptor.RoleDescriptors.OfType<SecurityTokenServiceDescriptor>().First<SecurityTokenServiceDescriptor>();
						List<string> list = new List<string>();
						foreach (KeyDescriptor keyDescriptor in from k in securityTokenServiceDescriptor.Keys
						where k.Use == KeyType.Signing
						select k)
						{
							foreach (SecurityKeyIdentifierClause securityKeyIdentifierClause in keyDescriptor.KeyInfo)
							{
								X509RawDataKeyIdentifierClause x509RawDataKeyIdentifierClause = securityKeyIdentifierClause as X509RawDataKeyIdentifierClause;
								if (x509RawDataKeyIdentifierClause != null)
								{
									list.Add(Convert.ToBase64String(x509RawDataKeyIdentifierClause.GetX509RawData()));
								}
							}
						}
						AuthMetadata authMetadata = new AuthMetadata();
						authMetadata.CertificateStrings = list.ToArray();
						string serviceName;
						string realm;
						if (AuthMetadataParser.TryExtractUrlFormatServiceNameAndRealm(entityDescriptor.EntityId.Id, out serviceName, out realm))
						{
							authMetadata.ServiceName = serviceName;
							authMetadata.Realm = realm;
						}
						else
						{
							authMetadata.ServiceName = entityDescriptor.EntityId.Id;
							authMetadata.Realm = null;
						}
						if (string.IsNullOrEmpty(authMetadata.ServiceName))
						{
							throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataCannotResolveServiceName);
						}
						result = authMetadata;
					}
				}
			}
			catch (XmlException innerException)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorCannotParseAuthMetadata, innerException);
			}
			catch (IOException innerException2)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorCannotParseAuthMetadata, innerException2);
			}
			catch (SecurityException innerException3)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorCannotParseAuthMetadata, innerException3);
			}
			catch (SystemException innerException4)
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorCannotParseAuthMetadata, innerException4);
			}
			return result;
		}

		public static void SetEndpointsIfWSFed(AuthMetadata authData, AuthServerType authServerType, string authMetadataUrl)
		{
			AuthMetadataParser.MetadataDocType metadataDocType = AuthMetadataParser.DecideMetadataDocumentType(authMetadataUrl);
			if (metadataDocType != AuthMetadataParser.MetadataDocType.WSFedMetadata)
			{
				return;
			}
			Uri uri = new Uri(authMetadataUrl);
			authData.AuthorizationEndpoint = string.Format("{0}/{1}/oauth2/authorize", uri.GetLeftPart(UriPartial.Authority), (authServerType == AuthServerType.ADFS) ? "adfs" : "common");
			authData.IssuingEndpoint = string.Format("{0}/{1}/oauth2/token", uri.GetLeftPart(UriPartial.Authority), (authServerType == AuthServerType.ADFS) ? "adfs" : "common");
		}

		private static void ExtractServiceNameRealmAndIssuer(JsonMetadataDocument document, AuthMetadata authData)
		{
			string serviceName;
			string realm;
			if (document.version == AuthMetadataConstants.SelfIssuingAuthorityMetadataVersion)
			{
				authData.ServiceName = document.serviceName;
				authData.Issuer = document.issuer;
				authData.Realm = document.realm;
				if (string.IsNullOrEmpty(authData.Issuer))
				{
					throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataCannotResolveIssuer);
				}
			}
			else if (AuthMetadataParser.TryExtractServiceNameAndRealm(document.issuer, out serviceName, out realm))
			{
				authData.ServiceName = serviceName;
				authData.Realm = realm;
			}
			else if (!string.IsNullOrEmpty(document.serviceName))
			{
				authData.ServiceName = document.serviceName;
				authData.Realm = document.realm;
			}
			if (string.IsNullOrEmpty(authData.ServiceName))
			{
				throw new AuthMetadataParserException(DirectoryStrings.ErrorAuthMetadataCannotResolveServiceName);
			}
		}

		private static bool TryExtractServiceNameAndRealm(string issuerOrAudience, out string serviceName, out string realm)
		{
			serviceName = null;
			realm = null;
			if (issuerOrAudience == null)
			{
				return false;
			}
			int num = issuerOrAudience.IndexOf('@');
			if (num == -1)
			{
				return false;
			}
			serviceName = issuerOrAudience.Substring(0, num);
			realm = issuerOrAudience.Substring(num + 1);
			return !string.IsNullOrEmpty(serviceName);
		}

		private static bool TryExtractUrlFormatServiceNameAndRealm(string issuer, out string serviceName, out string realm)
		{
			serviceName = null;
			realm = null;
			if (string.IsNullOrEmpty(issuer))
			{
				return false;
			}
			Uri uri = null;
			try
			{
				uri = new Uri(issuer);
			}
			catch (UriFormatException)
			{
				return false;
			}
			if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
			{
				return false;
			}
			serviceName = issuer;
			if (uri.OriginalString.IndexOf(AuthMetadataConstants.AzureADTenantIdTemplate, StringComparison.OrdinalIgnoreCase) < 0 && uri.Segments.Length > 1)
			{
				realm = (uri.Segments[1].EndsWith("/") ? uri.Segments[1].Substring(0, uri.Segments[1].Length - 1) : uri.Segments[1]);
				Guid guid;
				if (!Guid.TryParse(realm, out guid))
				{
					realm = null;
				}
			}
			return !string.IsNullOrEmpty(serviceName);
		}

		public enum MetadataDocType
		{
			Unknown,
			OAuthS2SV1Metadata,
			WSFedMetadata,
			OAuthOpenIdConnectMetadata
		}
	}
}
