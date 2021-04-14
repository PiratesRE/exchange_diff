using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class CannotRegisterNewNamedPropertyMapping : StoreException
	{
		public CannotRegisterNewNamedPropertyMapping(LID lid, string message) : base(lid, ErrorCodeValue.CannotRegisterNewNamedPropertyMapping, message)
		{
		}

		public CannotRegisterNewNamedPropertyMapping(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.CannotRegisterNewNamedPropertyMapping, message, innerException)
		{
		}
	}
}
