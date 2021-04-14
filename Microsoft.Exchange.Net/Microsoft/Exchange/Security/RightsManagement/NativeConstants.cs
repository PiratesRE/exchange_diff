using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal static class NativeConstants
	{
		public const uint DrmCallbackVersion = 1U;

		internal const string TAGASCII = "ASCII Tag";

		internal const string TAGXRML = "XrML Tag";

		internal const string TAGFILENAME = "filename";

		internal const string TAGMSGUID = "MS-GUID";

		internal const string PLUGSTANDARDENABLINGPRINCIPAL = "UDStdPlg Enabling Principal";

		internal const string PLUGSTANDARDRIGHTSINTERPRETER = "XrMLv2a";

		internal const string PLUGSTANDARDEBDECRYPTOR = "UDStdPlg Enabling Bits Decryptor";

		internal const string PLUGSTANDARDEBENCRYPTOR = "UDStdPlg Enabling Bits Encryptor";

		internal const string PLUGSTANDARDEBCRYPTOPROVIDER = "UDStdPlg Enabling Bits Crypto Provider";

		internal const string PLUGSTANDARDLIBRARY = "UDStdPlg";

		internal const string ALGORITHMIDDES = "DES";

		internal const string ALGORITHMIDCOCKTAIL = "COCKTAIL";

		internal const string ALGORITHMIDAES = "AES";

		internal const string ALGORITHMIDRC4 = "RC4";

		internal const string QUERYOBJECTIDTYPE = "object-id-type";

		internal const string QUERYOBJECTID = "object-id";

		internal const string QUERYNAME = "name";

		internal const string QUERYCONTENTIDTYPE = "content-id-type";

		internal const string QUERYCONTENTIDVALUE = "content-id-value";

		internal const string QUERYCONTENTSKUTYPE = "content-sku-type";

		internal const string QUERYCONTENTSKUVALUE = "content-sku-value";

		internal const string QUERYMANIFESTSOURCE = "manifest-xrml";

		internal const string QUERYMACHINECERTSOURCE = "machine-certificate-xrml";

		internal const string QUERYAPIVERSION = "api-version";

		internal const string QUERYSECREPVERSION = "secrep-version";

		internal const string QUERYBLOCKSIZE = "block-size";

		internal const string QUERYSYMMETRICKEYTYPE = "symmetric-key-type";

		internal const string QUERYACCESSCONDITION = "access-condition";

		internal const string QUERYADDRESSTYPE = "address-type";

		internal const string QUERYADDRESSVALUE = "address-value";

		internal const string QUERYAPPDATANAME = "appdata-name";

		internal const string QUERYAPPDATAVALUE = "appdata-value";

		internal const string QUERYCONDITIONLIST = "condition-list";

		internal const string QUERYDISTRIBUTIONPOINT = "distribution-point";

		internal const string QUERYOBJECTTYPE = "object-type";

		internal const string QUERYENABLINGPRINCIPALIDTYPE = "enabling-principal-id-type";

		internal const string QUERYENABLINGPRINCIPALIDVALUE = "enabling-principal-id-value";

		internal const string QUERYGROUPIDENTITYPRINCIPAL = "group-identity-principal";

		internal const string QUERYFIRSTUSETAG = "first-use-tag";

		internal const string QUERYFROMTIME = "from-time";

		internal const string QUERYIDTYPE = "id-type";

		internal const string QUERYIDVALUE = "id-value";

		internal const string QUERYISSUEDPRINCIPAL = "issued-principal";

		internal const string QUERYISSUEDTIME = "issued-time";

		internal const string QUERYISSUER = "issuer";

		internal const string QUERYOWNER = "owner";

		internal const string QUERYPRINCIPAL = "principal";

		internal const string QUERYPRINCIPALIDVALUE = "principal-id-value";

		internal const string QUERYPRINCIPALIDTYPE = "principal-id-type";

		internal const string QUERYRANGETIMECONDITION = "rangetime-condition";

		internal const string QUERYOSEXCLUSIONCONDITION = "os-exclusion-condition";

		internal const string QUERYINTERVALTIMECONDITION = "intervaltime-condition";

		internal const string QUERYINTERVALTIMEINTERVAL = "intervaltime-interval";

		internal const string QUERYMAXVERSION = "max-version";

		internal const string QUERYMINVERSION = "min-version";

		internal const string QUERYREFRESHPERIOD = "refresh-period";

		internal const string QUERYREVOCATIONCONDITION = "revocation-condition";

		internal const string QUERYRIGHT = "right";

		internal const string QUERYRIGHTSGROUP = "rights-group";

		internal const string QUERYRIGHTSPARAMETERNAME = "rights-parameter-name";

		internal const string QUERYRIGHTSPARAMETERVALUE = "rights-parameter-value";

		internal const string QUERYSKUTYPE = "sku-type";

		internal const string QUERYSKUVALUE = "sku-value";

		internal const string QUERYTIMEINTERVAL = "time-interval";

		internal const string QUERYUNTILTIME = "until-time";

		internal const string QUERYVALIDITYFROMTIME = "valid-from";

		internal const string QUERYVALIDITYUNTILTIME = "valid-until";

		internal const string QUERYWORK = "work";
	}
}
