using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Win32;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	internal class MemoryRestartResponderChecker : RestartResponderChecker
	{
		internal MemoryRestartResponderChecker(ResponderDefinition definition) : base(definition)
		{
		}

		internal override string SkipReasonOrException
		{
			get
			{
				return this.skipReasonOrException;
			}
		}

		protected override bool IsWithinThreshold()
		{
			this.skipReasonOrException = null;
			try
			{
				if (this.memoryThreshold > 0)
				{
					uint memoryLoad = MemoryRestartResponderChecker.GetMemoryLoad();
					if ((ulong)memoryLoad < (ulong)((long)this.memoryThreshold))
					{
						this.skipReasonOrException = string.Format("Skipped Due to Low Memory. Real value = {0}, Threshold = {1}.", memoryLoad, this.memoryThreshold);
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				this.skipReasonOrException = ex.ToString();
			}
			return true;
		}

		internal override string KeyOfEnabled
		{
			get
			{
				return "MemoryRestartResponderCheckerEnabled";
			}
		}

		internal override string KeyOfSetting
		{
			get
			{
				return "MemoryLoadThreshold";
			}
		}

		internal override string DefaultSetting
		{
			get
			{
				return 4.ToString();
			}
		}

		protected override bool OnSettingChange(string newSetting)
		{
			try
			{
				this.memoryThreshold = int.Parse(newSetting);
			}
			catch (Exception ex)
			{
				this.skipReasonOrException = ex.ToString();
				return false;
			}
			return true;
		}

		private static uint GetMemoryLoad()
		{
			NativeMethods.MemoryStatusEx memoryStatusEx = default(NativeMethods.MemoryStatusEx);
			memoryStatusEx.Length = (uint)Marshal.SizeOf(typeof(NativeMethods.MemoryStatusEx));
			if (!NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
			{
				return 100U;
			}
			return memoryStatusEx.MemoryLoad;
		}

		private const string ReasonToSkipFormat = "Skipped Due to Low Memory. Real value = {0}, Threshold = {1}.";

		private const int DefaultMinimumMemoryThreshold = 4;

		private const string MemoryRestartResponderCheckerEnabled = "MemoryRestartResponderCheckerEnabled";

		private const string MemoryLoadThreshold = "MemoryLoadThreshold";

		private string skipReasonOrException;

		private int memoryThreshold;
	}
}
