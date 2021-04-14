using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class AssociatedCalendarItemIdProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private AssociatedCalendarItemIdProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static AssociatedCalendarItemIdProperty CreateCommand(CommandContext commandContext)
		{
			return new AssociatedCalendarItemIdProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("AssociatedCalendarItemIdProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MeetingMessage meetingMessage = (MeetingMessage)commandSettings.StoreObject;
			try
			{
				IdAndSession correlatedItemIdAndSession = AssociatedCalendarItemIdProperty.GetCorrelatedItemIdAndSession(meetingMessage);
				if (correlatedItemIdAndSession != null)
				{
					ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(correlatedItemIdAndSession.Id, correlatedItemIdAndSession, null);
					commandSettings.ServiceObject.PropertyBag[propertyInformation] = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
				}
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.CreateItemCallTracer.TraceDebug<bool, LogonType, ObjectNotFoundException>((long)this.GetHashCode(), "[AssosiatedCalendarItemIdProperty::ToServiceObject] meetingMessage.IsDelegated='{0}'; meetingMessage.Session.LogonType='{1}'; Exception: '{2}'", meetingMessage.IsDelegated(), meetingMessage.Session.LogonType, arg);
			}
		}

		private static IdAndSession GetCorrelatedItemIdAndSession(MeetingMessage meetingMessage)
		{
			try
			{
				IdAndSession result = null;
				CalendarItemBase cachedCorrelatedItem = meetingMessage.GetCachedCorrelatedItem();
				if (cachedCorrelatedItem != null)
				{
					result = new IdAndSession(cachedCorrelatedItem.Id, cachedCorrelatedItem.Session);
				}
				return result;
			}
			catch (CorrelationFailedException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<CorrelationFailedException>(0L, "CalendarItem associated with MeetingMessage could not be found. Exception '{0}'.", ex);
				if (ex.InnerException is NotSupportedWithServerVersionException)
				{
					throw new WrongServerVersionDelegateException(ex);
				}
			}
			catch (CorruptDataException arg)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<CorruptDataException>(0L, "CalendarItem associated with the meeting message is corrupt. Exception '{0}'.", arg);
			}
			catch (VirusException arg2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<VirusException>(0L, "CalendarItem associated with the meeting message has a virus. Exception '{0}'.", arg2);
			}
			catch (RecurrenceException arg3)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<RecurrenceException>(0L, "CalendarItem associated with the meeting message has a recurrence problem. Exception '{0}'.", arg3);
			}
			return null;
		}
	}
}
