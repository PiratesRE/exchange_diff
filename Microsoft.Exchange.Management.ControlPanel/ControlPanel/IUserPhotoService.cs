using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UserPhoto")]
	public interface IUserPhotoService
	{
		[OperationContract]
		PowerShellResults SavePhoto(Identity identity);

		[OperationContract]
		PowerShellResults CancelPhoto(Identity identity);

		[OperationContract]
		PowerShellResults RemovePhoto(Identity identity);
	}
}
