using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiMailbox : DisposableBase
	{
		private MapiMailbox(StoreDatabase database, MailboxState mailboxState, int lcid)
		{
			this.database = database;
			this.sharedState = mailboxState;
			this.lcid = lcid;
			this.valid = true;
		}

		private MapiMailbox(Mailbox storeMailbox, int lcid) : this(storeMailbox.Database, storeMailbox.SharedState, lcid)
		{
			this.storeMailbox = storeMailbox;
		}

		public static MapiMailbox OpenMailbox(MapiContext context, MailboxState mailboxState, MailboxInfo.MailboxType mailboxType, int lcid)
		{
			MapiMailbox result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MapiMailbox mapiMailbox = disposeGuard.Add<MapiMailbox>(new MapiMailbox(context.Database, mailboxState, lcid));
				mapiMailbox.OpenMailbox(context, mailboxType);
				disposeGuard.Success();
				result = mapiMailbox;
			}
			return result;
		}

		internal static void CreateMailbox(MapiContext context, MailboxState mailboxState, AddressInfo addressInfoMailbox, MailboxInfo mailboxInfo, DatabaseInfo databaseInfo, TenantHint tenantHint, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, CrucialFolderId fidc, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool createdByMove)
		{
			if (ExTraceGlobals.CreateMailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CreateMailboxTracer.TraceDebug<Guid, string>(0L, "Creating mailbox mailboxGuid={0}, TenantHint={1}", mailboxInfo.MailboxGuid, tenantHint.ToString());
			}
			if (databaseInfo.IsRecoveryDatabase)
			{
				throw new ExExceptionInvalidParameter((LID)60104U, "Cannot create a new mailbox on recovery database.");
			}
			if (mailboxInfo.IsTenantMailbox && tenantHint.IsRootOrg)
			{
				throw new StoreException((LID)52864U, ErrorCodeValue.UnknownUser, "No tenant hint provided while creating a tenant scoped mailbox");
			}
			using (Mailbox mailbox = MapiMailbox.CreateMailboxPhaseOne(context, mailboxState, mailboxInfo, tenantHint, mailboxInstanceGuid, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange, nextCnCounter, reservedCnCounterRange, numberToNameMap, replidGuidMap, defaultFoldersReplGuid, createdByMove))
			{
				MapiMailbox.CreateMailboxPhaseTwo(context, addressInfoMailbox, mailboxInfo, databaseInfo, mailbox, fidc);
			}
			FaultInjection.InjectFault(MapiMailbox.staticTestHook);
		}

		internal static void CreateMailbox(MapiContext context, MailboxState mailboxState, AddressInfo addressInfoMailbox, MailboxInfo mailboxInfo, DatabaseInfo databaseInfo, TenantHint tenantHint, Guid mailboxInstanceGuid, Guid localIdGuid, CrucialFolderId fidc, bool createdByMove)
		{
			MapiMailbox.CreateMailbox(context, mailboxState, addressInfoMailbox, mailboxInfo, databaseInfo, tenantHint, mailboxInstanceGuid, localIdGuid, Guid.NewGuid(), Mailbox.GetFirstAvailableIdGlobcount(mailboxInfo), null, 1UL, null, fidc, null, null, localIdGuid, createdByMove);
		}

		internal static void CreateMailbox(MapiContext context, MailboxState mailboxState, AddressInfo addressInfoMailbox, MailboxInfo mailboxInfo, DatabaseInfo databaseInfo, TenantHint tenantHint, Guid mailboxInstanceGuid, Guid localIdGuid)
		{
			MapiMailbox.CreateMailbox(context, mailboxState, addressInfoMailbox, mailboxInfo, databaseInfo, tenantHint, mailboxInstanceGuid, localIdGuid, null, false);
		}

		internal static CrucialFolderId GetFIDC(Context context, ExchangeId[] fidList, bool isPublicFolderMailbox)
		{
			CrucialFolderId crucialFolderId = new CrucialFolderId();
			if (isPublicFolderMailbox)
			{
				crucialFolderId.FidPublicFolderEFormsRegistry = fidList[9];
				crucialFolderId.FidPublicFolderIpmSubTree = fidList[7];
				crucialFolderId.FidPublicFolderNonIpmSubTree = fidList[8];
				crucialFolderId.FidPublicFolderInternalSubmission = fidList[11];
				crucialFolderId.FidPublicFolderAsyncDeleteState = fidList[12];
				crucialFolderId.FidPublicFolderDumpsterRoot = fidList[13];
				crucialFolderId.FidPublicFolderTombstonesRoot = fidList[2];
			}
			else
			{
				crucialFolderId.FidIPMsubtree = fidList[9];
				crucialFolderId.FidDAF = fidList[7];
				crucialFolderId.FidSpoolerQ = fidList[8];
				crucialFolderId.FidOutbox = fidList[11];
				crucialFolderId.FidSentmail = fidList[12];
				crucialFolderId.FidWastebasket = fidList[13];
				crucialFolderId.FidFinder = fidList[2];
			}
			crucialFolderId.FidRoot = fidList[1];
			crucialFolderId.FidInbox = fidList[10];
			crucialFolderId.FidViews = fidList[3];
			crucialFolderId.FidCommonViews = fidList[4];
			crucialFolderId.FidSchedule = fidList[5];
			crucialFolderId.FidShortcuts = fidList[6];
			crucialFolderId.FidConversations = fidList[20];
			return crucialFolderId;
		}

		internal StoreDatabase Database
		{
			get
			{
				this.ThrowIfNotValid();
				this.ThrowIfNotOpen();
				return this.database;
			}
		}

		public Mailbox StoreMailbox
		{
			get
			{
				this.ThrowIfNotValid();
				this.ThrowIfNotOpen();
				return this.storeMailbox;
			}
		}

		public bool IsConnected
		{
			get
			{
				this.ThrowIfNotValid();
				return this.storeMailbox.IsConnected;
			}
		}

		public string MailboxOwnerLegacyDN
		{
			get
			{
				this.ThrowIfNotValid();
				return null;
			}
		}

		public bool IsDL()
		{
			this.ThrowIfNotValid();
			return false;
		}

		public SecurityIdentifier PrimaryAccountSID
		{
			get
			{
				this.ThrowIfNotValid();
				return null;
			}
		}

		public int MailboxNumber
		{
			get
			{
				this.ThrowIfNotValid();
				return this.sharedState.MailboxNumber;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				this.ThrowIfNotValid();
				return this.sharedState.MailboxGuid;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				this.ThrowIfNotValid();
				return this.database.MdbGuid;
			}
		}

		internal Guid MailboxInstanceGuid
		{
			get
			{
				this.ThrowIfNotValid();
				return this.sharedState.MailboxInstanceGuid;
			}
		}

		internal MailboxState SharedState
		{
			get
			{
				this.ThrowIfNotValid();
				return this.sharedState;
			}
		}

		internal int Lcid
		{
			get
			{
				this.ThrowIfNotValid();
				return this.lcid;
			}
		}

		internal bool IsPublicFolderMailbox
		{
			get
			{
				this.ThrowIfNotValid();
				return this.StoreMailbox.IsPublicFolderMailbox;
			}
		}

		internal bool IsGroupMailbox
		{
			get
			{
				this.ThrowIfNotValid();
				return this.StoreMailbox.IsGroupMailbox;
			}
		}

		public static IDisposable SetStaticTestHook(Action action)
		{
			return MapiMailbox.staticTestHook.SetTestHook(action);
		}

		public static ErrorCode PrepareReportMessage(MapiContext context, MapiLogon mapiLogon, IList<Properties> recipientsProperties, out MapiMessage reportMessage)
		{
			ErrorCode result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				reportMessage = new MapiMessage();
				disposeGuard.Add<MapiMessage>(reportMessage);
				ErrorCode first = reportMessage.ConfigureMessage(context, mapiLogon, mapiLogon.FidC.FidRoot, ExchangeId.Zero, MessageConfigureFlags.CreateNewMessage | MessageConfigureFlags.IsReportMessage | MessageConfigureFlags.SkipQuotaCheck, mapiLogon.Session.CodePage);
				if (first == ErrorCode.NoError)
				{
					MapiPersonCollection recipients = reportMessage.GetRecipients();
					for (int i = 0; i < recipientsProperties.Count; i++)
					{
						MapiPerson item = recipients.GetItem(i, true);
						disposeGuard.Add<MapiPerson>(item);
						item.InternalSetPropsShouldNotFail(context, recipientsProperties[i]);
					}
					recipients.SaveChanges(context);
				}
				disposeGuard.Success();
				result = first.Propagate((LID)29780U);
			}
			return result;
		}

		public static Properties GetMailboxInfoTableEntry(Context context, StoreDatabase database, Guid mailboxGuid, GetMailboxInfoTableFlags flags, StorePropTag[] propTags)
		{
			bool flag = (flags & GetMailboxInfoTableFlags.IncludeSoftDeleted) == GetMailboxInfoTableFlags.IncludeSoftDeleted;
			MailboxTable mailboxTable = Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailboxGuid
			});
			SearchCriteria searchCriteria = Factory.CreateSearchCriteriaOr(new SearchCriteria[]
			{
				Factory.CreateSearchCriteriaCompare(mailboxTable.Status, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(2)),
				Factory.CreateSearchCriteriaCompare(mailboxTable.Status, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(3))
			});
			if (flag)
			{
				searchCriteria = Factory.CreateSearchCriteriaOr(new SearchCriteria[]
				{
					searchCriteria,
					Factory.CreateSearchCriteriaCompare(mailboxTable.Status, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(4))
				});
			}
			List<Column> list = MapiMailbox.MapPropTagToColumn(database, propTags);
			int num = list.IndexOf(mailboxTable.MailboxNumber);
			if (-1 == num)
			{
				list.Add(mailboxTable.MailboxNumber);
				num = list.IndexOf(mailboxTable.MailboxNumber);
			}
			int num2 = -1;
			int num3 = -1;
			if (UnifiedMailbox.IsReady(context, context.Database))
			{
				num2 = Array.IndexOf<StorePropTag>(propTags, PropTag.Mailbox.MailboxPartitionMailboxGuids);
				if (num2 != -1)
				{
					num3 = list.IndexOf(mailboxTable.UnifiedMailboxGuid);
					if (num3 == -1)
					{
						list.Add(mailboxTable.UnifiedMailboxGuid);
						num3 = list.IndexOf(mailboxTable.UnifiedMailboxGuid);
					}
				}
			}
			Properties result = new Properties(propTags.Length);
			Guid? guid = null;
			List<PrequarantinedMailbox> list2 = null;
			PrequarantinedMailbox prequarantinedMailbox = null;
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxTable.MailboxGuidIndex, list.ToArray(), searchCriteria, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (!reader.Read())
					{
						throw new ExExceptionNotFound((LID)50048U, "Mailbox not found.");
					}
					int @int = reader.GetInt32(list[num]);
					if (num3 != -1)
					{
						guid = reader.GetNullableGuid(list[num3]);
					}
					for (int i = 0; i < propTags.Length; i++)
					{
						object obj = null;
						ushort propId = propTags[i].PropId;
						if (propId != 13320)
						{
							switch (propId)
							{
							case 26650:
								obj = MapiMailbox.GetMailboxQuarantined(context, @int);
								goto IL_332;
							case 26651:
							case 26652:
							case 26653:
								obj = null;
								if (MapiMailbox.GetMailboxQuarantined(context, @int) && list2 == null)
								{
									list2 = MailboxQuarantineProvider.Instance.GetPreQuarantinedMailboxes(context.Database.MdbGuid);
									foreach (PrequarantinedMailbox prequarantinedMailbox2 in list2)
									{
										if (prequarantinedMailbox2.MailboxGuid == mailboxGuid)
										{
											prequarantinedMailbox = prequarantinedMailbox2;
											break;
										}
									}
								}
								if (prequarantinedMailbox == null)
								{
									goto IL_332;
								}
								if (propTags[i].PropId == 26651)
								{
									obj = prequarantinedMailbox.QuarantineReason;
									goto IL_332;
								}
								if (propTags[i].PropId == 26652)
								{
									obj = prequarantinedMailbox.LastCrashTime;
									goto IL_332;
								}
								obj = prequarantinedMailbox.LastCrashTime + prequarantinedMailbox.QuarantineDuration;
								goto IL_332;
							case 26655:
								obj = @int;
								goto IL_332;
							}
							obj = reader.GetValue(list[i]);
							obj = Mailbox.PostProcessMailboxPropValue(obj, propTags[i]);
						}
						else
						{
							obj = null;
						}
						IL_332:
						if (obj != null)
						{
							result.Add(propTags[i], obj);
						}
						else
						{
							result.Add(propTags[i].ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
						}
					}
				}
			}
			if (num2 != -1 && guid != null)
			{
				Guid[] partitionMailboxGuids = MapiMailbox.GetPartitionMailboxGuids(context, guid.Value);
				result[num2] = new Property(PropTag.Mailbox.MailboxPartitionMailboxGuids, partitionMailboxGuids);
			}
			return result;
		}

		public static Guid[] GetPartitionMailboxGuids(Context context, Guid unifiedMailboxGuid)
		{
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				unifiedMailboxGuid
			});
			MailboxTable mailboxTable = Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(context.Database);
			SearchCriteria restriction = Factory.CreateSearchCriteriaOr(new SearchCriteria[]
			{
				Factory.CreateSearchCriteriaCompare(mailboxTable.Status, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(2)),
				Factory.CreateSearchCriteriaCompare(mailboxTable.Status, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(3))
			});
			Guid[] result;
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxTable.UnifiedMailboxGuidIndex, new Column[]
			{
				mailboxTable.MailboxGuid
			}, restriction, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				List<Guid> list = new List<Guid>(10);
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						list.Add(reader.GetGuid(mailboxTable.MailboxGuid));
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		public static List<Properties> GetMailboxInfoTable(Context context, StoreDatabase database, GetMailboxInfoTableFlags flags, StorePropTag[] propTags)
		{
			List<Column> list = MapiMailbox.MapPropTagToColumn(database, propTags);
			MailboxTable mailboxTable = Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(context.Database);
			int num = list.IndexOf(mailboxTable.MailboxNumber);
			int num2 = list.IndexOf(mailboxTable.MailboxGuid);
			if (-1 == num)
			{
				list.Add(mailboxTable.MailboxNumber);
				num = list.IndexOf(mailboxTable.MailboxNumber);
			}
			if (-1 == num2)
			{
				list.Add(mailboxTable.MailboxGuid);
				num2 = list.IndexOf(mailboxTable.MailboxGuid);
			}
			List<Properties> list2 = new List<Properties>(500);
			MailboxList.ListType listType;
			switch (flags)
			{
			case GetMailboxInfoTableFlags.None:
				listType = MailboxList.ListType.ActiveAndDisabled;
				break;
			case GetMailboxInfoTableFlags.IncludeSoftDeleted:
				listType = MailboxList.ListType.ActiveAndDisconnected;
				break;
			case GetMailboxInfoTableFlags.FinalCleanup:
				listType = MailboxList.ListType.FinalCleanup;
				break;
			default:
				throw new StoreException((LID)39736U, ErrorCodeValue.NotSupported);
			}
			List<PrequarantinedMailbox> list3 = null;
			using (MailboxList mailboxList = new MailboxList(context, list.ToArray(), database, listType))
			{
				using (Reader reader = mailboxList.OpenList())
				{
					while (reader.Read())
					{
						int @int = reader.GetInt32(list[num]);
						Guid guid = reader.GetGuid(list[num2]);
						PrequarantinedMailbox prequarantinedMailbox = null;
						Properties item = new Properties(propTags.Length);
						int i = 0;
						while (i < propTags.Length)
						{
							object obj = null;
							switch (propTags[i].PropId)
							{
							case 26650:
								obj = MapiMailbox.GetMailboxQuarantined(context, @int);
								break;
							case 26651:
							case 26652:
							case 26653:
								obj = null;
								if (MapiMailbox.GetMailboxQuarantined(context, @int))
								{
									if (list3 == null)
									{
										list3 = MailboxQuarantineProvider.Instance.GetPreQuarantinedMailboxes(context.Database.MdbGuid);
									}
									if (prequarantinedMailbox == null)
									{
										foreach (PrequarantinedMailbox prequarantinedMailbox2 in list3)
										{
											if (prequarantinedMailbox2.MailboxGuid == guid)
											{
												prequarantinedMailbox = prequarantinedMailbox2;
												break;
											}
										}
									}
								}
								if (prequarantinedMailbox != null)
								{
									if (propTags[i].PropId == 26651)
									{
										obj = prequarantinedMailbox.QuarantineReason;
									}
									else if (propTags[i].PropId == 26652)
									{
										obj = prequarantinedMailbox.LastCrashTime;
									}
									else
									{
										obj = prequarantinedMailbox.LastCrashTime + prequarantinedMailbox.QuarantineDuration;
									}
								}
								break;
							case 26654:
								goto IL_23A;
							case 26655:
								obj = @int;
								break;
							default:
								goto IL_23A;
							}
							IL_261:
							if (obj != null)
							{
								item.Add(propTags[i], obj);
							}
							else
							{
								item.Add(propTags[i].ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
							}
							i++;
							continue;
							IL_23A:
							obj = reader.GetValue(list[i]);
							obj = Mailbox.PostProcessMailboxPropValue(obj, propTags[i]);
							goto IL_261;
						}
						list2.Add(item);
					}
				}
			}
			return list2;
		}

		public bool GetLocalized(MapiContext context)
		{
			bool flag = false;
			if (this.localized != null)
			{
				flag = this.localized.Value;
			}
			else
			{
				object propertyValue = this.storeMailbox.GetPropertyValue(context, PropTag.Mailbox.Localized);
				if (propertyValue != null)
				{
					flag = (bool)propertyValue;
				}
				this.localized = new bool?(flag);
			}
			return flag;
		}

		internal void Connect(MapiContext context)
		{
			this.ThrowIfNotValid();
			if (!this.storeMailbox.IsConnected)
			{
				this.storeMailbox.Reconnect(context);
			}
		}

		public void SaveChanges(MapiContext context)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			this.storeMailbox.Save(context);
		}

		[Conditional("DEBUG")]
		internal void AssertMailboxDisconnected()
		{
			this.ThrowIfNotValid();
		}

		internal StorePropInfo GetNameFromNumber(Context context, ushort propNumber)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			return this.storeMailbox.NamedPropertyMap.GetNameFromNumber(context, propNumber);
		}

		internal ushort GetNumberFromName(MapiContext context, bool create, StorePropName propName, MapiLogon logon)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			ushort result;
			StoreNamedPropInfo storeNamedPropInfo;
			if (!this.storeMailbox.NamedPropertyMap.GetNumberFromName(context, propName, create, this.storeMailbox.QuotaInfo, out result, out storeNamedPropInfo))
			{
				return 0;
			}
			return result;
		}

		public Guid GetGuidFromReplid(MapiContext context, ushort replid)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			Guid guidFromReplid;
			try
			{
				guidFromReplid = this.storeMailbox.ReplidGuidMap.GetGuidFromReplid(context, replid);
			}
			catch (ReplidNotFoundException exception)
			{
				context.OnExceptionCatch(exception);
				throw new ObjectNotFoundException((LID)56120U, this.MailboxGuid, "Replid " + replid + " not found for this mailbox");
			}
			return guidFromReplid;
		}

		public ushort GetReplidFromGuid(MapiContext context, Guid guid)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			return this.storeMailbox.ReplidGuidMap.GetReplidFromGuid(context, guid);
		}

		internal ReceiveFolder GetReceiveFolder(MapiContext context, string messageClass)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			return ReceiveFolder.GetReceiveFolder(context, this.storeMailbox, messageClass);
		}

		internal IList<ReceiveFolder> GetReceiveFolders(MapiContext context)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			return ReceiveFolder.GetReceiveFolders(context, this.storeMailbox);
		}

		internal void SetReceiveFolder(MapiContext context, string messageClass, ExchangeId folderId)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			ReceiveFolder.SetReceiveFolder(context, this.storeMailbox, messageClass, folderId);
		}

		internal CrucialFolderId GetFIDC(MapiContext context)
		{
			this.ThrowIfNotValid();
			this.ThrowIfNotOpen();
			ExchangeId[] specialFolders = SpecialFoldersCache.GetSpecialFolders(context, this.storeMailbox);
			CrucialFolderId fidc = MapiMailbox.GetFIDC(context, specialFolders, this.IsPublicFolderMailbox);
			if (this.storeMailbox.ConversationFolderId.IsNullOrZero)
			{
				this.storeMailbox.ConversationFolderId = fidc.FidConversations;
			}
			return fidc;
		}

		private static MapiFolder CreateFolder(MapiContext context, MapiLogon logon, MapiFolder parentFolder, ref ExchangeId newFid, SpecialFolders specialFolder, string displayName, string comment)
		{
			return MapiMailbox.CreateFolder(context, logon, parentFolder, ref newFid, specialFolder, displayName, comment, FolderConfigureFlags.None);
		}

		private static MapiFolder CreateFolder(MapiContext context, MapiLogon logon, MapiFolder parentFolder, ref ExchangeId newFid, SpecialFolders specialFolder, string displayName, string comment, FolderConfigureFlags folderConfigureFlags)
		{
			MapiFolder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MapiFolder mapiFolder;
				MapiFolder.CreateFolder(context, logon, ref newFid, true, folderConfigureFlags, parentFolder, false, out mapiFolder);
				disposeGuard.Add<MapiFolder>(mapiFolder);
				mapiFolder.StoreFolder.SetSpecialFolderNumber(context, specialFolder);
				StorePropTag[] tags = new StorePropTag[]
				{
					PropTag.Folder.DisplayName,
					PropTag.Folder.Comment
				};
				object[] values = new object[]
				{
					displayName,
					comment
				};
				mapiFolder.InternalSetPropsShouldNotFail(context, new Properties(tags, values));
				disposeGuard.Success();
				result = mapiFolder;
			}
			return result;
		}

		private static Mailbox CreateMailboxPhaseOne(MapiContext context, MailboxState mailboxState, MailboxInfo mailboxInfo, TenantHint tenantHint, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool createdByMove)
		{
			Mailbox mailbox = Mailbox.CreateMailbox(context, mailboxState, mailboxInfo, mailboxInstanceGuid, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange, nextCnCounter, reservedCnCounterRange, numberToNameMap, replidGuidMap, defaultFoldersReplGuid, createdByMove);
			bool flag = false;
			try
			{
				mailbox.SetProperty(context, PropTag.Mailbox.Localized, false);
				mailbox.SetComment(context, string.Empty);
				mailbox.SetProperty(context, PropTag.Mailbox.MessageSize, 0L);
				mailbox.SetProperty(context, PropTag.Mailbox.DeletedMessageSize, 0L);
				mailbox.SetProperty(context, PropTag.Mailbox.DeletedNormalMessageSize, 0L);
				mailbox.SetProperty(context, PropTag.Mailbox.DeletedAssociatedMessageSize, 0L);
				if (mailboxInfo.Type == MailboxInfo.MailboxType.Private)
				{
					Conversations.EnableConversationsForMailbox(context, mailbox);
				}
				mailbox.SetOofState(context, false);
				if (tenantHint.IsEmpty)
				{
					tenantHint = TenantHint.RootOrg;
					mailbox.SharedState.TenantHint = tenantHint;
				}
				mailbox.SetProperty(context, PropTag.Mailbox.InternalTenantHint, tenantHint.TenantHintBlob);
				mailbox.Save(context);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					mailbox.Dispose();
					mailbox = null;
				}
			}
			return mailbox;
		}

		private static void CreateMailboxPhaseTwo(MapiContext context, AddressInfo addressInfoMailbox, MailboxInfo mailboxInfo, DatabaseInfo databaseInfo, Mailbox storeMailbox, CrucialFolderId fidc)
		{
			bool flag = fidc != null;
			if (fidc == null)
			{
				fidc = new CrucialFolderId();
			}
			else
			{
				switch (mailboxInfo.Type)
				{
				case MailboxInfo.MailboxType.Private:
					fidc.AdjustPrivateMailboxReplids(context, storeMailbox.ReplidGuidMap);
					break;
				case MailboxInfo.MailboxType.PublicFolderPrimary:
				case MailboxInfo.MailboxType.PublicFolderSecondary:
					fidc.AdjustPublicFoldersMailboxReplids(context, storeMailbox.ReplidGuidMap);
					break;
				default:
					throw new CorruptDataException((LID)47936U, "Invalid mailbox type.");
				}
			}
			MapiMailbox mapiMailbox = null;
			try
			{
				mapiMailbox = new MapiMailbox(storeMailbox, CultureHelper.GetLcidFromCulture(context.Culture));
				using (MapiLogon mapiLogon = new MapiLogon())
				{
					mapiLogon.ConfigureLogon(context, null, addressInfoMailbox, mailboxInfo, databaseInfo, ref mapiMailbox, OpenStoreFlags.NoLocalization, false, -1);
					switch (mailboxInfo.Type)
					{
					case MailboxInfo.MailboxType.Private:
						MapiMailbox.CreateSystemFoldersForPrivateMailbox(mapiLogon, context, fidc, flag);
						break;
					case MailboxInfo.MailboxType.PublicFolderPrimary:
					case MailboxInfo.MailboxType.PublicFolderSecondary:
						MapiMailbox.CreateSystemFoldersForPublicFolderMailbox(mapiLogon, context, fidc, mailboxInfo, flag);
						break;
					default:
						throw new CorruptDataException((LID)64320U, "Invalid mailbox type.");
					}
					SpecialFoldersCache.Reset(context, storeMailbox);
					MailboxNotificationEvent nev;
					if (flag)
					{
						nev = NotificationEvents.CreateMailboxMoveStartedNotificationEvent(context, storeMailbox, false);
					}
					else
					{
						nev = NotificationEvents.CreateMailboxCreatedNotificationEvent(context, storeMailbox);
					}
					context.RiseNotificationEvent(nev);
					mapiLogon.StoreMailbox.MakeUserAccessible(context);
					mapiLogon.StoreMailbox.Save(context);
				}
			}
			finally
			{
				if (mapiMailbox != null)
				{
					mapiMailbox.Dispose();
					mapiMailbox = null;
				}
			}
		}

		private static void CreateSystemFoldersForPrivateMailbox(MapiLogon logon, MapiContext context, CrucialFolderId fidc, bool forMoveMailbox)
		{
			using (MapiFolder mapiFolder = MapiMailbox.CreateFolder(context, logon, null, ref fidc.FidRoot, SpecialFolders.MailboxRoot, string.Empty, string.Empty))
			{
				using (MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidDAF, SpecialFolders.DeferredAction, "Deferred Action", string.Empty))
				{
				}
				using (MapiFolder mapiFolder3 = MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidSpoolerQ, SpecialFolders.SpoolerQueue, "Spooler Queue", string.Empty, FolderConfigureFlags.CreateSearchFolder))
				{
					Restriction restriction = new RestrictionAND(new Restriction[]
					{
						new RestrictionBitmask(PropTag.Message.MessageFlagsActual, 4L, BitmaskOperation.NotEqualToZero),
						new RestrictionBitmask(PropTag.Message.SubmitResponsibility, 1L, BitmaskOperation.EqualToZero)
					});
					mapiFolder3.SetSearchCriteria(context, restriction.Serialize(), new ExchangeId[]
					{
						fidc.FidRoot
					}, SetSearchCriteriaFlags.Recursive);
				}
				using (MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidShortcuts, SpecialFolders.Shortcuts, "Shortcuts", string.Empty, FolderConfigureFlags.CreateLastWriterWinsFolder))
				{
				}
				using (MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidFinder, SpecialFolders.Finder, "Finder", string.Empty))
				{
				}
				using (MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidViews, SpecialFolders.Views, "Views", string.Empty))
				{
				}
				using (MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidCommonViews, SpecialFolders.CommonViews, "Common Views", string.Empty))
				{
				}
				using (MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidSchedule, SpecialFolders.Schedule, "Schedule", string.Empty))
				{
				}
				using (MapiFolder mapiFolder9 = MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidIPMsubtree, SpecialFolders.TopofInformationStore, "Top of Information Store", string.Empty, FolderConfigureFlags.CreateIpmFolder))
				{
					using (MapiFolder mapiFolder10 = MapiMailbox.CreateFolder(context, logon, mapiFolder9, ref fidc.FidSentmail, SpecialFolders.SentItems, "Sent Items", string.Empty))
					{
						StorePropTag[] tags = new StorePropTag[]
						{
							PropTag.Folder.ContainerClass
						};
						object[] values = new object[]
						{
							"IPF.Note"
						};
						mapiFolder10.InternalSetPropsShouldNotFail(context, new Properties(tags, values));
					}
					using (MapiFolder mapiFolder11 = MapiMailbox.CreateFolder(context, logon, mapiFolder9, ref fidc.FidWastebasket, SpecialFolders.DeletedItems, "Deleted Items", string.Empty))
					{
						StorePropTag[] tags2 = new StorePropTag[]
						{
							PropTag.Folder.ContainerClass
						};
						object[] values2 = new object[]
						{
							"IPF.Note"
						};
						mapiFolder11.InternalSetPropsShouldNotFail(context, new Properties(tags2, values2));
					}
					using (MapiFolder mapiFolder12 = MapiMailbox.CreateFolder(context, logon, mapiFolder9, ref fidc.FidOutbox, SpecialFolders.Outbox, "Outbox", string.Empty))
					{
						StorePropTag[] tags3 = new StorePropTag[]
						{
							PropTag.Folder.ContainerClass
						};
						object[] values3 = new object[]
						{
							"IPF.Note"
						};
						mapiFolder12.InternalSetPropsShouldNotFail(context, new Properties(tags3, values3));
					}
					using (MapiFolder mapiFolder13 = MapiMailbox.CreateFolder(context, logon, mapiFolder9, ref fidc.FidInbox, SpecialFolders.Inbox, "Inbox", string.Empty))
					{
						StorePropTag[] tags4 = new StorePropTag[]
						{
							PropTag.Folder.ContainerClass
						};
						object[] values4 = new object[]
						{
							"IPF.Note"
						};
						mapiFolder13.InternalSetPropsShouldNotFail(context, new Properties(tags4, values4));
						if (!forMoveMailbox)
						{
							MapiMailbox.FolderCruft[] array = new MapiMailbox.FolderCruft[]
							{
								new MapiMailbox.FolderCruft("Calendar", "IPF.Appointment", PropTag.Folder.IPMAppointmentEntryId),
								new MapiMailbox.FolderCruft("Contacts", "IPF.Contact", PropTag.Folder.IPMContactEntryId),
								new MapiMailbox.FolderCruft("Drafts", "IPF.Note", PropTag.Folder.IPMDraftsEntryId),
								new MapiMailbox.FolderCruft("Journal", "IPF.Journal", PropTag.Folder.IPMJournalEntryId),
								new MapiMailbox.FolderCruft("Notes", "IPF.StickyNote", PropTag.Folder.IPMNoteEntryId),
								new MapiMailbox.FolderCruft("Tasks", "IPF.Task", PropTag.Folder.IPMTaskEntryId)
							};
							for (int i = 0; i < array.Length; i++)
							{
								ExchangeId zero = ExchangeId.Zero;
								using (MapiFolder mapiFolder14 = MapiMailbox.CreateFolder(context, logon, mapiFolder9, ref zero, SpecialFolders.Regular, array[i].FolderName, string.Empty))
								{
									StorePropTag[] tags5 = new StorePropTag[]
									{
										PropTag.Folder.ContainerClass
									};
									object[] values5 = new object[]
									{
										array[i].MsgClass
									};
									mapiFolder14.InternalSetPropsShouldNotFail(context, new Properties(tags5, values5));
								}
								StorePropTag[] tags6 = new StorePropTag[]
								{
									array[i].PtagFolderEid
								};
								object[] values6 = new object[]
								{
									EntryIdHelpers.ExchangeIdTo46ByteEntryId(zero, logon.MapiMailbox.MailboxInstanceGuid, EntryIdHelpers.EIDType.eitLTPrivateFolder)
								};
								mapiFolder.InternalSetPropsShouldNotFail(context, new Properties(tags6, values6));
								mapiFolder13.InternalSetPropsShouldNotFail(context, new Properties(tags6, values6));
							}
						}
					}
				}
			}
			ReceiveFolder.SetReceiveFolder(context, logon.StoreMailbox, string.Empty, fidc.FidInbox);
			ReceiveFolder.SetReceiveFolder(context, logon.StoreMailbox, "IPM", fidc.FidInbox);
			ReceiveFolder.SetReceiveFolder(context, logon.StoreMailbox, "REPORT.IPM", fidc.FidInbox);
			ReceiveFolder.SetReceiveFolder(context, logon.StoreMailbox, "IPC", fidc.FidRoot);
		}

		private static void CreateSystemFoldersForPublicFolderMailbox(MapiLogon logon, MapiContext context, CrucialFolderId fidc, MailboxInfo mailboxInfo, bool forMoveMailbox)
		{
			using (MapiFolder mapiFolder = MapiMailbox.CreateFolder(context, logon, null, ref fidc.FidRoot, SpecialFolders.MailboxRoot, string.Empty, string.Empty))
			{
				using (MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidPublicFolderIpmSubTree, SpecialFolders.DeferredAction, "IPM_SUBTREE", string.Empty, FolderConfigureFlags.CreateIpmFolder))
				{
				}
				using (MapiFolder mapiFolder3 = MapiMailbox.CreateFolder(context, logon, mapiFolder, ref fidc.FidPublicFolderNonIpmSubTree, SpecialFolders.SpoolerQueue, "NON_IPM_SUBTREE", string.Empty))
				{
					using (MapiMailbox.CreateFolder(context, logon, mapiFolder3, ref fidc.FidPublicFolderEFormsRegistry, SpecialFolders.TopofInformationStore, "EFORMS REGISTRY", string.Empty))
					{
					}
					using (MapiMailbox.CreateFolder(context, logon, mapiFolder3, ref fidc.FidPublicFolderDumpsterRoot, SpecialFolders.DeletedItems, "DUMPSTER_ROOT", string.Empty))
					{
					}
					using (MapiMailbox.CreateFolder(context, logon, mapiFolder3, ref fidc.FidPublicFolderTombstonesRoot, SpecialFolders.Finder, "TOMBSTONES_ROOT", string.Empty))
					{
					}
					using (MapiMailbox.CreateFolder(context, logon, mapiFolder3, ref fidc.FidPublicFolderAsyncDeleteState, SpecialFolders.SentItems, "AsyncDeleteState", string.Empty))
					{
					}
					using (MapiFolder mapiFolder8 = MapiMailbox.CreateFolder(context, logon, mapiFolder3, ref fidc.FidPublicFolderInternalSubmission, SpecialFolders.Outbox, "InternalSubmission", string.Empty))
					{
						Properties properties = new Properties(1)
						{
							{
								PropTag.Folder.AclTableAndSecurityDescriptor,
								FolderSecurity.AclTableAndSecurityDescriptorProperty.GetDefaultBlobForInternalSubmissionPublicFolder()
							}
						};
						mapiFolder8.InternalSetPropsShouldNotFail(context, properties);
					}
				}
			}
			if (!forMoveMailbox && logon.MailboxInfo.Type == MailboxInfo.MailboxType.PublicFolderPrimary)
			{
				using (MapiFolder mapiFolder9 = MapiFolder.OpenFolder(context, logon, fidc.FidRoot))
				{
					using (MapiFolder mapiFolder10 = MapiFolder.OpenFolder(context, logon, fidc.FidPublicFolderIpmSubTree))
					{
						using (MapiFolder mapiFolder11 = MapiFolder.OpenFolder(context, logon, fidc.FidPublicFolderNonIpmSubTree))
						{
							using (MapiFolder mapiFolder12 = MapiFolder.OpenFolder(context, logon, fidc.FidPublicFolderEFormsRegistry))
							{
								using (MapiFolder mapiFolder13 = MapiFolder.OpenFolder(context, logon, fidc.FidPublicFolderDumpsterRoot))
								{
									using (MapiFolder mapiFolder14 = MapiFolder.OpenFolder(context, logon, fidc.FidPublicFolderTombstonesRoot))
									{
										using (MapiFolder mapiFolder15 = MapiFolder.OpenFolder(context, logon, fidc.FidPublicFolderAsyncDeleteState))
										{
											using (MapiFolder mapiFolder16 = MapiFolder.OpenFolder(context, logon, fidc.FidPublicFolderInternalSubmission))
											{
												if (mapiFolder9 == null || mapiFolder10 == null || mapiFolder11 == null || mapiFolder12 == null || mapiFolder13 == null || mapiFolder14 == null || mapiFolder15 == null || mapiFolder16 == null)
												{
													throw new StoreException((LID)60032U, ErrorCodeValue.NotFound);
												}
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder9);
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder10);
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder11);
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder12);
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder13);
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder14);
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder15);
												MapiFolder.CreateAssociatedDumpsterFolder(context, logon, mapiFolder13, mapiFolder16);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static List<Column> MapPropTagToColumn(StoreDatabase database, StorePropTag[] propTags)
		{
			List<Column> list = new List<Column>(15);
			for (int i = 0; i < propTags.Length; i++)
			{
				if (propTags[i].PropType == PropertyType.Unspecified)
				{
					throw new ExExceptionInvalidParameter((LID)62264U, "PT_UNSPECIFIED property is not supported in mailbox list");
				}
				Column item = PropertySchema.MapToColumn(database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Mailbox, propTags[i]);
				list.Add(item);
			}
			return list;
		}

		private static bool GetMailboxQuarantined(Context context, int mailboxNumber)
		{
			MailboxState mailboxState = MailboxStateCache.Get(context, mailboxNumber);
			return mailboxState != null && mailboxState.Quarantined;
		}

		private void OpenMailbox(MapiContext context, MailboxInfo.MailboxType mailboxType)
		{
			bool flag = false;
			this.ThrowIfNotValid();
			try
			{
				this.storeMailbox = Mailbox.OpenMailbox(context, this.sharedState);
				if (this.storeMailbox == null)
				{
					throw new StoreException((LID)49976U, ErrorCodeValue.LogonFailed);
				}
				if ((this.IsPublicFolderMailbox && mailboxType == MailboxInfo.MailboxType.Private) || (!this.IsPublicFolderMailbox && mailboxType != MailboxInfo.MailboxType.Private))
				{
					throw new StoreException((LID)49256U, ErrorCodeValue.LogonFailed, "Mismatching mailbox types!");
				}
				bool flag2 = this.GetLocalized(context);
				if (flag2)
				{
					this.lcid = this.storeMailbox.GetLCID(context);
				}
				flag = true;
			}
			finally
			{
				if (!flag && this.storeMailbox != null)
				{
					this.storeMailbox.Dispose();
					this.storeMailbox = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiMailbox>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			try
			{
				if (calledFromDispose && this.storeMailbox != null)
				{
					this.storeMailbox.Dispose();
					this.storeMailbox = null;
				}
			}
			finally
			{
				this.storeMailbox = null;
				this.valid = false;
			}
		}

		private void ThrowIfNotValid()
		{
			if (!this.valid)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Invalid MapiMailbox object!");
				throw new ExExceptionInvalidObject((LID)60472U, "Invalid MapiMailbox object!");
			}
		}

		private void ThrowIfNotOpen()
		{
			if (this.storeMailbox == null)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "MapiMailbox is not open!");
				throw new ExExceptionInvalidObject((LID)54072U, "MapiMailbox is not open!");
			}
		}

		public const string IpmSubtreeDisplayName = "IPM_SUBTREE";

		public const string IpmSubtreePathName = "\\IPM_SUBTREE";

		private const int AverageMailboxCount = 500;

		private const int AverageColumnCount = 15;

		private static Hookable<Action> staticTestHook = Hookable<Action>.Create(true, null);

		private bool valid;

		private StoreDatabase database;

		private MailboxState sharedState;

		private Mailbox storeMailbox;

		private int lcid;

		private bool? localized;

		[Flags]
		public enum BehaviorFlags : uint
		{
			None = 0U,
			ForCreate = 1U
		}

		private struct FolderCruft
		{
			internal FolderCruft(string folderName, string msgClass, StorePropTag ptagFolderEid)
			{
				this.FolderName = folderName;
				this.MsgClass = msgClass;
				this.PtagFolderEid = ptagFolderEid;
			}

			public string FolderName;

			public string MsgClass;

			public StorePropTag PtagFolderEid;
		}
	}
}
