using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class EwsStoreDataProvider : IConfigDataProvider
	{
		protected EwsStoreDataProvider(LazilyInitialized<IExchangePrincipal> mailbox)
		{
			this.mailbox = mailbox;
			this.CanRetry = true;
		}

		protected EwsStoreDataProvider(LazilyInitialized<IExchangePrincipal> mailbox, SpecialLogonType logonType, OpenAsAdminOrSystemServiceBudgetTypeType budgetType) : this(mailbox)
		{
			this.logonType = new SpecialLogonType?(logonType);
			this.budgetType = budgetType;
		}

		protected EwsStoreDataProvider(LazilyInitialized<IExchangePrincipal> mailbox, SecurityAccessToken securityAccessToken) : this(mailbox)
		{
			this.securityAccessToken = securityAccessToken;
		}

		protected Item BindItem(ItemId itemId, PropertySet propertySet)
		{
			return this.InvokeServiceCall<Item>(() => Item.Bind(this.Service, itemId, propertySet ?? PropertySet.FirstClassProperties));
		}

		protected virtual void ExpireCache()
		{
			SecurityIdentifier userSid = this.Mailbox.Sid;
			ADSessionSettings adSetting = this.Mailbox.MailboxInfo.OrganizationId.ToADSessionSettings();
			this.mailbox = new LazilyInitialized<IExchangePrincipal>(delegate()
			{
				IExchangePrincipal result;
				try
				{
					result = ExchangePrincipal.FromUserSid(adSetting, userSid);
				}
				catch (ObjectNotFoundException ex)
				{
					throw new DataSourceOperationException(ex.LocalizedString, ex);
				}
				return result;
			});
			this.service = null;
			this.cache = null;
			this.defaultFolder = null;
			this.requestedServerVersion = null;
			this.mailboxVersion = null;
			EwsStoreDataProvider.caches.Remove(this.CacheKey);
		}

		protected virtual EwsStoreObject FilterObject(EwsStoreObject ewsStoreObject)
		{
			return ewsStoreObject;
		}

		protected Folder GetOrCreateFolder(string folderName)
		{
			return this.GetOrCreateFolder(folderName, new FolderId(10));
		}

		protected Folder GetOrCreateFolder(string folderName, FolderId parentFolder)
		{
			return this.GetOrCreateFolderCore(folderName, parentFolder, () => new Folder(this.Service));
		}

		private Folder GetOrCreateFolderCore(string folderName, FolderId parentFolder, Func<Folder> creator)
		{
			SearchFilter filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);
			FolderView view = new FolderView(1);
			FindFoldersResults findFoldersResults = this.InvokeServiceCall<FindFoldersResults>(() => this.Service.FindFolders(parentFolder, filter, view));
			if (findFoldersResults.Folders.Count == 0)
			{
				Folder folder = creator();
				folder.DisplayName = folderName;
				try
				{
					this.InvokeServiceCall(delegate()
					{
						folder.Save(parentFolder);
					});
				}
				catch (DataSourceOperationException ex)
				{
					ServiceResponseException ex2 = ex.InnerException as ServiceResponseException;
					if (ex2 == null || ex2.ErrorCode != 107)
					{
						throw;
					}
				}
				findFoldersResults = this.InvokeServiceCall<FindFoldersResults>(() => this.Service.FindFolders(parentFolder, filter, view));
			}
			return findFoldersResults.Folders[0];
		}

		public virtual T FindByAlternativeId<T>(string alternativeId) where T : IConfigurable, new()
		{
			if (string.IsNullOrEmpty(alternativeId))
			{
				throw new ArgumentNullException("alternativeId");
			}
			ItemId itemId = null;
			if (!this.Cache.TryGetItemId(alternativeId, out itemId))
			{
				SearchFilter filter = new SearchFilter.IsEqualTo(EwsStoreObjectSchema.AlternativeId.StorePropertyDefinition, alternativeId);
				IEnumerable<T> source = this.InternalFindPaged<T>(filter, null, false, null, 1, new ProviderPropertyDefinition[0]);
				T t = source.FirstOrDefault<T>();
				this.Cache.SetItemId(alternativeId, (t == null) ? null : ((EwsStoreObjectId)t.Identity).EwsObjectId);
				return t;
			}
			if (itemId != null)
			{
				return (T)((object)this.Read<T>(new EwsStoreObjectId(itemId)));
			}
			return default(T);
		}

		protected Item FindItemByAlternativeId(string alternativeId)
		{
			if (string.IsNullOrEmpty(alternativeId))
			{
				throw new ArgumentNullException("alternativeId");
			}
			SearchFilter filter = new SearchFilter.IsEqualTo(EwsStoreObjectSchema.AlternativeId.StorePropertyDefinition, alternativeId);
			return this.InvokeServiceCall<FindItemsResults<Item>>(() => this.Service.FindItems(this.DefaultFolder, filter, new ItemView(1))).FirstOrDefault<Item>();
		}

		public virtual IEnumerable<T> FindInFolder<T>(SearchFilter filter, FolderId rootId) where T : IConfigurable, new()
		{
			return this.InternalFindPaged<T>(filter, rootId, false, null, 0, new ProviderPropertyDefinition[0]);
		}

		protected virtual IEnumerable<T> InternalFindPaged<T>(SearchFilter filter, FolderId rootId, bool deepSearch, SortBy[] sortBy, int pageSize, params ProviderPropertyDefinition[] properties) where T : IConfigurable, new()
		{
			Func<GetItemResponse, bool> func = null;
			Func<GetItemResponse, Item> func2 = null;
			EwsStoreDataProvider.<>c__DisplayClass1b<T> CS$<>8__locals1 = new EwsStoreDataProvider.<>c__DisplayClass1b<T>();
			CS$<>8__locals1.filter = filter;
			CS$<>8__locals1.rootId = rootId;
			CS$<>8__locals1.<>4__this = this;
			if (pageSize < 0 || pageSize > 1000)
			{
				throw new ArgumentOutOfRangeException("pageSize", pageSize, string.Format("pageSize should be between 1 and {0} or 0 to use the default page size: {1}", 1000, this.DefaultPageSize));
			}
			EwsStoreObject dummyObject = (EwsStoreObject)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			List<SearchFilter> filters = new List<SearchFilter>();
			if (CS$<>8__locals1.filter != null)
			{
				filters.Add(CS$<>8__locals1.filter);
			}
			SearchFilter versioningFilter = dummyObject.VersioningFilter;
			if (versioningFilter != null)
			{
				filters.Add(versioningFilter);
			}
			SearchFilter itemClassFilter = dummyObject.ItemClassFilter;
			if (itemClassFilter != null)
			{
				filters.Add(itemClassFilter);
			}
			if (filters.Count == 1)
			{
				CS$<>8__locals1.filter = filters[0];
			}
			else if (filters.Count > 1)
			{
				CS$<>8__locals1.filter = new SearchFilter.SearchFilterCollection(0, filters.ToArray());
			}
			CS$<>8__locals1.itemView = new ItemView((pageSize == 0) ? this.DefaultPageSize : pageSize);
			if (sortBy != null && sortBy.Length > 0)
			{
				foreach (SortBy sortBy2 in sortBy)
				{
					CS$<>8__locals1.itemView.OrderBy.Add(((EwsStoreObjectPropertyDefinition)sortBy2.ColumnDefinition).StorePropertyDefinition, (sortBy2.SortOrder == SortOrder.Ascending) ? 0 : 1);
				}
			}
			bool useBindItem = false;
			CS$<>8__locals1.propertySet = null;
			if (properties != null && properties.Length > 0)
			{
				CS$<>8__locals1.propertySet = this.CreatePropertySet(properties, out useBindItem);
			}
			else
			{
				CS$<>8__locals1.propertySet = this.CreatePropertySet<T>(out useBindItem);
			}
			if (useBindItem)
			{
				CS$<>8__locals1.itemView.PropertySet = new PropertySet(0);
			}
			else
			{
				CS$<>8__locals1.itemView.PropertySet = CS$<>8__locals1.propertySet;
			}
			for (;;)
			{
				EwsStoreDataProvider.<>c__DisplayClass1e<T> CS$<>8__locals2 = new EwsStoreDataProvider.<>c__DisplayClass1e<T>();
				CS$<>8__locals2.CS$<>8__locals1c = CS$<>8__locals1;
				FindItemsResults<Item> results = this.InvokeServiceCall<FindItemsResults<Item>>(() => CS$<>8__locals1.<>4__this.Service.FindItems(CS$<>8__locals1.rootId ?? CS$<>8__locals1.<>4__this.DefaultFolder, CS$<>8__locals1.filter, CS$<>8__locals1.itemView));
				CS$<>8__locals2.items = results.Items;
				if (useBindItem && results.Items.Count > 0)
				{
					ServiceResponseCollection<GetItemResponse> serviceResponseCollection = this.InvokeServiceCall<ServiceResponseCollection<GetItemResponse>>(() => CS$<>8__locals2.CS$<>8__locals1c.<>4__this.Service.BindToItems(from x in CS$<>8__locals2.items
					select x.Id, CS$<>8__locals2.CS$<>8__locals1c.propertySet));
					EwsStoreDataProvider.<>c__DisplayClass1e<T> CS$<>8__locals3 = CS$<>8__locals2;
					IEnumerable<GetItemResponse> source = serviceResponseCollection;
					if (func == null)
					{
						func = ((GetItemResponse x) => x.Item != null);
					}
					IEnumerable<GetItemResponse> source2 = source.Where(func);
					if (func2 == null)
					{
						func2 = ((GetItemResponse x) => x.Item);
					}
					CS$<>8__locals3.items = source2.Select(func2);
				}
				foreach (Item item in CS$<>8__locals2.items)
				{
					T instance = this.ObjectFromItem<T>(item);
					if (instance != null)
					{
						yield return instance;
					}
				}
				if (!results.MoreAvailable)
				{
					break;
				}
				CS$<>8__locals1.itemView.Offset = results.NextPageOffset.Value;
			}
			yield break;
		}

		private PropertySet CreatePropertySet<T>(out bool hasReturnOnBindProperty) where T : IConfigurable, new()
		{
			return this.CreatePropertySet(((EwsStoreObject)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)))).ObjectSchema.AllProperties, out hasReturnOnBindProperty);
		}

		private PropertySet CreatePropertySet(IEnumerable<PropertyDefinition> properties, out bool hasReturnOnBindProperty)
		{
			List<PropertyDefinition> expandedList = new List<PropertyDefinition>(properties);
			for (int i = 0; i < expandedList.Count; i++)
			{
				ProviderPropertyDefinition providerPropertyDefinition = expandedList[i] as ProviderPropertyDefinition;
				if (providerPropertyDefinition.IsCalculated)
				{
					expandedList.AddRange(from x in providerPropertyDefinition.SupportingProperties
					where !expandedList.Contains(x)
					select x);
				}
			}
			properties = expandedList;
			hasReturnOnBindProperty = properties.Any((PropertyDefinition x) => x is EwsStoreObjectPropertyDefinition && ((EwsStoreObjectPropertyDefinition)x).ReturnOnBind);
			PropertySet propertySet = new PropertySet(0);
			propertySet.AddRange(from x in properties
			where x is EwsStoreObjectPropertyDefinition && x != EwsStoreObjectSchema.Identity && ((EwsStoreObjectPropertyDefinition)x).StorePropertyDefinition.Version <= this.RequestedServerVersion
			select ((EwsStoreObjectPropertyDefinition)x).StorePropertyDefinition);
			if (!properties.Any((PropertyDefinition x) => x == EwsStoreObjectSchema.ExchangeVersion))
			{
				propertySet.Add(EwsStoreObjectSchema.ExchangeVersion.StorePropertyDefinition);
			}
			return propertySet;
		}

		private T ObjectFromItem<T>(Item item) where T : IConfigurable, new()
		{
			EwsStoreObject ewsStoreObject = (EwsStoreObject)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			object originalValue = null;
			ExchangeObjectVersion exchangeVersion = (ExchangeObjectVersion)EwsStoreObjectSchema.ExchangeVersion.DefaultValue;
			if (item.TryGetProperty(EwsStoreObjectSchema.ExchangeVersion.StorePropertyDefinition, ref originalValue))
			{
				exchangeVersion = (ExchangeObjectVersion)ValueConvertor.ConvertValue(originalValue, typeof(ExchangeObjectVersion), null);
				ewsStoreObject.SetExchangeVersion(exchangeVersion);
				if (ewsStoreObject.ExchangeVersion.Major > ewsStoreObject.MaximumSupportedExchangeObjectVersion.Major)
				{
					ExTraceGlobals.StorageTracer.TraceWarning<ItemId, byte, byte>(0L, "{0} has major version {1} which is greater than current one ({2}) and will be ignored", item.Id, ewsStoreObject.ExchangeVersion.Major, ewsStoreObject.MaximumSupportedExchangeObjectVersion.Major);
					return default(T);
				}
			}
			if (!string.IsNullOrEmpty(ewsStoreObject.ItemClass) && !ewsStoreObject.ItemClass.Equals(item.ItemClass, StringComparison.OrdinalIgnoreCase))
			{
				return default(T);
			}
			ewsStoreObject.CopyFromItemObject(item, this.RequestedServerVersion);
			if (ewsStoreObject.MaximumSupportedExchangeObjectVersion.IsOlderThan(ewsStoreObject.ExchangeVersion))
			{
				ExTraceGlobals.StorageTracer.TraceWarning<ItemId, ExchangeObjectVersion, ExchangeObjectVersion>(0L, "{0} has version {1} which is greater than current one ({2}) and will be read-only", item.Id, ewsStoreObject.ExchangeVersion, ewsStoreObject.MaximumSupportedExchangeObjectVersion);
				ewsStoreObject.SetIsReadOnly(true);
			}
			ValidationError[] array = ewsStoreObject.ValidateRead();
			ewsStoreObject.ResetChangeTracking(true);
			if (array.Length > 0)
			{
				foreach (ValidationError validationError in array)
				{
					PropertyValidationError propertyValidationError = validationError as PropertyValidationError;
					ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "Object '{0}' read from '{1}' failed validation. Attribute: '{2}'. Invalid data: '{3}'. Error message: '{4}'.", new object[]
					{
						ewsStoreObject.Identity,
						this.Mailbox.ToString() + "\\" + this.DefaultFolder.ToString(),
						(propertyValidationError != null) ? propertyValidationError.PropertyDefinition.Name : "<null>",
						(propertyValidationError != null) ? (propertyValidationError.InvalidData ?? "<null>") : "<null>",
						validationError.Description
					});
				}
			}
			return (T)((object)this.FilterObject(ewsStoreObject));
		}

		protected void InvokeServiceCall(Action callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			this.InvokeServiceCall<object>(delegate()
			{
				callback();
				return null;
			});
		}

		protected T InvokeServiceCall<T>(Func<T> callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			int num = 2;
			while (num-- > 0)
			{
				Exception ex = null;
				bool flag = false;
				bool flag2 = false;
				try
				{
					T result = callback();
					this.Cache.FailedCount = 0;
					return result;
				}
				catch (ServiceResponseException ex2)
				{
					if (EwsStoreDataProvider.ServiceErrorsForWrongEws.Contains(ex2.ErrorCode))
					{
						flag2 = true;
					}
					else if (EwsStoreDataProvider.TransientServiceErrors.Contains(ex2.ErrorCode))
					{
						flag = true;
					}
					ex = ex2;
				}
				catch (ServiceVersionException ex3)
				{
					flag2 = true;
					ex = ex3;
				}
				catch (ServiceRemoteException ex4)
				{
					ex = ex4;
				}
				catch (ServiceLocalException ex5)
				{
					ex = ex5;
				}
				if (this.Cache.FailedCount >= 5 && (ExDateTime.Now - this.Cache.LastDiscoverTime).TotalMinutes >= 5.0)
				{
					this.ExpireCache();
				}
				else if (flag2 && ((ExDateTime.Now - this.Cache.LastDiscoverTime).Seconds >= 30 || this.Cache.FailedCount == 0))
				{
					this.ExpireCache();
				}
				this.Cache.FailedCount++;
				if ((!this.CanRetry && !flag2) || num <= 0 || !flag)
				{
					throw new DataSourceOperationException(new LocalizedString(ex.Message), ex);
				}
			}
			throw new InvalidOperationException();
		}

		private void OnSerializeCustomSoapHeaders(XmlWriter writer)
		{
			object obj = null;
			if (this.logonType != null)
			{
				obj = EwsHelper.CreateSpecialLogonAuthenticationHeader(this.Mailbox, this.logonType.Value, this.budgetType, this.RequiredServerVersion);
			}
			else if (this.securityAccessToken != null)
			{
				obj = EwsHelper.CreateSerializedSecurityContext(this.Mailbox, this.securityAccessToken);
			}
			if (obj != null)
			{
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(obj.GetType());
				safeXmlSerializer.Serialize(writer, obj);
			}
		}

		public void Delete(IConfigurable instance)
		{
			EwsStoreObject ewsStoreObject = (EwsStoreObject)instance;
			if (ewsStoreObject.IsReadOnly)
			{
				throw new InvalidOperationException("Can't delete read-only object.");
			}
			if (instance.ObjectState == ObjectState.Deleted)
			{
				throw new InvalidOperationException(ServerStrings.ExceptionObjectHasBeenDeleted);
			}
			Item item = this.BindItem(ewsStoreObject.Identity.EwsObjectId, PropertySet.IdOnly);
			if (item != null)
			{
				this.InvokeServiceCall(delegate()
				{
					item.Delete(0);
				});
			}
			this.Cache.ClearItemCache(ewsStoreObject);
			ewsStoreObject.ResetChangeTracking();
			ewsStoreObject.MarkAsDeleted();
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.FindPaged<T>(filter, rootId, deepSearch, sortBy, 0).Cast<IConfigurable>().ToArray<IConfigurable>();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			if (filter != null)
			{
				throw new ArgumentException("filter isn't supported.");
			}
			FolderId rootId2 = null;
			if (rootId != null)
			{
				rootId2 = new FolderId(((EwsStoreObjectId)rootId).EwsObjectId.UniqueId);
			}
			return this.InternalFindPaged<T>(null, rootId2, deepSearch, (sortBy == null) ? null : new SortBy[]
			{
				sortBy
			}, pageSize, new ProviderPropertyDefinition[0]);
		}

		public virtual IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			bool flag;
			Item item = this.BindItem(((EwsStoreObjectId)identity).EwsObjectId, this.CreatePropertySet<T>(out flag));
			return (item == null) ? default(T) : this.ObjectFromItem<T>(item);
		}

		public virtual void Save(IConfigurable instance)
		{
			if (instance.ObjectState == ObjectState.Unchanged)
			{
				return;
			}
			if (instance.ObjectState == ObjectState.Deleted)
			{
				throw new NotSupportedException("Can't save deleted object.");
			}
			EwsStoreObject ewsStoreObject = (EwsStoreObject)instance;
			if (ewsStoreObject.IsReadOnly)
			{
				throw new InvalidOperationException("Can't save read-only object.");
			}
			ValidationError[] array = ewsStoreObject.Validate();
			if (array.Length > 0)
			{
				throw new DataValidationException(array[0]);
			}
			if (ewsStoreObject.MaximumSupportedExchangeObjectVersion.IsOlderThan(ewsStoreObject.ExchangeVersion))
			{
				throw new DataValidationException(new PropertyValidationError(DataStrings.ErrorCannotSaveBecauseTooNew(ewsStoreObject.ExchangeVersion, ewsStoreObject.MaximumSupportedExchangeObjectVersion), ADObjectSchema.ExchangeVersion, ewsStoreObject.ExchangeVersion));
			}
			if (ewsStoreObject.ObjectState == ObjectState.New)
			{
				Item item = this.CreateItemObjectForNew();
				ewsStoreObject.CopyChangeToItemObject(item, this.RequestedServerVersion);
				this.InvokeServiceCall(delegate()
				{
					item.Save(this.DefaultFolder);
				});
				ewsStoreObject.CopyFromItemObject(item, this.RequestedServerVersion);
			}
			else
			{
				bool flag;
				Item item = this.BindItem(ewsStoreObject.Identity.EwsObjectId, this.CreatePropertySet(ewsStoreObject.GetChangedPropertyDefinitions(), out flag));
				if (item != null)
				{
					ewsStoreObject.CopyChangeToItemObject(item, this.RequestedServerVersion);
					this.InvokeServiceCall(delegate()
					{
						item.Update(2);
					});
				}
			}
			ewsStoreObject.ResetChangeTracking(true);
		}

		public bool ApplyPolicyTag(Guid policyTagGuid, Folder folder, bool ignoreError = false)
		{
			if (this.RequestedServerVersion >= FolderSchema.PolicyTag.Version)
			{
				try
				{
					if (folder.PolicyTag == null || folder.PolicyTag.RetentionId != policyTagGuid)
					{
						folder.PolicyTag = new PolicyTag(true, policyTagGuid);
						this.InvokeServiceCall(delegate()
						{
							folder.Update();
						});
						return true;
					}
					return false;
				}
				catch (LocalizedException ex)
				{
					ExTraceGlobals.StorageTracer.TraceError<IExchangePrincipal, string>(0L, "EwsStoreDataProvider::failed to apply PolicyTag to mailbox '{0}', message: {1}", this.Mailbox, ex.Message);
					if (!ignoreError)
					{
						throw;
					}
					return false;
				}
			}
			ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal, ServerVersion>(0L, "EwsStoreDataProvider::skip applying PolicyTag to mailbox '{0}' because the mailbox version '{1}' is too low.", this.Mailbox, this.MailboxVersion);
			return false;
		}

		public void ApplyPolicyTag(Guid policyTagGuid, params Item[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				Item item = items[i];
				if (item.PolicyTag == null || item.PolicyTag.RetentionId != policyTagGuid)
				{
					item.PolicyTag = new PolicyTag(true, policyTagGuid);
					this.InvokeServiceCall(delegate()
					{
						item.Update(1);
					});
				}
			}
		}

		protected virtual Item CreateItemObjectForNew()
		{
			return new EmailMessage(this.Service);
		}

		protected virtual EwsStoreDataProviderCacheEntry CreateCacheEntry()
		{
			return new EwsStoreDataProviderCacheEntry();
		}

		protected virtual FolderId GetDefaultFolder()
		{
			return new FolderId(10, new Mailbox(this.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString()));
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return this.mailbox.ToString();
			}
		}

		public ExchangeService Service
		{
			get
			{
				if (this.service == null)
				{
					string text = this.Cache.EwsUrl;
					if (text == null && (ExDateTime.Now - this.Cache.LastDiscoverTime).TotalMinutes >= 1.0)
					{
						text = (this.Cache.EwsUrl = EwsHelper.DiscoverEwsUrl(this.Mailbox));
						this.Cache.LastDiscoverTime = ExDateTime.Now;
					}
					if (string.IsNullOrEmpty(text))
					{
						StorageGlobals.EventLogger.LogEvent(StorageEventLogConstants.Tuple_ErrorDiscoverEwsUrlForMailbox, this.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString(), new object[]
						{
							this.Mailbox
						});
						throw new NoInternalEwsAvailableException(this.Mailbox.ToString());
					}
					ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal, string>(0L, "EwsStoreDataProvider: Connect to mailbox '{0}' with EWS at '{1}'.", this.Mailbox, text);
					this.service = new ExchangeService(this.RequestedServerVersion);
					this.service.HttpWebRequestFactory = new EwsHttpWebRequestFactoryEx
					{
						ServerCertificateValidationCallback = this.CertificateValidationCallback
					};
					this.service.Url = new Uri(text);
					this.service.UserAgent = WellKnownUserAgent.GetEwsNegoAuthUserAgent("EwsStoreDataProvider");
					this.service.HttpHeaders["X-AnchorMailbox"] = this.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString();
					this.service.OnSerializeCustomSoapHeaders += new CustomXmlSerializationDelegate(this.OnSerializeCustomSoapHeaders);
					this.OnExchangeServiceCreated(this.service);
				}
				return this.service;
			}
		}

		protected virtual void OnExchangeServiceCreated(ExchangeService service)
		{
		}

		public bool CanRetry { get; set; }

		public FolderId DefaultFolder
		{
			get
			{
				FolderId result;
				if ((result = this.defaultFolder) == null)
				{
					result = (this.defaultFolder = this.GetDefaultFolder());
				}
				return result;
			}
		}

		public ExchangeVersion RequestedServerVersion
		{
			get
			{
				if (this.requestedServerVersion == null)
				{
					if (this.MailboxVersion.Major == (int)ExchangeObjectVersion.Exchange2010.ExchangeBuild.Major)
					{
						this.requestedServerVersion = new ExchangeVersion?((this.MailboxVersion.Minor == 0) ? 1 : ((this.MailboxVersion.Minor == 1) ? 2 : ((this.MailboxVersion.Minor == 2) ? 3 : 2)));
					}
					else if (this.MailboxVersion.Major >= (int)ExchangeObjectVersion.Exchange2012.ExchangeBuild.Major)
					{
						this.requestedServerVersion = new ExchangeVersion?(4);
					}
					else
					{
						this.requestedServerVersion = new ExchangeVersion?(0);
					}
					if (this.requestedServerVersion.Value < this.RequiredServerVersion)
					{
						throw new MailboxVersionTooLowException(this.Mailbox.ToString(), this.RequiredServerVersion.ToString(), this.MailboxVersion.ToString());
					}
				}
				return this.requestedServerVersion.Value;
			}
		}

		protected virtual ExchangeVersion RequiredServerVersion
		{
			get
			{
				return 2;
			}
		}

		protected virtual int DefaultPageSize
		{
			get
			{
				return 50;
			}
		}

		internal IExchangePrincipal Mailbox
		{
			get
			{
				return this.mailbox.Value;
			}
		}

		protected ServerVersion MailboxVersion
		{
			get
			{
				if (this.mailboxVersion == null)
				{
					this.mailboxVersion = new ServerVersion(this.Mailbox.MailboxInfo.Location.ServerVersion);
				}
				return this.mailboxVersion;
			}
		}

		protected SpecialLogonType? LogonType
		{
			get
			{
				return this.logonType;
			}
			set
			{
				this.logonType = value;
			}
		}

		protected OpenAsAdminOrSystemServiceBudgetTypeType BudgetType
		{
			get
			{
				return this.budgetType;
			}
			set
			{
				this.budgetType = value;
			}
		}

		protected EwsStoreDataProviderCacheEntry Cache
		{
			get
			{
				if (this.cache == null)
				{
					string cacheKey = this.CacheKey;
					EwsStoreDataProviderCacheEntry ewsStoreDataProviderCacheEntry;
					if (!EwsStoreDataProvider.caches.TryGetValue(cacheKey, out ewsStoreDataProviderCacheEntry))
					{
						this.cache = this.CreateCacheEntry();
						EwsStoreDataProvider.caches[cacheKey] = this.cache;
					}
					else
					{
						this.cache = ewsStoreDataProviderCacheEntry;
					}
				}
				return this.cache;
			}
		}

		protected virtual RemoteCertificateValidationCallback CertificateValidationCallback
		{
			get
			{
				return new RemoteCertificateValidationCallback(EwsHelper.CertificateErrorHandler);
			}
		}

		private string CacheKey
		{
			get
			{
				return base.GetType().FullName + ":" + this.Mailbox.ObjectId.ObjectGuid.ToString();
			}
		}

		public const int MaximumPageSize = 1000;

		private static readonly MruDictionaryCache<string, EwsStoreDataProviderCacheEntry> caches = new MruDictionaryCache<string, EwsStoreDataProviderCacheEntry>(10, 100, 60);

		private static readonly ServiceError[] TransientServiceErrors = new ServiceError[]
		{
			6,
			8,
			75,
			126,
			128,
			262,
			263,
			363,
			101
		};

		private static readonly ServiceError[] ServiceErrorsForWrongEws = new ServiceError[]
		{
			83,
			414,
			222
		};

		private SpecialLogonType? logonType;

		private OpenAsAdminOrSystemServiceBudgetTypeType budgetType;

		private LazilyInitialized<IExchangePrincipal> mailbox;

		private ServerVersion mailboxVersion;

		private ExchangeService service;

		private SecurityAccessToken securityAccessToken;

		private FolderId defaultFolder;

		private ExchangeVersion? requestedServerVersion;

		private EwsStoreDataProviderCacheEntry cache;
	}
}
