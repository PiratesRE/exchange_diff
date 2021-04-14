﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddImBuddyRequestWrapper
	{
		[DataMember(Name = "instantMessageBuddy")]
		public InstantMessageBuddy InstantMessageBuddy { get; set; }

		[DataMember(Name = "instantMessageGroup")]
		public InstantMessageGroup InstantMessageGroup { get; set; }
	}
}