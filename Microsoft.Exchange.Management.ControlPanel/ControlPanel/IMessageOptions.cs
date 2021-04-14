using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MessageOptions")]
	public interface IMessageOptions : IMessagingBase<MessageOptionsConfiguration, SetMessageOptionsConfiguration>, IEditObjectService<MessageOptionsConfiguration, SetMessageOptionsConfiguration>, IGetObjectService<MessageOptionsConfiguration>
	{
	}
}
