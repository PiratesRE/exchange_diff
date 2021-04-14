using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[SuppressUnmanagedCodeSecurity]
	[SecurityCritical(SecurityCriticalScope.Everything)]
	internal static class SafeNativeMethods
	{
		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateClientSession([MarshalAs(UnmanagedType.FunctionPtr)] [In] CallbackDelegate pfnCallback, [MarshalAs(UnmanagedType.U4)] [In] uint uCallbackVersion, [MarshalAs(UnmanagedType.LPWStr)] [In] string GroupIDProviderType, [MarshalAs(UnmanagedType.LPWStr)] [In] string GroupID, out SafeRightsManagementSessionHandle phSession);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCloseSession([MarshalAs(UnmanagedType.U4)] [In] uint sessionHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCloseHandle([MarshalAs(UnmanagedType.U4)] [In] uint handle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCloseQueryHandle([MarshalAs(UnmanagedType.U4)] [In] uint queryHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCloseEnvironmentHandle([MarshalAs(UnmanagedType.U4)] [In] uint envHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMIsActivated([In] SafeRightsManagementSessionHandle hSession, [MarshalAs(UnmanagedType.U4)] [In] uint uFlags, [MarshalAs(UnmanagedType.LPStruct)] [In] ActivationServerInfo activationServerInfo);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMActivate([In] SafeRightsManagementSessionHandle hSession, [MarshalAs(UnmanagedType.U4)] [In] uint uFlags, [MarshalAs(UnmanagedType.U4)] [In] uint uLangID, [MarshalAs(UnmanagedType.LPStruct)] [In] ActivationServerInfo activationServerInfo, IntPtr context, IntPtr parentWindowHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateLicenseStorageSession([In] SafeRightsManagementEnvironmentHandle envHandle, [In] SafeRightsManagementHandle hDefLib, [In] SafeRightsManagementSessionHandle hClientSession, [MarshalAs(UnmanagedType.U4)] [In] uint uFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string IssuanceLicense, out SafeRightsManagementSessionHandle phLicenseStorageSession);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMAcquireLicense([In] SafeRightsManagementSessionHandle hSession, [MarshalAs(UnmanagedType.U4)] [In] uint uFlags, [MarshalAs(UnmanagedType.LPWStr)] [In] string GroupIdentityCredential, [MarshalAs(UnmanagedType.LPWStr)] [In] string RequestedRights, [MarshalAs(UnmanagedType.LPWStr)] [In] string CustomData, [MarshalAs(UnmanagedType.LPWStr)] [In] string url, IntPtr context);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMEnumerateLicense([In] SafeRightsManagementSessionHandle hSession, [MarshalAs(UnmanagedType.U4)] [In] EnumerateLicenseFlags uFlags, [MarshalAs(UnmanagedType.U4)] [In] uint uIndex, [MarshalAs(UnmanagedType.Bool)] [In] [Out] ref bool pfSharedFlag, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint puCertDataLen, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder wszCertificateData);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetServiceLocation([In] SafeRightsManagementSessionHandle clientSessionHandle, [MarshalAs(UnmanagedType.U4)] [In] ServiceType serviceType, [MarshalAs(UnmanagedType.U4)] [In] ServiceLocation serviceLocation, [MarshalAs(UnmanagedType.LPWStr)] [In] string issuanceLicense, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint serviceUrlLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder serviceUrl);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMDeconstructCertificateChain([MarshalAs(UnmanagedType.LPWStr)] [In] string chain, [MarshalAs(UnmanagedType.U4)] [In] uint index, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint certificateLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder certificate);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetCertificateChainCount([MarshalAs(UnmanagedType.LPWStr)] [In] string chain, [MarshalAs(UnmanagedType.U4)] out uint certCount);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMParseUnboundLicense([MarshalAs(UnmanagedType.LPWStr)] [In] string certificate, out SafeRightsManagementQueryHandle queryRootHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetUnboundLicenseObjectCount([In] SafeRightsManagementQueryHandle queryRootHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string subObjectType, [MarshalAs(UnmanagedType.U4)] out uint objectCount);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetUnboundLicenseAttributeCount([In] SafeRightsManagementQueryHandle queryRootHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string subAttributeType, [MarshalAs(UnmanagedType.U4)] out uint attributeCount);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetBoundLicenseObject([In] SafeRightsManagementHandle queryRootHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string subObjectType, [MarshalAs(UnmanagedType.U4)] [In] uint index, out SafeRightsManagementHandle subQueryHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetUnboundLicenseObject([In] SafeRightsManagementQueryHandle queryRootHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string subObjectType, [MarshalAs(UnmanagedType.U4)] [In] uint index, out SafeRightsManagementQueryHandle subQueryHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetUnboundLicenseAttribute([In] SafeRightsManagementQueryHandle queryRootHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string attributeType, [MarshalAs(UnmanagedType.U4)] [In] uint index, [MarshalAs(UnmanagedType.U4)] out uint encodingType, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint bufferSize, byte[] buffer);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetBoundLicenseAttribute([In] SafeRightsManagementHandle queryRootHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string attributeType, [MarshalAs(UnmanagedType.U4)] [In] uint index, [MarshalAs(UnmanagedType.U4)] out uint encodingType, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint bufferSize, byte[] buffer);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateIssuanceLicense([MarshalAs(UnmanagedType.LPStruct)] [In] SystemTime timeFrom, [MarshalAs(UnmanagedType.LPStruct)] [In] SystemTime timeUntil, [MarshalAs(UnmanagedType.LPWStr)] [In] string referralInfoName, [MarshalAs(UnmanagedType.LPWStr)] [In] string referralInfoUrl, [In] SafeRightsManagementPubHandle ownerUserHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string issuanceLicense, [In] SafeRightsManagementHandle boundLicenseHandle, out SafeRightsManagementPubHandle issuanceLicenseHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateUser([MarshalAs(UnmanagedType.LPWStr)] [In] string userName, [MarshalAs(UnmanagedType.LPWStr)] [In] string userId, [MarshalAs(UnmanagedType.LPWStr)] [In] string userIdType, out SafeRightsManagementPubHandle userHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetUsers([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] uint index, out SafeRightsManagementPubHandle userHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetUserRights([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In] SafeRightsManagementPubHandle userHandle, [MarshalAs(UnmanagedType.U4)] [In] uint index, out SafeRightsManagementPubHandle rightHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetUserInfo([In] SafeRightsManagementPubHandle userHandle, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint userNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder userName, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint userIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder userId, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint userIdTypeLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder userIdType);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetRightInfo([In] SafeRightsManagementPubHandle rightHandle, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint rightNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder rightName, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeFrom, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeUntil);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateRight([MarshalAs(UnmanagedType.LPWStr)] [In] string rightName, [MarshalAs(UnmanagedType.LPStruct)] [In] SystemTime timeFrom, [MarshalAs(UnmanagedType.LPStruct)] [In] SystemTime timeUntil, [MarshalAs(UnmanagedType.U4)] [In] uint countExtendedInfo, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] string[] extendedInfoNames, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] string[] extendedInfoValues, out SafeRightsManagementPubHandle rightHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetIssuanceLicenseTemplate([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint issuanceLicenseTemplateLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder issuanceLicenseTemplate);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMClosePubHandle([MarshalAs(UnmanagedType.U4)] [In] uint pubHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMAddRightWithUser([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In] SafeRightsManagementPubHandle rightHandle, [In] SafeRightsManagementPubHandle userHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMSetMetaData([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string contentId, [MarshalAs(UnmanagedType.LPWStr)] [In] string contentIdType, [MarshalAs(UnmanagedType.LPWStr)] [In] string SkuId, [MarshalAs(UnmanagedType.LPWStr)] [In] string SkuIdType, [MarshalAs(UnmanagedType.LPWStr)] [In] string contentType, [MarshalAs(UnmanagedType.LPWStr)] [In] string contentName);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetIssuanceLicenseInfo([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeFrom, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeUntil, [MarshalAs(UnmanagedType.U4)] [In] uint flags, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint distributionPointNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder DistributionPointName, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint distributionPointUriLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder DistributionPointUri, out SafeRightsManagementPubHandle ownerHandle, [MarshalAs(UnmanagedType.Bool)] out bool officialFlag);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetSecurityProvider([MarshalAs(UnmanagedType.U4)] [In] uint flags, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint typeLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder type, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint pathLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMDeleteLicense([In] SafeRightsManagementSessionHandle hSession, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszLicenseId);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMSetNameAndDescription([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.Bool)] [In] bool flagDelete, [MarshalAs(UnmanagedType.U4)] [In] uint localeId, [MarshalAs(UnmanagedType.LPWStr)] [In] string name, [MarshalAs(UnmanagedType.LPWStr)] [In] string description);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetNameAndDescription([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] uint uIndex, [MarshalAs(UnmanagedType.U4)] out uint localeId, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint nameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint descriptionLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder description);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetSignedIssuanceLicense([In] SafeRightsManagementEnvironmentHandle environmentHandle, [In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] SignIssuanceLicenseFlags flags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] [In] byte[] symmetricKey, [MarshalAs(UnmanagedType.U4)] [In] uint symmetricKeyByteCount, [MarshalAs(UnmanagedType.LPWStr)] [In] string symmetricKeyType, [MarshalAs(UnmanagedType.LPWStr)] [In] string clientLicensorCertificate, [MarshalAs(UnmanagedType.FunctionPtr)] [In] CallbackDelegate pfnCallback, [MarshalAs(UnmanagedType.LPWStr)] [In] string url, [In] IntPtr context);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetSignedIssuanceLicenseEx([In] SafeRightsManagementEnvironmentHandle environmentHandle, [In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] SignIssuanceLicenseFlags flags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] [In] byte[] symmetricKey, [MarshalAs(UnmanagedType.U4)] [In] uint symmetricKeyByteCount, [MarshalAs(UnmanagedType.LPWStr)] [In] string symmetricKeyType, [In] IntPtr pReserved, [In] SafeRightsManagementHandle enablingPrincipalHandle, [In] SafeRightsManagementHandle boundLicenseCLCHandle, [MarshalAs(UnmanagedType.FunctionPtr)] [In] CallbackDelegate pfnCallback, [In] IntPtr context);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetOwnerLicense([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint ownerLicenseLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder ownerLicense);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateBoundLicense([In] SafeRightsManagementEnvironmentHandle environmentHandle, [MarshalAs(UnmanagedType.LPStruct)] [In] BoundLicenseParams boundLicenseParams, [MarshalAs(UnmanagedType.LPWStr)] [In] string licenseChain, out SafeRightsManagementHandle boundLicenseHandle, [MarshalAs(UnmanagedType.U4)] out uint errorLogHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateEnablingBitsDecryptor([In] SafeRightsManagementHandle boundLicenseHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string right, [MarshalAs(UnmanagedType.U4)] [In] uint auxLibrary, [MarshalAs(UnmanagedType.LPWStr)] [In] string auxPlugin, out SafeRightsManagementHandle decryptorHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateEnablingBitsEncryptor([In] SafeRightsManagementHandle boundLicenseHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string right, [MarshalAs(UnmanagedType.U4)] [In] uint auxLibrary, [MarshalAs(UnmanagedType.LPWStr)] [In] string auxPlugin, out SafeRightsManagementHandle encryptorHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMDecrypt([In] SafeRightsManagementHandle cryptoProvHandle, [MarshalAs(UnmanagedType.U4)] [In] uint position, [MarshalAs(UnmanagedType.U4)] [In] uint inputByteCount, byte[] inputBuffer, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint outputByteCount, byte[] outputBuffer);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMEncrypt([In] SafeRightsManagementHandle cryptoProvHandle, [MarshalAs(UnmanagedType.U4)] [In] uint position, [MarshalAs(UnmanagedType.U4)] [In] uint inputByteCount, byte[] inputBuffer, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint outputByteCount, byte[] outputBuffer);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetInfo([In] SafeRightsManagementHandle handle, [MarshalAs(UnmanagedType.LPWStr)] [In] string attributeType, [MarshalAs(UnmanagedType.U4)] out uint encodingType, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint outputByteCount, byte[] outputBuffer);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetApplicationSpecificData([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] uint index, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint nameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint valueLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMSetApplicationSpecificData([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.Bool)] [In] bool flagDelete, [MarshalAs(UnmanagedType.LPWStr)] [In] string name, [MarshalAs(UnmanagedType.LPWStr)] [In] string value);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetIntervalTime([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint days);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMSetIntervalTime([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] uint days);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetRevocationPoint([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint idLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder id, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint idTypeLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder idType, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint urlLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder url, [MarshalAs(UnmanagedType.LPStruct)] SystemTime frequency, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint nameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint publicKeyLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder publicKey);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMSetRevocationPoint([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.Bool)] [In] bool flagDelete, [MarshalAs(UnmanagedType.LPWStr)] [In] string id, [MarshalAs(UnmanagedType.LPWStr)] [In] string idType, [MarshalAs(UnmanagedType.LPWStr)] [In] string url, [MarshalAs(UnmanagedType.LPStruct)] [In] SystemTime frequency, [MarshalAs(UnmanagedType.LPWStr)] [In] string name, [MarshalAs(UnmanagedType.LPWStr)] [In] string publicKey);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMSetGlobalOptions([MarshalAs(UnmanagedType.U4)] [In] GlobalOptions globalOptions, IntPtr dataPtr, [MarshalAs(UnmanagedType.U4)] [In] int length);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMGetClientVersion([MarshalAs(UnmanagedType.LPStruct)] [In] [Out] DRMClientVersionInfo drmClientVersionInfo);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMRegisterRevocationList([In] SafeRightsManagementEnvironmentHandle environmentHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string revocationList);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMAcquireIssuanceLicenseTemplate([In] SafeRightsManagementSessionHandle hSession, [MarshalAs(UnmanagedType.U4)] [In] TemplateDistribution uFlags, [In] IntPtr pvReserved, [MarshalAs(UnmanagedType.U4)] [In] uint cReserved, [In] IntPtr pwszReserved, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszURL, [In] IntPtr context);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMCreateEnablingPrincipal([In] SafeRightsManagementEnvironmentHandle environmentHandle, [In] SafeRightsManagementHandle libraryHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszObject, [MarshalAs(UnmanagedType.LPStruct)] [In] [Out] DRMId idPrincipal, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszCredentials, out SafeRightsManagementHandle enablingPrincipalHandle);

		[DllImport("msdrm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		internal static extern int DRMDuplicateHandle([In] SafeRightsManagementHandle drmHandle, out SafeRightsManagementHandle duplicateHandle);

		private const string MsDrm = "msdrm.dll";
	}
}
