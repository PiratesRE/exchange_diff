using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddSharedCalendarRequest
	{
		[DataMember]
		public ItemId MessageId { get; set; }

		internal StoreObjectId MessageStoreId { get; private set; }

		internal void ValidateRequest()
		{
			if (this.MessageId == null || string.IsNullOrEmpty(this.MessageId.Id))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			this.MessageStoreId = ServiceIdConverter.ConvertFromConcatenatedId(this.MessageId.Id, BasicTypes.Item, null).ToStoreObjectId();
		}
	}
}
