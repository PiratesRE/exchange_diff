using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public sealed class SyncCallerContext
	{
		private SyncCallerContext()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.context = string.Join("-", new string[]
				{
					Environment.MachineName,
					currentProcess.ProcessName,
					FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
					DateTime.UtcNow.ToString("u"),
					Guid.NewGuid().ToString("N")
				});
			}
		}

		[DataMember]
		public string PartnerName { get; private set; }

		public static SyncCallerContext Create(string partnerName)
		{
			if (string.IsNullOrWhiteSpace(partnerName))
			{
				throw new ArgumentException("partnerName");
			}
			return new SyncCallerContext
			{
				PartnerName = partnerName
			};
		}

		public override string ToString()
		{
			return string.Join("-", new string[]
			{
				this.PartnerName,
				this.context
			});
		}

		private const string ContextSeparator = "-";

		[DataMember]
		private readonly string context;
	}
}
