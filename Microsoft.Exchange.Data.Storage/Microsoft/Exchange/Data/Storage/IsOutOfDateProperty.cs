using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class IsOutOfDateProperty : SmartPropertyDefinition
	{
		internal IsOutOfDateProperty() : base("IsOutOfDate", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[0])
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			MeetingMessage meetingMessage = propertyBag.Context.StoreObject as MeetingMessage;
			if (meetingMessage != null)
			{
				return meetingMessage.IsOutOfDate();
			}
			return new PropertyError(this, PropertyErrorCode.GetCalculatedPropertyError);
		}
	}
}
