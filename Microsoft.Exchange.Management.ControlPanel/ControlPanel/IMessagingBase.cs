using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MessagingBase")]
	public interface IMessagingBase<O, U> : IEditObjectService<O, U>, IGetObjectService<O> where O : MessagingConfigurationBase where U : SetMessagingConfigurationBase
	{
	}
}
