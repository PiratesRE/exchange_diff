using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EntitySelector
	{
		public abstract bool IsEmpty { get; }

		public abstract IEnumerable<LoadEntity> GetEntities(LoadContainer targetContainer);
	}
}
