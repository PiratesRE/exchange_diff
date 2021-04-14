using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class IsOutOfDateProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private IsOutOfDateProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static IsOutOfDateProperty CreateCommand(CommandContext commandContext)
		{
			return new IsOutOfDateProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("IsOutOfDateProperty.ToXml should not be called");
		}

		public override bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			MeetingMessage meetingMessage = (MeetingMessage)commandSettings.StoreObject;
			try
			{
				serviceObject[propertyInformation] = meetingMessage.IsOutOfDate();
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.CreateItemCallTracer.TraceDebug<bool, LogonType, ObjectNotFoundException>((long)this.GetHashCode(), "[IsOutOfDate::ToServiceObject] meetingMessage.IsDelegated='{0}'; meetingMessage.Session.LogonType='{1}'; Exception: '{2}'", meetingMessage.IsDelegated(), meetingMessage.Session.LogonType, arg);
			}
		}
	}
}
