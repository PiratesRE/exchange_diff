using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	public delegate void AggregateType<TResult>(Type baseType, Type type, List<TResult> results) where TResult : class;
}
