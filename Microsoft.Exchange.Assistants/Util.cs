using System;
using System.Configuration;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Assistants
{
	public static class Util
	{
		public static void CatchMeIfYouCan(CatchMe function, string nonLocalizedAssistantName)
		{
			try
			{
				Util.CoreCatchMeIfYouCan(function, nonLocalizedAssistantName);
			}
			catch (GrayException innerException)
			{
				AIException aiException = new AIGrayException(innerException);
				Util.TraceAndThrow(function, aiException, nonLocalizedAssistantName);
			}
		}

		public static void CoreCatchMeIfYouCan(CatchMe function, string nonLocalizedAssistantName)
		{
			AIException aiException = null;
			GrayException.MapAndReportGrayExceptions(delegate()
			{
				try
				{
					Util.Tracer.TraceDebug<MethodInfo, object>((long)function.GetHashCode(), "CatchMeIfYouCan: calling {0} on target {1}...", function.Method, function.Target);
					function();
					Util.Tracer.TraceDebug<MethodInfo, object>((long)function.GetHashCode(), "CatchMeIfYouCan: {0} on target {1} without exception", function.Method, function.Target);
				}
				catch (AIException aiException)
				{
					AIException aiException = aiException;
				}
				catch (AccessDeniedException innerException)
				{
					AIException aiException = new SkipException(innerException);
				}
				catch (TenantAccessBlockedException innerException2)
				{
					AIException aiException = new SkipException(innerException2);
				}
				catch (AccountDisabledException innerException3)
				{
					AIException aiException = new DisconnectedMailboxException(innerException3);
				}
				catch (MailboxInTransitException innerException4)
				{
					AIException aiException = new AIMailboxInTransitException(innerException4);
				}
				catch (WrongServerException innerException5)
				{
					AIException aiException = new DeadMailboxException(innerException5);
				}
				catch (ConnectionFailedPermanentException innerException6)
				{
					AIException aiException = new TransientMailboxException(innerException6);
				}
				catch (ConnectionFailedTransientException innerException7)
				{
					AIException aiException = new DatabaseIneptException(innerException7);
				}
				catch (DataSourceTransientException innerException8)
				{
					AIException aiException = new ServerIneptException(innerException8);
				}
				catch (MailboxUnavailableException innerException9)
				{
					AIException aiException = new AIMailboxUnavailableException(innerException9);
				}
				catch (MapiExceptionAmbiguousAlias innerException10)
				{
					AIException aiException = new AmbiguousAliasMailboxException(innerException10);
				}
				catch (MapiExceptionJetErrorReadVerifyFailure innerException11)
				{
					AIException aiException = new DatabaseIneptException(innerException11);
				}
				catch (MapiExceptionMdbOffline innerException12)
				{
					AIException aiException = new DatabaseIneptException(innerException12);
				}
				catch (MapiExceptionDatabaseError innerException13)
				{
					AIException aiException = new DatabaseIneptException(innerException13);
				}
				catch (MapiExceptionNotEnoughMemory innerException14)
				{
					AIException aiException = new ServerIneptException(innerException14);
				}
				catch (MapiExceptionJetErrorLogDiskFull innerException15)
				{
					AIException aiException = new DatabaseIneptException(innerException15);
				}
				catch (MapiExceptionJetErrorInstanceUnavailableDueToFatalLogDiskFull innerException16)
				{
					AIException aiException = new DatabaseIneptException(innerException16);
				}
				catch (QuotaExceededException innerException17)
				{
					AIException aiException = new MailboxIneptException(innerException17);
				}
				catch (ResourcesException innerException18)
				{
					AIException aiException = new ServerIneptException(innerException18);
				}
				catch (ServerPausedException innerException19)
				{
					AIException aiException = new TransientMailboxException(innerException19);
				}
				catch (VirusScanInProgressException innerException20)
				{
					AIException aiException = new MailboxIneptException(innerException20);
				}
				catch (MapiExceptionJetErrorIndexNotFound innerException21)
				{
					AIException aiException = new DatabaseIneptException(innerException21);
				}
				catch (MapiExceptionJetErrorFileNotFound innerException22)
				{
					AIException aiException = new MapiTransientException(innerException22);
				}
				catch (MapiExceptionRpcServerTooBusy innerException23)
				{
					AIException aiException = new DatabaseIneptException(innerException23);
				}
				catch (TransientException innerException24)
				{
					AIException aiException = new TransientMailboxException(innerException24);
				}
				catch (VirusDetectedException innerException25)
				{
					AIException aiException = new SkipException(innerException25);
				}
				catch (CannotResolveExternalDirectoryOrganizationIdException innerException26)
				{
					AIException aiException = new SkipException(innerException26);
				}
				catch (DataSourceOperationException innerException27)
				{
					AIException aiException = new ServerIneptException(innerException27);
				}
				catch (MapiExceptionNotFound innerException28)
				{
					AIException aiException = new DatabaseIneptException(innerException28);
				}
				catch (ConfigurationErrorsException innerException29)
				{
					AIException aiException = new AppConfigurationErrorsException(innerException29);
				}
				catch (StoragePermanentException ex)
				{
					if (ex.InnerException is MapiExceptionJetErrorFileNotFound || ex.InnerException is MapiExceptionJetErrorInvalidSesid || ex.InnerException is MapiExceptionJetErrorReadVerifyFailure || ex.InnerException is MapiExceptionJetErrorKeyDuplicate || ex.InnerException is MapiExceptionCallFailed || ex.InnerException is MapiExceptionJetErrorLogDiskFull || ex.InnerException is MapiExceptionDuplicateObject || ex.InnerException is MapiExceptionMailboxSoftDeleted || ex.InnerException is MapiExceptionUnknownMailbox || ex.InnerException is MapiExceptionInvalidParameter)
					{
						AIException aiException = new MapiTransientException(ex);
					}
					else if (ex.InnerException is DataSourceTransientException || ex.InnerException is DataSourceOperationException || ex.InnerException is DataValidationException)
					{
						AIException aiException = new ServerIneptException(ex);
					}
					else if (ex.InnerException is MapiExceptionAmbiguousAlias)
					{
						AIException aiException = new AmbiguousAliasMailboxException(ex);
					}
					else if (ex is ObjectNotFoundException && ex.InnerException is MapiExceptionNotFound)
					{
						AIException aiException = new TransientMailboxException(ex);
					}
					else if (ex.InnerException is MapiExceptionDatabaseError)
					{
						AIException aiException = new DatabaseIneptException(ex);
					}
					else
					{
						if (!(ex is FolderSaveException) || !(ex.InnerException is MailboxUnavailableException))
						{
							throw;
						}
						AIException aiException = new AIMailboxUnavailableException(ex);
					}
				}
			});
			Util.TraceAndThrow(function, aiException, nonLocalizedAssistantName);
		}

		private static void TraceAndThrow(CatchMe function, AIException aiException, string nonLocalizedAssistantName)
		{
			if (aiException != null)
			{
				Util.Tracer.TraceError((long)function.GetHashCode(), "CatchMeIfYouCan: Assistant:{0}, {1} on target {2} with exception: {3}", new object[]
				{
					nonLocalizedAssistantName,
					function.Method,
					function.Target,
					aiException
				});
				aiException.Data["AssistantName"] = nonLocalizedAssistantName;
				throw aiException;
			}
		}

		internal static long ReadRegistryLong(RegistryKey key, string valueName)
		{
			byte[] array = key.GetValue(valueName) as byte[];
			if (array == null)
			{
				throw new FormatException("Bad value: " + valueName);
			}
			if (array.Length != 8)
			{
				throw new FormatException("Bogus length: " + valueName);
			}
			return BitConverter.ToInt64(array, 0);
		}

		internal static void WriteRegistryLong(RegistryKey key, string valueName, long value)
		{
			key.SetValue(valueName, BitConverter.GetBytes(value));
		}

		private const string AssistantNameKey = "AssistantName";

		private static readonly Trace Tracer = ExTraceGlobals.ErrorHandlerTracer;
	}
}
