using System;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	internal sealed class MsoMainStreamCookieContainer : SyncServiceInstance
	{
		internal MultiValuedProperty<byte[]> MsoForwardSyncRecipientCookie
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MsoMainStreamCookieContainerSchema.MsoForwardSyncRecipientCookie];
			}
		}

		internal MultiValuedProperty<byte[]> MsoForwardSyncNonRecipientCookie
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MsoMainStreamCookieContainerSchema.MsoForwardSyncNonRecipientCookie];
			}
		}

		internal MultiValuedProperty<FullSyncObjectRequest> MsoForwardSyncObjectFullSyncRequests
		{
			get
			{
				return (MultiValuedProperty<FullSyncObjectRequest>)this[MsoMainStreamCookieContainerSchema.MsoForwardSyncObjectFullSyncRequests];
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMSOSyncServiceInstance";
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MsoMainStreamCookieContainer.schema;
			}
		}

		private const string MostDerivedObjectClassInternal = "msExchMSOSyncServiceInstance";

		private static MsoMainStreamCookieContainerSchema schema = ObjectSchema.GetInstance<MsoMainStreamCookieContainerSchema>();
	}
}
