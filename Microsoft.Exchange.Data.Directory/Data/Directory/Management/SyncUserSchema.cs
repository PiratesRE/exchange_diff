using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncUserSchema : UserSchema
	{
		public static readonly ADPropertyDefinition OnPremisesObjectId = ADRecipientSchema.OnPremisesObjectId;

		public static readonly ADPropertyDefinition IsDirSynced = ADRecipientSchema.IsDirSynced;

		public static readonly ADPropertyDefinition DirSyncAuthorityMetadata = ADRecipientSchema.DirSyncAuthorityMetadata;

		public static readonly ADPropertyDefinition UsageLocation = ADRecipientSchema.UsageLocation;

		public static readonly ADPropertyDefinition RemoteRecipientType = ADUserSchema.RemoteRecipientType;

		public static readonly ADPropertyDefinition UsnCreated = ADRecipientSchema.UsnCreated;

		public static readonly ADPropertyDefinition ReleaseTrack = ADRecipientSchema.ReleaseTrack;

		public static readonly ADPropertyDefinition PreviousExchangeGuid = IADMailStorageSchema.PreviousExchangeGuid;

		public static readonly ADPropertyDefinition PreviousDatabase = IADMailStorageSchema.PreviousDatabase;

		public static readonly ADPropertyDefinition AccountDisabled = ADUserSchema.AccountDisabled;

		public static readonly ADPropertyDefinition StsRefreshTokensValidFrom = ADUserSchema.StsRefreshTokensValidFrom;
	}
}
