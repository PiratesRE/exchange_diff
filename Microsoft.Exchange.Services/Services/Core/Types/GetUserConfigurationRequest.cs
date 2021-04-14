using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUserConfigurationRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetUserConfigurationRequest : BaseRequest
	{
		[DataMember(IsRequired = true)]
		[XmlElement("UserConfigurationName")]
		public UserConfigurationNameType UserConfigurationName { get; set; }

		[IgnoreDataMember]
		[XmlElement]
		public UserConfigurationProperties UserConfigurationProperties { get; set; }

		[XmlIgnore]
		[DataMember(Name = "UserConfigurationProperties", IsRequired = true)]
		public string UserConfigurationPropertiesString
		{
			get
			{
				return EnumUtilities.ToString<UserConfigurationProperties>(this.UserConfigurationProperties);
			}
			set
			{
				this.UserConfigurationProperties = EnumUtilities.Parse<UserConfigurationProperties>(value);
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUserConfiguration(callContext, this);
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
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
