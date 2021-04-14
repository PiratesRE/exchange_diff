using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Mobility;

namespace Microsoft.Exchange.Management.Extension
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OWAExtensionDataProvider : XsoMailboxDataProviderBase
	{
		public bool IsDebug
		{
			get
			{
				return this.isDebug;
			}
		}

		public string RawMasterTableXml
		{
			get
			{
				return this.rawMasterTableXml;
			}
		}

		public string RawOrgMasterTableXml
		{
			get
			{
				return this.rawOrgMasterTableXml;
			}
		}

		public OWAExtensionDataProvider(string domain, IRecipientSession adRecipientSession, ADSessionSettings adSessionSettings, bool isUserScope, ADUser mailboxOwner, string action, bool isDebug) : base(adSessionSettings, mailboxOwner, action)
		{
			this.domain = domain;
			this.adRecipientSession = adRecipientSession;
			this.isUserScope = isUserScope;
			this.isDebug = isDebug;
		}

		internal OWAExtensionDataProvider()
		{
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
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
			InstalledExtensionTable installedList = null;
			OWAExtensionDataProvider.RunAction(delegate
			{
				installedList = InstalledExtensionTable.CreateInstalledExtensionTable(this.domain, this.isUserScope, null, this.MailboxSession);
			});
			if (owaExtensionId == null || (owaExtensionId.DisplayName == null && owaExtensionId.AppIdValue == null))
			{
				List<ExtensionData> extensions = null;
				OWAExtensionDataProvider.RunAction(delegate
				{
					extensions = installedList.GetExtensions(null, false, this.isDebug, out this.rawOrgMasterTableXml);
					this.rawMasterTableXml = (this.isDebug ? installedList.MasterTableXml.InnerXml : string.Empty);
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
					installedList.TryGetExtension(owaExtensionId.AppIdValue, out extensionData, this.isDebug, out this.rawOrgMasterTableXml);
					this.rawMasterTableXml = (this.isDebug ? installedList.MasterTableXml.InnerXml : string.Empty);
				});
				if (extensionData != null)
				{
					yield return (T)((object)this.ConvertStoreObjectToPresentationObject(extensionData));
				}
			}
			yield break;
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
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
				using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(this.domain, this.isUserScope, null, this.MailboxSession))
				{
					switch (owaExtension.ObjectState)
					{
					case ObjectState.New:
						if (!owaExtension.IsDownloadOnly)
						{
							installedExtensionTable.InstallExtension(owaExtension.GetExtensionDataForInstall(this.adRecipientSession), false);
						}
						break;
					case ObjectState.Changed:
						if (this.isUserScope)
						{
							installedExtensionTable.ConfigureUserExtension(owaExtension.AppId, owaExtension.Enabled);
						}
						else
						{
							OrgApp orgApp = instance as OrgApp;
							if (orgApp == null)
							{
								throw new NotSupportedException("Save: " + instance.GetType().FullName);
							}
							installedExtensionTable.ConfigureOrgExtension(orgApp.AppId, orgApp.Enabled, orgApp.DefaultStateForUser == DefaultStateForUser.AlwaysEnabled, orgApp.DefaultStateForUser == DefaultStateForUser.Enabled, orgApp.ProvidedTo, OrgApp.ConvertPresentationFormatToWireUserList(this.adRecipientSession, orgApp.UserList));
						}
						break;
					}
				}
			});
		}

		protected override void InternalDelete(ConfigurableObject instance)
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
				using (InstalledExtensionTable installedExtensionTable = InstalledExtensionTable.CreateInstalledExtensionTable(this.domain, this.isUserScope, null, this.MailboxSession))
				{
					if (this.isUserScope)
					{
						if (ExtensionInstallScope.User != owaExtension.Scope)
						{
							throw new OwaExtensionOperationException(Strings.ErrorUninstallProvidedExtension(owaExtension.DisplayName));
						}
						if (this.TryRemovePerExtensionFai(owaExtension.AppId, owaExtension.AppVersion))
						{
							installedExtensionTable.UninstallExtension(owaExtension.AppId);
						}
						else
						{
							installedExtensionTable.ConfigureUserExtension(owaExtension.AppId, false);
						}
					}
					else
					{
						if (ExtensionInstallScope.Default == owaExtension.Scope)
						{
							throw new OwaExtensionOperationException(Strings.ErrorUninstallDefaultExtension(owaExtension.DisplayName));
						}
						installedExtensionTable.UninstallExtension(owaExtension.AppId);
					}
				}
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OWAExtensionDataProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		internal static void RunAction(Action action)
		{
			Exception ex = InstalledExtensionTable.RunClientExtensionAction(action);
			if (ex != null)
			{
				throw (ex is LocalizedException) ? ex : new OwaExtensionOperationException(new LocalizedString(ex.Message), ex);
			}
		}

		private bool TryRemovePerExtensionFai(string extensionId, string version)
		{
			StoreObjectId extensionFolderId = ExtensionPackageManager.GetExtensionFolderId(base.MailboxSession);
			bool result;
			using (UserConfiguration folderConfiguration = UserConfigurationHelper.GetFolderConfiguration(base.MailboxSession, extensionFolderId, ExtensionPackageManager.GetFaiName(extensionId, version), UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, false, false))
			{
				if (folderConfiguration != null && OperationResult.Succeeded != base.MailboxSession.UserConfigurationManager.DeleteFolderConfigurations(extensionFolderId, new string[]
				{
					ExtensionPackageManager.GetFaiName(extensionId, version)
				}))
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private App ConvertStoreObjectToPresentationObject(ExtensionData extensionData)
		{
			DefaultStateForUser? defaultStateForUser = new DefaultStateForUser?(extensionData.IsMandatory ? DefaultStateForUser.AlwaysEnabled : (extensionData.IsEnabledByDefault ? DefaultStateForUser.Enabled : DefaultStateForUser.Disabled));
			Uri uri = extensionData.IconURL;
			string uriString;
			Exception ex;
			if (null != uri && ExtensionInstallScope.Default == extensionData.Scope.GetValueOrDefault() && DefaultExtensionTable.TryConvertToCompleteUrl(base.MailboxSession.MailboxOwner, uri.OriginalString, extensionData.ExtensionId, out uriString, out ex))
			{
				uri = new Uri(uriString, UriKind.RelativeOrAbsolute);
			}
			if (this.isUserScope)
			{
				return new App((ExtensionInstallScope.User == extensionData.Scope.GetValueOrDefault()) ? null : defaultStateForUser, extensionData.MarketplaceAssetID, extensionData.MarketplaceContentMarket, extensionData.ProviderName, uri, extensionData.ExtensionId, extensionData.VersionAsString, extensionData.Type, extensionData.Scope, extensionData.RequestedCapabilities, extensionData.DisplayName, extensionData.Description, extensionData.Enabled, extensionData.Manifest.OuterXml, base.MailboxOwner.ObjectId, extensionData.Etoken, extensionData.EtokenData, extensionData.AppStatus);
			}
			return new OrgApp(defaultStateForUser, extensionData.ProvidedTo, OrgApp.ConvertWireUserListToPresentationFormat(this.adRecipientSession, extensionData.SpecificUsers), extensionData.MarketplaceAssetID, extensionData.MarketplaceContentMarket, extensionData.ProviderName, uri, extensionData.ExtensionId, extensionData.VersionAsString, extensionData.Type, extensionData.Scope, extensionData.RequestedCapabilities, extensionData.DisplayName, extensionData.Description, extensionData.Enabled, extensionData.Manifest.OuterXml, extensionData.Etoken, extensionData.EtokenData, extensionData.AppStatus, base.MailboxOwner.ObjectId);
		}

		private readonly bool isUserScope;

		private readonly string domain;

		private readonly bool isDebug;

		private string rawMasterTableXml;

		private string rawOrgMasterTableXml;

		private readonly IRecipientSession adRecipientSession;
	}
}
