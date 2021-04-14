using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContactProtectedPropertiesSchema : Schema
	{
		public new static ContactProtectedPropertiesSchema Instance
		{
			get
			{
				return ContactProtectedPropertiesSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition ProtectedEmailAddress = InternalSchema.ProtectedEmailAddress;

		public static readonly StorePropertyDefinition ProtectedPhoneNumber = InternalSchema.ProtectedPhoneNumber;

		private static readonly ContactProtectedPropertiesSchema instance = new ContactProtectedPropertiesSchema();
	}
}
