using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CacheValidationError : ValidationError
	{
		public CacheValidationError(LocalizedString description) : base(description)
		{
		}
	}
}
