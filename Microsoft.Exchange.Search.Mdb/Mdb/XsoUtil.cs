using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Mdb
{
	internal static class XsoUtil
	{
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

		internal static ExchangePrincipal GetExchangePrincipal(ISearchServiceConfig config, MdbInfo mdbInfo, Guid mailboxGuid)
		{
			ExchangePrincipal result;
			if (config.ReadFromPassiveEnabled && !mdbInfo.IsLagCopy)
			{
				DatabaseLocationInfo databaseLocationInfo = new DatabaseLocationInfo(LocalServer.GetServer(), false);
				result = ExchangePrincipal.FromMailboxData(mailboxGuid, mdbInfo.Guid, null, Array<CultureInfo>.Empty, RemotingOptions.LocalConnectionsOnly, databaseLocationInfo);
			}
			else
			{
				result = ExchangePrincipal.FromMailboxData(mailboxGuid, mdbInfo.Guid, Array<CultureInfo>.Empty, RemotingOptions.AllowCrossSite);
			}
			return result;
		}

		internal static StoreSession GetStoreSession(ISearchServiceConfig config, ExchangePrincipal principal, bool isPublicFolderMailbox, string clientInfo)
		{
			if (isPublicFolderMailbox)
			{
				return PublicFolderSession.OpenAsSearch(principal, clientInfo, config.ReadFromPassiveEnabled);
			}
			return MailboxSession.OpenAsSystemService(principal, CultureInfo.InvariantCulture, clientInfo, config.ReadFromPassiveEnabled);
		}

		internal static Folder GetRootFolder(StoreSession storeSession)
		{
			if (storeSession.IsPublicFolderSession)
			{
				PublicFolderSession publicFolderSession = (PublicFolderSession)storeSession;
				return Folder.Bind(publicFolderSession, publicFolderSession.GetPublicFolderRootId(), new PropertyDefinition[]
				{
					FolderSchema.Id
				});
			}
			return Folder.Bind(storeSession, DefaultFolderType.Configuration, new PropertyDefinition[]
			{
				FolderSchema.Id
			});
		}

		internal static bool ShouldSkipMessageClass(string messageClass)
		{
			return XsoUtil.MessageClassesToSkip.Contains(messageClass);
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

		private static readonly HashSet<string> MessageClassesToSkip = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Exchange.ContentsSyncData",
			"IPM.Microsoft.WunderBar.Link",
			"IPM.Configuration.OWA.AutocompleteCache",
			"IPC.Microsoft Exchange 4.0.Deferred Action",
			"IPM.MS-Exchange.MailboxMoveHistory"
		};

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
