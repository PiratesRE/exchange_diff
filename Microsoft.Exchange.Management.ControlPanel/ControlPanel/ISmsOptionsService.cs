using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SmsOptions")]
	public interface ISmsOptionsService : IEditObjectService<SmsOptions, SetSmsOptions>, IGetObjectService<SmsOptions>
	{
		[OperationContract]
		PowerShellResults<SmsOptions> DisableObject(Identity identity);

		[OperationContract]
		PowerShellResults<SmsOptions> SendVerificationCode(Identity identity, SetSmsOptions properties);
	}
}
