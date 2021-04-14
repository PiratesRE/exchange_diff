using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ResponseMessage")]
	public interface IResponseMessage : IResourceBase<ResponseMessageConfiguration, SetResponseMessageConfiguration>, IEditObjectService<ResponseMessageConfiguration, SetResponseMessageConfiguration>, IGetObjectService<ResponseMessageConfiguration>
	{
	}
}
