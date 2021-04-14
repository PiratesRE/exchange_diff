using System;

namespace Microsoft.Exchange.Diagnostics
{
	public delegate TResult MatchType<TResult, TParam>(Type baseType, Type type, TParam param) where TResult : class;
}
