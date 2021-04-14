using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ResetUMMailboxType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ResetUMMailboxRequest : BaseRequest
	{
		[DataMember(Name = "KeepProperties")]
		[XmlElement("KeepProperties")]
		public bool KeepProperties { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new ResetUMMailbox(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
