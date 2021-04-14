using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(IndexedPageView))]
	[XmlType(TypeName = "FindPeopleType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class FindPeopleRequest : BaseRequest
	{
		[DataMember(Name = "PersonaShape", IsRequired = false)]
		public PersonaResponseShape PersonaShape { get; set; }

		[DataMember(Name = "IndexedPageItemView", IsRequired = true)]
		[XmlElement("IndexedPageItemView", typeof(IndexedPageView))]
		public BasePagingType Paging { get; set; }

		[XmlArrayItem("FieldOrder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "SortOrder", IsRequired = false)]
		public SortResults[] SortOrder { get; set; }

		[DataMember(Name = "ParentFolderId", IsRequired = false)]
		[XmlElement("ParentFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public TargetFolderId ParentFolderId { get; set; }

		[XmlElement("Restriction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "Restriction", IsRequired = false)]
		public RestrictionType Restriction { get; set; }

		[XmlElement("AggregationRestriction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "AggregationRestriction", IsRequired = false)]
		public RestrictionType AggregationRestriction { get; set; }

		[XmlElement("QueryString", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "QueryString", IsRequired = false)]
		public string QueryString { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ShouldResolveOneOffEmailAddress", IsRequired = false)]
		public bool ShouldResolveOneOffEmailAddress { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new FindPeople(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.ParentFolderId == null)
			{
				return IdConverter.GetServerInfoForCallContext(callContext);
			}
			return BaseRequest.GetServerInfoForFolderId(callContext, this.ParentFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
