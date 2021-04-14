using System;
using System.Runtime.Versioning;

namespace System
{
	[NonVersionable]
	[__DynamicallyInvokable]
	[Serializable]
	public struct Nullable<T> where T : struct
	{
		[NonVersionable]
		[__DynamicallyInvokable]
		public Nullable(T value)
		{
			this.value = value;
			this.hasValue = true;
		}

		[__DynamicallyInvokable]
		public bool HasValue
		{
			[NonVersionable]
			[__DynamicallyInvokable]
			get
			{
				return this.hasValue;
			}
		}

		[__DynamicallyInvokable]
		public T Value
		{
			[__DynamicallyInvokable]
			get
			{
				if (!this.hasValue)
				{
					ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_NoValue);
				}
				return this.value;
			}
		}

		[NonVersionable]
		[__DynamicallyInvokable]
		public T GetValueOrDefault()
		{
			return this.value;
		}

		[NonVersionable]
		[__DynamicallyInvokable]
		public T GetValueOrDefault(T defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		[__DynamicallyInvokable]
		public override bool Equals(object other)
		{
			if (!this.hasValue)
			{
				return other == null;
			}
			return other != null && this.value.Equals(other);
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			if (!this.hasValue)
			{
				return 0;
			}
			return this.value.GetHashCode();
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString();
		}

		[NonVersionable]
		[__DynamicallyInvokable]
		public static implicit operator T?(T value)
		{
			return new T?(value);
		}

		[NonVersionable]
		[__DynamicallyInvokable]
		public static explicit operator T(T? value)
		{
			return value.Value;
		}

		private bool hasValue;

		internal T value;
	}
}
