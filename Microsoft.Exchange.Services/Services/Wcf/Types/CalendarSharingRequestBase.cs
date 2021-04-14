using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class CalendarSharingRequestBase
	{
		[DataMember]
		public FolderId CalendarId { get; set; }

		internal StoreObjectId CalendarStoreId { get; private set; }

		internal virtual void ValidateRequest()
		{
			if (this.CalendarId == null)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			IdHeaderInformation idHeaderInformation = ServiceIdConverter.ConvertFromConcatenatedId(this.CalendarId.Id, BasicTypes.Folder, null);
			this.CalendarStoreId = StoreObjectId.FromProviderSpecificId(idHeaderInformation.StoreIdBytes, StoreObjectType.CalendarFolder);
		}
	}
}
