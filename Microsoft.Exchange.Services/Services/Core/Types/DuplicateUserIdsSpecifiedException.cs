using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class DuplicateUserIdsSpecifiedException : ServicePermanentExceptionWithXmlNodeArray
	{
		public DuplicateUserIdsSpecifiedException(List<List<BasePermissionType>> duplicatePermissionsLists) : base((CoreResources.IDs)4289255106U)
		{
			this.Initialize(duplicatePermissionsLists);
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}

		public DuplicateUserIdsSpecifiedException(List<List<XmlElement>> duplicatePermissionsLists) : base((CoreResources.IDs)4289255106U)
		{
			this.Initialize(duplicatePermissionsLists);
		}

		private void Initialize(List<List<BasePermissionType>> duplicatePermissionsLists)
		{
			List<List<XmlElement>> list = new List<List<XmlElement>>();
			foreach (List<BasePermissionType> list2 in duplicatePermissionsLists)
			{
				List<XmlElement> list3 = new List<XmlElement>();
				foreach (BasePermissionType serializationObject in list2)
				{
					list3.Add(base.SerializeObjectToXml(serializationObject, DuplicateUserIdsSpecifiedException.basePermissionTypeSerializer));
				}
				list.Add(list3);
			}
			this.Initialize(list);
		}

		private void Initialize(List<List<XmlElement>> duplicatePermissionsLists)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlElement parentElement = ServiceXml.CreateElement(safeXmlDocument, "MessageXml", "http://schemas.microsoft.com/exchange/services/2006/types");
			foreach (List<XmlElement> list in duplicatePermissionsLists)
			{
				XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "DuplicateUserIds", "http://schemas.microsoft.com/exchange/services/2006/types");
				foreach (XmlElement node in list)
				{
					xmlElement.AppendChild(safeXmlDocument.ImportNode(node, true));
				}
				base.NodeArray.Nodes.Add(xmlElement);
			}
		}

		private static SafeXmlSerializer basePermissionTypeSerializer = new SafeXmlSerializer(typeof(BasePermissionType));
	}
}
