using System;
using System.Management.Automation;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.LiveServices;
using Microsoft.Exchange.Management.NamespaceServices;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Sharing
{
	[Cmdlet("Get", "WindowsLiveIdNamespace", DefaultParameterSetName = "Namespace")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class GetWindowsLiveIdNamespace : WindowsLiveIdTask
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Namespace", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public string Namespace
		{
			get
			{
				return (string)base.Fields["Namespace"];
			}
			set
			{
				base.Fields["Namespace"] = value;
			}
		}

		protected override ConfigurableObject ParseResponse(LiveIdInstanceType liveIdInstanceType, XmlDocument xmlResponse)
		{
			return WindowsLiveIdNamespace.ParseXml(liveIdInstanceType, xmlResponse);
		}

		protected override XmlDocument WindowsLiveIdMethod(LiveIdInstanceType liveIdInstanceType)
		{
			XmlDocument xmlDocument = null;
			using (NamespaceServiceAPISoapServer namespaceServiceAPISoapServer = LiveServicesHelper.ConnectToNamespaceService(liveIdInstanceType))
			{
				base.WriteVerbose(Strings.NamespaceServiceUrl(namespaceServiceAPISoapServer.Url.ToString()));
				string namespaceAttributes = namespaceServiceAPISoapServer.GetNamespaceAttributes(this.Namespace, string.Empty);
				xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(namespaceAttributes);
			}
			return xmlDocument;
		}

		private const string NamespaceParameterSet = "Namespace";
	}
}
