using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SubscribeInternetCalendarRequest
	{
		[DataMember]
		public string ICalUrl { get; set; }

		[DataMember]
		public string CalendarName { get; set; }

		[DataMember]
		private string ParentGroupGuid { get; set; }

		internal Guid GroupId { get; private set; }

		internal void ValidateRequest()
		{
			if (this.ICalUrl == null)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			Guid groupId;
			if (!Guid.TryParse(this.ParentGroupGuid, out groupId))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(CoreResources.IDs.ErrorInvalidId), FaultParty.Sender);
			}
			this.GroupId = groupId;
		}
	}
}
