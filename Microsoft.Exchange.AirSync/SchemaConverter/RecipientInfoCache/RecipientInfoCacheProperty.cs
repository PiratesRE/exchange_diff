using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.RecipientInfoCache
{
	internal abstract class RecipientInfoCacheProperty : PropertyBase
	{
		public abstract void Bind(RecipientInfoCacheEntry entry);
	}
}
