using System;
using System.IO;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class StoreObjectId : StoreId, IEquatable<StoreObjectId>, IComparable<StoreObjectId>, IComparable
	{
		protected StoreObjectId(byte[] entryId, StoreObjectType itemType)
		{
			Util.ThrowOnNullArgument(entryId, "entryId");
			EnumValidator.AssertValid<StoreObjectType>(itemType);
			if (entryId.Length > 255)
			{
				throw new CorruptDataException(ServerStrings.ExEntryIdToLong);
			}
			this.EntryId = entryId;
			this.SetObjectType(itemType);
			this.Validate();
		}

		protected StoreObjectId(byte[] byteArray, int startingIndex)
		{
			Util.ThrowOnNullArgument(byteArray, "byteArray");
			if (byteArray.Length <= startingIndex + 1)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
			int num = (int)byteArray[startingIndex];
			StoreObjectId.CheckDataFormat(startingIndex, byteArray.Length, num);
			this.EntryId = new byte[num];
			Array.Copy(byteArray, 1 + startingIndex, this.EntryId, 0, num);
			this.SetObjectType((StoreObjectType)byteArray[1 + num + startingIndex]);
			this.Validate();
		}

		private static void CheckDataFormat(int startingIndex, int byteArrayLength, int countEntryIdBytes)
		{
			if (countEntryIdBytes <= 0 || byteArrayLength != countEntryIdBytes + 1 + 1 + startingIndex)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
		}

		public static StoreObjectId DummyId
		{
			get
			{
				return StoreObjectId.dummyId;
			}
		}

		public StoreObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		public byte[] ProviderLevelItemId
		{
			get
			{
				return (byte[])this.EntryId.Clone();
			}
		}

		public byte[] LongTermFolderId
		{
			get
			{
				if (this.IsFolderId)
				{
					byte[] array = new byte[22];
					Array.Copy(this.EntryId, 22, array, 0, 22);
					return array;
				}
				return null;
			}
		}

		public bool IsFakeId
		{
			get
			{
				return this.EntryId.Length == 0;
			}
		}

		public bool IsFolderId
		{
			get
			{
				return IdConverter.IsFolderId(this.EntryId);
			}
		}

		internal bool IsMessageId
		{
			get
			{
				return this.IsFakeId || IdConverter.IsMessageId(this.EntryId);
			}
		}

		public static StoreObjectId Deserialize(string base64Id)
		{
			if (base64Id == null)
			{
				throw new ArgumentNullException("base64Id");
			}
			byte[] byteArray = StoreId.Base64ToByteArray(base64Id);
			return StoreObjectId.Deserialize(byteArray);
		}

		public static StoreObjectId Deserialize(byte[] byteArray)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			return StoreObjectId.Parse(byteArray, 0);
		}

		public static StoreObjectId Deserialize(BinaryReader reader, int byteArrayLength)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return StoreObjectId.Parse(reader, 0, byteArrayLength);
		}

		public static void Serialize(StoreObjectId storeObjectId, BinaryWriter writer)
		{
			storeObjectId.WriteBytes(writer);
		}

		public static StoreObjectId FromProviderSpecificId(byte[] entryId)
		{
			return StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Unknown);
		}

		public static StoreObjectId FromProviderSpecificIdOrNull(byte[] entryId)
		{
			if (entryId != null)
			{
				return StoreObjectId.FromProviderSpecificId(entryId);
			}
			return null;
		}

		public static bool TryParseFromHexEntryId(string hexEntryId, out StoreObjectId storeObjectId)
		{
			storeObjectId = null;
			try
			{
				if (!string.IsNullOrWhiteSpace(hexEntryId))
				{
					storeObjectId = StoreObjectId.FromHexEntryId(hexEntryId);
					return true;
				}
			}
			catch (FormatException)
			{
			}
			catch (CorruptDataException)
			{
			}
			return false;
		}

		public static StoreObjectId FromHexEntryId(string hexEntryId)
		{
			return StoreObjectId.FromHexEntryId(hexEntryId, StoreObjectType.Unknown);
		}

		public static StoreObjectId FromHexEntryId(string hexEntryId, StoreObjectType storeObjectType)
		{
			Util.ThrowOnNullArgument(hexEntryId, "hexEntryId");
			EnumValidator.ThrowIfInvalid<StoreObjectType>(storeObjectType);
			if (storeObjectType == StoreObjectType.CalendarItemOccurrence)
			{
				throw new ArgumentException("StoreObjectId shouldn't be created for occurrences.", "storeObjectType");
			}
			return new StoreObjectId(HexConverter.HexStringToByteArray(hexEntryId), storeObjectType);
		}

		public static StoreObjectId FromProviderSpecificId(byte[] entryId, StoreObjectType objectType)
		{
			if (entryId == null)
			{
				throw new ArgumentNullException("entryId");
			}
			EnumValidator.ThrowIfInvalid<StoreObjectType>(objectType);
			if (objectType == StoreObjectType.CalendarItemOccurrence)
			{
				throw new ArgumentException("StoreObjectId shouldn't be created for occurrences.", "objectType");
			}
			return new StoreObjectId(entryId, objectType);
		}

		public static StoreObjectId FromLegacyFavoritePublicFolderId(StoreObjectId other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (!other.IsPublicFolderType())
			{
				throw new ArgumentException("other StoreObjectId is not a public folder type: " + other.GetFolderType());
			}
			if (!StoreObjectId.IsValidFlagsForStoreObjectId(other))
			{
				return StoreObjectId.ToNormalizedPublicFolderId(other);
			}
			StoreObjectId result = other;
			byte[] providerLevelItemId = other.ProviderLevelItemId;
			if (providerLevelItemId[21] == 128)
			{
				providerLevelItemId[21] = 0;
				result = StoreObjectId.FromProviderSpecificId(providerLevelItemId);
			}
			return result;
		}

		public static StoreObjectId ToLegacyFavoritePublicFolderId(StoreObjectId other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (!other.IsPublicFolderType())
			{
				throw new ArgumentException("other StoreObjectId is not a legacy public folder type: " + other.GetFolderType());
			}
			byte[] providerLevelItemId = other.ProviderLevelItemId;
			providerLevelItemId[21] = 128;
			return StoreObjectId.FromProviderSpecificId(providerLevelItemId);
		}

		public static bool IsValidFlagsForStoreObjectId(StoreObjectId folderId)
		{
			ArgumentValidator.ThrowIfNull("sourceFolderId", folderId);
			return folderId.EntryId[0] == 0 && folderId.EntryId[1] == 0 && folderId.EntryId[2] == 0 && folderId.EntryId[3] == 0;
		}

		public static StoreObjectId ToNormalizedPublicFolderId(StoreObjectId sourceFolderId)
		{
			ArgumentValidator.ThrowIfNull("sourceFolderId", sourceFolderId);
			if (!sourceFolderId.IsFolderId)
			{
				throw new ArgumentException("sourceFolderId is not a folder id");
			}
			byte[] array = new byte[sourceFolderId.EntryId.Length];
			Array.Copy(StoreObjectId.PublicFolderProviderUIDInBytes, 0, array, 4, 16);
			Array.Copy(StoreObjectId.LegacyPublicFolderTypeInBytes, 0, array, 20, StoreObjectId.LegacyPublicFolderTypeInBytes.Length);
			Array.Copy(sourceFolderId.EntryId, 22, array, 22, 22);
			return StoreObjectId.FromProviderSpecificId(array, sourceFolderId.ObjectType);
		}

		public bool IsLegacyPublicFolderType()
		{
			return this.GetFolderType() == 3;
		}

		public bool IsPublicFolderType()
		{
			return this.IsLegacyPublicFolderType() || this.GetFolderType() == 2;
		}

		public bool IsNormalizedPublicFolderId()
		{
			return this.IsFolderId && this.IsPublicFolderType() && Util.CompareByteArraySegments(this.EntryId, 4U, StoreObjectId.PublicFolderProviderUIDInBytes, 0U, 16U);
		}

		public override string ToBase64String()
		{
			return Convert.ToBase64String(this.GetBytes());
		}

		public virtual int GetByteArrayLength()
		{
			return this.InternalGetByteArrayLength();
		}

		public override byte[] GetBytes()
		{
			byte[] array = new byte[this.InternalGetByteArrayLength()];
			array[0] = (byte)this.EntryId.Length;
			this.EntryId.CopyTo(array, 1);
			array[1 + this.EntryId.Length] = (byte)this.objectType;
			return array;
		}

		protected virtual void WriteBytes(BinaryWriter writer)
		{
			writer.Write((byte)this.EntryId.Length);
			writer.Write(this.EntryId);
			writer.Write((byte)this.objectType);
		}

		public string ToHexEntryId()
		{
			return HexConverter.ByteArrayToHexString(this.EntryId);
		}

		public virtual StoreObjectId Clone()
		{
			return new StoreObjectId(this.EntryId, this.objectType);
		}

		public virtual void UpdateItemType(StoreObjectType newItemType)
		{
			EnumValidator.ThrowIfInvalid<StoreObjectType>(newItemType, "newItemType");
			if (newItemType == StoreObjectType.CalendarItemOccurrence)
			{
				throw new ArgumentException("StoreObjectId shouldn't be created for occurrences.", "newItemType");
			}
			this.objectType = newItemType;
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				int num = 0;
				int num2 = this.EntryId.Length;
				int i = 0;
				while (i + 3 < num2)
				{
					num ^= ((int)this.EntryId[i] | (int)this.EntryId[i + 1] << 8 | (int)this.EntryId[i + 2] << 16 | (int)this.EntryId[i + 3] << 24);
					i += 4;
				}
				while (i < num2)
				{
					num ^= (int)this.EntryId[i] << 8 * (i & 3);
					i++;
				}
				this.hashCode = num;
			}
			return this.hashCode;
		}

		public override string ToString()
		{
			return this.ToBase64String();
		}

		public override bool Equals(object id)
		{
			StoreObjectId id2 = id as StoreObjectId;
			return this.Equals(id2);
		}

		public override bool Equals(StoreId id)
		{
			StoreObjectId id2 = id as StoreObjectId;
			return this.Equals(id2);
		}

		public virtual bool Equals(StoreObjectId id)
		{
			if (id == null || !base.GetType().Equals(id.GetType()))
			{
				return false;
			}
			if (this.IsNormalizedPublicFolderId() || id.IsNormalizedPublicFolderId())
			{
				return Util.CompareByteArraySegments(this.EntryId, 22U, id.EntryId, 22U, 22U);
			}
			return ArrayComparer<byte>.Comparer.Equals(this.EntryId, id.EntryId);
		}

		public virtual int CompareTo(object o)
		{
			if (o == null)
			{
				return 1;
			}
			if (!base.GetType().Equals(o.GetType()))
			{
				throw new ArgumentException();
			}
			return this.CompareTo(o as StoreObjectId);
		}

		public int CompareTo(StoreObjectId o)
		{
			if (o == null)
			{
				return 1;
			}
			return ArrayComparer<byte>.Comparer.Compare(this.EntryId, o.EntryId);
		}

		internal string ToBase64ProviderLevelItemId()
		{
			string result = null;
			byte[] providerLevelItemId = this.ProviderLevelItemId;
			if (providerLevelItemId != null)
			{
				result = Convert.ToBase64String(providerLevelItemId);
			}
			return result;
		}

		internal static bool StoreEntryIdsAreForSamePrivateStore(byte[] storeEntryId1, byte[] storeEntryId2)
		{
			StoreObjectId.StoreEntryId storeEntryId3 = StoreObjectId.ParseStoreEntryId(storeEntryId1);
			StoreObjectId.StoreEntryId storeEntryId4 = StoreObjectId.ParseStoreEntryId(storeEntryId2);
			if (storeEntryId3.ServerName.Contains(".") && !storeEntryId4.ServerName.Contains("."))
			{
				storeEntryId3.ServerName = storeEntryId3.ServerName.Split(new char[]
				{
					'.'
				})[0];
			}
			else if (storeEntryId4.ServerName.Contains(".") && !storeEntryId3.ServerName.Contains("."))
			{
				storeEntryId4.ServerName = storeEntryId4.ServerName.Split(new char[]
				{
					'.'
				})[0];
			}
			return (storeEntryId3.StoreFlags & OpenStoreFlag.Public) != OpenStoreFlag.Public && (storeEntryId4.StoreFlags & OpenStoreFlag.Public) != OpenStoreFlag.Public && storeEntryId3.ServerName.Equals(storeEntryId4.ServerName, StringComparison.OrdinalIgnoreCase) && storeEntryId3.LegacyDn.Equals(storeEntryId4.LegacyDn, StringComparison.OrdinalIgnoreCase);
		}

		internal static StoreObjectId Parse(byte[] byteArray, int startingIndex)
		{
			if (byteArray == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			if (byteArray.Length <= 1 + startingIndex)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
			if (byteArray[byteArray.Length - 1] == 16)
			{
				return new OccurrenceStoreObjectId(byteArray, startingIndex);
			}
			return new StoreObjectId(byteArray, startingIndex);
		}

		internal static StoreObjectId Parse(BinaryReader reader, int startingIndex, int byteArrayLength)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (byteArrayLength <= 1 + startingIndex)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
			if (startingIndex > 0)
			{
				reader.ReadBytes(startingIndex);
			}
			byte b = reader.ReadByte();
			byte[] array = reader.ReadBytes((int)b);
			int num = byteArrayLength - (startingIndex + 1 + (int)b);
			if (num < 1)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidIdFormat);
			}
			byte[] array2 = reader.ReadBytes(num);
			StoreObjectType storeObjectType = (StoreObjectType)array2[num - 1];
			if (storeObjectType == StoreObjectType.CalendarItemOccurrence)
			{
				return new OccurrenceStoreObjectId(b, array, array2);
			}
			StoreObjectId.CheckDataFormat(startingIndex, byteArrayLength, (int)b);
			return new StoreObjectId(array, storeObjectType);
		}

		private static StoreObjectId.StoreEntryId ParseStoreEntryId(byte[] entryId)
		{
			StoreObjectId.StoreEntryId result;
			using (ParticipantEntryId.Reader reader = new ParticipantEntryId.Reader(entryId))
			{
				try
				{
					bool flag = false;
					reader.BaseStream.Seek(22L, SeekOrigin.Begin);
					if (reader.BytesRemaining > 0)
					{
						string text = reader.ReadZString(CTSGlobals.AsciiEncoding);
						if (text.Equals("emsmdb.dll", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
					}
					if (flag)
					{
						reader.BaseStream.Seek(36L, SeekOrigin.Begin);
					}
					else
					{
						reader.BaseStream.Seek(0L, SeekOrigin.Begin);
					}
					result.MapiFlags = reader.ReadInt32();
					result.MapiUid = reader.ReadGuid();
					result.StoreFlags = (OpenStoreFlag)reader.ReadInt32();
					result.ServerName = reader.ReadZString(CTSGlobals.AsciiEncoding);
					if ((result.StoreFlags & OpenStoreFlag.Public) != OpenStoreFlag.Public)
					{
						result.LegacyDn = reader.ReadZString(CTSGlobals.AsciiEncoding);
					}
					else
					{
						result.LegacyDn = string.Empty;
					}
				}
				catch (EndOfStreamException innerException)
				{
					throw new CorruptDataException(ServerStrings.ExInvalidIdFormat, innerException);
				}
			}
			return result;
		}

		private void SetObjectType(StoreObjectType objectType)
		{
			this.objectType = objectType;
			if (!EnumValidator.IsValidValue<StoreObjectType>(this.objectType))
			{
				this.objectType = StoreObjectType.Unknown;
			}
			if (this.objectType == StoreObjectType.Unknown && this.IsFolderId)
			{
				this.objectType = StoreObjectType.Folder;
				return;
			}
			bool flag = StoreObjectTypeClassifier.IsFolderObjectType(this.objectType);
			if (flag && this.IsMessageId)
			{
				this.objectType = StoreObjectType.Message;
				return;
			}
			if (!flag && this.IsFolderId)
			{
				this.objectType = StoreObjectType.Folder;
				return;
			}
			if (!StoreObjectTypeExclusions.E12KnownObjectType(this.objectType))
			{
				if (this.IsFolderId)
				{
					this.objectType = StoreObjectType.Folder;
					return;
				}
				if (!StoreObjectTypeClassifier.AlwaysReportRealType(this.objectType))
				{
					this.objectType = StoreObjectType.Message;
				}
			}
		}

		private void Validate()
		{
			if (this.objectType != StoreObjectType.Mailbox && !this.IsMessageId && !this.IsFolderId)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidStoreObjectId);
			}
		}

		private int InternalGetByteArrayLength()
		{
			return 1 + this.EntryId.Length + 1;
		}

		private byte GetFolderType()
		{
			return this.ProviderLevelItemId[20];
		}

		private const int WrappedIdDllNameIndex = 22;

		private const int WrappedIdEntryIdIndex = 36;

		private const int FolderTypeIndex = 20;

		private const int LegacyPublicFolderType = 3;

		private const int AlternateLegacyPublicFolderType = 2;

		private const int ProviderIdIndexInFolderEntryId = 4;

		private const int ProviderIdLengthInFolderEntryId = 16;

		private const int PublicFolderFavoriteFlagIndex = 21;

		private const int PublicFolderFavoriteFlag = 128;

		private const int NonPublicFolderFavoriteFlag = 0;

		private const int LongTermFolderIdIndex = 22;

		public const int LongTermFolderIdLength = 22;

		protected readonly byte[] EntryId;

		private static readonly StoreObjectId dummyId = StoreObjectId.FromProviderSpecificId(Array<byte>.Empty);

		private static readonly byte[] LegacyPublicFolderTypeInBytes = BitConverter.GetBytes(3);

		private StoreObjectType objectType;

		[NonSerialized]
		private int hashCode;

		public static readonly Guid PublicFolderProviderUID = Guid.Parse("9073441A-66AA-CD11-9BC8-00AA002FC45A");

		public static readonly byte[] PublicFolderProviderUIDInBytes = StoreObjectId.PublicFolderProviderUID.ToByteArray();

		internal struct StoreEntryId
		{
			public int MapiFlags;

			public Guid MapiUid;

			public OpenStoreFlag StoreFlags;

			public string ServerName;

			public string LegacyDn;
		}
	}
}
