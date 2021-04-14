using System;
using System.Collections;

namespace Microsoft.Exchange.Diagnostics
{
	public class BaseTrace
	{
		public static ThreadTraceSettings CurrentThreadSettings
		{
			get
			{
				return PerThreadData.CurrentThreadData.ThreadTraceSettings;
			}
		}

		public BaseTrace(Guid guid, int traceTag)
		{
			this.category = guid;
			this.traceTag = traceTag;
			this.enabledTypes = ExTraceConfiguration.Instance.EnabledTypesArray();
			this.enabledTags = ExTraceConfiguration.Instance.EnabledTagArray(this.category);
			this.enabledInMemoryTags = ExTraceConfiguration.Instance.EnabledInMemoryTagArray(this.category);
			this.perThreadModeEnabledTags = ExTraceConfiguration.Instance.PerThreadModeTagArray(this.category);
		}

		public Guid Category
		{
			get
			{
				return this.category;
			}
		}

		public int TraceTag
		{
			get
			{
				return this.traceTag;
			}
		}

		public bool IsTraceEnabled(TraceType traceType)
		{
			if (this.TestHook != null)
			{
				return this.TestHook.IsTraceEnabled(traceType);
			}
			return this.enabledTypes[(int)traceType] && ExTraceInternal.AreAnyTraceProvidersEnabled && (this.IsOtherProviderTracesEnabled || this.IsInMemoryTraceEnabled);
		}

		protected bool IsInMemoryTraceEnabled
		{
			get
			{
				return this.enabledInMemoryTags[this.traceTag];
			}
		}

		protected bool IsOtherProviderTracesEnabled
		{
			get
			{
				return this.enabledTags[this.traceTag] || (ExTraceConfiguration.Instance.PerThreadTracingConfigured && this.perThreadModeEnabledTags[this.traceTag] && BaseTrace.CurrentThreadSettings.IsEnabled);
			}
		}

		internal ITraceTestHook TestHook
		{
			get
			{
				if (this.testHook == null)
				{
					return null;
				}
				return this.testHook.Value;
			}
		}

		internal IDisposable SetTestHook(ITraceTestHook testHook)
		{
			if (this.testHook == null)
			{
				this.testHook = Hookable<ITraceTestHook>.Create(false, null);
			}
			return this.testHook.SetTestHook(testHook);
		}

		private Hookable<ITraceTestHook> testHook;

		private BitArray enabledTypes;

		private BitArray enabledTags;

		private BitArray enabledInMemoryTags;

		private BitArray perThreadModeEnabledTags;

		protected Guid category;

		protected int traceTag;
	}
}
