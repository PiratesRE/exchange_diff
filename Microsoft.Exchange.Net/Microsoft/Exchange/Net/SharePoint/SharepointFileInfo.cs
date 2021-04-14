using System;
using System.Xml;

namespace Microsoft.Exchange.Net.SharePoint
{
	public class SharepointFileInfo
	{
		private SharepointFileInfo()
		{
		}

		public string IsFolderRaw { get; private set; }

		public string IsCollectionRaw { get; private set; }

		public string IsHiddenRaw { get; private set; }

		public bool IsFolder
		{
			get
			{
				return "t".Equals(this.IsFolderRaw, StringComparison.OrdinalIgnoreCase);
			}
		}

		public bool IsCollection
		{
			get
			{
				return "1".Equals(this.IsCollectionRaw, StringComparison.OrdinalIgnoreCase);
			}
		}

		public bool IsHidden
		{
			get
			{
				return "0".Equals(this.IsHiddenRaw, StringComparison.OrdinalIgnoreCase);
			}
		}

		public string DisplayName { get; private set; }

		public string LastModifiedRaw { get; private set; }

		public string CreationDateRaw { get; private set; }

		public string Href { get; private set; }

		public string Status { get; private set; }

		public static SharepointFileInfo ParseNode(XmlNode node, XmlNamespaceManager nsmgr)
		{
			SharepointFileInfo sharepointFileInfo = new SharepointFileInfo();
			XmlNode xmlNode = node.SelectSingleNode("D:href", nsmgr);
			if (xmlNode != null)
			{
				sharepointFileInfo.Href = xmlNode.InnerText.Trim();
			}
			XmlNode xmlNode2 = node.SelectSingleNode("D:propstat/D:prop", nsmgr);
			if (xmlNode2 == null)
			{
				return sharepointFileInfo;
			}
			XmlNode xmlNode3 = xmlNode2.SelectSingleNode("D:displayname", nsmgr);
			if (xmlNode3 != null)
			{
				sharepointFileInfo.DisplayName = xmlNode3.InnerText.Trim();
			}
			xmlNode3 = xmlNode2.SelectSingleNode("D:isFolder", nsmgr);
			if (xmlNode3 != null)
			{
				sharepointFileInfo.IsFolderRaw = xmlNode3.InnerText;
			}
			xmlNode3 = xmlNode2.SelectSingleNode("D:iscollection", nsmgr);
			if (xmlNode3 != null)
			{
				sharepointFileInfo.IsCollectionRaw = xmlNode3.InnerText;
			}
			xmlNode3 = xmlNode2.SelectSingleNode("D:ishidden", nsmgr);
			if (xmlNode3 != null)
			{
				sharepointFileInfo.IsHiddenRaw = xmlNode3.InnerText;
			}
			xmlNode3 = xmlNode2.SelectSingleNode("D:getlastmodified", nsmgr);
			if (xmlNode3 != null)
			{
				sharepointFileInfo.LastModifiedRaw = xmlNode3.InnerText;
			}
			xmlNode3 = xmlNode2.SelectSingleNode("D:creationdate", nsmgr);
			if (xmlNode3 != null)
			{
				sharepointFileInfo.CreationDateRaw = xmlNode3.InnerText;
			}
			return sharepointFileInfo;
		}
	}
}
