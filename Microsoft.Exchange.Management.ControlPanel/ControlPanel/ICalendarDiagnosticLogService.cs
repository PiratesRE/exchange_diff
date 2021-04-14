using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "CalendarDiagnosticLog")]
	public interface ICalendarDiagnosticLogService
	{
		[OperationContract]
		PowerShellResults SendLog(CalendarDiagnosticLog properties);
	}
}
