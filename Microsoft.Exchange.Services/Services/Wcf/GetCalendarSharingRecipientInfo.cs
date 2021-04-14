using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetCalendarSharingRecipientInfo : CalendarActionBase<GetCalendarSharingRecipientInfoResponse>
	{
		private GetCalendarSharingRecipientInfoRequest Request { get; set; }

		private ExchangePrincipal AccessingPrincipal { get; set; }

		public GetCalendarSharingRecipientInfo(MailboxSession session, GetCalendarSharingRecipientInfoRequest request, ExchangePrincipal accessingPrincipal) : base(session)
		{
			this.Request = request;
			this.sharingProviderLocator = new SharingProviderLocator(accessingPrincipal, () => session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
			this.AccessingPrincipal = accessingPrincipal;
		}

		public override GetCalendarSharingRecipientInfoResponse Execute()
		{
			this.TraceDebug("Internal Execute", new object[0]);
			List<CalendarSharingRecipientInfo> list = new List<CalendarSharingRecipientInfo>(this.Request.Recipients.Count);
			foreach (KeyValuePair<SmtpAddress, ADRecipient> keyValuePair in this.Request.Recipients)
			{
				this.TraceDebug("Get Response object for {0}", new object[]
				{
					keyValuePair.Key
				});
				list.Add(this.CreateResponseRecipientInfo(keyValuePair.Key, keyValuePair.Value));
			}
			return new GetCalendarSharingRecipientInfoResponse
			{
				Recipients = list.ToArray()
			};
		}

		private CalendarSharingRecipientInfo CreateResponseRecipientInfo(SmtpAddress address, ADRecipient adRecipient)
		{
			EmailAddressWrapper emailAddressWrapper;
			if (adRecipient != null)
			{
				emailAddressWrapper = ResolveNames.EmailAddressWrapperFromRecipient(adRecipient);
			}
			else
			{
				emailAddressWrapper = new EmailAddressWrapper();
				emailAddressWrapper.EmailAddress = address.ToString();
				emailAddressWrapper.Name = emailAddressWrapper.EmailAddress;
				emailAddressWrapper.RoutingType = "SMTP";
				emailAddressWrapper.MailboxType = MailboxHelper.MailboxTypeType.Unknown.ToString();
			}
			CalendarSharingRecipientInfo calendarSharingRecipientInfo = new CalendarSharingRecipientInfo
			{
				EmailAddress = emailAddressWrapper
			};
			MailboxSession mailboxSession = base.MailboxSession;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			bool isSharingDefaultCalendar = defaultFolderId.Equals(this.Request.CalendarStoreId);
			SharingProvider sharingProvider;
			DetailLevelEnumType detailLevelEnumType;
			if (this.sharingProviderLocator.TryGetProvider(address, adRecipient, new FrontEndLocator(), out sharingProvider, out detailLevelEnumType))
			{
				if (sharingProvider == SharingProvider.SharingProviderInternal)
				{
					calendarSharingRecipientInfo.IsInsideOrganization = true;
					calendarSharingRecipientInfo.HandlerType = SharingHandlerType.Internal.ToString();
					calendarSharingRecipientInfo.AllowedDetailLevels = CalendarSharingPermissionsUtils.CalculateAllowedDetailLevels(detailLevelEnumType, isSharingDefaultCalendar, PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal);
				}
				else if (sharingProvider == SharingProvider.SharingProviderExternal)
				{
					calendarSharingRecipientInfo.IsInsideOrganization = CalendarSharingPermissionsUtils.CheckIfRecipientDomainIsInternal(this.AccessingPrincipal.MailboxInfo.OrganizationId, address.Domain);
					calendarSharingRecipientInfo.HandlerType = SharingHandlerType.Federated.ToString();
					calendarSharingRecipientInfo.AllowedDetailLevels = CalendarSharingPermissionsUtils.CalculateAllowedDetailLevels(detailLevelEnumType, isSharingDefaultCalendar, PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal);
				}
				else if (sharingProvider == SharingProvider.SharingProviderPublishReach)
				{
					calendarSharingRecipientInfo.IsInsideOrganization = CalendarSharingPermissionsUtils.CheckIfRecipientDomainIsInternal(this.AccessingPrincipal.MailboxInfo.OrganizationId, address.Domain);
					calendarSharingRecipientInfo.HandlerType = SharingHandlerType.Publishing.ToString();
					calendarSharingRecipientInfo.AllowedDetailLevels = CalendarSharingPermissionsUtils.CalculateAllowedDetailLevels(detailLevelEnumType, isSharingDefaultCalendar, PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal);
				}
				else
				{
					if (sharingProvider != SharingProvider.SharingProviderConsumer)
					{
						throw new NotSupportedException(sharingProvider.ToString());
					}
					calendarSharingRecipientInfo.IsInsideOrganization = false;
					calendarSharingRecipientInfo.HandlerType = SharingHandlerType.Consumer.ToString();
					calendarSharingRecipientInfo.AllowedDetailLevels = new string[]
					{
						CalendarSharingDetailLevel.AvailabilityOnly.ToString(),
						CalendarSharingDetailLevel.LimitedDetails.ToString(),
						CalendarSharingDetailLevel.FullDetails.ToString(),
						CalendarSharingDetailLevel.Editor.ToString(),
						CalendarSharingDetailLevel.Delegate.ToString()
					};
				}
				CalendarSharingDetailLevel calendarSharingDetailLevel = CalendarSharingPermissionsUtils.ConvertToCalendarSharingDetailLevelEnum(detailLevelEnumType, isSharingDefaultCalendar);
				if (calendarSharingDetailLevel > CalendarSharingDetailLevel.FullDetails)
				{
					calendarSharingDetailLevel = CalendarSharingDetailLevel.FullDetails;
				}
				calendarSharingRecipientInfo.CurrentDetailLevel = calendarSharingDetailLevel.ToString();
			}
			return calendarSharingRecipientInfo;
		}

		private void TraceDebug(string messageFormat, params object[] args)
		{
			ExTraceGlobals.GetCalendarSharingRecipientInfoCallTracer.TraceDebug((long)this.GetHashCode(), messageFormat, args);
		}

		private readonly SharingProviderLocator sharingProviderLocator;
	}
}
