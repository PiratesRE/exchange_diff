using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RawImGroup
	{
		public string DisplayName { get; set; }

		public string GroupType { get; set; }

		public StoreObjectId ExchangeStoreId { get; set; }

		public StoreObjectId[] MemberCorrelationKey { get; set; }

		public ExtendedPropertyType[] ExtendedProperties { get; set; }

		public string SmtpAddress { get; set; }
	}
}
