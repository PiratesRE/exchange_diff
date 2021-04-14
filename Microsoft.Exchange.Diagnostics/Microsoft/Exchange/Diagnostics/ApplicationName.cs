using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ApplicationName
	{
		private ApplicationName(string name, string uniqueId, int processId)
		{
			this.name = name;
			this.uniqueId = uniqueId;
			this.processId = processId;
		}

		public static ApplicationName Current
		{
			get
			{
				if (ApplicationName.current == null)
				{
					ApplicationName.current = ApplicationName.GetCurrentApplicationName();
				}
				return ApplicationName.current;
			}
		}

		public string UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int ProcessId
		{
			get
			{
				return this.processId;
			}
		}

		private static ApplicationName GetCurrentApplicationName()
		{
			ApplicationName result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(currentProcess.ProcessName, "w3wp") && currentProcess.StartInfo != null && currentProcess.StartInfo.EnvironmentVariables != null && currentProcess.StartInfo.EnvironmentVariables.ContainsKey("APP_POOL_ID"))
				{
					string text = currentProcess.StartInfo.EnvironmentVariables["APP_POOL_ID"];
					if (!string.IsNullOrEmpty(text))
					{
						return new ApplicationName(text, string.Concat(new object[]
						{
							"w3wp_",
							text,
							"_",
							currentProcess.Id
						}), currentProcess.Id);
					}
				}
				result = new ApplicationName(currentProcess.ProcessName, currentProcess.ProcessName + "_" + currentProcess.Id, currentProcess.Id);
			}
			return result;
		}

		private const string IISWorkerProcessName = "w3wp";

		private const string AppPoolId = "APP_POOL_ID";

		private static ApplicationName current;

		private readonly string uniqueId;

		private readonly string name;

		private readonly int processId;
	}
}
