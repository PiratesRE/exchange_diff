using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	public static class ValidationConstant
	{
		public const string DNSPort = "53";

		public const string PrimaryDNSPortString = "PrimaryDNSPort";

		public const string Exchange2000SerialNumber = "Version 6.0";

		public const string Exchange2003SerialNumber = "Version 6.5";

		public const string Exchange200xSerialNumber = "Version 6";

		public const string ExchangeProductName = "Exchange 15";

		public const string FileInUseProcessExclusionListPattern = "^(Setup|Exsetup|ExSetupUI|WmiPrvSE|MOM|MonitoringHost|w3wp|msftesql|msftefd|EdgeTransport|mad|store|umservice|UMWorkerProcess|TranscodingService|SESWorker|ExBPA|ExFBA|wsbexchange|hostcontrollerservice|noderunner|parserserver|Microsoft\\.Exchange\\..*|MSExchange.*|fms|scanningprocess|FSCConfigurationServer|updateservice|ScanEngineTest|EngineUpdateLogger|sftracing|ForefrontActiveDirectoryConnector|rundll32|MSMessageTracingClient)$";

		public const string DatacenterFileInUseProcessExclusionListPattern = "^(Setup|Exsetup|ExSetupUI|WmiPrvSE|MOM|MonitoringHost|w3wp|msftesql|msftefd|EdgeTransport|mad|store|umservice|UMWorkerProcess|TranscodingService|SESWorker|ExBPA|ExFBA|wsbexchange|hostcontrollerservice|noderunner|parserserver|Microsoft\\.Exchange\\..*|MSExchange.*|fms|scanningprocess|FSCConfigurationServer|updateservice|ScanEngineTest|EngineUpdateLogger|sftracing|ForefrontActiveDirectoryConnector|rundll32|MSMessageTracingClient|wsmprovhost|Microsoft.Office.BigData.DataLoader)$";

		public const string FileVersionPattern = "^(\\d+\\.\\d+\\.\\d+\\.\\d+).*$";

		public const string IISVersion = "IIS 6";

		public const string IPAddressPattern = "^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}).*$";

		public const string IPAddressMatch = "^\\[\\d+\\.\\d+\\.\\d+\\.\\d+\\]$";

		public const string LocalAdminSid = "S-1-5-32-544";

		public const string SchemaAdminSid = "S-1-5-21-1413246421-4226699797-92697236-518";

		public const string EnterpriseAdminSid = "S-1-5-21-1413246421-4226699797-92697236-519";

		public const int MinExchangeBuild = 10000;

		public const int MinSchemaVersionRangeUpper = 14622;

		public const string RegistryExchange2010Path = "SOFTWARE\\Microsoft\\ExchangeServer\\v14";

		public const string RegistryExchange2007Path = "SOFTWARE\\Microsoft\\Exchange\\v8.0";

		public const string HubTransportRoleName = "HubTransportRole";

		public const string ClientAccessRoleName = "ClientAccessRole";

		public const string EdgeRoleName = "Hygiene";

		public const string MailboxRoleName = "MailboxRole";

		public const string UnifiedMessagingRoleName = "UnifiedMessagingRole";

		public const string AdminToolsRoleName = "AdminTools";

		public const string RegistryExchangeSetupPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		public const string RegistryMicrosoftInetStp = "Software\\Microsoft\\InetStp";

		public const string RegistryMicrosoftInetStpComponents = "Software\\Microsoft\\InetStp\\Components";

		public const string RegistryMicrosoftWindowsNTCurrentVersion = "Software\\Microsoft\\Windows NT\\CurrentVersion";

		public const string RegistryUCMA = "SOFTWARE\\Microsoft\\UCMA\\{902F4F35-D5DC-4363-8671-D5EF0D26C21D}";

		public const string RegistryMicrosoftWindowsCurrentVersion = "Software\\Microsoft\\Windows\\CurrentVersion";

		public const string RegistrySystemCurrentControlSetServices = "System\\CurrentControlSet\\Services";

		public const string RoleMatch = "(.*Role)";

		public const string RoleFilterPattern = "^.*\\\\(.*)Role$";

		public const string SmtpAddressPattern = "^((?i:smtp)\\:.*\\@(?'smtpaddress'.*))?.*$";

		public const string SmtpAddressReplacement = "${smtpaddress}";

		public const string SmtpDomainPattern = "^smtp:.*\\@(?'domain'.*))?.*$";

		public const string DomainReplacement = "${domain}";

		public const string SystemDrive = "%systemdrive%";

		public const int WindowsMajorVersion = 6;

		public const int WindowsMinorVersion = 1;

		public const int WindowsServicePack = 1;

		public const int Windows8MinorVerison = 2;

		public const string PrepareDomainGuid = "F63C3A12-7852-4654-B208-125C32EB409A";

		public const string PrepareLegacyExchangePermissionsGuid = "2A7F95FC-66C6-445F-AAB9-19744C05E70E";

		public const int AdjustMajorBuild = 10000;

		public const string ObjectTypeListString = "'0;a8df74a7-c5ea-11d1-bbcb-0080c76670c0'|'0;a8df74b2-c5ea-11d1-bbcb-0080c76670c0'|'0;bf967a8b-0de6-11d0-a285-00aa003049e2'|'0;28630ec1-41d5-11d1-a9c1-0000f80367c1'|'0;031b371a-a981-11d2-a9ff-00c04f8eedd8'|'0;3435244a-a982-11d2-a9ff-00c04f8eedd8'|'0;36145cf4-a982-11d2-a9ff-00c04f8eedd8'|'0;966540a1-75f7-4d27-ace9-3858b5dea688'|'0;9432cae6-b09e-11d2-aa06-00c04f8eedd8'|'0;93da93e4-b09e-11d2-aa06-00c04f8eedd8'|'0;a8df74d1-c5ea-11d1-bbcb-0080c76670c0'|'0;a8df74c5-c5ea-11d1-bbcb-0080c76670c0'|'0;a8df74ce-c5ea-11d1-bbcb-0080c76670c0'|'0;3378ca84-a982-11d2-a9ff-00c04f8eedd8'|'0;33bb8c5c-a982-11d2-a9ff-00c04f8eedd8'|'0;3397c916-a982-11d2-a9ff-00c04f8eedd8'|'0;8ef628c6-b093-11d2-aa06-00c04f8eedd8'|'0;8ef628c6-b093-11d2-aa06-00c04f8eedd8'|'0;93bb9552-b09e-11d2-aa06-00c04f8eedd8'|'0;44601346-776a-46e7-b4a4-2472e1c66806'|'0;20309cbd-0ae3-4876-9114-5738c65f845c'";

		public const string RegistryExchangePath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		public const string MinVersionAtl110VC2012 = "11.00.50727.1";

		public const string MinVersionMsvcr120VC2013 = "12.00.21005.1";

		public static readonly Version AllServersOfHigherVersionMinimum = new Version("15.1");

		public static readonly Version E12MinCoExistVersionNumber = new Version("8.3.83.0");

		public static readonly Version E14MinCoExistVersionNumber = new Version("14.3.71.0");

		public static readonly Version E14MinCoExistMajorVersionNumber = new Version("14.3.0.0");

		public enum DomainRole : ushort
		{
			StandaloneWorkstation,
			MemberWorkstation,
			StandaloneServer,
			MemberServer,
			BackupDomainController,
			PrimaryDomainController,
			None = 10
		}

		public enum ComputerNameFormat
		{
			ComputerNameNetBIOS,
			ComputerNameDnsHostname,
			ComputerNameDnsDomain,
			ComputerNameDnsFullyQualified,
			ComputerNamePhysicalNetBIOS,
			ComputerNamePhysicalDnsHostname,
			ComputerNamePhysicalDnsDomain,
			ComputerNamePhysicalDnsFullyQualified,
			ComputerNameMax
		}

		public enum ExtendedNameFormat
		{
			NameUnknown,
			NameFullyQualifiedDN,
			NameSamCompatible,
			NameDisplay,
			NameUniqueId = 6,
			NameCanonical,
			NameUserPrinciple,
			NameCanonicalEx,
			NameServicePrinciple,
			NameDnsDomain = 12
		}

		[Flags]
		public enum SecurityDescriptorControl : ushort
		{
			SE_OWNER_DEFAULTED = 1,
			SE_GROUP_DEFAULTED = 2,
			SE_DACL_PRESENT = 4,
			SE_DACL_DEFAULTED = 8,
			SE_SACL_PRESENT = 16,
			SE_SACL_DEFAULTED = 32,
			SE_DACL_UNTRUSTED = 64,
			SE_SERVER_SECURITY = 128,
			SE_DACL_AUTO_INHERIT_REQ = 256,
			SE_SACL_AUTO_INHERIT_REQ = 512,
			SE_DACL_AUTO_INHERITED = 1024,
			SE_SACL_AUTO_INHERITED = 2048,
			SE_DACL_PROTECTED = 4096,
			SE_SACL_PROTECTED = 8192,
			SE_RM_CONTROL_VALID = 16384,
			SE_SELF_RELATIVE = 32768
		}
	}
}
