using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	public class DiagnosticInfo
	{
		protected DiagnosticInfo(string serverName)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentException("This class should be instatiated only via GetCachedInstance()");
			}
			this.serverName = serverName;
			this.Refresh();
		}

		public ExDateTime? LastDatabaseDiscoveryStartTime { get; private set; }

		public static DiagnosticInfo GetCachedInstance(string serverName)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				serverName = Environment.MachineName;
			}
			lock (DiagnosticInfo.diagInfoCache)
			{
				if (DiagnosticInfo.timeoutTimes.ContainsKey(serverName) && DiagnosticInfo.timeoutTimes[serverName] > ExDateTime.UtcNow)
				{
					return DiagnosticInfo.diagInfoCache[serverName];
				}
			}
			DiagnosticInfo diagnosticInfo = new DiagnosticInfo(serverName);
			lock (DiagnosticInfo.diagInfoCache)
			{
				DiagnosticInfo.timeoutTimes[serverName] = ExDateTime.UtcNow + DiagnosticInfoConstants.DiagnosticInfoCacheTimeout;
				DiagnosticInfo.diagInfoCache[serverName] = diagnosticInfo;
			}
			return diagnosticInfo;
		}

		public static DiagnosticInfo GetCachedInstance()
		{
			return DiagnosticInfo.GetCachedInstance(null);
		}

		public MdbHealthInfo GetHealthInfoPerMdb(Guid mdbGuid)
		{
			MdbHealthInfo result = null;
			this.healthInfoPerMdb.TryGetValue(mdbGuid, out result);
			return result;
		}

		public SlaDiagnosticInfo GetSLAInfoPerMdb(Guid mdbGuid)
		{
			SlaDiagnosticInfo result = null;
			this.slaInfoPerMdb.TryGetValue(mdbGuid, out result);
			return result;
		}

		public void Refresh()
		{
			this.Refresh(DiagnosticMode.Basic);
		}

		public void Refresh(DiagnosticMode mode)
		{
			string componentArgument = null;
			if (mode == DiagnosticMode.Basic)
			{
				componentArgument = string.Format("{0} {1}", "basic", "databasemanager dispatchmanager");
				this.preArgument = DiagnosticMode.Basic;
			}
			else if (mode == DiagnosticMode.Info)
			{
				if (this.preArgument == DiagnosticMode.Info)
				{
					return;
				}
				componentArgument = string.Format("{0} {1}", "info", "databasemanager dispatchmanager");
				this.preArgument = DiagnosticMode.Info;
			}
			string diagnosticInformation = ProcessAccessManager.ClientRunProcessCommand(this.serverName, Configurations.TransportSyncManagerProcessName, "syncmanager", componentArgument, false, true, null);
			this.Parse(diagnosticInformation);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MdbHealthInfo mdbHealthInfo in this.healthInfoPerMdb.Values)
			{
				stringBuilder.AppendLine(mdbHealthInfo.ToString());
			}
			return stringBuilder.ToString();
		}

		private void Parse(string diagnosticInformation)
		{
			this.healthInfoPerMdb = new Dictionary<Guid, MdbHealthInfo>(DiagnosticInfoConstants.AverageNumberOfMdbsPerServer);
			this.slaInfoPerMdb = new Dictionary<Guid, SlaDiagnosticInfo>(DiagnosticInfoConstants.AverageNumberOfMdbsPerServer);
			bool flag = false;
			bool flag2 = false;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(diagnosticInformation)))
				{
					using (XmlReader xmlReader = XmlReader.Create(memoryStream, new XmlReaderSettings
					{
						IgnoreComments = true
					}))
					{
						while (xmlReader.Read())
						{
							string name;
							if ((name = xmlReader.Name) != null)
							{
								if (!(name == "GlobalDatabaseHandler"))
								{
									if (name == "DispatchManager")
									{
										this.ParseTransportSyncSLAInfo(xmlReader);
										flag2 = true;
									}
								}
								else
								{
									this.ParseTransportSyncInfo(xmlReader);
									flag = true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("Unable to parse diagnostic information string, error {0}: {1}", ex.Message, diagnosticInformation), ex);
			}
			if (!flag || !flag2)
			{
				throw new InvalidOperationException(string.Format("Unable to locate expected elements in the diagnostic information string: {0}", diagnosticInformation));
			}
		}

		private void ParseTransportSyncInfo(XmlReader xmlReader)
		{
			int depth = xmlReader.Depth;
			do
			{
				if (xmlReader.Name == "Database" && xmlReader.IsStartElement("Database"))
				{
					MdbHealthInfo mdbHealthInfo = new MdbHealthInfo(xmlReader);
					if (mdbHealthInfo.MdbGuid != Guid.Empty)
					{
						this.healthInfoPerMdb[mdbHealthInfo.MdbGuid] = mdbHealthInfo;
					}
				}
				else if (xmlReader.Name == "LastDatabaseDiscoveryStartTime" && xmlReader.IsStartElement("LastDatabaseDiscoveryStartTime"))
				{
					this.LastDatabaseDiscoveryStartTime = new ExDateTime?(ExDateTime.Parse(xmlReader.ReadString()).ToUtc());
				}
				else
				{
					xmlReader.Read();
				}
			}
			while (xmlReader.Depth >= depth && xmlReader.Name != "GlobalDatabaseHandler");
		}

		private void ParseTransportSyncSLAInfo(XmlReader xmlReader)
		{
			int depth = xmlReader.Depth;
			do
			{
				if (xmlReader.Name == "DatabaseQueueManager" && xmlReader.IsStartElement("DatabaseQueueManager"))
				{
					SlaDiagnosticInfo slaDiagnosticInfo = new SlaDiagnosticInfo(xmlReader);
					if (slaDiagnosticInfo.MdbGuid != Guid.Empty)
					{
						this.slaInfoPerMdb[slaDiagnosticInfo.MdbGuid] = slaDiagnosticInfo;
					}
				}
				else
				{
					xmlReader.Read();
				}
			}
			while (xmlReader.Depth >= depth && xmlReader.Name != "DispatchManager");
		}

		private readonly string serverName;

		private static Dictionary<string, DiagnosticInfo> diagInfoCache = new Dictionary<string, DiagnosticInfo>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, ExDateTime> timeoutTimes = new Dictionary<string, ExDateTime>(StringComparer.OrdinalIgnoreCase);

		private DiagnosticMode preArgument;

		private Dictionary<Guid, MdbHealthInfo> healthInfoPerMdb;

		private Dictionary<Guid, SlaDiagnosticInfo> slaInfoPerMdb;
	}
}
