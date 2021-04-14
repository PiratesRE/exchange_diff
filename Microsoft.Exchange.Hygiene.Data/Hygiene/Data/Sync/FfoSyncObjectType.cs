using System;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal enum FfoSyncObjectType
	{
		Unknown = -1,
		Account,
		Company,
		Contact,
		Device,
		ForeignPrincipal,
		Group,
		KeyGroup,
		ServicePrincipal,
		SubscribedPlan,
		Subscription,
		User,
		PublicFolder
	}
}
