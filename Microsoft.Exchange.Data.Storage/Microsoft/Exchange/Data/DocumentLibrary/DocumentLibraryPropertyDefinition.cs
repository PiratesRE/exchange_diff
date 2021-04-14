using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DocumentLibraryPropertyDefinition : PropertyDefinition
	{
		internal DocumentLibraryPropertyDefinition(string name, Type type, object defaultValue, DocumentLibraryPropertyId propertyId) : base(name, type)
		{
			this.defaultValue = defaultValue;
			this.propertyId = propertyId;
			if (defaultValue != null && defaultValue.GetType() != type)
			{
				throw new ArgumentException();
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		internal DocumentLibraryPropertyId PropertyId
		{
			get
			{
				return this.propertyId;
			}
		}

		private readonly object defaultValue;

		private readonly DocumentLibraryPropertyId propertyId;
	}
}
