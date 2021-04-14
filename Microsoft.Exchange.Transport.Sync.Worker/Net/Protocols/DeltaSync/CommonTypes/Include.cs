using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.CommonTypes
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Include
	{
		internal Include()
		{
			this.anyAttrField = new List<XmlAttribute>();
			this.anyField = new List<XmlElement>();
		}

		[XmlAnyElement(Order = 0)]
		internal List<XmlElement> Any
		{
			get
			{
				return this.anyField;
			}
			set
			{
				this.anyField = value;
			}
		}

		internal string href
		{
			get
			{
				return this.hrefField;
			}
			set
			{
				this.hrefField = value;
			}
		}

		[XmlAnyAttribute]
		internal List<XmlAttribute> AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private List<XmlElement> anyField;

		private string hrefField;

		private List<XmlAttribute> anyAttrField;
	}
}
