using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal static class ElcMailboxHelper
	{
		public static void PopulateFolderPathProperty(List<object[]> entireList, FolderPathIndices indices)
		{
			for (int i = 0; i < entireList.Count; i++)
			{
				List<object> list = new List<object>(entireList[i]);
				string text = '/' + ((string)entireList[i][indices.DisplayNameIndex]).Replace('/', '');
				if ((int)entireList[i][indices.FolderDepthIndex] > 1)
				{
					for (int j = i - 1; j >= 0; j--)
					{
						if (((VersionedId)entireList[j][indices.FolderIdIndex]).ObjectId.Equals((StoreObjectId)entireList[i][indices.ParentIdIndex]))
						{
							text = (string)entireList[j][indices.FolderPathIndex] + text;
							break;
						}
					}
				}
				list.Add(text);
				entireList[i] = list.ToArray();
			}
		}

		internal static bool Exists(object property)
		{
			return property != null && !(property is PropertyError);
		}

		internal static void SetPropAndTrace(Folder folder, PropertyDefinition prop, object value)
		{
			folder.SetProperties(new PropertyDefinition[]
			{
				prop
			}, new object[]
			{
				value
			});
			ElcMailboxHelper.Tracer.TraceDebug<PropertyDefinition, string>(0L, "Property '{0}' has been set on folder '{1}'.", prop, folder.DisplayName);
		}

		internal static Folder GetDefaultFolder(MailboxSession itemStore, ELCFolder elcFolder)
		{
			Folder result;
			if (elcFolder.FolderType == ElcFolderType.Inbox)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Inbox);
			}
			else if (elcFolder.FolderType == ElcFolderType.SentItems)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.SentItems);
			}
			else if (elcFolder.FolderType == ElcFolderType.DeletedItems)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.DeletedItems);
			}
			else if (elcFolder.FolderType == ElcFolderType.Calendar)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Calendar);
			}
			else if (elcFolder.FolderType == ElcFolderType.Contacts)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Contacts);
			}
			else if (elcFolder.FolderType == ElcFolderType.Drafts)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Drafts);
			}
			else if (elcFolder.FolderType == ElcFolderType.Outbox)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Outbox);
			}
			else if (elcFolder.FolderType == ElcFolderType.JunkEmail)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.JunkEmail);
			}
			else if (elcFolder.FolderType == ElcFolderType.Tasks)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Tasks);
			}
			else if (elcFolder.FolderType == ElcFolderType.Journal)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Journal);
			}
			else if (elcFolder.FolderType == ElcFolderType.Notes)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Notes);
			}
			else if (elcFolder.FolderType == ElcFolderType.All)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.Root);
			}
			else if (elcFolder.FolderType == ElcFolderType.SyncIssues)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.SyncIssues);
			}
			else if (elcFolder.FolderType == ElcFolderType.RssSubscriptions)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.RssSubscription);
			}
			else if (elcFolder.FolderType == ElcFolderType.ConversationHistory)
			{
				result = Folder.Bind(itemStore, DefaultFolderType.CommunicatorHistory);
			}
			else
			{
				if (elcFolder.FolderType != ElcFolderType.LegacyArchiveJournals)
				{
					throw new ELCUnknownDefaultFolderException(elcFolder.FolderName, itemStore.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				result = Folder.Bind(itemStore, DefaultFolderType.LegacyArchiveJournals);
			}
			return result;
		}

		internal static bool IsElcFolder(object[] folderProps)
		{
			if (ElcMailboxHelper.Exists(folderProps[3]))
			{
				return true;
			}
			if (ElcMailboxHelper.Exists(folderProps[5]) && folderProps[5] is int?)
			{
				int num = (int)folderProps[5];
				if ((num & 1) != 0)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsElcFolder(Folder folder)
		{
			object[] properties = folder.GetProperties(new PropertyDefinition[]
			{
				FolderSchema.ELCPolicyIds,
				FolderSchema.AdminFolderFlags
			});
			if (ElcMailboxHelper.Exists(properties[0]))
			{
				return true;
			}
			if (ElcMailboxHelper.Exists(properties[1]))
			{
				int num = (int)properties[1];
				if ((num & 1) != 0)
				{
					return true;
				}
			}
			return false;
		}

		internal static StoreObjectId GetElcRootFolderId(MailboxSession mailboxSession)
		{
			StoreObjectId result = null;
			string text = null;
			string text2 = null;
			ProvisionedFolderReader.GetElcRootFolderInfo(mailboxSession, out result, out text, out text2);
			return result;
		}

		internal static bool RemoveElcInMailbox(MailboxSession mailboxSession)
		{
			return ElcMailboxHelper.ScrubElcMailbox(false, mailboxSession, null);
		}

		internal static bool UpgradeElcMailbox(MailboxSession mailboxSession, Dictionary<Guid, AdTagData> allAdTags, out UpgradeStatus status)
		{
			return ElcMailboxHelper.ScrubElcMailbox(true, mailboxSession, allAdTags, out status);
		}

		internal static void UpgradeElcFolder(bool userIsOnRetentionPolcyTags, MailboxSession mailboxSession, StoreId folderId, Dictionary<Guid, AdTagData> allAdTags)
		{
			using (Folder folder = Folder.Bind(mailboxSession, folderId, ProvisionedFolderReader.ElcFolderProps))
			{
				ElcMailboxHelper.UpgradeElcFolder(userIsOnRetentionPolcyTags, mailboxSession, folder, allAdTags);
				ProvisionedFolderCreator.SaveELCFolder(folder, false);
			}
		}

		internal static UpgradeStatus UpgradeElcFolder(bool userIsOnRetentionPolcyTags, MailboxSession mailboxSession, Folder folder, Dictionary<Guid, AdTagData> allAdTags)
		{
			UpgradeStatus result = UpgradeStatus.None;
			AdTagData adTagData = null;
			if (allAdTags != null)
			{
				object obj = folder.TryGetProperty(FolderSchema.ELCPolicyIds);
				if (obj is string)
				{
					string g = obj as string;
					Guid empty = Guid.Empty;
					if (GuidHelper.TryParseGuid(g, out empty))
					{
						foreach (AdTagData adTagData2 in allAdTags.Values)
						{
							if (adTagData2.Tag != null && adTagData2.Tag.LegacyManagedFolder != null && adTagData2.Tag.LegacyManagedFolder.Value == empty)
							{
								ElcMailboxHelper.Tracer.TraceDebug<IExchangePrincipal, string, string>(0L, "Mailbox:{0}. Folder {1} was upgraded to tag {2} because they were linked match.", (mailboxSession != null) ? mailboxSession.MailboxOwner : null, (folder != null) ? folder.DisplayName : null, (adTagData != null && adTagData.Tag != null) ? adTagData.Tag.Name : null);
								adTagData = adTagData2;
							}
						}
					}
				}
			}
			mailboxSession.IsDefaultFolderType(folder.Id);
			ElcMailboxHelper.DeleteElcFolderProperties(folder);
			if (adTagData != null && adTagData.Tag.Type != ElcFolderType.All)
			{
				foreach (ContentSetting contentSetting in adTagData.ContentSettings.Values)
				{
					if (contentSetting.RetentionEnabled)
					{
						folder[StoreObjectSchema.RetentionPeriod] = (int)contentSetting.AgeLimitForRetention.Value.TotalDays;
						break;
					}
				}
				folder[StoreObjectSchema.PolicyTag] = adTagData.Tag.RetentionId.ToByteArray();
				folder[StoreObjectSchema.RetentionFlags] = 1;
				result = UpgradeStatus.AppliedFolderTag;
			}
			return result;
		}

		internal static bool RemoveElcFolder(bool userIsOnRetentionPolcyTags, MailboxSession mailboxSession, StoreId folderId, Dictionary<Guid, AdTagData> allAdTags, out UpgradeStatus status)
		{
			status = UpgradeStatus.None;
			Stack<ElcMailboxHelper.FolderNode> stack = new Stack<ElcMailboxHelper.FolderNode>(50);
			ElcMailboxHelper.FolderNode folderNode = new ElcMailboxHelper.FolderNode(folderId, null);
			stack.Push(folderNode);
			while (stack.Count > 0)
			{
				ElcMailboxHelper.FolderNode folderNode2 = stack.Peek();
				if (folderNode2.ChildCount == -1)
				{
					using (Folder folder = Folder.Bind(mailboxSession, folderNode2.FolderId, ProvisionedFolderReader.ElcFolderProps))
					{
						folderNode2.ChildCount = folder.ItemCount + folder.SubfolderCount;
						using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, ProvisionedFolderReader.ElcFolderProps))
						{
							for (;;)
							{
								object[][] rows = queryResult.GetRows(100);
								if (rows.Length <= 0)
								{
									break;
								}
								foreach (object[] array in rows)
								{
									stack.Push(new ElcMailboxHelper.FolderNode((VersionedId)array[0], folderNode2));
								}
							}
						}
						if (ElcMailboxHelper.IsElcFolder(folder))
						{
							status |= ElcMailboxHelper.UpgradeElcFolder(userIsOnRetentionPolcyTags, mailboxSession, folder, allAdTags);
							ProvisionedFolderCreator.SaveELCFolder(folder, false);
						}
						continue;
					}
				}
				if (folderNode2.ChildCount == 0)
				{
					if (folderNode2.Parent != null)
					{
						folderNode2.Parent.ChildCount--;
					}
					mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						folderNode2.FolderId
					});
				}
				stack.Pop();
			}
			return folderNode.ChildCount == 0;
		}

		internal static DefaultFolderType? GetDefaultFolderType(MailboxSession session, StoreObjectId folderId)
		{
			foreach (DefaultFolderType defaultFolderType in ElcMailboxHelper.DefaultFolderTypeList)
			{
				if (folderId.Equals(session.GetDefaultFolderId(defaultFolderType)))
				{
					return new DefaultFolderType?(defaultFolderType);
				}
			}
			return null;
		}

		internal static ElcFolderType? GetElcFolderType(DefaultFolderType defaultType)
		{
			for (int i = 0; i < ElcMailboxHelper.DefaultFolderTypeList.Length; i++)
			{
				if (ElcMailboxHelper.DefaultFolderTypeList[i] == defaultType)
				{
					return new ElcFolderType?(ElcMailboxHelper.ElcFolderTypeList[i]);
				}
			}
			return null;
		}

		internal static void ForeachQueryResult(QueryResult queryResult, ElcMailboxHelper.RowInvokeDelegate rowInvoke)
		{
			ElcMailboxHelper.ForeachQueryResult(queryResult, 100, rowInvoke);
		}

		internal static void ForeachQueryResult(QueryResult queryResult, int fetchCount, ElcMailboxHelper.RowInvokeDelegate rowInvoke)
		{
			bool flag = false;
			while (!flag)
			{
				object[][] rows = queryResult.GetRows(fetchCount);
				if (rows == null)
				{
					break;
				}
				if (rows.Length <= 0)
				{
					return;
				}
				for (int i = 0; i < rows.Length; i++)
				{
					rowInvoke(rows[i], ref flag);
					if (flag)
					{
						return;
					}
				}
			}
		}

		internal static void DeleteElcFolderProperties(Folder folder)
		{
			folder.DeleteProperties(new PropertyDefinition[]
			{
				FolderSchema.ELCPolicyIds,
				FolderSchema.ELCFolderComment,
				FolderSchema.FolderQuota
			});
			object[] properties = folder.GetProperties(ProvisionedFolderReader.ElcFolderProps);
			int num = 0;
			if (properties != null && !(properties[5] is PropertyError) && properties[5] is int?)
			{
				num = (int)properties[5];
			}
			num &= -60;
			folder.SetOrDeleteProperty(FolderSchema.AdminFolderFlags, num);
		}

		internal static UserConfiguration OpenFaiMessage(MailboxSession mailboxSession, string faiMessageClass, bool createIfMissing)
		{
			StoreId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			return ElcMailboxHelper.OpenFaiMessage(mailboxSession, faiMessageClass, createIfMissing, defaultFolderId);
		}

		internal static UserConfiguration OpenFaiMessage(MailboxSession mailboxSession, string faiMessageClass, bool createIfMissing, StoreId folderId)
		{
			UserConfiguration userConfiguration = null;
			bool flag = true;
			int num = 0;
			while (flag)
			{
				flag = false;
				Exception ex = null;
				try
				{
					userConfiguration = mailboxSession.UserConfigurationManager.GetFolderConfiguration(faiMessageClass, UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, folderId);
				}
				catch (ObjectNotFoundException ex2)
				{
					ex = ex2;
				}
				catch (CorruptDataException ex3)
				{
					ex = ex3;
				}
				if (userConfiguration == null)
				{
					ElcMailboxHelper.Tracer.TraceDebug<string, Exception>(0L, "FAI message '{0}' is missing or corrupt. Exception: {1}", faiMessageClass, ex);
					if (createIfMissing)
					{
						if (ex is CorruptDataException)
						{
							mailboxSession.UserConfigurationManager.DeleteFolderConfigurations(folderId, new string[]
							{
								faiMessageClass
							});
						}
						try
						{
							userConfiguration = mailboxSession.UserConfigurationManager.CreateFolderConfiguration(faiMessageClass, UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, folderId);
						}
						catch (ObjectExistedException arg)
						{
							ElcMailboxHelper.Tracer.TraceDebug<string, ObjectExistedException>(0L, "FAI message '{0}' already exists. Exception: {1}", faiMessageClass, arg);
							if (num < 1)
							{
								ElcMailboxHelper.Tracer.TraceDebug<string>(0L, "Try to get or create FAI message '{0}' one more time.", faiMessageClass);
								flag = true;
								num++;
							}
						}
					}
				}
			}
			return userConfiguration;
		}

		internal static bool UpdateHoldCleanupWatermarkFAIMessage(DefaultFolderType folderType, string internetMessageId, string clientString, IExchangePrincipal principal, out Exception ex)
		{
			return ElcMailboxHelper.UpdateFAIMessage("HoldCleanupWatermark", string.Format("{0}:{1}", folderType, internetMessageId), principal, clientString, out ex);
		}

		internal static bool ClearHoldCleanupWatermarkFAIMessage(string clientString, IExchangePrincipal principal, out Exception ex)
		{
			return ElcMailboxHelper.UpdateFAIMessage("HoldCleanupWatermark", null, principal, clientString, out ex);
		}

		internal static bool SetEHAHiddenFolderCleanupWatermark(IExchangePrincipal principal, string clientString, out Exception ex)
		{
			return ElcMailboxHelper.UpdateFAIMessage("EHAHiddenFolderCleanupWatermark", "EHAHiddenFolderCleanupWatermark", principal, clientString, out ex);
		}

		internal static bool ClearEHAHiddenFolderCleanupWatermark(IExchangePrincipal principal, string clientString, out Exception ex)
		{
			return ElcMailboxHelper.UpdateFAIMessage("EHAHiddenFolderCleanupWatermark", null, principal, clientString, out ex);
		}

		private static bool UpdateFAIMessage(string userConfigKey, string serializedValue, IExchangePrincipal principal, string clientString, out Exception ex)
		{
			ex = null;
			bool result = false;
			try
			{
				using (MailboxSession mailboxSession = ElcMailboxHelper.OpenMailboxSessionAsSystemService(principal, clientString))
				{
					if (mailboxSession != null)
					{
						StoreId storeId = mailboxSession.GetSystemFolderId();
						if (storeId == null)
						{
							storeId = mailboxSession.CreateSystemFolder();
						}
						using (UserConfiguration userConfiguration = ElcMailboxHelper.OpenFaiMessage(mailboxSession, "MRM", true, storeId))
						{
							if (userConfiguration != null)
							{
								IDictionary dictionary = userConfiguration.GetDictionary();
								if (dictionary != null)
								{
									if (dictionary.Contains(userConfigKey))
									{
										dictionary[userConfigKey] = serializedValue;
									}
									else
									{
										dictionary.Add(userConfigKey, serializedValue);
									}
									userConfiguration.Save();
									result = true;
								}
							}
						}
					}
				}
			}
			catch (CorruptDataException ex2)
			{
				result = false;
				ex = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				result = false;
				ex = ex3;
			}
			catch (ObjectNotFoundException ex4)
			{
				result = false;
				ex = ex4;
			}
			catch (StorageTransientException ex5)
			{
				result = false;
				ex = ex5;
			}
			catch (StoragePermanentException ex6)
			{
				result = false;
				ex = ex6;
			}
			return result;
		}

		internal static void TryGetExistingHoldDurationInStore(IExchangePrincipal exchangePrincipal, string clientString, out Unlimited<EnhancedTimeSpan> existingHoldDuration, out ElcMailboxHelper.ConfigState state, out Exception exception)
		{
			Func<string, KeyValuePair<Unlimited<EnhancedTimeSpan>, ElcMailboxHelper.ConfigState>> valueFunction = delegate(string value)
			{
				Unlimited<EnhancedTimeSpan> key = default(Unlimited<EnhancedTimeSpan>);
				ElcMailboxHelper.ConfigState value2;
				double value3;
				if (string.Compare(value, Globals.UnlimitedHoldDuration, StringComparison.OrdinalIgnoreCase) == 0)
				{
					key = Unlimited<EnhancedTimeSpan>.UnlimitedValue;
					value2 = ElcMailboxHelper.ConfigState.Found;
				}
				else if (double.TryParse(value.ToString(), out value3))
				{
					EnhancedTimeSpan fromValue = EnhancedTimeSpan.FromMilliseconds(value3);
					key = fromValue;
					value2 = ElcMailboxHelper.ConfigState.Found;
				}
				else
				{
					value2 = ElcMailboxHelper.ConfigState.Corrupt;
				}
				return new KeyValuePair<Unlimited<EnhancedTimeSpan>, ElcMailboxHelper.ConfigState>(key, value2);
			};
			ElcMailboxHelper.TryGetExistingValueInStore<Unlimited<EnhancedTimeSpan>>(exchangePrincipal, clientString, "LitigationHoldDuration", valueFunction, out existingHoldDuration, out state, out exception);
		}

		internal static void TryGetHoldCleanupWatermarkInStore(IExchangePrincipal exchangePrincipal, string clientString, out DefaultFolderType folderType, out string internetMessageId, out ElcMailboxHelper.ConfigState state, out Exception exception)
		{
			folderType = DefaultFolderType.None;
			internetMessageId = null;
			Func<string, KeyValuePair<string, ElcMailboxHelper.ConfigState>> valueFunction = delegate(string value)
			{
				ElcMailboxHelper.ConfigState value2;
				if (!string.IsNullOrEmpty(value))
				{
					value2 = ElcMailboxHelper.ConfigState.Found;
				}
				else
				{
					value2 = ElcMailboxHelper.ConfigState.Empty;
				}
				return new KeyValuePair<string, ElcMailboxHelper.ConfigState>(value, value2);
			};
			string text = null;
			ElcMailboxHelper.TryGetExistingValueInStore<string>(exchangePrincipal, clientString, "HoldCleanupWatermark", valueFunction, out text, out state, out exception);
			if (state == ElcMailboxHelper.ConfigState.Found)
			{
				string[] array = text.Split(new char[]
				{
					':'
				}, 2);
				try
				{
					folderType = (DefaultFolderType)Enum.Parse(typeof(DefaultFolderType), array[0]);
					if (array.Length == 2)
					{
						internetMessageId = array[1];
					}
				}
				catch (ArgumentException)
				{
					state = ElcMailboxHelper.ConfigState.Corrupt;
				}
			}
		}

		internal static void TryGetEHAHiddenFolderCleanupWatermarkInStore(IExchangePrincipal exchangePrincipal, string clientString, out ElcMailboxHelper.ConfigState state, out Exception exception)
		{
			Func<string, KeyValuePair<string, ElcMailboxHelper.ConfigState>> valueFunction = delegate(string value)
			{
				ElcMailboxHelper.ConfigState value2;
				if (!string.IsNullOrEmpty(value))
				{
					if (!value.Equals("EHAHiddenFolderCleanupWatermark"))
					{
						value2 = ElcMailboxHelper.ConfigState.Invalid;
					}
					else
					{
						value2 = ElcMailboxHelper.ConfigState.Found;
					}
				}
				else
				{
					value2 = ElcMailboxHelper.ConfigState.Empty;
				}
				return new KeyValuePair<string, ElcMailboxHelper.ConfigState>(value, value2);
			};
			string text = null;
			ElcMailboxHelper.TryGetExistingValueInStore<string>(exchangePrincipal, clientString, "EHAHiddenFolderCleanupWatermark", valueFunction, out text, out state, out exception);
		}

		internal static void TryGetExistingValueInStore<T>(IExchangePrincipal exchangePrincipal, string clientString, string userConfigKey, Func<string, KeyValuePair<T, ElcMailboxHelper.ConfigState>> valueFunction, out T existingValue, out ElcMailboxHelper.ConfigState state, out Exception exception)
		{
			existingValue = default(T);
			state = ElcMailboxHelper.ConfigState.Unknown;
			exception = null;
			try
			{
				using (MailboxSession mailboxSession = ElcMailboxHelper.OpenMailboxSessionAsSystemService(exchangePrincipal, clientString))
				{
					if (mailboxSession == null)
					{
						state = ElcMailboxHelper.ConfigState.ErrorWhileFetching;
					}
					else
					{
						StoreId systemFolderId = mailboxSession.GetSystemFolderId();
						if (systemFolderId != null)
						{
							using (UserConfiguration userConfiguration = ElcMailboxHelper.OpenFaiMessage(mailboxSession, "MRM", false, systemFolderId))
							{
								if (userConfiguration == null)
								{
									state = ElcMailboxHelper.ConfigState.FAINotFound;
								}
								else
								{
									IDictionary dictionary = userConfiguration.GetDictionary();
									if (dictionary != null && dictionary.Contains(userConfigKey))
									{
										string text = dictionary[userConfigKey] as string;
										if (text != null)
										{
											KeyValuePair<T, ElcMailboxHelper.ConfigState> keyValuePair = valueFunction(text);
											existingValue = keyValuePair.Key;
											state = keyValuePair.Value;
										}
										else
										{
											state = ElcMailboxHelper.ConfigState.Empty;
										}
									}
									else
									{
										state = ElcMailboxHelper.ConfigState.Empty;
									}
								}
								goto IL_AE;
							}
						}
						state = ElcMailboxHelper.ConfigState.FAINotFound;
					}
					IL_AE:;
				}
			}
			catch (InvalidCastException ex)
			{
				state = ElcMailboxHelper.ConfigState.Corrupt;
				exception = ex;
			}
			catch (CorruptDataException ex2)
			{
				state = ElcMailboxHelper.ConfigState.Corrupt;
				exception = ex2;
			}
			catch (InvalidOperationException ex3)
			{
				state = ElcMailboxHelper.ConfigState.Invalid;
				exception = ex3;
			}
			catch (StorageTransientException ex4)
			{
				state = ElcMailboxHelper.ConfigState.ErrorWhileFetching;
				exception = ex4;
			}
			catch (StoragePermanentException ex5)
			{
				state = ElcMailboxHelper.ConfigState.ErrorWhileFetching;
				exception = ex5;
			}
		}

		internal static MailboxSession OpenMailboxSessionAsSystemService(IExchangePrincipal principal, string clientString)
		{
			return MailboxSession.OpenAsSystemService(principal, CultureInfo.InvariantCulture, clientString);
		}

		internal static Guid? GetGuidFromBytes(object objectValue, Guid? defaultGuid, bool initToBadGuidIfCorrupt, object toTrace)
		{
			Guid? result = defaultGuid;
			byte[] array = objectValue as byte[];
			if (array != null)
			{
				if (array.Length == ElcMailboxHelper.SizeOfGuid)
				{
					result = new Guid?(new Guid(array));
				}
				else
				{
					ElcMailboxHelper.Tracer.TraceDebug<object, string>(0L, "Object {0} contains a corrupt guid: {1}.", toTrace ?? string.Empty, BitConverter.ToString(array));
					if (initToBadGuidIfCorrupt)
					{
						result = new Guid?(ElcMailboxHelper.BadGuid);
					}
				}
			}
			else
			{
				ElcMailboxHelper.Tracer.TraceDebug(0L, "Tag guid on Object {0} is not of type byte.", new object[]
				{
					toTrace ?? string.Empty
				});
			}
			return result;
		}

		internal static bool ExceptionFilter(object exception)
		{
			return exception is AccessDeniedException || exception is CorruptDataException || exception is DataValidationException || exception is ObjectNotFoundException || exception is PropertyErrorException || exception is RecurrenceException || exception is SaveConflictException || exception is VirusDetectedException || exception is VirusMessageDeletedException || exception is VirusScanInProgressException || exception is MessageSubmissionExceededException;
		}

		internal static bool IsArchiveTag(AdTagData tagData, bool enabledOnly)
		{
			if (tagData.ContentSettings != null)
			{
				foreach (ContentSetting contentSetting in tagData.ContentSettings.Values)
				{
					if (contentSetting.RetentionAction == RetentionActionType.MoveToArchive)
					{
						ElcMailboxHelper.Tracer.TraceDebug<string>(0L, "Tag {0} is MTA.", tagData.Tag.Name);
						if (!enabledOnly)
						{
							return true;
						}
						if (contentSetting.RetentionEnabled)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		private static bool ScrubElcMailbox(bool userIsOnRetentionPolcyTags, MailboxSession mailboxSession, Dictionary<Guid, AdTagData> allAdTags)
		{
			UpgradeStatus upgradeStatus = UpgradeStatus.None;
			return ElcMailboxHelper.ScrubElcMailbox(userIsOnRetentionPolcyTags, mailboxSession, allAdTags, out upgradeStatus);
		}

		private static bool ScrubElcMailbox(bool userIsOnRetentionPolcyTags, MailboxSession mailboxSession, Dictionary<Guid, AdTagData> allAdTags, out UpgradeStatus status)
		{
			status = UpgradeStatus.None;
			bool result = true;
			StoreObjectId elcRootFolderId = ElcMailboxHelper.GetElcRootFolderId(mailboxSession);
			if (elcRootFolderId != null)
			{
				result = ElcMailboxHelper.RemoveElcFolder(userIsOnRetentionPolcyTags, mailboxSession, elcRootFolderId, allAdTags, out status);
			}
			object[] array = null;
			using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Root, ProvisionedFolderReader.ElcFolderProps))
			{
				array = folder.GetProperties(ProvisionedFolderReader.ElcFolderProps);
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, ProvisionedFolderReader.ElcFolderProps))
				{
					for (;;)
					{
						object[][] rows = queryResult.GetRows(100);
						if (rows.Length <= 0)
						{
							break;
						}
						foreach (object[] array2 in rows)
						{
							if (ElcMailboxHelper.IsElcFolder(array2))
							{
								ElcMailboxHelper.UpgradeElcFolder(userIsOnRetentionPolcyTags, mailboxSession, (VersionedId)array2[0], allAdTags);
							}
						}
					}
				}
			}
			if (ElcMailboxHelper.IsElcFolder(array))
			{
				ElcMailboxHelper.UpgradeElcFolder(userIsOnRetentionPolcyTags, mailboxSession, (VersionedId)array[0], null);
			}
			StoreId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			mailboxSession.UserConfigurationManager.DeleteFolderConfigurations(defaultFolderId, new string[]
			{
				"ELC"
			});
			return result;
		}

		internal const char ForwardSlashCode = '';

		private const char Delimiter = '/';

		private const char HoldCleanupDelimiter = ':';

		private const string HoldCleanupWatermark = "{0}:{1}";

		internal static readonly DefaultFolderType[] DefaultFolderTypeList = new DefaultFolderType[]
		{
			DefaultFolderType.Calendar,
			DefaultFolderType.Contacts,
			DefaultFolderType.DeletedItems,
			DefaultFolderType.Drafts,
			DefaultFolderType.Inbox,
			DefaultFolderType.Journal,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.Notes,
			DefaultFolderType.Outbox,
			DefaultFolderType.SentItems,
			DefaultFolderType.Tasks,
			DefaultFolderType.Root,
			DefaultFolderType.RssSubscription,
			DefaultFolderType.SyncIssues,
			DefaultFolderType.CommunicatorHistory,
			DefaultFolderType.LegacyArchiveJournals
		};

		internal static readonly ElcFolderType[] ElcFolderTypeList = new ElcFolderType[]
		{
			ElcFolderType.Calendar,
			ElcFolderType.Contacts,
			ElcFolderType.DeletedItems,
			ElcFolderType.Drafts,
			ElcFolderType.Inbox,
			ElcFolderType.Journal,
			ElcFolderType.JunkEmail,
			ElcFolderType.Notes,
			ElcFolderType.Outbox,
			ElcFolderType.SentItems,
			ElcFolderType.Tasks,
			ElcFolderType.All,
			ElcFolderType.RssSubscriptions,
			ElcFolderType.SyncIssues,
			ElcFolderType.ConversationHistory,
			ElcFolderType.LegacyArchiveJournals
		};

		internal static readonly int SizeOfGuid = Marshal.SizeOf(Guid.NewGuid());

		internal static readonly Guid BadGuid = new Guid("bad0bad0bad0bad0bad0bad0bad0bad0");

		private static readonly Trace Tracer = ExTraceGlobals.ELCTracer;

		internal delegate void RowInvokeDelegate(object[] rowProps, ref bool breakLoop);

		internal enum ConfigState
		{
			Unknown,
			FAINotFound,
			Invalid,
			Found,
			ErrorWhileFetching,
			Empty,
			Corrupt
		}

		internal class FolderNode
		{
			internal FolderNode(StoreId folderId, ElcMailboxHelper.FolderNode parent)
			{
				this.FolderId = folderId;
				this.parent = parent;
				this.ChildCount = -1;
			}

			internal StoreId FolderId
			{
				get
				{
					return this.folderId;
				}
				set
				{
					this.folderId = value;
				}
			}

			internal ElcMailboxHelper.FolderNode Parent
			{
				get
				{
					return this.parent;
				}
				set
				{
					this.parent = value;
				}
			}

			internal int ChildCount
			{
				get
				{
					return this.childCount;
				}
				set
				{
					this.childCount = value;
				}
			}

			private StoreId folderId;

			private ElcMailboxHelper.FolderNode parent;

			private int childCount;
		}

		internal static class LitigationHoldDurationSchema
		{
			internal const string LitigationHoldDuration = "LitigationHoldDuration";
		}

		internal static class HoldCleanupSchema
		{
			internal const string Watermark = "HoldCleanupWatermark";
		}

		internal static class EHAHiddenFolderCleanupSchema
		{
			internal const string Watermark = "EHAHiddenFolderCleanupWatermark";
		}
	}
}
