using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateCalendarRequestWrapper
	{
		[DataMember(Name = "newCalendarName")]
		public string NewCalendarName { get; set; }

		[DataMember(Name = "parentGroupGuid")]
		public string ParentGroupGuid { get; set; }

		[DataMember(Name = "emailAddress")]
		public string EmailAddress { get; set; }
	}
}
