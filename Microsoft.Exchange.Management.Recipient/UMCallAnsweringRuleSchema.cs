using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMCallAnsweringRuleSchema : SimpleProviderObjectSchema
	{
		public const string PriorityParameterName = "Priority";

		public const string EnabledParameterName = "Enabled";

		public const string NameParameterName = "Name";

		public const string MailboxParameterName = "Mailbox";

		public const string ScheduleStatusParameterName = "ScheduleStatus";

		public const string TimeOfDayParameterName = "TimeOfDay";

		public const string CheckAutomaticRepliesParameterName = "CheckAutomaticReplies";

		public const string ExtensionsDialedParameterName = "ExtensionsDialed";

		public const string KeyMappingsParameterName = "KeyMappings";

		public const string CallersCanInterruptGreetingParameterName = "CallersCanInterruptGreeting";

		public const string CallerIdsParameterName = "CallerIds";

		private const int MaxScheduleStatus = 15;

		private const int MaximumPriority = 9;

		private const int MaxRuleNameLength = 256;

		public const int MaxKeyMappings = 10;

		public static readonly SimpleProviderPropertyDefinition Priority = new SimpleProviderPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2012, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 9, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 9)
		});

		public static readonly SimpleProviderPropertyDefinition Enabled = new SimpleProviderPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		});

		public static readonly SimpleProviderPropertyDefinition CallerIds = new SimpleProviderPropertyDefinition("CallerIds", ExchangeObjectVersion.Exchange2012, typeof(CallerIdItem), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CallersCanInterruptGreeting = new SimpleProviderPropertyDefinition("CallersCanInterruptGreeting", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CheckAutomaticReplies = new SimpleProviderPropertyDefinition("CheckAutomaticReplies", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExtensionsDialed = new SimpleProviderPropertyDefinition("ExtensionsDialed", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition KeyMappings = new SimpleProviderPropertyDefinition("KeyMappings", ExchangeObjectVersion.Exchange2012, typeof(KeyMapping), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ScheduleStatus = new SimpleProviderPropertyDefinition("ScheduleStatus", ExchangeObjectVersion.Exchange2012, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 15)
		});

		public static readonly SimpleProviderPropertyDefinition TimeOfDay = new SimpleProviderPropertyDefinition("TimeOfDay", ExchangeObjectVersion.Exchange2012, typeof(TimeOfDay), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
