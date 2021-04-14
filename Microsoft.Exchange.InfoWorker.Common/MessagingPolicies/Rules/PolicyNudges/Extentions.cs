using System;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges
{
	internal static class Extentions
	{
		internal static XElement FirstElement(this XElement element)
		{
			if (element == null)
			{
				return null;
			}
			XNode xnode = element.FirstNode;
			while (xnode != null && xnode.NodeType != XmlNodeType.Element)
			{
				xnode = xnode.NextNode;
			}
			return xnode as XElement;
		}

		internal static XElement NextElement(this XElement element)
		{
			if (element == null)
			{
				return null;
			}
			XNode nextNode = element.NextNode;
			while (nextNode != null && nextNode.NodeType != XmlNodeType.Element)
			{
				nextNode = nextNode.NextNode;
			}
			return nextNode as XElement;
		}
	}
}
