using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct FormatConverterContainer
	{
		internal FormatConverterContainer(FormatConverter converter, int level)
		{
			this.converter = converter;
			this.level = level;
		}

		public bool IsNull
		{
			get
			{
				return this.converter == null;
			}
		}

		public FormatContainerType Type
		{
			get
			{
				if (!this.IsNull)
				{
					return this.converter.BuildStack[this.level].Type;
				}
				return FormatContainerType.Null;
			}
		}

		public HtmlNameIndex TagName
		{
			get
			{
				if (!this.IsNull)
				{
					return this.converter.BuildStack[this.level].TagName;
				}
				return HtmlNameIndex._NOTANAME;
			}
		}

		public FormatConverterContainer Parent
		{
			get
			{
				if (this.level > 1)
				{
					return new FormatConverterContainer(this.converter, this.level - 1);
				}
				return FormatConverterContainer.Null;
			}
		}

		public FormatConverterContainer Child
		{
			get
			{
				if (this.level != this.converter.BuildStackTop - 1)
				{
					return new FormatConverterContainer(this.converter, this.level + 1);
				}
				return FormatConverterContainer.Null;
			}
		}

		public FlagProperties FlagProperties
		{
			get
			{
				return this.converter.BuildStack[this.level].FlagProperties;
			}
			set
			{
				this.converter.BuildStack[this.level].FlagProperties = value;
			}
		}

		public PropertyBitMask PropertyMask
		{
			get
			{
				return this.converter.BuildStack[this.level].PropertyMask;
			}
			set
			{
				this.converter.BuildStack[this.level].PropertyMask = value;
			}
		}

		public Property[] Properties
		{
			get
			{
				return this.converter.BuildStack[this.level].Properties;
			}
		}

		public FormatNode Node
		{
			get
			{
				return new FormatNode(this.converter.Store, this.converter.BuildStack[this.level].Node);
			}
		}

		public static bool operator ==(FormatConverterContainer x, FormatConverterContainer y)
		{
			return x.converter == y.converter && x.level == y.level;
		}

		public static bool operator !=(FormatConverterContainer x, FormatConverterContainer y)
		{
			return x.converter != y.converter || x.level != y.level;
		}

		public void SetProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, PropertyValue value)
		{
			if (value.IsString)
			{
				this.converter.GetStringValue(value).AddRef();
			}
			else if (value.IsMultiValue)
			{
				this.converter.GetMultiValue(value).AddRef();
			}
			this.converter.ContainerStyleBuildHelper.SetProperty((int)propertyPrecedence, propertyId, value);
		}

		public void SetProperties(PropertyPrecedence propertyPrecedence, Property[] properties, int propertyCount)
		{
			for (int i = 0; i < propertyCount; i++)
			{
				this.SetProperty(propertyPrecedence, properties[i].Id, properties[i].Value);
			}
		}

		public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, StringValue value)
		{
			value.AddRef();
			this.converter.ContainerStyleBuildHelper.SetProperty((int)propertyPrecedence, propertyId, value.PropertyValue);
		}

		public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, string value)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, value);
			this.converter.ContainerStyleBuildHelper.SetProperty((int)propertyPrecedence, propertyId, stringValue.PropertyValue);
		}

		public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, BufferString value)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, value);
			this.converter.ContainerStyleBuildHelper.SetProperty((int)propertyPrecedence, propertyId, stringValue.PropertyValue);
		}

		public void SetStringProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, char[] buffer, int offset, int count)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count));
			this.converter.ContainerStyleBuildHelper.SetProperty((int)propertyPrecedence, propertyId, stringValue.PropertyValue);
		}

		public void SetMultiValueProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, MultiValue value)
		{
			value.AddRef();
			this.converter.ContainerStyleBuildHelper.SetProperty((int)propertyPrecedence, propertyId, value.PropertyValue);
		}

		public void SetMultiValueProperty(PropertyPrecedence propertyPrecedence, PropertyId propertyId, out MultiValueBuilder multiValueBuilder)
		{
			MultiValue multiValue = this.converter.RegisterMultiValue(false, out multiValueBuilder);
			this.converter.ContainerStyleBuildHelper.SetProperty((int)propertyPrecedence, propertyId, multiValue.PropertyValue);
		}

		public void SetStyleReference(int stylePrecedence, int styleHandle)
		{
			this.converter.ContainerStyleBuildHelper.AddStyle(stylePrecedence, styleHandle);
		}

		public void SetStyleReference(int stylePrecedence, FormatStyle style)
		{
			this.converter.ContainerStyleBuildHelper.AddStyle(stylePrecedence, style.Handle);
		}

		public override bool Equals(object obj)
		{
			return obj is FormatConverterContainer && this.converter == ((FormatConverterContainer)obj).converter && this.level == ((FormatConverterContainer)obj).level;
		}

		public override int GetHashCode()
		{
			return this.level;
		}

		[Conditional("DEBUG")]
		private void AssertValidAndNotNull()
		{
		}

		[Conditional("DEBUG")]
		private void AssertValid()
		{
		}

		[Conditional("DEBUG")]
		private void AssertValidNotFlushed()
		{
		}

		public static readonly FormatConverterContainer Null = new FormatConverterContainer(null, -1);

		private FormatConverter converter;

		private int level;
	}
}
