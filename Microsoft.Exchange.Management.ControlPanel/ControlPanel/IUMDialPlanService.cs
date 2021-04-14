using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMDialPlan")]
	public interface IUMDialPlanService : IUploadHandler
	{
	}
}
