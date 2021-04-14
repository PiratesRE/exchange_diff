using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class CorruptSearchBacklinkException : StoreException
	{
		public CorruptSearchBacklinkException(LID lid, string message) : base(lid, ErrorCodeValue.CorruptSearchBacklink, message)
		{
		}

		public CorruptSearchBacklinkException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.CorruptSearchBacklink, message, innerException)
		{
		}
	}
}
