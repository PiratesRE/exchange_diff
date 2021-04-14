using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LazilyInitialized<T> : IEquatable<T>
	{
		public LazilyInitialized(Func<T> initializer)
		{
			if (initializer == null)
			{
				throw new ArgumentNullException("initializer");
			}
			this.initializer = initializer;
			this.value = default(T);
		}

		internal LazilyInitialized() : this(new Func<T>(LazilyInitialized<T>.InitializerNotSupported))
		{
		}

		public T Value
		{
			get
			{
				Func<T> func = this.initializer;
				if (func != null)
				{
					this.value = func();
					this.initializer = null;
				}
				return this.value;
			}
		}

		internal bool IsInitialized
		{
			get
			{
				return this.initializer == null;
			}
		}

		public static implicit operator T(LazilyInitialized<T> delayInitialized)
		{
			return delayInitialized.Value;
		}

		public static bool operator ==(LazilyInitialized<T> op1, T op2)
		{
			return op1 != null && op1.Equals(op2);
		}

		public static bool operator !=(LazilyInitialized<T> op1, T op2)
		{
			return !(op1 == op2);
		}

		public override bool Equals(object obj)
		{
			LazilyInitialized<T> lazilyInitialized = obj as LazilyInitialized<T>;
			if (lazilyInitialized == null || !this.Equals(lazilyInitialized.Value))
			{
				T t = this.Value;
				return t.Equals(obj);
			}
			return true;
		}

		public bool Equals(T v)
		{
			return EqualityComparer<T>.Default.Equals(this.Value, v);
		}

		public override int GetHashCode()
		{
			T t = this.Value;
			return t.GetHashCode();
		}

		public override string ToString()
		{
			if (this.initializer == null)
			{
				return this.value.ToString();
			}
			return "Uninitialized";
		}

		internal void Set(T value)
		{
			if (this.IsInitialized)
			{
				throw new InvalidOperationException();
			}
			this.initializer = null;
			this.value = value;
		}

		private static T InitializerNotSupported()
		{
			throw new InvalidOperationException();
		}

		private Func<T> initializer;

		private T value;
	}
}
