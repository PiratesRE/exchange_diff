using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetDiscoverySearchConfigurationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "GetDiscoverySearchConfigurationRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetDiscoverySearchConfigurationRequest : BaseRequest
	{
		[XmlElement("SearchId")]
		[DataMember(Name = "SearchId", IsRequired = true)]
		public string SearchId
		{
			get
			{
				return this.searchId;
			}
			set
			{
				this.searchId = value;
			}
		}

		[DataMember(Name = "ExpandGroupMembership", IsRequired = false)]
		[XmlElement("ExpandGroupMembership")]
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

		[DataMember(Name = "InPlaceHoldConfigurationOnly", IsRequired = false)]
		[XmlElement("InPlaceHoldConfigurationOnly")]
		public bool InPlaceHoldConfigurationOnly
		{
			get
			{
				return this.inPlaceHoldConfigurationOnly;
			}
			set
			{
				this.inPlaceHoldConfigurationOnly = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetDiscoverySearchConfiguration(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return null;
		}

		private string searchId;

		private bool expandGroupMembership;

		private bool inPlaceHoldConfigurationOnly;
	}
}
