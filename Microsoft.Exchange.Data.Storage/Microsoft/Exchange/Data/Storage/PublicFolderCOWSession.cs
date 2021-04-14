using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderCOWSession : COWSessionBase
	{
		private PublicFolderCOWSession(PublicFolderSession publicFolderSession)
		{
			this.publicFolderSession = publicFolderSession;
		}

		public override StoreSession StoreSession
		{
			get
			{
				this.CheckDisposed(null);
				return this.publicFolderSession;
			}
		}

		public static PublicFolderCOWSession Create(PublicFolderSession publicFolderSession)
		{
			Util.ThrowOnNullArgument(publicFolderSession, "publicFolderSession");
			return new PublicFolderCOWSession(publicFolderSession);
		}

		public static bool IsDumpsterFolder(CoreFolder folder)
		{
			if (!(folder.Session is PublicFolderSession))
			{
				throw new InvalidOperationException("folder must be associated with PublicFolderSession");
			}
			folder.PropertyBag.Load(PublicFolderCOWSession.propertiesToLoadForDumpster);
			return (folder.PropertyBag.GetValueOrDefault<ELCFolderFlags>(FolderSchema.AdminFolderFlags, ELCFolderFlags.None) & ELCFolderFlags.DumpsterFolder) != ELCFolderFlags.None;
		}

		public static StoreObjectId GetRecoverableItemsDeletionsFolderId(Folder folder)
		{
			return PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(folder.CoreObject as CoreFolder);
		}

		public static StoreObjectId GetRecoverableItemsDeletionsFolderId(CoreFolder folder)
		{
			byte[] valueOrDefault = folder.PropertyBag.GetValueOrDefault<byte[]>(CoreFolderSchema.DeletedItemsEntryId, Array<byte>.Empty);
			if (valueOrDefault.Length == 22)
			{
				return folder.Session.IdConverter.CreateFolderId(folder.Session.IdConverter.GetIdFromLongTermId(valueOrDefault));
			}
			if (valueOrDefault.Length == 46)
			{
				return StoreObjectId.FromProviderSpecificId(valueOrDefault, StoreObjectType.Folder);
			}
			return null;
		}

		public static string GenerateUniqueFolderName(string folderName)
		{
			if (string.IsNullOrWhiteSpace(folderName))
			{
				return Guid.NewGuid().ToString();
			}
			if (folderName.Length > 150)
			{
				folderName = folderName.Substring(0, 150);
			}
			return folderName + "_" + Guid.NewGuid();
		}

		protected override FolderIdState InternalInitFolders(MailboxSession sessionWithBestAccess)
		{
			return FolderIdState.FolderIdSuccess;
		}

		public bool OnBeforeFolderChange(FolderChangeOperation operation, FolderChangeOperationFlags flags, StoreSession sourceSession, StoreSession destinationSession, StoreObjectId sourceFolderId, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds, CallbackContext callbackContext)
		{
			PublicFolderCOWSession.<>c__DisplayClass4 CS$<>8__locals1 = new PublicFolderCOWSession.<>c__DisplayClass4();
			CS$<>8__locals1.sourceSession = sourceSession;
			CS$<>8__locals1.sourceFolderId = sourceFolderId;
			CS$<>8__locals1.<>4__this = this;
			this.CheckDisposed("OnBeforeFolderChange");
			EnumValidator.ThrowIfInvalid<FolderChangeOperation>(operation, "operation");
			EnumValidator.ThrowIfInvalid<FolderChangeOperationFlags>(flags, "flags");
			Util.ThrowOnNullArgument(callbackContext, "callbackContext");
			if (this.InCallback)
			{
				return false;
			}
			base.Results = new COWResults(this.publicFolderSession, itemIds);
			this.InCallback = true;
			try
			{
				PublicFolderCOWSession.<>c__DisplayClass7 CS$<>8__locals2 = new PublicFolderCOWSession.<>c__DisplayClass7();
				CS$<>8__locals2.CS$<>8__locals5 = CS$<>8__locals1;
				CS$<>8__locals2.action = COWSessionBase.GetTriggerAction(operation);
				if (itemIds.Count == 0 || !COWSessionBase.IsDeleteOperation(CS$<>8__locals2.action))
				{
					ExTraceGlobals.SessionTracer.TraceDebug<FolderChangeOperation, int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Skipping operation {0} for {1} items", operation, itemIds.Count);
					return false;
				}
				CS$<>8__locals2.messagesToSoftDelete = new List<StoreObjectId>(itemIds.Count);
				List<StoreObjectId> list = new List<StoreObjectId>(itemIds.Count);
				CS$<>8__locals2.messageIds = COWSessionBase.InternalFilterItems(itemIds);
				foreach (StoreObjectId storeObjectId in COWSessionBase.InternalFilterFolders(itemIds))
				{
					bool flag = CS$<>8__locals1.sourceFolderId != null && storeObjectId.Equals(CS$<>8__locals1.sourceFolderId);
					if (flag)
					{
						ExTraceGlobals.SessionTracer.TraceError<StoreObjectId>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Folder {0} skipped. We currently don't support EmptyFolder operation for public folders", storeObjectId);
						base.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, new StoreObjectId[]
						{
							storeObjectId
						}, new StoragePermanentException(ServerStrings.ErrorEmptyFolderNotSupported)));
					}
					else
					{
						list.Add(storeObjectId);
					}
				}
				if (CS$<>8__locals2.messageIds.Count > 0 && (flags & FolderChangeOperationFlags.IncludeItems) == FolderChangeOperationFlags.IncludeItems)
				{
					Util.ThrowOnNullArgument(CS$<>8__locals1.sourceFolderId, "sourceFolderId");
					ExTraceGlobals.SessionTracer.TraceDebug<COWTriggerAction, int, StoreObjectId>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Processing action {0} for {1} messages in folder {2}", CS$<>8__locals2.action, CS$<>8__locals2.messageIds.Count, CS$<>8__locals1.sourceFolderId);
					if (CS$<>8__locals2.action == COWTriggerAction.HardDelete)
					{
						if (CS$<>8__locals2.messageIds.Count == itemIds.Count)
						{
							ExTraceGlobals.SessionTracer.TraceDebug<int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Skipping hard delete for all {0} messages", CS$<>8__locals2.messageIds.Count);
							return false;
						}
						this.Execute(delegate()
						{
							using (Folder folder = Folder.Bind(CS$<>8__locals2.CS$<>8__locals5.sourceSession, CS$<>8__locals2.CS$<>8__locals5.sourceFolderId))
							{
								CS$<>8__locals2.CS$<>8__locals5.<>4__this.Results.AddResult(folder.InternalDeleteItems(DeleteItemFlags.HardDelete | DeleteItemFlags.SuppressReadReceipt, CS$<>8__locals2.messageIds.ToArray()));
							}
						}, CS$<>8__locals2.messageIds);
					}
					else
					{
						List<StoreObjectId> messagesFailed = new List<StoreObjectId>(CS$<>8__locals2.messageIds.Count);
						using (List<StoreObjectId>.Enumerator enumerator2 = CS$<>8__locals2.messageIds.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								StoreObjectId itemId = enumerator2.Current;
								if (itemId is OccurrenceStoreObjectId)
								{
									ExTraceGlobals.SessionTracer.TraceDebug<StoreObjectId>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Skipping item {0} since it is an OccurrenceStoreObjectId", itemId);
								}
								else
								{
									this.Execute(delegate()
									{
										using (Item item = Item.Bind(CS$<>8__locals1.<>4__this.publicFolderSession, itemId, PublicFolderCOWSession.propertiesToLoadForItems))
										{
											if ((item.PropertyBag.GetValueOrDefault<EffectiveRights>(StoreObjectSchema.EffectiveRights, EffectiveRights.None) & EffectiveRights.Delete) == EffectiveRights.None)
											{
												ExTraceGlobals.SessionTracer.TraceError<StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - User does not have delete permission for message id {0}", itemId);
												messagesFailed.Add(itemId);
											}
											else
											{
												CS$<>8__locals2.messagesToSoftDelete.Add(itemId);
											}
										}
									}, itemId);
								}
							}
						}
						if (messagesFailed.Count > 0)
						{
							ExTraceGlobals.SessionTracer.TraceError<int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - {0} messages skipped due to insufficient permissions", messagesFailed.Count);
							base.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, messagesFailed, new AccessDeniedException(ServerStrings.NotEnoughPermissionsToPerformOperation)));
						}
					}
				}
				if (CS$<>8__locals2.messagesToSoftDelete.Count == 0 && list.Count == 0)
				{
					ExTraceGlobals.SessionTracer.TraceError((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - No messages or folders to delete");
					return true;
				}
				using (PublicFolderSession publicFolderAdminSession = PublicFolderSession.OpenAsAdmin(this.publicFolderSession.OrganizationId, null, this.publicFolderSession.MailboxGuid, null, this.publicFolderSession.Culture, string.Format("{0};{1}", "Client=PublicFolderSystem", "Action=CopyOnWrite"), this.publicFolderSession.AccountingObject))
				{
					if (CS$<>8__locals2.messagesToSoftDelete.Count > 0)
					{
						ExTraceGlobals.SessionTracer.TraceDebug<int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Soft deleting {0} messages", CS$<>8__locals2.messagesToSoftDelete.Count);
						this.Execute(delegate()
						{
							using (Folder folder = Folder.Bind(publicFolderAdminSession, CS$<>8__locals1.sourceFolderId))
							{
								StoreObjectId recoverableItemsDeletionsFolderId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId((CoreFolder)folder.CoreObject);
								if (recoverableItemsDeletionsFolderId == null)
								{
									ExTraceGlobals.SessionTracer.TraceError<StoreObjectId, int>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - No dumpster found for folder {0}. Skipping move for {1} messages.", CS$<>8__locals1.sourceFolderId, CS$<>8__locals2.messagesToSoftDelete.Count);
									CS$<>8__locals1.<>4__this.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, CS$<>8__locals2.messagesToSoftDelete.ToArray(), new ObjectNotFoundException(ServerStrings.DumpsterFolderNotFound)));
								}
								else
								{
									ExTraceGlobals.SessionTracer.TraceDebug<int, StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Moving {0} messages to dumpster folder {1}", CS$<>8__locals2.messagesToSoftDelete.Count, recoverableItemsDeletionsFolderId);
									CS$<>8__locals1.<>4__this.Results.AddResult(folder.MoveItems(recoverableItemsDeletionsFolderId, CS$<>8__locals2.messagesToSoftDelete.ToArray()));
								}
							}
						}, CS$<>8__locals2.messagesToSoftDelete);
					}
					if (list.Count > 0)
					{
						ExTraceGlobals.SessionTracer.TraceDebug<int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Hard deleting {0} folders", list.Count);
						List<StoreObjectId> mailEnabledFolders = new List<StoreObjectId>(list.Count);
						List<StoreObjectId> accessDeniedFolders = new List<StoreObjectId>(list.Count);
						int nonExistentFoldersCount = 0;
						using (List<StoreObjectId>.Enumerator enumerator3 = list.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								StoreObjectId folderId = enumerator3.Current;
								this.Execute(delegate()
								{
									try
									{
										using (Folder folder = Folder.Bind(CS$<>8__locals1.<>4__this.publicFolderSession, folderId, PublicFolderCOWSession.propertiesToLoadForPublicFolders))
										{
											string arg = folder.PropertyBag.TryGetProperty(FolderSchema.DisplayName) as string;
											if (folder.PropertyBag.GetValueOrDefault<bool>(FolderSchema.MailEnabled, false) || PublicFolderCOWSession.HasMailEnabledSubFolders(folder))
											{
												ExTraceGlobals.SessionTracer.TraceError<string, StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Folder {0} (id={1}) is mail enabled or has sub-folders that are mail-enabled. Skipping hard delete.", arg, folderId);
												mailEnabledFolders.Add(folderId);
											}
											else if ((folder.PropertyBag.GetValueOrDefault<EffectiveRights>(StoreObjectSchema.EffectiveRights, EffectiveRights.None) & EffectiveRights.Delete) == EffectiveRights.None || PublicFolderCOWSession.IsDumpsterFolder((CoreFolder)folder.CoreObject) || !PublicFolderCOWSession.CanDeleteSubFolders(folder))
											{
												ExTraceGlobals.SessionTracer.TraceError<string, StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - User does not have delete permission on folder {0}, id={1} or one of its sub-folders", arg, folderId);
												accessDeniedFolders.Add(folderId);
											}
											else
											{
												if (folder.ParentId != null)
												{
													using (Folder folder2 = Folder.Bind(publicFolderAdminSession, folder.ParentId))
													{
														if (PublicFolderCOWSession.IsDumpsterFolder((CoreFolder)folder2.CoreObject))
														{
															if (CS$<>8__locals2.action == COWTriggerAction.HardDelete)
															{
																ExTraceGlobals.SessionTracer.TraceDebug<StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - HardDeleting a folder {0} which is already under a Dumpster", folderId);
																AggregateOperationResult aggregateOperationResult = folder2.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
																{
																	folderId
																});
																if (aggregateOperationResult.GroupOperationResults.Length > 0)
																{
																	CS$<>8__locals1.<>4__this.Results.AddResult(aggregateOperationResult.GroupOperationResults[0]);
																}
															}
														}
														else
														{
															StoreObjectId recoverableItemsDeletionsFolderId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId((CoreFolder)folder2.CoreObject);
															if (recoverableItemsDeletionsFolderId == null)
															{
																ExTraceGlobals.SessionTracer.TraceError<StoreObjectId, StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - No dumpster found for parent folder {0}. Skipping move of folder {1}.", folder.ParentId, folderId);
																CS$<>8__locals1.<>4__this.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, new StoreObjectId[]
																{
																	folderId
																}, new ObjectNotFoundException(ServerStrings.DumpsterFolderNotFound)));
															}
															else
															{
																ExTraceGlobals.SessionTracer.TraceDebug<StoreObjectId, StoreObjectId, StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Moving folder {0} to dumpster folder {1} of parent folder {2}", folderId, recoverableItemsDeletionsFolderId, folder.ParentId);
																GroupOperationResult groupOperationResult = folder2.MoveFolder(recoverableItemsDeletionsFolderId, folderId);
																CS$<>8__locals1.<>4__this.Results.AddResult(groupOperationResult);
																if (groupOperationResult.OperationResult == OperationResult.Succeeded)
																{
																	CS$<>8__locals1.<>4__this.MoveSubFoldersInDumpsterToFolder(folder, publicFolderAdminSession);
																}
															}
														}
														goto IL_343;
													}
												}
												ExTraceGlobals.SessionTracer.TraceError<StoreObjectId>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Parent folder is null, this should be the Root folder. Skipping move of folder {0}.", folderId);
												CS$<>8__locals1.<>4__this.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, new StoreObjectId[]
												{
													folderId
												}, new ObjectNotFoundException(ServerStrings.CannotDeleteRootFolder)));
											}
											IL_343:;
										}
									}
									catch (ObjectNotFoundException arg2)
									{
										ExTraceGlobals.SessionTracer.TraceWarning<StoreObjectId, ObjectNotFoundException>((long)CS$<>8__locals1.<>4__this.StoreSession.GetHashCode(), "Folder Id {0} was not found. Exception - {1}", folderId, arg2);
										nonExistentFoldersCount++;
									}
								}, folderId);
							}
						}
						if (mailEnabledFolders.Count > 0)
						{
							ExTraceGlobals.SessionTracer.TraceError<int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - {0} folders skipped due to it or its sub-folders being mail enabled ", mailEnabledFolders.Count);
							base.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, mailEnabledFolders, new StoragePermanentException(ServerStrings.ErrorFolderIsMailEnabled)));
						}
						if (accessDeniedFolders.Count > 0)
						{
							ExTraceGlobals.SessionTracer.TraceError<int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - {0} folders skipped due to insufficient permissions", accessDeniedFolders.Count);
							base.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, accessDeniedFolders, new AccessDeniedException(ServerStrings.NotEnoughPermissionsToPerformOperation)));
						}
						if (nonExistentFoldersCount > 0)
						{
							ExTraceGlobals.SessionTracer.TraceError<int>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - {0} folders skipped as they do not exist", nonExistentFoldersCount);
						}
					}
				}
			}
			finally
			{
				this.InCallback = false;
			}
			return true;
		}

		private void MoveSubFoldersInDumpsterToFolder(Folder folder, PublicFolderSession sessionToUse)
		{
			StoreObjectId recoverableItemsDeletionsFolderId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId((CoreFolder)folder.CoreObject);
			if (recoverableItemsDeletionsFolderId != null)
			{
				using (Folder folder2 = Folder.Bind(sessionToUse, recoverableItemsDeletionsFolderId))
				{
					using (QueryResult queryResult = folder2.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
					{
						FolderSchema.Id
					}))
					{
						for (;;)
						{
							object[][] rows = queryResult.GetRows(100);
							if (rows == null || rows.Length == 0)
							{
								break;
							}
							foreach (object[] array2 in rows)
							{
								VersionedId versionedId = array2[0] as VersionedId;
								if (versionedId != null)
								{
									ExTraceGlobals.SessionTracer.TraceDebug<StoreObjectId, StoreObjectId, StoreObjectId>((long)this.StoreSession.GetHashCode(), "PublicFolderCOWSession::OnBeforeFolderChange - Moving sub-folder {0} from dumpster folder {1} to under folder {2}", versionedId.ObjectId, recoverableItemsDeletionsFolderId, folder.Id.ObjectId);
									base.Results.AddResult(folder2.MoveFolder(folder.Id, versionedId));
								}
							}
						}
					}
				}
			}
		}

		private static bool HasMailEnabledSubFolders(Folder folder)
		{
			if (folder.HasSubfolders)
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.MailEnabled, true), null, new PropertyDefinition[]
				{
					FolderSchema.Id
				}))
				{
					return queryResult.EstimatedRowCount > 0;
				}
				return false;
			}
			return false;
		}

		private static bool CanDeleteSubFolders(Folder folder)
		{
			if (folder.HasSubfolders)
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, new OrFilter(new QueryFilter[]
				{
					new BitMaskFilter(StoreObjectSchema.EffectiveRights, 4UL, false),
					new BitMaskFilter(FolderSchema.AdminFolderFlags, 64UL, true)
				}), null, new PropertyDefinition[]
				{
					FolderSchema.Id
				}))
				{
					return queryResult.EstimatedRowCount == 0;
				}
				return true;
			}
			return true;
		}

		private void Execute(PublicFolderCOWSession.Call call, StoreObjectId storeObjectId)
		{
			this.Execute(call, new List<StoreObjectId>(1)
			{
				storeObjectId
			});
		}

		private void Execute(PublicFolderCOWSession.Call call, IList<StoreObjectId> storeObjectIds)
		{
			LocalizedException ex = null;
			try
			{
				call();
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (VirusDetectedException ex3)
			{
				ex = ex3;
			}
			catch (VirusMessageDeletedException ex4)
			{
				ex = ex4;
			}
			catch (VirusException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				ExTraceGlobals.SessionTracer.TraceWarning<LocalizedException>((long)this.publicFolderSession.GetHashCode(), "Got Exception: {0}", ex);
				base.Results.AddResult(new GroupOperationResult(OperationResult.PartiallySucceeded, storeObjectIds, ex));
			}
		}

		internal const string PublicFolderAssistantName = "PublicFolderAssistant";

		private static readonly StorePropertyDefinition[] propertiesToLoadForPublicFolders = new StorePropertyDefinition[]
		{
			InternalSchema.EffectiveRights,
			FolderSchema.MailEnabled,
			FolderSchema.AdminFolderFlags,
			FolderSchema.DisplayName
		};

		private static readonly StorePropertyDefinition[] propertiesToLoadForItems = new StorePropertyDefinition[]
		{
			InternalSchema.EffectiveRights
		};

		private static readonly StorePropertyDefinition[] propertiesToLoadForDumpster = new StorePropertyDefinition[]
		{
			FolderSchema.AdminFolderFlags
		};

		private readonly PublicFolderSession publicFolderSession;

		private delegate void Call();
	}
}
