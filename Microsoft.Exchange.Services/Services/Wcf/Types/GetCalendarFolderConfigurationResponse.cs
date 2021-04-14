using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCalendarFolderConfigurationResponse : CalendarActionResponse
	{
		internal GetCalendarFolderConfigurationResponse(CalendarFolderType folder, MasterCategoryListType masterList = null)
		{
			this.CalendarFolder = folder;
			this.MasterList = masterList;
		}

		internal GetCalendarFolderConfigurationResponse(CalendarActionError errorCode) : base(errorCode)
		{
		}

		[DataMember]
		public CalendarFolderType CalendarFolder { get; set; }

		[DataMember]
		public MasterCategoryListType MasterList { get; set; }
	}
}
