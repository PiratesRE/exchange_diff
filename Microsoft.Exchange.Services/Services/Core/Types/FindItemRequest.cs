using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FindItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(SeekToConditionPageView))]
	[KnownType(typeof(GroupByType))]
	[KnownType(typeof(DistinguishedGroupByType))]
	[KnownType(typeof(CalendarPageView))]
	[KnownType(typeof(ContactsPageView))]
	[KnownType(typeof(IndexedPageView))]
	[KnownType(typeof(FractionalPageView))]
	[Serializable]
	public class FindItemRequest : BaseRequest
	{
		[DataMember(Name = "ItemShape", IsRequired = true, Order = 0)]
		public ItemResponseShape ItemShape { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ShapeName", IsRequired = false)]
		public string ShapeName { get; set; }

		[XmlElement("SeekToConditionPageItemView", typeof(SeekToConditionPageView))]
		[XmlElement("CalendarView", typeof(CalendarPageView))]
		[DataMember(Name = "Paging", IsRequired = false, Order = 1)]
		[XmlElement("IndexedPageItemView", typeof(IndexedPageView))]
		[XmlElement("FractionalPageItemView", typeof(FractionalPageView))]
		[XmlElement("ContactsView", typeof(ContactsPageView))]
		public BasePagingType Paging { get; set; }

		[DataMember(Name = "Grouping", IsRequired = false, Order = 2)]
		[XmlElement("DistinguishedGroupBy", typeof(DistinguishedGroupByType))]
		[XmlElement("GroupBy", typeof(GroupByType))]
		public BaseGroupByType Grouping
		{
			get
			{
				if (this.grouping == null)
				{
					this.grouping = new NoGrouping();
				}
				return this.grouping;
			}
			set
			{
				this.grouping = value;
			}
		}

		[XmlElement("Restriction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "Restriction", IsRequired = false, Order = 3)]
		public RestrictionType Restriction { get; set; }

		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "SortOrder", IsRequired = false, Order = 4)]
		public SortResults[] SortOrder { get; set; }

		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("ParentFolderIds")]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "ParentFolderIds", IsRequired = true, Order = 5)]
		public BaseFolderId[] ParentFolderIds { get; set; }

		[DataMember(Name = "QueryString", IsRequired = false, Order = 6)]
		public QueryStringType QueryString { get; set; }

		[IgnoreDataMember]
		[XmlAttribute]
		public ItemQueryTraversal Traversal { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Traversal", IsRequired = true, Order = 7)]
		public string TraversalString
		{
			get
			{
				return EnumUtilities.ToString<ItemQueryTraversal>(this.Traversal);
			}
			set
			{
				this.Traversal = EnumUtilities.Parse<ItemQueryTraversal>(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public ViewFilter ViewFilter
		{
			get
			{
				return this.viewFilter;
			}
			set
			{
				this.viewFilter = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ViewFilter", IsRequired = false, Order = 8)]
		public string ViewFilterString
		{
			get
			{
				return EnumUtilities.ToString<ViewFilter>(this.ViewFilter);
			}
			set
			{
				this.ViewFilter = EnumUtilities.Parse<ViewFilter>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "SearchFolderIdentity", IsRequired = false)]
		public string SearchFolderIdentity { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SearchFolderId", IsRequired = false)]
		public BaseFolderId SearchFolderId { get; set; }

		[DataMember(Name = "RefinerRestriction", IsRequired = false)]
		[XmlIgnore]
		public RestrictionType RefinerRestriction { get; set; }

		[DataMember(Name = "IsWarmUpSearch", IsRequired = false)]
		[XmlIgnore]
		public bool IsWarmUpSearch { get; set; }

		[DataMember(Name = "FromFilter", IsRequired = false)]
		[XmlIgnore]
		public string FromFilter { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public ClutterFilter ClutterFilter
		{
			get
			{
				return this.clutterFilter;
			}
			set
			{
				this.clutterFilter = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ClutterFilter", IsRequired = false)]
		public string ClutterFilterString
		{
			get
			{
				return EnumUtilities.ToString<ClutterFilter>(this.ClutterFilter);
			}
			set
			{
				this.ClutterFilter = EnumUtilities.Parse<ClutterFilter>(value);
			}
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			return new List<ServiceObjectId>(this.ParentFolderIds);
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new FindItem(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.ParentFolderIds == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderIdList(callContext, this.ParentFolderIds);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.ParentFolderIds == null || this.ParentFolderIds.Length < taskStep)
			{
				return null;
			}
			return base.GetResourceKeysForFolderId(false, callContext, this.ParentFolderIds[taskStep]);
		}

		internal const string ParentFolderIdsElementName = "ParentFolderIds";

		private BaseGroupByType grouping;

		private ViewFilter viewFilter;

		private ClutterFilter clutterFilter;
	}
}
