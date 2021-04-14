using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "Supervision")]
	public interface ISupervision : IEditObjectService<SupervisionStatus, SetSupervisionStatus>, IGetObjectService<SupervisionStatus>
	{
	}
}
