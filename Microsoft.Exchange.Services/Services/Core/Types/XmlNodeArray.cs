using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class XmlNodeArray
	{
		internal void Normalize()
		{
			for (int i = this.Nodes.Count - 1; i >= 0; i--)
			{
				XmlNode xmlNode = this.Nodes[i];
				switch (ServiceXml.GetNormalizationAction(xmlNode))
				{
				case ServiceXml.NormalizationAction.Normalize:
					ServiceXml.NormalizeXml(xmlNode);
					break;
				case ServiceXml.NormalizationAction.Remove:
					this.Nodes.Remove(xmlNode);
					break;
				}
			}
		}

		[XmlAnyElement]
		public List<XmlNode> Nodes
		{
			get
			{
				return this.nodes;
			}
			set
			{
				this.nodes = value;
			}
		}

		private List<XmlNode> nodes = new List<XmlNode>();
	}
}
