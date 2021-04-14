using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "AutomaticReplies")]
	public interface IAutomaticReplies : IEditObjectService<AutoReplyConfiguration, SetAutoReplyConfiguration>, IGetObjectService<AutoReplyConfiguration>
	{
	}
}
