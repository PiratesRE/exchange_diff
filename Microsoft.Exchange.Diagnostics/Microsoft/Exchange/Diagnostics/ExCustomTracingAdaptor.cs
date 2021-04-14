using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	internal abstract class ExCustomTracingAdaptor<T>
	{
		public bool IsTracingEnabled(T identity)
		{
			ExTraceConfiguration instance = ExTraceConfiguration.Instance;
			if (!instance.PerThreadTracingConfigured)
			{
				return false;
			}
			if (this.traceEnabledFields == null || this.tracingConfigVersion < instance.Version)
			{
				lock (this)
				{
					if (this.traceEnabledFields == null || this.tracingConfigVersion < instance.Version)
					{
						this.traceEnabledFields = this.LoadEnabledIdentities(instance);
						this.tracingConfigVersion = instance.Version;
					}
				}
			}
			return this.traceEnabledFields.Contains(identity);
		}

		protected abstract HashSet<T> LoadEnabledIdentities(ExTraceConfiguration currentInstance);

		private int tracingConfigVersion;

		private HashSet<T> traceEnabledFields;
	}
}
