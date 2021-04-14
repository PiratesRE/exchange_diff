using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Management.Extension
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OWAAppDataProviderForNonMailboxUser : DisposeTrackableBase, IConfigDataProvider
	{
		string IConfigDataProvider.Source
		{
			get
			{
				return this.ToString();
			}
		}

		public OWAAppDataProviderForNonMailboxUser(string domain, IRecipientSession adRecipientSession, ADSessionSettings adSessionSettings, bool isUserScope, string action)
		{
			this.domain = domain;
			this.adRecipientSession = adRecipientSession;
			this.isUserScope = isUserScope;
		}

		public virtual IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			IConfigurable[] array = this.Find<T>(new FalseFilter(), identity, true, null);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public virtual IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return (IConfigurable[])this.FindPaged<T>(filter, rootId, deepSearch, sortBy, 0).ToArray<T>();
		}

		public virtual IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			if (!typeof(ConfigurableObject).IsAssignableFrom(typeof(T)))
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

		public virtual void Save(IConfigurable instance)
		{
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

		protected IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			AppId owaExtensionId = rootId as AppId;
			if (sortBy != null)
			{
				throw new NotSupportedException("sortBy");
			}
			if (rootId != null && owaExtensionId == null)
			{
				throw new NotSupportedException("rootId");
			}
			if (owaExtensionId == null || (owaExtensionId.DisplayName == null && owaExtensionId.AppIdValue == null))
			{
				List<ExtensionData> extensions = new List<ExtensionData>();
				OWAExtensionDataProvider.RunAction(delegate
				{
					Dictionary<string, ExtensionData> providedExtensions = this.GetProvidedExtensions(null, this.domain, false);
					foreach (ExtensionData item in providedExtensions.Values)
					{
						extensions.Add(item);
					}
				});
				foreach (ExtensionData extensionData2 in extensions)
				{
					yield return (T)((object)this.ConvertStoreObjectToPresentationObject(extensionData2));
				}
			}
			else if (!string.IsNullOrEmpty(owaExtensionId.AppIdValue))
			{
				ExtensionData extensionData = null;
				OWAExtensionDataProvider.RunAction(delegate
				{
					Dictionary<string, ExtensionData> providedExtensions = this.GetProvidedExtensions(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
					{
						owaExtensionId.AppIdValue
					}, null, false);
					providedExtensions.TryGetValue(owaExtensionId.AppIdValue, out extensionData);
				});
				if (extensionData != null)
				{
					yield return (T)((object)this.ConvertStoreObjectToPresentationObject(extensionData));
				}
			}
			yield break;
		}

		public void Delete(IConfigurable instance)
		{
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

		protected void InternalSave(ConfigurableObject instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			App owaExtension = instance as App;
			if (owaExtension == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			OWAExtensionDataProvider.RunAction(delegate
			{
				switch (owaExtension.ObjectState)
				{
				case ObjectState.New:
					if (!owaExtension.IsDownloadOnly)
					{
						OrgExtensionTable.SetOrgExtension(this.domain, 0, null, owaExtension.GetExtensionDataForInstall(this.adRecipientSession));
						return;
					}
					break;
				case ObjectState.Unchanged:
					break;
				case ObjectState.Changed:
				{
					OrgApp orgApp = instance as OrgApp;
					if (orgApp == null)
					{
						throw new NotSupportedException("Save: " + instance.GetType().FullName);
					}
					OrgExtensionTable.ConfigureOrgExtension(this.domain, orgApp.AppId, orgApp.Enabled, orgApp.DefaultStateForUser == DefaultStateForUser.AlwaysEnabled, orgApp.DefaultStateForUser == DefaultStateForUser.Enabled, orgApp.ProvidedTo, OrgApp.ConvertPresentationFormatToWireUserList(this.adRecipientSession, orgApp.UserList));
					break;
				}
				default:
					return;
				}
			});
		}

		protected void InternalDelete(ConfigurableObject instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			App app = instance as App;
			if (app == null)
			{
				throw new NotSupportedException("Save: " + instance.GetType().FullName);
			}
			OrgExtensionTable.SetOrgExtension(this.domain, 1, app.AppId, null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OWAAppDataProviderForNonMailboxUser>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private App ConvertStoreObjectToPresentationObject(ExtensionData extensionData)
		{
			DefaultStateForUser? defaultStateForUser = new DefaultStateForUser?(extensionData.IsMandatory ? DefaultStateForUser.AlwaysEnabled : (extensionData.IsEnabledByDefault ? DefaultStateForUser.Enabled : DefaultStateForUser.Disabled));
			Uri iconURL = extensionData.IconURL;
			if (this.isUserScope)
			{
				return new App((ExtensionInstallScope.User == extensionData.Scope.GetValueOrDefault()) ? null : defaultStateForUser, extensionData.MarketplaceAssetID, extensionData.MarketplaceContentMarket, extensionData.ProviderName, iconURL, extensionData.ExtensionId, extensionData.VersionAsString, extensionData.Type, extensionData.Scope, extensionData.RequestedCapabilities, extensionData.DisplayName, extensionData.Description, extensionData.Enabled, extensionData.Manifest.OuterXml, new ADObjectId(), extensionData.Etoken, extensionData.EtokenData, extensionData.AppStatus);
			}
			return new OrgApp(defaultStateForUser, extensionData.ProvidedTo, OrgApp.ConvertWireUserListToPresentationFormat(this.adRecipientSession, extensionData.SpecificUsers), extensionData.MarketplaceAssetID, extensionData.MarketplaceContentMarket, extensionData.ProviderName, iconURL, extensionData.ExtensionId, extensionData.VersionAsString, extensionData.Type, extensionData.Scope, extensionData.RequestedCapabilities, extensionData.DisplayName, extensionData.Description, extensionData.Enabled, extensionData.Manifest.OuterXml, extensionData.Etoken, extensionData.EtokenData, extensionData.AppStatus, new ADObjectId());
		}

		private Dictionary<string, ExtensionData> GetProvidedExtensions(HashSet<string> formattedRequestedExtensionIds, string domain, bool shouldReturnEnabledOnly)
		{
			StringList userEnabledExtensionIds = null;
			StringList userDisabledExtensionIds = null;
			StringList requestedExtensionIds = null;
			if (formattedRequestedExtensionIds != null)
			{
				requestedExtensionIds = new StringList(formattedRequestedExtensionIds);
			}
			OrgExtensionTable.RequestData requestData;
			string text;
			return OrgExtensionTable.GetOrgExtensions(requestedExtensionIds, domain, shouldReturnEnabledOnly, false, null, userEnabledExtensionIds, userDisabledExtensionIds, out requestData, false, out text, false);
		}

		private readonly bool isUserScope;

		private readonly string domain;

		private readonly IRecipientSession adRecipientSession;
	}
}
