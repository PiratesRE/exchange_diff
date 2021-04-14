using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxTableManager
	{
		internal MailboxTableManager(Guid databaseGuid)
		{
			this.databaseGuid = databaseGuid;
			this.syncObject = new object();
		}

		internal Dictionary<Guid, Pair<ExDateTime?, ExDateTime?>> GetAllMailboxes(ExDateTime? baselineTime, DatabaseManager databaseManager)
		{
			SyncUtilities.ThrowIfArgumentNull("databaseManager", databaseManager);
			PropValue[][] array = null;
			Dictionary<Guid, Pair<ExDateTime?, ExDateTime?>> dictionary = new Dictionary<Guid, Pair<ExDateTime?, ExDateTime?>>();
			Dictionary<Guid, Pair<ExDateTime?, ExDateTime?>> result;
			lock (this.syncObject)
			{
				try
				{
					using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=TransportSync", null, null, null, null))
					{
						array = exRpcAdmin.GetMailboxTable(this.databaseGuid, new PropTag[]
						{
							PropTag.UserGuid,
							PropTag.DateDiscoveredAbsentInDS,
							PropTag.MailboxMiscFlags,
							PropTag.TransportSyncSubscriptionListTimestamp,
							PropTag.MailboxType,
							PropTag.MailboxTypeDetail
						});
					}
				}
				catch (MapiRetryableException ex)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)396UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), "GetAllMailboxes: Encountered:{0}.", new object[]
					{
						ex
					});
					return null;
				}
				catch (MapiPermanentException ex2)
				{
					ContentAggregationConfig.SyncLogSession.LogError((TSLID)397UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), "GetAllMailboxes: Encountered:{0}.", new object[]
					{
						ex2
					});
					return null;
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Length != 6)
					{
						ContentAggregationConfig.SyncLogSession.LogError((TSLID)398UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), "GetAllMailboxes: Columns in current row are not as desired: {0}.", new object[]
						{
							array[i].Length
						});
						return null;
					}
					if (array[i][0].PropTag != PropTag.UserGuid)
					{
						ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)399UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), "GetAllMailboxes: Moving to next row as the 1st prop tag is not userGuid but {0}.", new object[]
						{
							array[i][0].PropTag
						});
					}
					else
					{
						Guid guid = new Guid(array[i][0].GetBytes());
						if (array[i][1].PropTag == PropTag.DateDiscoveredAbsentInDS)
						{
							ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)573UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), Guid.Empty, guid, "GetAllMailboxes: This mailbox is not in the directory.", new object[0]);
						}
						else
						{
							MailboxMiscFlags flags = MailboxMiscFlags.None;
							if (array[i][2].PropTag == PropTag.MailboxMiscFlags)
							{
								flags = (MailboxMiscFlags)array[i][2].GetInt();
							}
							else
							{
								ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)402UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), Guid.Empty, guid, "GetAllMailboxes: Couldn't perform archive mailbox check as the 3rd prop tag is {0}.", new object[]
								{
									array[i][2].PropTag
								});
							}
							int @int = array[i][4].GetInt();
							StoreMailboxTypeDetail int2 = (StoreMailboxTypeDetail)array[i][5].GetInt();
							if (!this.ShouldSkipMailbox(guid, flags, @int, int2))
							{
								if (!databaseManager.IsUserMailbox(guid))
								{
									ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)403UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), Guid.Empty, guid, "GetAllMailboxes: This mailbox is a system mailbox and will be skipped.", new object[0]);
								}
								else
								{
									ExDateTime? exDateTime = null;
									if (array[i][3].PropTag == PropTag.TransportSyncSubscriptionListTimestamp)
									{
										exDateTime = new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, array[i][3].GetDateTime()));
										if (exDateTime == MailboxTableManager.MinSystemDateTimeValue)
										{
											ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)404UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), Guid.Empty, guid, "GetAllMailboxes: This mailbox does not have any subscriptions and will be skipped.", new object[0]);
											goto IL_4A1;
										}
										if (baselineTime != null && baselineTime != null && exDateTime != null && exDateTime != null && exDateTime.Value < baselineTime.Value)
										{
											ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)405UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), Guid.Empty, guid, "GetAllMailboxes: This mailbox was not updated since the last polling and will be skipped.Baseline: {0}. SubscriptionListTimestamp: {1} ", new object[]
											{
												baselineTime.Value.ToString("MM/dd/yyyy hh:mm:ss.fff"),
												exDateTime.Value.ToString("MM/dd/yyyy hh:mm:ss.fff")
											});
											goto IL_4A1;
										}
									}
									dictionary.Add(guid, new Pair<ExDateTime?, ExDateTime?>(exDateTime, null));
								}
							}
						}
					}
					IL_4A1:;
				}
				ContentAggregationConfig.SyncLogSession.LogInformation((TSLID)406UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), "GetAllMailboxes: {0} relevant mailboxes retrieved from the mailbox table on database {1}.", new object[]
				{
					dictionary.Count,
					this.databaseGuid
				});
				result = dictionary;
			}
			return result;
		}

		internal ExDateTime? GetMailboxSubscriptionListTimestamp(MailboxSession mailboxSession)
		{
			MapiStore mapiStore = mailboxSession.Mailbox.MapiStore;
			PropValue[] array = null;
			try
			{
				array = mapiStore.GetProps(new PropTag[]
				{
					PropTag.TransportSyncSubscriptionListTimestamp
				});
			}
			catch (MapiRetryableException exception)
			{
				bool flag;
				Exception ex = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception, out flag);
				throw ex;
			}
			catch (MapiPermanentException exception2)
			{
				bool flag2;
				Exception ex = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception2, out flag2);
				throw ex;
			}
			if (array == null || array.Length == 0 || (array[0].IsError() && array[0].RawValue != null && (int)array[0].RawValue == -2147221233))
			{
				return null;
			}
			if (array.Length > 1 || array[0].IsError())
			{
				Exception ex = this.CreateCacheTransientException(mailboxSession.MailboxGuid, Strings.FailedGetMailboxSubscriptionListTimestamp);
				throw ex;
			}
			return new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, array[0].GetDateTime()));
		}

		internal void SetMailboxSubscriptionListTimestamp(MailboxSession mailboxSession, ExDateTime subscriptionListTimestamp)
		{
			MapiStore mapiStore = mailboxSession.Mailbox.MapiStore;
			PropProblem[] array = null;
			try
			{
				array = mapiStore.SetProps(new PropValue[]
				{
					new PropValue(PropTag.TransportSyncSubscriptionListTimestamp, (DateTime)subscriptionListTimestamp)
				});
				if (array == null)
				{
					using (MapiFolder inboxFolder = mapiStore.GetInboxFolder())
					{
						ContentAggregationFlags contentAggregationFlags = (subscriptionListTimestamp == MailboxTableManager.MinSystemDateTimeValue) ? ContentAggregationFlags.None : ContentAggregationFlags.HasSubscriptions;
						PropValue prop = inboxFolder.GetProp(PropTag.ContentAggregationFlags);
						ContentAggregationFlags contentAggregationFlags2 = ContentAggregationFlags.None;
						if (!prop.IsNull() && !prop.IsError())
						{
							contentAggregationFlags2 = (ContentAggregationFlags)prop.GetInt();
						}
						if (contentAggregationFlags2 != contentAggregationFlags)
						{
							array = inboxFolder.SetProps(new PropValue[]
							{
								new PropValue(PropTag.ContentAggregationFlags, (int)contentAggregationFlags)
							});
						}
					}
				}
			}
			catch (MapiRetryableException exception)
			{
				bool flag;
				Exception ex = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception, out flag);
				throw ex;
			}
			catch (MapiPermanentException exception2)
			{
				bool flag2;
				Exception ex = this.ConvertToCacheException(mailboxSession.MailboxGuid, exception2, out flag2);
				throw ex;
			}
			if (array != null)
			{
				Exception ex = this.CreateCacheTransientException(mailboxSession.MailboxGuid, Strings.FailedSetMailboxSubscriptionListTimestamp);
				throw ex;
			}
		}

		[Conditional("DEBUG")]
		private static void CheckMailboxTableFlags()
		{
			string[] names = Enum.GetNames(typeof(MailboxMiscFlags));
			if (names.Length != MailboxTableManager.KnownMailboxFlags.Count)
			{
				throw new InvalidOperationException(string.Format("Unexpected mailbox flags count. Expected: {0}. Actual: {1}.", MailboxTableManager.KnownMailboxFlags.Count, names.Length));
			}
			foreach (string text in names)
			{
				if (!MailboxTableManager.KnownMailboxFlags.Contains(text))
				{
					throw new InvalidOperationException("Unknown mailbox flag " + text);
				}
			}
		}

		private CacheTransientException CreateCacheTransientException(Guid mailboxGuid, LocalizedString exceptionInfo)
		{
			return GlobalDatabaseHandler.CreateCacheTransientException(MailboxTableManager.Tracer, this.GetHashCode(), this.databaseGuid, mailboxGuid, exceptionInfo);
		}

		private Exception ConvertToCacheException(Guid mailboxGuid, Exception exception, out bool reuseSession)
		{
			reuseSession = false;
			return GlobalDatabaseHandler.ConvertToCacheException(MailboxTableManager.Tracer, this.GetHashCode(), this.databaseGuid, mailboxGuid, exception, out reuseSession);
		}

		private bool ShouldSkipMailbox(Guid mailboxGuid, MailboxMiscFlags flags, int mailboxType, StoreMailboxTypeDetail mailboxTypeDetail)
		{
			bool flag = false;
			string text = null;
			if (StoreSession.IsPublicFolderMailbox(mailboxType))
			{
				flag = true;
				text = "public folder";
			}
			else if (mailboxTypeDetail == StoreMailboxTypeDetail.SharedMailbox || mailboxTypeDetail == StoreMailboxTypeDetail.TeamMailbox)
			{
				flag = true;
				text = "shared mailbox";
			}
			else if (MailboxMiscFlags.ArchiveMailbox == (flags & MailboxMiscFlags.ArchiveMailbox))
			{
				flag = true;
				text = "archive";
			}
			else if (MailboxMiscFlags.DisabledMailbox == (flags & MailboxMiscFlags.DisabledMailbox))
			{
				flag = true;
				text = "disabled";
			}
			else if (MailboxMiscFlags.CreatedByMove == (flags & MailboxMiscFlags.CreatedByMove))
			{
				flag = true;
				text = "move target";
			}
			else if (MailboxMiscFlags.SoftDeletedMailbox == (flags & MailboxMiscFlags.SoftDeletedMailbox))
			{
				flag = true;
				text = "soft deleted";
			}
			else if (MailboxMiscFlags.MRSSoftDeletedMailbox == (flags & MailboxMiscFlags.MRSSoftDeletedMailbox))
			{
				flag = true;
				text = "soft deleted (MRS)";
			}
			if (flag)
			{
				ContentAggregationConfig.SyncLogSession.LogDebugging((TSLID)401UL, MailboxTableManager.Tracer, (long)this.GetHashCode(), Guid.Empty, mailboxGuid, "GetAllMailboxes: Skipping {0} mailbox.", new object[]
				{
					text
				});
			}
			return flag;
		}

		internal static readonly ExDateTime MinSystemDateTimeValue = new ExDateTime(ExTimeZone.UtcTimeZone, DateTime.FromFileTimeUtc(0L));

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.MailboxTableManagerTracer;

		private static readonly HashSet<string> KnownMailboxFlags = new HashSet<string>
		{
			MailboxMiscFlags.None.ToString(),
			MailboxMiscFlags.CreatedByMove.ToString(),
			MailboxMiscFlags.ArchiveMailbox.ToString(),
			MailboxMiscFlags.DisabledMailbox.ToString(),
			MailboxMiscFlags.SoftDeletedMailbox.ToString(),
			MailboxMiscFlags.MRSSoftDeletedMailbox.ToString()
		};

		private object syncObject;

		private Guid databaseGuid;
	}
}
