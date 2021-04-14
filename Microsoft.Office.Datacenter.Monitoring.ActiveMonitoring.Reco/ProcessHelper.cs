using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class ProcessHelper
	{
		public static void Kill(List<int> processIds, ProcessKillMode killMode, string context)
		{
			foreach (int processId in processIds)
			{
				using (Process processById = Process.GetProcessById(processId))
				{
					if (processById != null)
					{
						ProcessHelper.Kill(processById, killMode, context);
					}
				}
			}
		}

		public static void Kill(Process process, ProcessKillMode killMode, string context)
		{
			List<Process> list = new List<Process>();
			try
			{
				if (killMode != ProcessKillMode.SelfAndChildren)
				{
					if (killMode != ProcessKillMode.ChildrenAndSelf)
					{
						goto IL_69;
					}
				}
				try
				{
					List<int> children = ProcessHelper.GetChildren(process);
					foreach (int processId in children)
					{
						Process processByIdBestEffort = ProcessHelper.GetProcessByIdBestEffort(processId);
						if (processByIdBestEffort != null)
						{
							list.Add(processByIdBestEffort);
						}
					}
				}
				catch (Win32Exception ex)
				{
					ManagedAvailabilityCrimsonEvents.ActiveMonitoringUnexpectedError.Log<string, string>(context, ex.Message);
				}
				IL_69:
				if (killMode == ProcessKillMode.ChildrenAndSelf)
				{
					foreach (Process process2 in list)
					{
						ProcessHelper.KillProcess(process2, true, context);
					}
				}
				ProcessHelper.KillProcess(process, false, context);
				if (killMode == ProcessKillMode.SelfAndChildren)
				{
					foreach (Process process3 in list)
					{
						ProcessHelper.KillProcess(process3, true, context);
					}
				}
			}
			finally
			{
				foreach (Process process4 in list)
				{
					if (process4 != null)
					{
						process4.Dispose();
					}
				}
			}
		}

		public static void KillProcess(Process process, bool isChild, string context)
		{
			Exception ex = null;
			int id = process.Id;
			string processName = process.ProcessName;
			try
			{
				ManagedAvailabilityCrimsonEvents.ProcessKillAttempted.Log<int, string, bool, string>(id, processName, isChild, context);
				if (!process.HasExited)
				{
					process.Kill();
				}
			}
			catch (Exception ex2)
			{
				if (!(ex2 is InvalidOperationException))
				{
					ex = ex2;
					throw;
				}
			}
			finally
			{
				if (ex != null)
				{
					ManagedAvailabilityCrimsonEvents.ProcessKillFailed.Log<int, string, bool, string, string>(id, processName, isChild, context, ex.Message);
				}
				else
				{
					ManagedAvailabilityCrimsonEvents.ProcessKillSucceeded.Log<int, string, bool, string>(id, processName, isChild, context);
				}
			}
		}

		public static bool IsValidPid(int pid)
		{
			return pid > 0;
		}

		public static void Enumerate(Func<ProcessNativeMethods.PROCESSENTRY32W, bool> action)
		{
			int num = 0;
			using (ToolhelpSnapshotSafeHandle toolhelpSnapshotSafeHandle = ProcessNativeMethods.CreateToolhelp32Snapshot(2U, 0U))
			{
				if (toolhelpSnapshotSafeHandle.IsInvalid)
				{
					num = Marshal.GetLastWin32Error();
				}
				else
				{
					ProcessNativeMethods.PROCESSENTRY32W processentry32W = default(ProcessNativeMethods.PROCESSENTRY32W);
					processentry32W.Size = (uint)Marshal.SizeOf(processentry32W);
					if (ProcessNativeMethods.Process32FirstW(toolhelpSnapshotSafeHandle, ref processentry32W))
					{
						while (action(processentry32W))
						{
							processentry32W.Size = (uint)Marshal.SizeOf(processentry32W);
							if (!ProcessNativeMethods.Process32NextW(toolhelpSnapshotSafeHandle, ref processentry32W))
							{
								num = Marshal.GetLastWin32Error();
								break;
							}
						}
					}
					else
					{
						num = Marshal.GetLastWin32Error();
					}
				}
			}
			if (num != 0 && num != 18)
			{
				throw new Win32Exception(num);
			}
		}

		public static int GetParent(int pid)
		{
			int parent = -1;
			ProcessHelper.Enumerate(delegate(ProcessNativeMethods.PROCESSENTRY32W processEntry)
			{
				bool result = true;
				if ((ulong)processEntry.ProcessId == (ulong)((long)pid))
				{
					parent = (int)processEntry.ParentProcessId;
					result = false;
				}
				return result;
			});
			return parent;
		}

		public static List<int> GetChildren(Process process)
		{
			List<int> children = new List<int>();
			ProcessHelper.Enumerate(delegate(ProcessNativeMethods.PROCESSENTRY32W processEntry)
			{
				int parentProcessId = (int)processEntry.ParentProcessId;
				if (parentProcessId == process.Id)
				{
					children.Add((int)processEntry.ProcessId);
				}
				return true;
			});
			return children;
		}

		public static Process GetProcessByIdBestEffort(int processId)
		{
			Process result = null;
			try
			{
				result = Process.GetProcessById(processId);
			}
			catch (ArgumentException)
			{
			}
			return result;
		}
	}
}
