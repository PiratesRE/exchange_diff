using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[Serializable]
	public sealed class Notification : ConfigurableObject
	{
		public Notification() : this(new NotificationIdentity(), string.Empty, 0, 0, ExDateTime.UtcNow, EventLogEntryType.Information, Notification.EmptyStringArray, false, ExDateTime.UtcNow, Notification.EmptyStringArray, Notification.EmptyStringArray, string.Empty, ObjectState.New)
		{
		}

		internal Notification(NotificationIdentity identity, string eventSource, int eventId, int eventCategoryId, ExDateTime eventTime, EventLogEntryType entryType, IEnumerable<string> insertionStrings, bool emailSent, ExDateTime creationTime, ICollection notificationRecipients, ICollection notificationMessageIds, string periodicKey, ObjectState objectState) : base(new SimpleProviderPropertyBag())
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = identity;
			if (!string.IsNullOrEmpty(eventSource))
			{
				this.EventSource = eventSource;
			}
			this.EventInstanceId = eventId;
			this.EventCategoryId = eventCategoryId;
			this.EventTimeUtc = eventTime;
			this.EntryType = entryType;
			if (insertionStrings != null)
			{
				this.InsertionStrings = new List<string>(insertionStrings);
			}
			if (notificationRecipients != null)
			{
				this.NotificationRecipients = new MultiValuedProperty<string>(notificationRecipients);
			}
			if (notificationMessageIds != null)
			{
				this.NotificationMessageIds = new MultiValuedProperty<string>(notificationMessageIds);
			}
			this.EmailSent = emailSent;
			this.CreationTimeUtc = creationTime;
			this.PeriodicKey = periodicKey;
			this.propertyBag.SetField(SimpleProviderObjectSchema.ObjectState, objectState);
			this.propertyBag.ResetChangeTracking(SimpleProviderObjectSchema.ObjectState);
		}

		public int EventInstanceId
		{
			get
			{
				return (int)this[Notification.NotificationSchema.EventInstanceId];
			}
			internal set
			{
				this[Notification.NotificationSchema.EventInstanceId] = value;
			}
		}

		public int EventDisplayId
		{
			get
			{
				return Utils.ExtractCodeFromEventIdentifier(this.EventInstanceId);
			}
		}

		public string EventSource
		{
			get
			{
				return (string)this[Notification.NotificationSchema.EventSource];
			}
			internal set
			{
				this[Notification.NotificationSchema.EventSource] = value;
			}
		}

		public string EventMessage
		{
			get
			{
				return (string)this[Notification.NotificationSchema.EventMessage];
			}
			internal set
			{
				this[Notification.NotificationSchema.EventMessage] = value;
			}
		}

		public int EventCategoryId
		{
			get
			{
				return (int)this[Notification.NotificationSchema.EventCategoryId];
			}
			internal set
			{
				this[Notification.NotificationSchema.EventCategoryId] = value;
			}
		}

		public string EventCategory
		{
			get
			{
				return (string)this[Notification.NotificationSchema.EventCategory];
			}
			internal set
			{
				this[Notification.NotificationSchema.EventCategory] = value;
			}
		}

		public ExDateTime EventTimeUtc
		{
			get
			{
				return ((ExDateTime)this[Notification.NotificationSchema.EventTimeUtc]).ToUtc();
			}
			internal set
			{
				this[Notification.NotificationSchema.EventTimeUtc] = value;
			}
		}

		public DateTime EventTimeLocal
		{
			get
			{
				return this.EventTimeUtc.UniversalTime.ToLocalTime();
			}
		}

		public bool EmailSent
		{
			get
			{
				return (bool)this[Notification.NotificationSchema.EmailSent];
			}
			internal set
			{
				this[Notification.NotificationSchema.EmailSent] = value;
			}
		}

		public MultiValuedProperty<string> NotificationRecipients
		{
			get
			{
				return (MultiValuedProperty<string>)this[Notification.NotificationSchema.NotificationRecipients];
			}
			internal set
			{
				this[Notification.NotificationSchema.NotificationRecipients] = value;
			}
		}

		public MultiValuedProperty<string> NotificationMessageIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[Notification.NotificationSchema.NotificationMessageIds];
			}
			internal set
			{
				this[Notification.NotificationSchema.NotificationMessageIds] = value;
			}
		}

		public ExDateTime CreationTimeUtc
		{
			get
			{
				return ((ExDateTime)this[Notification.NotificationSchema.CreationTimeUtc]).ToUtc();
			}
			internal set
			{
				this[Notification.NotificationSchema.CreationTimeUtc] = value;
			}
		}

		public DateTime CreationTimeLocal
		{
			get
			{
				return this.CreationTimeUtc.UniversalTime.ToLocalTime();
			}
		}

		public EventLogEntryType EntryType
		{
			get
			{
				return (EventLogEntryType)this[Notification.NotificationSchema.EntryType];
			}
			set
			{
				this[Notification.NotificationSchema.EntryType] = value;
			}
		}

		public string EventHelpUrl
		{
			get
			{
				return (string)this[Notification.NotificationSchema.EventHelpUrl];
			}
			internal set
			{
				this[Notification.NotificationSchema.EventHelpUrl] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			foreach (string value in this.InsertionStrings)
			{
				PropertyConstraintViolationError propertyConstraintViolationError = Notification.InsertionStringLengthConstraint.Validate(value, Notification.NotificationSchema.InsertionStrings, this);
				if (propertyConstraintViolationError != null)
				{
					errors.Add(propertyConstraintViolationError);
				}
			}
		}

		internal IList<string> InsertionStrings
		{
			get
			{
				return this.insertionStrings ?? ((IList<string>)Notification.EmptyStringArray);
			}
			set
			{
				this.insertionStrings = value;
			}
		}

		internal string PeriodicKey
		{
			get
			{
				return (string)this[Notification.NotificationSchema.PeriodicKey];
			}
			set
			{
				this[Notification.NotificationSchema.PeriodicKey] = value;
			}
		}

		internal void CopyChangesFrom(IConfigurable source)
		{
			throw new NotSupportedException();
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return Notification.Schema;
			}
		}

		internal long ComputeHashCodeForDuplicateDetection()
		{
			int num = this.EventSource.GetHashCodeCaseInsensitive() ^ this.PeriodicKey.GetHashCode();
			return (long)this.EventInstanceId << 32 | (long)((ulong)num);
		}

		internal const int MaxEventSourceLength = 256;

		internal const int MaxInsertionStringsCount = 100;

		internal const int MaxInsertionStringLength = 4096;

		internal const int MaxNotificationRecipientsCount = 64;

		private static readonly ObjectSchema Schema = ObjectSchema.GetInstance<Notification.NotificationSchema>();

		private static readonly string[] EmptyStringArray = new string[0];

		private static readonly StringLengthConstraint InsertionStringLengthConstraint = new StringLengthConstraint(0, 4096);

		[NonSerialized]
		private IList<string> insertionStrings;

		private sealed class NotificationSchema : SimpleProviderObjectSchema
		{
			internal static readonly SimpleProviderPropertyDefinition EventInstanceId = new SimpleProviderPropertyDefinition("EventInstanceId", ExchangeObjectVersion.Exchange2007, typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition EventSource = new SimpleProviderPropertyDefinition("EventSource", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
			{
				new StringLengthConstraint(1, 256)
			});

			internal static readonly SimpleProviderPropertyDefinition EventMessage = new SimpleProviderPropertyDefinition("EventMessage", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition EventCategoryId = new SimpleProviderPropertyDefinition("EventCategoryId", ExchangeObjectVersion.Exchange2007, typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition EventCategory = new SimpleProviderPropertyDefinition("EventCategory", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition EventTimeUtc = new SimpleProviderPropertyDefinition("EventTimeUtc", ExchangeObjectVersion.Exchange2007, typeof(ExDateTime), PropertyDefinitionFlags.None, ExDateTime.UtcNow, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition InsertionStrings = new SimpleProviderPropertyDefinition("InsertionStrings", ExchangeObjectVersion.Exchange2007, typeof(IList<string>), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition EntryType = new SimpleProviderPropertyDefinition("EntryType", ExchangeObjectVersion.Exchange2007, typeof(EventLogEntryType), PropertyDefinitionFlags.None, EventLogEntryType.Information, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition EmailSent = new SimpleProviderPropertyDefinition("EmailSent", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition CreationTimeUtc = new SimpleProviderPropertyDefinition("CreationTimeUtc", ExchangeObjectVersion.Exchange2007, typeof(ExDateTime), PropertyDefinitionFlags.Mandatory, ExDateTime.UtcNow, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition NotificationRecipients = new SimpleProviderPropertyDefinition("NotificationRecipients", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition EventHelpUrl = new SimpleProviderPropertyDefinition("EventHelpUrl", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition NotificationMessageIds = new SimpleProviderPropertyDefinition("NotificationMessageIds", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			internal static readonly SimpleProviderPropertyDefinition PeriodicKey = new SimpleProviderPropertyDefinition("PeriodicKey", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
