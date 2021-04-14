using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class ProcessDumpHelper
	{
		public ProcessDumpHelper(CommonDumpParameters dumpParameters, CancellationToken cancellationToken)
		{
			this.args = dumpParameters.Clone();
			if (!this.args.IgnoreRegistryOverride)
			{
				this.ApplyOverrides();
			}
			this.cancellationToken = cancellationToken;
		}

		public static double GetFreeSpacePercentage(string path)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			ulong num3 = 0UL;
			bool diskFreeSpaceEx = ProcessNativeMethods.GetDiskFreeSpaceEx(path, out num, out num2, out num3);
			if (!diskFreeSpaceEx)
			{
				throw new Win32Exception();
			}
			if (num2 > 0UL)
			{
				return 100UL * num / num2;
			}
			throw new InvalidOperationException("Total bytes available is zero");
		}

		public string Generate(Process process, string initiator)
		{
			if (this.args.Mode == DumpMode.None)
			{
				return null;
			}
			if (!Directory.Exists(this.args.Path))
			{
				Directory.CreateDirectory(this.args.Path);
			}
			double freeSpacePercentage = ProcessDumpHelper.GetFreeSpacePercentage(this.args.Path);
			if (freeSpacePercentage < this.args.MinimumFreeSpace)
			{
				throw new DumpFreeSpaceRequirementNotSatisfiedException(this.args.Path, freeSpacePercentage, this.args.MinimumFreeSpace);
			}
			string machineName = Environment.MachineName;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(process.MainModule.FileName);
			DiagnosticsNativeMethods.MINIDUMP_TYPE dumpFlags = DiagnosticsNativeMethods.MINIDUMP_TYPE.MiniDumpWithDataSegs | DiagnosticsNativeMethods.MINIDUMP_TYPE.MiniDumpWithHandleData | DiagnosticsNativeMethods.MINIDUMP_TYPE.MiniDumpWithUnloadedModules | DiagnosticsNativeMethods.MINIDUMP_TYPE.MiniDumpWithProcessThreadData | DiagnosticsNativeMethods.MINIDUMP_TYPE.MiniDumpWithPrivateReadWriteMemory;
			if (this.args.Mode == DumpMode.FullDump)
			{
				dumpFlags |= DiagnosticsNativeMethods.MINIDUMP_TYPE.MiniDumpWithFullMemory;
			}
			string text = string.Format("{0}-{1}-{2}-{3}-{4}-{5}.dmp", new object[]
			{
				fileNameWithoutExtension.ToLower(),
				DateTime.UtcNow.ToString("yyMMddHHmmssfff"),
				(this.args.Mode == DumpMode.FullDump) ? "full" : "mini",
				machineName.ToLower(),
				process.Id,
				initiator.ToLower()
			});
			text = Utilities.NormalizeStringToValidFileOrRegistryKeyName(text);
			string dumpFileFullName = Path.Combine(this.args.Path, text);
			this.cancellationToken.ThrowIfCancellationRequested();
			RecoveryActionHelper.RunAndMeasure(string.Format("Dump({0})", dumpFileFullName), true, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
			{
				using (FileStream fileStream = File.OpenWrite(dumpFileFullName))
				{
					if (!ProcessNativeMethods.MiniDumpWriteDump(process.Handle, process.Id, fileStream.SafeFileHandle.DangerousGetHandle(), (uint)dumpFlags, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
					{
						throw new Win32Exception(Marshal.GetLastWin32Error());
					}
				}
				return string.Empty;
			});
			return dumpFileFullName;
		}

		private void ApplyOverrides()
		{
			string propertyAsString = RegistryHelper.GetPropertyAsString("MaxDumpMode", null, null, false);
			DumpMode dumpMode;
			if (string.IsNullOrEmpty(propertyAsString) || !EnumValidator.TryParse<DumpMode>(propertyAsString, EnumParseOptions.AllowNumericConstants | EnumParseOptions.IgnoreCase, out dumpMode))
			{
				dumpMode = DumpMode.None;
			}
			if (this.args.Mode > dumpMode)
			{
				this.args.Mode = dumpMode;
			}
		}

		internal const string MaxDumpModeOverrideValueName = "MaxDumpMode";

		private CommonDumpParameters args;

		private CancellationToken cancellationToken;
	}
}
