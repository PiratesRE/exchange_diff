using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.MdbCommon;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal static class XsoUtil
	{
		internal static string GetDefaultFolderName(MailboxSession session, StoreObjectId folderId)
		{
			DefaultFolderType defaultFolderType = session.IsDefaultFolderType(folderId);
			return XsoUtil.GetDefaultFolderName(defaultFolderType);
		}

		internal static string GetDefaultFolderName(DefaultFolderType defaultFolderType)
		{
			switch (defaultFolderType)
			{
			case DefaultFolderType.DeletedItems:
				return ClientStrings.DeletedItems;
			case DefaultFolderType.Drafts:
				return ClientStrings.Drafts;
			case DefaultFolderType.Inbox:
				return ClientStrings.Inbox;
			case DefaultFolderType.JunkEmail:
				return ClientStrings.JunkEmail;
			case DefaultFolderType.Journal:
			case DefaultFolderType.Notes:
			case DefaultFolderType.Tasks:
			case DefaultFolderType.Reminders:
				break;
			case DefaultFolderType.Outbox:
				return ClientStrings.Outbox;
			case DefaultFolderType.SentItems:
				return ClientStrings.SentItems;
			case DefaultFolderType.Conflicts:
				return ClientStrings.Conflicts;
			default:
				if (defaultFolderType == DefaultFolderType.RecoverableItemsDeletions)
				{
					return ClientStrings.DeletedItems;
				}
				break;
			}
			return "Other";
		}

		internal static AttachmentType ConvertFromXSOAttachmentType(AttachmentType attachmentType)
		{
			switch (attachmentType)
			{
			case AttachmentType.NoAttachment:
				return AttachmentType.NoAttachment;
			case AttachmentType.Stream:
				return AttachmentType.Stream;
			case AttachmentType.EmbeddedMessage:
				return AttachmentType.EmbeddedMessage;
			case AttachmentType.Ole:
				return AttachmentType.Ole;
			default:
				return AttachmentType.Unknown;
			}
		}

		internal static bool CheckResponseBasedOnType(object lastVerb, ItemResponseType responseType)
		{
			if (lastVerb is PropertyError)
			{
				return false;
			}
			int num = (int)lastVerb;
			if (responseType == ItemResponseType.RepliedTo)
			{
				return num == 103 || num == 102 || num == 108 || (num >= 1 && num <= 31);
			}
			if (responseType == ItemResponseType.Forwarded)
			{
				return num == 104;
			}
			throw new ArgumentException("Invalid responseType.");
		}

		internal static TResult MapXsoExceptions<TResult>(Func<TResult> call)
		{
			Exception innerException = null;
			try
			{
				return call();
			}
			catch (StorageTransientException ex)
			{
				innerException = ex;
			}
			catch (StoragePermanentException ex2)
			{
				innerException = ex2;
			}
			catch (TextConvertersException ex3)
			{
				innerException = ex3;
			}
			throw new OperationFailedException(innerException);
		}

		internal static void MapXsoExceptions(Action call)
		{
			Exception ex = null;
			try
			{
				call();
			}
			catch (StorageTransientException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			catch (TextConvertersException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				throw new OperationFailedException(ex);
			}
		}

		internal static UserConfiguration ResetModel(string userConfigurationName, UserConfigurationTypes userConfigType, MailboxSession session, bool deleteOld, IDiagnosticsSession diagnosticSession)
		{
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Inbox);
			OperationResult operationResult = OperationResult.Succeeded;
			Exception ex = null;
			if (deleteOld)
			{
				try
				{
					operationResult = session.UserConfigurationManager.DeleteFolderConfigurations(defaultFolderId, new string[]
					{
						userConfigurationName
					});
				}
				catch (ObjectNotFoundException ex2)
				{
					ex = ex2;
					if (diagnosticSession != null)
					{
						diagnosticSession.TraceDebug<string, ObjectNotFoundException>("FAI message '{0}' is missing. Exception: {1}", userConfigurationName, ex2);
					}
				}
				if (operationResult != OperationResult.Succeeded && ex == null)
				{
					if (diagnosticSession != null)
					{
						diagnosticSession.TraceError(string.Format("Deletion of user configuration (userConfiguration Name = {0}) failed. OperationResult = {1}. ObjectNotFoundException = {2}", userConfigurationName, operationResult.ToString(), ex), new object[0]);
					}
					throw new DeleteItemsException(string.Format("Deletion of user configuration (userConfiguration Name = {0}) failed. OperationResult = {1}. ObjectNotFoundException = {2}", userConfigurationName, operationResult.ToString(), ex));
				}
			}
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = session.UserConfigurationManager.CreateFolderConfiguration(userConfigurationName, userConfigType, defaultFolderId);
				userConfiguration.Save();
			}
			catch (Exception ex3)
			{
				if (diagnosticSession != null && !(ex3 is QuotaExceededException))
				{
					if (ex3 is StoragePermanentException)
					{
						diagnosticSession.SendInformationalWatsonReport(ex3, string.Format("Creation of user configuration failed (userConfiguration Name = {0}) deleteOld flag was {1}. Result of deletion of user configuration OperationResult = {2}. ObjectNotFoundException = {3}", new object[]
						{
							userConfigurationName,
							deleteOld,
							deleteOld ? operationResult.ToString() : "Not Applicable",
							ex
						}));
					}
					else
					{
						diagnosticSession.TraceError("Creation of user configuration failed (userConfiguration Name = {0}) deleteOld flag was {1}. Result of deletion of user configuration OperationResult = {2}. ObjectNotFoundException = {3}. Exception = {4}", new object[]
						{
							userConfigurationName,
							deleteOld,
							deleteOld ? operationResult.ToString() : "Not Applicable",
							ex,
							ex3.ToString()
						});
					}
				}
				if (userConfiguration != null)
				{
					userConfiguration.Dispose();
				}
				throw;
			}
			return userConfiguration;
		}

		internal static TReturnValue TranslateXsoExceptionsWithReturnValue<TReturnValue>(IDiagnosticsSession tracer, LocalizedString errorString, XsoUtil.XsoExceptionHandlingFlags flags, Func<TReturnValue> xsoCall)
		{
			TReturnValue result = default(TReturnValue);
			XsoUtil.TranslateXsoExceptions(tracer, errorString, flags, delegate()
			{
				result = xsoCall();
			});
			return result;
		}

		internal static TReturnValue TranslateXsoExceptionsWithReturnValue<TReturnValue>(IDiagnosticsSession tracer, LocalizedString errorString, Func<TReturnValue> xsoCall)
		{
			return XsoUtil.TranslateXsoExceptionsWithReturnValue<TReturnValue>(tracer, errorString, XsoUtil.XsoExceptionHandlingFlags.None, xsoCall);
		}

		internal static void TranslateXsoExceptions(IDiagnosticsSession tracer, LocalizedString errorString, Action xsoCall)
		{
			XsoUtil.TranslateXsoExceptions(tracer, errorString, XsoUtil.XsoExceptionHandlingFlags.None, xsoCall);
		}

		internal static void TranslateXsoExceptions(IDiagnosticsSession tracer, LocalizedString errorString, XsoUtil.XsoExceptionHandlingFlags flags, Action xsoCall)
		{
			try
			{
				xsoCall();
			}
			catch (ConnectionFailedTransientException ex)
			{
				XsoUtil.TraceAndThrowTransientException(tracer, errorString, ex);
			}
			catch (ConnectionFailedPermanentException ex2)
			{
				XsoUtil.TraceAndThrowPermanentException(tracer, errorString, ex2);
			}
			catch (MailboxUnavailableException ex3)
			{
				XsoUtil.TraceAndThrowPermanentException(tracer, errorString, ex3);
			}
			catch (ObjectNotFoundException ex4)
			{
				if ((flags & XsoUtil.XsoExceptionHandlingFlags.DoNotExpectObjectNotFound) == XsoUtil.XsoExceptionHandlingFlags.DoNotExpectObjectNotFound)
				{
					tracer.SendInformationalWatsonReport(ex4, null);
					if ((flags & XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions) == XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions)
					{
						XsoUtil.TraceAndThrowPermanentException(tracer, errorString, ex4);
					}
				}
				else
				{
					tracer.TraceDebug<LocalizedString, ObjectNotFoundException>("Error: {0}, exception: {1}", errorString, ex4);
				}
			}
			catch (CorruptDataException ex5)
			{
				if ((flags & XsoUtil.XsoExceptionHandlingFlags.DoNotExpectCorruptData) == XsoUtil.XsoExceptionHandlingFlags.DoNotExpectCorruptData)
				{
					tracer.SendInformationalWatsonReport(ex5, null);
					if ((flags & XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions) == XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions)
					{
						XsoUtil.TraceAndThrowPermanentException(tracer, errorString, ex5);
					}
				}
				else
				{
					tracer.TraceDebug<LocalizedString, CorruptDataException>("Error: {0}, exception: {1}", errorString, ex5);
				}
			}
			catch (AccessDeniedException ex6)
			{
				tracer.SendWatsonReport(ex6);
				XsoUtil.TraceAndThrowPermanentException(tracer, errorString, ex6);
			}
			catch (StoragePermanentException ex7)
			{
				if (ex7.GetType() != typeof(StoragePermanentException))
				{
					tracer.SendInformationalWatsonReport(ex7, null);
				}
				if ((flags & XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions) == XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions)
				{
					XsoUtil.TraceAndThrowPermanentException(tracer, errorString, ex7);
				}
			}
			catch (StorageTransientException ex8)
			{
				if (ex8.GetType() != typeof(StorageTransientException))
				{
					tracer.SendInformationalWatsonReport(ex8, null);
				}
				if ((flags & XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions) == XsoUtil.XsoExceptionHandlingFlags.RethrowUnexpectedExceptions)
				{
					XsoUtil.TraceAndThrowPermanentException(tracer, errorString, ex8);
				}
			}
		}

		private static void TraceAndThrowTransientException(IDiagnosticsSession tracer, LocalizedString errorString, LocalizedException ex)
		{
			tracer.TraceError<LocalizedString, LocalizedException>("Error: {0}, exception: {1}", errorString, ex);
			throw new ComponentFailedTransientException(errorString, ex);
		}

		private static void TraceAndThrowPermanentException(IDiagnosticsSession tracer, LocalizedString errorString, LocalizedException ex)
		{
			tracer.TraceError<LocalizedString, LocalizedException>("Error: {0}, exception: {1}", errorString, ex);
			throw new ComponentFailedPermanentException(errorString, ex);
		}

		internal static IEnumerable<StoreObjectId> GetSubfolders(IDiagnosticsSession tracer, Folder parentFolder, QueryFilter filter)
		{
			XsoUtil.<>c__DisplayClass6 CS$<>8__locals1 = new XsoUtil.<>c__DisplayClass6();
			CS$<>8__locals1.parentFolder = parentFolder;
			CS$<>8__locals1.filter = filter;
			Util.ThrowOnNullArgument(CS$<>8__locals1.parentFolder, "parentFolder");
			Guid mailboxGuid = CS$<>8__locals1.parentFolder.Session.MailboxGuid;
			using (QueryResult queryResult = XsoUtil.TranslateXsoExceptionsWithReturnValue<QueryResult>(tracer, Strings.ConnectionToMailboxFailed(mailboxGuid), () => CS$<>8__locals1.parentFolder.FolderQuery(FolderQueryFlags.DeepTraversal, CS$<>8__locals1.filter, null, new PropertyDefinition[]
			{
				FolderSchema.Id
			})))
			{
				for (;;)
				{
					object[][] folders = XsoUtil.TranslateXsoExceptionsWithReturnValue<object[][]>(tracer, Strings.ConnectionToMailboxFailed(mailboxGuid), () => queryResult.GetRows(10000));
					if (folders == null || folders.Length == 0)
					{
						break;
					}
					foreach (object[] folderProps in folders)
					{
						if (folderProps[0] != null && !PropertyError.IsPropertyError(folderProps[0]))
						{
							yield return StoreId.GetStoreObjectId((StoreId)folderProps[0]);
						}
					}
				}
			}
			yield break;
		}

		[Flags]
		internal enum XsoExceptionHandlingFlags
		{
			None = 0,
			DoNotExpectObjectNotFound = 1,
			DoNotExpectCorruptData = 2,
			RethrowUnexpectedExceptions = 4,
			RethrowAllExceptions = 7
		}
	}
}
