using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ForwardEmails")]
	public interface IForwardEmails : IEditObjectService<ForwardEmailMailbox, SetForwardEmailMailbox>, IGetObjectService<ForwardEmailMailbox>
	{
		[OperationContract]
		PowerShellResults<ForwardEmailMailbox> StopForward(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
