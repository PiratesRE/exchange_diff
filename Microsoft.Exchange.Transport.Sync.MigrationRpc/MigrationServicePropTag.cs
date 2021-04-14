﻿using System;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	internal enum MigrationServicePropTag : uint
	{
		InArgMethodCode = 2684420099U,
		InArgUserLegacyDN = 2684485663U,
		InArgSubscriptionMessageId = 2684551426U,
		InArgAggregationSubscriptionType = 2684616707U,
		InArgCreateSyncSubscriptionName = 2685403167U,
		InArgCreateSyncSubscriptionImapServer = 2685468703U,
		InArgCreateSyncSubscriptionImapPort = 2685534211U,
		InArgCreateSyncSubscriptionEmailaddress = 2685599775U,
		InArgCreateSyncSubscriptionLogonName = 2685665311U,
		InArgCreateSyncSubscriptionSecurePassword = 2685730847U,
		InArgCreateSyncSubscriptionSecurityMechanism = 2685796355U,
		InArgCreateSyncSubscriptionAuthenticationMechanism = 2685861891U,
		InArgCreateSyncSubscriptionDisplayName = 2685927455U,
		InArgCreateSyncSubscriptionEncryptedPassword = 2685992991U,
		InArgCreateSyncSubscriptionFoldersToExclude = 2686058527U,
		InArgCreateSyncSubscriptionUserRootFolder = 2686124063U,
		InArgCreateSyncSubscriptionNetId = 2686189599U,
		InArgCreateSyncSubscriptionForceNew = 2686255115U,
		InArgUpdateSyncSubscriptionAction = 2162691U,
		InArgSubscriptionGuid = 2228296U,
		InArgMdbGuid = 2688614472U,
		InArgMigrationMailboxId = 2688679967U,
		InArgTenantAdmin = 2688745503U,
		InArgOrganizationId = 2688811266U,
		InArgRefresh = 2688876555U,
		InArgOrganizationName = 2688876802U,
		InArgMigrationMailboxUserLegacyDN = 2684420127U,
		InArgIsInitialSyncComplete = 2684485643U,
		InArgUpdateMigrationRequestAction = 2684551171U,
		InArgSubscriptionStatus = 2684616707U,
		InArgSubscriptionDetailedStatus = 2684682243U,
		InArgLastSuccessfulSyncTime = 2684747796U,
		InArgLastSyncTime = 2684813332U,
		InArgMigrationMailboxOrgId = 2684813384U,
		InArgMigrationDetailedStatus = 2684878851U,
		InArgUserExchangeMailboxSmtpAddress = 2684944415U,
		InArgItemsSynced = 2685009940U,
		InArgItemsSkipped = 2685075476U,
		InArgLastSyncNowRequestTime = 2685141012U,
		InArgUserExchangeMailboxLegDN = 2685206559U,
		OutArgErrorCode = 2936012803U,
		OutArgSubscriptionMessageId = 2936078594U,
		OutArgSubscriptionStatus = 2936143875U,
		OutArgDetailedSubscriptionStatus = 2936209411U,
		OutArgMigrationRequestUpdateResponse = 2936274947U,
		OutArgErrorDetails = 2936340511U,
		OutArgIsInitialSyncComplete = 2936406027U,
		OutArgLastSuccessfulSyncTime = 2936471572U,
		OutArgLastSyncTime = 2936537108U,
		OutArgItemsSynced = 2936602644U,
		OutArgItemsSkipped = 2936668180U,
		OutArgLastSyncNowRequestTime = 2936733716U,
		OutArgSubscriptionGuid = 2936799304U
	}
}
