using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidUserInfoException : ServicePermanentExceptionWithXmlNodeArray
	{
		public InvalidUserInfoException(BasePermissionType permissionElement) : base(CoreResources.IDs.ErrorInvalidUserInfo)
		{
			this.SerializePermissionElement(permissionElement);
		}

		public InvalidUserInfoException(BasePermissionType permissionElement, Exception innerException) : base(CoreResources.IDs.ErrorInvalidUserInfo, innerException)
		{
			this.SerializePermissionElement(permissionElement);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}

		public InvalidUserInfoException(XmlElement permissionElement) : base(CoreResources.IDs.ErrorInvalidUserInfo)
		{
			this.SetupXmlNodeArray(permissionElement);
		}

		public InvalidUserInfoException(XmlElement permissionElement, Exception innerException) : base(CoreResources.IDs.ErrorInvalidUserInfo, innerException)
		{
			this.SetupXmlNodeArray(permissionElement);
		}

		private void SerializePermissionElement(BasePermissionType permissionElement)
		{
			this.SetupXmlNodeArray(base.SerializeObjectToXml(permissionElement, InvalidUserInfoException.basePermissionTypeSerializer));
		}

		private void SetupXmlNodeArray(XmlElement invalidPermission)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlElement parentElement = ServiceXml.CreateElement(safeXmlDocument, "MessageXml", "http://schemas.microsoft.com/exchange/services/2006/types");
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "InvalidPermission", "http://schemas.microsoft.com/exchange/services/2006/types");
			xmlElement.AppendChild(safeXmlDocument.ImportNode(invalidPermission, true));
			base.NodeArray.Nodes.Add(xmlElement);
		}

		private static readonly SafeXmlSerializer basePermissionTypeSerializer = new SafeXmlSerializer(typeof(BasePermissionType));
	}
}
