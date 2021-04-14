using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDiscoverySearchRequestExtendedStoreSchema : ObjectSchema
	{
		public static readonly ExtendedPropertyDefinition AsynchronousActionRequest = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "AsynchronousActionRequest", 14);

		public static readonly ExtendedPropertyDefinition AsynchronousActionRbacContext = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "AsynchronousActionRbacContext", 25);
	}
}
