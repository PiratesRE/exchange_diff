﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class TerminateChatSessionRequestWrapper
	{
		[DataMember(Name = "chatSessionId")]
		public int ChatSessionId { get; set; }
	}
}
