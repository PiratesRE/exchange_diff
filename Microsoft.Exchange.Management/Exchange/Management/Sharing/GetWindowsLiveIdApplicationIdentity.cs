using System;
using System.Management.Automation;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.AppIdServices;
using Microsoft.Exchange.Management.LiveServices;
using Microsoft.Exchange.Management.LiveServicesHelper;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Sharing
{
	[Cmdlet("Get", "WindowsLiveIdApplicationIdentity", DefaultParameterSetName = "AppID")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class GetWindowsLiveIdApplicationIdentity : WindowsLiveIdTask
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Uri", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public string Uri
		{
			get
			{
				return (string)base.Fields["Uri"];
			}
			set
			{
				base.Fields["Uri"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AppID", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public string AppId
		{
			get
			{
				return (string)base.Fields["AppID"];
			}
			set
			{
				base.Fields["AppID"] = value;
			}
		}

		protected override ConfigurableObject ParseResponse(LiveIdInstanceType liveIdInstanceType, XmlDocument xmlResponse)
		{
			return WindowsLiveIdApplicationIdentity.ParseXml(liveIdInstanceType, xmlResponse);
		}

		protected override XmlDocument WindowsLiveIdMethod(LiveIdInstanceType liveIdInstanceType)
		{
			XmlDocument xmlDocument = null;
			using (AppIDServiceAPISoapServer appIDServiceAPISoapServer = LiveServicesHelper.ConnectToAppIDService(liveIdInstanceType))
			{
				base.WriteVerbose(Strings.AppIDServiceUrl(appIDServiceAPISoapServer.Url.ToString()));
				if (!string.IsNullOrEmpty(this.Uri))
				{
					new Uri(this.Uri, UriKind.RelativeOrAbsolute);
					string text = string.Format(GetWindowsLiveIdApplicationIdentity.AppIDFindTemplate, this.Uri);
					string xml = appIDServiceAPISoapServer.FindApplication(text);
					XmlDocument xmlDocument2 = new SafeXmlDocument();
					xmlDocument2.LoadXml(xml);
					XmlNode xmlNode = xmlDocument2.SelectSingleNode("AppidData/Application/ID");
					if (xmlNode == null)
					{
						base.WriteVerbose(Strings.AppIdElementIsEmpty);
						throw new LiveServicesException(Strings.AppIdElementIsEmpty);
					}
					base.WriteVerbose(Strings.FoundAppId(xmlNode.InnerText));
					this.AppId = xmlNode.InnerText;
				}
				if (!string.IsNullOrEmpty(this.AppId))
				{
					xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml(appIDServiceAPISoapServer.GetApplicationEntity(new tagPASSID(), this.AppId));
				}
			}
			return xmlDocument;
		}

		private const string UriParameterSet = "Uri";

		private const string AppIDParameterSet = "AppID";

		private static readonly string AppIDFindTemplate = "<SearchCriteria><Object name=\"Application\"><Property name=\"URI\">{0}</Property></Object></SearchCriteria>";
	}
}
