using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface IConversationDatum
	{
		IList<IMessageRecipient> Participants { get; set; }

		string[] TopicWords { get; set; }
	}
}
