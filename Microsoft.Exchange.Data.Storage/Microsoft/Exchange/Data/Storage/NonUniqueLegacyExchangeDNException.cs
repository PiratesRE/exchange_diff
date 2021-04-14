using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class NonUniqueLegacyExchangeDNException : StoragePermanentException
	{
		public NonUniqueLegacyExchangeDNException(LocalizedString message) : base(message)
		{
		}
	}
}
