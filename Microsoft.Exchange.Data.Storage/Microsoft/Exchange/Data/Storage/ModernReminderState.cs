using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ModernReminderState : IReminderState
	{
		public ModernReminderState()
		{
			this.Initialize();
		}

		[DataMember]
		public Guid Identifier { get; set; }

		[IgnoreDataMember]
		public ExDateTime ScheduledReminderTime
		{
			get
			{
				return this.ToExDateTime(this.InternalScheduledReminderTime);
			}
			set
			{
				this.InternalScheduledReminderTime = this.ToDateTime(value);
			}
		}

		[DataMember]
		private DateTime InternalScheduledReminderTime { get; set; }

		[OnDeserializing]
		public void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}

		public int GetCurrentVersion()
		{
			return 1;
		}

		private void Initialize()
		{
			this.ScheduledReminderTime = ExDateTime.MinValue;
		}

		private DateTime ToDateTime(ExDateTime exDateTime)
		{
			return exDateTime.UniversalTime;
		}

		private ExDateTime ToExDateTime(DateTime dateTime)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
		}

		private const int CurrentVersion = 1;
	}
}
