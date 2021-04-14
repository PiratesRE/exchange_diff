﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class JsonResponseHeaders
	{
		public JsonResponseHeaders()
		{
			this.ServerVersionInfo = ServerVersionInfo.CurrentAssemblyVersion;
		}

		[DataMember]
		public ServerVersionInfo ServerVersionInfo;
	}
}
