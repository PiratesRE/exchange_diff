using System;
using System.Configuration;

namespace Microsoft.Exchange.Diagnostics
{
	public abstract class AppSettingsReader<T>
	{
		public AppSettingsReader(string name, T defaultValue)
		{
			this.Name = name;
			this.value = defaultValue;
		}

		public string Name { get; private set; }

		public T Value
		{
			get
			{
				this.InitializeIfNeeded();
				return this.value;
			}
		}

		internal void Initialize(T value)
		{
			lock (this.locker)
			{
				this.initialized = true;
				this.value = value;
			}
		}

		protected abstract bool TryParseValue(string inputValue, out T outputValue);

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
							if (text != null && this.TryParseValue(text, out t))
							{
								this.value = t;
							}
						}
						catch (ConfigurationErrorsException)
						{
						}
						finally
						{
							this.initialized = true;
						}
					}
				}
			}
		}

		private T value;

		private bool initialized;

		private object locker = new object();
	}
}
