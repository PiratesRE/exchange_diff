using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public static class HpAcuCliWrapper
	{
		private static string InvokeHpAcuCli(string command)
		{
			foreach (string processName in HpAcuCliWrapper.HpAcuProcesses)
			{
				Process[] processesByName = Process.GetProcessesByName(processName);
				if (processesByName != null && processesByName.Length > 0)
				{
					foreach (Process process in processesByName)
					{
						using (process)
						{
							process.Kill();
						}
					}
				}
			}
			if (string.IsNullOrEmpty(HpAcuCliWrapper.HpAcuCliProcessLocation))
			{
				HpAcuCliWrapper.HpAcuCliProcessLocation = string.Format("{0}\\Compaq\\Hpacucli\\bin\\hpacucli.exe", "C:\\Program Files");
				if (!File.Exists(HpAcuCliWrapper.HpAcuCliProcessLocation))
				{
					HpAcuCliWrapper.HpAcuCliProcessLocation = string.Format("{0}\\Compaq\\Hpacucli\\bin\\hpacucli.exe", "C:\\Program Files (x86)");
				}
			}
			string result = string.Empty;
			string text = string.Empty;
			using (Process process3 = new Process())
			{
				process3.StartInfo.FileName = HpAcuCliWrapper.HpAcuCliProcessLocation;
				process3.StartInfo.Arguments = command;
				process3.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process3.StartInfo.CreateNoWindow = true;
				process3.StartInfo.RedirectStandardOutput = true;
				process3.StartInfo.RedirectStandardError = true;
				process3.StartInfo.UseShellExecute = false;
				process3.Start();
				result = process3.StandardOutput.ReadToEnd();
				text = process3.StandardError.ReadToEnd();
				process3.WaitForExit(30000);
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				throw new ApplicationException(string.Format("HpAcuCli returned error - {0}", text));
			}
			return result;
		}

		public static HpAcuCliWrapper.ControllerStatusSimple[] ListAllControllers()
		{
			return HpAcuCliWrapper.ControllerStatusSimple.ConvertFromRawString(HpAcuCliWrapper.InvokeHpAcuCli("controller all show status"));
		}

		public static HpAcuCliWrapper.ModifyCommandResult ModifyLogicalDriveCaching(int slotNumber, bool enable)
		{
			return new HpAcuCliWrapper.ModifyCommandResult(HpAcuCliWrapper.InvokeHpAcuCli(string.Format("controller slot={0} logicaldrive all modify caching={1}", slotNumber, enable ? "enable" : "disable")));
		}

		public static HpAcuCliWrapper.ModifyCommandResult ModifyLogicalDriveArrayAccelerator(int slotNumber, bool enable)
		{
			return new HpAcuCliWrapper.ModifyCommandResult(HpAcuCliWrapper.InvokeHpAcuCli(string.Format("controller slot={0} logicaldrive all modify arrayaccelerator={1}", slotNumber, enable ? "enable" : "disable")));
		}

		public static HpAcuCliWrapper.ModifyCommandResult ResetNoBatteryWriteCache(int slotNumber)
		{
			return new HpAcuCliWrapper.ModifyCommandResult(HpAcuCliWrapper.InvokeHpAcuCli(string.Format("controller slot={0} modify nobatterywritecache=disable", slotNumber)));
		}

		public static HpAcuCliWrapper.ModifyCommandResult ResetCacheRatio(int slotNumber)
		{
			return new HpAcuCliWrapper.ModifyCommandResult(HpAcuCliWrapper.InvokeHpAcuCli(string.Format("controller slot={0} modify cacheratio=0/100", slotNumber)));
		}

		private const string ListAllControllersCmd = "controller all show status";

		private const string ModifyLogicalDriveCachingCmd = "controller slot={0} logicaldrive all modify caching={1}";

		private const string ModifyLogicalDriveArrayAcceleratorCmd = "controller slot={0} logicaldrive all modify arrayaccelerator={1}";

		private const string ModifyCacheRatioCmd = "controller slot={0} modify cacheratio=0/100";

		private const string SetNoBatteryWriteCacheCmd = "controller slot={0} modify nobatterywritecache=disable";

		private const string HpAcuCliLocationPattern = "{0}\\Compaq\\Hpacucli\\bin\\hpacucli.exe";

		private static readonly string[] HpAcuProcesses = new string[]
		{
			"hpacuhost",
			"hpacucli"
		};

		private static string HpAcuCliProcessLocation = string.Empty;

		public class ControllerStatusSimple
		{
			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public int SlotNumber
			{
				get
				{
					return this.slotNumber;
				}
			}

			public string Status
			{
				get
				{
					return this.status;
				}
			}

			public string CacheStatus
			{
				get
				{
					return this.cacheStatus;
				}
			}

			public string BatteryStatus
			{
				get
				{
					return this.batteryStatus;
				}
			}

			public static HpAcuCliWrapper.ControllerStatusSimple[] ConvertFromRawString(string rawOutput)
			{
				if (string.IsNullOrWhiteSpace(rawOutput) && !rawOutput.StartsWith("Error"))
				{
					return null;
				}
				string[] array = rawOutput.Split(HpAcuCliWrapper.ControllerStatusSimple.splitList, StringSplitOptions.None);
				HpAcuCliWrapper.ControllerStatusSimple controllerStatusSimple = null;
				List<HpAcuCliWrapper.ControllerStatusSimple> list = new List<HpAcuCliWrapper.ControllerStatusSimple>();
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrWhiteSpace(array[i]))
					{
						if (!array[i].StartsWith(" "))
						{
							if (controllerStatusSimple != null)
							{
								list.Add(controllerStatusSimple);
							}
							controllerStatusSimple = new HpAcuCliWrapper.ControllerStatusSimple();
							string[] array2 = array[i].Split(new string[]
							{
								" in "
							}, StringSplitOptions.None);
							if (array2.Length >= 2)
							{
								controllerStatusSimple.name = array2[0].Trim();
								Match match = HpAcuCliWrapper.ControllerStatusSimple.slotMatch.Match(array2[1]);
								if (match.Success)
								{
									string value = match.Groups["slotnumber"].Value;
									int num = int.Parse(value);
									controllerStatusSimple.slotNumber = num;
								}
							}
						}
						else if (controllerStatusSimple != null)
						{
							string[] array3 = array[i].Split(new char[]
							{
								':'
							});
							if (array3 != null && array3.Length >= 2)
							{
								string text = array3[0].Trim();
								string text2 = array3[1].Trim();
								if (text.Equals("Controller Status", StringComparison.OrdinalIgnoreCase))
								{
									controllerStatusSimple.status = text2;
								}
								else if (text.Equals("Cache Status", StringComparison.OrdinalIgnoreCase))
								{
									controllerStatusSimple.cacheStatus = text2;
								}
								else if (text.Equals("Battery/Capacitor Status", StringComparison.OrdinalIgnoreCase))
								{
									controllerStatusSimple.batteryStatus = text2;
								}
							}
						}
					}
				}
				if (controllerStatusSimple != null)
				{
					list.Add(controllerStatusSimple);
				}
				return list.ToArray();
			}

			private static readonly string[] splitList = new string[]
			{
				"\r\n",
				"\n"
			};

			private static Regex slotMatch = new Regex("Slot\\ (?<slotnumber>[0-9]+)", RegexOptions.Compiled);

			private string name = string.Empty;

			private int slotNumber = -1;

			private string status = string.Empty;

			private string cacheStatus = string.Empty;

			private string batteryStatus = string.Empty;
		}

		public class ModifyCommandResult
		{
			public bool Success
			{
				get
				{
					return this.success;
				}
			}

			public string ErrorMessage
			{
				get
				{
					return this.errorMessage;
				}
			}

			public ModifyCommandResult(string rawOutput)
			{
				if (string.IsNullOrWhiteSpace(rawOutput))
				{
					this.success = true;
					this.errorMessage = string.Empty;
					return;
				}
				this.success = false;
				this.errorMessage = rawOutput;
			}

			private readonly bool success;

			private readonly string errorMessage;
		}
	}
}
