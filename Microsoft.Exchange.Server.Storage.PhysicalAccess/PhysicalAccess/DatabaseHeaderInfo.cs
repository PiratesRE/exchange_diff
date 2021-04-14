using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class DatabaseHeaderInfo
	{
		public DatabaseHeaderInfo(byte[] serializedLgposLastAttached, DateTime lastRepairedTime, int repairCountSinceLastDefrag, int repairCountBeforeLastDefrag)
		{
			this.serializedLgposLastAttached = serializedLgposLastAttached;
			this.lastRepairedTime = lastRepairedTime;
			this.repairCountSinceLastDefrag = repairCountSinceLastDefrag;
			this.repairCountBeforeLastDefrag = repairCountBeforeLastDefrag;
		}

		public byte[] SerializedLgposLastAttached
		{
			get
			{
				return this.serializedLgposLastAttached;
			}
		}

		public DateTime LastRepairedTime
		{
			get
			{
				return this.lastRepairedTime;
			}
		}

		public int RepairCountSinceLastDefrag
		{
			get
			{
				return this.repairCountSinceLastDefrag;
			}
		}

		public int RepairCountBeforeLastDefrag
		{
			get
			{
				return this.repairCountBeforeLastDefrag;
			}
		}

		public bool DatabaseRepaired
		{
			get
			{
				return this.repairCountSinceLastDefrag > 0 || this.repairCountBeforeLastDefrag > 0;
			}
		}

		private readonly byte[] serializedLgposLastAttached;

		private readonly DateTime lastRepairedTime;

		private readonly int repairCountSinceLastDefrag;

		private readonly int repairCountBeforeLastDefrag;
	}
}
