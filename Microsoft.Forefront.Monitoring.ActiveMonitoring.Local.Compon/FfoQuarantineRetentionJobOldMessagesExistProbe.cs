using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class FfoQuarantineRetentionJobOldMessagesExistProbe : ProbeWorkItem
	{
		public FfoQuarantineRetentionJobOldMessagesExistProbe()
		{
			string path;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\FfoBackground\\Setup"))
			{
				path = (string)registryKey.GetValue("MsiInstallPath");
			}
			string exePath = Path.Combine(path, "Microsoft.Exchange.Hygiene.QuarantineManager.exe");
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(exePath);
			this.quarantineFileShare = configuration.AppSettings.Settings["QuarantineFileShare"].Value;
			this.quarantineDirectoryPrefix = configuration.AppSettings.Settings["QuarantineDirectoryPrefix"].Value;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			ProbeResult result = base.Result;
			result.ExecutionContext += "FfoQuarantineRetentionJobOldMessagesExistProbe started. ";
			if (this.QuarantineHostNotFound())
			{
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += "Not running in a Quarantine DC.";
			}
			else
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				string text = base.Definition.Attributes["DaysOld"];
				ProbeResult result3 = base.Result;
				result3.ExecutionContext += string.Format("Checking for files older than {0} days from {1}.", text, utcNow.ToString("d"));
				int num;
				if (!int.TryParse(text, out num))
				{
					ProbeResult result4 = base.Result;
					result4.ExecutionContext += string.Format("ERROR: DaysOld attribute is invalid.", new object[0]);
					throw new ArgumentException("Not an integer.", "DaysOld");
				}
				int num2 = FfoQuarantineRetentionJobOldMessagesExistProbe.DaysSinceY2K(utcNow);
				int num3 = num2 - num;
				bool flag = false;
				DirectoryInfo directoryInfo = new DirectoryInfo(this.quarantineFileShare);
				try
				{
					foreach (DirectoryInfo directoryInfo2 in directoryInfo.EnumerateDirectories(this.quarantineDirectoryPrefix + "-*"))
					{
						string[] array = directoryInfo2.Name.Split(new char[]
						{
							'-'
						}, 3);
						int num4 = Convert.ToInt32(array[1]);
						if (num4 < num3)
						{
							flag = true;
							ProbeResult result5 = base.Result;
							result5.ExecutionContext += string.Format("ERROR: Quarantine has Old Messages in {0}.", directoryInfo2.Name);
						}
					}
					if (flag)
					{
						throw new Exception("ERROR: Quarantine has old messages.");
					}
				}
				catch (DirectoryNotFoundException)
				{
				}
			}
			ProbeResult result6 = base.Result;
			result6.ExecutionContext += "FfoQuarantineRetentionJobOldMessagesExistProbe finished success.";
		}

		private static int DaysSinceY2K(ExDateTime date)
		{
			return date.ToUtc().Subtract(new ExDateTime(ExTimeZone.UtcTimeZone, 2000, 1, 1, 0, 0, 0)).Days;
		}

		private bool QuarantineHostNotFound()
		{
			Uri uri = new Uri(this.quarantineFileShare);
			if (uri.IsUnc)
			{
				try
				{
					Dns.GetHostEntry(uri.Host);
				}
				catch (SocketException ex)
				{
					if (ex.SocketErrorCode == SocketError.HostNotFound || ex.SocketErrorCode == SocketError.NoData)
					{
						return true;
					}
					throw;
				}
				return false;
			}
			return false;
		}

		private readonly string quarantineFileShare;

		private readonly string quarantineDirectoryPrefix;
	}
}
