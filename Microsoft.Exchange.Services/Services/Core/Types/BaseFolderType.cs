using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(SearchFolderType))]
	[XmlInclude(typeof(FolderType))]
	[KnownType(typeof(FolderType))]
	[KnownType(typeof(TasksFolderType))]
	[XmlInclude(typeof(ContactsFolderType))]
	[XmlInclude(typeof(CalendarFolderType))]
	[KnownType(typeof(CalendarFolderType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(TasksFolderType))]
	[XmlInclude(typeof(SearchFolderType))]
	[KnownType(typeof(ContactsFolderType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public abstract class BaseFolderType : ServiceObject
	{
		internal static BaseFolderType CreateFromStoreObjectType(StoreObjectType storeObjectType)
		{
			if (BaseFolderType.createMethods.Member.ContainsKey(storeObjectType))
			{
				return BaseFolderType.createMethods.Member[storeObjectType]();
			}
			return BaseFolderType.createMethods.Member[StoreObjectType.Folder]();
		}

		public BaseFolderType()
		{
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public FolderId FolderId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<FolderId>(BaseFolderSchema.FolderId);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.FolderId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public FolderId ParentFolderId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<FolderId>(BaseFolderSchema.ParentFolderId);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ParentFolderId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string FolderClass
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(BaseFolderSchema.FolderClass);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.FolderClass] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string DisplayName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(BaseFolderSchema.DisplayName);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.DisplayName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public int? TotalCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(BaseFolderSchema.TotalCount);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.TotalCount] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool TotalCountSpecified
		{
			get
			{
				return base.IsSet(BaseFolderSchema.TotalCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public int? ChildFolderCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(BaseFolderSchema.ChildFolderCount);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ChildFolderCount] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool ChildFolderCountSpecified
		{
			get
			{
				return base.IsSet(BaseFolderSchema.ChildFolderCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 7)]
		[XmlElement("ExtendedProperty")]
		public ExtendedPropertyType[] ExtendedProperty
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ExtendedPropertyType[]>(BaseFolderSchema.ExtendedProperty);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ExtendedProperty] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 8)]
		public ManagedFolderInformationType ManagedFolderInformation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ManagedFolderInformationType>(BaseFolderSchema.ManagedFolderInformation);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ManagedFolderInformation] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 9)]
		public EffectiveRightsType EffectiveRights
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<EffectiveRightsType>(BaseFolderSchema.EffectiveRights);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.EffectiveRights] = value;
			}
		}

		[DataMember(Name = "DistinguishedFolderId", EmitDefaultValue = false, Order = 9)]
		public string DistinguishedFolderId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(BaseFolderSchema.DistinguishedFolderId);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.DistinguishedFolderId] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool DistinguishedFolderIdSpecified
		{
			get
			{
				return base.IsSet(BaseFolderSchema.DistinguishedFolderId);
			}
			set
			{
			}
		}

		[DataMember(Name = "PolicyTag", EmitDefaultValue = false, Order = 11)]
		public RetentionTagType PolicyTag
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RetentionTagType>(BaseFolderSchema.PolicyTag);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.PolicyTag] = value;
			}
		}

		[DataMember(Name = "ArchiveTag", EmitDefaultValue = false, Order = 12)]
		public RetentionTagType ArchiveTag
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<RetentionTagType>(BaseFolderSchema.ArchiveTag);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ArchiveTag] = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "UnClutteredViewFolderEntryId", EmitDefaultValue = false, Order = 13)]
		public FolderId UnClutteredViewFolderEntryId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<FolderId>(BaseFolderSchema.UnClutteredViewFolderEntryId);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.UnClutteredViewFolderEntryId] = value;
			}
		}

		[DataMember(Name = "ClutteredViewFolderEntryId", EmitDefaultValue = false, Order = 14)]
		[XmlIgnore]
		public FolderId ClutteredViewFolderEntryId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<FolderId>(BaseFolderSchema.ClutteredViewFolderEntryId);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ClutteredViewFolderEntryId] = value;
			}
		}

		[DataMember(Name = "ClutterCount", EmitDefaultValue = false, Order = 15)]
		[XmlIgnore]
		public int? ClutterCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(BaseFolderSchema.ClutterCount);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ClutterCount] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ClutterCountSpecified
		{
			get
			{
				return base.IsSet(BaseFolderSchema.ClutterCount);
			}
			set
			{
			}
		}

		[DataMember(Name = "UnreadClutterCount", EmitDefaultValue = false, Order = 16)]
		[XmlIgnore]
		public int? UnreadClutterCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(BaseFolderSchema.UnreadClutterCount);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.UnreadClutterCount] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool UnreadClutterCountSpecified
		{
			get
			{
				return base.IsSet(BaseFolderSchema.UnreadClutterCount);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 17)]
		public string[] ReplicaList
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(BaseFolderSchema.ReplicaList);
			}
			set
			{
				base.PropertyBag[BaseFolderSchema.ReplicaList] = value;
			}
		}

		internal override void AddExtendedPropertyValue(ExtendedPropertyType extendedPropertyToAdd)
		{
			ExtendedPropertyType[] extendedProperty = this.ExtendedProperty;
			int num = (extendedProperty == null) ? 0 : extendedProperty.Length;
			ExtendedPropertyType[] array = new ExtendedPropertyType[num + 1];
			if (num > 0)
			{
				Array.Copy(extendedProperty, array, num);
			}
			array[num] = extendedPropertyToAdd;
			this.ExtendedProperty = array;
		}

		private static LazyMember<Dictionary<StoreObjectType, Func<BaseFolderType>>> createMethods = new LazyMember<Dictionary<StoreObjectType, Func<BaseFolderType>>>(delegate()
		{
			Dictionary<StoreObjectType, Func<BaseFolderType>> dictionary = new Dictionary<StoreObjectType, Func<BaseFolderType>>();
			dictionary.Add(StoreObjectType.Folder, () => new FolderType());
			dictionary.Add(StoreObjectType.CalendarFolder, () => new CalendarFolderType());
			dictionary.Add(StoreObjectType.ContactsFolder, () => new ContactsFolderType());
			dictionary.Add(StoreObjectType.JournalFolder, () => new FolderType());
			dictionary.Add(StoreObjectType.NotesFolder, () => new FolderType());
			dictionary.Add(StoreObjectType.OutlookSearchFolder, () => new SearchFolderType());
			dictionary.Add(StoreObjectType.SearchFolder, () => new SearchFolderType());
			dictionary.Add(StoreObjectType.TasksFolder, () => new TasksFolderType());
			return dictionary;
		});
	}
}
