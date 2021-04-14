using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ToXmlForPropertyBagUsingStoreObject : ToXmlPropertyList
	{
		public ToXmlForPropertyBagUsingStoreObject(Shape shape, ResponseShape responseShape) : base(shape, responseShape)
		{
		}

		protected override bool IsErrorReturnedForInvalidBaseShapeProperty
		{
			get
			{
				return false;
			}
		}

		protected override bool ValidateProperty(PropertyInformation propertyInformation, bool returnErrorForInvalidProperty)
		{
			bool implementsToXmlForPropertyBagCommand = propertyInformation.ImplementsToXmlForPropertyBagCommand;
			if (!implementsToXmlForPropertyBagCommand && returnErrorForInvalidProperty)
			{
				throw new InvalidPropertyForOperationException(propertyInformation.PropertyPath);
			}
			return implementsToXmlForPropertyBagCommand;
		}
	}
}
