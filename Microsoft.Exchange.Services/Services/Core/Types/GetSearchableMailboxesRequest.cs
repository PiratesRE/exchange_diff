using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetSearchableMailboxesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "GetSearchableMailboxesRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetSearchableMailboxesRequest : BaseRequest
	{
		[DataMember(Name = "SearchFilter", IsRequired = false)]
		[XmlElement("SearchFilter")]
		public string SearchFilter
		{
			get
			{
				return this.searchFilter;
			}
			set
			{
				this.searchFilter = value;
			}
		}

		[XmlElement("ExpandGroupMembership")]
		[DataMember(Name = "ExpandGroupMembership", IsRequired = false)]
		public bool ExpandGroupMembership
		{
			get
			{
				return this.expandGroupMembership;
			}
			set
			{
				this.expandGroupMembership = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetSearchableMailboxes(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return null;
		}

		private string searchFilter;

		private bool expandGroupMembership;
	}
}
