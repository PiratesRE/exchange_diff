using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadPartition
	{
		public LoadPartition(LoadContainer root, string constraintSetIdentity)
		{
			this.Root = root;
			this.ConstraintSetIdentity = constraintSetIdentity;
		}

		public LoadContainer Root { get; private set; }

		public string ConstraintSetIdentity { get; private set; }
	}
}
