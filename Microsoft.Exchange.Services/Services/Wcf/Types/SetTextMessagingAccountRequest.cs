﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetTextMessagingAccountRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetTextMessagingAccountData Options { get; set; }

		public override string ToString()
		{
			return string.Format("SetTextMessagingAccountRequest: {0}", this.Options);
		}
	}
}
