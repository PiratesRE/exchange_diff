using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbStateInfo
	{
		internal AmDbStateInfo(Guid databaseGuid)
		{
			this.IsEntryExist = false;
			this.DatabaseGuid = databaseGuid;
			this.ActiveServer = AmServerName.Empty;
			this.LastMountedServer = AmServerName.Empty;
			this.MountStatus = MountStatus.Dismounted;
			this.IsAdminDismounted = false;
			this.IsAutomaticActionsAllowed = false;
			this.LastMountedTime = DateTime.UtcNow;
			this.FailoverSequenceNumber = 0L;
		}

		internal AmServerName ActiveServer { get; set; }

		internal AmServerName LastMountedServer { get; set; }

		internal DateTime LastMountedTime { get; set; }

		internal MountStatus MountStatus { get; set; }

		internal bool IsAdminDismounted { get; set; }

		internal bool IsAutomaticActionsAllowed { get; set; }

		internal long FailoverSequenceNumber { get; set; }

		internal bool IsEntryExist { get; set; }

		internal Guid DatabaseGuid { get; set; }

		internal bool IsActiveServerValid
		{
			get
			{
				return !AmServerName.IsNullOrEmpty(this.ActiveServer);
			}
		}

		internal bool IsMountSucceededAtleastOnce
		{
			get
			{
				return !AmServerName.IsNullOrEmpty(this.LastMountedServer);
			}
		}

		internal bool IsMountAttemptedAtleastOnce
		{
			get
			{
				return !AmServerName.IsNullOrEmpty(this.ActiveServer);
			}
		}

		internal bool IsMounted
		{
			get
			{
				return this.MountStatus == MountStatus.Mounted;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(500);
			AmDbStateInfo.AppendNameValue(stringBuilder, "IsEntryExist", this.IsEntryExist.ToString());
			AmDbStateInfo.AppendNameValue(stringBuilder, AmDbStateInfo.PropertyNames.ActiveServer.ToString(), this.ActiveServer.NetbiosName);
			AmDbStateInfo.AppendNameValue(stringBuilder, AmDbStateInfo.PropertyNames.LastMountedServer.ToString(), this.LastMountedServer.NetbiosName);
			AmDbStateInfo.AppendNameValue(stringBuilder, AmDbStateInfo.PropertyNames.LastMountedTime.ToString(), this.LastMountedTime.ToString("s"));
			AmDbStateInfo.AppendNameValue(stringBuilder, AmDbStateInfo.PropertyNames.MountStatus.ToString(), this.MountStatus.ToString());
			AmDbStateInfo.AppendNameValue(stringBuilder, AmDbStateInfo.PropertyNames.IsAdminDismounted.ToString(), this.IsAdminDismounted.ToString());
			AmDbStateInfo.AppendNameValue(stringBuilder, AmDbStateInfo.PropertyNames.IsAutomaticActionsAllowed.ToString(), this.IsAutomaticActionsAllowed.ToString());
			AmDbStateInfo.AppendNameValue(stringBuilder, AmDbStateInfo.PropertyNames.FailoverSequenceNumber.ToString(), this.FailoverSequenceNumber.ToString());
			return stringBuilder.ToString();
		}

		internal static void AppendNameValue(StringBuilder sb, string propName, object value)
		{
			sb.Append(propName);
			sb.Append('?');
			sb.Append(value);
			sb.Append('*');
		}

		[Conditional("DEBUG")]
		internal static void AssertAreEqual(AmDbStateInfo stateA, AmDbStateInfo stateB)
		{
		}

		internal static AmDbStateInfo ReplaceProperty(AmDbStateInfo oldStateInfo, AmDbStateInfo.PropertyNames propName, string propValue)
		{
			bool isPropExist = false;
			string str = oldStateInfo.ToString();
			StringBuilder sb = new StringBuilder(500);
			AmDbStateInfo.ParseNameValuePairs(str, delegate(string name, string value)
			{
				if (AmDbStateInfo.IsMatching(name, propName))
				{
					if (!isPropExist)
					{
						AmDbStateInfo.AppendNameValue(sb, propName.ToString(), propValue);
						isPropExist = true;
						return;
					}
				}
				else
				{
					AmDbStateInfo.AppendNameValue(sb, name, value);
				}
			});
			if (!isPropExist)
			{
				AmDbStateInfo.AppendNameValue(sb, propName.ToString(), propValue);
			}
			return AmDbStateInfo.Parse(oldStateInfo.DatabaseGuid, sb.ToString());
		}

		internal static AmDbStateInfo Parse(Guid databaseGuid, string entryStr)
		{
			AmDbStateInfo stateInfo = new AmDbStateInfo(databaseGuid);
			stateInfo.IsEntryExist = true;
			AmDbStateInfo.ParseNameValuePairs(entryStr, delegate(string name, string value)
			{
				if (AmDbStateInfo.IsMatching(name, AmDbStateInfo.PropertyNames.ActiveServer))
				{
					stateInfo.ActiveServer = new AmServerName(SharedHelper.GetNodeNameFromFqdn(value));
					return;
				}
				if (AmDbStateInfo.IsMatching(name, AmDbStateInfo.PropertyNames.LastMountedServer))
				{
					stateInfo.LastMountedServer = new AmServerName(SharedHelper.GetNodeNameFromFqdn(value));
					return;
				}
				if (AmDbStateInfo.IsMatching(name, AmDbStateInfo.PropertyNames.LastMountedTime))
				{
					DateTime lastMountedTime;
					DateTime.TryParseExact(value, DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out lastMountedTime);
					stateInfo.LastMountedTime = lastMountedTime;
					return;
				}
				if (AmDbStateInfo.IsMatching(name, AmDbStateInfo.PropertyNames.MountStatus))
				{
					MountStatus mountStatus;
					EnumUtility.TryParse<MountStatus>(value, out mountStatus, MountStatus.Dismounted);
					stateInfo.MountStatus = mountStatus;
					return;
				}
				if (AmDbStateInfo.IsMatching(name, AmDbStateInfo.PropertyNames.IsAdminDismounted))
				{
					stateInfo.IsAdminDismounted = bool.Parse(value);
					return;
				}
				if (AmDbStateInfo.IsMatching(name, AmDbStateInfo.PropertyNames.IsAutomaticActionsAllowed))
				{
					stateInfo.IsAutomaticActionsAllowed = bool.Parse(value);
					return;
				}
				if (AmDbStateInfo.IsMatching(name, AmDbStateInfo.PropertyNames.FailoverSequenceNumber))
				{
					stateInfo.FailoverSequenceNumber = long.Parse(value);
				}
			});
			return stateInfo;
		}

		internal AmDbStateInfo Copy()
		{
			return new AmDbStateInfo(this.DatabaseGuid)
			{
				ActiveServer = new AmServerName(this.ActiveServer),
				LastMountedServer = new AmServerName(this.LastMountedServer),
				MountStatus = this.MountStatus,
				IsAdminDismounted = this.IsAdminDismounted,
				IsAutomaticActionsAllowed = this.IsAutomaticActionsAllowed,
				LastMountedTime = this.LastMountedTime,
				FailoverSequenceNumber = this.FailoverSequenceNumber
			};
		}

		internal void UpdateActiveServerAndIncrementFailoverSequenceNumber(AmServerName activeServer)
		{
			long ticks = DateTime.UtcNow.Ticks;
			bool flag = false;
			if (!this.IsEntryExist || !AmServerName.IsEqual(this.ActiveServer, activeServer))
			{
				flag = true;
			}
			this.ActiveServer = activeServer;
			if (flag)
			{
				if (!this.IsEntryExist || this.FailoverSequenceNumber == 0L)
				{
					this.FailoverSequenceNumber = ticks;
					return;
				}
				if (this.FailoverSequenceNumber < ticks)
				{
					this.FailoverSequenceNumber = ticks;
					return;
				}
				this.FailoverSequenceNumber += 1L;
				ReplayCrimsonEvents.DatabaseFailoverSequenceNumberNotBasedOnTime.LogPeriodic<Guid, long>(this.DatabaseGuid, TimeSpan.FromMinutes(30.0), this.DatabaseGuid, this.FailoverSequenceNumber);
			}
		}

		private static bool IsMatching(string key, AmDbStateInfo.PropertyNames propName)
		{
			return string.Equals(key, propName.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		private static int ParseNameValuePairs(string str, AmDbStateInfo.ProcessNameValuePair processNameValueDelegate)
		{
			int num = 0;
			if (!string.IsNullOrEmpty(str))
			{
				string[] array = str.Split(new char[]
				{
					'*'
				});
				if (array != null && array.Length > 0)
				{
					foreach (string text in array)
					{
						string text2 = string.Empty;
						string value = string.Empty;
						string[] array3 = text.Split(new char[]
						{
							'?'
						});
						if (array3 != null && array3.Length > 0)
						{
							text2 = array3[0].Trim();
							if (array3.Length > 1)
							{
								value = array3[1].Trim();
							}
							if (!string.IsNullOrEmpty(text2))
							{
								num++;
								processNameValueDelegate(text2, value);
							}
						}
					}
				}
			}
			return num;
		}

		private const char NameValueSeparatorChar = '?';

		private const char EntrySeparatorChar = '*';

		internal enum PropertyNames
		{
			None,
			ActiveServer,
			LastMountedServer,
			LastMountedTime,
			MountStatus,
			IsAdminDismounted,
			IsAutomaticActionsAllowed,
			FailoverSequenceNumber
		}

		private delegate void ProcessNameValuePair(string name, string value);
	}
}
