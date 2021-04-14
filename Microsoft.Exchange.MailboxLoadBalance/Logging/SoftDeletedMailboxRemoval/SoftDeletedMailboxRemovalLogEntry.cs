using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging.SoftDeletedMailboxRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedMailboxRemovalLogEntry : ConfigurableObject
	{
		public SoftDeletedMailboxRemovalLogEntry() : base(new SimpleProviderPropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<SoftDeletedMailboxRemovalLogEntrySchema>();
			}
		}
	}
}
