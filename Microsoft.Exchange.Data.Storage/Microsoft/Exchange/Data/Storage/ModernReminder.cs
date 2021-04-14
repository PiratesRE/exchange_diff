using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class ModernReminder : IModernReminder, IReminder
	{
		public ModernReminder()
		{
			this.Initialize();
		}

		[DataMember]
		public Guid Identifier { get; set; }

		[DataMember]
		public ReminderTimeHint ReminderTimeHint { get; set; }

		[DataMember]
		public Hours Hours { get; set; }

		[DataMember]
		public Priority Priority { get; set; }

		[DataMember]
		public int Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				if (value < 0)
				{
					throw new InvalidParamException(ServerStrings.InvalidDuration(value));
				}
				this.duration = value;
			}
		}

		[IgnoreDataMember]
		public ExDateTime ReferenceTime
		{
			get
			{
				return this.ToExDateTime(this.InternalReferenceTime);
			}
			set
			{
				this.InternalReferenceTime = this.ToDateTime(value);
			}
		}

		[IgnoreDataMember]
		public ExDateTime CustomReminderTime
		{
			get
			{
				return this.ToExDateTime(this.InternalCustomReminderTime);
			}
			set
			{
				this.InternalCustomReminderTime = this.ToDateTime(value);
			}
		}

		[IgnoreDataMember]
		public ExDateTime DueDate
		{
			get
			{
				return this.ToExDateTime(this.InternalDueDate);
			}
			set
			{
				this.InternalDueDate = this.ToDateTime(value);
			}
		}

		[DataMember]
		private DateTime InternalReferenceTime { get; set; }

		[DataMember]
		private DateTime InternalCustomReminderTime { get; set; }

		[DataMember]
		private DateTime InternalDueDate { get; set; }

		[OnDeserializing]
		public void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			this.Validate();
		}

		[OnSerializing]
		public void OnSerializing(StreamingContext context)
		{
			this.Validate();
		}

		public int GetCurrentVersion()
		{
			return 1;
		}

		private void Initialize()
		{
			this.ReminderTimeHint = ReminderTimeHint.Tomorrow;
			this.Hours = Hours.Any;
			this.Priority = Priority.Normal;
			this.Duration = 30;
			this.CustomReminderTime = ModernReminder.DefaultCustomReminderTime;
			this.DueDate = ModernReminder.DefaultDueDate;
			this.ReferenceTime = ModernReminder.DefaultReferenceTime;
		}

		private void Validate()
		{
			if (this.InternalDueDate < this.InternalReferenceTime)
			{
				throw new InvalidParamException(ServerStrings.InvalidDueDate1(this.InternalDueDate.ToString(), this.InternalReferenceTime.ToString()));
			}
			if (this.ReminderTimeHint == ReminderTimeHint.Custom)
			{
				if (this.InternalCustomReminderTime < this.InternalReferenceTime)
				{
					throw new InvalidParamException(ServerStrings.InvalidReminderTime(this.InternalCustomReminderTime.ToString(), this.InternalReferenceTime.ToString()));
				}
				if (this.InternalDueDate < this.InternalCustomReminderTime)
				{
					throw new InvalidParamException(ServerStrings.InvalidDueDate2(this.InternalDueDate.ToString(), this.InternalCustomReminderTime.ToString()));
				}
			}
		}

		private DateTime ToDateTime(ExDateTime exDateTime)
		{
			return exDateTime.UniversalTime;
		}

		private ExDateTime ToExDateTime(DateTime dateTime)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
		}

		private const ReminderTimeHint DefaultReminderTimeHint = ReminderTimeHint.Tomorrow;

		private const Hours DefaultHours = Hours.Any;

		private const Priority DefaultPriority = Priority.Normal;

		private const int DefaultDuration = 30;

		private const int CurrentVersion = 1;

		private static readonly ExDateTime DefaultReferenceTime = ExDateTime.MinValue;

		private static readonly ExDateTime DefaultCustomReminderTime = ExDateTime.MaxValue;

		private static readonly ExDateTime DefaultDueDate = ExDateTime.MaxValue;

		private int duration;
	}
}
