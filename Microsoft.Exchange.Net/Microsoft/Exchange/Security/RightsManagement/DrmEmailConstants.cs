using System;
using System.IO;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal static class DrmEmailConstants
	{
		public const int MajorStgVersion = 2;

		public const int MinorStgVersion = 2;

		public const int CurrentProtection = 0;

		public const uint StgCookie = 366883359U;

		public const string AttachmentList = "Attachment List";

		public const string AttachmentListInfo = "Attachment Info";

		public const string Attachment = "MailAttachment {0}";

		public const string BodyInfo = "OutlookBodyStreamInfo";

		public const string RTFBody = "BodyRTF";

		public const string PTHTMLBody = "BodyPT-HTML";

		public const string BodyPTAsHTML = "BodyPTAsHTML";

		public const string DRMStorageInformation = "RpmsgStorageInfo";

		public const string MailStreamInOleAttachment = "\u0003MailStream";

		public const string MailAttachment = "\u0003MailAttachment";

		public const string AttachDesc = "AttachDesc";

		public const string AttachPres = "AttachPres";

		public const string AttachContents = "AttachContents";

		public const string StgMessage = "MAPIMessage";

		public const ushort AttachDescVersion = 515;

		public const ushort AttachPresVersion = 256;

		public const string DRMContent = "\tDRMContent";

		public const string DataSpaces = "\u0006DataSpaces";

		public const string Version = "Version";

		public const string DataSpaceMap = "DataSpaceMap";

		public const string DataSpaceInfo = "DataSpaceInfo";

		public const string DRMDataSpace = "\tDRMDataSpace";

		public const string TransformInfo = "TransformInfo";

		public const string DRMTransform = "\tDRMTransform";

		public const string Primary = "\u0006Primary";

		public const string VersionFeature = "Microsoft.Container.DataSpaces";

		public const string DRMTransformFeature = "Microsoft.Metadata.DRMTransform";

		public const string DRMTransformClass = "{C73DFACD-061F-43B0-8B64-0C620D2A8B50}";

		public const string RACPrefix = "GIC";

		public const string CLCPrefix = "CLC";

		public const string DrmExtension = "drm";

		public const string RacDrmFilePattern = "GIC-*.drm";

		public const string ClcDrmFilePattern = "CLC-*.drm";

		public const string VersionDataValueMin = "1.1.0.0";

		public const string VersionDataValueMax = "1.1.0.0";

		public const int AcquireRmsTemplateMax = 25;

		public const int MaxSupportedRmsTemplates = 200;

		public const string ReachPackageVersionDataValueMin = "1.2.0.0";

		public const string ReachPackageVersionDataValueMax = "1.2.0.0";

		public const string RmsSvcPipelineMaxVersion = "1.0.0.0";

		public const string RmsSvcPipelineMinVersion = "1.0.0.0";

		public const string TemplateDistributionPipeline = "_wmcs/licensing/templatedistribution.asmx";

		public const string ServerCertificationPipeline = "_wmcs/certification/servercertification.asmx";

		public const string PublishPipeline = "_wmcs/licensing/publish.asmx";

		public const string LicensePipeline = "_wmcs/licensing/license.asmx";

		public const string LicenseServerPipeline = "_wmcs/licensing/server.asmx";

		public const string CertificationServerPipeline = "_wmcs/certification/server.asmx";

		public const string LicensingLocation = "_wmcs/licensing";

		public const string CertificationLocation = "_wmcs/certification";

		public const string MexSpecifier = "/mex";

		public const string WsdlSpecifier = "?wsdl";

		public const string XmlVersionTag = "<?xml version=\"1.0\"?>";

		public const string XrmlEndTag = "</XrML>";

		public const string ContentType = "Microsoft Office Document";

		public const string ContentName = "Microsoft Office Document";

		public const string ContentIdType = "MS-GUID";

		public const string IssueRights = "ISSUE";

		public const string MainRightsGroup = "Main-Rights";

		public const string WindowsAuthProvider = "WindowsAuthProvider";

		public const string ExchangeRecipientOrganizationExtractAllowedName = "ExchangeRecipientOrganizationExtractAllowed";

		public const string ExchangeRecipientOrganizationExtractAllowedValue = "true";

		public const string TestSetupRegKeyPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15\\Setup";

		public const string TestSetupRegValueName = "MsiInstallPath";

		public const string NoLicenseCacheName = "NOLICCACHE";

		public const string NoLicenseCacheValue = "1";

		public const string XmlDeclarationStart = "<?xml";

		public const string TemplateTypeAndXrmlDelimiter = ":";

		public const string XmlVersionTemplateAndTypeTag = ":<?xml";

		public static readonly Guid FileAttachmentObjectGuid = new Guid(454705, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

		public static readonly Guid MessageAttachmentObjectGuid = new Guid(454706, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

		public static readonly Guid MsgAttGuid = new Guid(134409, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

		public static readonly byte[] Null22Bytes = new byte[22];

		public static readonly RmsTemplate[] EmptyTemplateArray = new RmsTemplate[0];

		public static readonly string LicenseStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\DRM\\Server");

		public static byte[] CompressedDRMHeader = new byte[]
		{
			118,
			232,
			4,
			96,
			196,
			17,
			227,
			134
		};

		public static uint MagicDrmHeaderChecksum = 4000U;
	}
}
