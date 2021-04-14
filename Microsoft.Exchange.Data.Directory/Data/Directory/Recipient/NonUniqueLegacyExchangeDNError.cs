using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class NonUniqueLegacyExchangeDNError : NonUniqueAddressError
	{
		public NonUniqueLegacyExchangeDNError(LocalizedString description, ObjectId objectId, string dataSourceName) : base(description, objectId, description)
		{
		}
	}
}
