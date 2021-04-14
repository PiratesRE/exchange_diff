using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MessageClassifications")]
	public interface IMessageClassifications : IGetListService<MessageClassificationFilter, MessageClassification>
	{
	}
}
