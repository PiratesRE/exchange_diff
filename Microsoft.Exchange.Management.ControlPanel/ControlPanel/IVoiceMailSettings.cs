using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "VoiceMailSettings")]
	public interface IVoiceMailSettings : IEditObjectService<GetVoiceMailSettings, SetVoiceMailSettings>, IGetObjectService<GetVoiceMailSettings>
	{
		[OperationContract]
		PowerShellResults ResetPIN(Identity identity);
	}
}
