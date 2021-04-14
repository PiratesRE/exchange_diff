using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class CannotRegisterNewReplidGuidMapping : StoreException
	{
		public CannotRegisterNewReplidGuidMapping(LID lid, string message) : base(lid, ErrorCodeValue.CannotRegisterNewReplidGuidMapping, message)
		{
		}

		public CannotRegisterNewReplidGuidMapping(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.CannotRegisterNewReplidGuidMapping, message, innerException)
		{
		}
	}
}
