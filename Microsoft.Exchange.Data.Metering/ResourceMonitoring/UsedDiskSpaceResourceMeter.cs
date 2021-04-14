using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class UsedDiskSpaceResourceMeter : ResourceMeter
	{
		public UsedDiskSpaceResourceMeter(string directoryName, PressureTransitions pressureTransitions) : this("UsedDiskSpace", directoryName, pressureTransitions)
		{
		}

		protected UsedDiskSpaceResourceMeter(string resourceName, string directoryName, PressureTransitions pressureTransitions) : base(resourceName, directoryName, pressureTransitions)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("directoryName", directoryName);
		}

		private protected long TotalNumberOfBytes { protected get; private set; }

		protected override long GetCurrentPressure()
		{
			ulong num;
			ulong num2;
			ulong num3;
			if (this.GetSpace(out num, out num2, out num3))
			{
				this.TotalNumberOfBytes = (long)num2;
				return (long)((num2 - num) * 100UL / num2);
			}
			return 0L;
		}

		protected bool GetSpace(out ulong freeBytesAvailable, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes)
		{
			return this.nativeMethods.GetDiskFreeSpaceEx(base.Resource.InstanceName, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);
		}

		private readonly INativeMethodsWrapper nativeMethods = NativeMethodsWrapperFactory.CreateNativeMethodsWrapper();
	}
}
