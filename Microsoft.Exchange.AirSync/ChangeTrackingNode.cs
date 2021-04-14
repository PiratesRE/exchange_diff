using System;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal class ChangeTrackingNode
	{
		internal ChangeTrackingNode(string namespaceUri, string name)
		{
			this.qualifiedName = namespaceUri + name;
		}

		internal static ChangeTrackingNode AllOtherNodes
		{
			get
			{
				return ChangeTrackingNode.allOtherNodes;
			}
		}

		internal static ChangeTrackingNode AllNodes
		{
			get
			{
				return ChangeTrackingNode.allNodes;
			}
		}

		internal string QualifiedName
		{
			get
			{
				return this.qualifiedName;
			}
		}

		internal static string GetQualifiedName(XmlNode node)
		{
			return node.NamespaceURI + node.Name;
		}

		private static ChangeTrackingNode allOtherNodes = new ChangeTrackingNode("AirSync:", "ApplicationData");

		private static ChangeTrackingNode allNodes = ChangeTrackingNode.AllOtherNodes;

		private string qualifiedName;
	}
}
