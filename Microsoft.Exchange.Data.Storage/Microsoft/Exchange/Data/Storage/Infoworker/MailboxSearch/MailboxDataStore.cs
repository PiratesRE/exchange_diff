using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDataStore : IDisposeTrackable, IDisposable
	{
		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
			private set
			{
				this.mailboxSession = value;
			}
		}

		internal ADObjectId MailboxOwnerId
		{
			get
			{
				return this.mailboxSession.MailboxOwner.ObjectId;
			}
		}

		private static MailboxSession OpenMailboxSession(ADUser adUser)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(adUser.OrganizationId.ToADSessionSettings(), adUser, RemotingOptions.AllowCrossSite);
			return MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=ELC;Action=Manage-SearchObject;Interactive=false", null, true);
		}

		private string GetObjectMessageClass<T>(Guid guid) where T : IConfigurable, new()
		{
			SearchObjectBase searchObjectBase = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T)) as SearchObjectBase;
			if (searchObjectBase == null)
			{
				throw new ArgumentException(string.Format("Invalid type: {0}, only SearchObject or SearchStatus are supported!", typeof(T).Name));
			}
			SearchObjectId searchObjectId = new SearchObjectId(this.MailboxOwnerId, searchObjectBase.ObjectType, guid);
			return "IPM.Configuration." + searchObjectId.ConfigurationName;
		}

		private string GetObjectMessageClass<T>() where T : IConfigurable, new()
		{
			return this.GetObjectMessageClass<T>(Guid.Empty);
		}

		private UserConfiguration OpenFaiMessage(string configName, bool createIfMissing)
		{
			UserConfiguration userConfiguration = null;
			StoreId defaultFolderId = this.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			Exception ex = null;
			try
			{
				userConfiguration = this.MailboxSession.UserConfigurationManager.GetFolderConfiguration(configName, UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, defaultFolderId);
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (CorruptDataException ex3)
			{
				ex = ex3;
			}
			if (userConfiguration == null)
			{
				MailboxDataStore.Tracer.TraceDebug<string, Exception>(0L, "FAI message '{0}' is missing or corrupt. Exception: {1}", configName, ex);
				if (createIfMissing)
				{
					if (ex is CorruptDataException)
					{
						this.MailboxSession.UserConfigurationManager.DeleteFolderConfigurations(defaultFolderId, new string[]
						{
							configName
						});
					}
					userConfiguration = this.MailboxSession.UserConfigurationManager.CreateFolderConfiguration(configName, UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, defaultFolderId);
				}
			}
			return userConfiguration;
		}

		private void CacheSearchStatus(SearchObject searchObject)
		{
			SearchObjectId identity = new SearchObjectId(searchObject.Id, ObjectType.SearchStatus);
			searchObject.SearchStatus = (SearchStatus)this.Read<SearchStatus>(identity);
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(this.ToString());
			}
		}

		public MailboxDataStore(ADUser adUser)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (adUser == null)
				{
					throw new ArgumentNullException("adUser");
				}
				this.disposeTracker = this.GetDisposeTracker();
				this.mailboxSession = MailboxDataStore.OpenMailboxSession(adUser);
				MailboxDataStore.Tracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "Instantiating search object data store in discovery mailbox {0}.", this.mailboxSession.MailboxOwner);
				disposeGuard.Success();
			}
		}

		public IConfigurable Read<T>(SearchObjectId identity) where T : IConfigurable, new()
		{
			this.CheckDisposed("Read");
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			SearchObjectBase searchObjectBase = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T)) as SearchObjectBase;
			if (searchObjectBase == null || searchObjectBase.ObjectType != identity.ObjectType)
			{
				return null;
			}
			IConfigurable result;
			using (UserConfiguration userConfiguration = this.OpenFaiMessage(identity.ConfigurationName, false))
			{
				if (userConfiguration == null)
				{
					result = null;
				}
				else
				{
					using (Stream stream = userConfiguration.GetStream())
					{
						T t = default(T);
						try
						{
							IFormatter formatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
							t = (T)((object)formatter.Deserialize(stream));
						}
						catch (InvalidOperationException ex)
						{
							MailboxDataStore.Tracer.TraceError<InvalidOperationException, SearchObjectId>((long)this.GetHashCode(), "SerializationException: {0} for search {1}!", ex, identity);
							throw new CorruptDataException(ServerStrings.StoreDataInvalid(identity.ToString()), ex);
						}
						catch (SerializationException ex2)
						{
							MailboxDataStore.Tracer.TraceError<SerializationException, SearchObjectId>((long)this.GetHashCode(), "SerializationException: {0} for search {1}!", ex2, identity);
							throw new CorruptDataException(ServerStrings.StoreDataInvalid(identity.ToString()), ex2);
						}
						if (t == null || t.Identity == null || !t.Identity.Equals(identity))
						{
							MailboxDataStore.Tracer.TraceError<SearchObjectId, IExchangePrincipal>((long)this.GetHashCode(), "Reading {0} from discovery mailbox {1} failed with null object or incorrect identity!", identity, this.MailboxSession.MailboxOwner);
							throw new CorruptDataException(ServerStrings.StoreDataInvalid(identity.ToString()));
						}
						(t as SearchObjectBase).Id.MailboxOwnerId = this.MailboxOwnerId;
						MailboxDataStore.Tracer.TraceDebug<T, IExchangePrincipal>((long)this.GetHashCode(), "Read {0} from discovery mailbox {1}.", t, this.MailboxSession.MailboxOwner);
						ValidationError[] array = (t as ConfigurableObject).ValidateRead();
						if (array.Length > 0)
						{
							throw new DataValidationException(array[0]);
						}
						if (t.GetType() == typeof(SearchObject))
						{
							this.CacheSearchStatus(t as SearchObject);
						}
						result = t;
					}
				}
			}
			return result;
		}

		public IEnumerable<T> FindPaged<T>(TextFilter filter, int pageSize) where T : IConfigurable, new()
		{
			this.CheckDisposed("FindPaged");
			TextFilter textFilter = new TextFilter(StoreObjectSchema.ItemClass, this.GetObjectMessageClass<T>(), MatchOptions.Prefix, MatchFlags.IgnoreCase);
			QueryFilter queryFilter;
			if (filter == null || string.IsNullOrEmpty(filter.Text))
			{
				queryFilter = textFilter;
			}
			else
			{
				if (filter.Property != SearchObjectBaseSchema.Name)
				{
					throw new ArgumentException("Property in the filter must be SearchObjectSchema.Name");
				}
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new TextFilter(ItemSchema.Subject, filter.Text, filter.MatchOptions, filter.MatchFlags),
					textFilter
				});
			}
			List<T> list = new List<T>();
			using (Folder folder = Folder.Bind(this.MailboxSession, DefaultFolderType.Inbox, null))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, queryFilter, new SortBy[]
				{
					new SortBy(StoreObjectSchema.CreationTime, SortOrder.Descending)
				}, new PropertyDefinition[]
				{
					StoreObjectSchema.ItemClass
				}))
				{
					bool flag = false;
					while (!flag)
					{
						object[][] rows = queryResult.GetRows(100);
						if (rows == null || rows.Length <= 0)
						{
							break;
						}
						for (int i = 0; i < rows.Length; i++)
						{
							string text = (string)rows[i][0];
							string configName = text.Substring("IPM.Configuration.".Length);
							using (UserConfiguration userConfiguration = this.OpenFaiMessage(configName, false))
							{
								using (Stream stream = userConfiguration.GetStream())
								{
									T t = default(T);
									try
									{
										IFormatter formatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
										t = (T)((object)formatter.Deserialize(stream));
									}
									catch (InvalidOperationException arg)
									{
										MailboxDataStore.Tracer.TraceError<InvalidOperationException, string>((long)this.GetHashCode(), "InvalidOperationException: {0} when reading configuration item {1}!", arg, userConfiguration.ConfigurationName);
										goto IL_2A7;
									}
									catch (SerializationException arg2)
									{
										MailboxDataStore.Tracer.TraceError<SerializationException, string>((long)this.GetHashCode(), "SerializationException: {0} when reading configuration item {1}!", arg2, userConfiguration.ConfigurationName);
										goto IL_2A7;
									}
									if (t == null || t.Identity == null)
									{
										MailboxDataStore.Tracer.TraceError<string, IExchangePrincipal>((long)this.GetHashCode(), "Reading {0} from discovery mailbox {1} failed!", userConfiguration.ConfigurationName, this.MailboxSession.MailboxOwner);
									}
									else
									{
										(t as SearchObjectBase).Id.MailboxOwnerId = this.MailboxOwnerId;
										MailboxDataStore.Tracer.TraceDebug<T, IExchangePrincipal>((long)this.GetHashCode(), "Read {0} from discovery mailbox {1}.", t, this.MailboxSession.MailboxOwner);
										ValidationError[] array = (t as ConfigurableObject).ValidateRead();
										if (array.Length > 0)
										{
											throw new DataValidationException(array[0]);
										}
										if (t.GetType() == typeof(SearchObject))
										{
											this.CacheSearchStatus(t as SearchObject);
										}
										list.Add(t);
										if (pageSize != 0 && list.Count >= pageSize)
										{
											flag = true;
											break;
										}
									}
								}
							}
							IL_2A7:;
						}
					}
				}
			}
			return list;
		}

		public void Save(SearchObjectBase savedObject)
		{
			this.CheckDisposed("Save");
			if (savedObject == null)
			{
				throw new ArgumentNullException("savedObject");
			}
			using (UserConfiguration userConfiguration = this.OpenFaiMessage(savedObject.Id.ConfigurationName, true))
			{
				bool flag = savedObject.IsChanged(SearchObjectBaseSchema.Name);
				using (Stream stream = userConfiguration.GetStream())
				{
					savedObject.ResetChangeTracking(true);
					IFormatter formatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					formatter.Serialize(stream, savedObject);
					MailboxDataStore.Tracer.TraceDebug<SearchObjectBase, IExchangePrincipal>((long)this.GetHashCode(), "Saved {0} from discovery mailbox {1}.", savedObject, this.MailboxSession.MailboxOwner);
				}
				ConflictResolutionResult conflictResolutionResult = userConfiguration.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new SaveConflictException(ServerStrings.ErrorSavingSearchObject(savedObject.Id.ToString()), conflictResolutionResult);
				}
				if (flag)
				{
					using (ConfigurationItem configurationItem = (ConfigurationItem)Item.Bind(this.MailboxSession, userConfiguration.Id, new PropertyDefinition[]
					{
						ItemSchema.Subject
					}))
					{
						configurationItem.OpenAsReadWrite();
						configurationItem.Subject = savedObject.Name;
						configurationItem.Save(SaveMode.FailOnAnyConflict);
					}
				}
			}
		}

		public void Delete(SearchObjectBase deletedObject)
		{
			this.CheckDisposed("Delete");
			if (deletedObject == null || deletedObject.Id == null)
			{
				throw new ArgumentNullException("deletedObject");
			}
			OperationResult operationResult = this.MailboxSession.UserConfigurationManager.DeleteFolderConfigurations(this.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox), new string[]
			{
				deletedObject.Id.ConfigurationName
			});
			if (operationResult != OperationResult.Succeeded)
			{
				MailboxDataStore.Tracer.TraceError<SearchObjectBase, IExchangePrincipal>((long)this.GetHashCode(), "Deleting object {0} from discovery mailbox {1} failed!", deletedObject, this.MailboxSession.MailboxOwner);
			}
		}

		public bool Exists(SearchObjectId identity)
		{
			this.CheckDisposed("Exists");
			if (identity == null || string.IsNullOrEmpty(identity.ConfigurationName))
			{
				throw new ArgumentNullException("identity");
			}
			using (UserConfiguration userConfiguration = this.OpenFaiMessage(identity.ConfigurationName, false))
			{
				if (userConfiguration != null)
				{
					return true;
				}
			}
			return false;
		}

		public bool Exists<T>(string name) where T : IConfigurable, new()
		{
			this.CheckDisposed("Exists<T>");
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new TextFilter(ItemSchema.Subject, name, MatchOptions.FullString, MatchFlags.IgnoreCase),
				new TextFilter(StoreObjectSchema.ItemClass, this.GetObjectMessageClass<T>(), MatchOptions.Prefix, MatchFlags.IgnoreCase)
			});
			bool result;
			using (Folder folder = Folder.Bind(this.MailboxSession, DefaultFolderType.Inbox))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, queryFilter, null, new PropertyDefinition[]
				{
					StoreObjectSchema.ItemClass
				}))
				{
					result = (queryResult != null && queryResult.EstimatedRowCount > 0);
				}
			}
			return result;
		}

		public virtual void Dispose()
		{
			if (!this.isDisposed)
			{
				if (this.MailboxSession != null)
				{
					this.MailboxSession.Dispose();
					this.MailboxSession = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				this.isDisposed = true;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxDataStore>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override string ToString()
		{
			if (this.isDisposed || this.MailboxSession == null)
			{
				return string.Format("{0}{1}", base.GetType().FullName, "disposed");
			}
			return this.MailboxSession.ServerFullyQualifiedDomainName;
		}

		private const string FaiMessageClassPrefix = "IPM.Configuration.";

		private MailboxSession mailboxSession;

		private bool isDisposed;

		private DisposeTracker disposeTracker;

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;
	}
}
