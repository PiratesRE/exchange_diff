using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal static class Constants
	{
		public static class Soap
		{
			public const string UpnUri = "http://schemas.xmlsoap.org/claims/UPN";

			public const string IdentityClaimUri = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";

			public const string GenericClaimUri = "http://schemas.xmlsoap.org/claims";
		}

		public static class Saml
		{
			public const string AuthenticationMethodPasswordUri = "urn:oasis:names:tc:SAML:1.0:am:password";

			public const string TokenProfile11 = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1";

			public const string EmailAddressClaimName = "EmailAddress";

			public const string EmailAddressListClaimName = "EmailAddressList";

			public const string ConfirmationSenderVouches = "urn:oasis:names:tc:SAML:1.0:cm:sender-vouches";

			public const string EmailAddress = "http://schemas.xmlsoap.org/claims/EmailAddress";

			public const string EmailAddressList = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/EmailAddressList";

			public const string TokenProfileValueType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0#SAMLAssertionID";
		}

		public static class WSAddressing
		{
			public const string Anonymous = "http://www.w3.org/2005/08/addressing/anonymous";
		}

		public static class WSTrust
		{
			public const string Issue = "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue";

			public const string RequestTypeIssue = "http://schemas.xmlsoap.org/ws/2005/02/trust/Issue";

			public const string BinarySecretTypeNonce = "http://schemas.xmlsoap.org/ws/2005/02/trust/Nonce";

			public const string SymmetricKey = "http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey";

			public const string PSHA1 = "http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1";
		}

		public static class XmlEncryption
		{
			public const string C14Canonalization = "http://www.w3.org/2001/10/xml-exc-c14n#";

			public const string AES256_CBC = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";
		}

		public static class XmlSignature
		{
			public const string HMAC_SHA1 = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
		}

		public static class WSAuthorization
		{
			public const string RequestorScope = "http://schemas.xmlsoap.org/ws/2006/12/authorization/ctx/requestor";

			public const string Action = "http://schemas.xmlsoap.org/ws/2006/12/authorization/claims/action";

			public const string Dialect = "http://schemas.xmlsoap.org/ws/2006/12/authorization/authclaims";
		}

		public static class MicrosoftWLID
		{
			public const string RequestorName = "http://schemas.microsoft.com/wlid/requestor";

			public const string ImmutableIdUri = "http://schemas.microsoft.com/LiveID/Federation/2008/05/ImmutableID";
		}

		public static class X509TokenProfile
		{
			public const string X509v3 = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3";

			public const string SKI = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509SubjectKeyIdentifier";
		}
	}
}
