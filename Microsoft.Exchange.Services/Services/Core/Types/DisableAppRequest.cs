using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DisableAppRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DisableAppRequest : BaseRequest
	{
		[XmlElement]
		public string ID { get; set; }

		[IgnoreDataMember]
		[XmlElement]
		public DisableReasonType DisableReason { get; set; }

		[XmlIgnore]
		[DataMember(Name = "DisableReason", IsRequired = true)]
		public string DisableReasonString
		{
			get
			{
				return EnumUtilities.ToString<DisableReasonType>(this.DisableReason);
			}
			set
			{
				this.DisableReason = EnumUtilities.Parse<DisableReasonType>(value);
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new DisableApp(callContext, this);
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
