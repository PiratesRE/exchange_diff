using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface IGenericSyncPropertyFactory
	{
		object Create(object value, bool multiValued);

		object GetDefault(bool multiValued);
	}
}
