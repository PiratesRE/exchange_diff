using System;

namespace System.Security.Cryptography.X509Certificates
{
	internal static class X509Constants
	{
		internal const uint CRYPT_EXPORTABLE = 1U;

		internal const uint CRYPT_USER_PROTECTED = 2U;

		internal const uint CRYPT_MACHINE_KEYSET = 32U;

		internal const uint CRYPT_USER_KEYSET = 4096U;

		internal const uint PKCS12_ALWAYS_CNG_KSP = 512U;

		internal const uint PKCS12_NO_PERSIST_KEY = 32768U;

		internal const uint CERT_QUERY_CONTENT_CERT = 1U;

		internal const uint CERT_QUERY_CONTENT_CTL = 2U;

		internal const uint CERT_QUERY_CONTENT_CRL = 3U;

		internal const uint CERT_QUERY_CONTENT_SERIALIZED_STORE = 4U;

		internal const uint CERT_QUERY_CONTENT_SERIALIZED_CERT = 5U;

		internal const uint CERT_QUERY_CONTENT_SERIALIZED_CTL = 6U;

		internal const uint CERT_QUERY_CONTENT_SERIALIZED_CRL = 7U;

		internal const uint CERT_QUERY_CONTENT_PKCS7_SIGNED = 8U;

		internal const uint CERT_QUERY_CONTENT_PKCS7_UNSIGNED = 9U;

		internal const uint CERT_QUERY_CONTENT_PKCS7_SIGNED_EMBED = 10U;

		internal const uint CERT_QUERY_CONTENT_PKCS10 = 11U;

		internal const uint CERT_QUERY_CONTENT_PFX = 12U;

		internal const uint CERT_QUERY_CONTENT_CERT_PAIR = 13U;

		internal const uint CERT_STORE_PROV_MEMORY = 2U;

		internal const uint CERT_STORE_PROV_SYSTEM = 10U;

		internal const uint CERT_STORE_NO_CRYPT_RELEASE_FLAG = 1U;

		internal const uint CERT_STORE_SET_LOCALIZED_NAME_FLAG = 2U;

		internal const uint CERT_STORE_DEFER_CLOSE_UNTIL_LAST_FREE_FLAG = 4U;

		internal const uint CERT_STORE_DELETE_FLAG = 16U;

		internal const uint CERT_STORE_SHARE_STORE_FLAG = 64U;

		internal const uint CERT_STORE_SHARE_CONTEXT_FLAG = 128U;

		internal const uint CERT_STORE_MANIFOLD_FLAG = 256U;

		internal const uint CERT_STORE_ENUM_ARCHIVED_FLAG = 512U;

		internal const uint CERT_STORE_UPDATE_KEYID_FLAG = 1024U;

		internal const uint CERT_STORE_BACKUP_RESTORE_FLAG = 2048U;

		internal const uint CERT_STORE_READONLY_FLAG = 32768U;

		internal const uint CERT_STORE_OPEN_EXISTING_FLAG = 16384U;

		internal const uint CERT_STORE_CREATE_NEW_FLAG = 8192U;

		internal const uint CERT_STORE_MAXIMUM_ALLOWED_FLAG = 4096U;

		internal const uint CERT_NAME_EMAIL_TYPE = 1U;

		internal const uint CERT_NAME_RDN_TYPE = 2U;

		internal const uint CERT_NAME_SIMPLE_DISPLAY_TYPE = 4U;

		internal const uint CERT_NAME_FRIENDLY_DISPLAY_TYPE = 5U;

		internal const uint CERT_NAME_DNS_TYPE = 6U;

		internal const uint CERT_NAME_URL_TYPE = 7U;

		internal const uint CERT_NAME_UPN_TYPE = 8U;
	}
}
