using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MapiSession : IConfigDataProvider, IDisposeTrackable, IDisposable
	{
		public void Delete(IConfigurable instance)
		{
			MapiObject mapiObject = instance as MapiObject;
			if (mapiObject == null)
			{
				throw new ArgumentException("instance");
			}
			if (mapiObject.Identity == null)
			{
				throw new ArgumentException(Strings.ExceptionIdentityInvalid);
			}
			if (mapiObject.MapiSession == null)
			{
				mapiObject.MapiSession = this;
			}
			this.InvokeWithWrappedException(delegate()
			{
				mapiObject.Delete();
			}, Strings.ExceptionDeleteObject(instance.Identity.ToString()), null);
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return (IConfigurable[])this.Find<T>(filter, (MapiObjectId)rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, sortBy, 0);
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			return this.Read<T>(identity, false);
		}

		public void Save(IConfigurable instance)
		{
			this.Save(instance, false);
		}

		public string Source
		{
			get
			{
				return this.ServerName;
			}
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			return this.FindPaged<T>(filter, (MapiObjectId)rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, sortBy, pageSize, 0);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MapiSession>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal static void ThrowWrappedException(LocalizedException exception, LocalizedString message, MapiObjectId source, MapiSession mapiSession)
		{
			if (exception is MapiExceptionNotFound)
			{
				if (source is PublicFolderId)
				{
					throw new PublicFolderNotFoundException(source.ToString(), exception);
				}
				if (source is MailboxId)
				{
					mapiSession.AnalyzeMailboxNotFoundAndThrow((MailboxId)source, exception);
				}
				throw new MapiObjectNotFoundException(message, exception);
			}
			else if (exception is MapiExceptionLogonFailed)
			{
				if (source is PublicFolderId)
				{
					throw new PublicStoreLogonFailedException(mapiSession.ServerName, exception);
				}
				if (source is MailboxId)
				{
					mapiSession.AnalyzeMailboxLogonFailedAndThrow((MailboxId)source, exception);
				}
				throw new MapiLogonFailedException(Strings.LogonFailedExceptionError(message, mapiSession.ServerName), exception);
			}
			else
			{
				if (exception is MapiExceptionSessionLimit)
				{
					throw new MapiLogonFailedException(Strings.SessionLimitExceptionError, exception);
				}
				if (exception is MapiExceptionUnknownMailbox)
				{
					if (source is MailboxId)
					{
						mapiSession.AnalyzeMailboxNotFoundAndThrow((MailboxId)source, exception);
					}
					throw new MapiObjectNotFoundException(message, exception);
				}
				if (exception is MapiExceptionMdbOffline)
				{
					if (mapiSession == null)
					{
						throw new DatabaseUnavailableException(Strings.DatabaseUnavailableExceptionErrorSimple, exception);
					}
					DatabaseId databaseId = null;
					if (source is MailboxId && null != ((MailboxId)source).MailboxDatabaseId)
					{
						databaseId = ((MailboxId)source).MailboxDatabaseId;
					}
					else if (source is DatabaseId)
					{
						databaseId = (DatabaseId)source;
					}
					if (null == databaseId)
					{
						throw new DatabaseUnavailableException(Strings.DatabaseUnavailableExceptionError(mapiSession.ServerName), exception);
					}
					throw new DatabaseUnavailableException(Strings.DatabaseUnavailableByIdentityExceptionError(mapiSession.GetDatabaseIdentityString(databaseId), mapiSession.ServerName), exception);
				}
				else
				{
					if (exception is MapiExceptionPartialCompletion)
					{
						throw new MapiPartialCompletionException(message, exception);
					}
					if (exception is MapiExceptionNetworkError)
					{
						if (mapiSession != null)
						{
							throw new MapiNetworkErrorException(Strings.MapiNetworkErrorExceptionError(mapiSession.ServerName), exception);
						}
						throw new MapiNetworkErrorException(Strings.MapiNetworkErrorExceptionErrorSimple, exception);
					}
					else if (exception is MapiExceptionNoAccess)
					{
						if (null != source)
						{
							throw new MapiAccessDeniedException(Strings.MapiAccessDeniedExceptionError(source.ToString()), exception);
						}
						throw new MapiAccessDeniedException(Strings.MapiAccessDeniedExceptionErrorSimple, exception);
					}
					else
					{
						if (exception is MapiRetryableException)
						{
							throw new MapiTransientException(message, exception);
						}
						if (exception is MapiPermanentException)
						{
							throw new MapiOperationException(message, exception);
						}
						if (exception is MapiInvalidOperationException)
						{
							throw new MapiOperationException(message, exception);
						}
						return;
					}
				}
			}
		}

		internal void DisposeCheck()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void ReturnBackConnections()
		{
			if (this.exRpcAdmin != null)
			{
				this.connectionPool.ReturnBack(this.exRpcAdmin);
				this.exRpcAdmin = null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.ReturnBackConnections();
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
				this.disposed = true;
			}
		}

		public T Read<T>(ObjectId identity, bool keepUnmanagedResources) where T : IConfigurable, new()
		{
			this.DisposeCheck();
			if (!typeof(MapiObject).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException("T");
			}
			if (!(identity is MapiObjectId))
			{
				throw new ArgumentException("identity");
			}
			MapiObject mapiObject = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T)) as MapiObject;
			mapiObject.MapiIdentity = (MapiObjectId)identity;
			mapiObject.MapiSession = this;
			this.InvokeWithWrappedException(delegate()
			{
				mapiObject.Read(keepUnmanagedResources);
			}, Strings.ExceptionReadObject(typeof(T).Name, identity.ToString()), null);
			return (T)((object)mapiObject);
		}

		public void Save(IConfigurable instance, bool keepUnmanagedResources)
		{
			this.DisposeCheck();
			MapiObject mapiObject = instance as MapiObject;
			if (mapiObject == null)
			{
				throw new ArgumentException("instance");
			}
			if (mapiObject.MapiSession == null)
			{
				mapiObject.MapiSession = this;
			}
			ValidationError[] array = mapiObject.Validate();
			if (array != null && 0 < array.Length)
			{
				throw new DataValidationException(array[0]);
			}
			this.InvokeWithWrappedException(delegate()
			{
				mapiObject.Save(keepUnmanagedResources);
			}, (instance.ObjectState == ObjectState.New) ? Strings.ExceptionNewObject((instance.Identity == null) ? Strings.ConstantNull : instance.Identity.ToString()) : Strings.ExceptionSaveObject((instance.Identity == null) ? Strings.ConstantNull : instance.Identity.ToString()), null);
		}

		public T[] Find<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int maximumResultsSize) where T : IConfigurable, new()
		{
			return this.Find<T, T>(filter, root, scope, sort, maximumResultsSize);
		}

		private TTarget[] Find<TSource, TTarget>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int maximumResultsSize) where TSource : IConfigurable, new() where TTarget : IConfigurable, new()
		{
			MapiSession.<>c__DisplayClassa<TSource, TTarget> CS$<>8__locals1 = new MapiSession.<>c__DisplayClassa<TSource, TTarget>();
			CS$<>8__locals1.filter = filter;
			CS$<>8__locals1.root = root;
			CS$<>8__locals1.scope = scope;
			CS$<>8__locals1.sort = sort;
			CS$<>8__locals1.maximumResultsSize = maximumResultsSize;
			this.DisposeCheck();
			if (!typeof(MapiObject).IsAssignableFrom(typeof(TSource)))
			{
				throw new ArgumentException("TSource");
			}
			if (!typeof(MapiObject).IsAssignableFrom(typeof(TTarget)))
			{
				throw new ArgumentException("TTarget");
			}
			CS$<>8__locals1.objects = null;
			using (MapiObject mapiObject = ((default(TSource) == null) ? Activator.CreateInstance<TSource>() : default(TSource)) as MapiObject)
			{
				mapiObject.MapiSession = this;
				this.InvokeWithWrappedException(delegate()
				{
					CS$<>8__locals1.objects = mapiObject.Find<TTarget>(CS$<>8__locals1.filter, CS$<>8__locals1.root, CS$<>8__locals1.scope, CS$<>8__locals1.sort, CS$<>8__locals1.maximumResultsSize);
				}, Strings.ExceptionFindObject(typeof(TTarget).Name, (null == CS$<>8__locals1.root) ? Strings.ConstantNull : CS$<>8__locals1.root.ToString()), null);
			}
			if (CS$<>8__locals1.objects != null)
			{
				return CS$<>8__locals1.objects;
			}
			return new TTarget[0];
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int pageSize, int maximumResultsSize) where T : IConfigurable, new()
		{
			return this.FindPaged<T, T>(filter, root, scope, sort, pageSize, maximumResultsSize);
		}

		public IEnumerable<TTarget> FindPaged<TSource, TTarget>(QueryFilter filter, MapiObjectId root, QueryScope scope, SortBy sort, int pageSize, int maximumResultsSize) where TSource : IConfigurable, new() where TTarget : IConfigurable, new()
		{
			MapiSession.<>c__DisplayClass10<TSource, TTarget> CS$<>8__locals1 = new MapiSession.<>c__DisplayClass10<TSource, TTarget>();
			CS$<>8__locals1.filter = filter;
			CS$<>8__locals1.root = root;
			CS$<>8__locals1.scope = scope;
			CS$<>8__locals1.sort = sort;
			CS$<>8__locals1.pageSize = pageSize;
			CS$<>8__locals1.maximumResultsSize = maximumResultsSize;
			this.DisposeCheck();
			if (!typeof(MapiObject).IsAssignableFrom(typeof(TSource)))
			{
				throw new ArgumentException("TSource");
			}
			if (!typeof(MapiObject).IsAssignableFrom(typeof(TTarget)))
			{
				throw new ArgumentException("TTarget");
			}
			CS$<>8__locals1.objects = null;
			using (MapiObject mapiObject = ((default(TSource) == null) ? Activator.CreateInstance<TSource>() : default(TSource)) as MapiObject)
			{
				mapiObject.MapiSession = this;
				this.InvokeWithWrappedException(delegate()
				{
					CS$<>8__locals1.objects = mapiObject.FindPaged<TTarget>(CS$<>8__locals1.filter, CS$<>8__locals1.root, CS$<>8__locals1.scope, CS$<>8__locals1.sort, CS$<>8__locals1.pageSize, CS$<>8__locals1.maximumResultsSize);
				}, Strings.ExceptionFindObject(typeof(TTarget).Name, (null == CS$<>8__locals1.root) ? Strings.ConstantNull : CS$<>8__locals1.root.ToString()), null);
			}
			if (CS$<>8__locals1.objects != null)
			{
				return CS$<>8__locals1.objects;
			}
			return (IEnumerable<TTarget>)new TTarget[0];
		}

		internal ExRpcAdmin Administration
		{
			get
			{
				ExRpcAdmin result;
				if ((result = this.exRpcAdmin) == null)
				{
					result = (this.exRpcAdmin = this.connectionPool.GetAdministration(this.serverName));
				}
				return result;
			}
		}

		public bool IsConnectionConfigurated
		{
			get
			{
				return !string.IsNullOrEmpty(this.serverExchangeLegacyDn);
			}
		}

		public void RedirectServer(string serverExchangeLegacyDn, Fqdn serverFqdn)
		{
			if (string.Equals(this.serverExchangeLegacyDn, serverExchangeLegacyDn, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			this.ReturnBackConnections();
			this.serverName = serverFqdn.ToString().ToLowerInvariant();
			this.serverExchangeLegacyDn = serverExchangeLegacyDn.ToLowerInvariant();
		}

		public MapiSession(string serverExchangeLegacyDn, Fqdn serverFqdn) : this(serverExchangeLegacyDn, serverFqdn, ConsistencyMode.PartiallyConsistent)
		{
		}

		public MapiSession(string serverExchangeLegacyDn, Fqdn serverFqdn, ConsistencyMode consistencyMode)
		{
			this.serverName = serverFqdn.ToString().ToLowerInvariant();
			this.serverExchangeLegacyDn = serverExchangeLegacyDn.ToLowerInvariant();
			this.consistencyMode = consistencyMode;
			this.disposeTracker = this.GetDisposeTracker();
			this.disposed = false;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string ServerExchangeLegacyDn
		{
			get
			{
				return this.serverExchangeLegacyDn;
			}
		}

		public ConsistencyMode ConsistencyMode
		{
			get
			{
				return this.consistencyMode;
			}
			set
			{
				this.consistencyMode = value;
			}
		}

		protected static string GetCommonNameFromLegacyDistinguishName(string legacyDn)
		{
			if (string.IsNullOrEmpty(legacyDn))
			{
				return null;
			}
			int num = legacyDn.LastIndexOf("/cn=", StringComparison.OrdinalIgnoreCase);
			if (-1 != num)
			{
				return legacyDn.Substring("/cn=".Length + num);
			}
			return null;
		}

		internal bool CheckIfInconsistentObjectDisallowed(LocalizedString message)
		{
			if (ConsistencyMode.IgnoreInvalid == this.ConsistencyMode)
			{
				return true;
			}
			if (this.ConsistencyMode == ConsistencyMode.FullyConsistent)
			{
				throw new MapiInconsistentObjectException(message);
			}
			return false;
		}

		private string GetDatabaseIdentityString(DatabaseId databaseId)
		{
			this.DisposeCheck();
			if (null == databaseId)
			{
				throw new ArgumentNullException("databaseId");
			}
			if (string.IsNullOrEmpty(databaseId.DatabaseName))
			{
				try
				{
					MdbStatus databaseStatus = this.GetDatabaseStatus(databaseId, false);
					if (databaseStatus == null)
					{
						ExTraceGlobals.MapiSessionTracer.TraceError<DatabaseId>((long)this.GetHashCode(), "Cannot get status for database '{0}'", databaseId);
					}
					else
					{
						databaseId = new DatabaseId(databaseId.MapiEntryId, databaseStatus.VServerName, databaseStatus.MdbName, databaseId.Guid);
					}
				}
				catch (MapiTransientException ex)
				{
					ExTraceGlobals.MapiSessionTracer.TraceError<DatabaseId, string>((long)this.GetHashCode(), "Getting status for database '{0}' caught an exception: '{1}'", databaseId, ex.Message);
				}
				catch (MapiOperationException ex2)
				{
					ExTraceGlobals.MapiSessionTracer.TraceError<DatabaseId, string>((long)this.GetHashCode(), "Getting status for database '{0}' caught an exception: '{1}'", databaseId, ex2.Message);
				}
			}
			return databaseId.ToString();
		}

		private void AnalyzeMailboxNotFoundAndThrow(MailboxId mailboxId, Exception innerException)
		{
			if (null == mailboxId)
			{
				throw new ArgumentNullException("mailboxId");
			}
			if (null != mailboxId.MailboxDatabaseId)
			{
				throw new MailboxNotFoundException(Strings.MailboxNotFoundInDatabaseExceptionError(mailboxId.MailboxGuid.ToString(), this.GetDatabaseIdentityString(mailboxId.MailboxDatabaseId)), innerException);
			}
			if (string.IsNullOrEmpty(mailboxId.MailboxExchangeLegacyDn))
			{
				throw new MailboxNotFoundException(Strings.MailboxNotFoundExceptionError(mailboxId.ToString()), innerException);
			}
			throw new MailboxNotFoundException(Strings.MailboxNotFoundExceptionError(mailboxId.MailboxExchangeLegacyDn), innerException);
		}

		private void AnalyzeMailboxLogonFailedAndThrow(MailboxId mailboxId, Exception innerException)
		{
			if (null == mailboxId)
			{
				throw new ArgumentNullException("mailboxId");
			}
			if (null != mailboxId.MailboxDatabaseId)
			{
				throw new MailboxLogonFailedException(Strings.MailboxLogonFailedInDatabaseExceptionError(mailboxId.MailboxGuid.ToString(), this.GetDatabaseIdentityString(mailboxId.MailboxDatabaseId), this.ServerName), innerException);
			}
			if (string.IsNullOrEmpty(mailboxId.MailboxExchangeLegacyDn))
			{
				throw new MailboxLogonFailedException(Strings.MailboxLogonFailedExceptionError(mailboxId.ToString(), this.ServerName), innerException);
			}
			throw new MailboxLogonFailedException(Strings.MailboxLogonFailedExceptionError(mailboxId.MailboxExchangeLegacyDn, this.ServerName), innerException);
		}

		internal void InvokeWithWrappedException(ParameterlessReturnlessDelegate invoke, LocalizedString message, MapiObjectId source)
		{
			this.InvokeWithWrappedException(invoke, message, source, null);
		}

		internal void InvokeWithWrappedException(ParameterlessReturnlessDelegate invoke, LocalizedString message, MapiObjectId source, MapiSession.ErrorTranslatorDelegate translateError)
		{
			try
			{
				invoke();
			}
			catch (MapiRetryableException ex)
			{
				LocalizedException ex2;
				if (translateError != null && (ex2 = translateError(ref message, ex)) != null)
				{
					throw ex2;
				}
				MapiSession.ThrowWrappedException(ex, message, source, this);
			}
			catch (MapiPermanentException ex3)
			{
				LocalizedException ex2;
				if (translateError != null && (ex2 = translateError(ref message, ex3)) != null)
				{
					throw ex2;
				}
				MapiSession.ThrowWrappedException(ex3, message, source, this);
			}
			catch (MapiInvalidOperationException ex4)
			{
				LocalizedException ex2;
				if (translateError != null && (ex2 = translateError(ref message, ex4)) != null)
				{
					throw ex2;
				}
				MapiSession.ThrowWrappedException(ex4, message, source, this);
			}
		}

		public MdbStatus GetDatabaseStatus(DatabaseId databaseId)
		{
			return this.GetDatabaseStatus(databaseId, true);
		}

		public MdbStatus GetDatabaseStatus(DatabaseId databaseId, bool basicInformation)
		{
			MdbStatus[] databaseStatus = this.GetDatabaseStatus(new DatabaseId[]
			{
				databaseId
			}, basicInformation);
			if (databaseStatus.Length != 0)
			{
				return databaseStatus[0];
			}
			return null;
		}

		public MdbStatus[] GetDatabaseStatus(DatabaseId[] databaseIds, bool basicInformation)
		{
			this.DisposeCheck();
			List<Guid> list = new List<Guid>();
			if (databaseIds != null && 0 < databaseIds.Length)
			{
				list.Capacity = databaseIds.Length;
				foreach (DatabaseId databaseId in databaseIds)
				{
					if (null == databaseId)
					{
						throw new ArgumentException("databaseIds");
					}
					list.Add(databaseId.Guid);
				}
			}
			MdbStatus[] result;
			try
			{
				MdbStatus[] array;
				if (list.Count == 0)
				{
					array = this.Administration.ListMdbStatus(basicInformation);
				}
				else
				{
					array = this.Administration.ListMdbStatus(list.ToArray());
					if (!basicInformation && array != null && 0 < array.Length)
					{
						Dictionary<Guid, MdbStatus> dictionary = new Dictionary<Guid, MdbStatus>(array.Length);
						foreach (MdbStatus mdbStatus in array)
						{
							dictionary[mdbStatus.MdbGuid] = mdbStatus;
						}
						MdbStatus[] array3 = this.Administration.ListMdbStatus(basicInformation);
						if (array3 != null && 0 < array3.Length)
						{
							foreach (MdbStatus mdbStatus2 in array3)
							{
								dictionary[mdbStatus2.MdbGuid] = mdbStatus2;
							}
						}
						List<MdbStatus> list2 = new List<MdbStatus>(list.Count);
						foreach (Guid key in list)
						{
							list2.Add(dictionary[key]);
						}
						array = list2.ToArray();
					}
				}
				result = array;
			}
			catch (MapiExceptionNetworkError innerException)
			{
				throw new MapiNetworkErrorException(Strings.MapiNetworkErrorExceptionError(this.ServerName), innerException);
			}
			catch (MapiRetryableException ex)
			{
				throw new MapiTransientException(Strings.ExceptionGetDatabaseStatus(ex.Message), ex);
			}
			catch (MapiPermanentException ex2)
			{
				throw new MapiOperationException(Strings.ExceptionGetDatabaseStatus(ex2.Message), ex2);
			}
			return result;
		}

		internal const string LiteralApplicationIdentity = "Client=MSExchangeRPC;Action=MapiDriver";

		private bool disposed;

		private string serverName;

		private string serverExchangeLegacyDn;

		private ConsistencyMode consistencyMode;

		private ExRpcAdmin exRpcAdmin;

		private DisposeTracker disposeTracker;

		private ConnectionPool connectionPool = ConnectionPool.Instance;

		internal delegate LocalizedException ErrorTranslatorDelegate(ref LocalizedString localizedString, Exception innerException);
	}
}
