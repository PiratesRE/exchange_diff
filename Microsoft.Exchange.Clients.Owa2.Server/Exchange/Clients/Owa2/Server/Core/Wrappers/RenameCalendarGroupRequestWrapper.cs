using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RenameCalendarGroupRequestWrapper
	{
		[DataMember(Name = "groupId")]
		public ItemId GroupId { get; set; }

		[DataMember(Name = "newGroupName")]
		public string NewGroupName { get; set; }
	}
}
