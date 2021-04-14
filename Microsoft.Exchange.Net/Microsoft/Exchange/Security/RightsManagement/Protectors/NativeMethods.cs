using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.Protectors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class NativeMethods
	{
		[DllImport("RightsManagementWrapper.dll", ExactSpelling = true)]
		public static extern int HrCreateIrmCrypt([In] SafeRightsManagementHandle encryptorHandle, [In] SafeRightsManagementHandle decryptorHandle, [MarshalAs(UnmanagedType.Interface)] out object irmCrypt);

		public const int S_OK = 0;

		public const int E_UNEXPECTED = -2147418113;

		public const int E_FAIL = -2147467259;

		public const int E_INVALIDARG = -2147024809;

		public const int E_NOTIMPL = -2147467263;

		public const int STG_E_FILENOTFOUND = -2147287038;

		private const string RightsManagementWrapper = "RightsManagementWrapper.dll";

		public enum XmlError : uint
		{
			MX_E_MX = 3222072832U,
			MX_E_INPUTEND,
			MX_E_ENCODING,
			MX_E_ENCODINGSWITCH,
			MX_E_ENCODINGSIGNATURE,
			WC_E_WC = 3222072864U,
			WC_E_WHITESPACE,
			WC_E_SEMICOLON,
			WC_E_GREATERTHAN,
			WC_E_QUOTE,
			WC_E_EQUAL,
			WC_E_LESSTHAN,
			WC_E_HEXDIGIT,
			WC_E_DIGIT,
			WC_E_LEFTBRACKET,
			WC_E_LEFTPAREN,
			WC_E_XMLCHARACTER,
			WC_E_NAMECHARACTER,
			WC_E_SYNTAX,
			WC_E_CDSECT,
			WC_E_COMMENT,
			WC_E_CONDSECT,
			WC_E_DECLATTLIST,
			WC_E_DECLDOCTYPE,
			WC_E_DECLELEMENT,
			WC_E_DECLENTITY,
			WC_E_DECLNOTATION,
			WC_E_NDATA,
			WC_E_PUBLIC,
			WC_E_SYSTEM,
			WC_E_NAME,
			WC_E_ROOTELEMENT,
			WC_E_ELEMENTMATCH,
			WC_E_UNIQUEATTRIBUTE,
			WC_E_TEXTXMLDECL,
			WC_E_LEADINGXML,
			WC_E_TEXTDECL,
			WC_E_XMLDECL,
			WC_E_ENCNAME,
			WC_E_PUBLICID,
			WC_E_PESINTERNALSUBSET,
			WC_E_PESBETWEENDECLS,
			WC_E_NORECURSION,
			WC_E_ENTITYCONTENT,
			WC_E_UNDECLAREDENTITY,
			WC_E_PARSEDENTITY,
			WC_E_NOEXTERNALENTITYREF,
			WC_E_PI,
			WC_E_SYSTEMID,
			WC_E_QUESTIONMARK,
			WC_E_CDSECTEND,
			WC_E_MOREDATA,
			WC_E_DTDPROHIBITED,
			WC_E_INVALIDXMLSPACE,
			NC_E_NC = 3222072928U,
			NC_E_QNAMECHARACTER,
			NC_E_QNAMECOLON,
			NC_E_NAMECOLON,
			NC_E_DECLAREDPREFIX,
			NC_E_UNDECLAREDPREFIX,
			NC_E_EMPTYURI,
			NC_E_XMLPREFIXRESERVED,
			NC_E_XMLNSPREFIXRESERVED,
			NC_E_XMLURIRESERVED,
			NC_E_XMLNSURIRESERVED,
			SC_E_SC = 3222072960U,
			SC_E_MAXELEMENTDEPTH,
			SC_E_MAXENTITYEXPANSION,
			WR_E_WR = 3222073088U,
			WR_E_NONWHITESPACE,
			WR_E_NSPREFIXDECLARED,
			WR_E_NSPREFIXWITHEMPTYNSURI,
			WR_E_DUPLICATEATTRIBUTE,
			WR_E_XMLNSPREFIXDECLARATION,
			WR_E_XMLPREFIXDECLARATION,
			WR_E_XMLURIDECLARATION,
			WR_E_XMLNSURIDECLARATION,
			WR_E_NAMESPACEUNDECLARED,
			WR_E_INVALIDXMLSPACE,
			WR_E_INVALIDACTION,
			WR_E_INVALIDSURROGATEPAIR,
			XML_E_INVALID_DECIMAL = 3222069277U,
			XML_E_INVALID_HEXIDECIMAL,
			XML_E_INVALID_UNICODE,
			XML_E_INVALIDENCODING = 3222069358U
		}
	}
}
