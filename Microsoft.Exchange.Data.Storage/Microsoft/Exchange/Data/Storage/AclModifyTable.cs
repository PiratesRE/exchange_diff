using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AclModifyTable : DisposableObject, IModifyTable, IDisposable
	{
		internal AclModifyTable(CoreFolder coreFolder, ModifyTableOptions options, IModifyTableRestriction modifyTableRestriction, bool useSecurityDescriptorOnly) : this(coreFolder, options, modifyTableRestriction, useSecurityDescriptorOnly, true)
		{
		}

		internal AclModifyTable(CoreFolder coreFolder, ModifyTableOptions options, IModifyTableRestriction modifyTableRestriction, bool useSecurityDescriptorOnly, bool loadTableEntries)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.coreFolder = coreFolder;
				this.options = options;
				this.modifyTableRestriction = modifyTableRestriction;
				this.recipientSession = coreFolder.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				this.useSecurityDescriptorOnly = useSecurityDescriptorOnly;
				if (loadTableEntries)
				{
					this.Load();
				}
				else
				{
					this.replaceAllRows = true;
				}
				disposeGuard.Success();
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AclModifyTable>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				Util.DisposeIfPresent(this.externalUsers);
			}
			base.InternalDispose(disposing);
		}

		public static byte[] BuildAclTableBlob(StoreSession session, RawSecurityDescriptor securityDescriptor, RawSecurityDescriptor freeBusySecurityDescriptor)
		{
			IRecipientSession adrecipientSession = session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
			ExternalUserCollection externalUserCollectionToDispose = null;
			bool flag;
			string canonicalErrorInformation;
			List<AclTableEntry> source;
			try
			{
				source = AclModifyTable.BuildAclTableFromSecurityDescriptor(securityDescriptor, freeBusySecurityDescriptor, new LazilyInitialized<ExternalUserCollection>(delegate()
				{
					MailboxSession mailboxSession = session as MailboxSession;
					externalUserCollectionToDispose = ((mailboxSession != null) ? mailboxSession.GetExternalUsers() : null);
					return externalUserCollectionToDispose;
				}), adrecipientSession, new AclTableIdMap(), out flag, out canonicalErrorInformation);
			}
			finally
			{
				Util.DisposeIfPresent(externalUserCollectionToDispose);
			}
			if (!flag)
			{
				ExTraceGlobals.StorageTracer.TraceError(0L, "Cannot build blob ACL table blob with non-canonical SD");
				throw new NonCanonicalACLException(canonicalErrorInformation);
			}
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSD = new FolderSecurity.AclTableAndSecurityDescriptorProperty(new ArraySegment<byte>(AclModifyTable.SerializeTableEntries(source)), source.ToDictionary((AclTableEntry tableEntry) => tableEntry.SecurityIdentifier, delegate(AclTableEntry tableEntry)
			{
				if (!tableEntry.IsGroup)
				{
					return FolderSecurity.SecurityIdentifierType.User;
				}
				return FolderSecurity.SecurityIdentifierType.Group;
			}), SecurityDescriptor.FromRawSecurityDescriptor(securityDescriptor), SecurityDescriptor.FromRawSecurityDescriptor(freeBusySecurityDescriptor));
			return AclModifyTable.SerializeAclTableAndSecurityDecscriptor(aclTableAndSD);
		}

		public static FolderSecurity.AclTableAndSecurityDescriptorProperty ReadAclTableAndSecurityDescriptor(ICorePropertyBag propertyBag)
		{
			byte[] buffer;
			using (Stream stream = propertyBag.OpenPropertyStream(CoreFolderSchema.AclTableAndSecurityDescriptor, PropertyOpenMode.ReadOnly))
			{
				FolderPropertyStream stream2 = stream as FolderPropertyStream;
				buffer = Util.StreamHandler.ReadBytesFromStream(stream2);
			}
			return FolderSecurity.AclTableAndSecurityDescriptorProperty.Parse(buffer);
		}

		public static void SetFolderSecurityDescriptor(CoreFolder folder, RawSecurityDescriptor securityDescriptor)
		{
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclModifyTable.ReadAclTableAndSecurityDescriptor(folder.PropertyBag);
			byte[] propertyToSet = AclModifyTable.BuildAclTableBlob(folder.Session, securityDescriptor, (aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor != null) ? aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor.ToRawSecurityDescriptorThrow() : null);
			AclModifyTable.WriteFolderAclTable(folder, propertyToSet);
		}

		public static void SetFolderFreeBusySecurityDescriptor(CoreFolder folder, RawSecurityDescriptor freeBusySecurityDescriptor)
		{
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclModifyTable.ReadAclTableAndSecurityDescriptor(folder.PropertyBag);
			byte[] propertyToSet = AclModifyTable.BuildAclTableBlob(folder.Session, (aclTableAndSecurityDescriptorProperty.SecurityDescriptor != null) ? aclTableAndSecurityDescriptorProperty.SecurityDescriptor.ToRawSecurityDescriptorThrow() : null, freeBusySecurityDescriptor);
			AclModifyTable.WriteFolderAclTable(folder, propertyToSet);
		}

		public void Clear()
		{
			this.CheckDisposed(null);
			this.replaceAllRows = true;
			this.pendingModifyOperations.Clear();
		}

		public void AddRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingChange(ModifyTableOperationType.Add, propValues);
		}

		public void ModifyRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingChange(ModifyTableOperationType.Modify, propValues);
		}

		public void RemoveRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingChange(ModifyTableOperationType.Remove, propValues);
		}

		public IQueryResult GetQueryResult(QueryFilter queryFilter, ICollection<PropertyDefinition> columns)
		{
			this.CheckDisposed(null);
			return new AclQueryResult(this.Session, this.tableEntries, (this.options & ModifyTableOptions.ExtendedPermissionInformation) != ModifyTableOptions.None, (this.options & ModifyTableOptions.FreeBusyAware) == ModifyTableOptions.None, columns);
		}

		public void ApplyPendingChanges()
		{
			this.CheckDisposed(null);
			bool flag = (this.options & ModifyTableOptions.ExtendedPermissionInformation) == ModifyTableOptions.ExtendedPermissionInformation;
			IRecipientSession recipientSession = this.recipientSession;
			if (flag)
			{
				this.recipientSession = null;
			}
			try
			{
				if (!this.propertyTableRestrictionSuppressed && this.modifyTableRestriction != null)
				{
					this.modifyTableRestriction.Enforce(this, this.pendingModifyOperations);
				}
				if (this.replaceAllRows)
				{
					this.tableEntries.Clear();
				}
				List<long> list = new List<long>(1);
				AclTableEntry.ModifyOperation[] array = (from op in this.pendingModifyOperations
				select AclTableEntry.ModifyOperation.FromModifyTableOperation(op)).ToArray<AclTableEntry.ModifyOperation>();
				for (int i = 0; i < array.Length; i++)
				{
					AclTableEntry.ModifyOperation aclModifyOperation = array[i];
					if ((aclModifyOperation.Entry.MemberRights & (MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed)) != MemberRights.None && (this.options & ModifyTableOptions.FreeBusyAware) != ModifyTableOptions.FreeBusyAware)
					{
						throw new InvalidParamException(new LocalizedString("F/B unaware clients sent F/B rights"));
					}
					switch (aclModifyOperation.Operation)
					{
					case ModifyTableOperationType.Add:
					{
						SecurityIdentifier securityIdentifier = null;
						List<SecurityIdentifier> sidHistory = null;
						bool flag2 = false;
						string memberName = null;
						string arg;
						if (flag)
						{
							arg = AclHelper.LegacyDnFromEntryId(aclModifyOperation.Entry.MemberEntryId);
							bool flag3 = false;
							bool flag4 = false;
							bool flag5 = false;
							foreach (PropValue propValue in this.pendingModifyOperations[i].Properties)
							{
								if (propValue.Property == PermissionSchema.MemberIsGroup)
								{
									flag3 = true;
									flag2 = (bool)propValue.Value;
								}
								else if (propValue.Property == PermissionSchema.MemberSecurityIdentifier)
								{
									flag5 = true;
									securityIdentifier = new SecurityIdentifier((byte[])propValue.Value, 0);
								}
								else if (propValue.Property == PermissionSchema.MemberName)
								{
									flag4 = true;
									memberName = (string)propValue.Value;
								}
							}
							if (!flag3 || !flag4 || !flag5)
							{
								throw new InvalidOperationException(string.Format("Required property is missing. IsGroupFound={0}, DisplayNameFound={1}, SecurityIdentifierFound={2}", flag3, flag4, flag5));
							}
						}
						else if (!AclHelper.TryGetUserFromEntryId(aclModifyOperation.Entry.MemberEntryId, this.Session, this.recipientSession, new LazilyInitialized<ExternalUserCollection>(() => this.GetExternalUsers(this.Session)), out arg, out securityIdentifier, out sidHistory, out flag2, out memberName))
						{
							ExTraceGlobals.StorageTracer.TraceWarning<string>(0L, "Cannot find recipient for LegDN {0}, skip this entry", arg);
							break;
						}
						aclModifyOperation.Entry.SetSecurityIdentifier(securityIdentifier, flag2);
						aclModifyOperation.Entry.SetMemberId(AclModifyTable.GetIdForSecurityIdentifier(securityIdentifier, sidHistory, this.coreFolder.AclTableIdMap));
						aclModifyOperation.Entry.SetMemberName(memberName);
						int num = this.tableEntries.FindIndex((AclTableEntry aclTableEntry) => aclTableEntry.MemberId == aclModifyOperation.Entry.MemberId);
						if (num != -1)
						{
							this.tableEntries.RemoveAt(num);
						}
						this.FixRightsIfNeeded(aclModifyOperation.Entry);
						if (flag2)
						{
							this.tableEntries.Add(aclModifyOperation.Entry);
						}
						else if (this.tableEntries.Count == 0 || this.tableEntries[0].MemberId != 0L)
						{
							this.tableEntries.Insert(0, aclModifyOperation.Entry);
						}
						else
						{
							this.tableEntries.Insert(1, aclModifyOperation.Entry);
						}
						break;
					}
					case ModifyTableOperationType.Modify:
					{
						if (this.replaceAllRows && aclModifyOperation.Entry.MemberId != -1L && aclModifyOperation.Entry.MemberId != 0L)
						{
							throw new InvalidParamException(new LocalizedString("Modify with ReplaceAllRows"));
						}
						AclTableEntry aclTableEntry2 = this.tableEntries.Find((AclTableEntry aclTableEntry) => aclTableEntry.MemberId == aclModifyOperation.Entry.MemberId);
						if (aclTableEntry2 == null)
						{
							if (aclModifyOperation.Entry.MemberId == -1L)
							{
								aclTableEntry2 = AclModifyTable.BuildAnonymousDefaultEntry();
								this.tableEntries.Add(aclTableEntry2);
							}
							else
							{
								if (aclModifyOperation.Entry.MemberId != 0L)
								{
									throw new ObjectNotFoundException(new LocalizedString("AclTableEntry not found"));
								}
								aclTableEntry2 = AclModifyTable.BuildEveryoneDefaultEntry(MemberRights.FreeBusySimple);
								this.tableEntries.Add(aclTableEntry2);
							}
						}
						this.FixRightsIfNeeded(aclModifyOperation.Entry);
						aclTableEntry2.MemberRights = aclModifyOperation.Entry.MemberRights;
						break;
					}
					case ModifyTableOperationType.Remove:
					{
						if (this.replaceAllRows)
						{
							throw new InvalidParamException(new LocalizedString("Remove with ReplaceAllRows"));
						}
						bool flag6 = false;
						for (int k = i + 1; k < array.Length; k++)
						{
							if (array[k].Operation == ModifyTableOperationType.Modify && aclModifyOperation.Entry.MemberId == array[k].Entry.MemberId)
							{
								flag6 = true;
							}
						}
						if (!flag6)
						{
							int num2 = this.tableEntries.FindIndex((AclTableEntry aclTableEntry) => aclTableEntry.MemberId == aclModifyOperation.Entry.MemberId);
							if (num2 == -1)
							{
								if (!list.Contains(aclModifyOperation.Entry.MemberId))
								{
									throw new ObjectNotFoundException(new LocalizedString("AclTableEntry not found"));
								}
							}
							else
							{
								list.Add(aclModifyOperation.Entry.MemberId);
								this.tableEntries.RemoveAt(num2);
							}
						}
						break;
					}
					}
				}
				this.replaceAllRows = false;
				this.pendingModifyOperations.Clear();
				this.Save();
			}
			finally
			{
				this.recipientSession = recipientSession;
			}
		}

		public void SuppressRestriction()
		{
			this.CheckDisposed(null);
			this.propertyTableRestrictionSuppressed = true;
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed(null);
				return this.coreFolder.Session;
			}
		}

		private static FolderSecurity.SecurityIdentifierType GetSecurityIdentifierType(IRecipientSession recipientSession, SecurityIdentifier securityIdentifier)
		{
			if (ExternalUser.IsExternalUserSid(securityIdentifier))
			{
				return FolderSecurity.SecurityIdentifierType.User;
			}
			ADRecipient adrecipient = recipientSession.FindBySid(securityIdentifier);
			if (adrecipient == null)
			{
				return FolderSecurity.SecurityIdentifierType.Unknown;
			}
			if (!AclHelper.IsGroupRecipientType(adrecipient.RecipientType))
			{
				return FolderSecurity.SecurityIdentifierType.User;
			}
			return FolderSecurity.SecurityIdentifierType.Group;
		}

		private static AclTableEntry BuildEveryoneDefaultEntry(MemberRights rights)
		{
			AclTableEntry aclTableEntry = new AclTableEntry(0L, Array<byte>.Empty, string.Empty, rights);
			aclTableEntry.SetSecurityIdentifier(new SecurityIdentifier(WellKnownSidType.WorldSid, null), false);
			return aclTableEntry;
		}

		private static AclTableEntry BuildAnonymousDefaultEntry()
		{
			AclTableEntry aclTableEntry = new AclTableEntry(-1L, Array<byte>.Empty, "Anonymous", MemberRights.None);
			aclTableEntry.SetSecurityIdentifier(new SecurityIdentifier(WellKnownSidType.AnonymousSid, null), false);
			return aclTableEntry;
		}

		private static byte[] SerializeTableEntries(List<AclTableEntry> tableEntries)
		{
			byte[] result;
			using (BinarySerializer binarySerializer = new BinarySerializer())
			{
				FolderSecurity.AclTableEntry.SerializeTableEntries<AclTableEntry>(tableEntries, binarySerializer.Writer);
				result = binarySerializer.ToArray();
			}
			return result;
		}

		private static long GetIdForSecurityIdentifier(SecurityIdentifier securityIdentifier, List<SecurityIdentifier> sidHistory, AclTableIdMap aclTableIdMap)
		{
			if (securityIdentifier.IsWellKnown(WellKnownSidType.WorldSid))
			{
				return 0L;
			}
			if (securityIdentifier.IsWellKnown(WellKnownSidType.AnonymousSid))
			{
				return -1L;
			}
			return aclTableIdMap.GetIdForSecurityIdentifier(securityIdentifier, sidHistory);
		}

		private static List<AclTableEntry> BuildAclTableFromSecurityDescriptor(RawSecurityDescriptor securityDescriptor, RawSecurityDescriptor freeBusySecurityDescriptor, LazilyInitialized<ExternalUserCollection> lazilyInitializedExternalUserCollection, IRecipientSession recipientSession, AclTableIdMap aclTableIdMap, out bool isCanonical, out string canonicalErrorInformation)
		{
			FolderSecurity.AnnotatedAceList annotatedAceList = new FolderSecurity.AnnotatedAceList(securityDescriptor, freeBusySecurityDescriptor, (SecurityIdentifier securityIdentifier) => AclModifyTable.GetSecurityIdentifierType(recipientSession, securityIdentifier));
			isCanonical = annotatedAceList.IsCanonical(out canonicalErrorInformation);
			IList<FolderSecurity.SecurityIdentifierAndFolderRights> list;
			if (isCanonical)
			{
				list = annotatedAceList.GetSecurityIdentifierAndRightsList();
			}
			else
			{
				ExTraceGlobals.StorageTracer.TraceWarning<string, string>(0L, "Got non canonical SD: {0}, ErrorInfo: ", securityDescriptor.GetSddlForm(AccessControlSections.All), canonicalErrorInformation);
				list = Array<FolderSecurity.SecurityIdentifierAndFolderRights>.Empty;
			}
			List<AclTableEntry> list2 = new List<AclTableEntry>(list.Count + 1);
			foreach (FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights in list)
			{
				MemberRights memberRights = (MemberRights)(securityIdentifierAndFolderRights.AllowRights & ~(MemberRights)securityIdentifierAndFolderRights.DenyRights);
				bool flag = false;
				bool flag2 = false;
				byte[] entryId;
				string text;
				List<SecurityIdentifier> list3;
				SecurityIdentifier securityIdentifier;
				if (securityIdentifierAndFolderRights.SecurityIdentifier.IsWellKnown(WellKnownSidType.WorldSid))
				{
					entryId = Array<byte>.Empty;
					text = string.Empty;
					string legacyDN = string.Empty;
					securityIdentifier = securityIdentifierAndFolderRights.SecurityIdentifier;
					list3 = null;
					flag2 = true;
				}
				else if (securityIdentifierAndFolderRights.SecurityIdentifier.IsWellKnown(WellKnownSidType.AnonymousSid))
				{
					entryId = Array<byte>.Empty;
					text = "Anonymous";
					securityIdentifier = securityIdentifierAndFolderRights.SecurityIdentifier;
					list3 = null;
				}
				else if (ExternalUser.IsExternalUserSid(securityIdentifierAndFolderRights.SecurityIdentifier))
				{
					ExternalUser externalUser = AclHelper.TryGetExternalUser(securityIdentifierAndFolderRights.SecurityIdentifier, lazilyInitializedExternalUserCollection);
					if (externalUser == null)
					{
						ExTraceGlobals.StorageTracer.TraceWarning<SecurityIdentifier>(0L, "Cannot find external user with SID {0}, build entry from information we have", securityIdentifierAndFolderRights.SecurityIdentifier);
						string text2 = AclHelper.CreateLocalUserStrignRepresentation(securityIdentifierAndFolderRights.SecurityIdentifier);
						text = text2;
					}
					else
					{
						text = externalUser.Name;
						string legacyDN = externalUser.LegacyDn;
					}
					entryId = AddressBookEntryId.MakeAddressBookEntryIDFromLocalDirectorySid(securityIdentifierAndFolderRights.SecurityIdentifier);
					securityIdentifier = securityIdentifierAndFolderRights.SecurityIdentifier;
					list3 = null;
				}
				else
				{
					MiniRecipient miniRecipient = recipientSession.FindMiniRecipientBySid<MiniRecipient>(securityIdentifierAndFolderRights.SecurityIdentifier, Array<PropertyDefinition>.Empty);
					string legacyDN;
					if (miniRecipient == null)
					{
						ExTraceGlobals.StorageTracer.TraceWarning<SecurityIdentifier>(0L, "Cannot find recipient with SID {0}, build entry from the information we have", securityIdentifierAndFolderRights.SecurityIdentifier);
						flag = (securityIdentifierAndFolderRights.SecurityIdentifierType == FolderSecurity.SecurityIdentifierType.Group);
						string text3 = AclHelper.CreateNTUserStrignRepresentation(securityIdentifierAndFolderRights.SecurityIdentifier);
						text = text3;
						legacyDN = text3;
						securityIdentifier = securityIdentifierAndFolderRights.SecurityIdentifier;
						list3 = null;
					}
					else
					{
						flag = AclHelper.IsGroupRecipientType(miniRecipient.RecipientType);
						if (string.IsNullOrEmpty(miniRecipient.DisplayName))
						{
							text = AclHelper.CreateNTUserStrignRepresentation(securityIdentifierAndFolderRights.SecurityIdentifier);
						}
						else
						{
							text = miniRecipient.DisplayName;
						}
						if (string.IsNullOrEmpty(miniRecipient.LegacyExchangeDN))
						{
							legacyDN = text;
						}
						else
						{
							legacyDN = miniRecipient.LegacyExchangeDN;
						}
						SecurityIdentifier masterAccountSid = miniRecipient.MasterAccountSid;
						if (masterAccountSid != null && !masterAccountSid.IsWellKnown(WellKnownSidType.SelfSid))
						{
							securityIdentifier = masterAccountSid;
							list3 = null;
						}
						else
						{
							securityIdentifier = miniRecipient.Sid;
							MultiValuedProperty<SecurityIdentifier> sidHistory = miniRecipient.SidHistory;
							if (sidHistory != null && sidHistory.Count != 0)
							{
								list3 = new List<SecurityIdentifier>(sidHistory);
							}
							else
							{
								list3 = null;
							}
						}
					}
					entryId = AddressBookEntryId.MakeAddressBookEntryID(legacyDN, flag);
				}
				AclTableEntry aclTableEntry = list2.Find((AclTableEntry entry) => entry.SecurityIdentifier == securityIdentifier);
				if (aclTableEntry == null && list3 != null)
				{
					using (List<SecurityIdentifier>.Enumerator enumerator2 = list3.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SecurityIdentifier sid = enumerator2.Current;
							aclTableEntry = list2.Find((AclTableEntry entry) => entry.SecurityIdentifier == sid);
							if (aclTableEntry != null)
							{
								break;
							}
						}
					}
				}
				if (aclTableEntry == null)
				{
					aclTableEntry = new AclTableEntry(AclModifyTable.GetIdForSecurityIdentifier(securityIdentifier, list3, aclTableIdMap), entryId, text, memberRights);
					aclTableEntry.SetSecurityIdentifier(securityIdentifier, flag);
					if (flag2)
					{
						list2.Insert(0, aclTableEntry);
					}
					else
					{
						list2.Add(aclTableEntry);
					}
				}
				else
				{
					aclTableEntry.MemberRights &= memberRights;
					if (aclTableEntry.IsGroup != flag)
					{
						throw new NonCanonicalACLException(annotatedAceList.CreateErrorInformation((LID)35788U, new int[0]));
					}
					aclTableEntry.SetSecurityIdentifier(securityIdentifier, flag);
				}
			}
			return list2;
		}

		private static byte[] SerializeAclTableAndSecurityDecscriptor(FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSD)
		{
			if (aclTableAndSD.SecurityDescriptor != null && aclTableAndSD.SecurityDescriptor.BinaryForm != null && aclTableAndSD.SecurityDescriptor.BinaryForm.Length > 31744)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>(0L, "Folder SD exceeds allowed size: {0}", aclTableAndSD.SecurityDescriptor.BinaryForm.Length);
				throw new ACLTooBigException();
			}
			if (aclTableAndSD.FreeBusySecurityDescriptor != null && aclTableAndSD.FreeBusySecurityDescriptor.BinaryForm != null && aclTableAndSD.FreeBusySecurityDescriptor.BinaryForm.Length > 31744)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>(0L, "Folder F/B SD exceeds allowed size: {0}", aclTableAndSD.FreeBusySecurityDescriptor.BinaryForm.Length);
				throw new ACLTooBigException();
			}
			return aclTableAndSD.Serialize();
		}

		private List<AclTableEntry> ParseTableEntries(ArraySegment<byte> tableSegment)
		{
			List<AclTableEntry> result;
			using (BinaryDeserializer binaryDeserializer = new BinaryDeserializer(tableSegment))
			{
				List<AclTableEntry> list = FolderSecurity.AclTableEntry.ParseTableEntries<AclTableEntry>(binaryDeserializer.Reader, new Func<BinaryReader, AclTableEntry>(AclTableEntry.Parse));
				HashSet<string> hashSet = null;
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						AclTableEntry aclTableEntry = list[i];
						if (aclTableEntry.MemberEntryId != null && aclTableEntry.MemberEntryId.Length != 0)
						{
							if (hashSet == null)
							{
								hashSet = new HashSet<string>();
							}
							string text = AclHelper.LegacyDnFromEntryId(aclTableEntry.MemberEntryId);
							if (!hashSet.Add(text.ToLower()))
							{
								return null;
							}
						}
						if (aclTableEntry.MemberId != 0L && aclTableEntry.MemberId != -1L)
						{
							aclTableEntry.SetMemberId(AclModifyTable.GetIdForSecurityIdentifier(aclTableEntry.SecurityIdentifier, null, this.coreFolder.AclTableIdMap));
						}
					}
				}
				result = list;
			}
			return result;
		}

		private List<AclTableEntry> BuildAclTableFromSecurityDescriptor(out bool isCanonical, out string canonicalErrorInformation)
		{
			return AclModifyTable.BuildAclTableFromSecurityDescriptor(this.securityDescriptor, this.freeBusySecurityDescriptor, new LazilyInitialized<ExternalUserCollection>(() => this.GetExternalUsers(this.Session)), this.recipientSession, this.coreFolder.AclTableIdMap, out isCanonical, out canonicalErrorInformation);
		}

		private ExternalUserCollection GetExternalUsers(StoreSession session)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (this.externalUsers == null && mailboxSession != null)
			{
				this.externalUsers = mailboxSession.GetExternalUsers();
			}
			return this.externalUsers;
		}

		internal static void WriteFolderAclTable(CoreFolder coreFolder, byte[] propertyToSet)
		{
			using (FolderPropertyStream folderPropertyStream = (FolderPropertyStream)coreFolder.PropertyBag.OpenPropertyStream(CoreFolderSchema.AclTableAndSecurityDescriptor, PropertyOpenMode.Modify))
			{
				folderPropertyStream.Write(propertyToSet, 0, propertyToSet.Length);
			}
		}

		private void Load()
		{
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclModifyTable.ReadAclTableAndSecurityDescriptor(this.coreFolder.PropertyBag);
			if (aclTableAndSecurityDescriptorProperty.SecurityDescriptor == null)
			{
				return;
			}
			this.securityDescriptor = aclTableAndSecurityDescriptorProperty.SecurityDescriptor.ToRawSecurityDescriptorThrow();
			this.freeBusySecurityDescriptor = ((aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor != null) ? aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor.ToRawSecurityDescriptorThrow() : null);
			List<AclTableEntry> list = null;
			if (aclTableAndSecurityDescriptorProperty.SerializedAclTable.Count != 0 && !this.useSecurityDescriptorOnly)
			{
				list = this.ParseTableEntries(aclTableAndSecurityDescriptorProperty.SerializedAclTable);
			}
			if (list == null)
			{
				bool flag;
				string canonicalErrorInformation;
				this.tableEntries = this.BuildAclTableFromSecurityDescriptor(out flag, out canonicalErrorInformation);
				if (!flag && (this.options & ModifyTableOptions.ExtendedPermissionInformation) == ModifyTableOptions.ExtendedPermissionInformation)
				{
					ExTraceGlobals.StorageTracer.TraceError(0L, "Cannot build blob ACL table blob with non-canonical SD");
					throw new NonCanonicalACLException(canonicalErrorInformation);
				}
			}
			else
			{
				this.tableEntries = list;
			}
			if (this.tableEntries.Count == 0 || this.tableEntries[0].MemberId != 0L)
			{
				MemberRights rights = (this.freeBusySecurityDescriptor == null) ? MemberRights.FreeBusySimple : MemberRights.None;
				this.tableEntries.Insert(0, AclModifyTable.BuildEveryoneDefaultEntry(rights));
			}
		}

		private void Save()
		{
			List<FolderSecurity.SecurityIdentifierAndFolderRights> list = new List<FolderSecurity.SecurityIdentifierAndFolderRights>(this.tableEntries.Count);
			foreach (AclTableEntry aclTableEntry in this.tableEntries)
			{
				list.Add(new FolderSecurity.SecurityIdentifierAndFolderRights(aclTableEntry.SecurityIdentifier, (FolderSecurity.ExchangeFolderRights)aclTableEntry.MemberRights, aclTableEntry.IsGroup ? FolderSecurity.SecurityIdentifierType.Group : FolderSecurity.SecurityIdentifierType.User));
				aclTableEntry.MemberRights = (MemberRights)FolderSecurity.NormalizeFolderRights((FolderSecurity.ExchangeFolderRights)aclTableEntry.MemberRights);
			}
			byte[] array = AclModifyTable.SerializeTableEntries(this.tableEntries);
			RawAcl rawAcl = FolderSecurity.AnnotatedAceList.BuildFolderCanonicalAceList(list);
			if (this.securityDescriptor != null)
			{
				this.securityDescriptor.DiscretionaryAcl = rawAcl;
			}
			else
			{
				this.securityDescriptor = FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateFolderSecurityDescriptor(rawAcl).ToRawSecurityDescriptorThrow();
			}
			RawAcl rawAcl2 = FolderSecurity.AnnotatedAceList.BuildFreeBusyCanonicalAceList(list);
			if (this.freeBusySecurityDescriptor != null)
			{
				this.freeBusySecurityDescriptor.DiscretionaryAcl = rawAcl2;
			}
			else if ((this.options & ModifyTableOptions.FreeBusyAware) == ModifyTableOptions.FreeBusyAware)
			{
				this.freeBusySecurityDescriptor = FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateFolderSecurityDescriptor(rawAcl2).ToRawSecurityDescriptorThrow();
			}
			Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> dictionary = new Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType>(list.Count);
			foreach (FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights in list)
			{
				if (dictionary.ContainsKey(securityIdentifierAndFolderRights.SecurityIdentifier))
				{
					throw new InvalidParamException(new LocalizedString(string.Format("SID {0} is not unique.", securityIdentifierAndFolderRights.SecurityIdentifier)));
				}
				dictionary.Add(securityIdentifierAndFolderRights.SecurityIdentifier, securityIdentifierAndFolderRights.SecurityIdentifierType);
			}
			FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSD = new FolderSecurity.AclTableAndSecurityDescriptorProperty(new ArraySegment<byte>(array), dictionary, SecurityDescriptor.FromRawSecurityDescriptor(this.securityDescriptor), SecurityDescriptor.FromRawSecurityDescriptor(this.freeBusySecurityDescriptor));
			this.coreFolder.OnBeforeFolderSave();
			AclModifyTable.WriteFolderAclTable(this.coreFolder, AclModifyTable.SerializeAclTableAndSecurityDecscriptor(aclTableAndSD));
			this.coreFolder.OnAfterFolderSave();
		}

		private void AddPendingChange(ModifyTableOperationType operation, PropValue[] propValues)
		{
			this.pendingModifyOperations.Add(new ModifyTableOperation(operation, propValues));
		}

		private void FixRightsIfNeeded(AclTableEntry tableEntry)
		{
			if ((tableEntry.MemberRights & MemberRights.FreeBusyDetailed) == MemberRights.FreeBusyDetailed)
			{
				tableEntry.MemberRights |= MemberRights.FreeBusySimple;
			}
			if ((this.options & ModifyTableOptions.FreeBusyAware) != ModifyTableOptions.FreeBusyAware)
			{
				if (tableEntry.MemberId == -1L)
				{
					tableEntry.MemberRights &= ~(MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed);
				}
				else
				{
					tableEntry.MemberRights |= MemberRights.FreeBusySimple;
					if ((tableEntry.MemberRights & MemberRights.ReadAny) == MemberRights.ReadAny)
					{
						tableEntry.MemberRights |= MemberRights.FreeBusyDetailed;
					}
				}
			}
			if ((tableEntry.MemberRights & (MemberRights.ReadAny | MemberRights.Owner)) != MemberRights.None)
			{
				tableEntry.MemberRights |= MemberRights.Visible;
			}
			if ((tableEntry.MemberRights & MemberRights.DeleteAny) != MemberRights.None)
			{
				tableEntry.MemberRights |= MemberRights.DeleteOwned;
			}
			if ((tableEntry.MemberRights & MemberRights.EditAny) != MemberRights.None)
			{
				tableEntry.MemberRights |= MemberRights.EditOwned;
			}
		}

		internal const int MaxSecurityDescriptorSize = 31744;

		private readonly CoreFolder coreFolder;

		private readonly ModifyTableOptions options;

		private readonly IModifyTableRestriction modifyTableRestriction;

		private readonly bool useSecurityDescriptorOnly;

		private IRecipientSession recipientSession;

		private List<AclTableEntry> tableEntries = new List<AclTableEntry>();

		private RawSecurityDescriptor securityDescriptor;

		private RawSecurityDescriptor freeBusySecurityDescriptor;

		private bool replaceAllRows;

		private List<ModifyTableOperation> pendingModifyOperations = new List<ModifyTableOperation>();

		private bool propertyTableRestrictionSuppressed;

		private ExternalUserCollection externalUsers;
	}
}
