using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("DeleteUserConfigurationRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class DeleteUserConfigurationRequest : BaseRequest
	{
		[XmlElement("UserConfigurationName")]
		[DataMember]
		public UserConfigurationNameType UserConfigurationName { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new DeleteUserConfiguration(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.UserConfigurationName == null || this.UserConfigurationName.BaseFolderId == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForFolderId(callContext, this.UserConfigurationName.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}
	}
}
