using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "EMailSignature")]
	public interface IEMailSignature : IMessagingBase<EMailSignatureConfiguration, SetEMailSignatureConfiguration>, IEditObjectService<EMailSignatureConfiguration, SetEMailSignatureConfiguration>, IGetObjectService<EMailSignatureConfiguration>
	{
	}
}
