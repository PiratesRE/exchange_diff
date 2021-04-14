using System;

namespace Microsoft.Exchange.Management.Metabase
{
	internal static class MetabasePropertyTypes
	{
		public enum AppIsolated
		{
			InProc,
			OutProc,
			Pooled
		}

		public enum AppPoolIdentityType
		{
			LocalSystem,
			LocalService,
			NetworkService,
			SpecificUser
		}

		[Flags]
		public enum AuthFlags : uint
		{
			NoneSet = 0U,
			Anonymous = 1U,
			Basic = 2U,
			Ntlm = 4U,
			MD5 = 16U,
			Passport = 64U
		}

		[Flags]
		public enum DirBrowseFlags : uint
		{
			ShowDate = 2U,
			ShowTime = 4U,
			ShowSize = 8U,
			ShowExtension = 16U,
			ShowLongDate = 32U,
			DisableDirBrowsing = 64U,
			EnableDefaultDoc = 1073741824U,
			EnableDirBrowsing = 2147483648U
		}

		[Flags]
		public enum AccessFlags : uint
		{
			NoAccess = 0U,
			Read = 1U,
			Write = 2U,
			Execute = 4U,
			Source = 16U,
			Script = 512U,
			NoRemoteWrite = 1024U,
			NoRemoteRead = 4096U,
			NoRemoteExecute = 8192U,
			NoRemoteScript = 16384U,
			NoPhysicalDir = 32768U
		}

		[Flags]
		public enum AccessSSLFlags : uint
		{
			None = 0U,
			AccessSSL = 8U,
			AccessSSLNegotiateCert = 32U,
			AccessSSLRequireCert = 64U,
			AccessSSLMapCert = 128U,
			AccessSSL128 = 256U
		}

		public enum LogonMethod : uint
		{
			InteractiveLogon,
			BatchLogon,
			NetworkLogon,
			ClearTextLogon
		}

		[Flags]
		public enum FilterFlags : uint
		{
			NotifySecurePort = 1U,
			NotifyNonsecurePort = 2U,
			NotifyReadRawData = 32768U,
			NotifyPreprocHeaders = 16384U,
			NotifyAuthentication = 8192U,
			NotifyUrlMap = 4096U,
			NotifyAccessDenied = 2048U,
			NotifySendResponse = 64U,
			NotifySendRawData = 1024U,
			NotifyLog = 512U,
			NotifyEndOfRequest = 128U,
			NotifyEndOfNetSession = 256U,
			NotifyOrderHigh = 524288U,
			NotifyOrderMedium = 262144U,
			NotifyOrderLow = 131072U,
			NotifyOrderDefault = 131072U
		}

		public enum ManagedPipelineMode
		{
			Integrated,
			Classic
		}
	}
}
