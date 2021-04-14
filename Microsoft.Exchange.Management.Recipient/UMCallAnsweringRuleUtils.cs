using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class UMCallAnsweringRuleUtils
	{
		public static bool IsKnownException(Exception ex)
		{
			return ex is StoragePermanentException;
		}

		public static void DisposeCallAnsweringRuleDataProvider(IConfigDataProvider provider)
		{
			UMCallAnsweringRuleDataProvider umcallAnsweringRuleDataProvider = provider as UMCallAnsweringRuleDataProvider;
			if (provider != null)
			{
				umcallAnsweringRuleDataProvider.Dispose();
			}
		}

		public static IConfigDataProvider GetDataProviderForCallAnsweringRuleTasks(UMCallAnsweringRuleIdParameter identityParam, MailboxIdParameter mailboxParam, ADSessionSettings sessionSettings, IRecipientSession globalCatalogSession, ADObjectId executingUserId, string clientString, DataAccessHelper.GetDataObjectDelegate getDataObjectDelegate, Task.TaskErrorLoggingDelegate errorLogger)
		{
			ValidateArgument.NotNull(sessionSettings, "sessionSettings");
			ValidateArgument.NotNull(globalCatalogSession, "globalCatalogSession");
			ValidateArgument.NotNull(getDataObjectDelegate, "getDataObjectDelegate");
			ValidateArgument.NotNull(errorLogger, "errorLogger");
			ValidateArgument.NotNullOrEmpty(clientString, "clientString");
			MailboxIdParameter mailboxIdParameter = null;
			ADUser aduser = null;
			if (identityParam != null)
			{
				if (identityParam.CallAnsweringRuleId != null)
				{
					mailboxIdParameter = new MailboxIdParameter(identityParam.CallAnsweringRuleId.MailboxOwnerId);
				}
				else
				{
					mailboxIdParameter = identityParam.RawMailbox;
				}
			}
			if (mailboxIdParameter != null && mailboxParam != null)
			{
				errorLogger(new InvalidOperationException(Strings.ErrorConflictingMailboxes), ErrorCategory.InvalidOperation, identityParam);
			}
			if (mailboxIdParameter == null && executingUserId == null)
			{
				errorLogger(new RecipientTaskException(Strings.ErrorParameterRequired("Mailbox")), ErrorCategory.InvalidOperation, null);
			}
			if (mailboxIdParameter == null)
			{
				mailboxIdParameter = (mailboxParam ?? new MailboxIdParameter(executingUserId));
			}
			aduser = (ADUser)getDataObjectDelegate(mailboxIdParameter, globalCatalogSession, null, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			if (identityParam != null && identityParam.CallAnsweringRuleId == null)
			{
				identityParam.CallAnsweringRuleId = new UMCallAnsweringRuleId(aduser.Id, (identityParam.RawRuleGuid != null) ? identityParam.RawRuleGuid.Value : Guid.Empty);
			}
			ADScopeException ex;
			if (!globalCatalogSession.TryVerifyIsWithinScopes(aduser, true, out ex))
			{
				errorLogger(new InvalidOperationException(Strings.ErrorCannotChangeMailboxOutOfWriteScope(aduser.Identity.ToString(), (ex == null) ? string.Empty : ex.Message), ex), ErrorCategory.InvalidOperation, aduser.Identity);
			}
			UMCallAnsweringRuleDataProvider result = null;
			try
			{
				result = new UMCallAnsweringRuleDataProvider(sessionSettings, aduser, clientString);
			}
			catch (UMRecipientValidationException)
			{
				errorLogger(new InvalidOperationException(Strings.MailboxNotUmEnabled(aduser.Identity.ToString())), ErrorCategory.InvalidOperation, aduser.Identity);
			}
			return result;
		}
	}
}
