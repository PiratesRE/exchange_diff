using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal abstract class AppSettingsEntry<T>
	{
		public AppSettingsEntry(string name, T defaultValue, Trace tracer)
		{
			this.name = name;
			this.value = defaultValue;
			this.tracer = tracer;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public T Value
		{
			get
			{
				this.InitializeIfNeeded();
				return this.value;
			}
		}

		private void InitializeIfNeeded()
		{
			if (!this.initialized)
			{
				lock (this.locker)
				{
					if (!this.initialized)
					{
						try
						{
							string text = ConfigurationManager.AppSettings[this.Name];
							T t;
							if (text == null)
							{
								this.Trace("Property not defined, using default");
							}
							else if (this.TryParseValue(text, out t))
							{
								this.value = t;
								this.Trace("Property defined");
							}
							else
							{
								this.Trace("Could not read valid value, using default");
							}
						}
						catch (ConfigurationErrorsException e)
						{
							this.Trace("Caught configuration exception.  Using default.", e);
						}
						finally
						{
							this.initialized = true;
						}
					}
				}
			}
		}

		internal void Initialize(T value)
		{
			lock (this.locker)
			{
				this.initialized = true;
				this.value = value;
			}
			this.Trace("Explictly set");
		}

		private void Trace(string message)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceDebug<string, string, T>((long)this.GetHashCode(), "{0}: {1}={2}", message, this.name, this.value);
			}
		}

		private void Trace(string message, Exception e)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "{0}: {1}={2}. Exception: {3}", new object[]
				{
					message,
					this.name,
					this.value,
					e
				});
			}
		}

		protected abstract bool TryParseValue(string inputValue, out T outputValue);

		private string name;

		private T value;

		private Trace tracer;

		private bool initialized;

		private object locker = new object();
	}
}
