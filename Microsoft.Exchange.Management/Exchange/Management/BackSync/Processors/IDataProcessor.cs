using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal interface IDataProcessor
	{
		void Process(PropertyBag propertyBag);

		void Flush(Func<byte[]> getCookieDelegate, bool moreData);
	}
}
