using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetCalendarSharingPermissions : CalendarActionBase<CalendarActionResponse>
	{
		private SetCalendarSharingPermissionsRequest Request { get; set; }

		private StoreObjectId CalendarFolderId { get; set; }

		private ADObjectId AccessingPrincipalADObjectId { get; set; }

		private ExchangePrincipal AccessingPrincipal { get; set; }

		public SetCalendarSharingPermissions(MailboxSession session, SetCalendarSharingPermissionsRequest request, ADObjectId adObjectId, ADRecipientSessionContext adRecipientSessionContext, StoreObjectId calendarFolderId, ExchangePrincipal accessingPrincipal) : base(session)
		{
			this.Request = request;
			this.AccessingPrincipalADObjectId = adObjectId;
			this.adRecipientSessionContext = adRecipientSessionContext;
			this.CalendarFolderId = calendarFolderId;
			this.AccessingPrincipal = accessingPrincipal;
			this.loggingContextIdentifier = Guid.NewGuid().ToString();
		}

		public override CalendarActionResponse Execute()
		{
			CalendarActionResponse calendarActionResponse = new CalendarActionResponse();
			MailboxSession mailboxSession = base.MailboxSession;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			bool flag = defaultFolderId.Equals(this.CalendarFolderId);
			SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("Set calendar sharing permission; calendar type: {0}", flag ? "Primary" : "Seconday"), this.loggingContextIdentifier);
			if (flag)
			{
				CalendarActionError calendarActionError = this.RevokeDeletedDelegateUsersPermission();
				if (calendarActionError == CalendarActionError.CalendarActionDelegateManagementError)
				{
					SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Unexpected error while performing the requested operation on delegate users.", new object[0]);
					calendarActionResponse.ErrorCode = CalendarActionError.CalendarActionDelegateManagementError;
					calendarActionResponse.WasSuccessful = false;
					return calendarActionResponse;
				}
			}
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSession, this.Request.CalendarStoreId))
			{
				CalendarFolderPermissionSet permissionSet = calendarFolder.GetPermissionSet();
				SharingPolicy sharingPolicy = DirectoryHelper.ReadSharingPolicy(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, mailboxSession.MailboxOwner.MailboxInfo.IsArchive, mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
				List<PermissionSecurityPrincipal> list = new List<PermissionSecurityPrincipal>();
				List<SetCalendarSharingPermissions.CalendarSharingInfo> list2 = new List<SetCalendarSharingPermissions.CalendarSharingInfo>();
				List<SetCalendarSharingPermissions.CalendarSharingInfo> list3 = null;
				DelegateUserCollectionHandler delegateUserCollectionHandler = null;
				bool flag2 = false;
				if (flag)
				{
					delegateUserCollectionHandler = new DelegateUserCollectionHandler(mailboxSession, this.adRecipientSessionContext);
					list3 = new List<SetCalendarSharingPermissions.CalendarSharingInfo>();
				}
				foreach (CalendarFolderPermission calendarFolderPermission in permissionSet)
				{
					PermissionSecurityPrincipal permissionSecurityPrincipal;
					if (this.ValidateFolderPermission(calendarFolderPermission, out permissionSecurityPrincipal))
					{
						if (CalendarSharingPermissionsUtils.ShouldSkipPermission(calendarFolderPermission, flag))
						{
							SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), string.Format("Skipping permission: {0}; Default (primary) calendar: {1}", calendarFolderPermission.PermissionLevel, flag), new object[0]);
						}
						else
						{
							CalendarSharingPermissionInfo calendarSharingPermissionInfo = this.Request.RecipientGivenIndexString(permissionSecurityPrincipal.IndexString);
							if (calendarSharingPermissionInfo == null)
							{
								list.Add(calendarFolderPermission.Principal);
								SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), string.Format("Client has removed the permissions for the user {0}", permissionSecurityPrincipal.Type.IsInternalUserPrincipal() ? permissionSecurityPrincipal.ADRecipient.PrimarySmtpAddress.ToString() : permissionSecurityPrincipal.ToString()), new object[0]);
							}
							else
							{
								CalendarSharingDetailLevel calendarSharingDetailLevel;
								Enum.TryParse<CalendarSharingDetailLevel>(calendarSharingPermissionInfo.CurrentDetailLevel, out calendarSharingDetailLevel);
								CalendarSharingDetailLevel calendarSharingDetailLevel2 = CalendarSharingPermissionsUtils.ConvertToCalendarSharingDetailLevelEnum(CalendarSharingPermissionsUtils.GetMaximumDetailLevel(sharingPolicy, calendarFolderPermission), flag);
								if (calendarSharingDetailLevel > calendarSharingDetailLevel2)
								{
									SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("Requested level of detail: {0}, which is greater than what is allowed by the policy. Reverting to maximum allowed {1}.", calendarSharingDetailLevel, calendarSharingDetailLevel2), this.loggingContextIdentifier);
									SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Caller asked for a level of detail of {0}, which is greater than what is allowed by the policy. Reverting to maximum allowed {1}.", new object[]
									{
										calendarSharingDetailLevel,
										calendarSharingDetailLevel2
									});
									calendarSharingDetailLevel = calendarSharingDetailLevel2;
								}
								CalendarSharingDetailLevel calendarSharingDetailLevel3;
								bool flag3;
								if (!CalendarSharingPermissionsUtils.GetSharingDetailLevelFromPermissionLevel(calendarFolderPermission, flag, delegateUserCollectionHandler, out calendarSharingDetailLevel3, out flag3))
								{
									SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("OWA does not manage this permission level: {0}. Principal: {1}", calendarFolderPermission.PermissionLevel, permissionSecurityPrincipal.Type.IsInternalUserPrincipal() ? permissionSecurityPrincipal.ADRecipient.PrimarySmtpAddress.ToString() : permissionSecurityPrincipal.ToString()), this.loggingContextIdentifier);
								}
								else
								{
									if (calendarSharingDetailLevel3 != calendarSharingDetailLevel)
									{
										list2.Add(new SetCalendarSharingPermissions.CalendarSharingInfo
										{
											CurrentDetailLevel = calendarSharingDetailLevel,
											ViewPrivateAppointments = calendarSharingPermissionInfo.ViewPrivateAppointments,
											EmailAddress = SetCalendarSharingPermissions.EmailAddressWrapperFromPrincipal(calendarFolderPermission.Principal)
										});
									}
									else if (flag && calendarSharingDetailLevel == CalendarSharingDetailLevel.Delegate && flag3 != calendarSharingPermissionInfo.ViewPrivateAppointments)
									{
										list3.Add(new SetCalendarSharingPermissions.CalendarSharingInfo
										{
											CurrentDetailLevel = calendarSharingDetailLevel,
											ViewPrivateAppointments = calendarSharingPermissionInfo.ViewPrivateAppointments,
											AdRecipient = calendarFolderPermission.Principal.ADRecipient
										});
									}
									if (!flag2)
									{
										flag2 = (calendarSharingDetailLevel == CalendarSharingDetailLevel.Delegate);
									}
								}
							}
						}
					}
				}
				foreach (PermissionSecurityPrincipal permissionSecurityPrincipal2 in list)
				{
					SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("Calendar sharing permission removed for: {0}", permissionSecurityPrincipal2.Type.IsInternalUserPrincipal() ? permissionSecurityPrincipal2.ADRecipient.PrimarySmtpAddress.ToString() : permissionSecurityPrincipal2.ToString()), this.loggingContextIdentifier);
					permissionSet.RemoveEntry(permissionSecurityPrincipal2);
				}
				this.SavePublishedCalendarOptions(mailboxSession, sharingPolicy);
				if (flag && list3.Count > 0)
				{
					this.ProcessViewPrivateAppointmentStatus(list3);
				}
				if (list2.Count > 0)
				{
					SharingPermissionLogger.LogEntry(base.MailboxSession, "Creating new calendar sharing invites for all updated permissions", this.loggingContextIdentifier);
					CalendarShareInviteResponse calendarShareInviteResponse = this.CreateNewPermissionsAndSendEmailNotification(list2);
					if (calendarShareInviteResponse.FailureResponses.Length > 0)
					{
						SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Unexpected error while setting permissions and sending calendar sharing invite.", new object[0]);
						calendarActionResponse.ErrorCode = CalendarActionError.CalendarActionSendSharingInviteError;
						calendarActionResponse.WasSuccessful = false;
						return calendarActionResponse;
					}
				}
				if (flag && flag2)
				{
					this.SetGlobalDelegateMeetingDelivery(this.Request.SelectedDeliveryOption);
				}
				FolderSaveResult folderSaveResult = calendarFolder.Save();
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Folder save operation didn't succeed.", new object[0]);
					calendarActionResponse.ErrorCode = CalendarActionError.CalendarActionCannotSaveCalendar;
					calendarActionResponse.WasSuccessful = false;
					return calendarActionResponse;
				}
				SharingPermissionLogger.LogEntry(base.MailboxSession, "Calendar sharing permissions saved successfully", this.loggingContextIdentifier);
			}
			return calendarActionResponse;
		}

		private bool ValidateFolderPermission(CalendarFolderPermission permission, out PermissionSecurityPrincipal principal)
		{
			principal = permission.Principal;
			if (principal == null || (principal.Type.IsInternalUserPrincipal() && principal.ADRecipient == null))
			{
				SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("PermissionSecurityPrincipal is null or ADRecipient is null for an internal user. Permission: {0}", permission), this.loggingContextIdentifier);
				return false;
			}
			return true;
		}

		private static EmailAddressWrapper EmailAddressWrapperFromPrincipal(PermissionSecurityPrincipal principal)
		{
			EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
			SmtpAddress smtpAddress;
			if (principal.Type.IsInternalUserPrincipal())
			{
				emailAddressWrapper.Name = principal.ADRecipient.DisplayName;
				smtpAddress = principal.ADRecipient.PrimarySmtpAddress;
			}
			else
			{
				emailAddressWrapper.Name = principal.ExternalUser.Name;
				smtpAddress = principal.ExternalUser.OriginalSmtpAddress;
			}
			emailAddressWrapper.EmailAddress = smtpAddress.ToString();
			emailAddressWrapper.RoutingType = "SMTP";
			return emailAddressWrapper;
		}

		private CalendarActionError RevokeDeletedDelegateUsersPermission()
		{
			CalendarActionError result = CalendarActionError.None;
			new Dictionary<ADRecipient, CalendarSharingDetailLevel>();
			MailboxSession mailboxSession = base.MailboxSession;
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSession, this.Request.CalendarStoreId))
			{
				CalendarFolderPermissionSet permissionSet = calendarFolder.GetPermissionSet();
				try
				{
					DelegateUserCollectionHandler delegateUserCollectionHandler = new DelegateUserCollectionHandler(mailboxSession, this.adRecipientSessionContext);
					bool flag = false;
					foreach (CalendarFolderPermission permission in permissionSet)
					{
						PermissionSecurityPrincipal permissionSecurityPrincipal;
						if (this.ValidateFolderPermission(permission, out permissionSecurityPrincipal) && permissionSecurityPrincipal.Type.IsInternalUserPrincipal() && this.Request.RecipientGivenIndexString(permissionSecurityPrincipal.IndexString) == null)
						{
							DelegateUser delegateUser = delegateUserCollectionHandler.GetDelegateUser(permissionSecurityPrincipal.ADRecipient);
							if (delegateUser != null)
							{
								delegateUserCollectionHandler.RemoveDelegate(delegateUser);
								flag = true;
								SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("Delegate permission removal is requested for {0}", permissionSecurityPrincipal.ADRecipient.PrimarySmtpAddress), this.loggingContextIdentifier);
							}
						}
					}
					if (flag)
					{
						delegateUserCollectionHandler.SaveDelegate(false);
						SharingPermissionLogger.LogEntry(base.MailboxSession, "All remove requested delegates have been removed successfully!", this.loggingContextIdentifier);
					}
				}
				catch (DelegateExceptionNotDelegate delegateExceptionNotDelegate)
				{
					SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), delegateExceptionNotDelegate.Message, new object[0]);
					result = CalendarActionError.CalendarActionDelegateManagementError;
				}
			}
			return result;
		}

		private void SavePublishedCalendarOptions(MailboxSession session, SharingPolicy sharingPolicy)
		{
			if (this.Request.PublishedCalendarPermissions != null)
			{
				PublishedCalendar publishedCalendar = null;
				try
				{
					if (PublishedCalendar.TryGetPublishedCalendar(session, this.Request.CalendarStoreId, new ObscureKind?(ObscureKind.Normal), out publishedCalendar))
					{
						DetailLevelEnumType adjustedDetailLevel;
						Enum.TryParse<DetailLevelEnumType>(this.Request.PublishedCalendarPermissions.CurrentDetailLevel, out adjustedDetailLevel);
						adjustedDetailLevel = CalendarSharingPermissionsUtils.GetAdjustedDetailLevel(sharingPolicy.GetAllowedForAnonymousCalendarSharing(), adjustedDetailLevel);
						if (publishedCalendar.DetailLevel != adjustedDetailLevel)
						{
							publishedCalendar.PublishedOptions.MailboxFolderId = new MailboxFolderId(this.AccessingPrincipalADObjectId, this.Request.CalendarStoreId, null);
							publishedCalendar.PublishedOptions.DetailLevel = adjustedDetailLevel;
							publishedCalendar.SavePublishedOptions();
						}
					}
					else
					{
						SetCalendarSharingPermissions.TraceDebug(this.GetHashCode(), "Calendar isn't published. Ignoring Everyone entry.", new object[0]);
					}
				}
				finally
				{
					if (publishedCalendar != null)
					{
						publishedCalendar.Dispose();
					}
				}
			}
		}

		private CalendarShareInviteResponse CreateNewPermissionsAndSendEmailNotification(List<SetCalendarSharingPermissions.CalendarSharingInfo> calendarSharingInfoCollection)
		{
			CalendarShareInviteResponse calendarShareInviteResponse = new CalendarShareInviteResponse();
			CalendarShareInviteRequest calendarShareInviteRequest = new CalendarShareInviteRequest();
			Microsoft.Exchange.Services.Core.Types.ItemId itemIdFromStoreId = IdConverter.GetItemIdFromStoreId(this.CalendarFolderId, new MailboxId(base.MailboxSession.MailboxGuid));
			calendarShareInviteRequest.CalendarId = new Microsoft.Exchange.Services.Wcf.Types.ItemId
			{
				Id = itemIdFromStoreId.Id,
				ChangeKey = itemIdFromStoreId.ChangeKey
			};
			calendarShareInviteRequest.SharingRecipients = new CalendarSharingRecipient[calendarSharingInfoCollection.Count];
			calendarShareInviteRequest.Body = new BodyContentType
			{
				BodyType = BodyType.HTML
			};
			calendarShareInviteRequest.Subject = ClientStrings.SharingInvitationUpdatedSubjectLine.ToString(base.MailboxSession.PreferedCulture);
			int num = 0;
			foreach (SetCalendarSharingPermissions.CalendarSharingInfo calendarSharingInfo in calendarSharingInfoCollection)
			{
				CalendarSharingRecipient calendarSharingRecipient = new CalendarSharingRecipient();
				calendarSharingRecipient.EmailAddress = calendarSharingInfo.EmailAddress;
				calendarSharingRecipient.DetailLevel = calendarSharingInfo.CurrentDetailLevel.ToString();
				calendarSharingRecipient.ViewPrivateAppointments = calendarSharingInfo.ViewPrivateAppointments;
				calendarShareInviteRequest.SharingRecipients[num] = calendarSharingRecipient;
				num++;
			}
			calendarShareInviteRequest.ValidateRequest(base.MailboxSession, this.adRecipientSessionContext);
			SendCalendarSharingInvite sendCalendarSharingInvite = new SendCalendarSharingInvite(base.MailboxSession, calendarShareInviteRequest, this.AccessingPrincipal, this.adRecipientSessionContext, this.loggingContextIdentifier);
			return sendCalendarSharingInvite.Execute();
		}

		private void ProcessViewPrivateAppointmentStatus(List<SetCalendarSharingPermissions.CalendarSharingInfo> CalendarSharingInfoCollection)
		{
			DelegateUserCollectionHandler delegateUserCollectionHandler = new DelegateUserCollectionHandler(base.MailboxSession, this.adRecipientSessionContext);
			bool flag = false;
			foreach (SetCalendarSharingPermissions.CalendarSharingInfo calendarSharingInfo in CalendarSharingInfoCollection)
			{
				if (calendarSharingInfo.CurrentDetailLevel == CalendarSharingDetailLevel.Delegate)
				{
					ADRecipient adRecipient = calendarSharingInfo.AdRecipient;
					DelegateUser delegateUser = delegateUserCollectionHandler.GetDelegateUser(adRecipient);
					if (delegateUser != null)
					{
						bool viewPrivateAppointments = calendarSharingInfo.ViewPrivateAppointments;
						if (delegateUser.CanViewPrivateItems != viewPrivateAppointments)
						{
							delegateUser.CanViewPrivateItems = viewPrivateAppointments;
							flag = true;
							SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("Set view private appointment option: {0} for user: {1}", viewPrivateAppointments, adRecipient.PrimarySmtpAddress), this.loggingContextIdentifier);
						}
					}
				}
			}
			if (flag)
			{
				delegateUserCollectionHandler.SaveDelegate(false);
			}
		}

		private void SetGlobalDelegateMeetingDelivery(DeliverMeetingRequestsType delegateMeetingRequestDeliveryOption)
		{
			DelegateUserCollectionHandler delegateUserCollectionHandler = new DelegateUserCollectionHandler(base.MailboxSession, this.adRecipientSessionContext);
			DeliverMeetingRequestsType meetingRequestDeliveryOptionForDelegateUsers = delegateUserCollectionHandler.GetMeetingRequestDeliveryOptionForDelegateUsers();
			bool flag = meetingRequestDeliveryOptionForDelegateUsers != this.Request.SelectedDeliveryOption;
			if (flag)
			{
				delegateUserCollectionHandler.SetDelegateOptions(delegateMeetingRequestDeliveryOption);
				SharingPermissionLogger.LogEntry(base.MailboxSession, string.Format("Set view global delegate meeting request delivery to: {0}", delegateMeetingRequestDeliveryOption), this.loggingContextIdentifier);
				delegateUserCollectionHandler.SaveDelegate(false);
			}
		}

		internal static void TraceDebug(int hashCode, string messageFormat, params object[] args)
		{
			ExTraceGlobals.SetCalendarSharingPermissionsCallTracer.TraceDebug((long)hashCode, messageFormat, args);
		}

		private readonly ADRecipientSessionContext adRecipientSessionContext;

		private readonly string loggingContextIdentifier;

		private class CalendarSharingInfo
		{
			public CalendarSharingDetailLevel CurrentDetailLevel { get; set; }

			public bool ViewPrivateAppointments { get; set; }

			public EmailAddressWrapper EmailAddress { get; set; }

			public ADRecipient AdRecipient { get; set; }
		}
	}
}
