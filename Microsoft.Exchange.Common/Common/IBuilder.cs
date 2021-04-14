using System;

namespace Microsoft.Exchange.Common
{
	public interface IBuilder<T>
	{
		T Build();
	}
}
