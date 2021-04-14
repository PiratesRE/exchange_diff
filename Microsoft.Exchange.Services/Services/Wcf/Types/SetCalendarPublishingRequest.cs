using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetCalendarPublishingRequest : CalendarSharingRequestBase
	{
		[DataMember]
		public bool Publish { get; set; }

		[DataMember]
		public string DetailLevel { get; set; }

		public DetailLevelEnumType DetailLevelEnum
		{
			get
			{
				return this.detailLevelEnum;
			}
		}

		internal override void ValidateRequest()
		{
			base.ValidateRequest();
			if (this.Publish && !Enum.TryParse<DetailLevelEnumType>(this.DetailLevel, out this.detailLevelEnum))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
		}

		private DetailLevelEnumType detailLevelEnum;
	}
}
