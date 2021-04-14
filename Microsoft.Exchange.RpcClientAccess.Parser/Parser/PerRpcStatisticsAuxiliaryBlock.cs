using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class PerRpcStatisticsAuxiliaryBlock : AuxiliaryBlock
	{
		private PerRpcStatisticsAuxiliaryBlock(byte blockVersion, uint millisecondsInServer, uint millisecondsInCPU, uint pagesRead, uint pagesPreRead, uint logRecords, uint bytesLogRecord, ulong ldapReads, ulong ldapSearches, uint averageDbLatency, uint averageServerLatency, uint currentThreads, uint totalDbOperations, uint currentDbThreads, uint currentSCTThreads, uint currentSCTSessions, uint processId, uint dataProtectionHealth, uint dataAvailabilityHealth, uint currentCpuUsage) : base(blockVersion, AuxiliaryBlockTypes.PerRpcStatistics)
		{
			this.millisecondsInServer = millisecondsInServer;
			this.millisecondsInCPU = millisecondsInCPU;
			this.pagesRead = pagesRead;
			this.pagesPreRead = pagesPreRead;
			this.logRecords = logRecords;
			this.bytesLogRecord = bytesLogRecord;
			this.ldapReads = ldapReads;
			this.ldapSearches = ldapSearches;
			this.averageDbLatency = averageDbLatency;
			this.averageServerLatency = averageServerLatency;
			this.currentThreads = currentThreads;
			this.totalDbOperations = totalDbOperations;
			this.currentDbThreads = currentDbThreads;
			this.currentSCTThreads = currentSCTThreads;
			this.currentSCTSessions = currentSCTSessions;
			this.processId = processId;
			this.dataProtectionHealth = dataProtectionHealth;
			this.dataAvailabilityHealth = dataAvailabilityHealth;
			this.currentCpuUsage = currentCpuUsage;
		}

		internal PerRpcStatisticsAuxiliaryBlock(uint millisecondsInServer, uint millisecondsInCPU, uint pagesRead, uint pagesPreRead, uint logRecords, uint bytesLogRecord, ulong ldapReads, ulong ldapSearches, uint averageDbLatency, uint averageServerLatency, uint currentThreads) : this(1, millisecondsInServer, millisecondsInCPU, pagesRead, pagesPreRead, logRecords, bytesLogRecord, ldapReads, ldapSearches, averageDbLatency, averageServerLatency, currentThreads, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U)
		{
		}

		internal PerRpcStatisticsAuxiliaryBlock(uint millisecondsInServer, uint millisecondsInCPU, uint pagesRead, uint pagesPreRead, uint logRecords, uint bytesLogRecord, ulong ldapReads, ulong ldapSearches, uint averageDbLatency, uint averageServerLatency, uint currentThreads, uint totalDbOperations) : this(2, millisecondsInServer, millisecondsInCPU, pagesRead, pagesPreRead, logRecords, bytesLogRecord, ldapReads, ldapSearches, averageDbLatency, averageServerLatency, currentThreads, totalDbOperations, 0U, 0U, 0U, 0U, 0U, 0U, 0U)
		{
		}

		internal PerRpcStatisticsAuxiliaryBlock(uint millisecondsInServer, uint millisecondsInCPU, uint pagesRead, uint pagesPreRead, uint logRecords, uint bytesLogRecord, ulong ldapReads, ulong ldapSearches, uint averageDbLatency, uint averageServerLatency, uint currentThreads, uint totalDbOperations, uint currentDbThreads, uint currentSCTThreads, uint currentSCTSessions, uint processId) : this(3, millisecondsInServer, millisecondsInCPU, pagesRead, pagesPreRead, logRecords, bytesLogRecord, ldapReads, ldapSearches, averageDbLatency, averageServerLatency, currentThreads, totalDbOperations, currentDbThreads, currentSCTThreads, currentSCTSessions, processId, 0U, 0U, 0U)
		{
		}

		internal PerRpcStatisticsAuxiliaryBlock(uint millisecondsInServer, uint millisecondsInCPU, uint pagesRead, uint pagesPreRead, uint logRecords, uint bytesLogRecord, ulong ldapReads, ulong ldapSearches, uint averageDbLatency, uint averageServerLatency, uint currentThreads, uint totalDbOperations, uint currentDbThreads, uint currentSCTThreads, uint currentSCTSessions, uint processId, uint dataProtectionHealth, uint dataAvailabilityHealth) : this(4, millisecondsInServer, millisecondsInCPU, pagesRead, pagesPreRead, logRecords, bytesLogRecord, ldapReads, ldapSearches, averageDbLatency, averageServerLatency, currentThreads, totalDbOperations, currentDbThreads, currentSCTThreads, currentSCTSessions, processId, dataProtectionHealth, dataAvailabilityHealth, 0U)
		{
		}

		internal PerRpcStatisticsAuxiliaryBlock(uint millisecondsInServer, uint millisecondsInCPU, uint pagesRead, uint pagesPreRead, uint logRecords, uint bytesLogRecord, ulong ldapReads, ulong ldapSearches, uint averageDbLatency, uint averageServerLatency, uint currentThreads, uint totalDbOperations, uint currentDbThreads, uint currentSCTThreads, uint currentSCTSessions, uint processId, uint dataProtectionHealth, uint dataAvailabilityHealth, uint currentCpuUsage) : this(5, millisecondsInServer, millisecondsInCPU, pagesRead, pagesPreRead, logRecords, bytesLogRecord, ldapReads, ldapSearches, averageDbLatency, averageServerLatency, currentThreads, totalDbOperations, currentDbThreads, currentSCTThreads, currentSCTSessions, processId, dataProtectionHealth, dataAvailabilityHealth, currentCpuUsage)
		{
		}

		internal PerRpcStatisticsAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.millisecondsInServer = reader.ReadUInt32();
			this.millisecondsInCPU = reader.ReadUInt32();
			this.pagesRead = reader.ReadUInt32();
			this.pagesPreRead = reader.ReadUInt32();
			this.logRecords = reader.ReadUInt32();
			this.bytesLogRecord = reader.ReadUInt32();
			this.ldapReads = reader.ReadUInt64();
			this.ldapSearches = reader.ReadUInt64();
			this.averageDbLatency = reader.ReadUInt32();
			this.averageServerLatency = reader.ReadUInt32();
			this.currentThreads = reader.ReadUInt32();
			if (base.Version >= 2)
			{
				this.totalDbOperations = reader.ReadUInt32();
			}
			if (base.Version >= 3)
			{
				this.currentDbThreads = reader.ReadUInt32();
				this.currentSCTThreads = reader.ReadUInt32();
				this.currentSCTSessions = reader.ReadUInt32();
				this.processId = reader.ReadUInt32();
			}
			if (base.Version >= 4)
			{
				this.dataProtectionHealth = reader.ReadUInt32();
				this.dataAvailabilityHealth = reader.ReadUInt32();
			}
			if (base.Version >= 5)
			{
				this.currentCpuUsage = reader.ReadUInt32();
				reader.ReadUInt32();
			}
		}

		public uint MillisecondsInServer
		{
			get
			{
				return this.millisecondsInServer;
			}
		}

		public uint MillisecondsInCPU
		{
			get
			{
				return this.millisecondsInCPU;
			}
		}

		public uint PagesRead
		{
			get
			{
				return this.pagesRead;
			}
		}

		public uint PagesPreRead
		{
			get
			{
				return this.pagesPreRead;
			}
		}

		public uint LogRecords
		{
			get
			{
				return this.logRecords;
			}
		}

		public uint BytesLogRecord
		{
			get
			{
				return this.bytesLogRecord;
			}
		}

		public ulong LdapReads
		{
			get
			{
				return this.ldapReads;
			}
		}

		public ulong LdapSearches
		{
			get
			{
				return this.ldapSearches;
			}
		}

		public uint AverageDbLatency
		{
			get
			{
				return this.averageDbLatency;
			}
		}

		public uint AverageServerLatency
		{
			get
			{
				return this.averageServerLatency;
			}
		}

		public uint CurrentThreads
		{
			get
			{
				return this.currentThreads;
			}
		}

		public uint TotalDbOperations
		{
			get
			{
				return this.totalDbOperations;
			}
		}

		public uint CurrentDbThreads
		{
			get
			{
				return this.currentDbThreads;
			}
		}

		public uint CurrentSCTThreads
		{
			get
			{
				return this.currentSCTThreads;
			}
		}

		public uint CurrentSCTSessions
		{
			get
			{
				return this.currentSCTSessions;
			}
		}

		public uint ProcessId
		{
			get
			{
				return this.processId;
			}
		}

		public uint DataProtectionHealth
		{
			get
			{
				return this.dataProtectionHealth;
			}
		}

		public uint DataAvailabilityHealth
		{
			get
			{
				return this.dataAvailabilityHealth;
			}
		}

		public uint CurrentCpuUsage
		{
			get
			{
				return this.currentCpuUsage;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.millisecondsInServer);
			writer.WriteUInt32(this.millisecondsInCPU);
			writer.WriteUInt32(this.pagesRead);
			writer.WriteUInt32(this.pagesPreRead);
			writer.WriteUInt32(this.logRecords);
			writer.WriteUInt32(this.bytesLogRecord);
			writer.WriteUInt64(this.ldapReads);
			writer.WriteUInt64(this.ldapSearches);
			writer.WriteUInt32(this.averageDbLatency);
			writer.WriteUInt32(this.averageServerLatency);
			writer.WriteUInt32(this.currentThreads);
			if (base.Version >= 2)
			{
				writer.WriteUInt32(this.totalDbOperations);
			}
			if (base.Version >= 3)
			{
				writer.WriteUInt32(this.currentDbThreads);
				writer.WriteUInt32(this.currentSCTThreads);
				writer.WriteUInt32(this.currentSCTSessions);
				writer.WriteUInt32(this.processId);
			}
			if (base.Version >= 4)
			{
				writer.WriteUInt32(this.dataProtectionHealth);
				writer.WriteUInt32(this.dataAvailabilityHealth);
			}
			if (base.Version >= 5)
			{
				writer.WriteUInt32(this.currentCpuUsage);
				writer.WriteUInt32(0U);
			}
		}

		private readonly uint millisecondsInServer;

		private readonly uint millisecondsInCPU;

		private readonly uint pagesRead;

		private readonly uint pagesPreRead;

		private readonly uint logRecords;

		private readonly uint bytesLogRecord;

		private readonly ulong ldapReads;

		private readonly ulong ldapSearches;

		private readonly uint averageDbLatency;

		private readonly uint averageServerLatency;

		private readonly uint currentThreads;

		private readonly uint totalDbOperations;

		private readonly uint currentDbThreads;

		private readonly uint currentSCTThreads;

		private readonly uint currentSCTSessions;

		private readonly uint processId;

		private readonly uint dataProtectionHealth;

		private readonly uint dataAvailabilityHealth;

		private readonly uint currentCpuUsage;
	}
}
