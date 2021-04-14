using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.TransportLogSearchTasks
{
	internal sealed class LatencyComponent
	{
		public LatencyComponent(string serverFqdn, string code, LocalizedString name, ushort latency, int sequenceNumber)
		{
			this.serverFqdn = serverFqdn;
			this.code = code;
			this.name = name;
			this.latency = latency;
			this.sequenceNumber = sequenceNumber;
		}

		public string ServerFqdn
		{
			get
			{
				return this.serverFqdn;
			}
		}

		public string Code
		{
			get
			{
				return this.code;
			}
		}

		public LocalizedString Name
		{
			get
			{
				return this.name;
			}
		}

		public int Latency
		{
			get
			{
				return (int)this.latency;
			}
		}

		public int SequenceNumber
		{
			get
			{
				return this.sequenceNumber;
			}
		}

		private readonly string serverFqdn;

		private readonly string code;

		private readonly LocalizedString name;

		private readonly ushort latency;

		private readonly int sequenceNumber;
	}
}
