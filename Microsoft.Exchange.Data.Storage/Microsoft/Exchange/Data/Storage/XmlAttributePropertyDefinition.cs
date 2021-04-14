using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class XmlAttributePropertyDefinition : SimpleVirtualPropertyDefinition
	{
		internal XmlAttributePropertyDefinition(string displayName, Type type, string xmlAttributeName, params PropertyDefinitionConstraint[] constraints) : this(displayName, type, xmlAttributeName, XmlAttributePropertyDefinition.noDefaultValue, constraints)
		{
		}

		internal XmlAttributePropertyDefinition(string displayName, Type type, string xmlAttributeName, XmlAttributePropertyDefinition.GenerateDefaultValueDelegate generateDefaultValue, params PropertyDefinitionConstraint[] constraints) : base(PropertyTypeSpecifier.XmlNode, displayName, type, PropertyFlags.Automatic, constraints)
		{
			this.xmlAttributeName = xmlAttributeName;
			this.generateDefaultValue = generateDefaultValue;
		}

		internal object DefaultValue
		{
			get
			{
				return this.generateDefaultValue();
			}
		}

		internal bool HasDefaultValue
		{
			get
			{
				return this.generateDefaultValue != XmlAttributePropertyDefinition.noDefaultValue;
			}
		}

		internal string XmlAttributeName
		{
			get
			{
				return this.xmlAttributeName;
			}
		}

		private static object GenerateNoDefaultValue()
		{
			throw new NotSupportedException("Property doesn't have a default value");
		}

		protected override string GetPropertyDefinitionString()
		{
			return string.Format("XML: @{0}", this.xmlAttributeName);
		}

		private readonly string xmlAttributeName;

		private readonly XmlAttributePropertyDefinition.GenerateDefaultValueDelegate generateDefaultValue;

		private static readonly XmlAttributePropertyDefinition.GenerateDefaultValueDelegate noDefaultValue = new XmlAttributePropertyDefinition.GenerateDefaultValueDelegate(XmlAttributePropertyDefinition.GenerateNoDefaultValue);

		internal delegate object GenerateDefaultValueDelegate();
	}
}
