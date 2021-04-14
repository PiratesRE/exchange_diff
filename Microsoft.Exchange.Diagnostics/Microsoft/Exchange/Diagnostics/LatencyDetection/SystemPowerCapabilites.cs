using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal struct SystemPowerCapabilites
	{
		public bool PowerButtonPresent;

		public bool SleepButtonPresent;

		public bool LidPresent;

		public bool SystemS1;

		public bool SystemS2;

		public bool SystemS3;

		public bool SystemS4;

		public bool SystemS5;

		public bool HiberFilePresent;

		public bool FullWake;

		public bool VideoDimPresent;

		public bool ApmPresent;

		public bool UpsPresent;

		public bool ThermalControl;

		public bool ProcessorThrottle;

		public byte ProcessorMinThrottle;

		public byte ProcessorMaxThrottle;

		public bool FastSystemS4;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Bool)]
		public byte[] Spare2;

		public bool DiskSpinDown;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.Bool)]
		public byte[] Spare3;

		public bool SystemBatteriesPresent;

		public bool BatteriesAreShortTerm;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Struct)]
		public BatteryReportingScale[] BatteryScale;

		public int AcOnLineWake;

		public int SoftLidWake;

		public int RtcWake;

		public int MinDeviceWakeState;

		public int DefaultLowLatencyWake;
	}
}
