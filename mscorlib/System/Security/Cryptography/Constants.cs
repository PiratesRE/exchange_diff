using System;

namespace System.Security.Cryptography
{
	internal static class Constants
	{
		internal const int S_OK = 0;

		internal const int NTE_FILENOTFOUND = -2147024894;

		internal const int NTE_NO_KEY = -2146893811;

		internal const int NTE_BAD_KEYSET = -2146893802;

		internal const int NTE_KEYSET_NOT_DEF = -2146893799;

		internal const int KP_IV = 1;

		internal const int KP_MODE = 4;

		internal const int KP_MODE_BITS = 5;

		internal const int KP_EFFECTIVE_KEYLEN = 19;

		internal const int ALG_CLASS_SIGNATURE = 8192;

		internal const int ALG_CLASS_DATA_ENCRYPT = 24576;

		internal const int ALG_CLASS_HASH = 32768;

		internal const int ALG_CLASS_KEY_EXCHANGE = 40960;

		internal const int ALG_TYPE_DSS = 512;

		internal const int ALG_TYPE_RSA = 1024;

		internal const int ALG_TYPE_BLOCK = 1536;

		internal const int ALG_TYPE_STREAM = 2048;

		internal const int ALG_TYPE_ANY = 0;

		internal const int CALG_MD5 = 32771;

		internal const int CALG_SHA1 = 32772;

		internal const int CALG_SHA_256 = 32780;

		internal const int CALG_SHA_384 = 32781;

		internal const int CALG_SHA_512 = 32782;

		internal const int CALG_RSA_KEYX = 41984;

		internal const int CALG_RSA_SIGN = 9216;

		internal const int CALG_DSS_SIGN = 8704;

		internal const int CALG_DES = 26113;

		internal const int CALG_RC2 = 26114;

		internal const int CALG_3DES = 26115;

		internal const int CALG_3DES_112 = 26121;

		internal const int CALG_AES_128 = 26126;

		internal const int CALG_AES_192 = 26127;

		internal const int CALG_AES_256 = 26128;

		internal const int CALG_RC4 = 26625;

		internal const int PROV_RSA_FULL = 1;

		internal const int PROV_DSS_DH = 13;

		internal const int PROV_RSA_AES = 24;

		internal const int AT_KEYEXCHANGE = 1;

		internal const int AT_SIGNATURE = 2;

		internal const int PUBLICKEYBLOB = 6;

		internal const int PRIVATEKEYBLOB = 7;

		internal const int CRYPT_OAEP = 64;

		internal const uint CRYPT_VERIFYCONTEXT = 4026531840U;

		internal const uint CRYPT_NEWKEYSET = 8U;

		internal const uint CRYPT_DELETEKEYSET = 16U;

		internal const uint CRYPT_MACHINE_KEYSET = 32U;

		internal const uint CRYPT_SILENT = 64U;

		internal const uint CRYPT_EXPORTABLE = 1U;

		internal const uint CLR_KEYLEN = 1U;

		internal const uint CLR_PUBLICKEYONLY = 2U;

		internal const uint CLR_EXPORTABLE = 3U;

		internal const uint CLR_REMOVABLE = 4U;

		internal const uint CLR_HARDWARE = 5U;

		internal const uint CLR_ACCESSIBLE = 6U;

		internal const uint CLR_PROTECTED = 7U;

		internal const uint CLR_UNIQUE_CONTAINER = 8U;

		internal const uint CLR_ALGID = 9U;

		internal const uint CLR_PP_CLIENT_HWND = 10U;

		internal const uint CLR_PP_PIN = 11U;

		internal const string OID_RSA_SMIMEalgCMS3DESwrap = "1.2.840.113549.1.9.16.3.6";

		internal const string OID_RSA_MD5 = "1.2.840.113549.2.5";

		internal const string OID_RSA_RC2CBC = "1.2.840.113549.3.2";

		internal const string OID_RSA_DES_EDE3_CBC = "1.2.840.113549.3.7";

		internal const string OID_OIWSEC_desCBC = "1.3.14.3.2.7";

		internal const string OID_OIWSEC_SHA1 = "1.3.14.3.2.26";

		internal const string OID_OIWSEC_SHA256 = "2.16.840.1.101.3.4.2.1";

		internal const string OID_OIWSEC_SHA384 = "2.16.840.1.101.3.4.2.2";

		internal const string OID_OIWSEC_SHA512 = "2.16.840.1.101.3.4.2.3";

		internal const string OID_OIWSEC_RIPEMD160 = "1.3.36.3.2.1";
	}
}
