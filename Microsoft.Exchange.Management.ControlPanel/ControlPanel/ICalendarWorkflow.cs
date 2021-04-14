using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "CalendarWorkflow")]
	public interface ICalendarWorkflow : IEditObjectService<CalendarWorkflowConfiguration, SetCalendarWorkflowConfiguration>, IGetObjectService<CalendarWorkflowConfiguration>
	{
		[OperationContract]
		PowerShellResults<CalendarWorkflowConfiguration> UpdateObject(Identity identity);
	}
}
