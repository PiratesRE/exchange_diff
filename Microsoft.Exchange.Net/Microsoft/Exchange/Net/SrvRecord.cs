using System;

namespace Microsoft.Exchange.Net
{
	internal class SrvRecord
	{
		public SrvRecord(string name, string targetHost, int priority, int weight, int port)
		{
			this.name = name;
			this.targetHost = targetHost;
			this.priority = weight;
			this.weight = weight;
			this.port = port;
		}

		private SrvRecord()
		{
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string TargetHost
		{
			get
			{
				return this.targetHost;
			}
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
		}

		public int Weight
		{
			get
			{
				return this.weight;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		private string name;

		private string targetHost;

		private int priority;

		private int weight;

		private int port;
	}
}
