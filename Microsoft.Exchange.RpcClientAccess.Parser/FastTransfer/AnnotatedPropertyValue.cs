using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct AnnotatedPropertyValue
	{
		internal AnnotatedPropertyValue(PropertyTag propertyTag, PropertyValue propertyValue, NamedProperty namedProperty)
		{
			this.AnnotatedPropertyTag = new AnnotatedPropertyTag(propertyTag, namedProperty);
			this.PropertyValue = propertyValue;
		}

		public NamedProperty NamedProperty
		{
			get
			{
				return this.AnnotatedPropertyTag.NamedProperty;
			}
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.AnnotatedPropertyTag.PropertyTag;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}={1}{2}", this.AnnotatedPropertyTag, this.PropertyValue.IsError ? "(error)" : string.Empty, this.PropertyValue.Value);
		}

		public readonly PropertyValue PropertyValue;

		public readonly AnnotatedPropertyTag AnnotatedPropertyTag;
	}
}
