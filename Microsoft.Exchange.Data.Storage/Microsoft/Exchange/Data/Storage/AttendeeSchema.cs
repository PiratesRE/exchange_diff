using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttendeeSchema : RecipientSchema
	{
		public new static AttendeeSchema Instance
		{
			get
			{
				if (AttendeeSchema.instance == null)
				{
					AttendeeSchema.instance = new AttendeeSchema();
				}
				return AttendeeSchema.instance;
			}
		}

		public static readonly StorePropertyDefinition AttendeeType = InternalSchema.RecipientType;

		internal static readonly StorePropertyDefinition ObjectType = InternalSchema.ObjectType;

		private static AttendeeSchema instance = null;
	}
}
