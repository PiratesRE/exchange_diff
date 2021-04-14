using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class MeetingOrganizerInfo
	{
		internal static Func<GlobalObjectId, MeetingOrganizerEntry> OnCacheMissForTest { get; set; }

		public ExDateTime LastCleanTime { get; private set; }

		public bool IsDirty { get; private set; }

		public int Count
		{
			get
			{
				int count;
				lock (this.instanceLock)
				{
					count = this.map.Count;
				}
				return count;
			}
		}

		public MeetingOrganizerInfo()
		{
			this.LastCleanTime = (ExDateTime)TimeProvider.UtcNow;
		}

		public void Add(CalendarItemBase calendarItem)
		{
			GlobalObjectId globalObjectId = null;
			if (calendarItem == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] calendarItem is null.");
				return;
			}
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] Process CalendarItem and add an entry to Cache");
			string text = string.Empty;
			try
			{
				text = calendarItem.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			}
			catch (NullReferenceException)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] nullReference Exception when reading PrimarySmtpAddress");
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] PrimarySmtpAddress:{0}", text);
			try
			{
				globalObjectId = this.MakeCleanId(calendarItem.GlobalObjectId);
			}
			catch (CorruptDataException arg)
			{
				AirSyncDiagnostics.TraceDebug<string, string, CorruptDataException>(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] calendarItem does NOT have a global object id.  Subject: '{0}', Mailbox: '{1}', Exception: '{2}'", calendarItem.Subject, text, arg);
				return;
			}
			catch (NotInBagPropertyErrorException arg2)
			{
				AirSyncDiagnostics.TraceDebug<string, string, NotInBagPropertyErrorException>(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] calendarItem does NOT have a global object id.  Subject: '{0}', Mailbox: '{1}', Exception: '{2}'", calendarItem.Subject, text, arg2);
				return;
			}
			if (globalObjectId != null && globalObjectId.Uid != null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] Found CleanId & Uid:{0}", globalObjectId.Uid);
				MeetingOrganizerEntry meetingOrganizerEntry = null;
				bool flag = false;
				lock (this.instanceLock)
				{
					this.map.TryGetValue(globalObjectId.Uid, out meetingOrganizerEntry);
				}
				if (meetingOrganizerEntry == null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] Entry not found in Dictionary");
					meetingOrganizerEntry = ((MeetingOrganizerInfo.OnCacheMissForTest != null) ? MeetingOrganizerInfo.OnCacheMissForTest(globalObjectId) : this.BuildFromCalendarItem(globalObjectId, calendarItem));
					if (meetingOrganizerEntry != null)
					{
						if (meetingOrganizerEntry.CleanGlobalObjectId == null || meetingOrganizerEntry.CleanGlobalObjectId.Uid == null)
						{
							Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, "Add_NullUID");
						}
						else if (meetingOrganizerEntry.IsOrganizer != null)
						{
							this.Add(meetingOrganizerEntry);
						}
					}
				}
				else
				{
					flag = true;
				}
				if (Command.CurrentCommand != null)
				{
					Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.MeetingOrganizerLookup, flag.ToString());
				}
				return;
			}
			AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "[MeetingOrganizerInfo.Add] cleanId is missing Uid. Tell someone to go find it.  Subject: '{0}', Mailbox: '{1}'", calendarItem.Subject, text);
		}

		public MeetingOrganizerEntry GetEntry(GlobalObjectId globalObjectId)
		{
			if (globalObjectId == null)
			{
				return null;
			}
			GlobalObjectId globalObjectId2 = this.MakeCleanId(globalObjectId);
			MeetingOrganizerEntry meetingOrganizerEntry = null;
			bool flag = false;
			lock (this.instanceLock)
			{
				this.map.TryGetValue(globalObjectId2.Uid, out meetingOrganizerEntry);
			}
			if (meetingOrganizerEntry == null)
			{
				meetingOrganizerEntry = ((MeetingOrganizerInfo.OnCacheMissForTest != null) ? MeetingOrganizerInfo.OnCacheMissForTest(globalObjectId) : this.ReadFromStore(globalObjectId));
				if (meetingOrganizerEntry != null)
				{
					if (meetingOrganizerEntry.CleanGlobalObjectId.Uid == null)
					{
						Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, "Get_NullUID");
					}
					else if (meetingOrganizerEntry.IsOrganizer != null)
					{
						this.Add(meetingOrganizerEntry);
					}
				}
			}
			else
			{
				flag = true;
			}
			if (Command.CurrentCommand != null)
			{
				Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.MeetingOrganizerLookup, flag.ToString());
			}
			return meetingOrganizerEntry;
		}

		public bool Contains(GlobalObjectId globalObjectId)
		{
			GlobalObjectId globalObjectId2 = this.MakeCleanId(globalObjectId);
			bool result;
			lock (this.instanceLock)
			{
				result = this.map.ContainsKey(globalObjectId2.Uid);
			}
			return result;
		}

		public bool Add(MeetingOrganizerEntry entry)
		{
			this.CleanIfNecessary();
			if (entry.CleanGlobalObjectId == null || entry.CleanGlobalObjectId.Uid == null)
			{
				return false;
			}
			bool result;
			lock (this.instanceLock)
			{
				if (this.map.ContainsKey(entry.CleanGlobalObjectId.Uid))
				{
					result = false;
				}
				else
				{
					this.map[entry.CleanGlobalObjectId.Uid] = entry;
					this.IsDirty = true;
					result = true;
				}
			}
			return result;
		}

		internal bool IgnoreCleanupTimeIntervalCheckForTest { get; set; }

		internal Dictionary<string, MeetingOrganizerEntry> GetMapForTest()
		{
			return this.map;
		}

		internal void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
		{
			lock (this.instanceLock)
			{
				componentDataPool.GetDateTimeDataInstance().Bind(this.LastCleanTime).SerializeData(writer, componentDataPool);
				GenericDictionaryData<StringData, string, MeetingOrganizerEntryData, MeetingOrganizerEntry> genericDictionaryData = new GenericDictionaryData<StringData, string, MeetingOrganizerEntryData, MeetingOrganizerEntry>(this.map);
				genericDictionaryData.SerializeData(writer, componentDataPool);
			}
		}

		internal void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			lock (this.instanceLock)
			{
				DateTimeData dateTimeDataInstance = componentDataPool.GetDateTimeDataInstance();
				dateTimeDataInstance.DeserializeData(reader, componentDataPool);
				this.LastCleanTime = dateTimeDataInstance.Data;
				GenericDictionaryData<StringData, string, MeetingOrganizerEntryData, MeetingOrganizerEntry> genericDictionaryData = new GenericDictionaryData<StringData, string, MeetingOrganizerEntryData, MeetingOrganizerEntry>();
				genericDictionaryData.DeserializeData(reader, componentDataPool);
				this.map = genericDictionaryData.Data;
				this.IsDirty = false;
				if (this.LastCleanTime - (ExDateTime)TimeProvider.UtcNow > MeetingOrganizerInfo.AllowedClockSkew)
				{
					throw new CorruptSyncStateException(new LocalizedString("DeviceBehavior.DeserializeData"), null);
				}
				this.CleanIfNecessary();
			}
		}

		private MeetingOrganizerEntry ReadFromStore(GlobalObjectId globalObjectId)
		{
			MeetingOrganizerEntry result = null;
			if (Command.CurrentCommand != null)
			{
				string subject;
				string organizer;
				bool? isOrganizer = MeetingOrganizerValidator.IsOrganizer(Command.CurrentCommand.MailboxSession, globalObjectId, out subject, out organizer);
				result = new MeetingOrganizerEntry(globalObjectId, organizer, isOrganizer, subject);
			}
			return result;
		}

		private MeetingOrganizerEntry BuildFromCalendarItem(GlobalObjectId globalObjectId, CalendarItemBase calendarItem)
		{
			return new MeetingOrganizerEntry(globalObjectId, (calendarItem.Organizer == null) ? "<NULL>" : calendarItem.Organizer.EmailAddress, new bool?(calendarItem.IsOrganizer()), calendarItem.Subject);
		}

		internal void CleanIfNecessary()
		{
			if (this.IgnoreCleanupTimeIntervalCheckForTest || (this.map.Count > 0 && (ExDateTime)TimeProvider.UtcNow - this.LastCleanTime >= MeetingOrganizerInfo.CleanupCheckInterval))
			{
				lock (this.instanceLock)
				{
					List<string> list = null;
					if (this.IgnoreCleanupTimeIntervalCheckForTest || (this.map.Count > 0 && (ExDateTime)TimeProvider.UtcNow - this.LastCleanTime >= MeetingOrganizerInfo.CleanupCheckInterval))
					{
						foreach (KeyValuePair<string, MeetingOrganizerEntry> keyValuePair in this.map)
						{
							if (TimeProvider.UtcNow - keyValuePair.Value.EntryTime >= GlobalSettings.MeetingOrganizerEntryLiveTime)
							{
								if (list == null)
								{
									list = new List<string>();
								}
								list.Add(keyValuePair.Key);
							}
						}
						if (list != null)
						{
							foreach (string key in list)
							{
								this.map.Remove(key);
							}
						}
						this.LastCleanTime = (ExDateTime)TimeProvider.UtcNow;
						this.IsDirty = true;
					}
				}
			}
		}

		private GlobalObjectId MakeCleanId(GlobalObjectId globalObjectId)
		{
			if (!globalObjectId.IsCleanGlobalObjectId)
			{
				return new GlobalObjectId(globalObjectId.CleanGlobalObjectIdBytes);
			}
			return globalObjectId;
		}

		private static readonly TimeSpan AllowedClockSkew = TimeSpan.FromDays(1.0);

		private object instanceLock = new object();

		private Dictionary<string, MeetingOrganizerEntry> map = new Dictionary<string, MeetingOrganizerEntry>();

		internal static TimeSpan CleanupCheckInterval = GlobalSettings.MeetingOrganizerCleanupTime;
	}
}
