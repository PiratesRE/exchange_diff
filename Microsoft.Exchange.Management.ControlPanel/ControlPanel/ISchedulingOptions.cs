using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SchedulingOptions")]
	public interface ISchedulingOptions : IResourceBase<SchedulingOptionsConfiguration, SetSchedulingOptionsConfiguration>, IEditObjectService<SchedulingOptionsConfiguration, SetSchedulingOptionsConfiguration>, IGetObjectService<SchedulingOptionsConfiguration>
	{
	}
}
