using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CacheExceptionUtilities
	{
		protected CacheExceptionUtilities(GlobalSyncLogSession globalSyncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("globalSyncLogSession", globalSyncLogSession);
			this.globalSyncLogSession = globalSyncLogSession;
		}

		public static CacheExceptionUtilities Instance
		{
			get
			{
				if (CacheExceptionUtilities.instance == null)
				{
					lock (CacheExceptionUtilities.syncLock)
					{
						if (CacheExceptionUtilities.instance == null)
						{
							CacheExceptionUtilities.instance = new CacheExceptionUtilities(ContentAggregationConfig.SyncLogSession);
						}
					}
				}
				return CacheExceptionUtilities.instance;
			}
		}

		internal bool IsSessionReusableAfterException(Exception e)
		{
			return !(e is LocalizedException) || (e is StorageTransientException && !(e is ConnectionFailedTransientException) && (e.InnerException == null || !(e.InnerException is MapiExceptionRpcServerTooBusy)));
		}

		internal CacheTransientException CreateCacheTransientException(Trace tracer, int objectHash, Guid databaseGuid, Guid mailboxGuid, LocalizedString exceptionInfo)
		{
			this.globalSyncLogSession.LogError((TSLID)276UL, tracer, (long)objectHash, Guid.Empty, mailboxGuid, "In database {0}, encountered:{1}.", new object[]
			{
				databaseGuid,
				exceptionInfo
			});
			return new CacheTransientException(databaseGuid, mailboxGuid, exceptionInfo);
		}

		internal Exception ConvertToCacheException(Trace tracer, int objectHash, Guid databaseGuid, Guid mailboxGuid, Exception exception, out bool reuseSession)
		{
			reuseSession = this.IsSessionReusableAfterException(exception);
			bool flag = false;
			Exception ex;
			if (exception is TransientException)
			{
				ex = new CacheTransientException(databaseGuid, mailboxGuid, (TransientException)exception);
			}
			else
			{
				bool flag2 = ExceptionUtilities.IndicatesUserNotFound(exception);
				if (flag2 || exception is SerializationException || exception is CorruptDataException)
				{
					if (!mailboxGuid.Equals(Guid.Empty))
					{
						DataAccessLayer.ScheduleMailboxCrawl(databaseGuid, mailboxGuid);
					}
					if (flag2)
					{
						ex = new MailboxNotFoundException(databaseGuid, mailboxGuid, exception);
					}
					else
					{
						ex = new CacheCorruptException(databaseGuid, mailboxGuid, exception);
						flag = (exception is SerializationException && !(exception is VersionMismatchException));
					}
				}
				else if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(this.globalSyncLogSession, exception))
				{
					ex = new CacheTransientException(databaseGuid, mailboxGuid, exception);
				}
				else
				{
					ex = new CachePermanentException(databaseGuid, mailboxGuid, exception);
					flag = true;
				}
			}
			if (flag)
			{
				this.ReportWatson(null, ex);
			}
			else
			{
				bool flag3 = ex is CacheTransientException && ((CacheTransientException)ex).StoreRequestedBackOff;
				if (flag3)
				{
					this.globalSyncLogSession.LogError((TSLID)329UL, tracer, (long)objectHash, Guid.Empty, mailboxGuid, "In database {0}, encountered exception {1}, mailboxSession usable: {2}.", new object[]
					{
						databaseGuid,
						exception,
						reuseSession
					});
				}
				else
				{
					this.globalSyncLogSession.LogError((TSLID)277UL, tracer, (long)objectHash, Guid.Empty, mailboxGuid, "In database {0}, encountered: {1}:{2}:{3}, mailboxSession usable:{4}.", new object[]
					{
						databaseGuid,
						exception.GetType().FullName,
						exception.Message,
						(exception.InnerException != null) ? exception.InnerException.GetType().FullName : null,
						reuseSession
					});
				}
			}
			return ex;
		}

		protected virtual void ReportWatson(string message, Exception exception)
		{
			DataAccessLayer.ReportWatson(message, exception);
		}

		private static readonly object syncLock = new object();

		private static CacheExceptionUtilities instance;

		private readonly GlobalSyncLogSession globalSyncLogSession;
	}
}
