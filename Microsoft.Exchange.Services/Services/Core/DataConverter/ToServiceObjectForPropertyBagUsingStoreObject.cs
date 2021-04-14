using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ToServiceObjectForPropertyBagUsingStoreObject : ToServiceObjectPropertyList
	{
		public ToServiceObjectForPropertyBagUsingStoreObject(Shape shape, ResponseShape responseShape, IParticipantResolver participantResolver) : base(shape, responseShape, participantResolver)
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
			bool implementsToServiceObjectForPropertyBagCommand = propertyInformation.ImplementsToServiceObjectForPropertyBagCommand;
			if (!implementsToServiceObjectForPropertyBagCommand && returnErrorForInvalidProperty)
			{
				throw new InvalidPropertyForOperationException(propertyInformation.PropertyPath);
			}
			return implementsToServiceObjectForPropertyBagCommand;
		}
	}
}
