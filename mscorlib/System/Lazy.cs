using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System
{
	[ComVisible(false)]
	[DebuggerTypeProxy(typeof(System_LazyDebugView<>))]
	[DebuggerDisplay("ThreadSafetyMode={Mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
	[__DynamicallyInvokable]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	[Serializable]
	public class Lazy<T>
	{
		[__DynamicallyInvokable]
		public Lazy() : this(LazyThreadSafetyMode.ExecutionAndPublication)
		{
		}

		[__DynamicallyInvokable]
		public Lazy(Func<T> valueFactory) : this(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication)
		{
		}

		[__DynamicallyInvokable]
		public Lazy(bool isThreadSafe) : this(isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None)
		{
		}

		[__DynamicallyInvokable]
		public Lazy(LazyThreadSafetyMode mode)
		{
			this.m_threadSafeObj = Lazy<T>.GetObjectFromMode(mode);
		}

		[__DynamicallyInvokable]
		public Lazy(Func<T> valueFactory, bool isThreadSafe) : this(valueFactory, isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None)
		{
		}

		[__DynamicallyInvokable]
		public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
		{
			if (valueFactory == null)
			{
				throw new ArgumentNullException("valueFactory");
			}
			this.m_threadSafeObj = Lazy<T>.GetObjectFromMode(mode);
			this.m_valueFactory = valueFactory;
		}

		private static object GetObjectFromMode(LazyThreadSafetyMode mode)
		{
			if (mode == LazyThreadSafetyMode.ExecutionAndPublication)
			{
				return new object();
			}
			if (mode == LazyThreadSafetyMode.PublicationOnly)
			{
				return LazyHelpers.PUBLICATION_ONLY_SENTINEL;
			}
			if (mode != LazyThreadSafetyMode.None)
			{
				throw new ArgumentOutOfRangeException("mode", Environment.GetResourceString("Lazy_ctor_ModeInvalid"));
			}
			return null;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			T value = this.Value;
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			if (!this.IsValueCreated)
			{
				return Environment.GetResourceString("Lazy_ToString_ValueNotCreated");
			}
			T value = this.Value;
			return value.ToString();
		}

		internal T ValueForDebugDisplay
		{
			get
			{
				if (!this.IsValueCreated)
				{
					return default(T);
				}
				return ((Lazy<T>.Boxed)this.m_boxed).m_value;
			}
		}

		internal LazyThreadSafetyMode Mode
		{
			get
			{
				if (this.m_threadSafeObj == null)
				{
					return LazyThreadSafetyMode.None;
				}
				if (this.m_threadSafeObj == LazyHelpers.PUBLICATION_ONLY_SENTINEL)
				{
					return LazyThreadSafetyMode.PublicationOnly;
				}
				return LazyThreadSafetyMode.ExecutionAndPublication;
			}
		}

		internal bool IsValueFaulted
		{
			get
			{
				return this.m_boxed is Lazy<T>.LazyInternalExceptionHolder;
			}
		}

		[__DynamicallyInvokable]
		public bool IsValueCreated
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_boxed != null && this.m_boxed is Lazy<T>.Boxed;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[__DynamicallyInvokable]
		public T Value
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.m_boxed != null)
				{
					Lazy<T>.Boxed boxed = this.m_boxed as Lazy<T>.Boxed;
					if (boxed != null)
					{
						return boxed.m_value;
					}
					Lazy<T>.LazyInternalExceptionHolder lazyInternalExceptionHolder = this.m_boxed as Lazy<T>.LazyInternalExceptionHolder;
					lazyInternalExceptionHolder.m_edi.Throw();
				}
				Debugger.NotifyOfCrossThreadDependency();
				return this.LazyInitValue();
			}
		}

		private T LazyInitValue()
		{
			Lazy<T>.Boxed boxed = null;
			LazyThreadSafetyMode mode = this.Mode;
			if (mode == LazyThreadSafetyMode.None)
			{
				boxed = this.CreateValue();
				this.m_boxed = boxed;
			}
			else if (mode == LazyThreadSafetyMode.PublicationOnly)
			{
				boxed = this.CreateValue();
				if (boxed == null || Interlocked.CompareExchange(ref this.m_boxed, boxed, null) != null)
				{
					boxed = (Lazy<T>.Boxed)this.m_boxed;
				}
				else
				{
					this.m_valueFactory = Lazy<T>.ALREADY_INVOKED_SENTINEL;
				}
			}
			else
			{
				object obj = Volatile.Read<object>(ref this.m_threadSafeObj);
				bool flag = false;
				try
				{
					if (obj != Lazy<T>.ALREADY_INVOKED_SENTINEL)
					{
						Monitor.Enter(obj, ref flag);
					}
					if (this.m_boxed == null)
					{
						boxed = this.CreateValue();
						this.m_boxed = boxed;
						Volatile.Write<object>(ref this.m_threadSafeObj, Lazy<T>.ALREADY_INVOKED_SENTINEL);
					}
					else
					{
						boxed = (this.m_boxed as Lazy<T>.Boxed);
						if (boxed == null)
						{
							Lazy<T>.LazyInternalExceptionHolder lazyInternalExceptionHolder = this.m_boxed as Lazy<T>.LazyInternalExceptionHolder;
							lazyInternalExceptionHolder.m_edi.Throw();
						}
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(obj);
					}
				}
			}
			return boxed.m_value;
		}

		private Lazy<T>.Boxed CreateValue()
		{
			Lazy<T>.Boxed result = null;
			LazyThreadSafetyMode mode = this.Mode;
			if (this.m_valueFactory != null)
			{
				try
				{
					if (mode != LazyThreadSafetyMode.PublicationOnly && this.m_valueFactory == Lazy<T>.ALREADY_INVOKED_SENTINEL)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Lazy_Value_RecursiveCallsToValue"));
					}
					Func<T> valueFactory = this.m_valueFactory;
					if (mode != LazyThreadSafetyMode.PublicationOnly)
					{
						this.m_valueFactory = Lazy<T>.ALREADY_INVOKED_SENTINEL;
					}
					else if (valueFactory == Lazy<T>.ALREADY_INVOKED_SENTINEL)
					{
						return null;
					}
					return new Lazy<T>.Boxed(valueFactory());
				}
				catch (Exception ex)
				{
					if (mode != LazyThreadSafetyMode.PublicationOnly)
					{
						this.m_boxed = new Lazy<T>.LazyInternalExceptionHolder(ex);
					}
					throw;
				}
			}
			try
			{
				result = new Lazy<T>.Boxed((T)((object)Activator.CreateInstance(typeof(T))));
			}
			catch (MissingMethodException)
			{
				Exception ex2 = new MissingMemberException(Environment.GetResourceString("Lazy_CreateValue_NoParameterlessCtorForT"));
				if (mode != LazyThreadSafetyMode.PublicationOnly)
				{
					this.m_boxed = new Lazy<T>.LazyInternalExceptionHolder(ex2);
				}
				throw ex2;
			}
			return result;
		}

		private static readonly Func<T> ALREADY_INVOKED_SENTINEL = () => default(T);

		private object m_boxed;

		[NonSerialized]
		private Func<T> m_valueFactory;

		[NonSerialized]
		private object m_threadSafeObj;

		[Serializable]
		private class Boxed
		{
			internal Boxed(T value)
			{
				this.m_value = value;
			}

			internal T m_value;
		}

		private class LazyInternalExceptionHolder
		{
			internal LazyInternalExceptionHolder(Exception ex)
			{
				this.m_edi = ExceptionDispatchInfo.Capture(ex);
			}

			internal ExceptionDispatchInfo m_edi;
		}
	}
}
