using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal sealed class PropertyInconsistency : Inconsistency
	{
		private PropertyInconsistency()
		{
		}

		private PropertyInconsistency(RoleType owner, CalendarInconsistencyFlag flag, string propertyName, object expectedValue, object actualValue, CalendarValidationContext context) : base(owner, string.Empty, flag, context)
		{
			this.PropertyName = propertyName;
			this.ExpectedValue = ((expectedValue == null) ? "<NULL>" : expectedValue.ToString());
			this.ActualValue = ((actualValue == null) ? "<NULL>" : actualValue.ToString());
		}

		internal static PropertyInconsistency CreateInstance(RoleType owner, CalendarInconsistencyFlag flag, string propertyName, object expectedValue, object actualValue, CalendarValidationContext context)
		{
			return new PropertyInconsistency(owner, flag, propertyName, expectedValue, actualValue, context);
		}

		internal override RumInfo CreateRumInfo(CalendarValidationContext context, IList<Attendee> attendees)
		{
			CalendarInconsistencyFlag flag = base.Flag;
			if (flag != CalendarInconsistencyFlag.Cancellation)
			{
				return base.CreateRumInfo(context, attendees);
			}
			bool flag2;
			if (!bool.TryParse(this.ExpectedValue, out flag2))
			{
				throw new ArgumentException("Expected value for cancellation inconsistency should be Boolean.", "inconsistency.ExpectedValue");
			}
			if (flag2)
			{
				return CancellationRumInfo.CreateMasterInstance(attendees);
			}
			return UpdateRumInfo.CreateMasterInstance(attendees, base.Flag);
		}

		internal string PropertyName { get; private set; }

		internal string ExpectedValue { get; private set; }

		internal string ActualValue { get; private set; }

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Owner", base.Owner.ToString());
			writer.WriteElementString("PropertyName", this.PropertyName);
			writer.WriteElementString("ExpectedValue", this.ExpectedValue);
			writer.WriteElementString("ActualValue", this.ActualValue);
		}

		private const string NullValueString = "<NULL>";
	}
}
