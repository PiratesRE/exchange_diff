using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	public enum WellKnownSidType
	{
		NullSid,
		WorldSid,
		LocalSid,
		CreatorOwnerSid,
		CreatorGroupSid,
		CreatorOwnerServerSid,
		CreatorGroupServerSid,
		NTAuthoritySid,
		DialupSid,
		NetworkSid,
		BatchSid,
		InteractiveSid,
		ServiceSid,
		AnonymousSid,
		ProxySid,
		EnterpriseControllersSid,
		SelfSid,
		AuthenticatedUserSid,
		RestrictedCodeSid,
		TerminalServerSid,
		RemoteLogonIdSid,
		LogonIdsSid,
		LocalSystemSid,
		LocalServiceSid,
		NetworkServiceSid,
		BuiltinDomainSid,
		BuiltinAdministratorsSid,
		BuiltinUsersSid,
		BuiltinGuestsSid,
		BuiltinPowerUsersSid,
		BuiltinAccountOperatorsSid,
		BuiltinSystemOperatorsSid,
		BuiltinPrintOperatorsSid,
		BuiltinBackupOperatorsSid,
		BuiltinReplicatorSid,
		BuiltinPreWindows2000CompatibleAccessSid,
		BuiltinRemoteDesktopUsersSid,
		BuiltinNetworkConfigurationOperatorsSid,
		AccountAdministratorSid,
		AccountGuestSid,
		AccountKrbtgtSid,
		AccountDomainAdminsSid,
		AccountDomainUsersSid,
		AccountDomainGuestsSid,
		AccountComputersSid,
		AccountControllersSid,
		AccountCertAdminsSid,
		AccountSchemaAdminsSid,
		AccountEnterpriseAdminsSid,
		AccountPolicyAdminsSid,
		AccountRasAndIasServersSid,
		NtlmAuthenticationSid,
		DigestAuthenticationSid,
		SChannelAuthenticationSid,
		ThisOrganizationSid,
		OtherOrganizationSid,
		BuiltinIncomingForestTrustBuildersSid,
		BuiltinPerformanceMonitoringUsersSid,
		BuiltinPerformanceLoggingUsersSid,
		BuiltinAuthorizationAccessSid,
		WinBuiltinTerminalServerLicenseServersSid,
		MaxDefined = 60
	}
}
