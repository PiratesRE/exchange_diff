using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IProtocolLog : IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		void Add(string key, object value);

		void Merge(IProtocolLog other);
	}
}
