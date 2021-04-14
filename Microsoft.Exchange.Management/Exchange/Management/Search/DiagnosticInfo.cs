using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Data.ApplicationLogic;

namespace Microsoft.Exchange.Management.Search
{
	public class DiagnosticInfo
	{
		public DiagnosticInfo(string serverName)
		{
			this.serverName = serverName;
			this.Refresh();
		}

		internal DiagnosticInfo()
		{
		}

		public static int RetryCount
		{
			get
			{
				return DiagnosticInfo.retryCount;
			}
			set
			{
				DiagnosticInfo.retryCount = value;
			}
		}

		public static DiagnosticInfo GetCachedInstance(string serverName)
		{
			if (serverName == null)
			{
				serverName = Environment.MachineName;
			}
			lock (DiagnosticInfo.diagInfoCache)
			{
				if (DiagnosticInfo.timeoutTimes.ContainsKey(serverName) && DiagnosticInfo.timeoutTimes[serverName] > DateTime.UtcNow)
				{
					return DiagnosticInfo.diagInfoCache[serverName];
				}
			}
			DiagnosticInfo diagnosticInfo = new DiagnosticInfo(serverName);
			lock (DiagnosticInfo.diagInfoCache)
			{
				DiagnosticInfo.timeoutTimes[serverName] = DateTime.UtcNow + DiagnosticInfo.DiagnosticInfoCacheTimeout;
				DiagnosticInfo.diagInfoCache[serverName] = diagnosticInfo;
			}
			return diagnosticInfo;
		}

		public static DiagnosticInfo GetCachedInstance()
		{
			return DiagnosticInfo.GetCachedInstance(null);
		}

		public bool ProcessLoadedInMemory { get; private set; }

		public int? ProcessId { get; private set; }

		public int? ThreadCount { get; private set; }

		public string ServerName { get; private set; }

		public TimeSpan? ProcessUpTime { get; private set; }

		public DateTime? Timestamp { get; private set; }

		public string DiagnosticInfoXml { get; private set; }

		public DateTime? RecentGracefulDegradationExecutionTime { get; private set; }

		public DiagnosticInfo.FeedingControllerDiagnosticInfo GetFeedingControllerDiagnosticInfo(Guid mdbGuid)
		{
			DiagnosticInfo.FeedingControllerDiagnosticInfo result = null;
			if (this.ProcessLoadedInMemory)
			{
				this.perMdbFeedingController.TryGetValue(mdbGuid, out result);
			}
			return result;
		}

		public void Refresh()
		{
			for (int i = 1; i <= DiagnosticInfo.RetryCount + 1; i++)
			{
				string text = ProcessAccessManager.ClientRunProcessCommand(this.serverName, "Microsoft.Exchange.Search.Service", null, null, false, true, null);
				this.Parse(text);
				if (this.ProcessId != null || DiagnosticInfo.RetryCount == 0)
				{
					this.DiagnosticInfoXml = text;
					return;
				}
				if (i == DiagnosticInfo.RetryCount + 1)
				{
					throw new InvalidOperationException(string.Format("Process ID is not found from diagnostic info XML:\n{0}", text));
				}
				Thread.Sleep(TimeSpan.FromSeconds(3.0));
			}
		}

		public long LowWatermark(Guid mdbGuid)
		{
			long result = 0L;
			if (this.ProcessLoadedInMemory)
			{
				DiagnosticInfo.FeedingControllerDiagnosticInfo feedingControllerDiagnosticInfo = null;
				if (this.perMdbFeedingController.TryGetValue(mdbGuid, out feedingControllerDiagnosticInfo))
				{
					result = feedingControllerDiagnosticInfo.NotificationFeederLowWatermark;
				}
			}
			return result;
		}

		public long HighWatermark(Guid mdbGuid)
		{
			long result = 0L;
			if (this.ProcessLoadedInMemory)
			{
				DiagnosticInfo.FeedingControllerDiagnosticInfo feedingControllerDiagnosticInfo = null;
				if (this.perMdbFeedingController.TryGetValue(mdbGuid, out feedingControllerDiagnosticInfo))
				{
					result = feedingControllerDiagnosticInfo.NotificationFeederHighWatermark;
				}
			}
			return result;
		}

