using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal class CPUPowerWrapper
	{
		internal static bool IsProcessorThrottlingEnabled
		{
			get
			{
				int num = Marshal.SizeOf(typeof(SystemPowerCapabilites));
				IntPtr intPtr = Marshal.AllocCoTaskMem(num);
				bool processorThrottle;
				try
				{
					NativeMethods.NTSTATUS ntstatus = NativeMethods.CallNtPowerInformation(4, IntPtr.Zero, 0U, intPtr, (uint)num);
					if (ntstatus != NativeMethods.NTSTATUS.STATUS_SUCCESS)
					{
						throw new InvalidOperationException("Non-success result from CallNtPowerInformation: " + ntstatus);
					}
					processorThrottle = ((SystemPowerCapabilites)Marshal.PtrToStructure(intPtr, typeof(SystemPowerCapabilites))).ProcessorThrottle;
				}
				finally
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
				return processorThrottle;
			}
		}

		internal static bool GetCurrentPowerInfoForProcessor(int currentProcessor, out PROCESSOR_POWER_INFORMATION ppi)
		{
			IntPtr zero = IntPtr.Zero;
			bool flag = false;
			try
			{
				flag = CPUPowerWrapper.AllocateAndGetProcessorInfo(out zero);
				if (flag)
				{
					int offset = currentProcessor * CPUPowerWrapper.PpiSize;
					IntPtr ptr = CPUPowerWrapper.AddOffset(zero, offset);
					ppi = (PROCESSOR_POWER_INFORMATION)Marshal.PtrToStructure(ptr, typeof(PROCESSOR_POWER_INFORMATION));
				}
				else
				{
					ppi = default(PROCESSOR_POWER_INFORMATION);
				}
			}
			finally
			{
				Marshal.FreeCoTaskMem(zero);
			}
			return flag;
		}

		internal static bool GetMaxProcessorSpeeds(out int[] result)
		{
			IntPtr zero = IntPtr.Zero;
			bool flag = false;
			result = new int[Environment.ProcessorCount];
			try
			{
				flag = CPUPowerWrapper.AllocateAndGetProcessorInfo(out zero);
				if (flag)
				{
					int num = 0;
					for (int i = 0; i < Environment.ProcessorCount; i++)
					{
						IntPtr ptr = CPUPowerWrapper.AddOffset(zero, num);
						PROCESSOR_POWER_INFORMATION processor_POWER_INFORMATION = (PROCESSOR_POWER_INFORMATION)Marshal.PtrToStructure(ptr, typeof(PROCESSOR_POWER_INFORMATION));
						result[i] = (int)processor_POWER_INFORMATION.MaxMhz;
						num += CPUPowerWrapper.PpiSize;
					}
				}
			}
			finally
			{
				Marshal.FreeCoTaskMem(zero);
			}
			return flag;
		}

		private static IntPtr AddOffset(IntPtr pointer, int offset)
		{
			return (IntPtr)((long)pointer + (long)offset);
		}

		private static bool AllocateAndGetProcessorInfo(out IntPtr cpuInfo)
		{
			bool result = false;
			cpuInfo = Marshal.AllocCoTaskMem(CPUPowerWrapper.ProcessorSpeedArraySize);
			NativeMethods.NTSTATUS ntstatus = NativeMethods.CallNtPowerInformation(11, IntPtr.Zero, 0U, cpuInfo, (uint)CPUPowerWrapper.ProcessorSpeedArraySize);
			if (ntstatus == (NativeMethods.NTSTATUS)3221225507U)
			{
				throw new ArgumentException("CallNtPowerInformation was passed too small a buffer.", "cpuInfo");
			}
			if (ntstatus == NativeMethods.NTSTATUS.STATUS_SUCCESS)
			{
				result = true;
			}
			return result;
		}

		private const int SystemPowerCapabilitiesInformationLevel = 4;

		private const int ProcessorSpeedInformation = 11;

		private static readonly int PpiSize = Marshal.SizeOf(typeof(PROCESSOR_POWER_INFORMATION));

		private static readonly int ProcessorSpeedArraySize = Environment.ProcessorCount * CPUPowerWrapper.PpiSize;
	}
}
