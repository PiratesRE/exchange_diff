using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class EnumListSource<T> : EnumListSource
	{
		public EnumListSource() : base(typeof(T))
		{
		}

		public EnumListSource(Array values) : base(values, typeof(T))
		{
		}
	}
}
