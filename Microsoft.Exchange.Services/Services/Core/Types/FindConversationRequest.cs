using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FindConversationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[KnownType(typeof(IndexedPageView))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(SeekToConditionPageView))]
	[Serializable]
	public class FindConversationRequest : BaseRequest
	{
		[XmlElement("SeekToConditionPageItemView", typeof(SeekToConditionPageView))]
		[DataMember(Name = "Paging", IsRequired = false)]
		[XmlElement("IndexedPageItemView", typeof(IndexedPageView))]
		public BasePagingType Paging { get; set; }

		[DataMember(Name = "SortOrder", IsRequired = false)]
		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SortResults[] SortOrder { get; set; }

		[DataMember(Name = "ParentFolderId", IsRequired = true)]
		[XmlElement("ParentFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public TargetFolderId ParentFolderId { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool MailboxScopeSpecified { get; set; }

		[IgnoreDataMember]
		[XmlElement]
		public MailboxSearchLocation MailboxScope
		{
			get
			{
				return this.mailboxScope;
			}
			set
			{
				this.mailboxScope = value;
				this.MailboxScopeSpecified = true;
			}
		}

		[DataMember(Name = "MailboxScope", IsRequired = false)]
		[XmlIgnore]
		public string MailboxScopeString
		{
			get
			{
				if (!this.MailboxScopeSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<MailboxSearchLocation>(this.mailboxScope);
			}
			set
			{
				this.MailboxScope = EnumUtilities.Parse<MailboxSearchLocation>(value);
			}
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public ConversationQueryTraversal Traversal
		{
			get
			{
				return this.traversal;
			}
			set
			{
				this.traversal = value;
				this.TraversalSpecified = true;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool TraversalSpecified { get; set; }

		[DataMember(Name = "Traversal", IsRequired = false, EmitDefaultValue = false)]
		[XmlIgnore]
		public string TraversalString
		{
			get
			{
				if (!this.TraversalSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<ConversationQueryTraversal>(this.Traversal);
			}
			set
			{
				this.Traversal = EnumUtilities.Parse<ConversationQueryTraversal>(value);
			}
		}

		[IgnoreDataMember]
		[XmlAttribute]
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

		[DataMember(Name = "ViewFilter", IsRequired = false)]
		[XmlIgnore]
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

		[DataMember(Name = "QueryString", IsRequired = false)]
		[XmlElement("QueryString", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public QueryStringType QueryString { get; set; }

		[XmlElement]
		[DataMember(Name = "ConversationShape", IsRequired = false)]
		public ConversationResponseShape ConversationShape { get; set; }

		[DataMember(Name = "ShapeName", IsRequired = false)]
		[XmlIgnore]
		public string ShapeName { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SearchFolderIdentity", IsRequired = false)]
		public string SearchFolderIdentity { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SearchFolderId", IsRequired = false)]
		public BaseFolderId SearchFolderId { get; set; }

		[DataMember(Name = "RefinerRestriction", IsRequired = false)]
		[XmlIgnore]
		public RestrictionType RefinerRestriction { get; set; }

		[DataMember(Name = "FromFilter", IsRequired = false)]
		[XmlIgnore]
		public string FromFilter { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
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

		internal Guid[] MailboxGuids { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new FindConversation(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForFolderId(callContext, this.ParentFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		private ConversationQueryTraversal traversal;

		private ViewFilter viewFilter;

		private ClutterFilter clutterFilter;

		private MailboxSearchLocation mailboxScope;
	}
}
