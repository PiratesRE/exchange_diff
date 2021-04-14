using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "JournalReportNdrTo")]
	public interface IJournalReportNdrTo : IEditObjectService<JournalReportNdrTo, SetJournalReportNdrTo>, IGetObjectService<JournalReportNdrTo>
	{
	}
}
