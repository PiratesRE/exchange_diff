using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class ClientInformation
	{
		[DataMember(IsRequired = true)]
		public uint SessionId { get; private set; }

		[DataMember(IsRequired = true)]
		public string ClientProcessName { get; private set; }

		[DataMember(IsRequired = true)]
		public int ClientProcessId { get; private set; }

		[DataMember(IsRequired = true)]
		public string ClientMachineName { get; private set; }

		public void SetClientInformation()
		{
			this.ClientProcessId = ClientInformation.currentProcess.Id;
			this.ClientProcessName = ClientInformation.currentProcess.ProcessName;
			this.ClientMachineName = ClientInformation.machineName;
			this.SessionId = (uint)ClientInformation.random.Next();
		}

		private static readonly Process currentProcess = Process.GetCurrentProcess();

		private static readonly string machineName = Environment.MachineName;

		private static readonly Random random = new Random();
	}
}
