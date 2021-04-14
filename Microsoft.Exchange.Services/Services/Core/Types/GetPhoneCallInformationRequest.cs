using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetPhoneCallInformationRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetPhoneCallInformationRequest : BaseRequest
	{
		[XmlElement("PhoneCallId")]
		public PhoneCallId CallId
		{
			get
			{
				return this.callId;
			}
			set
			{
				this.callId = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetPhoneCallInformation(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		private PhoneCallId callId;
	}
}
