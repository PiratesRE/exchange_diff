using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.EventLog;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal abstract class Config : IConfig
	{
		internal Config() : this(AppConfigAdapter.Instance)
		{
		}

		protected Config(IConfigAdapter configAdapter)
		{
			Util.ThrowOnNullArgument(configAdapter, "configAdapter");
			this.configAdapter = configAdapter;
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("Config", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreComponentTracer, (long)this.GetHashCode());
			this.Load();
		}

		public virtual void Load()
		{
		}

		protected bool ReadBool(string key, bool defaultValue)
		{
			bool isDefault = false;
			bool? flag = null;
			try
			{
				string setting = this.configAdapter.GetSetting(key);
				if (!string.IsNullOrEmpty(setting))
				{
					flag = new bool?(bool.Parse(setting));
				}
			}
			catch (FormatException ex)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex
				});
			}
			catch (ConfigurationErrorsException ex2)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex2
				});
			}
			if (flag == null)
			{
				flag = new bool?(defaultValue);
				isDefault = true;
			}
			this.TraceConfigValue(key, flag, isDefault);
			return flag.Value;
		}

		protected int ReadInt(string key, int defaultValue)
		{
			bool isDefault = false;
			int? num = null;
			try
			{
				string setting = this.configAdapter.GetSetting(key);
				if (!string.IsNullOrEmpty(setting))
				{
					num = new int?(int.Parse(setting));
				}
			}
			catch (FormatException ex)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex
				});
			}
			catch (ConfigurationErrorsException ex2)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex2
				});
			}
			if (num == null)
			{
				num = new int?(defaultValue);
				isDefault = true;
			}
			this.TraceConfigValue(key, num, isDefault);
			return num.Value;
		}

		protected string ReadString(string key, string defaultValue)
		{
			bool isDefault = false;
			string text = null;
			try
			{
				text = this.configAdapter.GetSetting(key);
			}
			catch (ConfigurationErrorsException ex)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex
				});
			}
			if (string.IsNullOrEmpty(text))
			{
				text = defaultValue;
				isDefault = true;
			}
			this.TraceConfigValue(key, text, isDefault);
			return text;
		}

		protected Guid ReadGuid(string key, Guid defaultValue)
		{
			bool isDefault = false;
			Guid? guid = null;
			try
			{
				string setting = this.configAdapter.GetSetting(key);
				if (!string.IsNullOrEmpty(setting))
				{
					guid = new Guid?(Guid.Parse(setting));
				}
			}
			catch (FormatException ex)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex
				});
			}
			catch (ConfigurationErrorsException ex2)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex2
				});
			}
			if (guid == null)
			{
				guid = new Guid?(defaultValue);
				isDefault = true;
			}
			this.TraceConfigValue(key, guid, isDefault);
			return guid.Value;
		}

		protected TimeSpan ReadTimeSpan(string key, TimeSpan defaultValue)
		{
			bool isDefault = false;
			TimeSpan? timeSpan = null;
			try
			{
				string setting = this.configAdapter.GetSetting(key);
				if (!string.IsNullOrEmpty(setting))
				{
					timeSpan = new TimeSpan?(TimeSpan.Parse(setting));
				}
			}
			catch (FormatException ex)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex
				});
			}
			catch (ConfigurationErrorsException ex2)
			{
				this.diagnosticsSession.LogEvent(MSExchangeFastSearchEventLogConstants.Tuple_InvalidConfiguration, new object[]
				{
					ex2
				});
			}
			if (timeSpan == null)
			{
				timeSpan = new TimeSpan?(defaultValue);
				isDefault = true;
			}
			this.TraceConfigValue(key, timeSpan, isDefault);
			return timeSpan.Value;
		}

		private void TraceConfigValue(string key, object value, bool isDefault)
		{
			this.diagnosticsSession.TraceDebug<string, object, string>("Reading config: {0}, value: '{1}'{2}.", key, value, isDefault ? " (default)" : string.Empty);
		}

		private readonly IConfigAdapter configAdapter;

		private readonly IDiagnosticsSession diagnosticsSession;
	}
}
