﻿using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LikeItemRequestWrapper
	{
		[DataMember(Name = "request")]
		public LikeItemRequest Request { get; set; }
	}
}
