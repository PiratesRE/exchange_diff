using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetCalendarFolders : CalendarActionBase<GetCalendarFoldersResponse>
	{
		private IRecipientSession ADRecipientSession { get; set; }

		public GetCalendarFolders(MailboxSession session, IRecipientSession adRecipientSession, bool publicFoldersEnabled) : base(session)
		{
			this.ADRecipientSession = adRecipientSession;
			this.publicFoldersEnabled = publicFoldersEnabled;
		}

		public override GetCalendarFoldersResponse Execute()
		{
			this.calendars = this.FindCalendarFolders();
			CalendarGroupInfoList calendarGroupsView = CalendarGroup.GetCalendarGroupsView(base.MailboxSession);
			if (!calendarGroupsView.DefaultGroups.ContainsKey(CalendarGroupType.MyCalendars))
			{
				this.CreateMyCalendarsGroup(calendarGroupsView);
			}
			if (!calendarGroupsView.DefaultGroups.ContainsKey(CalendarGroupType.OtherCalendars))
			{
				this.CreateOtherCalendarsGroup(calendarGroupsView);
			}
			this.FixGroupAssociations(calendarGroupsView);
			if (calendarGroupsView.DuplicateNodes.Count > 0)
			{
				this.RemoveDuplicateEntries(calendarGroupsView.DuplicateNodes);
			}
			GetCalendarFolders.CalendarGroupList calendarGroupList = this.ProcessGroupInformation(calendarGroupsView);
			CalendarFolderType[] array = new CalendarFolderType[this.calendars.Count];
			this.calendars.Values.CopyTo(array, 0);
			return new GetCalendarFoldersResponse(calendarGroupList.ToArray(), array);
		}

		private Dictionary<StoreObjectId, CalendarFolderType> FindCalendarFolders()
		{
			Dictionary<StoreObjectId, CalendarFolderType> dictionary = new Dictionary<StoreObjectId, CalendarFolderType>();
			StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
			StoreObjectId defaultFolderId2 = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			using (Folder folder = Folder.Bind(base.MailboxSession, DefaultFolderType.Root))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, GetCalendarFolders.folderProperties))
				{
					for (;;)
					{
						bool flag = false;
						int num = -1;
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
						if (propertyBags.Length == 0)
						{
							break;
						}
						foreach (IStorePropertyBag storePropertyBag in propertyBags)
						{
							VersionedId versionedId = storePropertyBag.TryGetProperty(FolderSchema.Id) as VersionedId;
							int num2 = storePropertyBag.TryGetValueOrDefault(FolderSchema.FolderHierarchyDepth, -1);
							if (num2 == -1)
							{
								ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError<VersionedId>((long)this.GetHashCode(), "Folder without folder depth set. FolderId: {0}", versionedId);
							}
							else if (defaultFolderId.Equals(versionedId.ObjectId))
							{
								flag = true;
								num = num2;
								ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Found Deleted Items at folder depth: {0}. Ignoring folders until FolderDepth is the same again.", num2);
							}
							else
							{
								if (flag)
								{
									if (num2 > num)
									{
										ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Ignoring calendar found under Deleted Items - FolderId: {0}", versionedId);
										goto IL_1FA;
									}
									flag = false;
									ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Folder depth is the same or smaller than Deleted Items. Stop ignoring folders. FolderDepth: {0}", num2);
								}
								bool flag2 = storePropertyBag.TryGetValueOrDefault(FolderSchema.IsHidden, false);
								if (!flag2 || defaultFolderId2.Equals(versionedId.ObjectId))
								{
									string containerClass = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.ContainerClass, string.Empty);
									if (ObjectClass.IsCalendarFolder(containerClass))
									{
										StoreObjectId storeObjectId = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.ParentItemId, null);
										if (storeObjectId == null)
										{
											ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError<VersionedId>((long)this.GetHashCode(), "Encountered folder without parent folder set. FolderId: {1}", versionedId);
										}
										else
										{
											string text = storePropertyBag.TryGetValueOrDefault(FolderSchema.DisplayName, string.Empty);
											if (string.IsNullOrEmpty(text))
											{
												ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Calendar folder with empty name encountered. Skipping. FolderId: {0}", versionedId);
											}
											else
											{
												EffectiveRights effectiveRights = storePropertyBag.TryGetValueOrDefault(StoreObjectSchema.EffectiveRights, EffectiveRights.None);
												dictionary.Add(StoreId.GetStoreObjectId(versionedId), this.GetCalendarFolderType(versionedId, storeObjectId, text, effectiveRights));
											}
										}
									}
								}
							}
							IL_1FA:;
						}
					}
				}
			}
			return dictionary;
		}

		private CalendarFolderType GetCalendarFolderType(StoreId folderId, StoreObjectId parentFolderId, string folderName, EffectiveRights effectiveRights)
		{
			return new CalendarFolderType
			{
				FolderId = IdConverter.ConvertStoreFolderIdToFolderId(folderId, base.MailboxSession),
				ParentFolderId = IdConverter.ConvertStoreFolderIdToFolderId(parentFolderId, base.MailboxSession),
				DisplayName = folderName,
				FolderClass = "IPF.Appointment",
				EffectiveRights = EffectiveRightsProperty.GetFromEffectiveRights(effectiveRights, base.MailboxSession)
			};
		}

		private void RemoveDuplicateEntries(IList<FolderTreeDataInfo> duplicates)
		{
			List<StoreId> list = new List<StoreId>();
			foreach (FolderTreeDataInfo folderTreeDataInfo in duplicates)
			{
				list.Add(folderTreeDataInfo.Id);
				LocalCalendarGroupEntryInfo localCalendarGroupEntryInfo = folderTreeDataInfo as LocalCalendarGroupEntryInfo;
				CalendarGroupInfo calendarGroupInfo = folderTreeDataInfo as CalendarGroupInfo;
				if (localCalendarGroupEntryInfo != null)
				{
					ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<string, VersionedId, Guid>((long)this.GetHashCode(), "Removing duplicate CalendarEntry entry. CalendarName: {0}, StoreId: {1}, ParentGroupId: {2}. ", localCalendarGroupEntryInfo.CalendarName ?? string.Empty, localCalendarGroupEntryInfo.Id, localCalendarGroupEntryInfo.ParentGroupClassId);
				}
				if (calendarGroupInfo != null)
				{
					ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<string, VersionedId, Guid>((long)this.GetHashCode(), "Removing duplicate CalendarGroup entry. GroupName: {0}, StoreId: {1}, GroupId: {2}.", calendarGroupInfo.GroupName, calendarGroupInfo.Id, calendarGroupInfo.GroupClassId);
				}
			}
			if (list.Count > 0)
			{
				AggregateOperationResult aggregateOperationResult = base.MailboxSession.Delete(DeleteItemFlags.SoftDelete, list.ToArray());
				if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
				{
					ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug((long)this.GetHashCode(), "Unable to delete duplicate FolderTreeData entries.");
				}
			}
		}

		private void CreateMyCalendarsGroup(CalendarGroupInfoList groups)
		{
			Dictionary<StoreObjectId, LocalCalendarGroupEntryInfo> calendarFolderMapping = groups.CalendarFolderMapping;
			CalendarGroupInfo calendarGroupInfo;
			using (CalendarGroup calendarGroup = CalendarGroup.Bind(base.MailboxSession, CalendarGroupType.MyCalendars))
			{
				ReadOnlyCollection<CalendarGroupEntryInfo> childCalendars = calendarGroup.GetChildCalendars();
				calendarGroupInfo = calendarGroup.GetCalendarGroupInfo();
				foreach (CalendarGroupEntryInfo calendarGroupEntryInfo in childCalendars)
				{
					calendarGroupInfo.Calendars.Add(calendarGroupEntryInfo);
					LocalCalendarGroupEntryInfo localCalendarGroupEntryInfo = calendarGroupEntryInfo as LocalCalendarGroupEntryInfo;
					if (localCalendarGroupEntryInfo != null)
					{
						LocalCalendarGroupEntryInfo item;
						if (calendarFolderMapping.TryGetValue(localCalendarGroupEntryInfo.CalendarId, out item))
						{
							groups.DuplicateNodes.Add(item);
							calendarFolderMapping[localCalendarGroupEntryInfo.CalendarId] = localCalendarGroupEntryInfo;
						}
						else
						{
							calendarFolderMapping.Add(localCalendarGroupEntryInfo.CalendarId, localCalendarGroupEntryInfo);
						}
					}
				}
			}
			groups.Add(calendarGroupInfo);
			groups.DefaultGroups.Add(CalendarGroupType.MyCalendars, calendarGroupInfo);
		}

		private void CreateOtherCalendarsGroup(CalendarGroupInfoList groups)
		{
			CalendarGroupInfo calendarGroupInfo;
			try
			{
				using (CalendarGroup calendarGroup = CalendarGroup.Bind(base.MailboxSession, CalendarGroupType.OtherCalendars))
				{
					calendarGroupInfo = calendarGroup.GetCalendarGroupInfo();
				}
			}
			catch (StorageTransientException arg)
			{
				ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "Unable to create the other calendars group due to transient error. {0}", arg);
				return;
			}
			groups.Add(calendarGroupInfo);
			groups.DefaultGroups.Add(CalendarGroupType.OtherCalendars, calendarGroupInfo);
		}

		private GetCalendarFolders.CalendarGroupList ProcessGroupInformation(CalendarGroupInfoList groups)
		{
			GetCalendarFolders.CalendarGroupList calendarGroupList = new GetCalendarFolders.CalendarGroupList(groups.Count);
			StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			List<KeyValuePair<LinkedCalendarGroupEntryInfo, LinkedCalendarEntry>> list = new List<KeyValuePair<LinkedCalendarGroupEntryInfo, LinkedCalendarEntry>>();
			foreach (CalendarGroupInfo calendarGroupInfo in groups)
			{
				CalendarGroup calendarGroup = new CalendarGroup();
				calendarGroup.ItemId = IdConverter.ConvertStoreItemIdToItemId(calendarGroupInfo.Id, base.MailboxSession);
				calendarGroup.GroupId = calendarGroupInfo.GroupClassId.ToString();
				calendarGroup.GroupType = calendarGroupInfo.GroupType;
				calendarGroup.GroupName = calendarGroupInfo.GroupName;
				if (calendarGroup.GroupType == CalendarGroupType.MyCalendars)
				{
					calendarGroupList.DefaultGroup = calendarGroup;
				}
				List<CalendarEntry> list2 = new List<CalendarEntry>(calendarGroupInfo.Calendars.Count);
				foreach (CalendarGroupEntryInfo calendarGroupEntryInfo in calendarGroupInfo.Calendars)
				{
					CalendarEntry calendarEntry = this.GetCalendarEntry(calendarGroupEntryInfo, calendarGroupInfo, defaultFolderId);
					if (calendarEntry != null)
					{
						if (calendarEntry is LinkedCalendarEntry && calendarEntry.CalendarFolderType != CalendarFolderTypeEnum.PublicCalendarFolder)
						{
							LinkedCalendarGroupEntryInfo key = (LinkedCalendarGroupEntryInfo)calendarGroupEntryInfo;
							list.Add(new KeyValuePair<LinkedCalendarGroupEntryInfo, LinkedCalendarEntry>(key, (LinkedCalendarEntry)calendarEntry));
						}
						list2.Add(calendarEntry);
					}
				}
				calendarGroup.Calendars = list2.ToArray();
				calendarGroupList.Add(calendarGroup);
			}
			if (list.Count > 0)
			{
				this.ResolveSharedCalendarsInfo(list);
			}
			return calendarGroupList;
		}

		private CalendarEntry GetCalendarEntry(CalendarGroupEntryInfo calendarInfo, CalendarGroupInfo group, StoreObjectId defaultCalendarId)
		{
			CalendarEntry calendarEntry;
			if (calendarInfo is LocalCalendarGroupEntryInfo)
			{
				LocalCalendarGroupEntryInfo localCalendarGroupEntryInfo = (LocalCalendarGroupEntryInfo)calendarInfo;
				LocalCalendarEntry localCalendarEntry = calendarEntry = new LocalCalendarEntry();
				if (!this.calendars.ContainsKey(localCalendarGroupEntryInfo.CalendarId))
				{
					ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<string, VersionedId, Guid>((long)this.GetHashCode(), "Found local calendar entry without a matching calendar folder. Skipping. CalendarName: {0}, ItemId: {1}, ParentGroupId: {2}", calendarInfo.CalendarName, calendarInfo.Id, calendarInfo.ParentGroupClassId);
					return null;
				}
				localCalendarEntry.CalendarFolderId = IdConverter.ConvertStoreFolderIdToFolderId(localCalendarGroupEntryInfo.CalendarId, base.MailboxSession);
				localCalendarEntry.CalendarFolderType = this.GetCalendarType(localCalendarGroupEntryInfo);
				localCalendarEntry.IsDefaultCalendar = localCalendarEntry.CalendarFolderType.Equals(CalendarFolderTypeEnum.DefaultCalendar);
				localCalendarEntry.IsInternetCalendar = localCalendarEntry.CalendarFolderType.Equals(CalendarFolderTypeEnum.InternetCalendar);
				localCalendarEntry.IsGroupMailboxCalendar = base.MailboxSession.IsGroupMailbox();
			}
			else
			{
				if (!(calendarInfo is LinkedCalendarGroupEntryInfo))
				{
					ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<string, Guid, string>((long)this.GetHashCode(), "CalendarInfo with unexpected type encountered. GroupName: {0}, GroupId: {1}, CalendarName: {2}", group.GroupName, group.GroupClassId, calendarInfo.CalendarName);
					return null;
				}
				LinkedCalendarGroupEntryInfo linkedCalendarGroupEntryInfo = (LinkedCalendarGroupEntryInfo)calendarInfo;
				LinkedCalendarEntry linkedCalendarEntry = calendarEntry = new LinkedCalendarEntry();
				if (string.IsNullOrEmpty(linkedCalendarGroupEntryInfo.CalendarOwner) && !linkedCalendarGroupEntryInfo.IsPublicCalendarFolder)
				{
					ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError<string, VersionedId, Guid>((long)this.GetHashCode(), "Missing legDN for linked calendar. This calendar will not be returned in the list of calendars. CalendarName: {0}, ItemId: {1}, ParentGroupId: {2}", calendarInfo.CalendarName, calendarInfo.Id, calendarInfo.ParentGroupClassId);
					return null;
				}
				if (linkedCalendarGroupEntryInfo.IsPublicCalendarFolder)
				{
					if (!this.publicFoldersEnabled)
					{
						ExTraceGlobals.GetCalendarFoldersCallTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "Ignoring public calendar folder '{0}' because public folders are not enabled for this user", calendarInfo.Id);
						return null;
					}
					ConcatenatedIdAndChangeKey concatenatedIdForPublicFolder = IdConverter.GetConcatenatedIdForPublicFolder(linkedCalendarGroupEntryInfo.CalendarId);
					linkedCalendarEntry.SharedFolderId = new FolderId(concatenatedIdForPublicFolder.Id, concatenatedIdForPublicFolder.ChangeKey);
					linkedCalendarEntry.CalendarFolderType = CalendarFolderTypeEnum.PublicCalendarFolder;
					linkedCalendarEntry.OwnerEmailAddress = base.MailboxSession.DisplayAddress;
					linkedCalendarEntry.IsOwnerEmailAddressInvalid = false;
					linkedCalendarEntry.IsGeneralScheduleCalendar = false;
				}
				else
				{
					linkedCalendarEntry.OwnerEmailAddress = linkedCalendarGroupEntryInfo.CalendarOwner;
					linkedCalendarEntry.IsGeneralScheduleCalendar = linkedCalendarGroupEntryInfo.IsGeneralScheduleCalendar;
					linkedCalendarEntry.IsOwnerEmailAddressInvalid = true;
				}
			}
			calendarEntry.CalendarName = calendarInfo.CalendarName;
			calendarEntry.CalendarColor = calendarInfo.CalendarColor;
			calendarEntry.ParentGroupId = calendarInfo.ParentGroupClassId.ToString();
			calendarEntry.ItemId = IdConverter.ConvertStoreItemIdToItemId(calendarInfo.Id, base.MailboxSession);
			return calendarEntry;
		}

		private CalendarFolderTypeEnum GetCalendarType(LocalCalendarGroupEntryInfo localCalendarInfo)
		{
			if (localCalendarInfo.CalendarId.Equals(base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar)))
			{
				return CalendarFolderTypeEnum.DefaultCalendar;
			}
			if (localCalendarInfo.CalendarId.Equals(base.MailboxSession.GetDefaultFolderId(DefaultFolderType.BirthdayCalendar)))
			{
				return CalendarFolderTypeEnum.BirthdayCalendar;
			}
			if (localCalendarInfo.IsInternetCalendar)
			{
				return CalendarFolderTypeEnum.InternetCalendar;
			}
			return CalendarFolderTypeEnum.None;
		}

		private void ResolveSharedCalendarsInfo(List<KeyValuePair<LinkedCalendarGroupEntryInfo, LinkedCalendarEntry>> sharedCalendars)
		{
			List<string> legacyDns = new List<string>(sharedCalendars.Count);
			sharedCalendars.ForEach(delegate(KeyValuePair<LinkedCalendarGroupEntryInfo, LinkedCalendarEntry> calendarInfo)
			{
				legacyDns.Add(calendarInfo.Key.CalendarOwner);
			});
			Result<ADRawEntry>[] results = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				results = this.ADRecipientSession.FindByExchangeLegacyDNs(legacyDns.ToArray(), new PropertyDefinition[]
				{
					ADRecipientSchema.PrimarySmtpAddress,
					IADMailStorageSchema.ExchangeGuid,
					ADRecipientSchema.EmailAddresses,
					ADRecipientSchema.RecipientTypeDetails
				});
			});
			if (!adoperationResult.Succeeded)
			{
				ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Call to FindADRecipientsByLegacyExchangeDNs failed. OwnerEmailAddress property will not be set. LegacyDNs: {0}, Exception: {1}", string.Join(", ", legacyDns), adoperationResult.Exception);
				return;
			}
			if (results == null || results.Length == 0)
			{
				ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError<string>((long)this.GetHashCode(), "No ADRecipients were returned. OwnerEmailAddress properties will not be set. LegacyDNs: {0}", string.Join(", ", legacyDns));
				return;
			}
			for (int i = 0; i < results.Length; i++)
			{
				LinkedCalendarGroupEntryInfo key = sharedCalendars[i].Key;
				LinkedCalendarEntry value = sharedCalendars[i].Value;
				if (results[i].Data == null)
				{
					ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError((long)this.GetHashCode(), "No data for ADRecipient was returned. OwnerEmailAddress property will not be set. CalendarName: {0}, ItemId: {1}, ParentGroupId: {2}, OwnerLegDN: {3}", new object[]
					{
						key.CalendarName,
						key.Id,
						key.ParentGroupClassId,
						key.CalendarOwner
					});
				}
				else
				{
					SmtpAddress value2 = (SmtpAddress)results[i].Data[ADRecipientSchema.PrimarySmtpAddress];
					Guid guid = (Guid)results[i].Data[IADMailStorageSchema.ExchangeGuid];
					RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)results[i].Data[ADRecipientSchema.RecipientTypeDetails];
					string ownerSipUri = (from proxyAddress in (ProxyAddressCollection)results[i].Data[ADRecipientSchema.EmailAddresses]
					where string.Compare(proxyAddress.PrefixString, ProxyAddressPrefix.SIP.ToString(), StringComparison.OrdinalIgnoreCase) == 0
					select proxyAddress.ProxyAddressString).FirstOrDefault<string>();
					if (value2 == SmtpAddress.Empty)
					{
						ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError((long)this.GetHashCode(), "ADRecipient doesn't have a PrimarySmtpAddress. OwnerEmailAddress property will not be set. CalendarName: {0}, ItemId: {1}, ParentGroupId: {2}, OwnerLegDN: {3}", new object[]
						{
							key.CalendarName,
							key.Id,
							key.ParentGroupClassId,
							key.CalendarOwner
						});
					}
					else if (guid == Guid.Empty)
					{
						ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError((long)this.GetHashCode(), "ADRecipient doesn't have a Mailbox Guid. Calendar folder id will not be correct. CalendarName: {0}, ItemId: {1}, ParentGroupId: {2}, OwnerLegDN: {3}", new object[]
						{
							key.CalendarName,
							key.Id,
							key.ParentGroupClassId,
							key.CalendarOwner
						});
					}
					else
					{
						value.OwnerEmailAddress = value2.ToString();
						value.OwnerSipUri = ownerSipUri;
						value.IsOwnerEmailAddressInvalid = false;
						value.IsGroupMailboxCalendar = (recipientTypeDetails == RecipientTypeDetails.GroupMailbox);
						if (key.CalendarId != null)
						{
							ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(key.CalendarId, new MailboxId(guid), null);
							value.SharedFolderId = new FolderId(concatenatedId.Id, concatenatedId.ChangeKey);
						}
					}
				}
			}
		}

		private void FixGroupAssociations(CalendarGroupInfoList groups)
		{
			List<KeyValuePair<StoreObjectId, CalendarFolderType>> list = new List<KeyValuePair<StoreObjectId, CalendarFolderType>>();
			foreach (KeyValuePair<StoreObjectId, CalendarFolderType> item in this.calendars)
			{
				if (!groups.CalendarFolderMapping.ContainsKey(item.Key))
				{
					list.Add(item);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			CalendarGroupInfo calendarGroupInfo = groups.DefaultGroups[CalendarGroupType.MyCalendars];
			using (CalendarGroup calendarGroup = CalendarGroup.Bind(base.MailboxSession, CalendarGroupType.MyCalendars))
			{
				foreach (KeyValuePair<StoreObjectId, CalendarFolderType> keyValuePair in list)
				{
					StoreObjectId key = keyValuePair.Key;
					CalendarFolderType value = keyValuePair.Value;
					using (CalendarGroupEntry calendarGroupEntry = CalendarGroupEntry.Create(base.MailboxSession, key, calendarGroup))
					{
						calendarGroupEntry.CalendarName = value.DisplayName;
						calendarGroupEntry.CalendarRecordKey = key.ProviderLevelItemId;
						ConflictResolutionResult conflictResolutionResult = calendarGroupEntry.Save(SaveMode.NoConflictResolution);
						if (conflictResolutionResult.SaveStatus != SaveResult.Success)
						{
							ExTraceGlobals.GetCalendarFoldersCallTracer.TraceError<CalendarFolderType, StoreObjectId>((long)this.GetHashCode(), "Unable to associate calendar with MyCalendars group. CalendarName: {0}, FolderId: {1}.", value, key);
						}
						else
						{
							calendarGroupEntry.Load();
							calendarGroupInfo.Calendars.Add(calendarGroupEntry.GetCalendarGroupEntryInfo());
						}
					}
				}
			}
		}

		private static readonly PropertyDefinition[] folderProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName,
			StoreObjectSchema.ContainerClass,
			FolderSchema.IsHidden,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.EffectiveRights,
			FolderSchema.FolderHierarchyDepth
		};

		private Dictionary<StoreObjectId, CalendarFolderType> calendars;

		private bool publicFoldersEnabled;

		private class CalendarGroupList : List<CalendarGroup>
		{
			public CalendarGroupList(int capacity) : base(capacity)
			{
			}

			public CalendarGroup DefaultGroup { get; set; }
		}
	}
}
