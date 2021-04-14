using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NullEntitySelector : EntitySelector
	{
		public override bool IsEmpty
		{
			get
			{
				return true;
			}
		}

		public override IEnumerable<LoadEntity> GetEntities(LoadContainer targetContainer)
		{
			yield break;
		}
	}
}
