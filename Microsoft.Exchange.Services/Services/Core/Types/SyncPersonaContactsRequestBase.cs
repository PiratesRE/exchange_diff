using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract]
	public abstract class SyncPersonaContactsRequestBase : BaseRequest
	{
		[XmlElement("SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "SyncState", IsRequired = false)]
		public string SyncState { get; set; }

		[XmlElement("MaxChangesReturned", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "MaxChangesReturned", IsRequired = true)]
		public int MaxChangesReturned { get; set; }

		[XmlElement("MaxPersonas", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(Name = "MaxPersonas", IsRequired = false)]
		public int MaxPersonas { get; set; }

		[DataMember(Name = "JumpHeaderValues", IsRequired = false)]
		[XmlElement("JumpHeaderValues", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string[] JumpHeaderValues { get; set; }

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
