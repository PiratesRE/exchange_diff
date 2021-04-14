using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupPrincipal : ExchangePrincipal
	{
		public GroupPrincipal(IGenericADUser adGroup, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions) : base(adGroup, allMailboxes, mailboxSelector, remotingOptions)
		{
			ArgumentValidator.ThrowIfTypeInvalid<ADGroupGenericWrapper>("adGroup", adGroup);
		}

		private GroupPrincipal(GroupPrincipal sourceGroupPrincipal) : base(sourceGroupPrincipal)
		{
		}

		public override string PrincipalId
		{
			get
			{
				return base.Alias;
			}
		}

		protected override ExchangePrincipal Clone()
		{
			return new GroupPrincipal(this);
		}
	}
}
