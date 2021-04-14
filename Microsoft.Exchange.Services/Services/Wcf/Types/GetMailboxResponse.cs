﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMailboxResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public MailboxOptions MailboxOptions { get; set; }

		public override string ToString()
		{
			return string.Format("GetMailboxResponse: MailboxOptions = {0}", this.MailboxOptions);
		}
	}
}
