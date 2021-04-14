using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DistributionList : ContactBase, IDistributionList, IContactBase, IItem, IStoreObject, IRecipientBaseCollection<DistributionListMember>, IList<DistributionListMember>, ICollection<DistributionListMember>, IEnumerable<DistributionListMember>, IEnumerable, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal DistributionList(ICoreItem coreItem) : base(coreItem)
		{
			if (base.IsNew)
			{
				this.Initialize();
			}
		}

		private static ParticipantEntryId[] ParseEntryIds(byte[][] entryIds)
		{
			ParticipantEntryId[] array = new ParticipantEntryId[entryIds.Length];
			for (int i = 0; i < entryIds.Length; i++)
			{
				array[i] = ParticipantEntryId.TryFromEntryId(entryIds[i]);
			}
			return array;
		}

		private static void ParseEntryIdStream(Stream dlStream, out ParticipantEntryId[] mainEntryIds, out ParticipantEntryId[] oneOffEntryIds, out byte[][] mainIds, out byte[][] extraBytes, out bool alwaysStream)
		{
			using (BinaryReader binaryReader = new BinaryReader(dlStream, Encoding.Unicode))
			{
				binaryReader.ReadUInt16();
				binaryReader.ReadUInt16();
				binaryReader.ReadInt32();
				int num = binaryReader.ReadInt32();
				alwaysStream = ((num & 1) == 1);
				int num2 = binaryReader.ReadInt32();
				if (num2 > StorageLimits.Instance.DistributionListMaxNumberOfEntries)
				{
					throw new CorruptDataException(ServerStrings.ExPDLCorruptOutlookBlob("TooManyEntries"));
				}
				binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				mainIds = new byte[num2][];
				byte[][] array = new byte[num2][];
				extraBytes = new byte[num2][];
				for (int i = 0; i < num2; i++)
				{
					int num3 = binaryReader.ReadInt32();
					mainIds[i] = ((num3 > 0) ? binaryReader.ReadBytes(num3) : Array<byte>.Empty);
					num3 = binaryReader.ReadInt32();
					array[i] = ((num3 > 0) ? binaryReader.ReadBytes(num3) : Array<byte>.Empty);
					num3 = binaryReader.ReadInt32();
					extraBytes[i] = ((num3 > 0) ? binaryReader.ReadBytes(num3) : Array<byte>.Empty);
				}
				mainEntryIds = DistributionList.ParseEntryIds(mainIds);
				oneOffEntryIds = DistributionList.ParseEntryIds(array);
			}
		}

		private static bool NeedToStream(PropertyDefinition propertyDefinition, byte[][] value)
		{
			int num = 0;
			foreach (byte[] array in value)
			{
				num += array.Length;
			}
			return num > StorageLimits.Instance.DistributionListMaxMembersPropertySize;
		}

		private static void SerializeEntryIdsOnStream(bool alwaysStream, Stream dlStream, ParticipantEntryId[] mainEntryIds, ParticipantEntryId[] oneOffEntryIds, byte[][] extraBytes)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			using (BinaryWriter binaryWriter = new BinaryWriter(dlStream, Encoding.Unicode))
			{
				binaryWriter.Write(1);
				binaryWriter.Write(0);
				binaryWriter.Write(14000);
				binaryWriter.Write(alwaysStream ? 1 : 0);
				binaryWriter.Write(mainEntryIds.Length);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
				binaryWriter.Write(num3);
				for (int i = 0; i < mainEntryIds.Length; i++)
				{
					byte[] array = mainEntryIds[i].ToByteArray();
					num += array.Length;
					binaryWriter.Write(array.Length);
					if (array.Length > 0)
					{
						binaryWriter.Write(array);
					}
					array = oneOffEntryIds[i].ToByteArray();
					num2 += array.Length;
					binaryWriter.Write(array.Length);
					if (array.Length > 0)
					{
						binaryWriter.Write(array);
					}
					num3 += extraBytes[i].Length;
					binaryWriter.Write(extraBytes[i].Length);
					if (extraBytes[i].Length > 0)
					{
						binaryWriter.Write(extraBytes[i]);
					}
				}
				binaryWriter.Write(0);
				binaryWriter.Write(0);
				int offset = 16;
				binaryWriter.Seek(offset, SeekOrigin.Begin);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
				binaryWriter.Write(num3);
			}
		}

		private static byte[][] EncodeEntryIds(ParticipantEntryId[] entryIds)
		{
			byte[][] array = new byte[entryIds.Length][];
			for (int i = 0; i < entryIds.Length; i++)
			{
				if (entryIds[i] != null)
				{
					array[i] = entryIds[i].ToByteArray();
				}
				else
				{
					array[i] = Array<byte>.Empty;
				}
			}
			return array;
		}

		public static bool IsDL(RecipientType recipientType)
		{
			EnumValidator.ThrowIfInvalid<RecipientType>(recipientType);
			switch (recipientType)
			{
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				return true;
			default:
				return false;
			}
		}

		public static Participant[] ExpandDeep(StoreSession storeSession, StoreObjectId distributionListId, bool shouldAddNonExistPDL)
		{
			Dictionary<StoreObjectId, Participant> dictionary = new Dictionary<StoreObjectId, Participant>();
			Queue<StoreObjectId> queue = new Queue<StoreObjectId>();
			List<Participant> list = new List<Participant>();
			queue.Enqueue(distributionListId);
			while (queue.Count > 0)
			{
				StoreObjectId storeObjectId = queue.Dequeue();
				DistributionList distributionList = null;
				try
				{
					distributionList = DistributionList.Bind(storeSession, storeObjectId);
				}
				catch (ObjectNotFoundException arg)
				{
					if (storeSession.ItemBinder != null)
					{
						Item item = storeSession.ItemBinder.BindItem(storeObjectId, IdConverter.IsFromPublicStore(storeObjectId), IdConverter.GetParentIdFromMessageId(storeObjectId));
						distributionList = (item as DistributionList);
						if (item != null && distributionList == null)
						{
							item.Dispose();
						}
					}
					if (distributionList == null)
					{
						ExTraceGlobals.StorageTracer.TraceDebug<ObjectNotFoundException>(0L, "DistributionList::ExpandDeep. A PDL member in PDL doesn't exist. Ignore it and continue to expand other members. Exception = {0}.", arg);
						if (shouldAddNonExistPDL && dictionary.ContainsKey(storeObjectId))
						{
							list.Add(new Participant(dictionary[storeObjectId].DisplayName, null, "MAPIPDL"));
						}
					}
				}
				if (distributionList != null)
				{
					using (distributionList)
					{
						foreach (DistributionListMember distributionListMember in distributionList)
						{
							if (!(distributionListMember.Participant == null))
							{
								if (distributionListMember.IsDistributionList() == true && distributionListMember.Participant.Origin is StoreParticipantOrigin && distributionListMember.Participant.ValidationStatus == ParticipantValidationStatus.NoError)
								{
									StoreObjectId originItemId = ((StoreParticipantOrigin)distributionListMember.Participant.Origin).OriginItemId;
									if (!dictionary.ContainsKey(originItemId) && !originItemId.Equals(distributionListId))
									{
										queue.Enqueue(originItemId);
										dictionary.Add(originItemId, distributionListMember.Participant);
									}
								}
								else
								{
									list.Add(distributionListMember.Participant);
								}
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		public static Participant[] ExpandDeep(StoreSession storeSession, StoreObjectId distributionListId)
		{
			return DistributionList.ExpandDeep(storeSession, distributionListId, false);
		}

		public new static DistributionList Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<DistributionList>(session, storeId, DistributionListSchema.Instance, propsToReturn);
		}

		public new static DistributionList Bind(StoreSession session, StoreId storeId)
		{
			return DistributionList.Bind(session, storeId, null);
		}

		public new static DistributionList Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return DistributionList.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public static DistributionList Create(StoreSession session, StoreId contactFolderId)
		{
			return ItemBuilder.CreateNewItem<DistributionList>(session, contactFolderId, ItemCreateInfo.DistributionListInfo);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DistributionList>(this);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return DistributionListSchema.Instance;
			}
		}

		public override bool IsDirty
		{
			get
			{
				this.CheckDisposed("IsDirty::get");
				return base.IsDirty || this.areMembersChanged;
			}
		}

		public Participant GetAsParticipant()
		{
			return base.GetValueOrDefault<Participant>(InternalSchema.DistributionListParticipant);
		}

		public void Sort(IComparer<DistributionListMember> comparer)
		{
			this.CheckDisposed("Sort");
			this.EnsureExpanded();
			this.members.Sort(comparer);
		}

		public DistributionListMember Add(Participant participant)
		{
			this.CheckDisposed("Add");
			if (participant == null)
			{
				throw new ArgumentNullException("participant");
			}
			this.EnsureExpanded();
			this.MarkMembersChanged();
			DistributionListMember distributionListMember = new DistributionListMember(this, participant);
			this.members.Add(distributionListMember);
			return distributionListMember;
		}

		public DistributionListMember this[RecipientId id]
		{
			get
			{
				this.CheckDisposed("this[RecipientBaseId]::get");
				throw new NotSupportedException();
			}
		}

		public void Remove(RecipientId id)
		{
			this.CheckDisposed("Remove(RecipientId)");
			throw new NotSupportedException();
		}

		public int IndexOf(DistributionListMember item)
		{
			this.CheckDisposed("IndexOf");
			this.EnsureExpanded();
			return this.members.IndexOf(item);
		}

		public void Insert(int index, DistributionListMember item)
		{
			this.CheckDisposed("Insert");
			throw new NotSupportedException();
		}

		public DistributionListMember this[int index]
		{
			get
			{
				this.CheckDisposed("this[index]::get");
				this.EnsureExpanded();
				return this.members[index];
			}
			set
			{
				this.CheckDisposed("this[index]::set");
				throw new NotSupportedException();
			}
		}

		public void RemoveAt(int index)
		{
			this.CheckDisposed("RemoveAt");
			this.EnsureExpanded();
			this.MarkMembersChanged();
			this.UpdateContactsRemoved(this.members[index]);
			this.members.RemoveAt(index);
		}

		public void Add(DistributionListMember item)
		{
			this.CheckDisposed("Add(DistributionListMember)");
			this.EnsureExpanded();
			this.MarkMembersChanged();
			this.members.Add(DistributionListMember.CopyFrom(this, item));
		}

		public void Clear()
		{
			this.CheckDisposed("Clear");
			this.MarkMembersChanged();
			this.EnsureExpanded();
			foreach (DistributionListMember distributionListMember in this.members)
			{
				if (!(distributionListMember.Participant == null))
				{
					this.UpdateContactsRemoved(distributionListMember);
				}
			}
			if (this.members != null)
			{
				this.members.Clear();
				return;
			}
			this.members = new List<DistributionListMember>();
		}

		public bool Contains(DistributionListMember item)
		{
			this.CheckDisposed("Contains");
			this.EnsureExpanded();
			return this.members.Contains(item);
		}

		public void CopyTo(DistributionListMember[] array, int arrayIndex)
		{
			this.CheckDisposed("CopyTo");
			this.EnsureExpanded();
			this.members.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				this.CheckDisposed("Count::get");
				this.EnsureExpanded();
				return this.members.Count;
			}
		}

		bool ICollection<DistributionListMember>.IsReadOnly
		{
			get
			{
				this.CheckDisposed("IsReadOnly::get");
				return base.IsReadOnly;
			}
		}

		public bool Remove(IDistributionListMember item)
		{
			return this.Remove((DistributionListMember)item);
		}

		public bool Remove(DistributionListMember item)
		{
			this.CheckDisposed("Remove");
			this.EnsureExpanded();
			this.MarkMembersChanged();
			this.UpdateContactsRemoved(item);
			return this.members.Remove(item);
		}

		public IEnumerator<DistributionListMember> GetEnumerator()
		{
			this.CheckDisposed("GetEnumerator");
			this.EnsureExpanded();
			return this.members.GetEnumerator();
		}

		public IEnumerator<IDistributionListMember> IGetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal bool AlwaysStream
		{
			get
			{
				return this.alwaysStream;
			}
			set
			{
				this.alwaysStream = value;
			}
		}

		internal bool ChecksumIsCurrent()
		{
			object obj = base.PropertyBag.TryGetProperty(DistributionListSchema.DLChecksum);
			return !(obj is PropertyError) && (int)obj == (int)this.computedChecksum;
		}

		private static uint ComputeChecksum(byte[][] memberIds)
		{
			uint num = 0U;
			foreach (byte[] bytes in memberIds)
			{
				num = ComputeCRC.Compute(num, bytes);
			}
			return num;
		}

		private void MarkMembersChanged()
		{
			this.areMembersChanged = true;
		}

		private IList<StoreObjectId> FindContacts()
		{
			List<StoreObjectId> list = new List<StoreObjectId>();
			foreach (DistributionListMember distributionListMember in this)
			{
				if (!(distributionListMember.Participant == null) && distributionListMember.IsDistributionList() != true)
				{
					StoreParticipantOrigin storeParticipantOrigin = distributionListMember.Participant.Origin as StoreParticipantOrigin;
					if (storeParticipantOrigin != null)
					{
						list.Add(storeParticipantOrigin.OriginItemId);
					}
				}
			}
			return list;
		}

		private void UpdateContactsRemoved(DistributionListMember member)
		{
			if (member.IsDistributionList() != true)
			{
				StoreParticipantOrigin storeParticipantOrigin = member.Participant.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null)
				{
					if (this.contactsRemoved == null)
					{
						this.contactsRemoved = new List<StoreObjectId>();
					}
					this.contactsRemoved.Add(storeParticipantOrigin.OriginItemId);
				}
			}
		}

		private void SetFakeEntryInLegacyPDL()
		{
			string text = "Unknown";
			ParticipantEntryId participantEntryId = new OneOffParticipantEntryId(ClientStrings.LegacyPDLFakeEntry, text, text);
			byte[][] value = DistributionList.EncodeEntryIds(new ParticipantEntryId[]
			{
				participantEntryId
			});
			this[DistributionListSchema.Members] = value;
			this[DistributionListSchema.OneOffMembers] = value;
		}

		private void GetEntryIds(out ParticipantEntryId[] mainEntryIds, out ParticipantEntryId[] oneOffEntryIds, out byte[][] extraBytes)
		{
			DistributionList.GetEntryIds(base.PropertyBag, out mainEntryIds, out oneOffEntryIds, out extraBytes, out this.computedChecksum, out this.alwaysStream);
		}

		internal static void GetEntryIds(ICorePropertyBag propertyBag, out ParticipantEntryId[] mainEntryIds, out ParticipantEntryId[] oneOffEntryIds, out byte[][] extraBytes, out uint computedCheckSum, out bool alwaysStream)
		{
			mainEntryIds = new ParticipantEntryId[0];
			oneOffEntryIds = new ParticipantEntryId[0];
			extraBytes = new byte[0][];
			computedCheckSum = 0U;
			alwaysStream = false;
			PropertyError propertyError = propertyBag.TryGetProperty(DistributionListSchema.DLStream) as PropertyError;
			if (propertyError == null || PropertyError.IsPropertyValueTooBig(propertyError))
			{
				long num = -1L;
				try
				{
					using (Stream stream = propertyBag.OpenPropertyStream(DistributionListSchema.DLStream, PropertyOpenMode.ReadOnly))
					{
						if (stream != null && stream.Length > 0L)
						{
							num = stream.Length;
							byte[][] memberIds;
							DistributionList.ParseEntryIdStream(stream, out mainEntryIds, out oneOffEntryIds, out memberIds, out extraBytes, out alwaysStream);
							computedCheckSum = DistributionList.ComputeChecksum(memberIds);
							return;
						}
						ExTraceGlobals.StorageTracer.TraceWarning<string>(0L, "DistributionList::GetEntryIds. DLStream property is {0}.", (stream == null) ? "null" : "empty");
					}
				}
				catch (EndOfStreamException innerException)
				{
					string arg = (propertyError == null) ? "<null>" : propertyError.ToLocalizedString();
					LocalizedString message = ServerStrings.ExPDLCorruptOutlookBlob(string.Format("EndOfStreamException: propertyError={0}, streamLength={1}", arg, num.ToString()));
					throw new CorruptDataException(message, innerException);
				}
				catch (OutOfMemoryException innerException2)
				{
					throw new CorruptDataException(ServerStrings.ExPDLCorruptOutlookBlob("OutOfMemoryException"), innerException2);
				}
			}
			mainEntryIds = DistributionList.ParseEntryIds(propertyBag.GetValueOrDefault<byte[][]>(DistributionListSchema.Members, DistributionList.EmptyEntryIds));
			oneOffEntryIds = DistributionList.ParseEntryIds(propertyBag.GetValueOrDefault<byte[][]>(DistributionListSchema.OneOffMembers, DistributionList.EmptyEntryIds));
			extraBytes = new byte[mainEntryIds.Length][];
			computedCheckSum = DistributionList.ComputeChecksum(propertyBag.GetValueOrDefault<byte[][]>(DistributionListSchema.Members, DistributionList.EmptyEntryIds));
		}

		private void EnsureExpanded()
		{
			if (this.members != null)
			{
				return;
			}
			ParticipantEntryId[] array;
			ParticipantEntryId[] array2;
			byte[][] array3;
			this.GetEntryIds(out array, out array2, out array3);
			this.PreprocessEntryIds(array, ref array2);
			this.members = new List<DistributionListMember>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				this.members.Add(new DistributionListMember(this, array[i], (OneOffParticipantEntryId)array2[i], array3[i]));
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.members = null;
			}
			base.InternalDispose(disposing);
		}

		private void Initialize()
		{
			base.Load(new PropertyDefinition[]
			{
				InternalSchema.ItemClass
			});
			string text = base.TryGetProperty(InternalSchema.ItemClass) as string;
			if (!ObjectClass.IsDistributionList(text))
			{
				if (!string.IsNullOrEmpty(text))
				{
					ExTraceGlobals.StorageTracer.TraceWarning<string, string>(0L, "DistributionList::Initialize. Overwriting ItemClass from \"{0}\" to \"{1}\".", text, "IPM.DistList");
				}
				this[InternalSchema.ItemClass] = "IPM.DistList";
			}
		}

		private void NormalizeADEntryIds(ParticipantEntryId[] mainEntryIds, ParticipantEntryId[] oneOffEntryIds)
		{
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < mainEntryIds.Length; i++)
			{
				ADParticipantEntryId adparticipantEntryId = mainEntryIds[i] as ADParticipantEntryId;
				OneOffParticipantEntryId oneOffParticipantEntryId = (OneOffParticipantEntryId)oneOffEntryIds[i];
				if (adparticipantEntryId != null && (oneOffParticipantEntryId == null || adparticipantEntryId.IsDL == null))
				{
					list.Add(adparticipantEntryId.LegacyDN);
					list2.Add(i);
				}
				else if (oneOffParticipantEntryId != null && oneOffParticipantEntryId.EmailAddressType == "EX")
				{
					list.Add(oneOffParticipantEntryId.EmailAddress);
					list2.Add(i);
				}
			}
			if (list.Count > 0)
			{
				object[][] array = DistributionList.LookupInAD(() => base.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), list, DistributionList.adLookupProperties);
				for (int j = 0; j < list.Count; j++)
				{
					if (array[j] != null)
					{
						int num = list2[j];
						if (mainEntryIds[num] is ADParticipantEntryId && mainEntryIds[num].IsDL == null)
						{
							bool flag = DistributionList.IsDL((RecipientType)array[j][2]);
							mainEntryIds[num] = new ADParticipantEntryId(list[j], new LegacyRecipientDisplayType?(flag ? LegacyRecipientDisplayType.DistributionList : LegacyRecipientDisplayType.MailUser), true);
						}
						OneOffParticipantEntryId oneOffParticipantEntryId2 = oneOffEntryIds[num] as OneOffParticipantEntryId;
						oneOffEntryIds[num] = new OneOffParticipantEntryId((oneOffParticipantEntryId2 != null) ? oneOffParticipantEntryId2.EmailDisplayName : ((string)array[j][0]), array[j][1].ToString(), "SMTP");
					}
				}
			}
		}

		protected override void OnAfterSave(ConflictResolutionResult acrResults)
		{
			base.OnAfterSave(acrResults);
			if (acrResults.SaveStatus != SaveResult.IrresolvableConflict)
			{
				this.members = null;
			}
		}

		private void SetEntryIds(ParticipantEntryId[] mainEntryIds, ParticipantEntryId[] oneOffEntryIds, byte[][] extraBytes)
		{
			bool flag = this.alwaysStream;
			byte[][] array = DistributionList.EncodeEntryIds(mainEntryIds);
			byte[][] value = DistributionList.EncodeEntryIds(oneOffEntryIds);
			this.computedChecksum = DistributionList.ComputeChecksum(array);
			if (!flag)
			{
				flag = DistributionList.NeedToStream(DistributionListSchema.Members, array);
			}
			if (!flag)
			{
				flag = DistributionList.NeedToStream(DistributionListSchema.OneOffMembers, value);
			}
			if (flag)
			{
				using (Stream stream = base.OpenPropertyStream(DistributionListSchema.DLStream, PropertyOpenMode.Create))
				{
					DistributionList.SerializeEntryIdsOnStream(this.alwaysStream, stream, mainEntryIds, oneOffEntryIds, extraBytes);
					this.SetFakeEntryInLegacyPDL();
					goto IL_93;
				}
			}
			base.Delete(DistributionListSchema.DLStream);
			this[DistributionListSchema.Members] = array;
			this[DistributionListSchema.OneOffMembers] = value;
			IL_93:
			this[DistributionListSchema.DLChecksum] = (int)this.computedChecksum;
		}

		private void DeleteEntryIds()
		{
			base.Delete(DistributionListSchema.DLStream);
			base.Delete(DistributionListSchema.Members);
			base.Delete(DistributionListSchema.OneOffMembers);
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this.OnBeforeSaveUpdateOneOffIds();
		}

		private void OnBeforeSaveUpdateOneOffIds()
		{
			if (this.areMembersChanged)
			{
				if (this.members != null && this.members.Count > 0)
				{
					ParticipantEntryId[] mainEntryIds;
					ParticipantEntryId[] oneOffEntryIds;
					byte[][] extraBytes;
					this.ProcessAndReturnEntryIds(out mainEntryIds, out oneOffEntryIds, out extraBytes);
					this.SetEntryIds(mainEntryIds, oneOffEntryIds, extraBytes);
					this.areMembersChanged = false;
					return;
				}
				this.DeleteEntryIds();
			}
		}

		internal void ProcessAndReturnEntryIds(out ParticipantEntryId[] mainEntryIds, out ParticipantEntryId[] oneOffEntryIds, out byte[][] extraBytes)
		{
			mainEntryIds = new ParticipantEntryId[this.members.Count];
			oneOffEntryIds = new ParticipantEntryId[this.members.Count];
			extraBytes = new byte[this.members.Count][];
			for (int i = 0; i < this.members.Count; i++)
			{
				mainEntryIds[i] = this.members[i].MainEntryId;
				oneOffEntryIds[i] = this.members[i].OneOffEntryId;
				extraBytes[i] = this.members[i].ExtraBytes;
			}
			this.PostprocessEntryIds(mainEntryIds, oneOffEntryIds);
		}

		private void PostprocessEntryIds(ParticipantEntryId[] mainEntryIds, ParticipantEntryId[] oneOffEntryIds)
		{
			this.NormalizeADEntryIds(mainEntryIds, oneOffEntryIds);
		}

		private void PreprocessEntryIds(ParticipantEntryId[] mainEntryIds, ref ParticipantEntryId[] oneOffEntryIds)
		{
			OneOffParticipantEntryId[] array = new OneOffParticipantEntryId[mainEntryIds.Length];
			if (!this.ChecksumIsCurrent())
			{
				int i = 0;
				while (i < mainEntryIds.Length)
				{
					StoreParticipantEntryId storeParticipantEntryId = mainEntryIds[i] as StoreParticipantEntryId;
					OneOffParticipantEntryId oneOffParticipantEntryId = mainEntryIds[i] as OneOffParticipantEntryId;
					if (storeParticipantEntryId != null)
					{
						try
						{
							using (Item item = Microsoft.Exchange.Data.Storage.Item.Bind(base.Session, storeParticipantEntryId.ToUniqueItemId(), ContactSchema.Instance.AutoloadProperties))
							{
								Contact contact = item as Contact;
								DistributionList distributionList = item as DistributionList;
								if (contact != null)
								{
									Participant participant = contact.EmailAddresses[storeParticipantEntryId.EmailAddressIndex];
									if (participant != null)
									{
										Participant participant2 = new Participant(contact.DisplayName, participant.EmailAddress, participant.RoutingType, participant.Origin, Array<PropValue>.Empty);
										array[i] = (OneOffParticipantEntryId)ParticipantEntryId.FromParticipant(participant2, ParticipantEntryIdConsumer.SupportsNone);
									}
								}
								else if (distributionList != null)
								{
									array[i] = (OneOffParticipantEntryId)ParticipantEntryId.FromParticipant(distributionList.GetAsParticipant(), ParticipantEntryIdConsumer.SupportsNone);
								}
							}
							goto IL_F1;
						}
						catch (ObjectNotFoundException)
						{
							goto IL_F1;
						}
						goto IL_EA;
					}
					goto IL_EA;
					IL_F1:
					i++;
					continue;
					IL_EA:
					if (oneOffParticipantEntryId != null)
					{
						array[i] = oneOffParticipantEntryId;
						goto IL_F1;
					}
					goto IL_F1;
				}
			}
			else
			{
				for (int j = 0; j < mainEntryIds.Length; j++)
				{
					array[j] = (oneOffEntryIds[j] as OneOffParticipantEntryId);
				}
			}
			OneOffParticipantEntryId[] array2 = (OneOffParticipantEntryId[])array.Clone();
			this.NormalizeADEntryIds(mainEntryIds, array2);
			bool flag = oneOffEntryIds.Length == array.Length;
			int num = 0;
			while (flag && num < oneOffEntryIds.Length)
			{
				OneOffParticipantEntryId oneOffParticipantEntryId2 = oneOffEntryIds[num] as OneOffParticipantEntryId;
				if (oneOffParticipantEntryId2 == null)
				{
					flag = false;
				}
				else if (array2[num] != null)
				{
					flag = ParticipantComparer.EmailAddressIgnoringRoutingType.Equals(array2[num].ToParticipant(), oneOffParticipantEntryId2.ToParticipant());
				}
				num++;
			}
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k] == null)
				{
					if (array2[k] != null)
					{
						array[k] = array2[k];
					}
					else if (flag)
					{
						array[k] = (OneOffParticipantEntryId)oneOffEntryIds[k];
					}
				}
			}
			oneOffEntryIds = array;
		}

		private static object[][] LookupInAD(Func<IRecipientSession> adRecipientSessionFactory, List<string> legacyDNs, params PropertyDefinition[] properties)
		{
			object[][] result;
			try
			{
				Result<ADRawEntry>[] array = adRecipientSessionFactory().FindByLegacyExchangeDNs(legacyDNs.ToArray(), properties);
				if (array.Length != legacyDNs.Count)
				{
					ExDiagnostics.FailFast(string.Format(CultureInfo.InvariantCulture, "Number of results in IRecipientSession.FindByLegacyExchangeDNs() is unexpected: {0} instead of {1}", new object[]
					{
						array.Length,
						legacyDNs.Count
					}), false);
				}
				result = Array.ConvertAll<Result<ADRawEntry>, object[]>(array, delegate(Result<ADRawEntry> entry)
				{
					if (entry.Data == null)
					{
						return null;
					}
					return entry.Data.GetProperties(properties);
				});
			}
			catch (DataSourceOperationException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "DistributionList.LookupInAD. Failed due to directory exception {0}.", new object[]
				{
					ex
				});
			}
			catch (DataSourceTransientException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "DistributionList.LookupInAD. Failed due to directory exception {0}.", new object[]
				{
					ex2
				});
			}
			return result;
		}

		private const ushort StreamBlobVersion = 1;

		private const int StreamBlobBuildVersion = 14000;

		private const int StreamBlobFlags = 0;

		private const int AlwaysStreamFlag = 1;

		private static readonly byte[][] EmptyEntryIds = Array<byte[]>.Empty;

		private bool areMembersChanged;

		private List<DistributionListMember> members;

		private bool alwaysStream;

		private List<StoreObjectId> contactsRemoved;

		private uint computedChecksum;

		private static PropertyDefinition[] adLookupProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientType
		};
	}
}
