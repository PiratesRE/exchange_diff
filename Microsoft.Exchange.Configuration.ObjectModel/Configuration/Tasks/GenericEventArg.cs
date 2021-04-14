using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class GenericEventArg<T> : EventArgs
	{
		public T Data { get; set; }

		public GenericEventArg(T data)
		{
			this.Data = data;
		}
	}
}
