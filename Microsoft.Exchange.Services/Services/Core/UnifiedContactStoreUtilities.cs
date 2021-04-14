using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UnifiedContactStoreUtilities
	{
		public UnifiedContactStoreUtilities(IMailboxSession session, IXSOFactory xsoFactory) : this(session, xsoFactory, Global.UnifiedContactStoreConfiguration)
		{
		}

		public UnifiedContactStoreUtilities(IMailboxSession session, IXSOFactory xsoFactory, IUnifiedContactStoreConfiguration configuration)
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "UnifiedContactStoreUtilities: Created.");
			this.session = session;
			this.xsoFactory = xsoFactory;
			this.configuration = configuration;
		}

		protected IMailboxSession MailboxSession
		{
			get
			{
				return this.session;
			}
		}

		private int MaxPdlsAllowed
		{
			get
			{
				return this.configuration.MaxImGroups + UnifiedContactStoreUtilities.SystemPdls.Length;
			}
		}

		private int MaxContactsAllowed
		{
			get
			{
				return this.configuration.MaxImContacts;
			}
		}

		public ImItemList GetImItemList(ExtendedPropertyUri[] extendedPropertyUris)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<int>((long)this.GetHashCode(), "GetImItemList: Called with {0} extended properties.", (extendedPropertyUris == null) ? 0 : extendedPropertyUris.Length);
			List<RawImGroup> list;
			HashSet<StoreObjectId> hashSet;
			this.GetGroupsAndMembers(extendedPropertyUris, out list, out hashSet);
			if (list.Count == 0)
			{
				ExTraceGlobals.UCSTracer.TraceWarning((long)this.GetHashCode(), "GetImItemList: No groups found.");
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ImGroupCount", list.Count);
				return UnifiedContactStoreUtilities.EmptyImItemList;
			}
			List<Persona> personas = this.GetPersonas(extendedPropertyUris);
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (StoreObjectId storeId in hashSet)
			{
				ItemId itemId = this.Convert(storeId);
				hashSet2.Add(itemId.Id);
			}
			List<Persona> list2 = new List<Persona>(personas.Count);
			int num = 0;
			foreach (Persona persona in personas)
			{
				bool flag = false;
				foreach (Attribution attribution in persona.Attributions)
				{
					if (hashSet2.Contains(attribution.SourceId.Id))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					list2.Add(persona);
					num += persona.Attributions.Length;
				}
				else
				{
					ExTraceGlobals.UCSTracer.TraceWarning<ItemId>((long)this.GetHashCode(), "GetImItemList: Skipping persona {0} as it is not part of any PDL.", persona.PersonaId);
				}
			}
			ExTraceGlobals.UCSTracer.TraceDebug<int, int>((long)this.GetHashCode(), "GetImItemList: Groups: {0}, Personas: {1}", list.Count, list2.Count);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ImGroupCount", list.Count);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "PersonaCount", list2.Count);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ContactCount", num);
			ImGroup[] groups = this.Convert(list.ToArray());
			return new ImItemList
			{
				Groups = groups,
				Personas = list2.ToArray()
			};
		}

		public RawImItemList GetImItems(StoreId[] contactIds, StoreId[] groupIds, ExtendedPropertyUri[] extendedPropertyUris)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "GetImItems: Called for {0} contacts, {1} groups, {2} extended properties.", (contactIds == null) ? 0 : contactIds.Length, (groupIds == null) ? 0 : groupIds.Length, (extendedPropertyUris == null) ? 0 : extendedPropertyUris.Length);
			List<RawImGroup> list;
			HashSet<StoreObjectId> hashSet;
			this.GetGroupsAndMembers(extendedPropertyUris, out list, out hashSet);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (list.Count == 0)
			{
				ExTraceGlobals.UCSTracer.TraceWarning((long)this.GetHashCode(), "GetImItems: No groups found.");
				return UnifiedContactStoreUtilities.EmptyRawImItemList;
			}
			HashSet<PersonId> hashSet2 = new HashSet<PersonId>((contactIds == null) ? 0 : contactIds.Length);
			if (contactIds != null)
			{
				this.Convert(extendedPropertyUris);
				foreach (StoreId id in contactIds)
				{
					StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
					if (!hashSet.Contains(storeObjectId))
					{
						ExTraceGlobals.UCSTracer.TraceWarning<StoreObjectId>((long)this.GetHashCode(), "GetImItems: ContactId: {0} not part of any UCS pdl.", storeObjectId);
						num3++;
					}
					else
					{
						IContact contact;
						if (this.TryRetrieveContact(storeObjectId, out contact))
						{
							using (contact)
							{
								hashSet2.TryAdd((PersonId)contact[ContactSchema.PersonId]);
								goto IL_121;
							}
						}
						ExTraceGlobals.UCSTracer.TraceWarning<StoreObjectId>((long)this.GetHashCode(), "GetImItems: Failed to retrieve contact: {0}", storeObjectId);
						num++;
					}
					IL_121:;
				}
			}
			List<RawImGroup> list2 = new List<RawImGroup>((groupIds == null) ? 0 : groupIds.Length);
			if (groupIds != null)
			{
				for (int j = 0; j < groupIds.Length; j++)
				{
					StoreId id2 = groupIds[j];
					StoreObjectId groupId = StoreId.GetStoreObjectId(id2);
					RawImGroup rawImGroup = list.Find((RawImGroup group) => group.ExchangeStoreId.Equals(groupId));
					if (rawImGroup != null)
					{
						list2.Add(rawImGroup);
					}
					else
					{
						ExTraceGlobals.UCSTracer.TraceWarning<StoreObjectId>((long)this.GetHashCode(), "GetImItems: Failed to retrieve group: {0}", groupId);
						num2++;
					}
				}
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "MisGrpCnt", num2);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "InvCntCnt", num3);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "MisCntCnt", num);
			return new RawImItemList
			{
				Groups = list2.ToArray(),
				Personas = hashSet2.ToArray()
			};
		}

		public StoreObjectId GetSystemPdlId(string displayName, string className)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetSystemPdlId: Retrieving system PDL with displayName {0} className {1}", displayName ?? "<Null>", className ?? "<Null>");
			StoreObjectId storeObjectId = this.FindSystemPdl(className);
			if (storeObjectId != null)
			{
				return storeObjectId;
			}
			return this.CreatePDL(displayName, className, null);
		}

		public RawImGroup GetUserImDGWith(string smtpAddress, string displayName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetUserImDGWith: Retrieving DG-PDL with smtpAddress {0} displayName {1}", smtpAddress ?? "<Null>", displayName ?? "<Null>");
			StoreObjectId userDistributionGroupPdlIdWith = this.GetUserDistributionGroupPdlIdWith(smtpAddress, displayName);
			return this.LoadImGroupFrom(userDistributionGroupPdlIdWith);
		}

		public RawImGroup GetUserImGroupWith(string displayName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "GetUserImGroupWith: Retrieving PDL with displayName {0}", displayName ?? "<Null>");
			StoreObjectId userPdlIdWith = this.GetUserPdlIdWith(displayName);
			return this.LoadImGroupFrom(userPdlIdWith);
		}

		public void AddContactToGroup(StoreId contactId, string displayName, StoreId groupId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId, string, StoreId>((long)this.GetHashCode(), "AddContactToGroup: Adding contact: {0}/{1} to group: {2}", contactId, displayName ?? "<Null>", groupId);
			if (groupId == null)
			{
				groupId = this.GetSystemPdlId(UnifiedContactStoreUtilities.OtherContactsPdlDisplayName, "IPM.DistList.MOC.OtherContacts");
			}
			using (IDistributionList distributionList = this.RetrieveGroupPdl(groupId, new PropertyDefinition[0]))
			{
				this.AddContactToPdl(StoreId.GetStoreObjectId(contactId), displayName, distributionList);
			}
		}

		public void RemoveContactFromGroup(StoreId contactStoreId, StoreId groupStoreId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId, StoreId>((long)this.GetHashCode(), "RemoveContactFromGroup: Removing contact: {0} from group: {1}", contactStoreId, groupStoreId);
			StoreObjectId contactId = StoreId.GetStoreObjectId(contactStoreId);
			StoreObjectId groupId = StoreId.GetStoreObjectId(groupStoreId);
			using (this.RetrieveContact(contactId))
			{
			}
			this.ProcessPdlsContainingContact(contactId, delegate(IList<IDistributionList> parentPdls)
			{
				IDistributionList distributionList;
				IDistributionList distributionList2;
				bool flag;
				this.GetPdlsFromWhichContactNeedsToBeRemoved(contactId, parentPdls, groupId, out distributionList, out distributionList2, out flag);
				if (distributionList == null)
				{
					this.IndicateInvalidImGroupId();
				}
				this.RemoveContactFromPdl(contactId, distributionList);
				try
				{
					if (distributionList2 != null && distributionList2 != distributionList)
					{
						this.RemoveContactFromPdl(contactId, distributionList2);
					}
					if (flag)
					{
						this.DeleteContact(contactStoreId);
					}
				}
				catch (StorageTransientException ex)
				{
					ExTraceGlobals.UCSTracer.TraceWarning<string>((long)this.GetHashCode(), "RemoveContactFromGroup: Ignoring: {0}", ex.ToString());
				}
				catch (StoragePermanentException ex2)
				{
					ExTraceGlobals.UCSTracer.TraceWarning<string>((long)this.GetHashCode(), "RemoveContactFromGroup: Ignoring: {0}", ex2.ToString());
				}
			});
		}

		public void RemoveContactFromPdl(StoreObjectId contactId, IDistributionList distributionList)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, string>((long)this.GetHashCode(), "RemoveContactFromPdl: Removing contact: {0} from DL: {1}", contactId, distributionList.DisplayName ?? "<Null>");
			this.RemoveContactsFromPdl(new HashSet<StoreObjectId>(1)
			{
				contactId
			}, distributionList);
		}

		public string RetrieveContactDisplayName(StoreId contactStoreId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "RetrieveContactDisplayName: ContactStoreId: {0}", contactStoreId);
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(contactStoreId);
			using (IContact contact = this.RetrieveContact(storeObjectId))
			{
				object obj = contact.TryGetProperty(StoreObjectSchema.DisplayName);
				PropertyError propertyError = obj as PropertyError;
				if (propertyError != null)
				{
					ExTraceGlobals.UCSTracer.TraceDebug<StoreId, PropertyError>((long)this.GetHashCode(), "RetrieveContactDisplayName: ContactStoreId: {0}, Failed to retrieve displayName: {1}", contactStoreId, propertyError);
				}
				else
				{
					string text = (string)obj;
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
					ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "RetrieveContactDisplayName: ContactStoreId: {0}, DisplayName is empty", contactStoreId);
				}
				object obj2 = contact.TryGetProperty(ContactSchema.OtherTelephone);
				propertyError = (obj2 as PropertyError);
				if (propertyError != null)
				{
					ExTraceGlobals.UCSTracer.TraceDebug<StoreId, PropertyError>((long)this.GetHashCode(), "RetrieveContactDisplayName: ContactStoreId: {0}, Failed to retrieve otherTelephone: {1}", contactStoreId, propertyError);
				}
				else
				{
					string text2 = (string)obj2;
					if (!string.IsNullOrEmpty(text2))
					{
						return text2;
					}
					ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "RetrieveContactDisplayName: ContactStoreId: {0}, otherTelephone is empty", contactStoreId);
				}
			}
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "RetrieveContactDisplayName: ContactStoreId: {0}, Returning null.", contactStoreId);
			return null;
		}

		public void RetrieveOrCreateContact(string imAddress, string displayName, out StoreObjectId contactId, out PersonId personId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string>((long)this.GetHashCode(), "RetrieveOrCreateContact: ImAddress: {0}, DisplayName: {1}", imAddress ?? "<Null>", displayName ?? "<Null>");
			this.RetrieveOrCreateContact(imAddress, null, displayName, null, null, out contactId, out personId);
		}

		public void RetrieveOrCreateContact(string imAddress, EmailAddress emailAddress, string displayName, string firstName, string lastName, out StoreObjectId contactId, out PersonId personId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "RetrieveOrCreateContact: ImAddress: {0}, EmailAddress: {1}, DisplayName: {2}, FirstName: {3}, SurName: {4}", new object[]
			{
				imAddress ?? "<Null>",
				(emailAddress == null) ? "<Null>" : emailAddress.Address,
				displayName ?? "<Null>",
				firstName,
				lastName
			});
			if (emailAddress == null && string.IsNullOrEmpty(imAddress))
			{
				throw new ArgumentException("emailAddress or imAddress is mandatory.");
			}
			ArgumentValidator.ThrowIfNull(displayName, "displayName");
			StoreObjectId objectId;
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, DefaultFolderType.QuickContacts))
			{
				StoreObjectId storeObjectId = null;
				personId = null;
				if (!string.IsNullOrEmpty(imAddress))
				{
					storeObjectId = this.FindImContactByProperty(folder, ContactSchema.IMAddress, imAddress, out personId);
				}
				if (storeObjectId == null && emailAddress != null)
				{
					storeObjectId = this.FindImContactByProperty(folder, ContactSchema.Email1EmailAddress, emailAddress.Address, out personId);
				}
				objectId = folder.Id.ObjectId;
				if (storeObjectId != null)
				{
					contactId = storeObjectId;
					return;
				}
				this.CheckImContactLimit(folder);
			}
			contactId = this.CreateNewContact(objectId, delegate(IContact newContact)
			{
				if (displayName != null)
				{
					newContact.DisplayName = displayName;
				}
				if (firstName != null)
				{
					newContact[ContactSchema.GivenName] = firstName;
				}
				if (lastName != null)
				{
					newContact[ContactSchema.Surname] = lastName;
				}
				if (!string.IsNullOrEmpty(imAddress))
				{
					newContact.ImAddress = imAddress;
				}
				if (emailAddress != null)
				{
					newContact.EmailAddresses[EmailAddressIndex.Email1] = new Participant(displayName, emailAddress.Address, emailAddress.RoutingType);
				}
			}, out personId);
		}

		public void RetrieveOrCreateTelUriContact(string telUriAddress, string imContactSipUriAddress, string imTelephoneNumber, out StoreObjectId contactId, out PersonId personId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "RetrieveOrCreateTelUriContact: TelUriAddress: {0}, ImContactSipUriAddress:{1}, ImTelephoneNumber: {2}", telUriAddress ?? "<Null>", imContactSipUriAddress ?? "<Null>", imTelephoneNumber ?? "<Null>");
			StoreObjectId objectId;
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, DefaultFolderType.QuickContacts))
			{
				StoreObjectId storeObjectId = this.FindImContactByProperty(folder, ContactSchema.TelUri, telUriAddress, out personId);
				if (storeObjectId != null)
				{
					contactId = storeObjectId;
					return;
				}
				this.CheckImContactLimit(folder);
				objectId = folder.Id.ObjectId;
			}
			contactId = this.CreateNewContact(objectId, delegate(IContact newContact)
			{
				newContact[ContactSchema.TelUri] = telUriAddress;
				newContact[ContactSchema.ImContactSipUriAddress] = imContactSipUriAddress;
				newContact[ContactSchema.OtherTelephone] = imTelephoneNumber;
			}, out personId);
		}

		public void ModifyGroup(StoreId groupId, string newDisplayName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId, string>((long)this.GetHashCode(), "ModifyGroup: GroupId: {0}, NewDisplayName: {1}", groupId, newDisplayName ?? "<Null>");
			using (IDistributionList distributionList = this.RetrieveGroupPdl(groupId, new PropertyDefinition[0]))
			{
				if (!this.AreSameItemClasses(distributionList.ClassName, "IPM.DistList.MOC.UserGroup"))
				{
					this.IndicateInvalidImGroupId();
				}
				StoreObjectId storeObjectId;
				if (this.PdlExists(newDisplayName, out storeObjectId))
				{
					this.IndicatePdlDisplayNameAlreadyExists();
				}
				distributionList.OpenAsReadWrite();
				distributionList.DisplayName = newDisplayName;
				ConflictResolutionResult conflictResolutionResult = distributionList.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus != SaveResult.Success)
				{
					ExTraceGlobals.UCSTracer.TraceWarning<StoreId, ConflictResolutionResult>((long)this.GetHashCode(), "ModifyGroup: Failed to modify the group {0}: {1}", groupId, conflictResolutionResult);
					throw new IrresolvableConflictException(conflictResolutionResult.PropertyConflicts);
				}
			}
		}

		public void RemoveContactFromImList(StoreId contactStoreId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "RemoveContactFromImList: ContactId: {0}", contactStoreId);
			StoreObjectId contactId = StoreId.GetStoreObjectId(contactStoreId);
			using (this.RetrieveContact(contactId))
			{
				this.ProcessPdlsContainingContact(contactId, delegate(IList<IDistributionList> parentPdls)
				{
					foreach (IDistributionList distributionList in parentPdls)
					{
						this.RemoveContactFromPdl(contactId, distributionList);
					}
				});
			}
			this.session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				contactStoreId
			});
		}

		public void RemoveImGroup(StoreId groupStoreId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "RemoveImGroup: GroupId: {0}", groupStoreId);
			StoreObjectId groupId = StoreId.GetStoreObjectId(groupStoreId);
			this.ProcessAllPdls(delegate(IList<IDistributionList> allPdls)
			{
				this.RemoveImGroup(groupId, allPdls);
			});
		}

		public void RemoveDistributionGroupFromImList(StoreId groupId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "RemoveDistributionGroupFromImList: GroupId: {0}.", groupId);
			using (IDistributionList distributionList = this.RetrieveGroupPdl(groupId, new PropertyDefinition[0]))
			{
				if (!this.AreSameItemClasses(distributionList.ClassName, "IPM.DistList.MOC.DG"))
				{
					this.IndicateInvalidImGroupId();
				}
			}
			this.session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				groupId
			});
		}

		protected virtual Persona[] GetPageOfPersonas(IQueryResult queryResult, IndexedPageView paging, PropertyListForViewRowDeterminer determiner, PropertyDefinition[] fetchList, out bool doNotRetrieveMorePages)
		{
			BasePageResult basePageResult = BasePagingType.ApplyPostQueryPaging(queryResult, paging);
			Persona[] array = basePageResult.View.ConvertPersonViewToPersonaObjects(fetchList, determiner, (MailboxSession)this.session);
			ExTraceGlobals.UCSTracer.TraceDebug<bool, int>((long)this.GetHashCode(), "GetPersonas: View indicates RetrievedLastItem={0}, Page returned {1} personas", basePageResult.View.RetrievedLastItem, array.Length);
			doNotRetrieveMorePages = (basePageResult.View.RetrievedLastItem || array.Length == 0);
			return array;
		}

		protected virtual ImGroup[] Convert(RawImGroup[] groups)
		{
			ImGroup[] array = new ImGroup[groups.Length];
			for (int i = 0; i < groups.Length; i++)
			{
				array[i] = ImGroup.LoadFromRawImGroup(groups[i], (MailboxSession)this.session);
			}
			return array;
		}

		protected virtual ItemId Convert(StoreId storeId)
		{
			return IdConverter.ConvertStoreItemIdToItemId(storeId, (StoreSession)this.session);
		}

		protected StoreObjectId CreatePDL(string displayName, string className, Action<IDistributionList> additionalPropertiesSetter)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string, bool>((long)this.GetHashCode(), "CreatePDL: DisplayName: {0}, ClassName: {1}, AdditionalPropertiesSetter-Null: {2}", displayName ?? "<Null>", className, additionalPropertiesSetter == null);
			this.CheckImGroupLimit();
			StoreObjectId objectId;
			using (IDistributionList distributionList = this.xsoFactory.CreateDistributionList(this.session, this.session.GetDefaultFolderId(DefaultFolderType.ImContactList)))
			{
				distributionList.DisplayName = displayName;
				distributionList.ClassName = className;
				if (additionalPropertiesSetter != null)
				{
					additionalPropertiesSetter(distributionList);
				}
				distributionList.Save(SaveMode.NoConflictResolution);
				distributionList.Load(UnifiedContactStoreUtilities.DistributionListId);
				ExTraceGlobals.UCSTracer.TraceDebug<string, string, VersionedId>((long)this.GetHashCode(), "CreatePDL: DisplayName: {0}, ClassName: {1}, Id: {2}", displayName ?? "<Null>", className, distributionList.Id);
				objectId = distributionList.Id.ObjectId;
			}
			return objectId;
		}

		protected bool PdlExists(string displayName, out StoreObjectId existingPdlId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "PdlExists: DisplayName: {0}.", displayName ?? "<Null>");
			existingPdlId = this.FindPdlBy(StoreObjectSchema.DisplayName, displayName, true, null);
			return existingPdlId != null;
		}

		protected StoreObjectId FindSystemPdl(string itemClass)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "FindSystemPdl: ItemClass: {0}.", itemClass);
			return this.FindPdlBy(StoreObjectSchema.ItemClass, itemClass, false, null);
		}

		private bool DgPdlExists(string smtpAddress, out StoreObjectId existingPdlId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "DgPdlExists: SmtpAddress: {0}.", smtpAddress ?? "<Null>");
			existingPdlId = this.FindPdlBy(DistributionListSchema.Email1EmailAddress, smtpAddress, true, null);
			return existingPdlId != null;
		}

		private void RemoveImGroup(StoreObjectId groupId, IList<IDistributionList> allPdls)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, int>((long)this.GetHashCode(), "RemoveImGroup: GroupId: {0}: {1} PDLs found", groupId, allPdls.Count);
			IDistributionList currentPdl;
			IDistributionList taggedPdl;
			this.FindCurrentAndTaggedPdl(allPdls, groupId, out currentPdl, out taggedPdl);
			if (currentPdl == null)
			{
				this.IndicateInvalidImGroupId();
			}
			if (!this.AreSameItemClasses(currentPdl.ClassName, "IPM.DistList.MOC.UserGroup"))
			{
				this.IndicateInvalidImGroupId();
			}
			HashSet<StoreObjectId> contactIdsToDelete = new HashSet<StoreObjectId>(currentPdl.Count);
			this.ForEachContactDistributionListMember(currentPdl, delegate(IDistributionListMember member, StoreParticipantOrigin storeParticipantOrigin)
			{
				StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
				bool flag = this.ShouldDeleteMemberContact(originItemId, allPdls, currentPdl, taggedPdl);
				if (flag)
				{
					if (!contactIdsToDelete.TryAdd(originItemId))
					{
						ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "RemoveImGroup: Already marked for delete. ContactId {0}.", originItemId);
					}
					else
					{
						ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "RemoveImGroup: Will permanently delete ContactId {0}.", originItemId);
					}
				}
				return true;
			});
			if (contactIdsToDelete.Count > 0)
			{
				if (taggedPdl != null)
				{
					this.RemoveContactsFromPdl(contactIdsToDelete, taggedPdl);
				}
				ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, int>((long)this.GetHashCode(), "RemoveImGroup: GroupId: {0}: Permanently deleting {1} contacts.", groupId, contactIdsToDelete.Count);
				StoreObjectId[] ids = contactIdsToDelete.ToArray();
				AggregateOperationResult arg = this.session.Delete(DeleteItemFlags.HardDelete, ids);
				ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, int, AggregateOperationResult>((long)this.GetHashCode(), "RemoveImGroup: GroupId: {0}: Permanently deleting {1} contacts - Result: {2}.", groupId, contactIdsToDelete.Count, arg);
			}
			this.session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
			{
				groupId
			});
		}

		private bool ShouldDeleteMemberContact(StoreObjectId contactId, IList<IDistributionList> allPdls, IDistributionList currentPdl, IDistributionList taggedPdl)
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "ShouldDeleteMemberContact: ContactId: {0}, All PDLs count: {1}, CurrentPDL: {2}, TaggedPDL: {3}", new object[]
			{
				contactId,
				allPdls.Count,
				currentPdl.DisplayName ?? "<<NULL>>",
				taggedPdl.DisplayName ?? "<<NULL>>"
			});
			IList<IDistributionList> parentPdls = new List<IDistributionList>();
			using (IEnumerator<IDistributionList> enumerator = allPdls.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IDistributionList otherPdl = enumerator.Current;
					this.ForEachContactDistributionListMember(otherPdl, delegate(IDistributionListMember otherPdlMember, StoreParticipantOrigin otherPdlMemberOrigin)
					{
						if (otherPdlMemberOrigin.OriginItemId.Equals(contactId))
						{
							parentPdls.Add(otherPdl);
							return false;
						}
						return true;
					});
				}
			}
			if (!parentPdls.Contains(currentPdl))
			{
				throw new InvalidOperationException("Either AllPdls does not contain currentPDL or the currentPDL does not contain the given ContactId");
			}
			bool result;
			this.GetPdlsFromWhichContactNeedsToBeRemoved(contactId, parentPdls, currentPdl, taggedPdl, out result);
			return result;
		}

		private bool AreSameItemClasses(string itemClass, string unifiedContactStoreItemClass)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string>((long)this.GetHashCode(), "AreSameItemClasses: ItemClass: {0}, UcsItemClass: {1}.", itemClass, unifiedContactStoreItemClass);
			return string.Equals(unifiedContactStoreItemClass, itemClass, StringComparison.OrdinalIgnoreCase);
		}

		private void AddContactToPdl(StoreObjectId contactId, string displayName, IDistributionList distributionList)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, string, string>((long)this.GetHashCode(), "AddContactToPdl: ContactId:{0}, DisplayName: {1}, DL:{2}", contactId, displayName ?? "<Null>", distributionList.DisplayName ?? "<Null>");
			if (this.IsMemberOfPdl(contactId, distributionList))
			{
				return;
			}
			Participant imParticipant = this.GetImParticipant(contactId, displayName);
			distributionList.OpenAsReadWrite();
			distributionList.Add(imParticipant);
			ConflictResolutionResult conflictResolutionResult = distributionList.Save(SaveMode.NoConflictResolution);
			if (conflictResolutionResult.SaveStatus != SaveResult.Success)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<Participant, string, ConflictResolutionResult>((long)this.GetHashCode(), "AddContactToPdl: Failed to add contact {0} to group {1}: {2}", imParticipant, distributionList.DisplayName ?? "<Null>", conflictResolutionResult);
				throw new IrresolvableConflictException(conflictResolutionResult.PropertyConflicts);
			}
		}

		private bool IsMemberOfPdl(StoreObjectId contactId, IDistributionList distributionList)
		{
			bool alreadyMember = false;
			this.ForEachContactDistributionListMember(distributionList, delegate(IDistributionListMember member, StoreParticipantOrigin storeParticipantOrigin)
			{
				if (object.Equals(storeParticipantOrigin.OriginItemId, contactId))
				{
					ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, StoreObjectId, string>((long)this.GetHashCode(), "IsMemberOfPdl: ContactId:{0} is already part of DL {1}:{2}", contactId, distributionList.Id.ObjectId, distributionList.DisplayName ?? "<Null>");
					alreadyMember = true;
					return false;
				}
				return true;
			});
			return alreadyMember;
		}

		private RawImGroup LoadImGroupFrom(StoreObjectId pdlId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "LoadImGroupFrom: PdlId:{0}", pdlId);
			return this.LoadImGroupFrom(pdlId, UnifiedContactStoreUtilities.DistributionListBasicData, null, null);
		}

		private RawImGroup LoadImGroupFrom(StoreObjectId pdlId, PropertyDefinition[] propertiesToLoad, ExtendedPropertyUri[] extendedPropertyUris, PropertyDefinition[] extendedPropertyDefinitions)
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "LoadImGroupFrom: PdlId:{0}, PropertiesToLoad:{1}, ExtendedPropertyUris:{2}, ExtendedPropertyDefinitions:{3}", new object[]
			{
				pdlId,
				(propertiesToLoad == null) ? 0 : propertiesToLoad.Length,
				(extendedPropertyUris == null) ? 0 : extendedPropertyUris.Length,
				(extendedPropertyDefinitions == null) ? 0 : extendedPropertyDefinitions.Length
			});
			RawImGroup result;
			using (IDistributionList distributionList = this.RetrieveGroupPdl(pdlId, propertiesToLoad))
			{
				result = this.LoadImGroupFrom(distributionList, extendedPropertyUris, extendedPropertyDefinitions);
			}
			return result;
		}

		private RawImGroup LoadImGroupFrom(IDistributionList distributionList, ExtendedPropertyUri[] extendedPropertyUris, PropertyDefinition[] extendedPropertyDefinitions)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "LoadImGroupFrom: DL:{0}, ExtendedPropertyUris:{1}, ExtendedPropertyDefinitions:{2}", distributionList.DisplayName ?? "<Null>", (extendedPropertyUris == null) ? 0 : extendedPropertyUris.Length, (extendedPropertyDefinitions == null) ? 0 : extendedPropertyDefinitions.Length);
			RawImGroup rawImGroup = new RawImGroup
			{
				DisplayName = distributionList.DisplayName,
				GroupType = distributionList.ClassName,
				ExchangeStoreId = distributionList.Id.ObjectId
			};
			object obj = distributionList.TryGetProperty(DistributionListSchema.Email1EmailAddress);
			rawImGroup.SmtpAddress = (obj as string);
			if (extendedPropertyUris != null)
			{
				List<ExtendedPropertyType> list = new List<ExtendedPropertyType>(extendedPropertyUris.Length);
				for (int i = 0; i < extendedPropertyUris.Length; i++)
				{
					object obj2 = distributionList.TryGetProperty(extendedPropertyDefinitions[i]);
					if (obj2 != null && !(obj2 is PropertyError))
					{
						list.Add(ExtendedPropertyProperty.GetExtendedPropertyForValues(extendedPropertyUris[i], extendedPropertyDefinitions[i], obj2));
					}
				}
				if (list.Count != 0)
				{
					rawImGroup.ExtendedProperties = list.ToArray();
				}
			}
			HashSet<StoreObjectId> members = new HashSet<StoreObjectId>(distributionList.Count);
			this.ForEachContactDistributionListMember(distributionList, delegate(IDistributionListMember member, StoreParticipantOrigin storeParticipantOrigin)
			{
				StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
				if (!members.TryAdd(originItemId))
				{
					ExTraceGlobals.UCSTracer.TraceDebug<string, StoreObjectId>((long)this.GetHashCode(), "LoadImGroupFrom: DL:{0}, Found more than one entry for memberId: {1}", distributionList.DisplayName ?? "<Null>", originItemId);
				}
				return true;
			});
			rawImGroup.MemberCorrelationKey = members.ToArray();
			ExTraceGlobals.UCSTracer.TraceDebug<string, int>((long)this.GetHashCode(), "LoadImGroupFrom: DL:{0}, Members: {1}", distributionList.DisplayName ?? "<Null>", rawImGroup.MemberCorrelationKey.Length);
			return rawImGroup;
		}

		private bool IsImGroupMember(IDistributionListMember distributionListMember, out StoreObjectId groupMemberId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "IsImGroupMember: DLMember:{0}", (distributionListMember == null) ? "<Null>" : ((distributionListMember.Participant == null) ? "<Participant:Null>" : distributionListMember.Participant.ToString()));
			groupMemberId = null;
			if (distributionListMember == null)
			{
				ExTraceGlobals.UCSTracer.TraceWarning((long)this.GetHashCode(), "IsImGroupMember: Empty DL member.");
				return false;
			}
			if (distributionListMember.Participant == null)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<IDistributionListMember>((long)this.GetHashCode(), "IsImGroupMember: Empty Participant on DL member {0}.", distributionListMember);
				return false;
			}
			StoreParticipantOrigin storeParticipantOrigin = distributionListMember.Participant.Origin as StoreParticipantOrigin;
			if (storeParticipantOrigin == null)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<Participant>((long)this.GetHashCode(), "IsImGroupMember: The PDL member {0} is not a contact in store.", distributionListMember.Participant);
				return false;
			}
			if (storeParticipantOrigin.OriginItemId == null)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<Participant>((long)this.GetHashCode(), "IsImGroupMember: The PDL member {0} which is a contact in store does not have an OriginItemId.", distributionListMember.Participant);
				return false;
			}
			if (storeParticipantOrigin.OriginItemId.ObjectType != StoreObjectType.Contact)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<Participant, StoreObjectType>((long)this.GetHashCode(), "IsImGroupMember: The PDL member {0} is not a 'contact' type: {1}", distributionListMember.Participant, storeParticipantOrigin.OriginItemId.ObjectType);
				return false;
			}
			groupMemberId = storeParticipantOrigin.OriginItemId;
			return true;
		}

		private StoreObjectId GetUserDistributionGroupPdlIdWith(string smtpAddress, string displayName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetUserDistributionGroupPdlIdWith: SmtpAddress:{0}, DisplayName: {1}", smtpAddress ?? "<Null>", displayName ?? "<Null>");
			StoreObjectId result;
			if (this.DgPdlExists(smtpAddress, out result))
			{
				return result;
			}
			return this.CreateDgPDL(smtpAddress, displayName);
		}

		private StoreObjectId CreateDgPDL(string smtpAddress, string displayName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string>((long)this.GetHashCode(), "CreateDgPDL: SmtpAddress:{0}, DisplayName: {1}", smtpAddress ?? "<Null>", displayName ?? "<Null>");
			StoreObjectId storeObjectId = this.CreatePDL(displayName, "IPM.DistList.MOC.DG", delegate(IDistributionList contactsDl)
			{
				contactsDl[DistributionListSchema.Email1EmailAddress] = smtpAddress;
				Participant participant = new Participant(displayName, smtpAddress, "SMTP", new DirectoryParticipantOrigin(), new KeyValuePair<PropertyDefinition, object>[0]);
				contactsDl.Add(participant);
			});
			ExTraceGlobals.UCSTracer.TraceDebug<string, string, StoreObjectId>((long)this.GetHashCode(), "CreateDgPDL: SmtpAddress:{0}, DisplayName: {1}, GroupId: {2}", smtpAddress ?? "<Null>", displayName ?? "<Null>", storeObjectId);
			return storeObjectId;
		}

		private StoreObjectId GetUserPdlIdWith(string displayName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "GetUserPdlIdWith: DisplayName: {0}", displayName ?? "<Null>");
			StoreObjectId result;
			if (this.PdlExists(displayName, out result))
			{
				return result;
			}
			return this.CreatePDL(displayName, "IPM.DistList.MOC.UserGroup", null);
		}

		private StoreObjectId FindPdlBy(PropertyDefinition propertyDefinition, string value, bool caseSensitive, string itemClass = null)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, string, bool>((long)this.GetHashCode(), "FindPdlBy: PropertyDefinition: {0}, Value: {1}, CaseSensitive: {2}", propertyDefinition.Name, value ?? "<Null>", caseSensitive);
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, DefaultFolderType.ImContactList))
			{
				SortBy[] sortColumns = new SortBy[]
				{
					new SortBy(propertyDefinition, SortOrder.Ascending),
					UnifiedContactStoreUtilities.FindFirstItem
				};
				using (IQueryResult queryResult = folder.IItemQuery(ItemQueryType.None, null, sortColumns, UnifiedContactStoreUtilities.DistributionListId))
				{
					QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, propertyDefinition, value);
					SeekToConditionFlags flags = SeekToConditionFlags.None;
					if (!string.IsNullOrEmpty(itemClass))
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, itemClass),
							queryFilter
						});
						flags = SeekToConditionFlags.AllowExtendedFilters;
					}
					bool flag = queryResult.SeekToCondition(SeekReference.OriginBeginning, queryFilter, flags);
					if (!flag && !caseSensitive)
					{
						queryFilter = new TextFilter(propertyDefinition, value, MatchOptions.FullString, MatchFlags.IgnoreCase);
						if (!string.IsNullOrEmpty(itemClass))
						{
							queryFilter = new AndFilter(new QueryFilter[]
							{
								new TextFilter(StoreObjectSchema.ItemClass, itemClass, MatchOptions.FullString, MatchFlags.IgnoreCase),
								queryFilter
							});
						}
						flag = queryResult.SeekToCondition(SeekReference.OriginBeginning, queryFilter, SeekToConditionFlags.AllowExtendedFilters);
					}
					if (flag)
					{
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
						if (propertyBags.Length > 0)
						{
							StoreObjectId storeObjectId = this.StoreObjectIdFrom(propertyBags[0]);
							ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "FindPdlBy: PropertyDefinition: {0}, Value: {1}, CaseSensitive: {2} - PdlId:{3}", new object[]
							{
								propertyDefinition.Name,
								value ?? "<Null>",
								caseSensitive,
								storeObjectId
							});
							return storeObjectId;
						}
					}
				}
			}
			ExTraceGlobals.UCSTracer.TraceDebug<string, string, bool>((long)this.GetHashCode(), "FindPdlBy: PropertyDefinition: {0}, Value: {1}, CaseSensitive: {2} - NotFound", propertyDefinition.Name, value ?? "<Null>", caseSensitive);
			return null;
		}

		private StoreObjectId CreateNewContact(StoreObjectId quickContactsFolderId, Action<IContact> propertySetter, out PersonId personId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<bool>((long)this.GetHashCode(), "CreateNewContact: propertySetter_Null:{0}", propertySetter == null);
			StoreObjectId objectId;
			using (IContact contact = this.xsoFactory.CreateContact(this.session, quickContactsFolderId))
			{
				propertySetter(contact);
				contact[ContactSchema.PartnerNetworkId] = WellKnownNetworkNames.QuickContacts;
				Item item = contact as Item;
				if (item != null)
				{
					AutomaticLink.ForceAutomaticLinkingForItem(item);
				}
				contact.Save(SaveMode.NoConflictResolution);
				contact.Load(UnifiedContactStoreUtilities.ContactIdAndPersonId);
				personId = (PersonId)contact[ContactSchema.PersonId];
				objectId = contact.Id.ObjectId;
			}
			return objectId;
		}

		private void DeleteContact(StoreId contactId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "DeleteContact: ContactId: {0}", contactId);
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, this.session.GetDefaultFolderId(DefaultFolderType.QuickContacts)))
			{
				folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
				{
					contactId
				});
			}
		}

		private StoreObjectId FindImContactByProperty(IFolder quickContactsFolder, PropertyDefinition propertyDefinition, object propertyValue, out PersonId personId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<bool, string, object>((long)this.GetHashCode(), "FindImContactByProperty: QuickContactsFolder_Null: {0}, Property: {1}, Value: {2}", quickContactsFolder == null, propertyDefinition.Name, propertyValue);
			SortBy[] sortColumns = new SortBy[]
			{
				new SortBy(propertyDefinition, SortOrder.Ascending),
				UnifiedContactStoreUtilities.FindFirstItem
			};
			using (IQueryResult queryResult = quickContactsFolder.IItemQuery(ItemQueryType.None, null, sortColumns, UnifiedContactStoreUtilities.ContactIdAndPersonId))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, propertyDefinition, propertyValue)))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					if (propertyBags.Length > 0)
					{
						StoreObjectId objectId = ((VersionedId)propertyBags[0].TryGetProperty(ItemSchema.Id)).ObjectId;
						personId = (PersonId)propertyBags[0].TryGetProperty(ContactSchema.PersonId);
						ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "FindImContactByProperty: ContactId: {0}", objectId);
						return objectId;
					}
				}
			}
			ExTraceGlobals.UCSTracer.TraceDebug<string, object>((long)this.GetHashCode(), "FindImContactByProperty: Property: {0}, Value: {1} - NotFound", propertyDefinition.Name, propertyValue);
			personId = null;
			return null;
		}

		private void CheckImContactLimit(IFolder quickContactsFolder)
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "CheckImContactLimit.");
			int itemCount = quickContactsFolder.ItemCount;
			int maxContactsAllowed = this.MaxContactsAllowed;
			ExTraceGlobals.UCSTracer.TraceDebug<int, int>((long)this.GetHashCode(), "CheckImContactLimit: CurrentContactCount: {0}, MaxImContacts: {1}", itemCount, maxContactsAllowed);
			if (itemCount >= maxContactsAllowed)
			{
				throw new ImContactLimitReachedException();
			}
		}

		private void CheckImGroupLimit()
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "CheckImGroupLimit.");
			int itemCount;
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, DefaultFolderType.ImContactList))
			{
				itemCount = folder.ItemCount;
			}
			int maxPdlsAllowed = this.MaxPdlsAllowed;
			ExTraceGlobals.UCSTracer.TraceDebug<int, int>((long)this.GetHashCode(), "CheckImGroupLimit: CurrentGroupCount: {0}, MaxGroupCount: {1}", itemCount, maxPdlsAllowed);
			if (itemCount >= maxPdlsAllowed)
			{
				throw new ImGroupLimitReachedException();
			}
		}

		private IDistributionList RetrieveGroupPdl(StoreId groupPdlId, params PropertyDefinition[] propertiesToLoad)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId, int>((long)this.GetHashCode(), "RetrieveGroupPdl: PdlId: {0}, PropertiesToLoad: {1}", groupPdlId, (propertiesToLoad == null) ? 0 : propertiesToLoad.Length);
			IDistributionList result;
			if (!this.TryRetrieveGroupPdl(groupPdlId, propertiesToLoad, out result, true))
			{
				this.IndicateInvalidImGroupId();
			}
			return result;
		}

		private bool TryRetrieveGroupPdl(StoreId groupPdlId, PropertyDefinition[] propertiesToLoad, out IDistributionList distributionList, bool checkUcsPdlClassName = true)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreId, int>((long)this.GetHashCode(), "TryRetrieveGroupPdl: PdlId: {0}, PropertiesToLoad: {1}", groupPdlId, (propertiesToLoad == null) ? 0 : propertiesToLoad.Length);
			distributionList = null;
			IDistributionList distributionList2;
			try
			{
				distributionList2 = this.xsoFactory.BindToDistributionList(this.session, groupPdlId, propertiesToLoad);
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<StoreId, int, string>((long)this.GetHashCode(), "TryRetrieveGroupPdl: PdlId: {0}, PropertiesToLoad: {1}: Exception: {2}", groupPdlId, (propertiesToLoad == null) ? 0 : propertiesToLoad.Length, ex.ToString());
				return false;
			}
			catch (WrongObjectTypeException ex2)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<StoreId, int, string>((long)this.GetHashCode(), "TryRetrieveGroupPdl: PdlId: {0}, PropertiesToLoad: {1}: Exception: {2}", groupPdlId, (propertiesToLoad == null) ? 0 : propertiesToLoad.Length, ex2.ToString());
				return false;
			}
			StoreObjectId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.ImContactList);
			if (!distributionList2.ParentId.Equals(defaultFolderId) || (checkUcsPdlClassName && !this.IsUcsPdlClassName(distributionList2.ClassName)))
			{
				distributionList2.Dispose();
				return false;
			}
			distributionList = distributionList2;
			return true;
		}

		private StoreObjectId StoreObjectIdFrom(IStorePropertyBag pdlIdPropertyBag)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<bool>((long)this.GetHashCode(), "StoreObjectIdFrom: pdlIdPropertyBag_Null:{0}", pdlIdPropertyBag == null);
			return ((VersionedId)pdlIdPropertyBag.TryGetProperty(ItemSchema.Id)).ObjectId;
		}

		private void ForEachPropertyBagIn(int maxLimit, IQueryResult queryResult, Action<IStorePropertyBag> actionToPerform)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<int, bool, bool>((long)this.GetHashCode(), "ForEachPropertyBagIn: MaxLimit: {0}, QueryResult_Null:{1}, ActionToPerform_Null:{2}", maxLimit, queryResult == null, actionToPerform == null);
			int num = 0;
			for (;;)
			{
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
				ExTraceGlobals.UCSTracer.TraceDebug<int>((long)this.GetHashCode(), "ForEachPropertyBagIn: Found rows: {0}", propertyBags.Length);
				foreach (IStorePropertyBag obj in propertyBags)
				{
					actionToPerform(obj);
					num++;
					if (num >= 2 * maxLimit)
					{
						goto Block_1;
					}
				}
				if (propertyBags.Length <= 0)
				{
					return;
				}
			}
			Block_1:
			ExTraceGlobals.UCSTracer.TraceError((long)this.GetHashCode(), "ForEachPropertyBagIn: Found 2 times more than max supported entries");
		}

		private void EnsureSystemPdlsCreated()
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "EnsureSystemPdlsCreated.");
			foreach (UnifiedContactStoreUtilities.SystemPdl systemPdl in UnifiedContactStoreUtilities.SystemPdls)
			{
				this.GetSystemPdlId(systemPdl.DisplayName, systemPdl.ClassName);
			}
		}

		private void IndicateInvalidImContactId()
		{
			ExTraceGlobals.UCSTracer.TraceWarning((long)this.GetHashCode(), "IndicateInvalidImContactId.");
			throw new InvalidImContactIdException();
		}

		private void IndicateInvalidImGroupId()
		{
			ExTraceGlobals.UCSTracer.TraceWarning((long)this.GetHashCode(), "IndicateInvalidImGroupId.");
			throw new InvalidImGroupIdException();
		}

		private void IndicatePdlDisplayNameAlreadyExists()
		{
			ExTraceGlobals.UCSTracer.TraceWarning((long)this.GetHashCode(), "IndicatePdlDisplayNameAlreadyExists.");
			throw new ImGroupDisplayNameAlreadyExistsException();
		}

		private void IndicateUnableToRemoveImContactFromGroup()
		{
			ExTraceGlobals.UCSTracer.TraceWarning((long)this.GetHashCode(), "IndicateUnableToRemoveImContactFromGroup.");
			throw new UnableToRemoveImContactFromGroupException();
		}

		private IContact RetrieveContact(StoreObjectId contactId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "RetrieveContact: ContactId: {0}", contactId);
			IContact result;
			if (!this.TryRetrieveContact(contactId, out result))
			{
				this.IndicateInvalidImContactId();
			}
			return result;
		}

		private bool TryRetrieveContact(StoreObjectId contactId, out IContact contact)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "TryRetrieveContact: ContactId: {0}.", contactId);
			contact = null;
			IContact contact2;
			try
			{
				PropertyDefinition[] retrieveContactDataProperties = UnifiedContactStoreUtilities.RetrieveContactDataProperties;
				contact2 = this.xsoFactory.BindToContact(this.session, contactId, retrieveContactDataProperties);
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<StoreObjectId, string>((long)this.GetHashCode(), "TryRetrieveContact: ContactId: {0}, Exception: {1}", contactId, ex.ToString());
				return false;
			}
			catch (WrongObjectTypeException ex2)
			{
				ExTraceGlobals.UCSTracer.TraceWarning<StoreObjectId, string>((long)this.GetHashCode(), "TryRetrieveContact: ContactId: {0}, Exception: {1}", contactId, ex2.ToString());
				return false;
			}
			StoreObjectId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.QuickContacts);
			if (!defaultFolderId.Equals(contact2.ParentId))
			{
				ExTraceGlobals.UCSTracer.TraceWarning<StoreObjectId, StoreObjectId, StoreObjectId>((long)this.GetHashCode(), "TryRetrieveContact: ContactId: {0}, Does not belong to the QuickContacts folder, ParentFolderId: {1}, QuickContactsFolderId: {2}", contactId, contact2.ParentId, defaultFolderId);
				contact2.Dispose();
				return false;
			}
			contact = contact2;
			return true;
		}

		private bool IsUcsPdlClassName(string pdlClassName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "IsUcsPdlClassName: pdlClassName:{0}", pdlClassName ?? "<Null>");
			return this.AreSameItemClasses(pdlClassName, "IPM.DistList.MOC.Favorites") || this.AreSameItemClasses(pdlClassName, "IPM.DistList.MOC.OtherContacts") || this.AreSameItemClasses(pdlClassName, "IPM.DistList.MOC.Tagged") || this.AreSameItemClasses(pdlClassName, "IPM.DistList.MOC.UserGroup") || this.AreSameItemClasses(pdlClassName, "IPM.DistList.MOC.DG");
		}

		private void ForEachContactDistributionListMember(IDistributionList distributionList, Func<IDistributionListMember, StoreParticipantOrigin, bool> doActionAndIndicateIfContinueProcessing)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "ForEachContactDistributionListMember: DL:{0}, Action_Null:{1}", distributionList.DisplayName ?? "<Null>", doActionAndIndicateIfContinueProcessing == null);
			IEnumerator<IDistributionListMember> enumerator = distributionList.IGetEnumerator();
			while (enumerator.MoveNext())
			{
				IDistributionListMember distributionListMember = enumerator.Current;
				StoreObjectId storeObjectId;
				if (this.IsImGroupMember(distributionListMember, out storeObjectId))
				{
					StoreParticipantOrigin arg = (StoreParticipantOrigin)distributionListMember.Participant.Origin;
					ExTraceGlobals.UCSTracer.TraceDebug<string, Participant>((long)this.GetHashCode(), "ForEachContactDistributionListMember: DL:{0}, Processing Member: {1}", distributionList.DisplayName ?? "<Null>", distributionListMember.Participant);
					if (!doActionAndIndicateIfContinueProcessing(distributionListMember, arg))
					{
						ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "ForEachContactDistributionListMember: DL:{0}, Terminating processing in between.", distributionList.DisplayName ?? "<Null>");
						return;
					}
				}
			}
		}

		private Participant GetImParticipant(StoreObjectId contactId, string displayName)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, string>((long)this.GetHashCode(), "GetImParticipant: ContactId: {0}, DisplayName: {1}", contactId, displayName ?? "<Null>");
			Participant participant = new Participant(displayName ?? string.Empty, string.Empty, null, new StoreParticipantOrigin(contactId), new KeyValuePair<PropertyDefinition, object>[0]);
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, Participant>((long)this.GetHashCode(), "GetImParticipant: ContactId: {0}, Participant: {1}", contactId, participant);
			return participant;
		}

		private void GetPdlsFromWhichContactNeedsToBeRemoved(StoreObjectId contactId, IList<IDistributionList> parentPdls, StoreObjectId groupId, out IDistributionList currentPdl, out IDistributionList taggedPdl, out bool deleteContact)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, int, StoreObjectId>((long)this.GetHashCode(), "GetPdlsFromWhichContactNeedsToBeRemoved: ContactId: {0}, Parent PDLs count: {1}, Current Group Id: {2}", contactId, parentPdls.Count, groupId);
			this.FindCurrentAndTaggedPdl(parentPdls, groupId, out currentPdl, out taggedPdl);
			this.GetPdlsFromWhichContactNeedsToBeRemoved(contactId, parentPdls, currentPdl, taggedPdl, out deleteContact);
		}

		private void FindCurrentAndTaggedPdl(IList<IDistributionList> pdls, StoreObjectId pdlId, out IDistributionList currentPdl, out IDistributionList taggedPdl)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<int, StoreObjectId>((long)this.GetHashCode(), "FindCurrentAndTaggedPdl: PDLs count: {0}, PDLId: {1}", pdls.Count, pdlId);
			IDistributionList distributionList;
			taggedPdl = (distributionList = null);
			currentPdl = distributionList;
			foreach (IDistributionList distributionList2 in pdls)
			{
				if (this.AreSameItemClasses(distributionList2.ClassName, "IPM.DistList.MOC.Tagged"))
				{
					if (taggedPdl != null)
					{
						throw new InvalidOperationException("Found multiple Tagged PDLs.");
					}
					taggedPdl = distributionList2;
				}
				if (pdlId.Equals(distributionList2.StoreObjectId))
				{
					if (currentPdl != null)
					{
						throw new InvalidOperationException("Found multiple PDLs matching ID: " + pdlId);
					}
					currentPdl = distributionList2;
				}
			}
		}

		private void GetPdlsFromWhichContactNeedsToBeRemoved(StoreObjectId contactId, IList<IDistributionList> parentPdls, IDistributionList currentPdl, IDistributionList taggedPdl, out bool deleteContact)
		{
			ExTraceGlobals.UCSTracer.TraceDebug((long)this.GetHashCode(), "GetPdlsFromWhichContactNeedsToBeRemoved: ContactId: {0}, Parent PDLs count: {1}, CurrentPDL: {2}, TaggedPDL: {3}", new object[]
			{
				contactId,
				parentPdls.Count,
				(currentPdl == null) ? "<<NULL>>" : (currentPdl.DisplayName ?? "<<NULL_DisplayName>>"),
				(taggedPdl == null) ? "<<NULL>>" : (taggedPdl.DisplayName ?? "<<NULL_DisplayName>>")
			});
			if (parentPdls.Count == 0)
			{
				deleteContact = true;
				return;
			}
			if (parentPdls.Count == 1 || parentPdls.Count == 2)
			{
				bool flag = true;
				foreach (IDistributionList distributionList in parentPdls)
				{
					flag = (flag && (distributionList == currentPdl || distributionList == taggedPdl));
				}
				deleteContact = flag;
				return;
			}
			deleteContact = false;
		}

		private void RemoveContactsFromPdl(HashSet<StoreObjectId> contactIds, IDistributionList distributionList)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<int, string>((long)this.GetHashCode(), "RemoveContactsFromPdl: Removing {0} contact from DL: {1}", contactIds.Count, distributionList.DisplayName ?? "<Null>");
			List<IDistributionListMember> membersToRemove = new List<IDistributionListMember>(contactIds.Count);
			this.ForEachContactDistributionListMember(distributionList, delegate(IDistributionListMember member, StoreParticipantOrigin storeOrigin)
			{
				StoreObjectId originItemId = storeOrigin.OriginItemId;
				if (contactIds.Contains(originItemId))
				{
					membersToRemove.Add(member);
				}
				return true;
			});
			if (membersToRemove.Count > 0)
			{
				distributionList.OpenAsReadWrite();
				foreach (IDistributionListMember item in membersToRemove)
				{
					distributionList.Remove(item);
				}
				ConflictResolutionResult conflictResolutionResult = distributionList.Save(SaveMode.NoConflictResolution);
				if (conflictResolutionResult.SaveStatus != SaveResult.Success)
				{
					ExTraceGlobals.UCSTracer.TraceWarning<ConflictResolutionResult>((long)this.GetHashCode(), "RemoveContactFromPdl: Failed to save the remove operation: {0}", conflictResolutionResult);
					this.IndicateUnableToRemoveImContactFromGroup();
				}
			}
		}

		private void GetGroupsAndMembers(ExtendedPropertyUri[] extendedPropertyUris, out List<RawImGroup> imGroups, out HashSet<StoreObjectId> groupMemberStoreIds)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "GetGroupsAndMembers: {0} ExtendedProperties.", (extendedPropertyUris == null) ? "<<Null>>" : extendedPropertyUris.Length.ToString());
			this.EnsureSystemPdlsCreated();
			groupMemberStoreIds = new HashSet<StoreObjectId>(this.MaxContactsAllowed);
			imGroups = this.GetRawImGroupListAndPopulateGroupMemberIdsFromImContactListFolder(groupMemberStoreIds, extendedPropertyUris);
			ExTraceGlobals.UCSTracer.TraceDebug<int, int>((long)this.GetHashCode(), "GetGroupsAndMembers: A total of {0} groups found with {1} members in them.", imGroups.Count, groupMemberStoreIds.Count);
		}

		private PropertyDefinition[] Convert(ExtendedPropertyUri[] extendedPropertyUris)
		{
			if (extendedPropertyUris != null && extendedPropertyUris.Length > 0)
			{
				PropertyDefinition[] array = new PropertyDefinition[extendedPropertyUris.Length];
				for (int i = 0; i < extendedPropertyUris.Length; i++)
				{
					array[i] = extendedPropertyUris[i].ToPropertyDefinition();
				}
				return array;
			}
			return null;
		}

		private List<RawImGroup> GetRawImGroupListAndPopulateGroupMemberIdsFromImContactListFolder(HashSet<StoreObjectId> groupMemberStoreIds, ExtendedPropertyUri[] extendedPropertyUris)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<string>((long)this.GetHashCode(), "GetRawImGroupListAndPopulateGroupMemberIdsFromImContactListFolder: MemberIds: {0}", (groupMemberStoreIds == null) ? "<Null>" : groupMemberStoreIds.Count.ToString());
			PropertyDefinition[] extendedProperties = this.Convert(extendedPropertyUris);
			PropertyDefinition[] propertiesToLoad = UnifiedContactStoreUtilities.DistributionListBasicData;
			if (extendedProperties != null && extendedProperties.Length > 0)
			{
				propertiesToLoad = PropertyDefinitionCollection.Merge<PropertyDefinition>(propertiesToLoad, extendedProperties);
			}
			List<RawImGroup> imGroups = new List<RawImGroup>();
			this.ForEachImGroup(true, delegate(IStorePropertyBag pdlPropertyBag)
			{
				StoreObjectId pdlId = this.StoreObjectIdFrom(pdlPropertyBag);
				RawImGroup rawImGroup = this.LoadImGroupFrom(pdlId, propertiesToLoad, extendedPropertyUris, extendedProperties);
				foreach (StoreObjectId item in rawImGroup.MemberCorrelationKey)
				{
					groupMemberStoreIds.TryAdd(item);
				}
				imGroups.Add(rawImGroup);
			});
			ExTraceGlobals.UCSTracer.TraceDebug<int, int>((long)this.GetHashCode(), "GetRawImGroupListAndPopulateGroupMemberIdsFromImContactListFolder: MemberIds: {0}, Groups: {1}", groupMemberStoreIds.Count, imGroups.Count);
			return imGroups;
		}

		private HashSet<PersonId> GetPersonIdsForGroupMembersFrom(IFolder quickContactsFolder, HashSet<StoreObjectId> groupMemberStoreIds)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<bool, string>((long)this.GetHashCode(), "GetPersonIdsForGroupMembersFrom: QuickContactsFolder_Null: {0}, MemberIds: {1}", quickContactsFolder == null, (groupMemberStoreIds == null) ? "<Null>" : groupMemberStoreIds.Count.ToString());
			HashSet<PersonId> personIds = new HashSet<PersonId>(groupMemberStoreIds.Count);
			using (IQueryResult queryResult = quickContactsFolder.IItemQuery(ItemQueryType.None, UnifiedContactStoreUtilities.ContactQueryFilter, new SortBy[]
			{
				UnifiedContactStoreUtilities.FindFirstItem
			}, UnifiedContactStoreUtilities.ContactIdAndPersonId))
			{
				this.ForEachPropertyBagIn(this.MaxContactsAllowed, queryResult, delegate(IStorePropertyBag contactPropertyBag)
				{
					StoreObjectId storeObjectId = this.StoreObjectIdFrom(contactPropertyBag);
					if (!groupMemberStoreIds.Contains(storeObjectId))
					{
						ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "GetPersonIdsForGroupMembersFrom: Found contact in QuickContacts folder not part of any PDL: {0} - Skipping it.", storeObjectId);
						return;
					}
					personIds.TryAdd((PersonId)contactPropertyBag[ContactSchema.PersonId]);
				});
			}
			ExTraceGlobals.UCSTracer.TraceDebug<int, int>((long)this.GetHashCode(), "GetPersonIdsForGroupMembersFrom: Count of members from all PDLs: {0}, Count of contacts in QuickContacts folder part of any UCS PDL: {1}", groupMemberStoreIds.Count, personIds.Count);
			return personIds;
		}

		private void ProcessAllPdls(Action<IList<IDistributionList>> processAllPdls)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<bool>((long)this.GetHashCode(), "ProcessAllPdls: Processor_Null: {0}.", processAllPdls == null);
			this.ProcessMatchingPdls(processAllPdls, null);
		}

		private void ProcessPdlsContainingContact(StoreObjectId contactId, Action<IList<IDistributionList>> processor)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, bool>((long)this.GetHashCode(), "ProcessPdlsContainingContact: ContactId: {0}, Processor_Null: {1}.", contactId, processor == null);
			this.ProcessMatchingPdls(processor, delegate(IDistributionList distributionList)
			{
				bool toIncludeInResultSet = false;
				this.ForEachContactDistributionListMember(distributionList, delegate(IDistributionListMember member, StoreParticipantOrigin storeParticipantOrigin)
				{
					if (contactId.Equals(storeParticipantOrigin.OriginItemId))
					{
						ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, VersionedId, string>((long)this.GetHashCode(), "ProcessPdlsContainingContact: ContactId: {0}, Found parent: {1}/{2}.", contactId, distributionList.Id, distributionList.DisplayName ?? "<Null>");
						toIncludeInResultSet = true;
						return false;
					}
					return true;
				});
				return toIncludeInResultSet;
			});
		}

		private void ProcessMatchingPdls(Action<IList<IDistributionList>> processor, Func<IDistributionList, bool> toIncludeDistributionListInResultSet)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "ProcessMatchingPdls: Processor_Null: {0}, Filter_Null: {1}", processor == null, toIncludeDistributionListInResultSet == null);
			IList<IDistributionList> resultSet = new List<IDistributionList>();
			try
			{
				this.ForEachImGroup(false, delegate(IStorePropertyBag pdlPropertyBag)
				{
					StoreObjectId storeObjectId = this.StoreObjectIdFrom(pdlPropertyBag);
					IDistributionList distributionList2;
					if (!this.TryRetrieveGroupPdl(storeObjectId, UnifiedContactStoreUtilities.DistributionListBasicData, out distributionList2, true))
					{
						ExTraceGlobals.UCSTracer.TraceWarning<StoreObjectId>((long)this.GetHashCode(), "ProcessMatchingPdls: Found a non-UCS pdl: {0}. Skipping it.", storeObjectId);
						return;
					}
					try
					{
						if (toIncludeDistributionListInResultSet == null || toIncludeDistributionListInResultSet(distributionList2))
						{
							resultSet.Add(distributionList2);
						}
					}
					finally
					{
						if (!resultSet.Contains(distributionList2))
						{
							ExTraceGlobals.UCSTracer.TraceDebug<VersionedId, string>((long)this.GetHashCode(), "ProcessMatchingPdls: Skipping DL: {0}/{1}.", distributionList2.Id, distributionList2.DisplayName ?? "<Null>");
							distributionList2.Dispose();
							distributionList2 = null;
						}
					}
				});
				ExTraceGlobals.UCSTracer.TraceDebug<int>((long)this.GetHashCode(), "ProcessMatchingPdls: Processing {0} PDLs", resultSet.Count);
				processor(resultSet);
			}
			finally
			{
				foreach (IDistributionList distributionList in resultSet)
				{
					distributionList.Dispose();
				}
				resultSet.Clear();
			}
		}

		private void ForEachImGroup(bool fixCorruptedPdls, Action<IStorePropertyBag> actionOnEachImGroup)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<bool>((long)this.GetHashCode(), "ForEachImGroup: Action_Null: {0}", actionOnEachImGroup == null);
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, DefaultFolderType.ImContactList))
			{
				PropertyDefinition[] dataColumns = fixCorruptedPdls ? UnifiedContactStoreUtilities.DistributionListRecoveryProperties : UnifiedContactStoreUtilities.DistributionListIdAndItemClass;
				SortBy[] sortColumns = new SortBy[]
				{
					new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
					UnifiedContactStoreUtilities.FindFirstItem
				};
				using (IQueryResult queryResult = folder.IItemQuery(ItemQueryType.None, null, sortColumns, dataColumns))
				{
					Dictionary<string, VersionedId> systemPdlsFound = new Dictionary<string, VersionedId>(UnifiedContactStoreUtilities.SystemPdls.Length);
					int fixedCorruptedPdls = 0;
					int fixedSystemPdls = 0;
					int fixedUserPdlsItemClass = 0;
					int fixedUserPdlsCopyContacts = 0;
					this.ForEachPropertyBagIn(this.MaxPdlsAllowed, queryResult, delegate(IStorePropertyBag pdlPropertyBag)
					{
						string itemClass = (string)pdlPropertyBag[StoreObjectSchema.ItemClass];
						VersionedId versionedId = (VersionedId)pdlPropertyBag[ItemSchema.Id];
						if (StringComparer.OrdinalIgnoreCase.Equals(itemClass, "IPM.DistList"))
						{
							if (fixCorruptedPdls)
							{
								ExTraceGlobals.UCSTracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "ForEachImGroup: Fixing corrupt PDL: {0}", versionedId);
								bool flag;
								bool flag2;
								bool flag3;
								this.FixCorruptedPdl(pdlPropertyBag, out flag, out flag2, out flag3);
								fixedCorruptedPdls++;
								if (flag2)
								{
									fixedSystemPdls++;
								}
								else if (flag3)
								{
									fixedUserPdlsItemClass++;
								}
								else
								{
									fixedUserPdlsCopyContacts++;
								}
								if (!flag)
								{
									goto IL_12C;
								}
							}
							return;
						}
						if (!this.IsUcsPdlClassName(itemClass))
						{
							ExTraceGlobals.UCSTracer.TraceDebug<string, VersionedId>((long)this.GetHashCode(), "ForEachImGroup: Unexpected item class: {0}, Skipping it: {1}", itemClass, versionedId);
							return;
						}
						IL_12C:
						bool flag4 = Array.Exists<UnifiedContactStoreUtilities.SystemPdl>(UnifiedContactStoreUtilities.SystemPdls, (UnifiedContactStoreUtilities.SystemPdl systemPdl) => StringComparer.OrdinalIgnoreCase.Equals(systemPdl.ClassName, itemClass));
						if (flag4)
						{
							if (systemPdlsFound.ContainsKey(itemClass))
							{
								ExTraceGlobals.UCSTracer.TraceDebug<string, VersionedId>((long)this.GetHashCode(), "ForEachImGroup: Another PDL found for itemClass: {0}, Skipping it: {1}", itemClass, versionedId);
								return;
							}
							systemPdlsFound[itemClass] = versionedId;
						}
						ExTraceGlobals.UCSTracer.TraceDebug<string, bool, VersionedId>((long)this.GetHashCode(), "ForEachImGroup: Enumerating PDL: itemClass: {0}, SystemPdl: {1}, Id: {2}", itemClass, flag4, versionedId);
						actionOnEachImGroup(pdlPropertyBag);
					});
					if (fixedCorruptedPdls != 0)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "FixedCorruptedPDLs", fixedCorruptedPdls);
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "FixedSystemPdls", fixedSystemPdls);
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "FixedUserPdlsItemClass", fixedUserPdlsItemClass);
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "FixedUserPdlsCopyContacts", fixedUserPdlsCopyContacts);
					}
				}
			}
		}

		private List<Persona> GetPersonas(ExtendedPropertyUri[] extendedPropertyUris)
		{
			PropertyListForViewRowDeterminer propertyListForViewRowDeterminer = PropertyListForViewRowDeterminer.BuildForPersonObjects(Persona.FullPersonaShape);
			PropertyDefinition[] array = propertyListForViewRowDeterminer.GetPropertiesToFetch();
			if (extendedPropertyUris != null && extendedPropertyUris.Length > 0)
			{
				PropertyDefinition[] extendedProperties = this.Convert(extendedPropertyUris);
				PersonSchemaProperties personSchemaProperties = new PersonSchemaProperties(extendedProperties, new IEnumerable<PropertyDefinition>[]
				{
					array
				});
				array = personSchemaProperties.All;
			}
			List<Persona> result;
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, DefaultFolderType.QuickContacts))
			{
				List<Persona> list = new List<Persona>(folder.ItemCount);
				bool flag = false;
				while (!flag)
				{
					using (IQueryResult queryResult = folder.PersonItemQuery(null, null, null, array))
					{
						ExTraceGlobals.UCSTracer.TraceDebug<int>((long)this.GetHashCode(), "GetPersonas: QuickContacts Folder has an estimated count of {0} personas.", queryResult.EstimatedRowCount);
						IndexedPageView indexedPageView = new IndexedPageView
						{
							MaxRows = 2 * this.MaxContactsAllowed - list.Count,
							Offset = list.Count
						};
						ExTraceGlobals.UCSTracer.TraceDebug<int, int>((long)this.GetHashCode(), "GetPersonas: Querying page with MaxRows {0} and Offset {1}", indexedPageView.MaxRows, indexedPageView.Offset);
						Persona[] pageOfPersonas = this.GetPageOfPersonas(queryResult, indexedPageView, propertyListForViewRowDeterminer, array, out flag);
						list.AddRange(pageOfPersonas);
					}
				}
				ExTraceGlobals.UCSTracer.TraceDebug<int>((long)this.GetHashCode(), "GetPersonas: Returning a total of {0} personas", list.Count);
				result = list;
			}
			return result;
		}

		private void FixCorruptedPdl(IStorePropertyBag propertyBag, out bool pdlDeleted, out bool isSystemPdl, out bool setItemClass)
		{
			string text = (string)propertyBag[StoreObjectSchema.DisplayName];
			StoreObjectId storeObjectId = this.StoreObjectIdFrom(propertyBag);
			StoreObjectId storeObjectId2 = null;
			pdlDeleted = false;
			isSystemPdl = false;
			setItemClass = false;
			foreach (UnifiedContactStoreUtilities.SystemPdl systemPdl in UnifiedContactStoreUtilities.SystemPdls)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(systemPdl.DisplayName, text))
				{
					storeObjectId2 = this.GetSystemPdlId(systemPdl.DisplayName, systemPdl.ClassName);
					isSystemPdl = true;
					break;
				}
			}
			if (storeObjectId2 == null)
			{
				string text2 = propertyBag.TryGetProperty(DistributionListSchema.Email1EmailAddress) as string;
				if (text2 != null)
				{
					storeObjectId2 = this.FindPdlBy(DistributionListSchema.Email1EmailAddress, text2, true, "IPM.DistList.MOC.DG");
					if (storeObjectId2 == null)
					{
						this.SetPdlItemClass(storeObjectId, "IPM.DistList.MOC.DG");
						setItemClass = true;
					}
				}
				else
				{
					storeObjectId2 = this.FindPdlBy(StoreObjectSchema.DisplayName, text, true, "IPM.DistList.MOC.UserGroup");
					if (storeObjectId2 == null)
					{
						this.SetPdlItemClass(storeObjectId, "IPM.DistList.MOC.UserGroup");
						setItemClass = true;
					}
				}
			}
			if (storeObjectId2 != null)
			{
				this.CopyContacts(storeObjectId, storeObjectId2);
				this.session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					storeObjectId
				});
				pdlDeleted = true;
			}
		}

		private void CopyContacts(StoreObjectId fromId, StoreObjectId toId)
		{
			ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, StoreObjectId>((long)this.GetHashCode(), "CopyContacts: Copying contacts from: {0} to: {1}", fromId, toId);
			IDistributionList toGroup = null;
			IDistributionList distributionList = null;
			try
			{
				if (this.TryRetrieveGroupPdl(toId, null, out toGroup, false) && this.TryRetrieveGroupPdl(fromId, null, out distributionList, false))
				{
					bool contactAdded = false;
					this.ForEachContactDistributionListMember(distributionList, delegate(IDistributionListMember member, StoreParticipantOrigin storeParticipantOrigin)
					{
						StoreObjectId originItemId = storeParticipantOrigin.OriginItemId;
						string displayName = member.Participant.DisplayName;
						if (!this.IsMemberOfPdl(originItemId, toGroup))
						{
							ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, string, string>((long)this.GetHashCode(), "CopyContacts: Adding contact to group. ContactId:{0}, DisplayName: {1}, DL:{2}", originItemId, displayName ?? "<Null>", toGroup.DisplayName ?? "<Null>");
							Participant imParticipant = this.GetImParticipant(originItemId, displayName);
							toGroup.OpenAsReadWrite();
							toGroup.Add(imParticipant);
							contactAdded = true;
						}
						else
						{
							ExTraceGlobals.UCSTracer.TraceDebug<StoreObjectId, string, string>((long)this.GetHashCode(), "CopyContacts: Contact is already a member. ContactId:{0}, DisplayName: {1}, DL:{2}", originItemId, displayName ?? "<Null>", toGroup.DisplayName ?? "<Null>");
						}
						return true;
					});
					if (contactAdded)
					{
						ConflictResolutionResult conflictResolutionResult = toGroup.Save(SaveMode.NoConflictResolution);
						if (conflictResolutionResult.SaveStatus != SaveResult.Success)
						{
							ExTraceGlobals.UCSTracer.TraceWarning<string, ConflictResolutionResult>((long)this.GetHashCode(), "CopyContacts: Failed to add contacts to group {0}: {1}", toGroup.DisplayName ?? "<Null>", conflictResolutionResult);
						}
					}
				}
			}
			finally
			{
				if (toGroup != null)
				{
					toGroup.Dispose();
				}
				if (distributionList != null)
				{
					distributionList.Dispose();
				}
			}
		}

		private void SetPdlItemClass(StoreId pdlId, string itemClass)
		{
			IDistributionList distributionList = null;
			try
			{
				if (this.TryRetrieveGroupPdl(pdlId, null, out distributionList, false))
				{
					distributionList.OpenAsReadWrite();
					distributionList.ClassName = itemClass;
					ConflictResolutionResult conflictResolutionResult = distributionList.Save(SaveMode.NoConflictResolution);
					if (conflictResolutionResult.SaveStatus != SaveResult.Success)
					{
						ExTraceGlobals.UCSTracer.TraceWarning<ConflictResolutionResult>((long)this.GetHashCode(), "SetPdlItemClass: Failed to save the PDL: {0}", conflictResolutionResult);
					}
				}
			}
			finally
			{
				if (distributionList != null)
				{
					distributionList.Dispose();
				}
			}
		}

		internal static readonly string TaggedPdlDisplayName = "Tagged";

		internal static readonly string OtherContactsPdlDisplayName = "Other Contacts";

		internal static readonly string FavoritesPdlDisplayName = "Favorites";

		private static readonly ImItemList EmptyImItemList = new ImItemList
		{
			Groups = new ImGroup[0],
			Personas = new Persona[0]
		};

		private static readonly RawImItemList EmptyRawImItemList = new RawImItemList
		{
			Groups = new RawImGroup[0],
			Personas = new PersonId[0]
		};

		private static readonly UnifiedContactStoreUtilities.SystemPdl[] SystemPdls = new UnifiedContactStoreUtilities.SystemPdl[]
		{
			new UnifiedContactStoreUtilities.SystemPdl(UnifiedContactStoreUtilities.FavoritesPdlDisplayName, "IPM.DistList.MOC.Favorites"),
			new UnifiedContactStoreUtilities.SystemPdl(UnifiedContactStoreUtilities.TaggedPdlDisplayName, "IPM.DistList.MOC.Tagged"),
			new UnifiedContactStoreUtilities.SystemPdl(UnifiedContactStoreUtilities.OtherContactsPdlDisplayName, "IPM.DistList.MOC.OtherContacts")
		};

		private static readonly PropertyDefinition[] DistributionListId = new StorePropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] DistributionListIdAndItemClass = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass
		};

		private static readonly PropertyDefinition[] DistributionListRecoveryProperties = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			StoreObjectSchema.DisplayName,
			DistributionListSchema.Email1EmailAddress
		};

		private static readonly PropertyDefinition[] DistributionListBasicData = new StorePropertyDefinition[]
		{
			StoreObjectSchema.DisplayName,
			StoreObjectSchema.ItemClass,
			ItemSchema.Id,
			DistributionListSchema.Members,
			DistributionListSchema.Email1EmailAddress
		};

		private static readonly SortBy FindFirstItem = new SortBy(StoreObjectSchema.CreationTime, SortOrder.Ascending);

		private static readonly PropertyDefinition[] ContactIdAndPersonId = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			ContactSchema.PersonId
		};

		private static readonly PropertyDefinition[] RetrieveContactDataProperties = new StorePropertyDefinition[]
		{
			ContactSchema.PersonId,
			StoreObjectSchema.ParentItemId,
			ContactSchema.IMAddress,
			StoreObjectSchema.DisplayName,
			ContactSchema.TelUri,
			ContactSchema.ImContactSipUriAddress,
			ContactSchema.OtherTelephone
		};

		private static readonly TextFilter ContactQueryFilter = new TextFilter(StoreObjectSchema.ItemClass, "IPM.Contact", MatchOptions.Prefix, MatchFlags.Default);

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;

		private readonly IUnifiedContactStoreConfiguration configuration;

		private sealed class SystemPdl
		{
			public SystemPdl(string displayName, string className)
			{
				if (string.IsNullOrEmpty(displayName))
				{
					throw new ArgumentException("Invalid displayName: SystemPdl displayName cannot be null or empty ", "displayName");
				}
				if (string.IsNullOrEmpty(className))
				{
					throw new ArgumentException("Invalid className: SystemPdl className cannot be null or empty ", "className");
				}
				this.displayName = displayName;
				this.className = className;
				ExTraceGlobals.UCSTracer.TraceDebug<UnifiedContactStoreUtilities.SystemPdl>((long)this.GetHashCode(), "Initializing SystemPdl {0}", this);
			}

			public string DisplayName
			{
				get
				{
					return this.displayName;
				}
			}

			public string ClassName
			{
				get
				{
					return this.className;
				}
			}

			public override string ToString()
			{
				return string.Format("{0},{1}", this.DisplayName, this.ClassName);
			}

			private readonly string displayName;

			private readonly string className;
		}
	}
}
