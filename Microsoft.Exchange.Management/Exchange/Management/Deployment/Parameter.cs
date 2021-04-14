using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public class Parameter
	{
		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[XmlAttribute]
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.values.Count > 0)
				{
					throw new ArgumentException(Strings.ParameterValueIsAlreadySet(this.Name), this.Name);
				}
				this.value = value;
				this.isSingleValue = true;
			}
		}

		[XmlArrayItem(ElementName = "Value", Type = typeof(string))]
		[XmlArray]
		public List<string> Values
		{
			get
			{
				return this.values;
			}
		}

		internal object EffectiveValue
		{
			get
			{
				if (!this.isSingleValue)
				{
					return this.Values.ToArray();
				}
				if (this.Value.ToLower() == "true")
				{
					return true;
				}
				if (this.Value.ToLower() == "false")
				{
					return false;
				}
				return this.Value;
			}
		}

		private string name;

		private string value;

		private bool isSingleValue;

		private List<string> values = new List<string>();
	}
}
