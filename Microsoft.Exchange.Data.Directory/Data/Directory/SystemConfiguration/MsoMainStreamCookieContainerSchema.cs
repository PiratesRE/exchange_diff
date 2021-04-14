using System;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class MsoMainStreamCookieContainerSchema : SyncServiceInstanceSchema
	{
		public static readonly ADPropertyDefinition MsoForwardSyncRecipientCookie = new ADPropertyDefinition("MsoForwardSyncRecipientCookie", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchMsoForwardSyncRecipientCookie", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 49152)
		}, null, null);

		public static readonly ADPropertyDefinition MsoForwardSyncNonRecipientCookie = new ADPropertyDefinition("MsoForwardSyncNonRecipientCookie", ExchangeObjectVersion.Exchange2003, typeof(byte[]), "msExchMsoForwardSyncNonRecipientCookie", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 49152)
		}, null, null);

		public static readonly ADPropertyDefinition MsoForwardSyncObjectFullSyncRequests = new ADPropertyDefinition("MsoForwardSyncObjectFullSyncRequests", ExchangeObjectVersion.Exchange2003, typeof(FullSyncObjectRequest), "msExchMsoForwardSyncReplayList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
