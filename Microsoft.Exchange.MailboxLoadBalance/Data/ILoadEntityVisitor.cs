using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ILoadEntityVisitor
	{
		bool Visit(LoadContainer container);

		bool Visit(LoadEntity entity);
	}
}
