using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidPermissionSettingsException<PermissionType> : ServicePermanentExceptionWithXmlNodeArray where PermissionType : BasePermissionType
	{
		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}

		public InvalidPermissionSettingsException(PermissionInformationBase<PermissionType> permissionInformation) : base(CoreResources.IDs.ErrorInvalidPermissionSettings)
		{
			this.AddXmlElement(base.SerializeObjectToXml(permissionInformation, InvalidPermissionSettingsException<PermissionType>.permissionInformationBaseSerializer));
		}

		private void AddXmlElement(XmlElement permissionInformation)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlElement parentElement = ServiceXml.CreateElement(safeXmlDocument, "MessageXml", "http://schemas.microsoft.com/exchange/services/2006/types");
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "InvalidPermission", "http://schemas.microsoft.com/exchange/services/2006/types");
			xmlElement.AppendChild(safeXmlDocument.ImportNode(permissionInformation, true));
			base.NodeArray.Nodes.Add(xmlElement);
		}

		private static readonly SafeXmlSerializer permissionInformationBaseSerializer = new SafeXmlSerializer(typeof(PermissionInformationBase<PermissionType>));
	}
}
