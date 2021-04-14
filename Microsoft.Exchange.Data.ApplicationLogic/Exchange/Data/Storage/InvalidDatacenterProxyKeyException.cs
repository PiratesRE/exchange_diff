using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidDatacenterProxyKeyException : LocalizedException
	{
		public InvalidDatacenterProxyKeyException() : base(Strings.DatacenterSecretIsMissingOrInvalid)
		{
		}
	}
}
