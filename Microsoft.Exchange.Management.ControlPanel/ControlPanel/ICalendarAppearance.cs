using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "CalendarAppearance")]
	public interface ICalendarAppearance : ICalendarBase<CalendarAppearanceConfiguration, SetCalendarAppearanceConfiguration>, IEditObjectService<CalendarAppearanceConfiguration, SetCalendarAppearanceConfiguration>, IGetObjectService<CalendarAppearanceConfiguration>
	{
		[OperationContract]
		PowerShellResults<CalendarAppearanceConfiguration> UpdateObject(Identity identity);
	}
}
