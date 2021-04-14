using System;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	public abstract class Hookable<T>
	{
		private Hookable(bool setTestHookNullRestoresDefaultValue, T defaultValue)
		{
			this.setTestHookNullRestoresDefaultValue = setTestHookNullRestoresDefaultValue;
			this.defaultValue = defaultValue;
		}

		public static Hookable<T> Create(bool setTestHookNullRestoresDefaultValue, T defaultValue)
		{
			return new Hookable<T>.HookableByReference(setTestHookNullRestoresDefaultValue, defaultValue);
		}

		public static Hookable<T> Create(bool setTestHookNullRestoresDefaultValue, Func<T> activeValueGetter, Action<T> activeValueSetter)
		{
			return new Hookable<T>.HookableUsingDelegates(setTestHookNullRestoresDefaultValue, activeValueGetter, activeValueSetter);
		}

		public static Hookable<T> Create(bool setTestHookNullRestoresDefaultValue, FieldInfo field, object instance)
		{
			return new Hookable<T>.HookableUsingDelegates(setTestHookNullRestoresDefaultValue, () => (T)((object)field.GetValue(instance)), delegate(T value)
			{
				field.SetValue(instance, value);
			});
		}

		public static Hookable<T> Create<TInstance>(bool setTestHookNullRestoresDefaultValue, string fieldName, TInstance instance)
		{
			FieldInfo field = ReflectionHelper.TraverseTypeHierarchy<FieldInfo, string>(typeof(TInstance), fieldName, new MatchType<FieldInfo, string>(ReflectionHelper.MatchInstanceField));
			return Hookable<T>.Create(setTestHookNullRestoresDefaultValue, field, instance);
		}

		public static Hookable<T> Create(bool setTestHookNullRestoresDefaultValue, string fieldName, Type type)
		{
			FieldInfo field = ReflectionHelper.TraverseTypeHierarchy<FieldInfo, string>(type, fieldName, new MatchType<FieldInfo, string>(ReflectionHelper.MatchStaticField));
			return Hookable<T>.Create(setTestHookNullRestoresDefaultValue, field, null);
		}

		public abstract T Value { get; protected set; }

		public IDisposable SetTestHook(T testHook)
		{
			IDisposable result = new Hookable<T>.ResetTestHook(this, this.Value);
			this.Value = ((this.setTestHookNullRestoresDefaultValue && testHook == null) ? this.defaultValue : testHook);
			return result;
		}

		private void UnsetTestHook(T oldValue)
		{
			if (this.setTestHookNullRestoresDefaultValue && oldValue == null)
			{
				this.Value = this.defaultValue;
				return;
			}
			this.Value = oldValue;
		}

		private readonly T defaultValue;

		private readonly bool setTestHookNullRestoresDefaultValue;

		private class HookableByReference : Hookable<T>
		{
			public HookableByReference(bool setTestHookNullRestoresDefaultValue, T defaultValue) : base(setTestHookNullRestoresDefaultValue, defaultValue)
			{
				this.activeValue = this.defaultValue;
			}

			public override T Value
			{
				get
				{
					return this.activeValue;
				}
				protected set
				{
					this.activeValue = value;
				}
			}

			private T activeValue;
		}

		private class HookableUsingDelegates : Hookable<T>
		{
			public HookableUsingDelegates(bool setTestHookNullRestoresDefaultValue, Func<T> activeValueGetter, Action<T> activeValueSetter) : base(setTestHookNullRestoresDefaultValue, activeValueGetter())
			{
				this.activeValueGetter = activeValueGetter;
				this.activeValueSetter = activeValueSetter;
			}

			public override T Value
			{
				get
				{
					return this.activeValueGetter();
				}
				protected set
				{
					this.activeValueSetter(value);
				}
			}

			private readonly Func<T> activeValueGetter;

			private readonly Action<T> activeValueSetter;
		}

		private class ResetTestHook : IDisposable
		{
			public ResetTestHook(Hookable<T> parent, T oldValue)
			{
				this.disposed = false;
				this.oldValue = oldValue;
				this.parent = parent;
			}

			public void Dispose()
			{
				if (!this.disposed)
				{
					this.disposed = true;
					this.parent.UnsetTestHook(this.oldValue);
				}
			}

			private bool disposed;

			private T oldValue;

			private Hookable<T> parent;
		}
	}
}
