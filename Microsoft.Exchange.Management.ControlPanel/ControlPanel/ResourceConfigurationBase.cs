using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ResourceConfigurationBase : BaseRow
	{
		public ResourceConfigurationBase(CalendarConfiguration calendarConfiguration) : base(calendarConfiguration)
		{
			this.CalendarConfiguration = calendarConfiguration;
		}

		public CalendarConfiguration CalendarConfiguration { get; private set; }
	}
}
