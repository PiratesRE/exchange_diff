using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "VoiceMail")]
	public interface IVoiceMail : IEditObjectService<GetVoiceMailConfiguration, SetVoiceMailConfiguration>, IGetObjectService<GetVoiceMailConfiguration>
	{
		[OperationContract]
		PowerShellResults<GetVoiceMailConfiguration> ClearSettings(Identity identity);

		[OperationContract]
		PowerShellResults<GetVoiceMailConfiguration> RegisterPhone(Identity identity, SetVoiceMailConfiguration properties);
	}
}
