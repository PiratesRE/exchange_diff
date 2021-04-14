using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncStateTypeFactory
	{
		private SyncStateTypeFactory()
		{
			this.tables = new List<ICustomSerializableBuilder>[]
			{
				this.internalBuilderTable,
				this.externalBuilderTable
			};
		}

		internal static string ExternalSignature
		{
			get
			{
				return "External";
			}
		}

		internal static string InternalSignature
		{
			get
			{
				return "{A54F86CF-51AE-457b-90F3-1FCD0683C433}";
			}
		}

		internal static int ExternalVersion
		{
			get
			{
				return SyncStateTypeFactory.externalVersion;
			}
			set
			{
				SyncStateTypeFactory.externalVersion = value;
			}
		}

		internal static int InternalVersion
		{
			get
			{
				return 3;
			}
		}

		public static bool DoesTypeExistWithThisId(ushort typeId)
		{
			List<ICustomSerializableBuilder> builderTableFromTypeId = SyncStateTypeFactory.GetBuilderTableFromTypeId(typeId);
			ushort num = SyncStateTypeFactory.TypeIndexFromTypeId(typeId);
			return (int)num < builderTableFromTypeId.Count && builderTableFromTypeId[(int)num].TypeId == typeId;
		}

		public static SyncStateTypeFactory GetInstance()
		{
			return SyncStateTypeFactory.typeFactory;
		}

		public static bool IsTypeRegistered(ICustomSerializableBuilder typeBuilder)
		{
			return SyncStateTypeFactory.DoesTypeExistWithThisId(typeBuilder.TypeId);
		}

		public ICustomSerializable BuildObject(ushort typeId)
		{
			if (!SyncStateTypeFactory.DoesTypeExistWithThisId(typeId))
			{
				throw new CustomSerializationException(ServerStrings.ExSyncStateCorrupted("typeId " + typeId));
			}
			return SyncStateTypeFactory.GetBuilderTableFromTypeId(typeId)[(int)SyncStateTypeFactory.TypeIndexFromTypeId(typeId)].BuildObject();
		}

		public void RegisterBuilder(ICustomSerializableBuilder typeBuilder)
		{
			this.RegisterInternalBuilders();
			this.InternalRegisterBuilder(typeBuilder, 32768);
		}

		public void RegisterInternalBuilders()
		{
			if (this.initialized)
			{
				return;
			}
			lock (this.thisLock)
			{
				if (!this.initialized)
				{
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new NullableData<Int32Data, int>(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new GenericDictionaryData<StringData, string, BooleanData, bool>(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, FolderSync.ClientStateInformation>(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ServerManifestEntry>(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, ClientManifestEntry>(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new MailboxSyncItemId(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new MailboxSyncWatermark(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderStateEntry>(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new GenericDictionaryData<StoreObjectIdData, StoreObjectId, FolderManifestEntry>(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new StringData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new DateTimeData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new ByteData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new BooleanData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new Int32Data(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new UInt32Data(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new Int64Data(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new StoreObjectIdData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new ByteArrayData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new ConstStringData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new NullableDateTimeData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new ConversationIdData(), 0);
					SyncStateTypeFactory.GetInstance().InternalRegisterBuilder(new ADObjectIdData(), 0);
					this.initialized = true;
				}
			}
		}

		private static List<ICustomSerializableBuilder> GetBuilderTableFromTypeId(ushort typeId)
		{
			if (!SyncStateTypeFactory.IsExternalType(typeId))
			{
				return SyncStateTypeFactory.GetInstance().internalBuilderTable;
			}
			return SyncStateTypeFactory.GetInstance().externalBuilderTable;
		}

		private static bool IsExternalType(ushort typeId)
		{
			return (typeId & 32768) != 0;
		}

		private static ushort TypeIndexFromTypeId(ushort typeIndex)
		{
			return (ushort)(((int)typeIndex & -32769) - 1);
		}

		private bool IsTypeRegistered(Type type)
		{
			foreach (List<ICustomSerializableBuilder> list in this.tables)
			{
				foreach (ICustomSerializableBuilder customSerializableBuilder in list)
				{
					if (customSerializableBuilder.GetType() == type)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void InternalRegisterBuilder(ICustomSerializableBuilder typeBuilder, ushort typeMask)
		{
			List<ICustomSerializableBuilder> builderTableFromTypeId = SyncStateTypeFactory.GetBuilderTableFromTypeId(typeMask);
			builderTableFromTypeId.Add(typeBuilder);
			typeBuilder.TypeId = (ushort)(builderTableFromTypeId.Count | (int)typeMask);
		}

		private const string ExternalSignatureValue = "External";

		private const ushort ExternalTypeMask = 32768;

		private const string InternalSignatureValue = "{A54F86CF-51AE-457b-90F3-1FCD0683C433}";

		private const int InternalVersionValue = 3;

		private const ushort InternalTypeMask = 0;

		private readonly object thisLock = new object();

		private static int externalVersion = 1;

		private static SyncStateTypeFactory typeFactory = new SyncStateTypeFactory();

		private List<ICustomSerializableBuilder> externalBuilderTable = new List<ICustomSerializableBuilder>();

		private List<ICustomSerializableBuilder> internalBuilderTable = new List<ICustomSerializableBuilder>();

		private List<ICustomSerializableBuilder>[] tables;

		private bool initialized;
	}
}
