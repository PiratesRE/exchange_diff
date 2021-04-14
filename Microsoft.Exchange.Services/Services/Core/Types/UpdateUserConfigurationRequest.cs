using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("UpdateUserConfigurationRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UpdateUserConfigurationRequest : BaseRequest
	{
		[DataMember(IsRequired = true)]
		[XmlElement]
		public ServiceUserConfiguration UserConfiguration { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateUserConfiguration(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.UserConfiguration == null || this.UserConfiguration.UserConfigurationName == null || this.UserConfiguration.UserConfigurationName.BaseFolderId == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderId(callContext, this.UserConfiguration.UserConfigurationName.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}
	}
}
