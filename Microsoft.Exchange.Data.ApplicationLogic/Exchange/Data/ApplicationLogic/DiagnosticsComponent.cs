using System;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	public class DiagnosticsComponent : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get
			{
				return base["name"] as string;
			}
		}

		[ConfigurationProperty("methodName", IsRequired = true)]
		public string MethodName
		{
			get
			{
				return base["methodName"] as string;
			}
		}

		[ConfigurationProperty("implementation", IsRequired = true)]
		public string Implementation
		{
			get
			{
				return base["implementation"] as string;
			}
		}

		[ConfigurationProperty("argument", IsRequired = false)]
		public string Argument
		{
			get
			{
				return base["argument"] as string;
			}
		}

		public XElement Data { get; private set; }

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (!string.Equals(elementName, "Data", StringComparison.OrdinalIgnoreCase))
			{
				return base.OnDeserializeUnrecognizedElement(elementName, reader);
			}
			this.Data = (XNode.ReadFrom(reader) as XElement);
			return true;
		}
	}
}
