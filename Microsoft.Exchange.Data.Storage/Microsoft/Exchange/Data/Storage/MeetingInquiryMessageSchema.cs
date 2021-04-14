using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MeetingInquiryMessageSchema : MessageItemSchema
	{
		public new static MeetingInquiryMessageSchema Instance
		{
			get
			{
				if (MeetingInquiryMessageSchema.instance == null)
				{
					MeetingInquiryMessageSchema.instance = new MeetingInquiryMessageSchema();
				}
				return MeetingInquiryMessageSchema.instance;
			}
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				if (this.propertyRulesCache == null)
				{
					this.propertyRulesCache = base.PropertyRules.Concat(MeetingInquiryMessageSchema.MeetingInquiryMessagePropertyRules);
				}
				return this.propertyRulesCache;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition CalendarProcessed = InternalSchema.CalendarProcessed;

		[Autoload]
		public static readonly StorePropertyDefinition IsProcessed = InternalSchema.IsProcessed;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentAuxiliaryFlags = InternalSchema.AppointmentAuxiliaryFlags;

		[Autoload]
		internal static readonly StorePropertyDefinition GlobalObjectId = InternalSchema.GlobalObjectId;

		[Autoload]
		public static readonly StorePropertyDefinition CalendarLogTriggerAction = InternalSchema.CalendarLogTriggerAction;

		[Autoload]
		internal static readonly StorePropertyDefinition ItemVersion = InternalSchema.ItemVersion;

		[Autoload]
		internal static readonly StorePropertyDefinition CleanGlobalObjectId = InternalSchema.CleanGlobalObjectId;

		internal static readonly StorePropertyDefinition ChangeList = InternalSchema.ChangeList;

		private static readonly PropertyRule[] MeetingInquiryMessagePropertyRules = new PropertyRule[]
		{
			PropertyRuleLibrary.DefaultCleanGlobalObjectIdFromGlobalObjectId
		};

		private static MeetingInquiryMessageSchema instance = null;

		private ICollection<PropertyRule> propertyRulesCache;
	}
}
