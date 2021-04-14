using System;
using System.ComponentModel;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class SystemMemoryResourceMeter : ResourceMeter
	{
		public SystemMemoryResourceMeter(PressureTransitions pressureTransitions) : base("SystemMemory", string.Empty, pressureTransitions)
		{
		}

		protected override long GetCurrentPressure()
		{
			uint num;
			if (this.nativeMethods.GetSystemMemoryUsePercentage(out num))
			{
				return (long)((ulong)num);
			}
			throw new TransientException(new LocalizedString("Failed to get the system memory usage."), new Win32Exception());
		}

		private readonly INativeMethodsWrapper nativeMethods = NativeMethodsWrapperFactory.CreateNativeMethodsWrapper();
	}
}
