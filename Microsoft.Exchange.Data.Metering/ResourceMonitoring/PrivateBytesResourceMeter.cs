using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal sealed class PrivateBytesResourceMeter : ResourceMeter
	{
		public PrivateBytesResourceMeter(PressureTransitions pressureTransitions, ulong totalPhysicalMemory) : base("PrivateBytes", string.Empty, pressureTransitions)
		{
			ArgumentValidator.ThrowIfInvalidValue<ulong>("totalPhysicalMemory", totalPhysicalMemory, (ulong memory) => memory > 0UL);
			this.totalPhysicalMemory = totalPhysicalMemory;
		}

		protected override long GetCurrentPressure()
		{
			ulong num;
			if (this.nativeMethods.GetProcessPrivateBytes(out num))
			{
				return (long)(num * 100UL / this.totalPhysicalMemory);
			}
			return 0L;
		}

		private readonly INativeMethodsWrapper nativeMethods = NativeMethodsWrapperFactory.CreateNativeMethodsWrapper();

		private readonly ulong totalPhysicalMemory;
	}
}