		public long LastEvent(Guid mdbGuid)
		{
			long result = 0L;
			if (this.ProcessLoadedInMemory)
			{
				DiagnosticInfo.FeedingControllerDiagnosticInfo feedingControllerDiagnosticInfo = null;
				if (this.perMdbFeedingController.TryGetValue(mdbGuid, out feedingControllerDiagnosticInfo))
				{
					result = feedingControllerDiagnosticInfo.NotificationFeederLastEvent;
				}
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.ProcessLoadedInMemory)
			{
				stringBuilder.AppendFormat("{0}:{1}; ", "ProcessId", this.ProcessId);
				stringBuilder.AppendFormat("{0}:{1}; ", "ThreadCount", this.ThreadCount);
				stringBuilder.AppendFormat("{0}:{1}; ", "ServerName", this.ServerName);
				stringBuilder.AppendFormat("{0}:{1}; ", "ProcessUpTime", this.ProcessUpTime);
				stringBuilder.AppendFormat("{0}:{1}; ", "Timestamp", this.Timestamp);
				stringBuilder.AppendFormat("{0}:{1}; ", "RecentGracefulDegradationExecutionTime", this.RecentGracefulDegradationExecutionTime);
				stringBuilder.AppendLine();
				using (Dictionary<Guid, DiagnosticInfo.FeedingControllerDiagnosticInfo>.ValueCollection.Enumerator enumerator = this.perMdbFeedingController.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DiagnosticInfo.FeedingControllerDiagnosticInfo feedingControllerDiagnosticInfo = enumerator.Current;
						stringBuilder.AppendLine(feedingControllerDiagnosticInfo.ToString());
					}
					goto IL_108;
				}
			}
			stringBuilder.AppendLine("ProcessLoadedInMemory: False");
			IL_108:
			return stringBuilder.ToString();
		}

