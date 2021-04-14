using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface ITraceWrapper<T>
	{
		T Element { set; }
	}
}
