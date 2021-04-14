using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReminderMessageSchema : MessageItemSchema
	{
		public new static ReminderMessageSchema Instance
		{
			get
			{
				if (ReminderMessageSchema.instance == null)
				{
					lock (ReminderMessageSchema.syncObj)
					{
						if (ReminderMessageSchema.instance == null)
						{
							ReminderMessageSchema.instance = new ReminderMessageSchema();
						}
					}
				}
				return ReminderMessageSchema.instance;
			}
		}

		private static readonly object syncObj = new object();

		public static readonly StorePropertyDefinition ReminderText = InternalSchema.ReminderText;

		internal static readonly StorePropertyDefinition Location = InternalSchema.Location;

		public static readonly StorePropertyDefinition ReminderStartTime = InternalSchema.ReminderStartTime;

		public static readonly StorePropertyDefinition ReminderEndTime = InternalSchema.ReminderEndTime;

		public static readonly StorePropertyDefinition ReminderId = InternalSchema.ReminderId;

		public static readonly StorePropertyDefinition ReminderItemGlobalObjectId = InternalSchema.ReminderItemGlobalObjectId;

		public static readonly StorePropertyDefinition ReminderOccurrenceGlobalObjectId = InternalSchema.ReminderOccurrenceGlobalObjectId;

		private static ReminderMessageSchema instance = null;
	}
}
