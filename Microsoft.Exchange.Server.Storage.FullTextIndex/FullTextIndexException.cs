using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public class FullTextIndexException : StoreException
	{
		public FullTextIndexException(LID lid, ErrorCodeValue errorCode, string message) : base(lid, errorCode, message)
		{
		}
	}
}
