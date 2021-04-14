using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class XsoStoreDataProviderBase : DisposeTrackableBase, IConfigDataProvider
	{
		private bool IsStoreSessionFromOuter { get; set; }

		public StoreSession StoreSession { get; protected set; }

		public static ExchangePrincipal GetExchangePrincipalWithAdSessionSettingsForOrg(OrganizationId organizationId, ADUser user)
		{
			ADSessionSettings adSettings = organizationId.ToADSessionSettings();
			return ExchangePrincipal.FromADUser(adSettings, user, RemotingOptions.AllowCrossSite);
		}

		public IConfigurationSession GetSystemConfigurationSession(OrganizationId organizationId)
		{
			ADSessionSettings sessionSettings = organizationId.ToADSessionSettings();
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 80, "GetSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Management\\XsoDriver\\XsoStoreDataProviderBase.cs");
		}

		public XsoStoreDataProviderBase(StoreSession session)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (session == null)
				{
					throw new ArgumentNullException("session");
				}
				this.IsStoreSessionFromOuter = true;
				this.StoreSession = session;
				disposeGuard.Success();
			}
		}

		public XsoStoreDataProviderBase()
		{
		}

		string IConfigDataProvider.Source
		{
			get
			{
				if (this.source == null)
				{
					this.source = this.StoreSession.ToString();
				}
				return this.source;
			}
		}

		public virtual IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			base.CheckDisposed();
			IConfigurable[] array = this.Find<T>(new FalseFilter(), identity, true, null);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public virtual IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			base.CheckDisposed();
			return (IConfigurable[])this.FindPaged<T>(filter, rootId, deepSearch, sortBy, 0).ToArray<T>();
		}

		public virtual IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			base.CheckDisposed();
			if (!typeof(ConfigurableObject).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			foreach (T item in this.InternalFindPaged<T>(filter, rootId, deepSearch, sortBy, pageSize))
			{
				ConfigurableObject userConfigurationObject = (ConfigurableObject)((object)item);
				foreach (PropertyDefinition propertyDefinition in userConfigurationObject.ObjectSchema.AllProperties)
				{
					ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
					if (providerPropertyDefinition != null && !providerPropertyDefinition.IsCalculated)
					{
						object obj = null;
						userConfigurationObject.propertyBag.TryGetField(providerPropertyDefinition, ref obj);
						userConfigurationObject.InstantiationErrors.AddRange(providerPropertyDefinition.ValidateProperty(obj ?? providerPropertyDefinition.DefaultValue, userConfigurationObject.propertyBag, true));
					}
				}
				userConfigurationObject.ResetChangeTracking(true);
				yield return item;
			}
			yield break;
		}

		protected abstract IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new();

		public virtual void Save(IConfigurable instance)
		{
			base.CheckDisposed();
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			ConfigurableObject configurableObject = instance as ConfigurableObject;
			if (configurableObject == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			ValidationError[] array = configurableObject.Validate();
			if (array.Length > 0)
			{
				throw new DataValidationException(array[0]);
			}
			this.InternalSave(configurableObject);
			configurableObject.ResetChangeTracking(true);
		}

		protected abstract void InternalSave(ConfigurableObject instance);

		public void Delete(IConfigurable instance)
		{
			base.CheckDisposed();
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			ConfigurableObject configurableObject = instance as ConfigurableObject;
			if (configurableObject == null)
			{
				throw new NotSupportedException("Delete: " + instance.GetType().FullName);
			}
			if (configurableObject.ObjectState == ObjectState.Deleted)
			{
				throw new InvalidOperationException(ServerStrings.ExceptionObjectHasBeenDeleted);
			}
			this.InternalDelete(configurableObject);
			configurableObject.ResetChangeTracking();
			configurableObject.MarkAsDeleted();
		}

		protected virtual void InternalDelete(ConfigurableObject instance)
		{
			throw new NotImplementedException("InternalDelete");
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (!this.IsStoreSessionFromOuter && this.StoreSession != null)
				{
					this.StoreSession.Dispose();
				}
				this.StoreSession = null;
			}
		}

		private string source;
	}
}
