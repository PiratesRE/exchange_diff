using System;

namespace Microsoft.Exchange.Data
{
	public struct Result<T>
	{
		public Result(T data, ProviderError error)
		{
			this.data = data;
			this.error = error;
		}

		public ProviderError Error
		{
			get
			{
				return this.error;
			}
		}

		public T Data
		{
			get
			{
				return this.data;
			}
		}

		private T data;

		private ProviderError error;
	}
}
