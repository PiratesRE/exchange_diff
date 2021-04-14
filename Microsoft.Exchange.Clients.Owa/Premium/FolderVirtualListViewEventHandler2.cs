using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class FolderVirtualListViewEventHandler2 : VirtualListViewEventHandler2
	{
		protected Folder ContextFolder
		{
			get
			{
				return this.folder;
			}
		}

		protected Folder DataFolderForSearch
		{
			get
			{
				return this.filteredFolder ?? this.folder;
			}
		}

		protected Folder FilteredFolder
		{
			get
			{
				return this.filteredFolder;
			}
		}

		protected Folder DataFolder
		{
			get
			{
				Folder result;
				if ((result = this.searchFolder) == null)
				{
					result = (this.filteredFolder ?? this.folder);
				}
				return result;
			}
		}

		protected QueryFilter FolderQueryFilter
		{
			get
			{
				return this.folderQueryFilter;
			}
			set
			{
				this.folderQueryFilter = value;
			}
		}

		protected SearchScope SearchScope
		{
			get
			{
				if (this.folder is SearchFolder)
				{
					return SearchScope.AllFoldersAndItems;
				}
				if (this.IsFilteredView)
				{
					return SearchScope.SelectedFolder;
				}
				if (this.IsFiltered)
				{
					FolderVirtualListViewSearchFilter folderVirtualListViewSearchFilter = (FolderVirtualListViewSearchFilter)base.GetParameter("srchf");
					return folderVirtualListViewSearchFilter.Scope;
				}
				return SearchScope.SelectedFolder;
			}
		}

		protected bool IsFilteredView
		{
			get
			{
				return base.IsParameterSet("fldrfltr") && this.FilterCondition.IsValidFilter();
			}
		}

		protected FolderVirtualListViewFilter FilterCondition
		{
			get
			{
				if (this.filterObject == null)
				{
					this.filterObject = (FolderVirtualListViewFilter)base.GetParameter("fldrfltr");
					if (this.filterObject != null)
					{
						if (!string.IsNullOrEmpty(this.filterObject.To) && this.filterObject.To.Length > 255)
						{
							this.filterObject.To = this.filterObject.To.Substring(0, 255);
						}
						if (!string.IsNullOrEmpty(this.filterObject.From) && this.filterObject.From.Length > 255)
						{
							this.filterObject.From = this.filterObject.From.Substring(0, 255);
						}
					}
				}
				return this.filterObject;
			}
		}

		protected bool IsFiltered
		{
			get
			{
				return base.IsParameterSet("srchf");
			}
		}

		protected override VirtualListViewState ListViewState
		{
			get
			{
				return (FolderVirtualListViewState)base.GetParameter("St");
			}
		}

		[OwaEventParameter("SR", typeof(int))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEvent("LoadFresh")]
		public override void LoadFresh()
		{
			base.InternalLoadFresh();
		}

		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEvent("LoadNext")]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		public override void LoadNext()
		{
			base.InternalLoadNext();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEvent("LoadPrevious")]
		public override void LoadPrevious()
		{
			base.InternalLoadPrevious();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		[OwaEvent("SeekNext")]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		public override void SeekNext()
		{
			base.InternalSeekNext();
		}

		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEvent("SeekPrevious")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		public override void SeekPrevious()
		{
			base.InternalSeekPrevious();
		}

		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEvent("Sort")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId), false, true)]
		public override void Sort()
		{
			base.InternalSort();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SR", typeof(int))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEvent("SetML")]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		public override void SetMultiLineState()
		{
			base.InternalSetMultiLineState();
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("fShwELC", typeof(int))]
		[OwaEvent("PersistELCCommentState")]
		public void PersistELCCommentState()
		{
			if (base.UserContext.IsWebPartRequest)
			{
				return;
			}
			int num = (int)base.GetParameter("fShwELC");
			this.BindToFolder();
			int num2 = 0;
			object obj = this.folder.TryGetProperty(FolderSchema.ExtendedFolderFlags);
			if (!(obj is PropertyError))
			{
				num2 = (int)obj;
			}
			if (num == 0)
			{
				num2 |= 32;
			}
			else
			{
				num2 &= -33;
			}
			this.folder[FolderSchema.ExtendedFolderFlags] = num2;
			this.folder.Save();
		}

		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("td", typeof(string))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEvent("TypeDown")]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		public override void TypeDownSearch()
		{
			base.InternalTypeDownSearch();
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RTOcc", typeof(OwaStoreObjectId), true, true)]
		[OwaEvent("Delete")]
		[OwaEventParameter("Itms", typeof(DeleteItemInfo), true)]
		public virtual void Delete()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.Delete");
			this.InternalDelete(false, null);
		}

		[OwaEventParameter("RTOcc", typeof(OwaStoreObjectId), true, true)]
		[OwaEvent("PermanentDelete")]
		[OwaEventParameter("Itms", typeof(DeleteItemInfo), true)]
		[OwaEventParameter("JS", typeof(JunkEmailStatus), false, true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		public virtual void PermanentDelete()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.PermanentDelete");
			this.InternalDelete(true, null);
		}

		[OwaEventParameter("fAddToMRU", typeof(bool), false, true)]
		[OwaEvent("Move")]
		[OwaEventParameter("id", typeof(OwaStoreObjectId), true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("destId", typeof(OwaStoreObjectId))]
		public virtual void Move()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.Move");
			this.CheckSizeOfBatchOperation(null);
			this.CopyOrMoveItems(false, null);
		}

		[OwaEvent("Copy")]
		[OwaEventParameter("destId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("id", typeof(OwaStoreObjectId), true)]
		[OwaEventParameter("fAddToMRU", typeof(bool), false, true)]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId), false, true)]
		public virtual void Copy()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.Copy");
			this.CheckSizeOfBatchOperation(null);
			this.CopyOrMoveItems(true, null);
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("AddToMru")]
		public void AddToMru()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.AddToMru");
			this.Writer.Write('[');
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			IList<TargetFolderMRUEntry> list = TargetFolderMRU.AddAndGetFolders(folderId, base.UserContext);
			for (int i = 0; i < list.Count; i++)
			{
				if (i > 0)
				{
					this.Writer.Write(',');
				}
				this.Writer.Write('"');
				this.Writer.Write(Utilities.JavascriptEncode(list[i].FolderId));
				this.Writer.Write('"');
			}
			this.Writer.Write(']');
		}

		[OwaEvent("GetCopyMoveMenu")]
		public void GetCopyMoveMenu()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.GetCopyMoveMenu");
			CopyMoveContextMenu copyMoveContextMenu = new CopyMoveContextMenu(base.UserContext);
			copyMoveContextMenu.Render(this.Writer);
		}

		[OwaEvent("PersistWidth")]
		[OwaEventParameter("fId", typeof(ObjectId))]
		[OwaEventParameter("w", typeof(int))]
		public void PersistWidth()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.PersistWidth");
			this.PersistWidthOrHeight(true);
		}

		[OwaEventParameter("h", typeof(int))]
		[OwaEventParameter("fId", typeof(ObjectId))]
		[OwaEvent("PersistHeight")]
		public void PersistHeight()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.PersistHeight");
			this.PersistWidthOrHeight(false);
		}

		[OwaEvent("PersistReadingPane")]
		[OwaEventParameter("s", typeof(ReadingPanePosition))]
		[OwaEventParameter("fId", typeof(ObjectId))]
		public virtual void PersistReadingPane()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.PersistReadingPane");
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			using (this.folder = Utilities.GetFolder<Folder>(base.UserContext, folderId, new PropertyDefinition[]
			{
				StoreObjectSchema.EffectiveRights
			}))
			{
				if (Utilities.IsPublic(this.folder) || Utilities.IsOtherMailbox(this.folder) || Utilities.CanModifyFolderProperties(this.folder))
				{
					FolderViewStates folderViewStates = base.UserContext.GetFolderViewStates(this.folder);
					folderViewStates.ReadingPanePosition = (ReadingPanePosition)base.GetParameter("s");
					folderViewStates.Save();
				}
			}
		}

		[OwaEventParameter("isNewestOnTop", typeof(bool))]
		[OwaEvent("PersistReadingPaneSortOrder")]
		public virtual void PersistReadingPaneSortOrder()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.PersistReadingPaneSortOrder");
			bool flag = (bool)base.GetParameter("isNewestOnTop");
			base.UserContext.UserOptions.ConversationSortOrder = (flag ? ConversationSortOrder.ChronologicalNewestOnTop : ConversationSortOrder.ChronologicalNewestOnBottom);
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEventParameter("showTreeInListView", typeof(bool))]
		[OwaEvent("PersistShowTreeInListView")]
		public virtual void PersistShowTree()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.PersistShowTree");
			bool showTreeInListView = (bool)base.GetParameter("showTreeInListView");
			base.UserContext.UserOptions.ShowTreeInListView = showTreeInListView;
			base.UserContext.UserOptions.CommitChanges();
		}

		internal void InternalRespondToMeetingRequest(MeetingRequest meetingRequest, FolderVirtualListViewEventHandler2.RespondToMeetingRequestDelegate dele)
		{
			try
			{
				dele();
			}
			catch (CorrelationFailedException ex)
			{
				if (ex.InnerException is NotSupportedWithServerVersionException)
				{
					string displayName = meetingRequest.ReceivedRepresenting.DisplayName;
					throw new OwaRespondOlderVersionMeetingException("Cannot respond to meeting request shared from older version", displayName);
				}
				throw;
			}
		}

		[OwaEventParameter("Rsp", typeof(ResponseType))]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		[OwaEvent("ER")]
		public virtual void EditMeetingResponse()
		{
			FolderVirtualListViewEventHandler2.<>c__DisplayClass1 CS$<>8__locals1 = new FolderVirtualListViewEventHandler2.<>c__DisplayClass1();
			CS$<>8__locals1.<>4__this = this;
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.EditMeetingResponse");
			CS$<>8__locals1.owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			if (CS$<>8__locals1.owaStoreObjectId.IsPublic)
			{
				throw new OwaInvalidRequestException("Cannot response to a meeting request from public folder");
			}
			CS$<>8__locals1.responseType = (ResponseType)base.GetParameter("Rsp");
			using (MeetingRequest meetingRequest = Utilities.GetItem<MeetingRequest>(base.UserContext, CS$<>8__locals1.owaStoreObjectId, new PropertyDefinition[0]))
			{
				meetingRequest.OpenAsReadWrite();
				this.InternalRespondToMeetingRequest(meetingRequest, delegate
				{
					using (MeetingResponse meetingResponse = MeetingUtilities.EditResponse(CS$<>8__locals1.responseType, meetingRequest))
					{
						meetingResponse.Load();
						meetingRequest.Load();
						CS$<>8__locals1.<>4__this.Writer.Write("?ae=Item&a=New&s=Draft&t=");
						CS$<>8__locals1.<>4__this.Writer.Write(Utilities.UrlEncode(meetingResponse.ClassName));
						CS$<>8__locals1.<>4__this.Writer.Write("&id=");
						CS$<>8__locals1.<>4__this.Writer.Write(Utilities.UrlEncode(Utilities.GetIdAsString(meetingResponse)));
						CS$<>8__locals1.<>4__this.Writer.Write("&r=");
						TextWriter writer = CS$<>8__locals1.<>4__this.Writer;
						int responseType = (int)CS$<>8__locals1.responseType;
						writer.Write(responseType.ToString(CultureInfo.InvariantCulture));
						CS$<>8__locals1.<>4__this.Writer.Write("&mid=");
						CS$<>8__locals1.<>4__this.Writer.Write(Utilities.UrlEncode(CS$<>8__locals1.owaStoreObjectId.ToString()));
						if (Utilities.IsItemInDefaultFolder(meetingRequest, DefaultFolderType.DeletedItems))
						{
							CS$<>8__locals1.<>4__this.Writer.Write("&d=1");
						}
					}
				});
			}
		}

		[OwaEvent("SR")]
		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("Rsp", typeof(ResponseType))]
		public virtual void SendMeetingResponse()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.SendMeetingResponse");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			using (MeetingRequest meetingRequest = Utilities.GetItem<MeetingRequest>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]))
			{
				meetingRequest.OpenAsReadWrite();
				this.InternalRespondToMeetingRequest(meetingRequest, delegate
				{
					MeetingUtilities.NonEditResponse((ResponseType)this.GetParameter("Rsp"), meetingRequest, true);
				});
			}
		}

		[OwaEventParameter("id", typeof(OwaStoreObjectId))]
		[OwaEvent("DR")]
		[OwaEventParameter("Rsp", typeof(ResponseType))]
		public virtual void DontSendMeetingResponse()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.DontSendMeetingResponse");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("id");
			using (MeetingRequest meetingRequest = Utilities.GetItem<MeetingRequest>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]))
			{
				meetingRequest.OpenAsReadWrite();
				this.InternalRespondToMeetingRequest(meetingRequest, delegate
				{
					MeetingUtilities.NonEditResponse((ResponseType)this.GetParameter("Rsp"), meetingRequest, false);
				});
			}
		}

		[OwaEvent("flgItms")]
		[OwaEventParameter("flga", typeof(FlagAction))]
		[OwaEventParameter("Itms", typeof(ObjectId), true)]
		[OwaEventParameter("ddt", typeof(ExDateTime), false, true)]
		public void FlagItems()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.FlagItems");
			FlagAction flagAction = (FlagAction)base.GetParameter("flga");
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("Itms");
			ExDateTime? dueDate = null;
			if (array.Length == 0)
			{
				throw new OwaInvalidRequestException("There must be at least one item to flag");
			}
			bool[] array2 = new bool[array.Length];
			if (flagAction == FlagAction.SpecificDate)
			{
				if (!base.IsParameterSet("ddt"))
				{
					throw new OwaInvalidRequestException("Due date must be provided if specifying a specific due date");
				}
				dueDate = new ExDateTime?((ExDateTime)base.GetParameter("ddt"));
			}
			for (int i = 0; i < array.Length; i++)
			{
				OwaStoreObjectId[] array3 = null;
				if (array[i].IsConversationId)
				{
					OwaStoreObjectId owaStoreObjectId = array[i];
					MailboxSession mailboxSession = (MailboxSession)owaStoreObjectId.GetSession(base.UserContext);
					Conversation conversation = Conversation.Load(mailboxSession, owaStoreObjectId.ConversationId, base.UserContext.IsIrmEnabled, new PropertyDefinition[]
					{
						ItemSchema.Id,
						StoreObjectSchema.ParentItemId,
						ItemSchema.ReceivedTime,
						ItemSchema.FlagStatus
					});
					conversation.ConversationTree.Sort(ConversationTreeSortOrder.ChronologicalDescending);
					bool flag = false;
					if (flagAction == FlagAction.MarkComplete || flagAction == FlagAction.ClearFlag)
					{
						FlagStatus[] flagsStatus = (flagAction == FlagAction.ClearFlag) ? new FlagStatus[]
						{
							FlagStatus.Complete,
							FlagStatus.Flagged
						} : new FlagStatus[]
						{
							FlagStatus.Flagged
						};
						IList<StoreObjectId> flagedItems = ConversationUtilities.GetFlagedItems(mailboxSession, conversation, owaStoreObjectId.ParentFolderId, flagsStatus);
						if (flagedItems.Count > 0)
						{
							array3 = new OwaStoreObjectId[flagedItems.Count];
							for (int j = 0; j < array3.Length; j++)
							{
								array3[j] = OwaStoreObjectId.CreateFromStoreObjectId(flagedItems[j], owaStoreObjectId);
							}
							flag = true;
						}
					}
					if (flagAction != FlagAction.ClearFlag && !flag)
					{
						StoreObjectId latestMessage = ConversationUtilities.GetLatestMessage(mailboxSession, conversation, owaStoreObjectId.ParentFolderId);
						if (latestMessage != null)
						{
							array3 = new OwaStoreObjectId[]
							{
								OwaStoreObjectId.CreateFromStoreObjectId(latestMessage, owaStoreObjectId)
							};
						}
					}
				}
				else
				{
					array3 = new OwaStoreObjectId[]
					{
						array[i]
					};
				}
				if (array3 != null)
				{
					foreach (OwaStoreObjectId owaStoreObjectId2 in array3)
					{
						if (owaStoreObjectId2 == null)
						{
							array2[i] = false;
						}
						else
						{
							try
							{
								using (Item item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId2, new PropertyDefinition[]
								{
									ItemSchema.UtcDueDate
								}))
								{
									MessageItem messageItem = item as MessageItem;
									if (messageItem == null || !messageItem.IsDraft)
									{
										if (!(item is CalendarItemBase))
										{
											if (item is Task && flagAction == FlagAction.ClearFlag)
											{
												OperationResult operationResult = Utilities.Delete(base.UserContext, Utilities.IsPublic(item), new OwaStoreObjectId[]
												{
													OwaStoreObjectId.CreateFromStoreObject(item)
												}).OperationResult;
												array2[i] = (operationResult == OperationResult.Succeeded);
											}
											else
											{
												item.OpenAsReadWrite();
												switch (flagAction)
												{
												case FlagAction.MarkComplete:
													FlagEventHandler.FlagComplete(item);
													break;
												case FlagAction.ClearFlag:
													FlagEventHandler.ClearFlag(item);
													break;
												default:
													dueDate = FlagEventHandler.SetFlag(item, flagAction, dueDate);
													break;
												}
												item.Save(SaveMode.ResolveConflicts);
												array2[i] = true;
											}
										}
									}
								}
							}
							catch (StorageTransientException)
							{
								array2[i] = false;
							}
							catch (StoragePermanentException)
							{
								array2[i] = false;
							}
						}
					}
				}
			}
			this.Writer.Write("var dtDD = ");
			if (dueDate != null)
			{
				this.Writer.Write("new Date(\"");
				this.Writer.Write(DateTimeUtilities.GetJavascriptDate(dueDate.Value));
				this.Writer.Write("\");");
			}
			else
			{
				this.Writer.Write("0;");
			}
			this.RenderGroupOperationResult(array2, -115311901, -1537113578);
		}

		[OwaEvent("catItms")]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("catAdd", typeof(string), true, true)]
		[OwaEventParameter("catRem", typeof(string), true, true)]
		[OwaEventParameter("Itms", typeof(OwaStoreObjectId), true)]
		public void CategorizeItems()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.CategorizeItems");
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("Itms");
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			string[] addCategories = (string[])base.GetParameter("catAdd");
			string[] removeCategories = (string[])base.GetParameter("catRem");
			bool[] array2 = new bool[array.Length];
			this.Writer.Write("var rgItmCats = new Array(");
			for (int i = 0; i < array.Length; i++)
			{
				string s = string.Empty;
				string[] array3 = null;
				try
				{
					OwaStoreObjectId owaStoreObjectId;
					if (array[i].IsConversationId)
					{
						owaStoreObjectId = this.UpdateConversationCategories(array[i], folderId, addCategories, removeCategories);
					}
					else
					{
						owaStoreObjectId = array[i];
					}
					using (Item item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]))
					{
						item.OpenAsReadWrite();
						CategoryContextMenu.ModifyCategories(item, addCategories, removeCategories);
						array3 = ItemUtility.GetProperty<string[]>(item, ItemSchema.Categories, null);
						StringBuilder stringBuilder = new StringBuilder();
						StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
						CategorySwatch.RenderViewCategorySwatches(stringWriter, base.UserContext, item, folderId);
						stringWriter.Close();
						s = stringBuilder.ToString();
						MeetingMessage meetingMessage = item as MeetingMessage;
						if (meetingMessage != null)
						{
							CalendarItemBase calendarItemBase = MeetingUtilities.TryGetCorrelatedItem(meetingMessage);
							if (calendarItemBase != null && !(calendarItemBase is CalendarItemOccurrence))
							{
								CategoryContextMenu.ModifyCategories(calendarItemBase, addCategories, removeCategories);
								Utilities.SaveItem(calendarItemBase);
							}
						}
						item.Save(SaveMode.ResolveConflicts);
						array2[i] = true;
					}
				}
				catch (StorageTransientException)
				{
					array2[i] = false;
				}
				catch (StoragePermanentException)
				{
					array2[i] = false;
				}
				finally
				{
					if (0 < i)
					{
						this.Writer.Write(",");
					}
					this.Writer.Write("ci(\"");
					Utilities.JavascriptEncode(s, this.Writer);
					this.Writer.Write("\",\"");
					if (array3 != null)
					{
						for (int j = 0; j < array3.Length; j++)
						{
							if (j != 0)
							{
								this.Writer.Write("; ");
							}
							Utilities.JavascriptEncode(array3[j], this.Writer);
						}
					}
					this.Writer.Write("\")");
				}
			}
			this.Writer.Write(");");
			this.RenderGroupOperationResult(array2, -1750619075, 1748200060);
		}

		[OwaEventParameter("Itms", typeof(OwaStoreObjectId), true)]
		[OwaEvent("catClr")]
		public void ClearCategories()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.ClearCategories");
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("Itms");
			bool[] array2 = new bool[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				List<OwaStoreObjectId> list = new List<OwaStoreObjectId>();
				if (array[i].IsConversationId)
				{
					ConversationUtilities.ClearAlwaysCategorizeRule(base.UserContext, array[i]);
					list.AddRange(ConversationUtilities.GetAllItemIds(base.UserContext, new OwaStoreObjectId[]
					{
						array[i]
					}, new StoreObjectId[0]));
				}
				else
				{
					list.Add(array[i]);
				}
				try
				{
					foreach (OwaStoreObjectId owaStoreObjectId in list)
					{
						using (Item item = Utilities.GetItem<Item>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]))
						{
							item.OpenAsReadWrite();
							CategoryContextMenu.ClearCategories(item);
							item.Save(SaveMode.ResolveConflicts);
							array2[i] = true;
						}
					}
				}
				catch (StorageTransientException)
				{
					array2[i] = false;
				}
				catch (StoragePermanentException)
				{
					array2[i] = false;
				}
			}
			this.Writer.Write("var sECS = \"");
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
			CategorySwatch.RenderSwatch(stringWriter, null);
			stringWriter.Close();
			Utilities.JavascriptEncode(stringBuilder.ToString(), this.Writer);
			this.Writer.Write("\";");
			this.RenderGroupOperationResult(array2, 1991025408, -103531289);
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEvent("gtAF")]
		public void GetAdvancedFind()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.GetAdvancedFind");
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			this.RenderAdvancedFind(this.Writer, folderId);
		}

		[OwaEvent("gtSIPF")]
		[OwaEventVerb(OwaEventVerb.Get)]
		public void GetSearchInPublicFolder()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.GetSearchInPublicFolder");
			this.RenderSearchInPublicFolder(this.Writer);
		}

		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("GetSearchScopeMenu")]
		[OwaEventVerb(OwaEventVerb.Get)]
		public void GetSearchScopeMenu()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.GetSearchScopeMenu");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("fId");
			OutlookModule moduleForFolder;
			using (this.folder = Utilities.GetFolder<Folder>(base.UserContext, owaStoreObjectId, new PropertyDefinition[0]))
			{
				moduleForFolder = Utilities.GetModuleForFolder(this.folder, base.UserContext);
			}
			SearchScope searchScope = base.UserContext.UserOptions.GetSearchScope(moduleForFolder);
			this.Writer.Write("g_iDftSS = ");
			this.Writer.Write((int)searchScope);
			this.Writer.Write(";");
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
			SearchScopeMenu searchScopeMenu = new SearchScopeMenu(base.UserContext, moduleForFolder, owaStoreObjectId.IsArchive ? Utilities.GetMailboxOwnerDisplayName((MailboxSession)owaStoreObjectId.GetSession(base.UserContext)) : null);
			searchScopeMenu.Render(stringWriter);
			stringWriter.Close();
			this.Writer.Write("var sSM = \"");
			Utilities.JavascriptEncode(stringBuilder.ToString(), this.Writer);
			this.Writer.Write("\";");
		}

		[OwaEventParameter("scp", typeof(SearchScope))]
		[OwaEventParameter("fId", typeof(OwaStoreObjectId))]
		[OwaEvent("PSS")]
		[OwaEventVerb(OwaEventVerb.Post)]
		public void PersistSearchScope()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.PersistSearchScope");
			if (base.UserContext.IsWebPartRequest)
			{
				return;
			}
			OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
			OutlookModule moduleForFolder;
			using (this.folder = Utilities.GetFolder<Folder>(base.UserContext, folderId, new PropertyDefinition[0]))
			{
				moduleForFolder = Utilities.GetModuleForFolder(this.folder, base.UserContext);
			}
			base.UserContext.UserOptions.SetSearchScope(moduleForFolder, (SearchScope)base.GetParameter("scp"));
			base.UserContext.UserOptions.CommitChanges();
		}

		protected static void Register()
		{
			if (!FolderVirtualListViewEventHandler2.isRegistered)
			{
				OwaEventRegistry.RegisterEnum(typeof(SearchRecipient));
				OwaEventRegistry.RegisterStruct(typeof(FolderVirtualListViewState));
				OwaEventRegistry.RegisterStruct(typeof(FolderVirtualListViewSearchFilter));
				OwaEventRegistry.RegisterStruct(typeof(FolderVirtualListViewFilter));
				FolderVirtualListViewEventHandler2.isRegistered = true;
			}
		}

		protected void BindToFolder()
		{
			if (this.folder == null)
			{
				OwaStoreObjectId folderId;
				if (base.IsParameterSet("St"))
				{
					folderId = (OwaStoreObjectId)this.ListViewState.SourceContainerId;
				}
				else if (base.IsParameterSet("fldrfltr"))
				{
					folderId = this.FilterCondition.SourceFolderId;
				}
				else
				{
					if (!base.IsParameterSet("fId"))
					{
						throw new OwaInvalidRequestException("Should have at least state or id.");
					}
					folderId = (OwaStoreObjectId)base.GetParameter("fId");
				}
				this.folder = Utilities.GetFolderForContent<Folder>(base.UserContext, folderId, new PropertyDefinition[]
				{
					FolderSchema.ItemCount,
					FolderSchema.UnreadCount,
					FolderSchema.ExtendedFolderFlags,
					ViewStateProperties.ViewWidth,
					ViewStateProperties.ReadingPanePosition,
					ViewStateProperties.ViewFilter,
					ViewStateProperties.SortColumn,
					ViewStateProperties.SortOrder,
					ViewStateProperties.FilteredViewLabel,
					FolderSchema.SearchFolderAllowAgeout,
					StoreObjectSchema.EffectiveRights
				});
				if (base.UserContext.IsInMyMailbox(this.folder) || Utilities.IsInArchiveMailbox(this.folder))
				{
					this.filteredFolder = this.GetFilteredView(this.folder);
				}
				if (this.IsFiltered && (base.UserContext.IsInMyMailbox(this.folder) || Utilities.IsInArchiveMailbox(this.folder)))
				{
					FolderVirtualListViewSearchFilter folderVirtualListViewSearchFilter = (FolderVirtualListViewSearchFilter)base.GetParameter("srchf");
					if (folderVirtualListViewSearchFilter.SearchString != null && 256 < folderVirtualListViewSearchFilter.SearchString.Length)
					{
						throw new OwaInvalidRequestException("Search string is longer than maximum length of " + 256);
					}
					FolderSearch folderSearch = new FolderSearch();
					string text = folderVirtualListViewSearchFilter.SearchString;
					if (folderVirtualListViewSearchFilter.ResultsIn != SearchResultsIn.DefaultFields && text != null)
					{
						text = null;
					}
					QueryFilter searchFilter = this.GetSearchFilter();
					folderSearch.AdvancedQueryFilter = FolderVirtualListViewEventHandler2.BuildAndFilter(folderSearch.AdvancedQueryFilter, searchFilter);
					this.searchFolder = folderSearch.Execute(base.UserContext, this.DataFolderForSearch, folderVirtualListViewSearchFilter.Scope, text, folderVirtualListViewSearchFilter.ReExecuteSearch, folderVirtualListViewSearchFilter.IsAsyncSearchEnabled);
				}
			}
		}

		protected override void PersistSort()
		{
			if (!base.UserContext.IsWebPartRequest)
			{
				this.BindToFolder();
				FolderViewStates folderViewStates = base.UserContext.GetFolderViewStates(this.folder);
				folderViewStates.SortColumn = ColumnIdParser.GetString(this.ListViewState.SortedColumn);
				folderViewStates.SortOrder = this.ListViewState.SortOrder;
				folderViewStates.Save();
				this.folder.Load();
			}
		}

		protected override void PersistMultiLineState()
		{
			if (!base.UserContext.IsWebPartRequest)
			{
				this.BindToFolder();
				FolderViewStates folderViewStates = base.UserContext.GetFolderViewStates(this.folder);
				folderViewStates.MultiLine = this.ListViewState.IsMultiLine;
				folderViewStates.Save();
				this.folder.Load();
			}
		}

		protected abstract void RenderAdvancedFind(TextWriter writer, OwaStoreObjectId folderId);

		protected abstract void RenderSearchInPublicFolder(TextWriter writer);

		protected void RenderAdvancedFind(TextWriter writer, AdvancedFindComponents advancedFindComponents, OwaStoreObjectId folderId)
		{
			int num = 0;
			int num2 = 28;
			writer.Write("<div id=divASExpanded>");
			if ((advancedFindComponents & AdvancedFindComponents.SubjectBody) != (AdvancedFindComponents)0)
			{
				DropDownListItem[] columnThreeDropDownListItems = new DropDownListItem[]
				{
					new DropDownListItem(3, 779520799),
					new DropDownListItem(2, -911352830),
					new DropDownListItem(1, 1732412034)
				};
				this.RenderAdvancedFindRow(writer, num, "RI", LocalizedStrings.GetHtmlEncoded(-891993694), 3.ToString(CultureInfo.InvariantCulture), columnThreeDropDownListItems);
				num += num2;
			}
			if ((advancedFindComponents & AdvancedFindComponents.SearchTextInSubject) != (AdvancedFindComponents)0 || (advancedFindComponents & AdvancedFindComponents.SearchTextInName) != (AdvancedFindComponents)0)
			{
				string columnTwoLabelId = ((advancedFindComponents & AdvancedFindComponents.SearchTextInName) != (AdvancedFindComponents)0) ? LocalizedStrings.GetHtmlEncoded(-2058911220) : LocalizedStrings.GetHtmlEncoded(-881075747);
				this.RenderAdvancedFindRow(writer, num, "SS", columnTwoLabelId);
				num += num2;
			}
			if ((advancedFindComponents & AdvancedFindComponents.FromTo) != (AdvancedFindComponents)0)
			{
				DropDownListItem[] columnTwoDropDownListItems = new DropDownListItem[]
				{
					new DropDownListItem(0, -1327933906),
					new DropDownListItem(1, 300298801)
				};
				this.RenderAdvancedFindRow(writer, num, "Rcp", 0.ToString(CultureInfo.InvariantCulture), columnTwoDropDownListItems);
				num += num2;
			}
			if ((advancedFindComponents & AdvancedFindComponents.Categories) != (AdvancedFindComponents)0)
			{
				this.RenderAdvancedFindRow(writer, num, "Cat", LocalizedStrings.GetHtmlEncoded(-1642040455), folderId);
				num += num2;
			}
			if ((advancedFindComponents & AdvancedFindComponents.SearchButton) != (AdvancedFindComponents)0)
			{
				writer.Write("<div class=\"ASRow\" style=\"top:");
				writer.Write(num);
				writer.Write("px;text-align:");
				writer.Write(base.UserContext.IsRtl ? "left" : "right");
				writer.Write("\">");
				using (StringWriter stringWriter = new StringWriter())
				{
					base.UserContext.RenderThemeImage(stringWriter, ThemeFileId.Search, null, new object[]
					{
						"id=imgBtnAF"
					});
					stringWriter.Write("<span id=\"spnAFSch\">");
					stringWriter.Write(LocalizedStrings.GetHtmlEncoded(656259478));
					stringWriter.Write("</span><span id=\"spnAFClr\" style=\"display:none;\">");
					stringWriter.Write(LocalizedStrings.GetHtmlEncoded(613695225));
					stringWriter.Write("</span>");
					RenderingUtilities.RenderButton(writer, "btnAF", "class=dsbl", string.Empty, stringWriter.ToString(), true);
				}
				writer.Write("</div>");
			}
			writer.Write("</div>");
		}

		private static QueryFilter BuildAndFilter(QueryFilter queryFilter1, QueryFilter queryFilter2)
		{
			if (queryFilter1 == null)
			{
				return queryFilter2;
			}
			if (queryFilter2 == null)
			{
				return queryFilter1;
			}
			return new AndFilter(new QueryFilter[]
			{
				queryFilter1,
				queryFilter2
			});
		}

		private void RenderAdvancedFindRow(TextWriter writer, int rowTop, string rowId, string columnTwoLabelId, string columnThreeDropDownSelectedValue, DropDownListItem[] columnThreeDropDownListItems)
		{
			this.RenderAdvancedFindRow(writer, rowTop, rowId, AdvancedFindColumnType.Label, columnTwoLabelId, null, null, AdvancedFindColumnType.DropDownList, columnThreeDropDownSelectedValue, columnThreeDropDownListItems, null);
		}

		private void RenderAdvancedFindRow(TextWriter writer, int rowTop, string rowId, string columnTwoLabelId)
		{
			this.RenderAdvancedFindRow(writer, rowTop, rowId, AdvancedFindColumnType.Label, columnTwoLabelId, null, null, AdvancedFindColumnType.InputBox, null, null, null);
		}

		private void RenderAdvancedFindRow(TextWriter writer, int rowTop, string rowId, string columnTwoDropDownSelectedValue, DropDownListItem[] columnTwoDropDownListItems)
		{
			this.RenderAdvancedFindRow(writer, rowTop, rowId, AdvancedFindColumnType.DropDownList, null, columnTwoDropDownSelectedValue, columnTwoDropDownListItems, AdvancedFindColumnType.InputBox, null, null, null);
		}

		private void RenderAdvancedFindRow(TextWriter writer, int rowTop, string rowId, string columnTwoLabelId, OwaStoreObjectId folderId)
		{
			this.RenderAdvancedFindRow(writer, rowTop, rowId, AdvancedFindColumnType.Label, columnTwoLabelId, null, null, AdvancedFindColumnType.CategoryDropDownList, null, null, folderId);
		}

		private void RenderAdvancedFindRow(TextWriter writer, int rowTop, string rowId, AdvancedFindColumnType columnTwoType, string columnTwoLabelId, string columnTwoDropDownSelectedValue, DropDownListItem[] columnTwoDropDownListItems, AdvancedFindColumnType columnThreeType, string columnThreeDropDownSelectedValue, DropDownListItem[] columnThreeDropDownListItems, OwaStoreObjectId folderId)
		{
			writer.Write("<div class=ASRow style=\"top:");
			writer.Write(rowTop);
			writer.Write("px;\">");
			writer.Write("<div id=divASCol1 class=ASCol1><input type=checkbox id=");
			writer.Write("chk" + rowId);
			writer.Write("></div>");
			writer.Write("<div id=divASCol2 class=\"ASCol2");
			switch (columnTwoType)
			{
			case AdvancedFindColumnType.Label:
				writer.Write(" label nowrap\">");
				writer.Write("<label for=");
				writer.Write("chk" + rowId);
				writer.Write(">");
				writer.Write(columnTwoLabelId);
				writer.Write("</label>");
				break;
			case AdvancedFindColumnType.DropDownList:
				writer.Write("\">");
				DropDownList.RenderDropDownList(writer, "div" + rowId, columnTwoDropDownSelectedValue, columnTwoDropDownListItems);
				break;
			default:
				throw new ArgumentException("Invalid value for columnTwoType");
			}
			writer.Write("</div>");
			writer.Write("<div id=divASCol3 class=\"ASCol3");
			switch (columnThreeType)
			{
			case AdvancedFindColumnType.DropDownList:
				writer.Write("\">");
				DropDownList.RenderDropDownList(writer, "div" + rowId, columnThreeDropDownSelectedValue, columnThreeDropDownListItems);
				break;
			case AdvancedFindColumnType.CategoryDropDownList:
				writer.Write("\">");
				CategoryDropDownList.RenderCategoryDropDownList(writer, folderId);
				break;
			case AdvancedFindColumnType.InputBox:
				writer.Write(" input\">");
				writer.Write("<input type=text maxlength=256 id=");
				writer.Write("txt" + rowId);
				writer.Write(">");
				break;
			default:
				throw new ArgumentException("Invalid value for columnThreeType");
			}
			writer.Write("</div>");
			writer.Write("</div>");
		}

		protected virtual QueryFilter GetViewFilter()
		{
			FolderVirtualListViewSearchFilter folderVirtualListViewSearchFilter = (FolderVirtualListViewSearchFilter)base.GetParameter("srchf");
			if (folderVirtualListViewSearchFilter != null && (Utilities.IsPublic(this.folder) || Utilities.IsOtherMailbox(this.folder)))
			{
				QueryFilter queryFilter = this.GetSearchFilter();
				if (folderVirtualListViewSearchFilter.SearchString != null && folderVirtualListViewSearchFilter.ResultsIn == SearchResultsIn.DefaultFields && Utilities.IsOtherMailbox(this.folder))
				{
					QueryFilter queryFilter2 = this.BuildDefaultQueryFilter();
					queryFilter = FolderVirtualListViewEventHandler2.BuildAndFilter(queryFilter2, queryFilter);
				}
				return FolderVirtualListViewEventHandler2.BuildAndFilter(this.folderQueryFilter, queryFilter);
			}
			return this.folderQueryFilter;
		}

		protected QueryFilter BuildDefaultQueryFilter()
		{
			FolderVirtualListViewSearchFilter folderVirtualListViewSearchFilter = (FolderVirtualListViewSearchFilter)base.GetParameter("srchf");
			string searchString = folderVirtualListViewSearchFilter.SearchString;
			if (string.IsNullOrEmpty(searchString))
			{
				return null;
			}
			bool isContentIndexingEnabled = ((this.folder.Session as MailboxSession) ?? base.UserContext.MailboxSession).Mailbox.IsContentIndexingEnabled;
			return AqsParser.ParseAndBuildQuery(searchString, SearchFilterGenerator.GetAqsParseOption(this.folder, isContentIndexingEnabled), base.UserContext.UserCulture, RescopedAll.Default, null, new PolicyTagMailboxProvider(base.UserContext.MailboxSession));
		}

		protected override void RenderExtraData(VirtualListView2 listView)
		{
			if (this.IsFiltered)
			{
				this.RenderSearchResultsHeader(listView);
			}
			if (this.IsFilteredView)
			{
				string s = string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(2114296142), new object[]
				{
					Utilities.GetDisplayNameByFolder(this.ContextFolder, base.UserContext),
					this.FilterCondition.ToDescription()
				});
				this.Writer.Write("<div id=\"fltrDesc\">");
				Utilities.HtmlEncode(s, this.Writer, true);
				this.Writer.Write("</div>");
			}
		}

		protected override void EndProcessEvent()
		{
			if (this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
			if (this.filteredFolder != null)
			{
				if (this.filteredFolder.PropertyBag.HasAllPropertiesLoaded)
				{
					this.filteredFolder[ViewStateProperties.FilteredViewAccessTime] = ExDateTime.Now;
					this.filteredFolder.Save();
				}
				this.filteredFolder.Dispose();
				this.filteredFolder = null;
			}
			if (this.searchFolder != null)
			{
				this.searchFolder.Dispose();
				this.searchFolder = null;
			}
		}

		protected void InternalDelete(bool permanentDelete, OwaStoreObjectId[] itemIds)
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("fId");
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("RTOcc");
			OperationResult operationResult;
			if (itemIds == null)
			{
				DeleteItemInfo[] deleteItemInfos = (DeleteItemInfo[])base.GetParameter("Itms");
				operationResult = this.DeleteItems(deleteItemInfos, permanentDelete);
			}
			else
			{
				operationResult = this.DeleteItems(itemIds, permanentDelete);
			}
			OperationResult operationResult2 = OperationResult.Succeeded;
			if (array != null)
			{
				operationResult2 = this.DeleteTaskOccurrences(array, permanentDelete);
			}
			if (operationResult != OperationResult.Succeeded || operationResult2 != OperationResult.Succeeded)
			{
				this.RenderErrorResult("errDel", 166628739);
			}
		}

		private OperationResult DeleteItems(DeleteItemInfo[] deleteItemInfos, bool permanentDelete)
		{
			OwaStoreObjectId[] array = new OwaStoreObjectId[deleteItemInfos.Length];
			this.CheckSizeOfBatchOperation(array);
			for (int i = 0; i < deleteItemInfos.Length; i++)
			{
				DeleteItemInfo deleteItemInfo = deleteItemInfos[i];
				array[i] = deleteItemInfo.OwaStoreObjectId;
				if (deleteItemInfo.IsMeetingMessage && !deleteItemInfo.OwaStoreObjectId.IsPublic)
				{
					try
					{
						MeetingUtilities.DeleteMeetingMessageCalendarItem(deleteItemInfo.OwaStoreObjectId.StoreObjectId);
					}
					catch (ObjectNotFoundException)
					{
					}
				}
			}
			return this.DeleteItems(array, permanentDelete);
		}

		private OperationResult DeleteItems(OwaStoreObjectId[] itemIds, bool permanentDelete)
		{
			List<OwaStoreObjectId> list = new List<OwaStoreObjectId>();
			List<OwaStoreObjectId> list2 = new List<OwaStoreObjectId>();
			this.CheckSizeOfBatchOperation(itemIds);
			foreach (OwaStoreObjectId owaStoreObjectId in itemIds)
			{
				if (permanentDelete || Utilities.IsDefaultFolderId(base.UserContext, Utilities.GetParentFolderId(owaStoreObjectId), DefaultFolderType.DeletedItems))
				{
					list.Add(owaStoreObjectId);
				}
				else
				{
					list2.Add(owaStoreObjectId);
				}
			}
			OwaStoreObjectId[] array = list.ToArray();
			OwaStoreObjectId[] array2 = list2.ToArray();
			OperationResult operationResult = (OperationResult)0;
			if (array2.Length > 0)
			{
				operationResult |= Utilities.Delete(base.UserContext, permanentDelete, array2).OperationResult;
			}
			if (array.Length > 0)
			{
				if (itemIds[0].IsPublic || Utilities.ShouldSuppressReadReceipt(base.UserContext))
				{
					operationResult |= Utilities.Delete(base.UserContext, DeleteItemFlags.SoftDelete | DeleteItemFlags.SuppressReadReceipt, array).OperationResult;
				}
				else
				{
					OwaStoreObjectId[] array3 = null;
					OwaStoreObjectId[] array4 = null;
					JunkEmailStatus junkEmailStatus = JunkEmailStatus.NotJunk;
					if (base.IsParameterSet("JS"))
					{
						junkEmailStatus = (JunkEmailStatus)base.GetParameter("JS");
					}
					switch (junkEmailStatus)
					{
					case JunkEmailStatus.NotJunk:
						array4 = itemIds;
						break;
					case JunkEmailStatus.Junk:
						array3 = array;
						break;
					default:
						JunkEmailUtilities.SortJunkEmailIds(base.UserContext, array, out array3, out array4);
						break;
					}
					OperationResult operationResult2 = (array3 == null || array3.Length == 0) ? OperationResult.Succeeded : Utilities.Delete(base.UserContext, DeleteItemFlags.SoftDelete | DeleteItemFlags.SuppressReadReceipt, array3).OperationResult;
					OperationResult operationResult3 = (array4 == null || array4.Length == 0) ? OperationResult.Succeeded : Utilities.Delete(base.UserContext, DeleteItemFlags.SoftDelete, array4).OperationResult;
					if (operationResult2 == OperationResult.Succeeded && operationResult3 == OperationResult.Succeeded)
					{
						operationResult |= OperationResult.Succeeded;
					}
					if (operationResult2 == OperationResult.Failed && operationResult3 == OperationResult.Failed)
					{
						operationResult |= OperationResult.Failed;
					}
				}
			}
			if (operationResult != (OperationResult)0)
			{
				return operationResult;
			}
			return OperationResult.PartiallySucceeded;
		}

		private OperationResult DeleteTaskOccurrences(OwaStoreObjectId[] recurringTasks, bool permanentDelete)
		{
			bool flag = true;
			bool flag2 = false;
			for (int i = 0; i < recurringTasks.Length; i++)
			{
				using (Task item = Utilities.GetItem<Task>(base.UserContext, recurringTasks[i], new PropertyDefinition[0]))
				{
					if (item.Recurrence.Pattern is RegeneratingPattern)
					{
						throw new OwaInvalidRequestException("Cannot delete an occurrence of a regenerating task.");
					}
					if (item.IsLastOccurrence)
					{
						OperationResult operationResult = Utilities.Delete(base.UserContext, permanentDelete, new OwaStoreObjectId[]
						{
							recurringTasks[i]
						}).OperationResult;
						if (operationResult != OperationResult.Succeeded)
						{
							flag = false;
						}
						else
						{
							flag2 = true;
						}
					}
					else
					{
						item.OpenAsReadWrite();
						item.DeleteCurrentOccurrence();
						Utilities.SaveItem(item);
					}
				}
			}
			if (flag)
			{
				return OperationResult.Succeeded;
			}
			if (flag2)
			{
				return OperationResult.PartiallySucceeded;
			}
			return OperationResult.Failed;
		}

		protected void CopyOrMoveItems(bool isCopy, OwaStoreObjectId[] sourceIds)
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("destId");
			if (sourceIds == null)
			{
				sourceIds = (OwaStoreObjectId[])base.GetParameter("id");
			}
			if (Utilities.IsELCRootFolder(owaStoreObjectId, base.UserContext))
			{
				throw new OwaInvalidRequestException("Cannot move messages to the root ELC folder.");
			}
			if (Utilities.IsExternalSharedInFolder(base.UserContext, owaStoreObjectId))
			{
				this.RenderErrorResult("errCpyMv", 995407892);
				return;
			}
			AggregateOperationResult aggregateOperationResult = Utilities.CopyOrMoveItems(base.UserContext, isCopy, owaStoreObjectId, sourceIds);
			if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
			{
				Strings.IDs errorMessage;
				if (aggregateOperationResult.OperationResult == OperationResult.Failed)
				{
					errorMessage = -2079833764;
				}
				else
				{
					errorMessage = (isCopy ? -133951803 : -1645967063);
					OwaStoreObjectId owaStoreObjectId2 = (OwaStoreObjectId)base.GetParameter("fId");
					if (owaStoreObjectId != null && owaStoreObjectId2 != null)
					{
						if (owaStoreObjectId.IsPublic || owaStoreObjectId2.IsPublic)
						{
							errorMessage = (isCopy ? 1524003350 : -551950934);
						}
						else if (owaStoreObjectId.IsOtherMailbox || owaStoreObjectId2.IsOtherMailbox)
						{
							errorMessage = (isCopy ? 198120285 : 404204505);
						}
					}
				}
				this.RenderErrorResult("errCpyMv", errorMessage);
				return;
			}
			bool flag = true;
			object parameter = base.GetParameter("fAddToMRU");
			if (parameter != null)
			{
				flag = (bool)parameter;
			}
			if (flag)
			{
				TargetFolderMRU.AddAndGetFolders(owaStoreObjectId, base.UserContext);
			}
		}

		private void PersistWidthOrHeight(bool isWidth)
		{
			if (!base.UserContext.IsWebPartRequest)
			{
				OwaStoreObjectId folderId = (OwaStoreObjectId)base.GetParameter("fId");
				using (this.folder = Utilities.GetFolder<Folder>(base.UserContext, folderId, new PropertyDefinition[0]))
				{
					FolderViewStates folderViewStates = base.UserContext.GetFolderViewStates(this.folder);
					try
					{
						if (isWidth)
						{
							folderViewStates.ViewWidth = (int)base.GetParameter("w");
						}
						else
						{
							folderViewStates.ViewHeight = (int)base.GetParameter("h");
						}
						folderViewStates.Save();
					}
					catch (ArgumentOutOfRangeException ex)
					{
						throw new OwaInvalidRequestException(ex.Message, ex);
					}
				}
			}
		}

		private QueryFilter GetSearchFilter()
		{
			FolderVirtualListViewSearchFilter folderVirtualListViewSearchFilter = (FolderVirtualListViewSearchFilter)base.GetParameter("srchf");
			string searchString = folderVirtualListViewSearchFilter.SearchString;
			QueryFilter queryFilter = null;
			bool isContentIndexingEnabled = ((this.folder.Session as MailboxSession) ?? base.UserContext.MailboxSession).Mailbox.IsContentIndexingEnabled;
			if (folderVirtualListViewSearchFilter.ResultsIn != SearchResultsIn.DefaultFields && searchString != null)
			{
				RescopedAll rescopedAll = RescopedAll.Default;
				switch (folderVirtualListViewSearchFilter.ResultsIn)
				{
				case SearchResultsIn.Subject:
					rescopedAll = RescopedAll.Subject;
					break;
				case SearchResultsIn.Body:
					rescopedAll = RescopedAll.Body;
					break;
				case SearchResultsIn.BodyAndSubject:
					rescopedAll = RescopedAll.BodyAndSubject;
					break;
				}
				QueryFilter queryFilter2 = AqsParser.ParseAndBuildQuery(searchString, SearchFilterGenerator.GetAqsParseOption(this.folder, isContentIndexingEnabled), base.UserContext.UserCulture, rescopedAll, null, new PolicyTagMailboxProvider(base.UserContext.MailboxSession));
				if (queryFilter2 != null)
				{
					queryFilter = FolderVirtualListViewEventHandler2.BuildAndFilter(queryFilter, queryFilter2);
				}
			}
			if (folderVirtualListViewSearchFilter.Category != null)
			{
				ComparisonFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Categories, folderVirtualListViewSearchFilter.Category);
				queryFilter = FolderVirtualListViewEventHandler2.BuildAndFilter(queryFilter, queryFilter3);
			}
			if (folderVirtualListViewSearchFilter.RecipientValue != null)
			{
				RescopedAll rescopedAll2 = RescopedAll.Default;
				switch (folderVirtualListViewSearchFilter.RecipientType)
				{
				case SearchRecipient.From:
					rescopedAll2 = RescopedAll.From;
					break;
				case SearchRecipient.SentTo:
					rescopedAll2 = RescopedAll.Participants;
					break;
				}
				QueryFilter queryFilter4 = AqsParser.ParseAndBuildQuery(folderVirtualListViewSearchFilter.RecipientValue, SearchFilterGenerator.GetAqsParseOption(this.folder, isContentIndexingEnabled), base.UserContext.UserCulture, rescopedAll2, null, new PolicyTagMailboxProvider(base.UserContext.MailboxSession));
				if (queryFilter4 != null)
				{
					queryFilter = FolderVirtualListViewEventHandler2.BuildAndFilter(queryFilter, queryFilter4);
				}
			}
			return queryFilter;
		}

		private void RenderSearchResultsHeader(VirtualListView2 listView)
		{
			int num = listView.TotalCount;
			string displayNameByFolder = Utilities.GetDisplayNameByFolder(this.folder, base.UserContext);
			if (!Utilities.IsPublic(this.folder) && !Utilities.IsOtherMailbox(this.folder))
			{
				if (listView.GetDidLastSearchFail())
				{
					num = 0;
				}
				else
				{
					object obj = this.searchFolder.TryGetProperty(FolderSchema.SearchFolderItemCount);
					if (obj is int)
					{
						num = (int)obj;
						if (num > 1000)
						{
							num = 1000;
						}
					}
				}
			}
			this.Writer.Write("<span id=\"spnSR\">");
			FolderVirtualListViewSearchFilter folderVirtualListViewSearchFilter = (FolderVirtualListViewSearchFilter)base.GetParameter("srchf");
			switch (folderVirtualListViewSearchFilter.Scope)
			{
			case SearchScope.SelectedFolder:
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-373089553), num, Utilities.HtmlEncode(displayNameByFolder));
				break;
			case SearchScope.SelectedAndSubfolders:
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(1952153313), num, Utilities.HtmlEncode(displayNameByFolder));
				break;
			case SearchScope.AllItemsInModule:
			{
				OutlookModule moduleForFolder = Utilities.GetModuleForFolder(this.folder, base.UserContext);
				if (moduleForFolder == OutlookModule.Contacts)
				{
					this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-1478799838), num);
				}
				else
				{
					this.Writer.Write(LocalizedStrings.GetHtmlEncoded(24888175), num);
				}
				break;
			}
			case SearchScope.AllFoldersAndItems:
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(59311153), num);
				break;
			}
			this.Writer.Write("</span>");
		}

		protected void RenderErrorResult(string divId, Strings.IDs errorMessage)
		{
			this.Writer.Write("<div id=\"");
			this.Writer.Write(divId);
			this.Writer.Write("\" _msg=\"");
			this.Writer.Write(LocalizedStrings.GetHtmlEncoded(errorMessage));
			this.Writer.Write("\"></div>");
		}

		private void RenderGroupOperationResult(bool[] operationSuccessResults, Strings.IDs partialFailureWarningMessage, Strings.IDs totalFailureWarningMessage)
		{
			bool flag = false;
			bool flag2 = true;
			this.Writer.Write("var sCR = \"");
			foreach (bool flag3 in operationSuccessResults)
			{
				this.Writer.Write(flag3 ? "1" : "0");
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					flag = true;
				}
			}
			this.Writer.Write("\";");
			if (flag)
			{
				this.Writer.Write("alrt(\"");
				if (flag2)
				{
					this.Writer.Write(LocalizedStrings.GetJavascriptEncoded(totalFailureWarningMessage));
				}
				else
				{
					this.Writer.Write(LocalizedStrings.GetJavascriptEncoded(partialFailureWarningMessage));
				}
				this.Writer.Write("\", null, Owa.BUTTON_DIALOG_ICON.WARNING, L_Wrng);");
			}
		}

		private Folder GetFilteredView(Folder contextFolder)
		{
			if (!this.IsFilteredView)
			{
				return null;
			}
			int num = 0;
			StoreObjectId storeObjectId = null;
			bool flag = false;
			MailboxSession mailboxSession = (contextFolder.Session as MailboxSession) ?? base.UserContext.MailboxSession;
			using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.SearchFolders))
			{
				object[][] array;
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, FolderList.FolderTreeQueryProperties))
				{
					array = Utilities.FetchRowsFromQueryResult(queryResult, 10000);
				}
				int num2 = FolderList.FolderTreePropertyIndexes[FolderSchema.Id];
				int num3 = FolderList.FolderTreePropertyIndexes[ViewStateProperties.FilteredViewLabel];
				int num4 = FolderList.FolderTreePropertyIndexes[FolderSchema.SearchFolderAllowAgeout];
				int num5 = FolderList.FolderTreePropertyIndexes[StoreObjectSchema.LastModifiedTime];
				int num6 = FolderList.FolderTreePropertyIndexes[ViewStateProperties.FilteredViewAccessTime];
				ExDateTime other = ExDateTime.MaxValue;
				for (int i = 0; i < array.Length; i++)
				{
					FolderVirtualListViewFilter folderVirtualListViewFilter = FolderVirtualListViewFilter.ParseFromPropertyValue(array[i][num3]);
					if (folderVirtualListViewFilter != null)
					{
						if (this.FilterCondition.Equals(folderVirtualListViewFilter))
						{
							return Folder.Bind(mailboxSession, (array[i][num2] as VersionedId).ObjectId, FolderList.FolderTreeQueryProperties);
						}
						if (this.FilterCondition.EqualsIgnoreVersion(folderVirtualListViewFilter))
						{
							storeObjectId = (array[i][num2] as VersionedId).ObjectId;
							flag = true;
						}
						if (!flag && array[i][num4] is bool && (bool)array[i][num4])
						{
							num++;
							if (!folderVirtualListViewFilter.IsCurrentVersion)
							{
								storeObjectId = (array[i][num2] as VersionedId).ObjectId;
								flag = true;
							}
							else if (storeObjectId == null)
							{
								storeObjectId = (array[i][num2] as VersionedId).ObjectId;
							}
							else
							{
								ExDateTime exDateTime = ExDateTime.MinValue;
								ExDateTime exDateTime2 = ExDateTime.MinValue;
								if (!(array[i][num5] is PropertyError))
								{
									exDateTime = (ExDateTime)array[i][num5];
								}
								if (!(array[i][num6] is PropertyError))
								{
									exDateTime2 = (ExDateTime)array[i][num6];
								}
								if (exDateTime2.CompareTo(exDateTime) < 0)
								{
									exDateTime2 = exDateTime;
								}
								if (exDateTime2.CompareTo(other) < 0)
								{
									other = exDateTime2;
									storeObjectId = (array[i][num2] as VersionedId).ObjectId;
								}
							}
						}
					}
				}
			}
			SearchFolder searchFolder;
			if (flag || num >= Globals.MaximumTemporaryFilteredViewPerUser)
			{
				searchFolder = SearchFolder.Bind(mailboxSession, storeObjectId, FolderList.FolderTreeQueryProperties);
			}
			else
			{
				searchFolder = SearchFolder.Create(mailboxSession, base.UserContext.GetSearchFoldersId(mailboxSession).StoreObjectId, Utilities.GetRandomNameForTempFilteredView(base.UserContext), CreateMode.OpenIfExists);
			}
			searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
			searchFolder[ViewStateProperties.FilteredViewLabel] = this.FilterCondition.GetPropertyValueToSave();
			this.CopyFolderProperties(contextFolder, searchFolder, new PropertyDefinition[]
			{
				ViewStateProperties.ReadingPanePosition,
				ViewStateProperties.ViewWidth,
				ViewStateProperties.SortColumn,
				ViewStateProperties.SortOrder
			});
			searchFolder.Save();
			searchFolder.Load(FolderList.FolderTreeQueryProperties);
			this.FilterCondition.ApplyFilter(searchFolder, FolderList.FolderTreeQueryProperties);
			return searchFolder;
		}

		private void CopyFolderProperties(Folder sourceFolder, Folder targetFolder, params PropertyDefinition[] properties)
		{
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				object obj = sourceFolder.TryGetProperty(propertyDefinition);
				targetFolder.SetOrDeleteProperty(propertyDefinition, (obj is PropertyError) ? null : obj);
			}
		}

		protected void CheckSizeOfBatchOperation(OwaStoreObjectId[] sourceIds)
		{
			sourceIds = (sourceIds ?? ((OwaStoreObjectId[])base.GetParameter("id")));
			int num = sourceIds.Length;
			if (num > 500)
			{
				throw new OwaInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Copying or moving {0} item(s) in a single request is not supported", new object[]
				{
					num
				}));
			}
		}

		private OwaStoreObjectId UpdateConversationCategories(OwaStoreObjectId conversationId, OwaStoreObjectId folderId, string[] addCategories, string[] removeCategories)
		{
			Conversation conversation = ConversationUtilities.LoadConversation(base.UserContext, conversationId, new PropertyDefinition[]
			{
				ItemSchema.Id,
				StoreObjectSchema.ParentItemId,
				ItemSchema.Categories
			});
			ConversationUtilities.UpdateAlwaysCategorizeRule(base.UserContext, conversationId, conversation, addCategories, removeCategories);
			if (removeCategories != null && removeCategories.Length > 0)
			{
				foreach (IConversationTreeNode conversationTreeNode in conversation.ConversationTree)
				{
					foreach (IStorePropertyBag storePropertyBag in conversationTreeNode.StorePropertyBags)
					{
						string[] property = ItemUtility.GetProperty<string[]>(storePropertyBag, ItemSchema.Categories, null);
						if (property != null)
						{
							foreach (string value in property)
							{
								if (Array.IndexOf<string>(removeCategories, value) != -1)
								{
									VersionedId versionedId = (VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id);
									using (Item item = Utilities.GetItem<Item>(base.UserContext, versionedId.ObjectId, new PropertyDefinition[0]))
									{
										item.OpenAsReadWrite();
										CategoryContextMenu.ModifyCategories(item, null, removeCategories);
										item.Save(SaveMode.ResolveConflicts);
										break;
									}
								}
							}
						}
					}
				}
			}
			StoreObjectId latestMessage = ConversationUtilities.GetLatestMessage((MailboxSession)conversationId.GetSession(base.UserContext), conversation, folderId.StoreObjectId);
			if (latestMessage == null)
			{
				latestMessage = ConversationUtilities.GetLatestMessage((MailboxSession)conversationId.GetSession(base.UserContext), conversation, null);
			}
			return OwaStoreObjectId.CreateFromStoreObjectId(latestMessage, conversationId);
		}

		public const string MethodDelete = "Delete";

		public const string MethodPermanentDelete = "PermanentDelete";

		public const string MethodMove = "Move";

		public const string MethodCopy = "Copy";

		public const string MethodAddToMru = "AddToMru";

		public const string MethodGetCopyMoveMenu = "GetCopyMoveMenu";

		public const string MethodPersistWidth = "PersistWidth";

		public const string MethodPersistHeight = "PersistHeight";

		public const string MethodPersistReadingPane = "PersistReadingPane";

		public const string MethodPersistReadingPaneSortOrder = "PersistReadingPaneSortOrder";

		public const string MethodPersistShowTreeInListView = "PersistShowTreeInListView";

		public const string MethodEditMeetingResponse = "ER";

		public const string MethodSendMeetingResponse = "SR";

		public const string MethodDontSendMeetingResponse = "DR";

		public const string MethodFlagItems = "flgItms";

		public const string MethodCategorizeItems = "catItms";

		public const string MethodClearCategories = "catClr";

		public const string MethodGetAdvancedFind = "gtAF";

		public const string MethodGetSearchInPublicFolder = "gtSIPF";

		public const string MethodGetSearchScopeMenu = "GetSearchScopeMenu";

		public const string MethodPersistELCCommentState = "PersistELCCommentState";

		public const string MethodPersistSearchScope = "PSS";

		public const string FolderId = "fId";

		public const string DestinationFolderId = "destId";

		public const string AddTargetFolderToMruParameter = "fAddToMRU";

		public const string SearchFilter = "srchf";

		public const string FolderFilter = "fldrfltr";

		public const string Items = "Itms";

		public const string Width = "w";

		public const string Height = "h";

		public const string Response = "Rsp";

		public const string FlagActionParameter = "flga";

		public const string DueDate = "ddt";

		public const string AddCategories = "catAdd";

		public const string RemoveCategories = "catRem";

		public const string SearchScopeParameter = "scp";

		public const string JunkStatus = "JS";

		public const string ExpandELCComment = "fShwELC";

		public const string RecurringTaskOccurrences = "RTOcc";

		protected const int MaxFolderNameLength = 256;

		private const int MaxSearchStringLength = 256;

		private const int MaxFromToFilterStringLength = 255;

		private static bool isRegistered;

		private Folder folder;

		private Folder searchFolder;

		private Folder filteredFolder;

		private QueryFilter folderQueryFilter;

		private FolderVirtualListViewFilter filterObject;

		internal delegate void RespondToMeetingRequestDelegate();
	}
}
