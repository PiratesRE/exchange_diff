using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	internal interface IConversationNodeProcessorFactory
	{
		IEnumerable<IConversationNodeProcessor> CreateNormalNodeProcessors();

		IEnumerable<IConversationNodeProcessor> CreateRootNodeProcessors();
	}
}
