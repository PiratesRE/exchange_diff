using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "Conversations")]
	public interface IConversations : IMessagingBase<ConversationsConfiguration, SetConversationsConfiguration>, IEditObjectService<ConversationsConfiguration, SetConversationsConfiguration>, IGetObjectService<ConversationsConfiguration>
	{
	}
}
