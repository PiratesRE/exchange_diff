using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal delegate void AggregatedConversationPropertySetter(ConversationType conversation, object propertyValue);
}
