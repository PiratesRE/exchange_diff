using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class FolderRec
	{
		public FolderRec()
		{
		}

		[DataMember(IsRequired = true)]
		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
			set
			{
				this.entryId = value;
			}
		}

		[DataMember(IsRequired = true)]
		public byte[] ParentId
		{
			get
			{
				return this.parentId;
			}
			set
			{
				this.parentId = value;
			}
		}

		[DataMember(IsRequired = true)]
		public string FolderName
		{
			get
			{
				return this.folderName;
			}
			set
			{
				this.folderName = value;
			}
		}

		[DataMember(IsRequired = true, Name = "FolderType")]
		public int FolderTypeValue
		{
			get
			{
				return this.folderType;
			}
			set
			{
				this.folderType = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string FolderClass
		{
			get
			{
				return this.folderClass;
			}
			set
			{
				this.folderClass = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public DateTime LastModifyTimestamp
		{
			get
			{
				return this.lastModifyTimestamp;
			}
			set
			{
				this.lastModifyTimestamp = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool IsGhosted { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public PropValueData[] AdditionalProps
		{
			get
			{
				return this.additionalProps;
			}
			set
			{
				this.additionalProps = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Name = "PromotedProperties")]
		public int[] PromotedPropertiesList
		{
			get
			{
				return this.promotedProperties;
			}
			set
			{
				this.promotedProperties = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public SortOrderData[] Views
		{
			get
			{
				return this.views;
			}
			set
			{
				this.views = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public ICSViewData[] ICSViews
		{
			get
			{
				return this.icsViews;
			}
			set
			{
				this.icsViews = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public RestrictionData[] Restrictions
		{
			get
			{
				return this.restrictions;
			}
			set
			{
				this.restrictions = value;
			}
		}

		public FolderRec(byte[] entryId, byte[] parentId, FolderType folderType, string folderName, DateTime lastModifyTimestamp, PropValueData[] additionalProps) : this(entryId, parentId, folderType, null, folderName, lastModifyTimestamp, additionalProps)
		{
		}

		public FolderRec(byte[] entryId, byte[] parentId, FolderType folderType, string folderClass, string folderName, DateTime lastModifyTimestamp, PropValueData[] additionalProps)
		{
			if (CommonUtils.IsSameEntryId(entryId, parentId))
			{
				parentId = null;
			}
			this.entryId = entryId;
			this.parentId = parentId;
			this.folderType = (int)folderType;
			this.folderClass = folderClass;
			this.folderName = folderName;
			this.lastModifyTimestamp = lastModifyTimestamp;
			this.additionalProps = additionalProps;
		}

		public FolderRec(FolderRec folderRec)
		{
			this.CopyFrom(folderRec);
		}

		public FolderType FolderType
		{
			get
			{
				return (FolderType)this.folderType;
			}
			set
			{
				this.folderType = (int)value;
			}
		}

		public DateTime LocalCommitTimeMax
		{
			get
			{
				object obj = this[PropTag.LocalCommitTimeMax];
				if (obj == null)
				{
					return DateTime.MinValue;
				}
				return (DateTime)obj;
			}
		}

		public int DeletedCountTotal
		{
			get
			{
				object obj = this[PropTag.DeletedCountTotal];
				if (obj == null)
				{
					return 0;
				}
				return (int)obj;
			}
		}

		public object this[PropTag additionalPtag]
		{
			get
			{
				if (this.additionalProps != null)
				{
					foreach (PropValueData propValueData in this.additionalProps)
					{
						if (propValueData.PropTag == (int)additionalPtag)
						{
							return propValueData.Value;
						}
					}
				}
				return null;
			}
		}

		public static FolderRec Create(StoreSession storageSession, NativeStorePropertyDefinition[] definitions, object[] values)
		{
			PropValue[] array = new PropValue[definitions.Length];
			ICollection<uint> collection = PropertyTagCache.Cache.PropertyTagsFromPropertyDefinitions(storageSession, definitions);
			byte[] array2 = null;
			int num = 0;
			foreach (uint num2 in collection)
			{
				if (num2 == 268370178U)
				{
					array2 = storageSession.IdConverter.GetLongTermIdFromId(storageSession.IdConverter.GetFidFromId(StoreObjectId.FromProviderSpecificId((byte[])values[num])));
				}
				num++;
			}
			num = 0;
			foreach (uint num3 in collection)
			{
				object obj = values[num];
				PropTag propTag = (PropTag)num3;
				if (propTag == PropTag.LTID)
				{
					obj = array2;
				}
				if (obj == null)
				{
					propTag = propTag.ChangePropType(PropType.Null);
				}
				else if (obj is PropertyError)
				{
					propTag = propTag.ChangePropType(PropType.Error);
					obj = (int)((PropertyError)obj).PropertyErrorCode;
				}
				else if (obj is ExDateTime)
				{
					obj = (DateTime)((ExDateTime)obj);
				}
				array[num] = new PropValue(propTag, obj);
				num++;
			}
			return FolderRec.Create(storageSession, array);
		}

		public static FolderRec Create(PropValue[] pva)
		{
			return FolderRec.Create(null, pva);
		}

		public static FolderRec Create(StoreSession storageSession, PropValue[] pva)
		{
			byte[] array = null;
			byte[] array2 = null;
			FolderType folderType = FolderType.Generic;
			string text = null;
			DateTime dateTime = DateTime.MinValue;
			List<PropValueData> list = new List<PropValueData>();
			foreach (PropValue native in pva)
			{
				if (!native.IsNull() && !native.IsError())
				{
					PropTag propTag = native.PropTag;
					if (propTag <= PropTag.EntryId)
					{
						if (propTag == PropTag.ParentEntryId)
						{
							array2 = native.GetBytes();
							goto IL_CD;
						}
						if (propTag == PropTag.EntryId)
						{
							array = native.GetBytes();
							goto IL_CD;
						}
					}
					else
					{
						if (propTag == PropTag.DisplayName)
						{
							text = native.GetString();
							goto IL_CD;
						}
						if (propTag == PropTag.LastModificationTime)
						{
							dateTime = native.GetDateTime();
							goto IL_CD;
						}
						if (propTag == PropTag.FolderType)
						{
							folderType = (FolderType)native.GetInt();
							goto IL_CD;
						}
					}
					list.Add(DataConverter<PropValueConverter, PropValue, PropValueData>.GetData(native));
				}
				IL_CD:;
			}
			if (array != null)
			{
				FolderRec folderRec = new FolderRec(array, array2, folderType, text, dateTime, (list.Count > 0) ? list.ToArray() : null);
				if (storageSession != null && folderRec[PropTag.ReplicaList] != null)
				{
					folderRec.IsGhosted = !CoreFolder.IsContentAvailable(storageSession, CoreFolder.GetContentMailboxInfo(ReplicaListProperty.GetStringArrayFromBytes((byte[])folderRec[PropTag.ReplicaList])));
				}
				return folderRec;
			}
			return null;
		}

		public static FolderRec Create(MapiFolder folder, PropTag[] additionalPtagsToLoad)
		{
			PropTag[] array;
			if (additionalPtagsToLoad == null)
			{
				array = FolderRec.PtagsToLoad;
			}
			else
			{
				List<PropTag> list = new List<PropTag>();
				list.AddRange(FolderRec.PtagsToLoad);
				list.AddRange(additionalPtagsToLoad);
				array = list.ToArray();
			}
			PropValue[] props = folder.GetProps(array);
			byte[] array2 = null;
			for (int i = 0; i < array.Length; i++)
			{
				PropTag propTag = array[i];
				PropTag propTag2 = propTag;
				if (propTag2 != PropTag.EntryId)
				{
					if (propTag2 == PropTag.LTID)
					{
						props[i] = new PropValue(PropTag.LTID, folder.MapiStore.GlobalIdFromId(folder.MapiStore.GetFidFromEntryId(array2)));
					}
				}
				else
				{
					array2 = (byte[])props[i].Value;
				}
			}
			return FolderRec.Create(props);
		}

		public PropTag[] GetPromotedProperties()
		{
			return DataConverter<PropTagConverter, PropTag, int>.GetNative(this.promotedProperties);
		}

		public void SetPromotedProperties(PropTag[] properties)
		{
			this.promotedProperties = DataConverter<PropTagConverter, PropTag, int>.GetData(properties);
		}

		public void CopyFrom(FolderRec sourceRec)
		{
			this.entryId = sourceRec.EntryId;
			this.parentId = sourceRec.ParentId;
			this.folderName = sourceRec.FolderName;
			this.folderType = sourceRec.folderType;
			this.folderClass = sourceRec.FolderClass;
			this.lastModifyTimestamp = sourceRec.LastModifyTimestamp;
			this.IsGhosted = sourceRec.IsGhosted;
			this.additionalProps = sourceRec.AdditionalProps;
			this.promotedProperties = sourceRec.promotedProperties;
			this.views = sourceRec.Views;
			this.icsViews = sourceRec.ICSViews;
			this.restrictions = sourceRec.Restrictions;
		}

		public override string ToString()
		{
			return string.Format("{0}: EntryID: {1}, ParentID: {2}, Type: {3}", new object[]
			{
				this.FolderName,
				TraceUtils.DumpEntryId(this.EntryId),
				TraceUtils.DumpEntryId(this.ParentId),
				this.FolderType
			});
		}

		private byte[] entryId;

		private byte[] parentId;

		private string folderName;

		private int folderType;

		private string folderClass;

		private DateTime lastModifyTimestamp;

		private PropValueData[] additionalProps;

		private int[] promotedProperties;

		private SortOrderData[] views;

		private ICSViewData[] icsViews;

		private RestrictionData[] restrictions;

		public static readonly PropTag[] PtagsToLoad = new PropTag[]
		{
			PropTag.EntryId,
			PropTag.ParentEntryId,
			PropTag.FolderType,
			PropTag.DisplayName,
			PropTag.LastModificationTime
		};
	}
}
