using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetCalendarSharingPermissions : CalendarActionBase<GetCalendarSharingPermissionsResponse>
	{
		private StoreObjectId CaldendarFolderId { get; set; }

		private OrganizationId OrganizationId { get; set; }

		public GetCalendarSharingPermissions(MailboxSession session, StoreObjectId calendarFolderId, OrganizationId organizationId, ADRecipientSessionContext adRecipientSessionContext) : base(session)
		{
			this.CaldendarFolderId = calendarFolderId;
			this.OrganizationId = organizationId;
			this.loggingContextIdentifier = Guid.NewGuid().ToString();
			this.adRecipientSessionContext = adRecipientSessionContext;
		}

		public override GetCalendarSharingPermissionsResponse Execute()
		{
			GetCalendarSharingPermissionsResponse getCalendarSharingPermissionsResponse = new GetCalendarSharingPermissionsResponse();
			List<CalendarSharingPermissionInfo> list = new List<CalendarSharingPermissionInfo>();
			MailboxSession mailboxSession = base.MailboxSession;
			SharingPolicy sharingPolicy = DirectoryHelper.ReadSharingPolicy(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, mailboxSession.MailboxOwner.MailboxInfo.IsArchive, mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
			DetailLevelEnumType detailLevelEnumType;
			getCalendarSharingPermissionsResponse.IsAnonymousSharingEnabled = (CalendarSharingPermissionsUtils.TryGetMaximumDetailLevel(sharingPolicy.GetAllowedForAnonymousCalendarSharing(), out detailLevelEnumType) && OwaAnonymousVdirLocater.Instance.IsPublishingAvailable(mailboxSession.MailboxOwner, new FrontEndLocator()));
			if (getCalendarSharingPermissionsResponse.IsAnonymousSharingEnabled)
			{
				getCalendarSharingPermissionsResponse.AnonymousSharingMaxDetailLevel = detailLevelEnumType.ToString();
			}
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			bool isSharingDefaultCalendar = defaultFolderId.Equals(this.CaldendarFolderId);
			DelegateUserCollectionHandler delegateUserCollectionHandler = new DelegateUserCollectionHandler(mailboxSession, this.adRecipientSessionContext);
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSession, this.CaldendarFolderId))
			{
				CalendarFolderPermissionSet permissionSet = calendarFolder.GetPermissionSet();
				foreach (CalendarFolderPermission calendarFolderPermission in permissionSet)
				{
					if (!CalendarSharingPermissionsUtils.IsPrincipalInteresting(calendarFolderPermission.Principal.Type))
					{
						GetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Skipping principal {0}.", new object[]
						{
							calendarFolderPermission.Principal.IndexString
						});
					}
					else
					{
						CalendarSharingPermissionInfo calendarSharingPermissionInfo = new CalendarSharingPermissionInfo();
						calendarSharingPermissionInfo.ID = calendarFolderPermission.Principal.IndexString;
						calendarSharingPermissionInfo.FromPermissionsTable = true;
						CalendarSharingDetailLevel calendarSharingDetailLevel;
						bool viewPrivateAppointments;
						if (!CalendarSharingPermissionsUtils.GetSharingDetailLevelFromPermissionLevel(calendarFolderPermission, isSharingDefaultCalendar, delegateUserCollectionHandler, out calendarSharingDetailLevel, out viewPrivateAppointments))
						{
							string text = string.Format("Skipping entry for User {0}. PermissionLevel {1}. FreeBusyAccess {2}.", calendarSharingPermissionInfo.ID, calendarFolderPermission.PermissionLevel, calendarFolderPermission.FreeBusyAccess);
							SharingPermissionLogger.LogEntry(base.MailboxSession, text, this.loggingContextIdentifier);
							GetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), text, new object[0]);
						}
						else
						{
							calendarSharingPermissionInfo.CurrentDetailLevel = calendarSharingDetailLevel.ToString();
							calendarSharingPermissionInfo.ViewPrivateAppointments = viewPrivateAppointments;
							if (calendarFolderPermission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal)
							{
								calendarSharingPermissionInfo.EmailAddress = new EmailAddressWrapper
								{
									Name = calendarFolderPermission.Principal.ExternalUser.Name,
									EmailAddress = calendarFolderPermission.Principal.ExternalUser.OriginalSmtpAddress.ToString(),
									RoutingType = "SMTP"
								};
								SharingPolicyAction sharingPolicyAction;
								if (calendarFolderPermission.Principal.ExternalUser.IsReachUser)
								{
									sharingPolicyAction = sharingPolicy.GetAllowedForAnonymousCalendarSharing();
									calendarSharingPermissionInfo.HandlerType = SharingHandlerType.Publishing.ToString();
								}
								else
								{
									sharingPolicyAction = sharingPolicy.GetAllowed(SmtpAddress.Parse(calendarSharingPermissionInfo.EmailAddress.EmailAddress).Domain);
									calendarSharingPermissionInfo.HandlerType = SharingHandlerType.Federated.ToString();
								}
								DetailLevelEnumType maxDetailLevel;
								if (!CalendarSharingPermissionsUtils.TryGetMaximumDetailLevel(sharingPolicyAction, out maxDetailLevel))
								{
									string text2 = string.Format("Sharing Policy Action: {0} - Sharing has been disabled for {1}", sharingPolicyAction, calendarFolderPermission.Principal.ToString());
									SharingPermissionLogger.LogEntry(base.MailboxSession, text2, this.loggingContextIdentifier);
									GetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), text2, new object[0]);
									continue;
								}
								calendarSharingPermissionInfo.AllowedDetailLevels = CalendarSharingPermissionsUtils.CalculateAllowedDetailLevels(maxDetailLevel, isSharingDefaultCalendar, PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal);
							}
							else if (calendarFolderPermission.Principal.Type.IsInternalUserPrincipal())
							{
								calendarSharingPermissionInfo.EmailAddress = ResolveNames.EmailAddressWrapperFromRecipient(calendarFolderPermission.Principal.ADRecipient);
								calendarSharingPermissionInfo.AllowedDetailLevels = CalendarSharingPermissionsUtils.CalculateAllowedDetailLevels(DetailLevelEnumType.Editor, isSharingDefaultCalendar, PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal);
								calendarSharingPermissionInfo.HandlerType = SharingHandlerType.Internal.ToString();
							}
							if (calendarSharingPermissionInfo.EmailAddress != null)
							{
								calendarSharingPermissionInfo.IsInsideOrganization = CalendarSharingPermissionsUtils.CheckIfRecipientDomainIsInternal(this.OrganizationId, SmtpAddress.Parse(calendarSharingPermissionInfo.EmailAddress.EmailAddress).Domain);
							}
							list.Add(calendarSharingPermissionInfo);
						}
					}
				}
			}
			PublishedCalendar publishedCalendar = null;
			try
			{
				if (getCalendarSharingPermissionsResponse.IsAnonymousSharingEnabled && PublishedCalendar.TryGetPublishedCalendar(mailboxSession, this.CaldendarFolderId, new ObscureKind?(ObscureKind.Normal), out publishedCalendar))
				{
					list.Add(new CalendarSharingPermissionInfo
					{
						FromPermissionsTable = false,
						CurrentDetailLevel = publishedCalendar.DetailLevel.ToString(),
						AllowedDetailLevels = CalendarSharingPermissionsUtils.CalculateAllowedDetailLevels(detailLevelEnumType, true, PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal),
						BrowseUrl = publishedCalendar.BrowseUrl,
						ICalUrl = publishedCalendar.ICalUrl
					});
				}
			}
			finally
			{
				if (publishedCalendar != null)
				{
					publishedCalendar.Dispose();
				}
			}
			getCalendarSharingPermissionsResponse.CurrentDeliveryOption = delegateUserCollectionHandler.GetMeetingRequestDeliveryOptionForDelegateUsers();
			getCalendarSharingPermissionsResponse.Recipients = list.ToArray();
			return getCalendarSharingPermissionsResponse;
		}

		internal static void TraceDebug(int hashCode, string messageFormat, params object[] args)
		{
			ExTraceGlobals.GetCalendarSharingPermissionsCallTracer.TraceDebug((long)hashCode, messageFormat, args);
		}

		private readonly string loggingContextIdentifier;

		private readonly ADRecipientSessionContext adRecipientSessionContext;
	}
}
