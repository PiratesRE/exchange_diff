using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingRequestSchema : MeetingMessageInstanceSchema
	{
		public new static MeetingRequestSchema Instance
		{
			get
			{
				if (MeetingRequestSchema.instance == null)
				{
					MeetingRequestSchema.instance = new MeetingRequestSchema();
				}
				return MeetingRequestSchema.instance;
			}
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				return base.PropertyRules.Concat(MeetingRequestSchema.MeetingRequestPropertyRules);
			}
		}

		[Autoload]
		internal static readonly StorePropertyDefinition AppointmentClass = InternalSchema.AppointmentClass;

		public static readonly StorePropertyDefinition AppointmentReplyTime = InternalSchema.AppointmentReplyTime;

		public static readonly StorePropertyDefinition IntendedFreeBusyStatus = InternalSchema.IntendedFreeBusyStatus;

		[Autoload]
		public static readonly StorePropertyDefinition OldStartWhole = InternalSchema.OldStartWhole;

		[Autoload]
		public static readonly StorePropertyDefinition OldEndWhole = InternalSchema.OldEndWhole;

		public static readonly StorePropertyDefinition OccurrencesExceptionalViewProperties = InternalSchema.OccurrencesExceptionalViewProperties;

		[Autoload]
		internal static readonly StorePropertyDefinition UnsendableRecipients = InternalSchema.UnsendableRecipients;

		private static readonly PropertyRule[] MeetingRequestPropertyRules = new PropertyRule[]
		{
			PropertyRuleLibrary.ResponseAndReplyRequested
		};

		private static MeetingRequestSchema instance = null;
	}
}
