using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class ListViewContents2
	{
		protected ListViewContents2(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		protected bool IsForVirtualListView
		{
			get
			{
				return this.isForVirtualListView;
			}
		}

		public Hashtable Properties
		{
			get
			{
				return this.properties;
			}
		}

		public virtual IListViewDataSource DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = value;
			}
		}

		public abstract ViewDescriptor ViewDescriptor { get; }

		protected static bool ShouldColumnHaveAnchor(ColumnId columnId)
		{
			if (ListViewContents2.columnsWithAnchorsMap == null)
			{
				ListViewContents2.InitializeColumnsWithAnchorsMap();
			}
			return ListViewContents2.columnsWithAnchorsMap[(int)columnId];
		}

		protected void AddProperty(PropertyDefinition propertyDefinition)
		{
			if (this.properties.ContainsKey(propertyDefinition))
			{
				return;
			}
			this.properties.Add(propertyDefinition, null);
		}

		public void RenderForVirtualListView(TextWriter writer)
		{
			this.isForVirtualListView = true;
			this.Render(writer);
		}

		public void Render(TextWriter writer)
		{
			this.Render(writer, 0, this.dataSource.RangeCount - 1);
		}

		public void RenderForVirtualListView(TextWriter writer, int startRange, int endRange)
		{
			this.isForVirtualListView = true;
			this.Render(writer, startRange, endRange);
		}

		public void Render(TextWriter writer, int startRange, int endRange)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (startRange < 0)
			{
				throw new ArgumentOutOfRangeException("startRange", "startRange must be greater than or equal to 0");
			}
			if (endRange < startRange)
			{
				throw new ArgumentOutOfRangeException("endRange", "endRange must be greater than or equal to startRange");
			}
			this.ValidatedRender(writer, startRange, endRange);
		}

		protected abstract void ValidatedRender(TextWriter writer, int startRange, int endRange);

		internal virtual void RenderRow(TextWriter writer, bool showFlag, ListViewContents2.ListViewRowType rowTypeToRender, bool renderContainer)
		{
			throw new NotImplementedException("Implement this in the inherited class if needed");
		}

		protected bool RenderColumn(TextWriter writer, ColumnId columnId)
		{
			return this.RenderColumn(writer, columnId, true);
		}

		protected virtual bool RenderIcon(TextWriter writer)
		{
			string itemClass = this.dataSource.GetItemClass();
			return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, itemClass);
		}

		protected bool RenderMessageIcon(TextWriter writer)
		{
			this.RenderCheckbox(writer);
			string itemProperty = this.dataSource.GetItemProperty<string>(StoreObjectSchema.ItemClass);
			bool itemProperty2 = this.dataSource.GetItemProperty<bool>(StoreObjectSchema.IsRestricted, false);
			int itemProperty3 = this.dataSource.GetItemProperty<int>(ItemSchema.IconIndex, -1);
			bool itemProperty4 = this.dataSource.GetItemProperty<bool>(MessageItemSchema.MessageInConflict, false);
			bool itemProperty5 = this.dataSource.GetItemProperty<bool>(MessageItemSchema.IsRead, false);
			return ListViewContentsRenderingUtilities.RenderMessageIcon(writer, this.userContext, itemProperty, itemProperty5, itemProperty4, itemProperty3, itemProperty2);
		}

		protected virtual bool RenderUncDocumentIcon(TextWriter writer)
		{
			bool itemProperty = this.dataSource.GetItemProperty<bool>(UncItemSchema.IsFolder, false);
			if (itemProperty)
			{
				return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, "ipf.documentlibrary.unc");
			}
			Uri itemProperty2 = this.dataSource.GetItemProperty<Uri>(UncItemSchema.Uri);
			string itemClass = Path.GetExtension(itemProperty2.ToString()).ToLowerInvariant();
			return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, itemClass, "ipm.document");
		}

		protected virtual bool RenderSharepointDocumentIcon(TextWriter writer)
		{
			if (this.dataSource.GetItemProperty<bool>(DocumentLibraryItemSchema.IsFolder, false))
			{
				return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, "ipf.documentlibrary.unc");
			}
			Uri itemProperty = this.dataSource.GetItemProperty<Uri>(SharepointDocumentLibraryItemSchema.EncodedAbsoluteUri);
			string itemClass = Path.GetExtension(itemProperty.ToString()).ToLowerInvariant();
			return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, itemClass, "ipm.document");
		}

		protected virtual bool IsAssignedTask
		{
			get
			{
				return false;
			}
		}

		protected bool RenderColumn(TextWriter writer, ColumnId columnId, bool nonBreakingSpaceOnEmpty)
		{
			bool flag;
			using (StringWriter stringWriter = new StringWriter())
			{
				flag = this.InternalRenderColumn(stringWriter, columnId);
				bool flag2 = ListViewContents2.ShouldColumnHaveAnchor(columnId) && !string.IsNullOrEmpty(stringWriter.ToString());
				if (flag2)
				{
					this.RenderVLVAnchorOpen(writer);
				}
				writer.Write(stringWriter.ToString());
				if (!flag && nonBreakingSpaceOnEmpty)
				{
					writer.Write("&nbsp;");
				}
				if (flag2)
				{
					this.RenderVLVAnchorClose(writer);
				}
			}
			return flag;
		}

		protected virtual bool InternalRenderColumn(TextWriter writer, ColumnId columnId)
		{
			Column column = ListViewColumns.GetColumn(columnId);
			if (columnId <= ColumnId.ADIcon)
			{
				switch (columnId)
				{
				case ColumnId.MailIcon:
				case ColumnId.ContactIcon:
					break;
				case ColumnId.From:
				case ColumnId.To:
				case ColumnId.Subject:
				case ColumnId.Department:
					goto IL_68B;
				case ColumnId.HasAttachment:
				{
					string itemClass = this.dataSource.GetItemClass();
					bool itemProperty = this.dataSource.GetItemProperty<bool>(ItemSchema.HasAttachment, false);
					return ListViewContentsRenderingUtilities.RenderHasAttachments(writer, this.userContext, itemProperty, itemClass);
				}
				case ColumnId.Importance:
					goto IL_17B;
				case ColumnId.DeliveryTime:
				{
					ExDateTime itemProperty2 = this.dataSource.GetItemProperty<ExDateTime>(ItemSchema.ReceivedTime, ExDateTime.MinValue);
					return this.RenderSmartDate(writer, itemProperty2);
				}
				case ColumnId.SentTime:
				{
					ExDateTime itemProperty3 = this.dataSource.GetItemProperty<ExDateTime>(ItemSchema.SentTime, ExDateTime.MinValue);
					return this.RenderSmartDate(writer, itemProperty3);
				}
				case ColumnId.Size:
				{
					int itemProperty4 = this.dataSource.GetItemProperty<int>(ItemSchema.Size, 0);
					Utilities.RenderSizeWithUnits(writer, (long)itemProperty4, true);
					return true;
				}
				default:
					switch (columnId)
					{
					case ColumnId.EmailAddresses:
						if (ObjectClass.IsDistributionList(this.dataSource.GetItemClass()))
						{
							this.RenderSingleEmailAddress(writer, this.dataSource.GetItemProperty<string>(ContactBaseSchema.FileAs, string.Empty), string.Empty, string.Empty, null, EmailAddressIndex.None, RecipientAddress.RecipientAddressFlags.DistributionList);
							return true;
						}
						return this.RenderEmailAddresses(writer);
					case ColumnId.Email1:
					case ColumnId.Email2:
					case ColumnId.Email3:
						goto IL_428;
					case ColumnId.GivenName:
					case ColumnId.Surname:
					case ColumnId.SharepointDocumentDisplayName:
					case ColumnId.SharepointDocumentLastModified:
					case ColumnId.SharepointDocumentModifiedBy:
					case ColumnId.SharepointDocumentCheckedOutTo:
					case ColumnId.UncDocumentDisplayName:
					case ColumnId.UncDocumentLastModified:
						goto IL_68B;
					case ColumnId.Categories:
					case ColumnId.ContactCategories:
						goto IL_32C;
					case ColumnId.SharepointDocumentIcon:
						return this.RenderSharepointDocumentIcon(writer);
					case ColumnId.SharepointDocumentFileSize:
					{
						long itemProperty5 = this.dataSource.GetItemProperty<long>(SharepointDocumentSchema.FileSize, 0L);
						Utilities.RenderSizeWithUnits(writer, itemProperty5, true);
						return true;
					}
					case ColumnId.UncDocumentIcon:
						return this.RenderUncDocumentIcon(writer);
					case ColumnId.UncDocumentLibraryIcon:
						return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, "ipf.documentlibrary.sharepoint");
					case ColumnId.UncDocumentFileSize:
					{
						long itemProperty6 = this.dataSource.GetItemProperty<long>(UncDocumentSchema.FileSize, 0L);
						Utilities.RenderSizeWithUnits(writer, itemProperty6, true);
						return true;
					}
					case ColumnId.SharepointDocumentLibraryIcon:
						return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, "ipf.documentlibrary.sharepoint");
					default:
						if (columnId != ColumnId.ADIcon)
						{
							goto IL_68B;
						}
						break;
					}
					break;
				}
			}
			else
			{
				if (columnId == ColumnId.EmailAddressAD)
				{
					return this.RenderADEmailAddress(this.dataSource, writer);
				}
				switch (columnId)
				{
				case ColumnId.YomiFullName:
				{
					StringBuilder stringBuilder = new StringBuilder();
					string itemProperty7 = this.dataSource.GetItemProperty<string>(ContactSchema.YomiLastName, string.Empty);
					if (!string.IsNullOrEmpty(itemProperty7))
					{
						Utilities.HtmlEncode(itemProperty7, stringBuilder);
					}
					string itemProperty8 = this.dataSource.GetItemProperty<string>(ContactSchema.YomiFirstName, string.Empty);
					if (!string.IsNullOrEmpty(itemProperty8))
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(" ");
						}
						Utilities.HtmlEncode(itemProperty8, stringBuilder);
					}
					if (stringBuilder.Length == 0)
					{
						return false;
					}
					writer.Write(stringBuilder.ToString());
					return true;
				}
				case ColumnId.YomiLastName:
				case ColumnId.YomiDisplayNameAD:
				case ColumnId.YomiDepartmentAD:
				case ColumnId.ResourceCapacityAD:
				case ColumnId.FlagStartDate:
				case ColumnId.ContactFlagStartDate:
				case ColumnId.MemberDisplayName:
				case ColumnId.ConversationLastDeliveryTime:
				case ColumnId.ConversationIcon:
				case ColumnId.ConversationSubject:
				case ColumnId.ConversationUnreadCount:
				case ColumnId.ConversationHasAttachment:
				case ColumnId.ConversationSenderList:
					goto IL_68B;
				case ColumnId.FlagDueDate:
				case ColumnId.ContactFlagDueDate:
				case ColumnId.TaskFlag:
				{
					FlagStatus itemProperty9 = this.dataSource.GetItemProperty<FlagStatus>(ItemSchema.FlagStatus, FlagStatus.NotFlagged);
					int itemProperty10 = this.dataSource.GetItemProperty<int>(ItemSchema.ItemColor, int.MinValue);
					ThemeFileId themeFileId = ThemeFileId.FlagEmpty;
					if (itemProperty9 == FlagStatus.NotFlagged)
					{
						string itemClass2 = this.dataSource.GetItemClass();
						if (ObjectClass.IsTask(itemClass2))
						{
							bool itemProperty11 = this.dataSource.GetItemProperty<bool>(ItemSchema.IsComplete, false);
							if (itemProperty11)
							{
								themeFileId = (this.IsAssignedTask ? ThemeFileId.FlagCompleteDisabled : ThemeFileId.FlagComplete);
							}
							else
							{
								themeFileId = (this.IsAssignedTask ? ThemeFileId.FlagDisabled : ThemeFileId.Flag);
							}
						}
					}
					else if (itemProperty10 == -2147483648 && itemProperty9 == FlagStatus.Flagged)
					{
						themeFileId = ThemeFileId.FlagSender;
					}
					else if (itemProperty9 == FlagStatus.Flagged)
					{
						themeFileId = ThemeFileId.Flag;
					}
					else
					{
						themeFileId = ThemeFileId.FlagComplete;
					}
					this.userContext.RenderThemeImage(writer, themeFileId, null, new object[]
					{
						"id=\"imgFlg\""
					});
					return true;
				}
				case ColumnId.TaskIcon:
				case ColumnId.MemberIcon:
					break;
				case ColumnId.MarkCompleteCheckbox:
				{
					bool itemProperty12 = this.dataSource.GetItemProperty<bool>(ItemSchema.IsComplete, false);
					writer.Write("<input id=\"chkMkCmp\" type=checkbox class=\"mkCmp\"");
					if (this.IsAssignedTask)
					{
						writer.Write(" disabled");
					}
					writer.Write(itemProperty12 ? " checked>" : ">");
					return true;
				}
				case ColumnId.DueDate:
				{
					ExDateTime itemProperty13 = this.dataSource.GetItemProperty<ExDateTime>(TaskSchema.DueDate, ExDateTime.MinValue);
					if (itemProperty13 != ExDateTime.MinValue)
					{
						writer.Write(itemProperty13.ToString(this.userContext.UserOptions.DateFormat));
						return true;
					}
					return false;
				}
				case ColumnId.MemberEmail:
					goto IL_428;
				case ColumnId.DeletedOnTime:
				{
					ExDateTime itemProperty14 = this.dataSource.GetItemProperty<ExDateTime>(StoreObjectSchema.LastModifiedTime, ExDateTime.MinValue);
					return this.RenderWeekdayDateTime(writer, itemProperty14);
				}
				case ColumnId.DumpsterReceivedTime:
				{
					ExDateTime itemProperty15 = this.dataSource.GetItemProperty<ExDateTime>(ItemSchema.ReceivedTime, ExDateTime.MinValue);
					return this.RenderWeekdayDateTime(writer, itemProperty15);
				}
				case ColumnId.ObjectDisplayName:
				{
					string itemProperty16 = this.dataSource.GetItemProperty<string>(FolderSchema.DisplayName, string.Empty);
					if (string.IsNullOrEmpty(itemProperty16))
					{
						itemProperty16 = this.dataSource.GetItemProperty<string>(ItemSchema.Subject, string.Empty);
					}
					if (itemProperty16.Length == 0)
					{
						return false;
					}
					Utilities.HtmlEncode(itemProperty16, writer);
					return true;
				}
				case ColumnId.ObjectIcon:
				{
					VersionedId itemProperty17 = this.dataSource.GetItemProperty<VersionedId>(FolderSchema.Id);
					if (itemProperty17 != null)
					{
						return ListViewContentsRenderingUtilities.RenderItemIcon(writer, this.userContext, ThemeFileId.Folder);
					}
					return this.RenderIcon(writer);
				}
				case ColumnId.ConversationImportance:
					goto IL_17B;
				case ColumnId.ConversationCategories:
					goto IL_32C;
				default:
				{
					if (columnId != ColumnId.IMAddress)
					{
						goto IL_68B;
					}
					string itemProperty18 = this.dataSource.GetItemProperty<string>(ContactSchema.IMAddress, string.Empty);
					string mobilePhoneNumber = Utilities.NormalizePhoneNumber(this.dataSource.GetItemProperty<string>(ContactSchema.MobilePhone, null) ?? this.dataSource.GetItemProperty<string>(ADOrgPersonSchema.MobilePhone, null));
					if (!string.IsNullOrEmpty(itemProperty18))
					{
						this.RenderSingleEmailAddress(writer, itemProperty18, itemProperty18, itemProperty18, null, EmailAddressIndex.None, RecipientAddress.RecipientAddressFlags.None, null, InstantMessageUtilities.ToSipFormat(itemProperty18), mobilePhoneNumber);
						return true;
					}
					return false;
				}
				}
			}
			return this.RenderIcon(writer);
			IL_17B:
			Importance importance = Importance.Normal;
			object itemProperty19 = this.dataSource.GetItemProperty<object>(ListViewColumns.GetColumn(columnId)[0]);
			if (itemProperty19 is Importance || itemProperty19 is int)
			{
				importance = (Importance)itemProperty19;
			}
			return ListViewContentsRenderingUtilities.RenderImportance(writer, this.UserContext, importance);
			IL_32C:
			this.RenderCategories(writer, columnId == ColumnId.ConversationCategories);
			return true;
			IL_428:
			PropertyDefinition propertyDefinition = ContactSchema.Email1;
			switch (columnId)
			{
			case ColumnId.Email1:
				break;
			case ColumnId.Email2:
				propertyDefinition = ContactSchema.Email2;
				goto IL_467;
			case ColumnId.Email3:
				propertyDefinition = ContactSchema.Email3;
				goto IL_467;
			default:
				if (columnId != ColumnId.MemberEmail)
				{
					goto IL_467;
				}
				break;
			}
			propertyDefinition = ContactSchema.Email1;
			IL_467:
			Participant itemProperty20 = this.dataSource.GetItemProperty<Participant>(propertyDefinition, null);
			if (itemProperty20 == null)
			{
				return false;
			}
			string text = null;
			string text2 = null;
			ContactUtilities.GetParticipantEmailAddress(itemProperty20, out text2, out text);
			if (string.IsNullOrEmpty(text2))
			{
				return false;
			}
			Utilities.HtmlEncode(text2, writer);
			return true;
			IL_68B:
			object itemProperty21 = this.dataSource.GetItemProperty<object>(column[0]);
			if (itemProperty21 is ExDateTime)
			{
				writer.Write(((ExDateTime)itemProperty21).ToString());
			}
			else if (itemProperty21 is DateTime)
			{
				ExDateTime exDateTime = new ExDateTime(this.userContext.TimeZone, (DateTime)itemProperty21);
				writer.Write(exDateTime.ToString());
			}
			else if (itemProperty21 is string)
			{
				string text3 = (string)itemProperty21;
				if (text3.Length == 0)
				{
					return false;
				}
				Utilities.HtmlEncode(text3, writer);
			}
			else if (itemProperty21 is int)
			{
				Utilities.HtmlEncode(((int)itemProperty21).ToString(CultureInfo.CurrentCulture.NumberFormat), writer);
			}
			else if (itemProperty21 is long)
			{
				Utilities.HtmlEncode(((long)itemProperty21).ToString(CultureInfo.CurrentCulture.NumberFormat), writer);
			}
			else if (itemProperty21 is Unlimited<int>)
			{
				if (((Unlimited<int>)itemProperty21).IsUnlimited)
				{
					return false;
				}
				Utilities.HtmlEncode(((Unlimited<int>)itemProperty21).Value.ToString(CultureInfo.CurrentCulture.NumberFormat), writer);
			}
			else
			{
				if (itemProperty21 is PropertyError)
				{
					return false;
				}
				if (itemProperty21 is PropertyError)
				{
					return false;
				}
			}
			return false;
		}

		protected void RenderSelectionImage(TextWriter writer)
		{
			writer.Write("<img class=\"selBg\" src=\"");
			this.UserContext.RenderThemeFileUrl(writer, this.UserContext.IsRtl ? ThemeFileId.HorizontalGradientMaskRtl : ThemeFileId.HorizontalGradientMask);
			writer.Write("\">");
		}

		protected void RenderRowDivider(TextWriter writer)
		{
			writer.Write("<img class=\"lvDiv\" src=\"");
			this.UserContext.RenderThemeFileUrl(writer, ThemeFileId.ListViewDivider);
			writer.Write("\">");
		}

		private static void GetEmailAddressData(Participant participant, out string emailAddress, out string emailAddressForDisplay, out string displayName, out string routingType)
		{
			emailAddressForDisplay = null;
			displayName = null;
			ContactUtilities.GetParticipantEmailAddress(participant, out emailAddressForDisplay, out displayName);
			emailAddress = participant.EmailAddress;
			routingType = participant.RoutingType;
		}

		private static void InitializeColumnsWithAnchorsMap()
		{
			ListViewContents2.columnsWithAnchorsMap = new bool[96];
			for (int i = 0; i < ListViewContents2.columnsWithAnchorsMap.Length; i++)
			{
				ListViewContents2.columnsWithAnchorsMap[i] = true;
			}
			ListViewContents2.columnsWithAnchorsMap[0] = false;
			ListViewContents2.columnsWithAnchorsMap[5] = false;
			ListViewContents2.columnsWithAnchorsMap[6] = false;
			ListViewContents2.columnsWithAnchorsMap[10] = false;
			ListViewContents2.columnsWithAnchorsMap[14] = false;
			ListViewContents2.columnsWithAnchorsMap[15] = false;
			ListViewContents2.columnsWithAnchorsMap[16] = false;
			ListViewContents2.columnsWithAnchorsMap[17] = false;
			ListViewContents2.columnsWithAnchorsMap[18] = false;
			ListViewContents2.columnsWithAnchorsMap[19] = false;
			ListViewContents2.columnsWithAnchorsMap[25] = false;
			ListViewContents2.columnsWithAnchorsMap[26] = false;
			ListViewContents2.columnsWithAnchorsMap[27] = false;
			ListViewContents2.columnsWithAnchorsMap[33] = false;
			ListViewContents2.columnsWithAnchorsMap[34] = false;
			ListViewContents2.columnsWithAnchorsMap[38] = false;
			ListViewContents2.columnsWithAnchorsMap[43] = false;
			ListViewContents2.columnsWithAnchorsMap[44] = false;
			ListViewContents2.columnsWithAnchorsMap[45] = false;
			ListViewContents2.columnsWithAnchorsMap[49] = false;
			ListViewContents2.columnsWithAnchorsMap[67] = false;
			ListViewContents2.columnsWithAnchorsMap[68] = false;
			ListViewContents2.columnsWithAnchorsMap[69] = false;
			ListViewContents2.columnsWithAnchorsMap[70] = false;
			ListViewContents2.columnsWithAnchorsMap[71] = false;
			ListViewContents2.columnsWithAnchorsMap[72] = false;
			ListViewContents2.columnsWithAnchorsMap[73] = false;
			ListViewContents2.columnsWithAnchorsMap[77] = false;
			ListViewContents2.columnsWithAnchorsMap[81] = false;
			ListViewContents2.columnsWithAnchorsMap[83] = false;
			ListViewContents2.columnsWithAnchorsMap[86] = false;
			ListViewContents2.columnsWithAnchorsMap[88] = false;
			ListViewContents2.columnsWithAnchorsMap[89] = false;
			ListViewContents2.columnsWithAnchorsMap[92] = false;
			ListViewContents2.columnsWithAnchorsMap[95] = false;
		}

		protected bool RenderADEmailAddress(IListViewDataSource dataSource, TextWriter writer)
		{
			string text = dataSource.GetItemProperty<SmtpAddress>(ADRecipientSchema.PrimarySmtpAddress, SmtpAddress.Empty).ToString();
			if (text.Length == 0)
			{
				return false;
			}
			RecipientAddress.RecipientAddressFlags recipientAddressFlags = RecipientAddress.RecipientAddressFlags.None;
			RecipientDisplayType? itemProperty = dataSource.GetItemProperty<RecipientDisplayType?>(ADRecipientSchema.RecipientDisplayType, null);
			if (itemProperty == RecipientDisplayType.ConferenceRoomMailbox || itemProperty == RecipientDisplayType.SyncedConferenceRoomMailbox)
			{
				recipientAddressFlags |= RecipientAddress.RecipientAddressFlags.Room;
			}
			RecipientType itemProperty2 = dataSource.GetItemProperty<RecipientType>(ADRecipientSchema.RecipientType, RecipientType.Invalid);
			if (Utilities.IsADDistributionList(itemProperty2))
			{
				recipientAddressFlags |= RecipientAddress.RecipientAddressFlags.DistributionList;
			}
			string itemProperty3 = dataSource.GetItemProperty<string>(ADRecipientSchema.LegacyExchangeDN, string.Empty);
			string mobilePhoneNumber = Utilities.NormalizePhoneNumber(dataSource.GetItemProperty<string>(ADOrgPersonSchema.MobilePhone, null));
			ProxyAddressCollection itemProperty4 = dataSource.GetItemProperty<ProxyAddressCollection>(ADRecipientSchema.EmailAddresses, null);
			string sipUri = InstantMessageUtilities.GetSipUri(itemProperty4);
			this.RenderSingleEmailAddress(writer, dataSource.GetItemProperty<string>(ADRecipientSchema.DisplayName, string.Empty), text, text, itemProperty3, EmailAddressIndex.None, recipientAddressFlags, null, sipUri, mobilePhoneNumber);
			return true;
		}

		protected void RenderCheckbox(TextWriter writer)
		{
			this.UserContext.RenderThemeImage(writer, ThemeFileId.ListViewCheckboxUnchecked, null, new object[]
			{
				"id=\"imgChk\""
			});
		}

		private void RenderCategories(TextWriter writer, bool conversationMode)
		{
			string[] itemProperty = this.dataSource.GetItemProperty<string[]>(conversationMode ? ConversationItemSchema.ConversationCategories : ItemSchema.Categories, null);
			writer.Write("<div id=\"");
			writer.Write("divCatGroup");
			writer.Write("\"");
			if (this.userContext.CanActAsOwner && itemProperty != null && 0 < itemProperty.Length)
			{
				writer.Write(" title=\"");
				for (int i = 0; i < itemProperty.Length; i++)
				{
					if (i != 0)
					{
						writer.Write("; ");
					}
					Utilities.HtmlEncode(itemProperty[i], writer);
				}
				writer.Write("\"");
			}
			writer.Write(">");
			int itemColorInt = -1;
			bool isToDoItem = false;
			FlagStatus itemProperty2;
			if (conversationMode)
			{
				itemProperty2 = (FlagStatus)this.dataSource.GetItemProperty<int>(ConversationItemSchema.ConversationFlagStatus, 0);
			}
			else
			{
				itemProperty2 = this.dataSource.GetItemProperty<FlagStatus>(ItemSchema.FlagStatus, FlagStatus.NotFlagged);
				itemColorInt = this.dataSource.GetItemProperty<int>(ItemSchema.ItemColor, -1);
				isToDoItem = this.dataSource.GetItemProperty<bool>(ItemSchema.IsToDoItem, false);
			}
			OwaStoreObjectId folderId = OwaStoreObjectId.CreateFromString(this.dataSource.ContainerId);
			CategorySwatch.RenderViewCategorySwatches(writer, this.userContext, itemProperty, isToDoItem, itemProperty2, itemColorInt, folderId);
			writer.Write("</div>");
		}

		private bool RenderEmailAddresses(TextWriter writer)
		{
			Dictionary<EmailAddressIndex, StorePropertyDefinition> dictionary = new Dictionary<EmailAddressIndex, StorePropertyDefinition>();
			Dictionary<EmailAddressIndex, Participant> dictionary2 = new Dictionary<EmailAddressIndex, Participant>();
			dictionary.Add(EmailAddressIndex.Email1, ContactSchema.Email1);
			dictionary.Add(EmailAddressIndex.Email2, ContactSchema.Email2);
			dictionary.Add(EmailAddressIndex.Email3, ContactSchema.Email3);
			dictionary.Add(EmailAddressIndex.BusinessFax, ContactSchema.ContactBusinessFax);
			dictionary.Add(EmailAddressIndex.HomeFax, ContactSchema.ContactHomeFax);
			dictionary.Add(EmailAddressIndex.OtherFax, ContactSchema.ContactOtherFax);
			EmailAddressIndex emailAddressIndex = EmailAddressIndex.None;
			foreach (KeyValuePair<EmailAddressIndex, StorePropertyDefinition> keyValuePair in dictionary)
			{
				Participant itemProperty = this.dataSource.GetItemProperty<Participant>(keyValuePair.Value, null);
				if (itemProperty != null && !string.IsNullOrEmpty(itemProperty.EmailAddress))
				{
					emailAddressIndex = keyValuePair.Key;
					dictionary2.Add(keyValuePair.Key, itemProperty);
				}
			}
			string itemProperty2 = this.dataSource.GetItemProperty<string>(ContactSchema.IMAddress, null);
			bool flag = string.IsNullOrEmpty(itemProperty2) || itemProperty2.Trim().Length == 0;
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			string text5 = Utilities.NormalizePhoneNumber(this.dataSource.GetItemProperty<string>(ContactSchema.MobilePhone, null));
			if (dictionary2.Count > 1)
			{
				bool flag2 = false;
				bool flag3 = !string.IsNullOrEmpty(text5);
				writer.Write("<select id=\"ea\">");
				foreach (KeyValuePair<EmailAddressIndex, Participant> keyValuePair2 in dictionary2)
				{
					Participant value = keyValuePair2.Value;
					if (value != null && !string.IsNullOrEmpty(value.EmailAddress))
					{
						ListViewContents2.GetEmailAddressData(value, out text, out text2, out text3, out text4);
						writer.Write("<option aO=1 dn=\"");
						Utilities.HtmlEncode(text3, writer);
						writer.Write("\" rt=\"");
						if (string.IsNullOrEmpty(text4))
						{
							writer.Write("SMTP");
						}
						else
						{
							writer.Write(text4);
						}
						writer.Write("\" rf=\"");
						writer.Write(0);
						writer.Write("\" em=\"");
						Utilities.HtmlEncode(text, writer);
						writer.Write("\" ei=\"");
						writer.Write((int)keyValuePair2.Key);
						if (!flag2)
						{
							string imAddress = flag ? text : itemProperty2;
							writer.Write("\" uri=\"");
							Utilities.HtmlEncode(InstantMessageUtilities.ToSipFormat(imAddress), writer);
							flag2 = true;
						}
						if (flag3)
						{
							writer.Write("\" mo=\"");
							Utilities.HtmlEncode(text5, writer);
							flag3 = false;
						}
						writer.Write("\">");
						Utilities.HtmlEncode(text2, writer);
						writer.Write("</option>");
					}
				}
				writer.Write("</select>");
			}
			else
			{
				if (dictionary2.Count != 1)
				{
					return false;
				}
				Participant participant = dictionary2[emailAddressIndex];
				ListViewContents2.GetEmailAddressData(participant, out text, out text2, out text3, out text4);
				this.RenderSingleEmailAddress(writer, text3, text, text2, string.Empty, emailAddressIndex, RecipientAddress.RecipientAddressFlags.None, text4, InstantMessageUtilities.ToSipFormat(flag ? text : itemProperty2), text5);
			}
			return true;
		}

		private void RenderSingleEmailAddress(TextWriter writer, string displayName, string emailAddress, string emailAddressForDisplay, string legacyExchangeDN, EmailAddressIndex emailAddressIndex, RecipientAddress.RecipientAddressFlags recipientAddressFlags)
		{
			this.RenderSingleEmailAddress(writer, displayName, emailAddress, emailAddressForDisplay, legacyExchangeDN, emailAddressIndex, recipientAddressFlags, null);
		}

		private void RenderSingleEmailAddress(TextWriter writer, string displayName, string emailAddress, string emailAddressForDisplay, string legacyExchangeDN, EmailAddressIndex emailAddressIndex, RecipientAddress.RecipientAddressFlags recipientAddressFlags, string routingType)
		{
			this.RenderSingleEmailAddress(writer, displayName, emailAddress, emailAddressForDisplay, legacyExchangeDN, emailAddressIndex, recipientAddressFlags, routingType, null, null);
		}

		internal void RenderSingleEmailAddress(TextWriter writer, string displayName, string emailAddress, string emailAddressForDisplay, string legacyExchangeDN, EmailAddressIndex emailAddressIndex, RecipientAddress.RecipientAddressFlags recipientAddressFlags, string routingType, string sipUri, string mobilePhoneNumber)
		{
			InstantMessageUtilities.RenderSingleEmailAddress(this.DataSource, writer, displayName, emailAddress, emailAddressForDisplay, legacyExchangeDN, emailAddressIndex, recipientAddressFlags, routingType, sipUri, mobilePhoneNumber);
		}

		public bool RenderWeekdayDateTime(TextWriter writer, ExDateTime date)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (date == ExDateTime.MinValue)
			{
				return false;
			}
			string text = date.ToString(this.userContext.UserOptions.GetWeekdayDateTimeFormat(false));
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			writer.Write("<span>");
			Utilities.HtmlEncode(text, writer);
			writer.Write("</span>");
			return true;
		}

		protected bool RenderSmartDate(TextWriter writer, ExDateTime date)
		{
			return ListViewContentsRenderingUtilities.RenderSmartDate(writer, this.userContext, date);
		}

		protected void RenderVLVAnchorOpen(TextWriter writer)
		{
			if (this.UserContext.BrowserType == BrowserType.Safari)
			{
				writer.Write("<a class=\"linksVLV\" href=\"#\">");
			}
		}

		protected void RenderVLVAnchorClose(TextWriter writer)
		{
			if (this.UserContext.BrowserType == BrowserType.Safari)
			{
				writer.Write("</a>");
			}
		}

		protected string RemoveVLVAnchors(string s)
		{
			if (!s.Contains("<a class=\"linksVLV\" href=\"#\">"))
			{
				return s;
			}
			return s.Replace("<a class=\"linksVLV\" href=\"#\">", string.Empty).Replace("</a>", string.Empty);
		}

		protected virtual string GetHtmlEncodedLocalizeString(Strings.IDs stringId)
		{
			return LocalizedStrings.GetHtmlEncoded(stringId, this.UserContext.UserCulture);
		}

		protected const string VirtualItemListContainerId = "divVLVIL";

		protected const string Unselected = "us";

		protected const string VlvRow = "vr";

		protected const string CategoryGroupId = "divCatGroup";

		private const string VlvAnchorOpenMarkup = "<a class=\"linksVLV\" href=\"#\">";

		private const string VlvAnchorCloseMarkup = "</a>";

		private static bool[] columnsWithAnchorsMap;

		private IListViewDataSource dataSource;

		private UserContext userContext;

		private bool isForVirtualListView;

		private Hashtable properties = new Hashtable();

		internal enum ListViewRowType
		{
			RenderOnlyRow1,
			RenderOnlyRow2,
			RenderAllRows
		}
	}
}
