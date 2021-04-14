using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ConfigDriverBase : DisposeTrackableBase, IConfigDriver, IDisposable
	{
		public ConfigDriverBase(IConfigSchema schema) : this(schema, new TimeSpan?(ConfigDriverBase.DefaultErrorThresholdInterval))
		{
		}

		public ConfigDriverBase(IConfigSchema schema, TimeSpan? errorThresholdInterval)
		{
			this.Schema = schema;
			this.IsInitialized = false;
			this.ErrorThresholdInterval = errorThresholdInterval;
			this.LastKnownErrors = new List<ConfigDriverBase.DiagnosticsError>();
			this.LastUpdated = DateTime.MinValue;
		}

		public bool IsInitialized { get; protected set; }

		public DateTime LastUpdated { get; protected set; }

		private protected IConfigSchema Schema { protected get; private set; }

		private TimeSpan? ErrorThresholdInterval { get; set; }

		private List<ConfigDriverBase.DiagnosticsError> LastKnownErrors { get; set; }

		protected object ParseAndValidateConfigValue(string settingName, string serializedValue, Type settingType)
		{
			return this.Schema.ParseAndValidateConfigValue(settingName, serializedValue, settingType);
		}

		public abstract void Initialize();

		public abstract bool TryGetBoxedSetting(ISettingsContext context, string settingName, Type settingType, out object settingValue);

		public virtual XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement(base.GetType().Name);
			xelement.Add(new XAttribute("name", this.Schema.Name));
			xelement.Add(new XAttribute("LastUpdated", this.LastUpdated));
			XElement xelement2 = new XElement("LastKnownErrors");
			lock (this.errorLock)
			{
				foreach (ConfigDriverBase.DiagnosticsError diagnosticsError in this.LastKnownErrors)
				{
					xelement2.Add(diagnosticsError.GetDiagnosticInfo());
				}
			}
			xelement.Add(xelement2);
			return xelement;
		}

		protected virtual void HandleLoadError(Exception ex)
		{
			lock (this.errorLock)
			{
				if (this.LastKnownErrors.Count >= 50)
				{
					this.LastKnownErrors[this.LastKnownErrors.Count - 1] = new ConfigDriverBase.DiagnosticsError(ex);
				}
				else
				{
					this.LastKnownErrors.Add(new ConfigDriverBase.DiagnosticsError(ex));
				}
				if (this.ErrorThresholdInterval != null && this.ErrorThresholdInterval.Value < DateTime.UtcNow - this.LastKnownErrors[0].RaisedAt)
				{
					throw ex;
				}
			}
		}

		protected void HandleLoadSuccess()
		{
			lock (this.errorLock)
			{
				this.LastKnownErrors.Clear();
			}
		}

		private const int MaximumLastKnownErrorSize = 50;

		public static readonly TimeSpan DefaultErrorThresholdInterval = TimeSpan.FromMinutes(5.0);

		private object errorLock = new object();

		protected class DiagnosticsError
		{
			public DiagnosticsError(Exception ex)
			{
				if (ex == null)
				{
					throw new ArgumentNullException("ex");
				}
				this.Exception = ex;
				this.RaisedAt = DateTime.UtcNow;
			}

			public Exception Exception { get; set; }

			public DateTime RaisedAt { get; private set; }

			public XElement GetDiagnosticInfo()
			{
				if (this.Exception == null)
				{
					return null;
				}
				return new XElement("LastKnownError", new object[]
				{
					new XAttribute("RaisedAt", this.RaisedAt),
					new XElement("Exception", this.Exception)
				});
			}
		}
	}
}
