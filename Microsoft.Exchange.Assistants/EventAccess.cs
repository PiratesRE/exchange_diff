using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Assistants.EventLog;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class EventAccess : Base, IDisposable
	{
		public static EventAccess Create(DatabaseInfo databaseInfo, EventBasedAssistantCollection assistants)
		{
			return new EventAccess(databaseInfo, assistants);
		}

		private EventAccess(DatabaseInfo databaseInfo, EventBasedAssistantCollection assistants)
		{
			this.databaseInfo = databaseInfo;
			this.assistantCollection = assistants;
		}

		public bool RestartRequired
		{
			get
			{
				return this.restartRequired;
			}
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "EventAccess for database " + this.databaseInfo.ToString();
			}
			return this.toString;
		}

		public void Dispose()
		{
			this.Disconnect();
		}

		public void Disconnect()
		{
			if (this.exRpcAdmin != null)
			{
				ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess>((long)this.GetHashCode(), "{0}: Disconnecting...", this);
				this.exRpcAdmin.Dispose();
				this.exRpcAdmin = null;
				this.pollingEventManager = null;
				this.mapiEventManagers.Clear();
				this.mapiEventManagers = null;
				ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess>((long)this.GetHashCode(), "{0}: Disconnected.", this);
			}
		}

		public Watermark[] GetWatermarksForAssistant(Guid assistantId)
		{
			Watermark[] watermarks = null;
			this.CallEventManager(delegate
			{
				watermarks = this.mapiEventManagers[assistantId].GetWatermarks();
			});
			return watermarks;
		}

		public Btree<Guid, Bookmark> LoadAllMailboxBookmarks(Bookmark currentDatabaseBookmark)
		{
			ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, Bookmark>((long)this.GetHashCode(), "{0}: Loading all mailbox bookmarks. Current database watermark is {1}", this, currentDatabaseBookmark);
			int numberOfActiveMailboxesWithDecayedWatermarks = 0;
			int numberOfStaleMailboxesWithDecayedWatermarks = 0;
			Dictionary<Guid, MailboxInformation> mailboxesWithDecayedWatermarks = new Dictionary<Guid, MailboxInformation>();
			Btree<Guid, Bookmark> allBookmarks = new Btree<Guid, Bookmark>(100);
			this.CallEventManager(delegate
			{
				long lowestEventCounter = this.GetLowestEventCounter();
				ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, long>((long)this.GetHashCode(), "{0}: Lowest Event Counter: {1}", this, lowestEventCounter);
				foreach (AssistantCollectionEntry assistantCollectionEntry in this.assistantCollection)
				{
					Watermark[] watermarksForAssistant = this.GetWatermarksForAssistant(assistantCollectionEntry.Identity);
					ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, int, LocalizedString>((long)this.GetHashCode(), "{0}: Retrieved {1} watermarks for assistant {2}", this, watermarksForAssistant.Length, assistantCollectionEntry.Instance.Name);
					foreach (Watermark watermark in watermarksForAssistant)
					{
						if (watermark.MailboxGuid == Guid.Empty)
						{
							ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess>((long)this.GetHashCode(), "{0}: Skipping database watermark.", this);
						}
						else
						{
							if (EventAccess.IsWatermarkBehindEventCounter(watermark.EventCounter, lowestEventCounter))
							{
								MailboxInformation mailboxInformation;
								if (!mailboxesWithDecayedWatermarks.TryGetValue(watermark.MailboxGuid, out mailboxInformation))
								{
									ExTraceGlobals.EventAccessTracer.TraceDebug((long)this.GetHashCode(), "{0}: Found a decayed watermark {1} for mailbox {2} and assistant {3}", new object[]
									{
										this,
										watermark.EventCounter,
										watermark.MailboxGuid,
										assistantCollectionEntry.Name
									});
									mailboxInformation = MailboxInformation.Create(this.exRpcAdmin, watermark.MailboxGuid, this.databaseInfo.Guid);
									mailboxesWithDecayedWatermarks.Add(watermark.MailboxGuid, mailboxInformation);
									if (!mailboxInformation.Active)
									{
										numberOfStaleMailboxesWithDecayedWatermarks++;
									}
									else
									{
										numberOfActiveMailboxesWithDecayedWatermarks++;
									}
								}
								else
								{
									ExTraceGlobals.EventAccessTracer.TraceDebug((long)this.GetHashCode(), "{0}: Found another decayed watermark {1} for mailbox {2} and assistant {3}", new object[]
									{
										this,
										watermark.EventCounter,
										watermark.MailboxGuid,
										assistantCollectionEntry.Name
									});
								}
								if (!mailboxInformation.Active)
								{
									ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, Guid>((long)this.GetHashCode(), "{0}: Mailbox {1} was identified as stale.", this, mailboxInformation.MailboxGuid);
									this.DeleteWatermark(assistantCollectionEntry.Identity, mailboxInformation.MailboxGuid);
									goto IL_2A5;
								}
							}
							Bookmark bookmark;
							if (!allBookmarks.TryGetValue(watermark.MailboxGuid, out bookmark))
							{
								bookmark = Bookmark.CreateFromDatabaseBookmark(watermark.MailboxGuid, currentDatabaseBookmark);
								allBookmarks.Add(bookmark);
							}
							bookmark[assistantCollectionEntry.Identity] = watermark.EventCounter;
						}
						IL_2A5:;
					}
				}
			});
			if (numberOfStaleMailboxesWithDecayedWatermarks != 0 || numberOfActiveMailboxesWithDecayedWatermarks != 0)
			{
				ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, int, int>((long)this.GetHashCode(), "{0}: Found a total of {1} active mailbox(s) and {2} stale mailbox(s) with decayed watermarks.", this, numberOfActiveMailboxesWithDecayedWatermarks, numberOfStaleMailboxesWithDecayedWatermarks);
				int num = Math.Min(100, numberOfActiveMailboxesWithDecayedWatermarks + numberOfStaleMailboxesWithDecayedWatermarks);
				StringBuilder stringBuilder = new StringBuilder(num * 100);
				using (Dictionary<Guid, MailboxInformation>.Enumerator enumerator = mailboxesWithDecayedWatermarks.GetEnumerator())
				{
					while (enumerator.MoveNext() && num-- > 0)
					{
						stringBuilder.Append(Environment.NewLine);
						StringBuilder stringBuilder2 = stringBuilder;
						object[] array = new object[4];
						object[] array2 = array;
						int num2 = 0;
						KeyValuePair<Guid, MailboxInformation> keyValuePair = enumerator.Current;
						array2[num2] = keyValuePair.Value.DisplayName;
						array[1] = " (";
						object[] array3 = array;
						int num3 = 2;
						KeyValuePair<Guid, MailboxInformation> keyValuePair2 = enumerator.Current;
						array3[num3] = keyValuePair2.Key;
						array[3] = ")";
						stringBuilder2.Append(string.Concat(array));
					}
				}
				base.LogEvent(AssistantsEventLogConstants.Tuple_MailboxesWithDecayedWatermarks, null, new object[]
				{
					numberOfActiveMailboxesWithDecayedWatermarks,
					numberOfStaleMailboxesWithDecayedWatermarks,
					this.databaseInfo.DisplayName,
					stringBuilder.ToString()
				});
			}
			return allBookmarks;
		}

		public Bookmark GetMailboxBookmark(Guid mailboxGuid, Bookmark currentDatabaseBookmark, bool mailboxIsUpToDate)
		{
			ExTraceGlobals.EventAccessTracer.TraceDebug((long)this.GetHashCode(), "{0}: Loading Bookmark for mailbox {1} while current database Bookmark is {2}. Mailbox is up-to-date: {3}", new object[]
			{
				this,
				mailboxGuid,
				currentDatabaseBookmark,
				mailboxIsUpToDate
			});
			Bookmark bookmark = Bookmark.CreateFromDatabaseBookmark(mailboxGuid, currentDatabaseBookmark);
			this.CallEventManager(delegate
			{
				foreach (AssistantCollectionEntry assistantCollectionEntry in this.assistantCollection)
				{
					Watermark watermark = this.mapiEventManagers[assistantCollectionEntry.Identity].GetWatermark(mailboxGuid);
					if (watermark != null)
					{
						if (!mailboxIsUpToDate)
						{
							bookmark[assistantCollectionEntry.Identity] = watermark.EventCounter;
						}
						else
						{
							bookmark[assistantCollectionEntry.Identity] = Math.Max(watermark.EventCounter, bookmark[assistantCollectionEntry.Identity]);
						}
					}
				}
			});
			ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, Bookmark>((long)this.GetHashCode(), "{0}: Got {1}.", this, bookmark);
			return bookmark;
		}

		public Bookmark GetDatabaseBookmark()
		{
			ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess>((long)this.GetHashCode(), "{0}: Getting Database Bookmark.", this);
			Bookmark bookmark = this.CreateBookmark(Guid.Empty, 0L);
			long lowestWatermark = long.MaxValue;
			List<Guid> missingWatermarks = null;
			this.CallEventManager(delegate
			{
				foreach (AssistantCollectionEntry assistantCollectionEntry in this.assistantCollection)
				{
					Watermark watermark = this.mapiEventManagers[assistantCollectionEntry.Identity].GetWatermark(Guid.Empty);
					if (watermark != null)
					{
						ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, long, LocalizedString>((long)this.GetHashCode(), "{0}: Found database watermark {1} for assistant {2}", this, watermark.EventCounter, assistantCollectionEntry.Instance.Name);
						bookmark[assistantCollectionEntry.Identity] = watermark.EventCounter;
						lowestWatermark = Math.Min(lowestWatermark, watermark.EventCounter);
					}
					else
					{
						ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, LocalizedString>((long)this.GetHashCode(), "{0}: Missing database watermark for assistant {1}", this, assistantCollectionEntry.Instance.Name);
						if (missingWatermarks == null)
						{
							missingWatermarks = new List<Guid>();
						}
						missingWatermarks.Add(assistantCollectionEntry.Identity);
					}
				}
			});
			if (missingWatermarks != null && lowestWatermark != 9223372036854775807L)
			{
				ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, long>((long)this.GetHashCode(), "{0}: There are missing database watermark(s). Will use watermark as {1}.", this, lowestWatermark);
				foreach (Guid guid in missingWatermarks)
				{
					ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess, Guid>((long)this.GetHashCode(), "{0}: Setting missing database watermark for assistant {1}.", this, guid);
					bookmark[guid] = lowestWatermark;
				}
			}
			return bookmark;
		}

		public Bookmark CreateBookmark(Guid mailboxGuid, long eventCounter)
		{
			Bookmark bookmark = Bookmark.Create(mailboxGuid, this.assistantCollection.Count);
			foreach (AssistantCollectionEntry assistantCollectionEntry in this.assistantCollection)
			{
				bookmark[assistantCollectionEntry.Identity] = eventCounter;
			}
			return bookmark;
		}

		public void SaveWatermarks(Guid assistantId, params Watermark[] watermarks)
		{
			this.CallEventManager(delegate
			{
				this.mapiEventManagers[assistantId].SaveWatermarks(watermarks);
			});
		}

		internal void DeleteWatermark(Guid assistantId, Guid mailboxGuid)
		{
			this.CallEventManager(delegate
			{
				this.mapiEventManagers[assistantId].DeleteWatermark(mailboxGuid);
			});
		}

		public MapiEvent ReadLastEvent()
		{
			MapiEvent retVal = null;
			this.CallEventManager(delegate
			{
				retVal = this.pollingEventManager.ReadLastEvent();
			});
			return retVal;
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted)
		{
			MapiEvent[] retVal = null;
			this.CallEventManager(delegate
			{
				retVal = this.pollingEventManager.ReadEvents(startCounter, eventCountWanted);
			});
			return retVal;
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, int eventCountToCheck, Restriction filter, out long endCounter)
		{
			MapiEvent[] retVal = null;
			long newEndCounter = 0L;
			this.CallEventManager(delegate
			{
				retVal = this.pollingEventManager.ReadEvents(startCounter, eventCountWanted, eventCountToCheck, filter, out newEndCounter);
			});
			endCounter = newEndCounter;
			return retVal;
		}

		private static bool IsWatermarkBehindEventCounter(long watermark, long eventCounter)
		{
			return watermark + 1L < eventCounter;
		}

		private long GetLowestEventCounter()
		{
			MapiEvent[] array = this.ReadEvents(0L, 1);
			if (array.Length <= 0)
			{
				return 0L;
			}
			return array[0].EventCounter;
		}

		private void CallEventManager(EventAccess.EventManagerFunction function)
		{
			lock (this)
			{
				this.ConnectIfNecessary();
				Exception ex = null;
				try
				{
					function();
				}
				catch (MapiExceptionVersion mapiExceptionVersion)
				{
					ex = mapiExceptionVersion;
				}
				catch (MapiExceptionMdbOffline mapiExceptionMdbOffline)
				{
					ex = mapiExceptionMdbOffline;
				}
				catch (MapiExceptionExiting mapiExceptionExiting)
				{
					ex = mapiExceptionExiting;
				}
				catch (MapiExceptionNetworkError mapiExceptionNetworkError)
				{
					ex = mapiExceptionNetworkError;
				}
				catch (MapiExceptionNoAccess mapiExceptionNoAccess)
				{
					ex = mapiExceptionNoAccess;
				}
				if (ex != null)
				{
					this.restartRequired = true;
					ExTraceGlobals.EventAccessTracer.TraceError<EventAccess, Exception>((long)this.GetHashCode(), "{0}: Encountered exception: {1}", this, ex);
					this.Disconnect();
					throw new DatabaseRestartRequiredException(ex);
				}
			}
		}

		private void ConnectIfNecessary()
		{
			if (this.restartRequired)
			{
				ExTraceGlobals.EventAccessTracer.TraceError<EventAccess>((long)this.GetHashCode(), "{0}: Unable to connect; restart required", this);
				throw new DatabaseRestartRequiredException();
			}
			if (this.exRpcAdmin == null)
			{
				bool flag = false;
				ExTraceGlobals.EventAccessTracer.TraceDebug<EventAccess>((long)this.GetHashCode(), "{0}: Creating exRpcAdmin/eventManager...", this);
				try
				{
					this.exRpcAdmin = ExRpcAdmin.Create("Client=EBA", null, null, null, null);
					this.pollingEventManager = MapiEventManager.Create(this.exRpcAdmin, Guid.Empty, this.databaseInfo.Guid);
					this.mapiEventManagers = new Dictionary<Guid, MapiEventManager>(this.assistantCollection.Count);
					foreach (AssistantCollectionEntry assistantCollectionEntry in this.assistantCollection)
					{
						this.mapiEventManagers.Add(assistantCollectionEntry.Identity, MapiEventManager.Create(this.exRpcAdmin, assistantCollectionEntry.Identity, this.databaseInfo.Guid));
					}
					flag = true;
					base.TracePfd("PFD AIS {0} {1}: Created eventManager successfully.", new object[]
					{
						25431,
						this
					});
				}
				finally
				{
					if (!flag)
					{
						ExTraceGlobals.EventAccessTracer.TraceError<EventAccess>((long)this.GetHashCode(), "{0}: Failed to create exRpcAdmin/eventManager.", this);
						this.Disconnect();
					}
				}
			}
		}

		private MapiEventManager pollingEventManager;

		private Dictionary<Guid, MapiEventManager> mapiEventManagers;

		private EventBasedAssistantCollection assistantCollection;

		private DatabaseInfo databaseInfo;

		private ExRpcAdmin exRpcAdmin;

		private string toString;

		private bool restartRequired;

		private delegate void EventManagerFunction();
	}
}
