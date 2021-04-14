using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExceptionUtilities
	{
		public static bool IndicatesDatabaseDismount(Exception exception)
		{
			SyncUtilities.ThrowIfArgumentNull("exception", exception);
			return ExceptionUtilities.ApplyCheckToExceptionAndInnerException(exception, new Func<Exception, bool>(ExceptionUtilities.CheckExceptionTypeForDatabaseDismount));
		}

		public static bool IndicatesUserNotFound(Exception exception)
		{
			SyncUtilities.ThrowIfArgumentNull("exception", exception);
			return ExceptionUtilities.ApplyCheckToExceptionAndInnerException(exception, (Exception e) => ExceptionUtilities.IsMapiExceptionIndicatingUserNotFound(e) || ExceptionUtilities.IsStorageExceptionIndicatingUserNotFound(e));
		}

		public static bool ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(SyncLogSession syncLogSession, Exception exception)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("exception", exception);
			return ExceptionUtilities.ApplyCheckToExceptionAndInnerException(exception, (Exception e) => ExceptionUtilities.CheckPermanentMapiOrXsoExceptionTypeShouldBeTreatedAsTransient(syncLogSession, e));
		}

		private static bool ApplyCheckToExceptionAndInnerException(Exception exception, Func<Exception, bool> function)
		{
			return function(exception) || (exception.InnerException != null && function(exception.InnerException));
		}

		private static bool CheckPermanentMapiOrXsoExceptionTypeShouldBeTreatedAsTransient(SyncLogSession syncLogSession, Exception exception)
		{
			return ExceptionUtilities.CheckExceptionTypeForDatabaseDismount(exception) || ExceptionUtilities.IsMapiPermanentExceptionToBeTreatedAsTransient(exception) || ExceptionUtilities.IsStoragePermanentExceptionToBeTreatedAsTransient(exception);
		}

		private static bool CheckExceptionTypeForDatabaseDismount(Exception exception)
		{
			return exception is MapiExceptionExiting || exception is MapiExceptionMdbOffline || exception is SessionDeadException || exception is MapiExceptionServerPaused;
		}

		private static bool IsMapiPermanentExceptionToBeTreatedAsTransient(Exception exception)
		{
			return exception is MapiExceptionCallFailed || exception is MapiExceptionJetErrorFileNotFound || exception is MapiExceptionJetErrorInvalidSesid || exception is MapiExceptionJetErrorLogDiskFull || exception is MapiExceptionJetErrorReadVerifyFailure || exception is MapiExceptionMaxObjsExceeded || exception is MapiExceptionUnknownMailbox || exception is MapiExceptionDuplicateObject;
		}

		private static bool IsStoragePermanentExceptionToBeTreatedAsTransient(Exception exception)
		{
			return exception is ConnectionFailedPermanentException || exception is MailboxUnavailableException || exception is AccountDisabledException;
		}

		private static bool IsMapiExceptionIndicatingUserNotFound(Exception exception)
		{
			return exception is MapiExceptionUnknownUser;
		}

		private static bool IsStorageExceptionIndicatingUserNotFound(Exception exception)
		{
			return exception is ObjectNotFoundException || exception is WrongServerException || exception is MailboxUnavailableException || exception is CannotResolveExternalDirectoryOrganizationIdException;
		}
	}
}
