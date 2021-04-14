using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MessageFormat")]
	public interface IMessageFormat : IMessagingBase<MessageFormatConfiguration, SetMessageFormatConfiguration>, IEditObjectService<MessageFormatConfiguration, SetMessageFormatConfiguration>, IGetObjectService<MessageFormatConfiguration>
	{
	}
}
