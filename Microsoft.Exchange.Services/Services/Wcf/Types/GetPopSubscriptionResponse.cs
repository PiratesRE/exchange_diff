﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetPopSubscriptionResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public PopSubscription PopSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("GetPopSubscriptionResponse: {0}", this.PopSubscription);
		}
	}
}
