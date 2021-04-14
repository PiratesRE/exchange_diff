using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal class DelegateUserCollectionHandler
	{
		public DelegateUserCollectionHandler(MailboxSession session, ADRecipientSessionContext adRecipientSessionContext)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(adRecipientSessionContext, "adRecipientSessionContext");
			this.MailboxOwner = session.MailboxOwner;
			this.xsoDelegateUsers = new DelegateUserCollection(session);
			this.adRecipientSessionContext = adRecipientSessionContext;
		}

		public DelegateUser AddDelegate(DelegateUserType delegateUser)
		{
			Dictionary<DefaultFolderType, PermissionLevel> folderPermissions = new Dictionary<DefaultFolderType, PermissionLevel>();
			return this.AddDelegate(delegateUser, folderPermissions);
		}

		public DelegateUser UpdateDelegate(DelegateUserType delegateUser)
		{
			Util.ThrowOnNullArgument(delegateUser, "delegateUser");
			DelegateUser delegateUser2 = DelegateUtilities.GetDelegateUser(delegateUser.UserId, this.xsoDelegateUsers, this.adRecipientSessionContext);
			DelegateUtilities.UpdateXsoDelegateUser(delegateUser2, delegateUser);
			return delegateUser2;
		}

		public void RemoveDelegate(DelegateUser xsoDelegateUser)
		{
			Util.ThrowOnNullArgument(xsoDelegateUser, "xsoDelegateUser");
			this.xsoDelegateUsers.Remove(xsoDelegateUser);
		}

		public DelegateUser GetDelegateUser(ADRecipient adRecipient)
		{
			Util.ThrowOnNullArgument(adRecipient, "adRecipient");
			return DelegateUtilities.GetDelegateUser(adRecipient, this.xsoDelegateUsers);
		}

		public void RemoveDelegate(UserId user)
		{
			Util.ThrowOnNullArgument(user, "user");
			DelegateUser delegateUser = DelegateUtilities.GetDelegateUser(user, this.xsoDelegateUsers, this.adRecipientSessionContext);
			this.xsoDelegateUsers.Remove(delegateUser);
		}

		public void SetDelegateOptions(DeliverMeetingRequestsType deliverMeetingRequests)
		{
			this.xsoDelegateUsers.DelegateRuleType = DelegateUtilities.ConvertToDeliverRuleType(deliverMeetingRequests);
		}

		public DeliverMeetingRequestsType GetMeetingRequestDeliveryOptionForDelegateUsers()
		{
			return DelegateUtilities.ConvertToDeliverMeetingRequestType(this.xsoDelegateUsers.DelegateRuleType);
		}

		public int DelegateUsersCount
		{
			get
			{
				return this.xsoDelegateUsers.Count;
			}
		}

		public DelegateUserCollectionSaveResult SaveDelegate(bool removeUnknown)
		{
			return this.xsoDelegateUsers.Save(removeUnknown);
		}

		public void AddDelegateWithCalendarEditorPermission(UserId user, bool viewPrivateAppointments)
		{
			Util.ThrowOnNullArgument(user, "user");
			this.AddDelegate(new DelegateUserType(user)
			{
				DelegatePermissions = new DelegatePermissionsType
				{
					CalendarFolderPermissionLevel = DelegateFolderPermissionLevelType.Editor
				},
				ViewPrivateItems = new bool?(viewPrivateAppointments),
				ReceiveCopiesOfMeetingMessages = new bool?(true)
			}, new Dictionary<DefaultFolderType, PermissionLevel>
			{
				{
					DefaultFolderType.Calendar,
					PermissionLevel.Editor
				}
			});
		}

		private DelegateUser AddDelegate(DelegateUserType delegateUser, Dictionary<DefaultFolderType, PermissionLevel> folderPermissions)
		{
			Util.ThrowOnNullArgument(delegateUser, "delegateUser");
			DelegateUser delegateUser2 = null;
			try
			{
				ExchangePrincipal exchangePrincipal = DelegateUtilities.GetExchangePrincipal(delegateUser.UserId, this.adRecipientSessionContext);
				delegateUser2 = DelegateUser.Create(exchangePrincipal, folderPermissions);
			}
			catch (DelegateExceptionInvalidDelegateUser)
			{
				VariantConfigurationSnapshot configuration = this.MailboxOwner.GetConfiguration();
				if (!configuration.DataStorage.CrossPremiseDelegate.Enabled)
				{
					throw;
				}
				delegateUser2 = DelegateUser.InternalCreate(delegateUser.UserId.DisplayName, delegateUser.UserId.PrimarySmtpAddress, folderPermissions);
			}
			DelegateUtilities.UpdateXsoDelegateUser(delegateUser2, delegateUser);
			try
			{
				this.xsoDelegateUsers.Add(delegateUser2);
			}
			catch (DelegateUserValidationException ex)
			{
				if (ex.Problem == DelegateValidationProblem.Duplicate)
				{
					throw new DelegateExceptionAlreadyExists(ex);
				}
				if (ex.Problem == DelegateValidationProblem.IsOwner)
				{
					throw new DelegateExceptionCannotAddOwner(ex);
				}
				throw new DelegateExceptionValidationFailed(ex);
			}
			return delegateUser2;
		}

		private readonly DelegateUserCollection xsoDelegateUsers;

		private readonly ADRecipientSessionContext adRecipientSessionContext;

		private readonly IExchangePrincipal MailboxOwner;
	}
}
