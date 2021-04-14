using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public struct Optional<T>
	{
		public Optional(T value)
		{
			this.value = value;
			this.hasValue = true;
		}

		public T Value
		{
			get
			{
				if (this.hasValue)
				{
					return this.value;
				}
				throw new InvalidOperationException();
			}
		}

		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		public static implicit operator Optional<T>(T value)
		{
			return new Optional<T>(value);
		}

		public T DefaultTo(T defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		private readonly T value;

		private readonly bool hasValue;
	}
}