		internal void Parse(string diagnosticInformation)
		{
			this.perMdbFeedingController = new Dictionary<Guid, DiagnosticInfo.FeedingControllerDiagnosticInfo>(50);
			try
			{
				using (TextReader textReader = new StringReader(diagnosticInformation))
				{
					using (XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings
					{
						IgnoreComments = true
					}))
					{
						while (xmlReader.Read())
						{
							string name;
							if ((name = xmlReader.Name) != null)
							{
								if (!(name == "ProcessInfo"))
								{
									if (!(name == "SearchFeedingController"))
									{
										if (name == "RecentGracefulDegradationExecutionTime")
										{
											this.RecentGracefulDegradationExecutionTime = new DateTime?(DateTime.Parse(xmlReader.ReadString()));
										}
									}
									else
									{
										this.ParseSearchFeedingControllerInfo(xmlReader);
									}
								}
								else
								{
									this.ProcessLoadedInMemory = true;
									this.ParseProcessInfo(xmlReader);
								}
							}
						}
					}
				}
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(diagnosticInformation, innerException);
			}
		}

		private void ParseProcessInfo(XmlReader xmlReader)
		{
			while (xmlReader.Read())
			{
				if (xmlReader.Name == "ProcessInfo")
				{
					return;
				}
				if (!string.IsNullOrWhiteSpace(xmlReader.Name))
				{
					string name = xmlReader.Name;
					string s = xmlReader.ReadString();
					string a;
					if ((a = name) != null)
					{
						if (!(a == "id"))
						{
							if (!(a == "serverName"))
							{
								if (!(a == "threadCount"))
								{
									if (!(a == "lifetime"))
									{
										if (a == "currentTime")
										{
											this.Timestamp = new DateTime?(DateTime.Parse(s).ToUniversalTime());
										}
									}
									else
									{
										this.ProcessUpTime = new TimeSpan?(TimeSpan.Parse(s));
									}
								}
								else
								{
									this.ThreadCount = new int?(int.Parse(s));
								}
							}
							else
							{
								this.ServerName = s;
							}
						}
						else
						{
							this.ProcessId = new int?(int.Parse(s));
						}
					}
				}
			}
		}

		private void ParseSearchFeedingControllerInfo(XmlReader xmlReader)
		{
			int depth = xmlReader.Depth;
			do
			{
				if (xmlReader.Name == "SearchFeedingController")
				{
					DiagnosticInfo.FeedingControllerDiagnosticInfo feedingControllerDiagnosticInfo = new DiagnosticInfo.FeedingControllerDiagnosticInfo(xmlReader);
					if (feedingControllerDiagnosticInfo.MdbGuid != Guid.Empty)
					{
						this.perMdbFeedingController[feedingControllerDiagnosticInfo.MdbGuid] = feedingControllerDiagnosticInfo;
					}
				}
				else
				{
					xmlReader.Read();
				}
			}
			while (xmlReader.Depth >= depth);
		}

		private const string ProcessName = "Microsoft.Exchange.Search.Service";

		private const int InitialPerMdbFeedingControllerDictionarySize = 50;

		private const int RetryIntervalSeconds = 3;

		private static int retryCount = 2;

		private static readonly TimeSpan DiagnosticInfoCacheTimeout = TimeSpan.FromMinutes(1.0);

		private readonly string serverName;

		private Dictionary<Guid, DiagnosticInfo.FeedingControllerDiagnosticInfo> perMdbFeedingController;

		private static Dictionary<string, DiagnosticInfo> diagInfoCache = new Dictionary<string, DiagnosticInfo>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, DateTime> timeoutTimes = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

		public class FeedingControllerDiagnosticInfo
		{
			internal FeedingControllerDiagnosticInfo(XmlReader xmlReader)
			{
				if (xmlReader.Name != "SearchFeedingController")
				{
					throw new ArgumentException("XmlReader Invalid Xml Node");
				}
				string name = xmlReader.Name;
				while (!(xmlReader.Name == name) || xmlReader.Read())
				{
					name = xmlReader.Name;
					string key;
					switch (key = name)
					{
					case "MdbGuid":
						this.MdbGuid = new Guid(xmlReader.ReadString());
						break;
					case "MdbName":
						this.MdbName = xmlReader.ReadString();
						break;
					case "OwningServer":
						this.OwningServer = xmlReader.ReadString();
						break;
					case "HighWatermark":
						this.NotificationFeederHighWatermark = xmlReader.ReadElementContentAsLong();
						break;
					case "LowWatermark":
						this.NotificationFeederLowWatermark = xmlReader.ReadElementContentAsLong();
						break;
					case "LastEvent":
						this.NotificationFeederLastEvent = xmlReader.ReadElementContentAsLong();
						break;
					case "Error":
						this.Error = xmlReader.ReadString();
						break;
					case "NotificationLastPollTime":
						this.NotificationFeederLastPollTime = xmlReader.ReadElementContentAsDateTime().ToUniversalTime();
						break;
					case "AgeOfLastNotificationProcessed":
						this.AgeOfLastNotificationProcessed = xmlReader.ReadElementContentAsInt();
						break;
					case "RetryItems":
						this.RetryItems = xmlReader.ReadElementContentAsInt();
						break;
					case "FailedItems":
						this.FailedItems = xmlReader.ReadElementContentAsInt();
						break;
					case "MailboxesLeftToCrawl":
						this.MailboxesLeftToCrawl = xmlReader.ReadElementContentAsInt();
						break;
					}
					if (name == "SearchFeedingController")
					{
						xmlReader.Read();
						return;
					}
				}
			}

			public Guid MdbGuid { get; private set; }

			public string MdbName { get; private set; }

			public string OwningServer { get; private set; }

			public long NotificationFeederLowWatermark { get; private set; }

			public long NotificationFeederHighWatermark { get; private set; }

			public long NotificationFeederLastEvent { get; private set; }

			public DateTime NotificationFeederLastPollTime { get; private set; }

			public string Error { get; private set; }

			public int AgeOfLastNotificationProcessed { get; private set; }

			public int RetryItems { get; private set; }

			public int FailedItems { get; private set; }

			public int MailboxesLeftToCrawl { get; private set; }

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}:{1}; ", "MDBGuid", this.MdbGuid);
				stringBuilder.AppendFormat("{0}:{1}; ", "MdbName", this.MdbName);
				stringBuilder.AppendFormat("{0}:{1}; ", "OwningServer", this.OwningServer);
				stringBuilder.AppendFormat("{0}:{1}; ", "LowWatermark", this.NotificationFeederLowWatermark);
				stringBuilder.AppendFormat("{0}:{1}; ", "HighWatermark", this.NotificationFeederHighWatermark);
				stringBuilder.AppendFormat("{0}:{1}; ", "LastEvent", this.NotificationFeederLastEvent);
				stringBuilder.AppendFormat("{0}:{1}; ", "NotificationLastPollTime", this.NotificationFeederLastPollTime);
				stringBuilder.AppendFormat("{0}:{1}; ", "AgeOfLastNotificationProcessed", this.AgeOfLastNotificationProcessed);
				stringBuilder.AppendFormat("{0}:{1}; ", "RetryItems", this.RetryItems);
				stringBuilder.AppendFormat("{0}:{1}; ", "FailedItems", this.FailedItems);
				stringBuilder.AppendFormat("{0}:{1}; ", "MailboxesLeftToCrawl", this.MailboxesLeftToCrawl);
				stringBuilder.AppendFormat("{0}:{1}; ", "Error", this.Error);
				return stringBuilder.ToString();
			}
		}
	}
}
