using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct AnnotatedPropertyTag
	{
		internal AnnotatedPropertyTag(PropertyTag propertyTag, NamedProperty namedProperty)
		{
			this.PropertyTag = propertyTag;
			this.NamedProperty = namedProperty;
		}

		public override string ToString()
		{
			return string.Format("{0}/{1}", this.PropertyTag, this.NamedProperty);
		}

		public readonly NamedProperty NamedProperty;

		public readonly PropertyTag PropertyTag;
	}
}
