using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ResponseMessageConfiguration : ResourceConfigurationBase
	{
		public ResponseMessageConfiguration(CalendarConfiguration calendarConfiguration) : base(calendarConfiguration)
		{
		}

		[DataMember]
		public bool AddAdditionalResponse
		{
			get
			{
				return base.CalendarConfiguration.AddAdditionalResponse;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AdditionalResponse
		{
			get
			{
				return base.CalendarConfiguration.AdditionalResponse ?? string.Empty;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
