using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MapiFolder : DisposeTrackableBase, IFolder, IDisposable
	{
		public MapiFolder()
		{
		}

		public MapiFolder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public MapiMailbox Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		FolderRec IFolder.GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			FolderRec folderRec;
			if (flags.HasFlag(GetFolderRecFlags.NoProperties))
			{
				folderRec = new FolderRec();
			}
			else
			{
				folderRec = FolderRec.Create(this.Folder, additionalPtagsToLoad);
				this.GetExtendedProps(folderRec, flags);
				this.TranslateParentFolderId(folderRec);
			}
			folderRec.IsGhosted = (this.Folder.MapiStore != this.Mailbox.MapiStore);
			return folderRec;
		}

		List<MessageRec> IFolder.EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad)
		{
			List<MessageRec> result = new List<MessageRec>();
			if (!this.Folder.IsContentAvailable)
			{
				return result;
			}
			ContentsTableFlags[] array = new ContentsTableFlags[]
			{
				ContentsTableFlags.None,
				ContentsTableFlags.Associated,
				ContentsTableFlags.ShowSoftDeletes,
				ContentsTableFlags.ShowSoftDeletes | ContentsTableFlags.Associated
			};
			ContentsTableFlags[] array2 = new ContentsTableFlags[]
			{
				ContentsTableFlags.None,
				ContentsTableFlags.Associated
			};
			ContentsTableFlags[] array3 = (this.Folder.MapiStore.VersionMajor < 15) ? array : array2;
			for (int i = 0; i < array3.Length; i++)
			{
				ContentsTableFlags flags = ContentsTableFlags.DeferredErrors | array3[i];
				bool doingFAI = (flags & ContentsTableFlags.Associated) != ContentsTableFlags.None;
				bool doingDeletes = (flags & ContentsTableFlags.ShowSoftDeletes) != ContentsTableFlags.None;
				if ((emFlags & ((!doingDeletes || 2 != 0) ? EnumerateMessagesFlags.RegularMessages : ((EnumerateMessagesFlags)0))) != (EnumerateMessagesFlags)0)
				{
					List<PropTag> pta = new List<PropTag>(5);
					int idxEntryId = -1;
					int idxCreationTime = -1;
					int idxMessageClass = -1;
					int idxRuleMsgVersion = -1;
					int idxMessageSize = -1;
					idxEntryId = pta.Count;
					pta.Add(PropTag.EntryId);
					if ((emFlags & EnumerateMessagesFlags.IncludeExtendedData) != (EnumerateMessagesFlags)0)
					{
						idxMessageSize = pta.Count;
						pta.Add(PropTag.MessageSize);
						idxCreationTime = pta.Count;
						pta.Add(PropTag.CreationTime);
					}
					if (doingFAI)
					{
						idxMessageClass = pta.Count;
						pta.Add(PropTag.MessageClass);
						idxRuleMsgVersion = pta.Count;
						pta.Add(PropTag.RuleMsgVersion);
					}
					int idxExtraPtags = pta.Count;
					if (additionalPtagsToLoad != null)
					{
						pta.AddRange(additionalPtagsToLoad);
					}
					MrsTracer.Provider.Debug("MapiFolder.GetContentsTable({0})", new object[]
					{
						flags
					});
					ExecutionContext.Create(new DataContext[]
					{
						new OperationDataContext("MapiFolder.GetContentsTable", OperationType.None),
						new SimpleValueDataContext("Flags", flags)
					}).Execute(delegate
					{
						int lcidValue = (!doingFAI && this.contentsRestriction != null) ? this.contentsRestriction.LCID : 0;
						MapiTable contentsTable;
						using (this.Mailbox.RHTracker.Start())
						{
							contentsTable = this.Folder.GetContentsTable(flags);
						}
						using (contentsTable)
						{
							using (new SortLCIDContext(this.Folder.MapiStore, lcidValue))
							{
								Restriction restriction = null;
								if (!doingFAI && this.contentsRestriction != null)
								{
									restriction = DataConverter<RestrictionConverter, Restriction, RestrictionData>.GetNative(this.contentsRestriction);
								}
								MapiUtils.InitQueryAllRows(contentsTable, restriction, pta);
								for (;;)
								{
									PropValue[][] array4;
									using (this.Mailbox.RHTracker.Start())
									{
										array4 = contentsTable.QueryRows(1000);
									}
									if (array4.GetLength(0) == 0)
									{
										break;
									}
									MrsTracer.Provider.Debug("QueryRows returned {0} items.", new object[]
									{
										array4.Length
									});
									PropValue[][] array5 = array4;
									int j = 0;
									while (j < array5.Length)
									{
										PropValue[] array6 = array5[j];
										if (!doingFAI)
										{
											goto IL_1F9;
										}
										string @string = array6[idxMessageClass].GetString();
										if ((this.Mailbox.Options & MailboxOptions.IgnoreExtendedRuleFAIs) != MailboxOptions.None)
										{
											if (!StringComparer.OrdinalIgnoreCase.Equals(@string, "IPM.Rule.Message") && !StringComparer.OrdinalIgnoreCase.Equals(@string, "IPM.Rule.Version2.Message"))
											{
												if (!StringComparer.OrdinalIgnoreCase.Equals(@string, "IPM.ExtendedRule.Message"))
												{
													goto IL_1F9;
												}
											}
										}
										else if (!StringComparer.OrdinalIgnoreCase.Equals(@string, "IPM.Rule.Message") || array6[idxRuleMsgVersion].GetInt(0) != 1)
										{
											goto IL_1F9;
										}
										IL_423:
										j++;
										continue;
										IL_1F9:
										DateTime dateTime = (idxCreationTime != -1) ? MapiUtils.GetDateTimeOrDefault(array6[idxCreationTime]) : DateTime.MinValue;
										byte[] bytes = array6[idxEntryId].GetBytes();
										if (emFlags.HasFlag(EnumerateMessagesFlags.SkipICSMidSetMissing) && bytes != null && this.Mailbox.SupportsSavingSyncState)
										{
											SyncContentsManifestState syncContentsManifestState = this.Mailbox.SyncState[this.FolderId];
											if (syncContentsManifestState != null && !syncContentsManifestState.IdSetGivenContainsEntryId(bytes))
											{
												MrsTracer.Provider.Debug("entry id {0} with creation time {1} not found in given items.", new object[]
												{
													TraceUtils.DumpEntryId(bytes),
													dateTime
												});
												goto IL_423;
											}
										}
										List<PropValueData> list = null;
										if (additionalPtagsToLoad != null && additionalPtagsToLoad.Length > 0)
										{
											list = new List<PropValueData>();
											for (int k = idxExtraPtags; k < array6.Length; k++)
											{
												list.Add(DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(array6[k]));
											}
										}
										if ((emFlags & EnumerateMessagesFlags.ReturnLongTermIDs) == EnumerateMessagesFlags.ReturnLongTermIDs && bytes != null)
										{
											if (list == null)
											{
												list = new List<PropValueData>();
											}
											list.Add(new PropValueData(PropTag.LTID, this.Mailbox.MapiStore.GlobalIdFromId(this.Mailbox.MapiStore.GetMidFromMessageEntryId(bytes))));
										}
										MsgRecFlags msgRecFlags = doingFAI ? MsgRecFlags.Associated : MsgRecFlags.None;
										if (doingDeletes)
										{
											msgRecFlags |= MsgRecFlags.Deleted;
										}
										MessageRec item = new MessageRec(bytes, this.FolderId, dateTime, (idxMessageSize != -1) ? array6[idxMessageSize].GetInt(1000) : 1000, msgRecFlags, (list == null) ? null : list.ToArray());
										result.Add(item);
										goto IL_423;
									}
								}
							}
						}
					});
				}
			}
			MrsTracer.Provider.Debug("MapiFolder.EnumerateMessages returns {0} items.", new object[]
			{
				result.Count
			});
			return result;
		}

		RawSecurityDescriptor IFolder.GetSecurityDescriptor(SecurityProp secProp)
		{
			MrsTracer.Provider.Function("MapiFolder.GetSecurityDescriptor", new object[0]);
			if (!this.HasSecurityDescriptor(secProp))
			{
				return null;
			}
			RawSecurityDescriptor securityDescriptor;
			using (this.Mailbox.RHTracker.Start())
			{
				securityDescriptor = this.Folder.GetSecurityDescriptor(secProp);
			}
			return securityDescriptor;
		}

		void IFolder.DeleteMessages(byte[][] entryIds)
		{
			MapiUtils.ProcessMapiCallInBatches<byte[]>(entryIds, delegate(byte[][] batch)
			{
				using (this.Mailbox.RHTracker.Start())
				{
					this.Folder.DeleteMessages(DeleteMessagesFlags.ForceHardDelete, batch);
				}
			});
		}

		byte[] IFolder.GetFolderId()
		{
			return this.FolderId;
		}

		void IFolder.SetContentsRestriction(RestrictionData restriction)
		{
			this.contentsRestriction = restriction;
		}

		PropValueData[] IFolder.GetProps(PropTag[] pta)
		{
			PropValueData[] data;
			using (this.Mailbox.RHTracker.Start())
			{
				data = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(this.Folder.GetProps(pta));
			}
			return data;
		}

		void IFolder.GetSearchCriteria(out RestrictionData restriction, out byte[][] entryIds, out SearchState state)
		{
			MrsTracer.Provider.Function("MapiFolder.GetSearchCriteria", new object[0]);
			try
			{
				Restriction native;
				using (this.Mailbox.RHTracker.Start())
				{
					this.Folder.GetSearchCriteria(out native, out entryIds, out state);
				}
				restriction = DataConverter<RestrictionConverter, Restriction, RestrictionData>.GetData(native);
			}
			catch (MapiExceptionNotInitialized ex)
			{
				MrsTracer.Provider.Warning("GetSearchCriteria failed with ecNotInitialized, ignoring.\n{0}", new object[]
				{
					CommonUtils.FullExceptionMessage(ex)
				});
				restriction = null;
				entryIds = null;
				state = SearchState.None;
			}
		}

		RuleData[] IFolder.GetRules(PropTag[] extraProps)
		{
			if (!this.Folder.IsContentAvailable)
			{
				return Array<RuleData>.Empty;
			}
			RuleData[] result;
			using (this.Mailbox.RHTracker.Start())
			{
				FolderType @int = (FolderType)this.Folder.GetProp(PropTag.FolderType).GetInt(1);
				if (@int == FolderType.Search)
				{
					result = Array<RuleData>.Empty;
				}
				else
				{
					result = DataConverter<RuleConverter, Rule, RuleData>.GetData(this.Folder.GetRules(extraProps));
				}
			}
			return result;
		}

		PropValueData[][] IFolder.GetACL(SecurityProp secProp)
		{
			if (!this.HasSecurityDescriptor(secProp))
			{
				return null;
			}
			if (this.Mailbox.ServerVersion >= Server.E15MinVersion)
			{
				MrsTracer.Provider.Warning("MAPI provider does not support GetACL against E15+ mailboxes", new object[0]);
				return null;
			}
			PropValueData[][] result;
			using (this.Mailbox.RHTracker.Start())
			{
				using (MapiModifyTable mapiModifyTable = (MapiModifyTable)this.Folder.OpenProperty(PropTag.AclTable, InterfaceIds.IExchangeModifyTable, 0, OpenPropertyFlags.DeferredErrors))
				{
					GetTableFlags getTableFlags = GetTableFlags.DeferredErrors;
					if (secProp == SecurityProp.FreeBusyNTSD)
					{
						getTableFlags |= GetTableFlags.FreeBusy;
					}
					using (MapiTable table = mapiModifyTable.GetTable(getTableFlags))
					{
						PropTag[] propTags = new PropTag[]
						{
							PropTag.EntryId,
							PropTag.MemberId,
							PropTag.MemberRights,
							PropTag.MemberName
						};
						table.SeekRow(BookMark.Beginning, 0);
						PropValue[][] array = MapiUtils.QueryAllRows(table, null, propTags);
						PropValueData[][] array2 = new PropValueData[array.Length][];
						int num = 0;
						foreach (PropValue[] a in array)
						{
							array2[num++] = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(a);
						}
						result = array2;
					}
				}
			}
			return result;
		}

		PropValueData[][] IFolder.GetExtendedAcl(AclFlags aclFlags)
		{
			throw new NotImplementedException();
		}

		List<MessageRec> IFolder.LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			EntryIdMap<MessageRec> result = new EntryIdMap<MessageRec>();
			ContentsTableFlags[] array = new ContentsTableFlags[]
			{
				ContentsTableFlags.None,
				ContentsTableFlags.Associated
			};
			List<PropTag> pta = new List<PropTag>(5);
			int idxEntryId = pta.Count;
			pta.Add(PropTag.EntryId);
			int idxMessageSize = pta.Count;
			pta.Add(PropTag.MessageSize);
			int idxCreationTime = pta.Count;
			pta.Add(PropTag.CreationTime);
			int idxExtraPtags = pta.Count;
			if (additionalPtagsToLoad != null)
			{
				pta.AddRange(additionalPtagsToLoad);
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (result.Count >= keysToLookup.Count)
				{
					break;
				}
				ContentsTableFlags flags = ContentsTableFlags.DeferredErrors | array[i];
				bool doingFAI = (flags & ContentsTableFlags.Associated) != ContentsTableFlags.None;
				ExecutionContext.Create(new DataContext[]
				{
					new OperationDataContext("MapiFolder.GetContentsTable", OperationType.None),
					new SimpleValueDataContext("Flags", flags)
				}).Execute(delegate
				{
					MapiTable msgTable;
					using (this.Mailbox.RHTracker.Start())
					{
						msgTable = this.Folder.GetContentsTable(flags);
					}
					using (msgTable)
					{
						if (ptagToLookup != PropTag.EntryId)
						{
							using (this.Mailbox.RHTracker.Start())
							{
								msgTable.SortTable(new SortOrder(ptagToLookup, SortFlags.Ascend), SortTableFlags.None);
							}
						}
						msgTable.SetColumns(pta);
						using (List<byte[]>.Enumerator enumerator = keysToLookup.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								byte[] key = enumerator.Current;
								if (!result.ContainsKey(key))
								{
									ExecutionContext.Create(new DataContext[]
									{
										new EntryIDsDataContext(key)
									}).Execute(delegate
									{
										try
										{
											using (this.Mailbox.RHTracker.Start())
											{
												if (!msgTable.FindRow(Restriction.EQ(ptagToLookup, key), BookMark.Beginning, FindRowFlag.None))
												{
													return;
												}
											}
										}
										catch (MapiExceptionNotFound)
										{
											return;
										}
										PropValue[][] array2;
										using (this.Mailbox.RHTracker.Start())
										{
											array2 = msgTable.QueryRows(1);
										}
										if (array2.Length == 1)
										{
											PropValue[] array3 = array2[0];
											if (array3.Length != pta.Count)
											{
												return;
											}
											PropValueData[] array4 = null;
											if (additionalPtagsToLoad != null && additionalPtagsToLoad.Length > 0)
											{
												array4 = new PropValueData[additionalPtagsToLoad.Length];
												for (int j = idxExtraPtags; j < array3.Length; j++)
												{
													array4[j - idxExtraPtags] = DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(array3[j]);
												}
											}
											MessageRec value = new MessageRec(array3[idxEntryId].GetBytes(), this.FolderId, MapiUtils.GetDateTimeOrDefault(array3[idxCreationTime]), array3[idxMessageSize].GetInt(1000), doingFAI ? MsgRecFlags.Associated : MsgRecFlags.None, array4);
											result.Add(key, value);
											return;
										}
									});
								}
							}
						}
					}
				});
			}
			MrsTracer.Provider.Debug("MapiFolder.LookupMessages returns {0} items.", new object[]
			{
				result.Count
			});
			return new List<MessageRec>(result.Values);
		}

		PropProblemData[] IFolder.SetProps(PropValueData[] pvda)
		{
			PropProblemData[] result = null;
			PropValue[] native = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(pvda);
			PropValue[] array = new PropValue[native.Length];
			for (int i = 0; i < native.Length; i++)
			{
				PropTag propTag = native[i].PropTag;
				object obj = native[i].Value;
				if (propTag == PropTag.IpmWasteBasketEntryId)
				{
					byte[] array2 = obj as byte[];
					if (array2 != null)
					{
						obj = this.Mailbox.MapiStore.GlobalIdFromId(this.Mailbox.MapiStore.GetFidFromEntryId(array2));
					}
				}
				array[i] = new PropValue(propTag, obj);
			}
			using (this.Mailbox.RHTracker.Start())
			{
				result = DataConverter<PropProblemConverter, PropProblem, PropProblemData>.GetData(this.Folder.SetProps(array));
			}
			return result;
		}

		internal virtual void Config(byte[] folderId, MapiFolder folder, MapiMailbox mailbox)
		{
			this.folderId = folderId;
			this.folder = folder;
			this.mailbox = mailbox;
			this.contentsRestriction = null;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiFolder>(this);
		}

		protected void GetExtendedProps(FolderRec folderRec, GetFolderRecFlags flags)
		{
			MrsTracer.Provider.Function("MapiFolder.GetExtendedProps", new object[0]);
			if ((flags & GetFolderRecFlags.PromotedProperties) != GetFolderRecFlags.None)
			{
				PropValue prop;
				using (this.Mailbox.RHTracker.Start())
				{
					prop = this.Folder.GetProp(PropTag.PromotedProperties);
				}
				if (!prop.IsNull() && !prop.IsError())
				{
					PropTag[] array = PropTagHelper.PropTagArray(prop.GetBytes());
					folderRec.SetPromotedProperties(array);
					MrsTracer.Provider.Debug("Found {0} promoted properties.", new object[]
					{
						array.Length
					});
				}
				else
				{
					folderRec.SetPromotedProperties(null);
					MrsTracer.Provider.Debug("Source server does not support promoted property retrieval.", new object[0]);
				}
			}
			if ((flags & (GetFolderRecFlags.Views | GetFolderRecFlags.Restrictions)) != GetFolderRecFlags.None)
			{
				this.Mailbox.GetFolderViewsOrRestrictions(folderRec, flags, this.FolderId);
			}
		}

		private bool HasSecurityDescriptor(SecurityProp secProp)
		{
			if (secProp != SecurityProp.NTSD && this.mailbox.IsTitanium)
			{
				return false;
			}
			if (secProp == SecurityProp.FreeBusyNTSD)
			{
				PropTag[] propList;
				using (this.Mailbox.RHTracker.Start())
				{
					propList = this.Folder.GetPropList();
				}
				bool flag = false;
				foreach (PropTag propTag in propList)
				{
					if (propTag == PropTag.FreeBusyNTSD)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private void TranslateParentFolderId(FolderRec folderRec)
		{
			if (this.Mailbox.IsPublicFolderMigrationSource && this.Folder.MapiStore != this.Mailbox.MapiStore && folderRec.ParentId != null)
			{
				PropTag[] array = new PropTag[]
				{
					PropTag.RootEntryId,
					PropTag.IpmSubtreeEntryId,
					PropTag.NonIpmSubtreeEntryId,
					PropTag.EFormsRegistryEntryId,
					PropTag.LocaleEFormsRegistryEntryId,
					PropTag.FreeBusyEntryId,
					PropTag.LocalSiteFreeBusyEntryId,
					PropTag.OfflineAddressBookEntryId,
					PropTag.LocalSiteOfflineAddressBookEntryId
				};
				foreach (PropTag tag in array)
				{
					if (CommonUtils.IsSameEntryId(folderRec.ParentId, this.Folder.MapiStore.GetProp(tag).GetBytes()))
					{
						folderRec.ParentId = this.Mailbox.MapiStore.GetProp(tag).GetBytes();
						return;
					}
				}
			}
		}

		private byte[] folderId;

		private MapiFolder folder;

		private MapiMailbox mailbox;

		private RestrictionData contentsRestriction;
	}
}
