﻿using System;

namespace System.Security.Principal
{
	[Serializable]
	internal enum TokenInformationClass
	{
		TokenUser = 1,
		TokenGroups,
		TokenPrivileges,
		TokenOwner,
		TokenPrimaryGroup,
		TokenDefaultDacl,
		TokenSource,
		TokenType,
		TokenImpersonationLevel,
		TokenStatistics,
		TokenRestrictedSids,
		TokenSessionId,
		TokenGroupsAndPrivileges,
		TokenSessionReference,
		TokenSandBoxInert,
		TokenAuditPolicy,
		TokenOrigin,
		TokenElevationType,
		TokenLinkedToken,
		TokenElevation,
		TokenHasRestrictions,
		TokenAccessInformation,
		TokenVirtualizationAllowed,
		TokenVirtualizationEnabled,
		TokenIntegrityLevel,
		TokenUIAccess,
		TokenMandatoryPolicy,
		TokenLogonSid,
		TokenIsAppContainer,
		TokenCapabilities,
		TokenAppContainerSid,
		TokenAppContainerNumber,
		TokenUserClaimAttributes,
		TokenDeviceClaimAttributes,
		TokenRestrictedUserClaimAttributes,
		TokenRestrictedDeviceClaimAttributes,
		TokenDeviceGroups,
		TokenRestrictedDeviceGroups,
		MaxTokenInfoClass
	}
}