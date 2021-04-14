using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMAutoAttendant")]
	public interface IUMAutoAttendantService : IUploadHandler
	{
	}
}
