using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class DelegateCommandBase<RequestType> : MultiStepServiceCommand<RequestType, DelegateUserType> where RequestType : BaseDelegateRequest
	{
		public DelegateCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		internal static void ValidateEmailAddress(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new MissingInformationEmailAddressException((CoreResources.IDs)2555117076U);
			}
			Util.ValidateSmtpAddress(emailAddress);
		}

		internal override void PreExecuteCommand()
		{
			this.InitializeXsoDelegateUsers();
		}

		protected void InitializeXsoDelegateUsers()
		{
			RequestType request = base.Request;
			string emailAddress = request.Mailbox.EmailAddress;
			DelegateCommandBase<RequestType>.ValidateEmailAddress(emailAddress);
			MailboxSession mailboxSession = base.GetMailboxSession(emailAddress);
			this.xsoDelegateUsers = new DelegateUserCollection(mailboxSession);
		}

		protected DelegateUserCollection xsoDelegateUsers;
	}
}
