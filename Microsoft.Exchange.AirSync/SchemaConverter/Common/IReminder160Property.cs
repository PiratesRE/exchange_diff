using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IReminder160Property : IIntegerProperty, IProperty
	{
		bool ReminderIsSet { get; }
	}
}
