using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	public static class PerformanceCounterFactory
	{
		public static IPerformanceCounter CreatePerformanceCounter()
		{
			return PerformanceCounterFactory.hookableFactory.Value.CreatePerformanceCounter();
		}

		public static IPerformanceCounterCategory CreatePerformanceCounterCategory(string categoryName)
		{
			return PerformanceCounterFactory.hookableFactory.Value.CreatePerformanceCounterCategory(categoryName);
		}

		internal static IDisposable SetTestHook(PerformanceCounterFactory.IFactory factory)
		{
			return PerformanceCounterFactory.hookableFactory.SetTestHook(factory);
		}

		private static Hookable<PerformanceCounterFactory.IFactory> hookableFactory = Hookable<PerformanceCounterFactory.IFactory>.Create(true, new PerformanceCounterFactory.Factory());

		internal interface IFactory
		{
			IPerformanceCounter CreatePerformanceCounter();

			IPerformanceCounterCategory CreatePerformanceCounterCategory(string categoryName);
		}

		private class Factory : PerformanceCounterFactory.IFactory
		{
			public IPerformanceCounter CreatePerformanceCounter()
			{
				return new PerformanceCounterFactory.PerformanceCounterWrapper();
			}

			public IPerformanceCounterCategory CreatePerformanceCounterCategory(string categoryName)
			{
				return new PerformanceCounterFactory.PerformanceCounterCategoryWrapper(categoryName);
			}
		}

		private class PerformanceCounterWrapper : IPerformanceCounter, IDisposable
		{
			public PerformanceCounterWrapper()
			{
				this.counter = new PerformanceCounter();
			}

			string IPerformanceCounter.CategoryName
			{
				get
				{
					return this.counter.CategoryName;
				}
				set
				{
					this.counter.CategoryName = value;
				}
			}

			string IPerformanceCounter.CounterName
			{
				get
				{
					return this.counter.CounterName;
				}
				set
				{
					this.counter.CounterName = value;
				}
			}

			string IPerformanceCounter.CounterHelp
			{
				get
				{
					return this.counter.CounterHelp;
				}
			}

			string IPerformanceCounter.InstanceName
			{
				get
				{
					return this.counter.InstanceName;
				}
				set
				{
					this.counter.InstanceName = value;
				}
			}

			bool IPerformanceCounter.ReadOnly
			{
				get
				{
					return this.counter.ReadOnly;
				}
				set
				{
					this.counter.ReadOnly = value;
				}
			}

			PerformanceCounterInstanceLifetime IPerformanceCounter.InstanceLifetime
			{
				get
				{
					return this.counter.InstanceLifetime;
				}
				set
				{
					this.counter.InstanceLifetime = value;
				}
			}

			PerformanceCounterType IPerformanceCounter.CounterType
			{
				get
				{
					return this.counter.CounterType;
				}
			}

			long IPerformanceCounter.RawValue
			{
				get
				{
					return this.counter.RawValue;
				}
				set
				{
					this.counter.RawValue = value;
				}
			}

			long IPerformanceCounter.IncrementBy(long incrementValue)
			{
				return this.counter.IncrementBy(incrementValue);
			}

			float IPerformanceCounter.NextValue()
			{
				return this.counter.NextValue();
			}

			void IPerformanceCounter.RemoveInstance()
			{
				this.counter.RemoveInstance();
			}

			void IPerformanceCounter.Close()
			{
				this.counter.Close();
			}

			void IDisposable.Dispose()
			{
				this.counter.Dispose();
			}

			private PerformanceCounter counter;
		}

		private class PerformanceCounterCategoryWrapper : IPerformanceCounterCategory
		{
			public PerformanceCounterCategoryWrapper(string categoryName)
			{
				this.category = new PerformanceCounterCategory(categoryName);
			}

			bool IPerformanceCounterCategory.InstanceExists(string instanceName)
			{
				return this.category.InstanceExists(instanceName);
			}

			string[] IPerformanceCounterCategory.GetInstanceNames()
			{
				return this.category.GetInstanceNames();
			}

			private PerformanceCounterCategory category;
		}
	}
}
