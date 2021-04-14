using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface IConversationTree : IEnumerable<IConversationTreeNode>, IEnumerable
	{
		ConversationTreeSortOrder SortOrder { get; }
	}
}
