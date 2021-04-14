using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class SharingEffectiveRightsProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		public SharingEffectiveRightsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static SharingEffectiveRightsProperty CreateCommand(CommandContext commandContext)
		{
			return new SharingEffectiveRightsProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ExternalUserIdAndSession externalUserIdAndSession = commandSettings.IdAndSession as ExternalUserIdAndSession;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			if (externalUserIdAndSession == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<IdAndSession>((long)this.GetHashCode(), "SharingEffectiveRightsProperty.ToServiceObject: The id and session {0} is not an external user id.", commandSettings.IdAndSession);
				return;
			}
			MailboxSession mailboxSession = externalUserIdAndSession.Session as MailboxSession;
			DefaultFolderType defaultFolderType = mailboxSession.IsDefaultFolderType(externalUserIdAndSession.Id);
			if (defaultFolderType == DefaultFolderType.Calendar)
			{
				CalendarPermissionReadAccess calendarPermissionReadAccess = CalendarPermissionReadAccess.None;
				if (externalUserIdAndSession.PermissionGranted.CanReadItems)
				{
					calendarPermissionReadAccess = CalendarPermissionReadAccess.FullDetails;
				}
				else
				{
					CalendarFolderPermission calendarFolderPermission = externalUserIdAndSession.PermissionGranted as CalendarFolderPermission;
					if (calendarFolderPermission != null)
					{
						if (calendarFolderPermission.FreeBusyAccess == FreeBusyAccess.Details)
						{
							calendarPermissionReadAccess = CalendarPermissionReadAccess.TimeAndSubjectAndLocation;
						}
						else if (calendarFolderPermission.FreeBusyAccess == FreeBusyAccess.Basic)
						{
							calendarPermissionReadAccess = CalendarPermissionReadAccess.TimeOnly;
						}
					}
				}
				serviceObject[propertyInformation] = calendarPermissionReadAccess;
				return;
			}
			if (defaultFolderType == DefaultFolderType.Contacts)
			{
				PermissionReadAccess permissionReadAccess = PermissionReadAccess.None;
				if (externalUserIdAndSession.PermissionGranted.CanReadItems)
				{
					permissionReadAccess = PermissionReadAccess.FullDetails;
				}
				serviceObject[propertyInformation] = permissionReadAccess;
				return;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceWarning<string>((long)this.GetHashCode(), "SharingEffectiveRightsProperty.ToServiceObject: The foldertype {0} is not supported.", defaultFolderType.ToString());
		}

		void IToXmlCommand.ToXml()
		{
			throw new InvalidOperationException("SharingEffectiveRightsProperty.ToXml should not be called.");
		}
	}
}
