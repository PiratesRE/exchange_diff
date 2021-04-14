using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace Microsoft.Exchange.Search.Core.Common
{
	public class NoderunnerResourceHelper
	{
		public NoderunnerResourceHelper()
		{
			this.ProcessDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			this.PopulateProcessDictionary();
		}

		public Dictionary<string, int> ProcessDictionary { get; private set; }

		public bool IsIndexNodeMemoryUsageExceeded(long memoryMaxUsage)
		{
			int pid = this.GetPid("IndexNode1");
			return pid != 0 && this.GetMemoryUsage(pid) > memoryMaxUsage;
		}

		private void PopulateProcessDictionary()
		{
			string queryString = "SELECT ProcessId, CommandLine from Win32_Process WHERE Name LIKE \"%NodeRunner%\"";
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						try
						{
							int value = 0;
							if (managementObject["CommandLine"] != null && managementObject["ProcessId"] != null && int.TryParse(managementObject["ProcessId"].ToString(), out value))
							{
								foreach (string text in NoderunnerResourceHelper.NodeRunnerInstanceNames)
								{
									if (managementObject["CommandLine"].ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
									{
										this.ProcessDictionary[text] = value;
										break;
									}
								}
							}
						}
						finally
						{
							managementObject.Dispose();
						}
					}
				}
			}
		}

		private int GetPid(string fastNodeName)
		{
			int result;
			if (!this.ProcessDictionary.TryGetValue(fastNodeName, out result))
			{
				return 0;
			}
			return result;
		}

		private long GetMemoryUsage(int pid)
		{
			long result = 0L;
			try
			{
				using (Process processById = Process.GetProcessById(pid))
				{
					result = processById.WorkingSet64;
				}
			}
			catch (ArgumentException)
			{
				return 0L;
			}
			return result;
		}

		private const string AdminNode1 = "AdminNode1";

		private const string ContentEngineNode1 = "ContentEngineNode1";

		private const string InteractionEngineNode1 = "InteractionEngineNode1";

		private const string IndexNode1 = "IndexNode1";

		private static readonly string[] NodeRunnerInstanceNames = new string[]
		{
			"AdminNode1",
			"ContentEngineNode1",
			"IndexNode1",
			"InteractionEngineNode1"
		};
	}
}
