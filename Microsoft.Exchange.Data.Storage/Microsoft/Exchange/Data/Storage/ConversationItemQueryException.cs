using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ConversationItemQueryException : StoragePermanentException
	{
		internal ConversationItemQueryException(LocalizedString message, PropertyDefinition unsupportedProperty) : base(message)
		{
			this.unsupportedProperty = unsupportedProperty;
		}

		protected ConversationItemQueryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.unsupportedProperty = (info.GetValue("unsupportedProperty", typeof(PropertyDefinition)) as PropertyDefinition);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("unsupportedProperty", this.unsupportedProperty);
		}

		public PropertyDefinition UnsupportedProperty
		{
			get
			{
				return this.unsupportedProperty;
			}
		}

		private const string UnsupportedPropertyLabel = "unsupportedProperty";

		private PropertyDefinition unsupportedProperty;
	}
}
