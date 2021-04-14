using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.MapiDisp;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreIntegrityCheck;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class SimpleQueryTargets
	{
		private SimpleQueryTargets()
		{
		}

		public static SimpleQueryTargets Instance
		{
			get
			{
				if (SimpleQueryTargets.instance == null)
				{
					SimpleQueryTargets.instance = new SimpleQueryTargets();
				}
				return SimpleQueryTargets.instance;
			}
		}

		public static void Initialize()
		{
			SimpleQueryTargets.Instance.Register<QueryableProperty>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<QueryableProperty>("ParsePropertyBlob", new Type[]
			{
				typeof(byte[])
			}, delegate(object[] parameters)
			{
				byte[] array = parameters[0] as byte[];
				if (array != null)
				{
					IList<QueryableProperty> list = new List<QueryableProperty>(100);
					PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(array, 0);
					for (int i = 0; i < blobReader.PropertyCount; i++)
					{
						StorePropTag tag = StorePropTag.CreateWithoutInfo(blobReader.GetPropertyTag(i));
						object propertyValue = blobReader.GetPropertyValue(i);
						Property property = new Property(tag, propertyValue);
						list.Add(QueryableProperty.Create(property.Tag.ToString(), property.Tag.PropName.ToString(), property.Tag.PropType.ToString(), property.Value));
					}
					return list;
				}
				return new QueryableProperty[0];
			}), Visibility.Redacted);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<byte[]>>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<byte[]>>("ParseMVBinaryBlob", new Type[]
			{
				typeof(byte[])
			}, delegate(object[] parameters)
			{
				byte[] array = parameters[0] as byte[];
				if (array != null)
				{
					int num = 0;
					byte[][] array2 = SerializedValue.ParseMVBinary(array, ref num);
					if (array2 != null)
					{
						List<SimpleQueryTargets.QueryableValue<byte[]>> list = new List<SimpleQueryTargets.QueryableValue<byte[]>>(array2.Length);
						foreach (byte[] value in array2)
						{
							list.Add(new SimpleQueryTargets.QueryableValue<byte[]>(value));
						}
						return list;
					}
				}
				return new SimpleQueryTargets.QueryableValue<byte[]>[0];
			}), Visibility.Redacted);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<object>>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<object>>("ParseValueBlob", new Type[]
			{
				typeof(byte[])
			}, delegate(object[] parameters)
			{
				byte[] array = parameters[0] as byte[];
				if (array != null)
				{
					int num = 0;
					IList<object> list = SerializedValue.ParseList(array, ref num);
					List<SimpleQueryTargets.QueryableValue<object>> list2 = new List<SimpleQueryTargets.QueryableValue<object>>(list.Count);
					foreach (object value in list)
					{
						list2.Add(new SimpleQueryTargets.QueryableValue<object>(value));
					}
					return list2;
				}
				return new SimpleQueryTargets.QueryableValue<object>[0];
			}), Visibility.Redacted);
			SimpleQueryTargets.Instance.Register<Breadcrumb>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<Breadcrumb>("Breadcrumbs", new Func<IEnumerable<Breadcrumb>>(ErrorHelper.GetBreadcrumbsHistorySnapshot)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<QueryableSession>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<QueryableSession>("Session", delegate()
			{
				IList<QueryableSession> sessions = new List<QueryableSession>(10);
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.ForEachSession(delegate(MapiSession mapiSession, Func<bool> shouldCallbackContinue)
				{
					sessions.Add(QueryableSession.Create(mapiSession));
				});
				return sessions;
			}), Visibility.Redacted);
			SimpleQueryTargets.Instance.Register<QueryableSessionsPerService>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<QueryableSessionsPerService>("SessionsPerService", delegate()
			{
				QueryableSessionsPerService[] array = new QueryableSessionsPerService[9];
				for (int i = 0; i <= 8; i++)
				{
					MapiServiceType mapiServiceType = (MapiServiceType)i;
					array[i] = new QueryableSessionsPerService(mapiServiceType, MapiSessionPerServiceCounter.GetObjectCounter(mapiServiceType).GetCount());
				}
				return array;
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<QueryableSessionsPerUser>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<QueryableSessionsPerUser>("SessionsPerUser", delegate()
			{
				ClientType[] array = new ClientType[]
				{
					ClientType.AirSync,
					ClientType.OWA,
					ClientType.Pop,
					ClientType.Imap,
					ClientType.UnifiedMessaging,
					ClientType.WebServices,
					ClientType.ELC,
					ClientType.Administrator,
					ClientType.MoMT
				};
				IList<SecurityIdentifier> usersSnapshot = MapiSessionPerUserCounter.GetUsersSnapshot();
				List<QueryableSessionsPerUser> list = new List<QueryableSessionsPerUser>(usersSnapshot.Count * 42);
				foreach (SecurityIdentifier userSid in usersSnapshot)
				{
					foreach (ClientType clientType in array)
					{
						long count = MapiSessionPerUserCounter.GetCount(userSid, clientType);
						if (count > 0L)
						{
							list.Add(new QueryableSessionsPerUser(userSid, clientType, count));
						}
					}
				}
				return list;
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<string>>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<string>>("ClusterDBSubkeys", new Type[]
			{
				typeof(string)
			}, new Func<object[], IEnumerable<SimpleQueryTargets.QueryableValue<string>>>(SimpleQueryTargets.ClusterDBSubkeys)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<KeyValuePair<string, object>>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<KeyValuePair<string, object>>("ClusterDBValues", new Type[]
			{
				typeof(string)
			}, new Func<object[], IEnumerable<KeyValuePair<string, object>>>(SimpleQueryTargets.ClusterDBValues)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<int>>(new SimpleQueryTargets.SingleRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<int>>("HashConversationId", new Type[]
			{
				typeof(byte[])
			}, delegate(object[] parameters)
			{
				byte[] inputBytes = parameters[0] as byte[];
				int conversationIdHash = HashHelpers.GetConversationIdHash(inputBytes);
				return new SimpleQueryTargets.QueryableValue<int>(conversationIdHash);
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<int>>(new SimpleQueryTargets.SingleRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<int>>("HashInternetMessageId", new Type[]
			{
				typeof(string)
			}, delegate(object[] parameters)
			{
				string input = parameters[0] as string;
				int internetMessageIdHash = HashHelpers.GetInternetMessageIdHash(input);
				return new SimpleQueryTargets.QueryableValue<int>(internetMessageIdHash);
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<int>>(new SimpleQueryTargets.SingleRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<int>>("HashConversationTopic", new Type[]
			{
				typeof(string)
			}, delegate(object[] parameters)
			{
				string input = parameters[0] as string;
				int conversationTopicHash = HashHelpers.GetConversationTopicHash(input);
				return new SimpleQueryTargets.QueryableValue<int>(conversationTopicHash);
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<long>>(new SimpleQueryTargets.SingleRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<long>>("HashDeliveredTo", new Type[]
			{
				typeof(string),
				typeof(byte[])
			}, delegate(object[] parameters)
			{
				string messageId = parameters[0] as string;
				byte[] array = parameters[1] as byte[];
				ExchangeId folderId = ExchangeId.Null;
				if (array != null)
				{
					if (array.Length != 26)
					{
						throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidFolderId());
					}
					using (Context context = StoreQueryRetriever.StoreQueryContext.Create())
					{
						folderId = ExchangeId.CreateFrom26ByteArray(context, null, array);
					}
				}
				long deliveredToHash = DeliveredTo.GetDeliveredToHash(messageId, folderId);
				return new SimpleQueryTargets.QueryableValue<long>(deliveredToHash);
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<IdSet>>(new SimpleQueryTargets.SingleRowSimpleQueryTarget<SimpleQueryTargets.QueryableValue<IdSet>>("ParseIdSetBlob", new Type[]
			{
				typeof(byte[])
			}, delegate(object[] parameters)
			{
				byte[] array = parameters[0] as byte[];
				if (array == null)
				{
					return new SimpleQueryTargets.QueryableValue<IdSet>();
				}
				SimpleQueryTargets.QueryableValue<IdSet> result;
				using (Context context = StoreQueryRetriever.StoreQueryContext.Create())
				{
					IdSet value = IdSet.ThrowableParse(context, array);
					result = new SimpleQueryTargets.QueryableValue<IdSet>(value);
				}
				return result;
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<DefaultSettings.DefaultSettingsValues>(new SimpleQueryTargets.SingleRowSimpleQueryTarget<DefaultSettings.DefaultSettingsValues>("DefaultSettings", () => DefaultSettings.Get), Visibility.Public);
			SimpleQueryTargets.Instance.Register<ProcessorCollection.QueryableProcessor>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<ProcessorCollection.QueryableProcessor>("Processors", new Func<IEnumerable<ProcessorCollection.QueryableProcessor>>(ProcessorCollection.GetCollection)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<QueryableActiveSetting>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<QueryableActiveSetting>("ActiveSettings", Array<Type>.Empty, new Func<object[], IEnumerable<QueryableActiveSetting>>(SimpleQueryTargets.ActiveSettings)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<VirtualColumnDefinition>(new SimpleQueryTargets.MultiRowSimpleQueryTarget<VirtualColumnDefinition>("VirtualColumns", new Func<IEnumerable<VirtualColumnDefinition>>(Factory.GetSupportedVirtualColumns)), Visibility.Public);
		}

		public static void MountEventHandler(StoreDatabase database)
		{
			SimpleQueryTargets.Instance.Register<QueryableInTransitInfo>(database, new SimpleQueryTargets.MultiRowDatabaseQueryTarget<QueryableInTransitInfo>("InTransitInfo", (Context context) => SimpleQueryTargets.ExecuteForAllMailboxes<QueryableInTransitInfo>(context, delegate(MailboxState state)
			{
				InTransitStatus inTransitStatus = InTransitInfo.GetInTransitStatus(state);
				if (inTransitStatus != InTransitStatus.NotInTransit)
				{
					List<object> inTransitClientHandles = InTransitInfo.GetInTransitClientHandles(state);
					return new QueryableInTransitInfo(state, inTransitStatus, inTransitClientHandles);
				}
				return null;
			})), Visibility.Public);
			SimpleQueryTargets.Instance.Register<FullTextDiagnosticRow>(database, new SimpleQueryTargets.MultiRowDatabaseQueryTarget<FullTextDiagnosticRow>("FullTextQuery", new Type[]
			{
				typeof(Guid),
				typeof(int),
				typeof(string)
			}, delegate(Context context, object[] parameters)
			{
				Guid mailboxGuid = (Guid)parameters[0];
				int mailboxNumber = (int)parameters[1];
				string text = parameters[2] as string;
				if (!string.IsNullOrEmpty(text))
				{
					IFullTextIndexQuery fullTextIndexQuery = SimpleQueryTargets.FullTextIndexQueryCreator.Value();
					try
					{
						return fullTextIndexQuery.ExecuteDiagnosticQuery(context.Database.MdbGuid, mailboxGuid, mailboxNumber, text, CultureInfo.InvariantCulture, Guid.NewGuid(), "-received", FullTextDiagnosticRow.FastColumns, null);
					}
					catch (FullTextIndexException ex)
					{
						context.OnExceptionCatch(ex);
						throw new DiagnosticQueryException(DiagnosticQueryStrings.FaultExecutingFullTextQuery(ex));
					}
					catch (CommunicationException ex2)
					{
						context.OnExceptionCatch(ex2);
						throw new DiagnosticQueryException(DiagnosticQueryStrings.FaultExecutingFullTextQuery(ex2));
					}
					catch (TimeoutException ex3)
					{
						context.OnExceptionCatch(ex3);
						throw new DiagnosticQueryException(DiagnosticQueryStrings.FaultExecutingFullTextQuery(ex3));
					}
				}
				return new FullTextDiagnosticRow[0];
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<QueryableEntryId>(database, new SimpleQueryTargets.SingleRowDatabaseQueryTarget<QueryableEntryId>("ParseEntryId", new Type[]
			{
				typeof(int),
				typeof(byte[])
			}, delegate(Context context, object[] parameters)
			{
				SimpleQueryTargets.<>c__DisplayClass39 CS$<>8__locals1 = new SimpleQueryTargets.<>c__DisplayClass39();
				CS$<>8__locals1.context = context;
				int mailboxNumber = (int)parameters[0];
				CS$<>8__locals1.entryId = (parameters[1] as byte[]);
				if (CS$<>8__locals1.entryId == null || (CS$<>8__locals1.entryId.Length != 46 && CS$<>8__locals1.entryId.Length != 70))
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidEntryIdFormat());
				}
				int offset = 4;
				Guid mailboxInstanceGuid = ParseSerialize.GetGuid(CS$<>8__locals1.entryId, ref offset, CS$<>8__locals1.entryId.Length);
				EntryIdHelpers.EIDType eidType = (EntryIdHelpers.EIDType)CS$<>8__locals1.entryId[offset++];
				offset++;
				if ((CS$<>8__locals1.entryId.Length == 46 && eidType == EntryIdHelpers.EIDType.eitLTPrivateFolder) || (CS$<>8__locals1.entryId.Length == 70 && eidType == EntryIdHelpers.EIDType.eitLTPrivateMessage))
				{
					return SimpleQueryTargets.ExecuteForSingleMailbox<QueryableEntryId>(CS$<>8__locals1.context, mailboxNumber, delegate(MailboxState state)
					{
						ReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(CS$<>8__locals1.context, state);
						ExchangeId folderId = ExchangeId.CreateFrom22ByteArray(CS$<>8__locals1.context, cacheForMailbox, CS$<>8__locals1.entryId, offset);
						offset += 24;
						ExchangeId messageId = ExchangeId.Null;
						if (CS$<>8__locals1.entryId.Length == 70)
						{
							messageId = ExchangeId.CreateFrom22ByteArray(CS$<>8__locals1.context, cacheForMailbox, CS$<>8__locals1.entryId, offset);
							offset += 24;
						}
						return new QueryableEntryId(mailboxInstanceGuid, eidType, folderId, messageId);
					});
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.UnableToParseEntryId());
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<QueryableExchangeId>(database, new SimpleQueryTargets.SingleRowDatabaseQueryTarget<QueryableExchangeId>("ParseExchangeIdBinary", new Type[]
			{
				typeof(int),
				typeof(byte[])
			}, delegate(Context context, object[] parameters)
			{
				int mailboxNumber = (int)parameters[0];
				byte[] idBytes = parameters[1] as byte[];
				if (idBytes != null && (idBytes.Length == 8 || idBytes.Length == 9 || idBytes.Length == 22 || idBytes.Length == 24 || idBytes.Length == 26))
				{
					return SimpleQueryTargets.ExecuteForSingleMailbox<QueryableExchangeId>(context, mailboxNumber, delegate(MailboxState state)
					{
						ReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, state);
						int num = idBytes.Length;
						switch (num)
						{
						case 8:
							return new QueryableExchangeId(ExchangeId.CreateFrom8ByteArray(context, cacheForMailbox, idBytes));
						case 9:
							return new QueryableExchangeId(ExchangeId.CreateFrom9ByteArray(context, cacheForMailbox, idBytes));
						default:
							switch (num)
							{
							case 22:
								return new QueryableExchangeId(ExchangeId.CreateFrom22ByteArray(context, cacheForMailbox, idBytes));
							case 24:
								return new QueryableExchangeId(ExchangeId.CreateFrom24ByteArray(context, cacheForMailbox, idBytes));
							case 26:
								return new QueryableExchangeId(ExchangeId.CreateFrom26ByteArray(context, cacheForMailbox, idBytes));
							}
							throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdBinaryFormat());
						}
					});
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdBinaryFormat());
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<QueryableExchangeId>(database, new SimpleQueryTargets.SingleRowDatabaseQueryTarget<QueryableExchangeId>("ParseExchangeIdLong", new Type[]
			{
				typeof(int),
				typeof(long)
			}, delegate(Context context, object[] parameters)
			{
				int mailboxNumber = (int)parameters[0];
				long idLong = (long)parameters[1];
				return SimpleQueryTargets.ExecuteForSingleMailbox<QueryableExchangeId>(context, mailboxNumber, delegate(MailboxState state)
				{
					ReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, state);
					return new QueryableExchangeId(ExchangeId.CreateFromInt64(context, cacheForMailbox, idLong));
				});
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<QueryableExchangeId>(database, new SimpleQueryTargets.SingleRowDatabaseQueryTarget<QueryableExchangeId>("ParseExchangeIdString", new Type[]
			{
				typeof(int),
				typeof(string)
			}, delegate(Context context, object[] parameters)
			{
				int mailboxNumber = (int)parameters[0];
				string text = (string)parameters[1];
				if (string.IsNullOrEmpty(text))
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdStringFormat());
				}
				int num = text.LastIndexOf('-');
				if (num < 0)
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdStringFormat());
				}
				string[] array = text.Substring(0, num).Split(new char[]
				{
					'[',
					']'
				}, StringSplitOptions.RemoveEmptyEntries);
				string s = text.Substring(num + 1);
				ulong globCnt;
				if (!ulong.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out globCnt))
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdStringFormat());
				}
				bool flag = false;
				Guid empty = Guid.Empty;
				ushort replid;
				if (array.Length == 1)
				{
					if (!ushort.TryParse(array[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out replid))
					{
						throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdStringFormat());
					}
				}
				else
				{
					if (array.Length != 2)
					{
						throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdStringFormat());
					}
					flag = true;
					if (!Guid.TryParse(array[0], out empty))
					{
						throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdStringFormat());
					}
					if (!ushort.TryParse(array[1], out replid))
					{
						throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdStringFormat());
					}
				}
				if (flag)
				{
					return new QueryableExchangeId(ExchangeId.Create(empty, globCnt, replid));
				}
				return SimpleQueryTargets.ExecuteForSingleMailbox<QueryableExchangeId>(context, mailboxNumber, delegate(MailboxState state)
				{
					ReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, state);
					byte[] array2 = new byte[6];
					ExchangeIdHelpers.GlobcntIntoByteArray(globCnt, array2, 0);
					return new QueryableExchangeId(ExchangeId.Create(context, cacheForMailbox, replid, array2));
				});
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<byte[]>>(database, new SimpleQueryTargets.MultiRowDatabaseQueryTarget<SimpleQueryTargets.QueryableValue<byte[]>>("ParseExchangeIdList", new Type[]
			{
				typeof(int),
				typeof(byte[])
			}, delegate(Context context, object[] parameters)
			{
				int mailboxNumber = (int)parameters[0];
				byte[] idListBytes = parameters[1] as byte[];
				if (idListBytes != null && idListBytes.Length > 0)
				{
					int position = 0;
					return SimpleQueryTargets.ExecuteForSingleMailbox<IEnumerable<SimpleQueryTargets.QueryableValue<byte[]>>>(context, mailboxNumber, delegate(MailboxState state)
					{
						ReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, state);
						IList<ExchangeId> list = ExchangeIdListHelpers.ListFromBytes(context, cacheForMailbox, idListBytes, ref position);
						SimpleQueryTargets.QueryableValue<byte[]>[] array = new SimpleQueryTargets.QueryableValue<byte[]>[list.Count];
						for (int i = 0; i < list.Count; i++)
						{
							array[i] = new SimpleQueryTargets.QueryableValue<byte[]>(list[i].To26ByteArray());
						}
						return array;
					});
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidExchangeIdListFormat());
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<string>>(database, new SimpleQueryTargets.SingleRowDatabaseQueryTarget<SimpleQueryTargets.QueryableValue<string>>("ParseRestriction", new Type[]
			{
				typeof(int),
				typeof(byte[])
			}, delegate(Context context, object[] parameters)
			{
				int mailboxNumber = (int)parameters[0];
				byte[] restrictionBytes = parameters[1] as byte[];
				if (restrictionBytes != null && restrictionBytes.Length > 0)
				{
					return SimpleQueryTargets.ExecuteForSingleMailbox<SimpleQueryTargets.QueryableValue<string>>(context, mailboxNumber, delegate(MailboxState state)
					{
						SimpleQueryTargets.QueryableValue<string> result;
						using (Mailbox mailbox = Mailbox.OpenMailbox(context, state))
						{
							if (mailbox == null)
							{
								throw new DiagnosticQueryException(DiagnosticQueryStrings.UnableToOpenMailbox(mailboxNumber));
							}
							Restriction restriction = Restriction.Deserialize(context, restrictionBytes, mailbox, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
							result = new SimpleQueryTargets.QueryableValue<string>(restriction.ToString());
						}
						return result;
					});
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidRestrictionFormat());
			}), Visibility.Redacted);
			SimpleQueryTargets.Instance.Register<QueryableCategorizationInfo>(database, new SimpleQueryTargets.SingleRowDatabaseQueryTarget<QueryableCategorizationInfo>("ParseCategorizationInfo", new Type[]
			{
				typeof(int),
				typeof(byte[])
			}, delegate(Context context, object[] parameters)
			{
				int mailboxNumber = (int)parameters[0];
				byte[] categorizationInfoBlob = parameters[1] as byte[];
				if (categorizationInfoBlob != null && categorizationInfoBlob.Length > 0)
				{
					return SimpleQueryTargets.ExecuteForSingleMailbox<QueryableCategorizationInfo>(context, mailboxNumber, delegate(MailboxState state)
					{
						QueryableCategorizationInfo result;
						using (Mailbox mailbox = Mailbox.OpenMailbox(context, state))
						{
							if (mailbox == null)
							{
								throw new DiagnosticQueryException(DiagnosticQueryStrings.UnableToOpenMailbox(mailboxNumber));
							}
							CategorizationInfo categorizationInfo = CategorizationInfo.Deserialize(categorizationInfoBlob, delegate(int serializedColumnId, string serializedColumnName)
							{
								Column result2;
								if (serializedColumnId != 0)
								{
									StorePropTag storePropTag = mailbox.GetStorePropTag(context, (uint)serializedColumnId, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message);
									result2 = PropertySchema.MapToColumn(context.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Message, storePropTag);
								}
								else
								{
									result2 = Factory.CreatePhysicalColumn(serializedColumnName, serializedColumnName, typeof(int), false, false, false, false, Visibility.Public, 0, 4, 4);
								}
								return result2;
							});
							result = new QueryableCategorizationInfo(categorizationInfo);
						}
						return result;
					});
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidCategorizationInfoFormat());
			}), Visibility.Public);
			SimpleQueryTargets.Instance.Register<IntegrityCheckJob>(database, new SimpleQueryTargets.MultiRowDatabaseQueryTarget<IntegrityCheckJob>("IsIntegJobQueue", new Func<Context, IEnumerable<IntegrityCheckJob>>(InMemoryJobStorage.GetRequestQueueSnapshot)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<MaintenanceHandler.QueryableMaintenanceState>(database, new SimpleQueryTargets.MultiRowDatabaseQueryTarget<MaintenanceHandler.QueryableMaintenanceState>("DatabaseMaintenance", new Func<Context, IEnumerable<MaintenanceHandler.QueryableMaintenanceState>>(MaintenanceHandler.GetDatabaseMaintenanceSnapshot)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<MaintenanceHandler.QueryableMailboxMaintenanceState>(database, new SimpleQueryTargets.MultiRowDatabaseQueryTarget<MaintenanceHandler.QueryableMailboxMaintenanceState>("MailboxMaintenance", new Func<Context, IEnumerable<MaintenanceHandler.QueryableMailboxMaintenanceState>>(MaintenanceHandler.GetMailboxMaintenanceSnapshot)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<AssistantActivityState>(database, new SimpleQueryTargets.MultiRowDatabaseQueryTarget<AssistantActivityState>("MaintenanceAssistant", new Func<Context, IEnumerable<AssistantActivityState>>(AssistantActivityMonitor.GetAssistantActivitySnapshot)), Visibility.Public);
			SimpleQueryTargets.Instance.Register<SimpleQueryTargets.QueryableValue<string>>(database, new SimpleQueryTargets.SingleRowDatabaseQueryTarget<SimpleQueryTargets.QueryableValue<string>>("ParseAclTableAndSD", new Type[]
			{
				typeof(byte[])
			}, delegate(Context context, object[] parameters)
			{
				byte[] array = parameters[0] as byte[];
				if (array == null)
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidAclTableAndSDFormat("<null>"));
				}
				SimpleQueryTargets.QueryableValue<string> result;
				try
				{
					FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = FolderSecurity.AclTableAndSecurityDescriptorProperty.Parse(array);
					StringBuilder stringBuilder = new StringBuilder(1000);
					stringBuilder.AppendLine("ACL table:");
					if (aclTableAndSecurityDescriptorProperty.SerializedAclTable.Count != 0)
					{
						using (MemoryStream memoryStream = new MemoryStream(aclTableAndSecurityDescriptorProperty.SerializedAclTable.Array, aclTableAndSecurityDescriptorProperty.SerializedAclTable.Offset, aclTableAndSecurityDescriptorProperty.SerializedAclTable.Count))
						{
							using (BinaryReader binaryReader = new BinaryReader(memoryStream))
							{
								List<FolderSecurity.AclTableEntry> list = FolderSecurity.AclTableEntry.ParseTableEntries<FolderSecurity.AclTableEntry>(binaryReader, new Func<BinaryReader, FolderSecurity.AclTableEntry>(FolderSecurity.AclTableEntry.Parse));
								for (int i = 0; i < list.Count; i++)
								{
									FolderSecurity.AclTableEntry aclTableEntry = list[i];
									stringBuilder.AppendFormat("Name({0}): {1}", i, aclTableEntry.Name);
									stringBuilder.AppendLine();
									stringBuilder.AppendFormat("SID({0}): {1}", i, aclTableEntry.SecurityIdentifier);
									stringBuilder.AppendLine();
									stringBuilder.AppendFormat("IsGroup({0}): {1}", i, aclTableEntry.IsGroup);
									stringBuilder.AppendLine();
									stringBuilder.AppendFormat("Rights({0}): {1}", i, aclTableEntry.Rights);
									stringBuilder.AppendLine();
									if (aclTableEntry.EntryId != null)
									{
										stringBuilder.AppendFormat("EntryId({0}): {1}", i, ToStringHelper.GetAsString(aclTableEntry.EntryId, 0, aclTableEntry.EntryId.Length));
										stringBuilder.AppendLine();
										Eidt eidt;
										string arg;
										if (AddressBookEID.IsAddressBookEntryId(context, aclTableEntry.EntryId, out eidt, out arg))
										{
											stringBuilder.AppendFormat("EntryId({0}).Eidt: {1}", i, eidt);
											stringBuilder.AppendLine();
											stringBuilder.AppendFormat("EntryId({0}).EmailAddress: {1}", i, arg);
											stringBuilder.AppendLine();
										}
									}
									else
									{
										stringBuilder.AppendFormat("EntryId({0}): <null>", i);
										stringBuilder.AppendLine();
									}
								}
							}
						}
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("SID to Type map:");
					if (aclTableAndSecurityDescriptorProperty.SecurityIdentifierToTypeMap != null)
					{
						foreach (KeyValuePair<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> keyValuePair in aclTableAndSecurityDescriptorProperty.SecurityIdentifierToTypeMap)
						{
							stringBuilder.AppendFormat("SID:{0}, Type:{1}", keyValuePair.Key, keyValuePair.Value);
							stringBuilder.AppendLine();
						}
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("SD:");
					if (aclTableAndSecurityDescriptorProperty.SecurityDescriptor != null)
					{
						stringBuilder.AppendLine(SecurityHelper.CreateRawSecurityDescriptor(aclTableAndSecurityDescriptorProperty.SecurityDescriptor).GetSddlForm(AccessControlSections.All));
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine("FreeBusy SD:");
					if (aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor != null)
					{
						stringBuilder.AppendLine(SecurityHelper.CreateRawSecurityDescriptor(aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor).GetSddlForm(AccessControlSections.All));
					}
					result = new SimpleQueryTargets.QueryableValue<string>(stringBuilder.ToString());
				}
				catch (ArgumentException ex)
				{
					context.OnExceptionCatch(ex);
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidAclTableAndSDFormat(ex.ToString()));
				}
				catch (EndOfStreamException ex2)
				{
					context.OnExceptionCatch(ex2);
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidAclTableAndSDFormat(ex2.ToString()));
				}
				return result;
			}), Visibility.Redacted);
		}

		public void Register<T>(IStoreSimpleQueryTarget<T> queryTarget, Visibility visibility)
		{
			this.targets[queryTarget.Name] = queryTarget;
			StoreQueryTargets.Register<T>(queryTarget, visibility);
		}

		public void Register<T>(StoreDatabase database, IStoreDatabaseQueryTarget<T> queryTarget, Visibility visibility)
		{
			Dictionary<string, object> dictionary = null;
			if (!this.databaseTargets.TryGetValue(database.MdbGuid, out dictionary))
			{
				dictionary = new Dictionary<string, object>();
				this.databaseTargets.Add(database.MdbGuid, dictionary);
			}
			dictionary[queryTarget.Name] = queryTarget;
			StoreQueryTargets.Register<T>(queryTarget, database.PhysicalDatabase, visibility);
		}

		internal static IDisposable SetFullTextIndexQueryTestHook(Func<IFullTextIndexQuery> testHook)
		{
			return SimpleQueryTargets.FullTextIndexQueryCreator.SetTestHook(testHook);
		}

		internal static IStoreSimpleQueryTarget<T> Target<T>(string name)
		{
			return (IStoreSimpleQueryTarget<T>)SimpleQueryTargets.instance.targets[name];
		}

		internal static IStoreDatabaseQueryTarget<T> Target<T>(StoreDatabase database, string name)
		{
			return (IStoreDatabaseQueryTarget<T>)SimpleQueryTargets.instance.databaseTargets[database.MdbGuid][name];
		}

		private static IEnumerable<QueryableActiveSetting> ActiveSettings(object[] parameters)
		{
			List<QueryableActiveSetting> list = new List<QueryableActiveSetting>(ConfigurationSchema.RegisteredConfigurations.Count);
			for (int i = 0; i < ConfigurationSchema.RegisteredConfigurations.Count; i++)
			{
				ConfigurationSchema configurationSchema = ConfigurationSchema.RegisteredConfigurations[i];
				Type type = configurationSchema.GetType();
				bool isReadOnce = type.Name.StartsWith("ReadOnceConfigurationSchema");
				while (type.BaseType != typeof(ConfigurationSchema))
				{
					type = type.BaseType;
				}
				PropertyInfo property = type.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				list.Add(new QueryableActiveSetting(configurationSchema.Name, isReadOnce, property.GetValue(configurationSchema)));
			}
			list.Add(new QueryableActiveSetting("NextConfigurationRefreshTimeUTC", false, ConfigurationSchema.NextRefreshTimeUTC));
			return list;
		}

		private static T ExecuteForSingleMailbox<T>(Context context, int mailboxNumber, Func<MailboxState, T> query)
		{
			context.InitializeMailboxOperation(mailboxNumber, ExecutionDiagnostics.OperationSource.SimpleQueryTarget, DefaultSettings.Get.DiagnosticQueryLockTimeout, true, true);
			T result;
			try
			{
				ErrorCodeValue errorCodeValue = context.StartMailboxOperation();
				ErrorCodeValue errorCodeValue2 = errorCodeValue;
				if (errorCodeValue2 == ErrorCodeValue.NotFound)
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.MailboxStateNotFound(mailboxNumber));
				}
				if (errorCodeValue2 == ErrorCodeValue.Timeout)
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.UnableToLockMailbox(mailboxNumber));
				}
				context.LockedMailboxState.AddReference();
				try
				{
					result = query(context.LockedMailboxState);
				}
				finally
				{
					context.LockedMailboxState.ReleaseReference();
				}
			}
			finally
			{
				if (context.IsMailboxOperationStarted)
				{
					context.EndMailboxOperation(false);
				}
			}
			return result;
		}

		private static IEnumerable<T> ExecuteForAllMailboxes<T>(Context context, Func<MailboxState, T> query)
		{
			List<T> list = new List<T>(10);
			foreach (MailboxState mailboxState in MailboxStateCache.GetStateListSnapshot(context, null))
			{
				int mailboxNumber = mailboxState.MailboxNumber;
				T t = SimpleQueryTargets.ExecuteForSingleMailbox<T>(context, mailboxNumber, query);
				if (t != null)
				{
					list.Add(t);
				}
			}
			return list;
		}

		private static IEnumerable<SimpleQueryTargets.QueryableValue<string>> ClusterDBSubkeys(object[] parameters)
		{
			string text = parameters[0] as string;
			List<SimpleQueryTargets.QueryableValue<string>> list = new List<SimpleQueryTargets.QueryableValue<string>>(10);
			if (text != null)
			{
				try
				{
					using (IClusterDB clusterDB = ClusterDB.Open())
					{
						if (!clusterDB.IsInstalled)
						{
							throw new DiagnosticQueryException(DiagnosticQueryStrings.ClusterNotInstalled());
						}
						if (!clusterDB.IsInitialized)
						{
							throw new DiagnosticQueryException(DiagnosticQueryStrings.ServerIsNotDAGMember());
						}
						foreach (string value in clusterDB.GetSubKeyNames(text))
						{
							list.Add(new SimpleQueryTargets.QueryableValue<string>(value));
						}
					}
				}
				catch (ClusterException ex)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
					throw new DiagnosticQueryException(ex.Message);
				}
			}
			return list;
		}

		private static IEnumerable<KeyValuePair<string, object>> ClusterDBValues(object[] parameters)
		{
			string text = parameters[0] as string;
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>(20);
			if (text != null)
			{
				try
				{
					using (IClusterDB clusterDB = ClusterDB.Open())
					{
						if (!clusterDB.IsInstalled)
						{
							throw new DiagnosticQueryException(DiagnosticQueryStrings.ClusterNotInstalled());
						}
						if (!clusterDB.IsInitialized)
						{
							throw new DiagnosticQueryException(DiagnosticQueryStrings.ServerIsNotDAGMember());
						}
						object value = null;
						foreach (Tuple<string, RegistryValueKind> tuple in clusterDB.GetValueInfos(text))
						{
							RegistryValueKind item = tuple.Item2;
							if (item <= RegistryValueKind.DWord)
							{
								if (item != RegistryValueKind.String)
								{
									if (item == RegistryValueKind.DWord)
									{
										value = clusterDB.GetValue<int>(text, tuple.Item1, 0);
									}
								}
								else
								{
									value = clusterDB.GetValue<string>(text, tuple.Item1, string.Empty);
								}
							}
							else if (item != RegistryValueKind.MultiString)
							{
								if (item == RegistryValueKind.QWord)
								{
									value = clusterDB.GetValue<long>(text, tuple.Item1, 0L);
								}
							}
							else
							{
								value = clusterDB.GetValue<string[]>(text, tuple.Item1, Array<string>.Empty);
							}
							list.Add(new KeyValuePair<string, object>(tuple.Item1, value));
						}
					}
				}
				catch (ClusterException ex)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
					throw new DiagnosticQueryException(ex.Message);
				}
			}
			return list;
		}

		private static readonly Hookable<Func<IFullTextIndexQuery>> FullTextIndexQueryCreator = Hookable<Func<IFullTextIndexQuery>>.Create(true, () => new FullTextIndexQuery());

		private static SimpleQueryTargets instance;

		private Dictionary<string, object> targets = new Dictionary<string, object>();

		private Dictionary<Guid, Dictionary<string, object>> databaseTargets = new Dictionary<Guid, Dictionary<string, object>>();

		public class QueryableValue<T>
		{
			public QueryableValue()
			{
				this.value = default(T);
			}

			public QueryableValue(T value)
			{
				this.value = value;
			}

			[Queryable]
			public T Value
			{
				get
				{
					return this.value;
				}
			}

			private readonly T value;
		}

		public abstract class QueryTargetBase<T> : IStoreQueryTargetBase<T>
		{
			public QueryTargetBase(string name, Type[] parameterTypes)
			{
				this.name = name;
				this.parameterTypes = parameterTypes;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public Type[] ParameterTypes
			{
				get
				{
					return this.parameterTypes;
				}
			}

			private readonly string name;

			private readonly Type[] parameterTypes;
		}

		public abstract class SingleRowQueryTargetBase<T> : SimpleQueryTargets.QueryTargetBase<T>
		{
			public SingleRowQueryTargetBase(string name, Type[] parameterTypes) : base(name, parameterTypes)
			{
			}

			protected IEnumerable<T> GetRows(object[] parameters, Func<object[], T> rowBuilder)
			{
				for (int num = 0; num != parameters.Length; num++)
				{
				}
				T rowValue = default(T);
				if (parameters != null && parameters.Length == base.ParameterTypes.Length)
				{
					rowValue = rowBuilder(parameters);
				}
				yield return rowValue;
				yield break;
			}
		}

		public class SingleRowSimpleQueryTarget<T> : SimpleQueryTargets.SingleRowQueryTargetBase<T>, IStoreSimpleQueryTarget<T>, IStoreQueryTargetBase<T>
		{
			public SingleRowSimpleQueryTarget(string name, Type[] parameterTypes, Func<object[], T> rowBuilder) : base(name, parameterTypes)
			{
				this.rowBuilder = rowBuilder;
			}

			public SingleRowSimpleQueryTarget(string name, Func<T> rowBuilder) : this(name, Array<Type>.Empty, (object[] parameters) => rowBuilder())
			{
			}

			public IEnumerable<T> GetRows(object[] parameters)
			{
				return base.GetRows(parameters, this.rowBuilder);
			}

			private readonly Func<object[], T> rowBuilder;
		}

		public class SingleRowDatabaseQueryTarget<T> : SimpleQueryTargets.SingleRowQueryTargetBase<T>, IStoreDatabaseQueryTarget<T>, IStoreQueryTargetBase<T>
		{
			public SingleRowDatabaseQueryTarget(string name, Type[] parameterTypes, Func<Context, object[], T> rowBuilder) : base(name, parameterTypes)
			{
				this.rowBuilder = rowBuilder;
			}

			public SingleRowDatabaseQueryTarget(string name, Func<Context, T> rowBuilder) : this(name, Array<Type>.Empty, (Context context, object[] parameters) => rowBuilder(context))
			{
			}

			public IEnumerable<T> GetRows(IConnectionProvider connectionProvider, object[] parameters)
			{
				IContextProvider contextProvider = connectionProvider as IContextProvider;
				if (contextProvider == null)
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidStoreContext());
				}
				return base.GetRows(parameters, (object[] rowBuilderParameters) => this.rowBuilder(contextProvider.CurrentContext, rowBuilderParameters));
			}

			private readonly Func<Context, object[], T> rowBuilder;
		}

		public abstract class MultiRowQueryTargetBase<T> : SimpleQueryTargets.QueryTargetBase<T>
		{
			public MultiRowQueryTargetBase(string name, Type[] parameterTypes) : base(name, parameterTypes)
			{
			}

			protected IEnumerable<T> GetRows(object[] parameters, Func<object[], IEnumerable<T>> rowBuilder)
			{
				for (int num = 0; num != parameters.Length; num++)
				{
				}
				if (parameters != null && parameters.Length == base.ParameterTypes.Length)
				{
					return rowBuilder(parameters);
				}
				return Array<T>.Empty;
			}
		}

		public class MultiRowSimpleQueryTarget<T> : SimpleQueryTargets.MultiRowQueryTargetBase<T>, IStoreSimpleQueryTarget<T>, IStoreQueryTargetBase<T>
		{
			public MultiRowSimpleQueryTarget(string name, Type[] parameterTypes, Func<object[], IEnumerable<T>> rowBuilder) : base(name, parameterTypes)
			{
				this.rowBuilder = rowBuilder;
			}

			public MultiRowSimpleQueryTarget(string name, Func<IEnumerable<T>> rowBuilder) : this(name, Array<Type>.Empty, (object[] parameters) => rowBuilder())
			{
			}

			public IEnumerable<T> GetRows(object[] parameters)
			{
				return base.GetRows(parameters, this.rowBuilder);
			}

			private readonly Func<object[], IEnumerable<T>> rowBuilder;
		}

		public class MultiRowDatabaseQueryTarget<T> : SimpleQueryTargets.MultiRowQueryTargetBase<T>, IStoreDatabaseQueryTarget<T>, IStoreQueryTargetBase<T>
		{
			public MultiRowDatabaseQueryTarget(string name, Type[] parameterTypes, Func<Context, object[], IEnumerable<T>> rowBuilder) : base(name, parameterTypes)
			{
				this.rowBuilder = rowBuilder;
			}

			public MultiRowDatabaseQueryTarget(string name, Func<Context, IEnumerable<T>> rowBuilder) : this(name, Array<Type>.Empty, (Context context, object[] parameters) => rowBuilder(context))
			{
			}

			public IEnumerable<T> GetRows(IConnectionProvider connectionProvider, object[] parameters)
			{
				IContextProvider contextProvider = connectionProvider as IContextProvider;
				if (contextProvider == null)
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.InvalidStoreContext());
				}
				return base.GetRows(parameters, (object[] rowBuilderParameters) => this.rowBuilder(contextProvider.CurrentContext, rowBuilderParameters));
			}

			private readonly Func<Context, object[], IEnumerable<T>> rowBuilder;
		}
	}
}
