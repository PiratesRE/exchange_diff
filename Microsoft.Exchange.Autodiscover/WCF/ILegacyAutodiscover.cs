using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[ServiceContract]
	public interface ILegacyAutodiscover
	{
		[WebGet]
		[OperationContract(Action = "GET", ReplyAction = "*")]
		Message LegacyGetAction(Message input);

		[OperationContract(Action = "*", ReplyAction = "*")]
		[WebInvoke]
		Message LegacyAction(Message input);
	}
}
