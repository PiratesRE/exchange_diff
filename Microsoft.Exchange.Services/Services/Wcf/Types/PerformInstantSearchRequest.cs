using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PerformInstantSearchRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class PerformInstantSearchRequest : BaseRequest
	{
		[DataMember(IsRequired = false)]
		public string DeviceId { get; set; }

		[DataMember(IsRequired = false)]
		public string ApplicationId { get; set; }

		[DataMember(IsRequired = true)]
		public string SearchSessionId { get; set; }

		[DataMember(IsRequired = true)]
		public InstantSearchItemType ItemType { get; set; }

		[DataMember(IsRequired = true)]
		public QueryOptionsType QueryOptions { get; set; }

		[DataMember(IsRequired = true)]
		public long SearchRequestId { get; set; }

		[DataMember(IsRequired = true)]
		public string KqlQuery { get; set; }

		[DataMember(IsRequired = true)]
		public FolderId[] FolderScope { get; set; }

		[DataMember]
		public int MaxSuggestionsCount { get; set; }

		[DataMember]
		public int MaximumResultCount { get; set; }

		[Deprecated(ExchangeVersionType.V2_4)]
		[DataMember]
		[XmlIgnore]
		public PropertyPath[] RequestedRefiners { get; set; }

		[DataMember]
		public RefinementFilterType RefinementFilter { get; set; }

		[DataMember]
		public SuggestionSourceType SuggestionSources { get; set; }

		[DataMember]
		public RestrictionType DateRestriction { get; set; }

		[DataMember]
		public bool IsDeepTraversal { get; set; }

		[DataMember]
		public bool WaitOnSearchResults { get; set; }

		public bool IsWarmUpRequest
		{
			get
			{
				return string.IsNullOrEmpty(this.KqlQuery) && (this.QueryOptions & QueryOptionsType.Results) == QueryOptionsType.Results;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new PerformInstantSearch(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		internal override void Validate()
		{
			base.Validate();
		}
	}
}
