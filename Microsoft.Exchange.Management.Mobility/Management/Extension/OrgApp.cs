using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Extension
{
	[Serializable]
	public sealed class OrgApp : App
	{
		public OrgApp()
		{
		}

		public OrgApp(DefaultStateForUser? defaultStateForUser, ClientExtensionProvidedTo providedTo, MultiValuedProperty<ADObjectId> userList, string marketplaceAssetID, string marketplaceContentMarket, string providerName, Uri iconURL, string extensionId, string version, ExtensionType? type, ExtensionInstallScope? scope, RequestedCapabilities? requirements, string displayName, string description, bool enabled, string manifestXml, string etoken, EntitlementTokenData eTokenData, string appStatus, ADObjectId mailboxOwnerId) : base(defaultStateForUser, marketplaceAssetID, marketplaceContentMarket, providerName, iconURL, extensionId, version, type, scope, requirements, displayName, description, enabled, manifestXml, mailboxOwnerId, etoken, eTokenData, appStatus)
		{
			this.ProvidedTo = providedTo;
			this.UserList = userList;
		}

		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return OrgApp.schema;
			}
		}

		public ClientExtensionProvidedTo ProvidedTo
		{
			get
			{
				return (ClientExtensionProvidedTo)this[OWAOrgExtensionSchema.ProvidedTo];
			}
			set
			{
				this[OWAOrgExtensionSchema.ProvidedTo] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> UserList
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OWAOrgExtensionSchema.UserList];
			}
			set
			{
				this[OWAOrgExtensionSchema.UserList] = value;
			}
		}

		internal static MultiValuedProperty<ADObjectId> ConvertUserListToPresentationFormat(DataAccessTask<App> task, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> userList)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = null;
			if (userList != null && userList.Count > 0)
			{
				multiValuedProperty = new MultiValuedProperty<ADObjectId>();
				foreach (RecipientWithAdUserIdParameter<RecipientIdParameter> recipientWithAdUserIdParameter in userList)
				{
					ADRecipient recipient = (ADRecipient)getDataObject(recipientWithAdUserIdParameter, task.TenantGlobalCatalogSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientWithAdUserIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientWithAdUserIdParameter.ToString())), ExchangeErrorCategory.Client);
					if (!Array.Exists<RecipientTypeDetails>(OrgApp.SupportedRecipientTypeDetails, (RecipientTypeDetails item) => item == recipient.RecipientTypeDetails))
					{
						task.WriteError(new LocalizedException(Strings.ErrorUnsupportedRecipientType(recipient.Id.ToString(), string.Join(",", (from detail in OrgApp.SupportedRecipientTypeDetails
						select detail.ToString()).ToArray<string>()))), ErrorCategory.InvalidArgument, null);
					}
					multiValuedProperty.Add(recipient.Guid);
				}
			}
			return multiValuedProperty;
		}

		internal static MultiValuedProperty<ADObjectId> ConvertWireUserListToPresentationFormat(IRecipientSession adRecipientSession, string[] specificUsers)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = null;
			if (specificUsers != null && specificUsers.Length > 0)
			{
				Result<ADRawEntry>[] array = null;
				if (InstalledExtensionTable.IsMultiTenancyEnabled)
				{
					if (adRecipientSession is ITenantRecipientSession)
					{
						array = ((ITenantRecipientSession)adRecipientSession).FindByExternalDirectoryObjectIds(specificUsers, false, new PropertyDefinition[]
						{
							ADObjectSchema.DistinguishedName
						});
					}
					else
					{
						array = ((ADRecipientObjectSession)adRecipientSession).FindByExternalDirectoryObjectIds(specificUsers, false, new PropertyDefinition[]
						{
							ADObjectSchema.DistinguishedName
						});
					}
				}
				else
				{
					List<ADObjectId> list = new List<ADObjectId>(specificUsers.Length);
					for (int i = 0; i < specificUsers.Length; i++)
					{
						Guid guid;
						if (Guid.TryParse(specificUsers[i], out guid))
						{
							list.Add(new ADObjectId(guid));
						}
					}
					if (list.Count > 0)
					{
						array = adRecipientSession.ReadMultiple(list.ToArray(), new PropertyDefinition[]
						{
							ADObjectSchema.DistinguishedName
						});
					}
				}
				if (array != null && array.Length > 0)
				{
					multiValuedProperty = new MultiValuedProperty<ADObjectId>();
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].Data != null)
						{
							multiValuedProperty.Add(array[j].Data.Id);
						}
					}
				}
			}
			return multiValuedProperty;
		}

		internal static string[] ConvertPresentationFormatToWireUserList(IRecipientSession adRecipientSession, MultiValuedProperty<ADObjectId> userList)
		{
			if (userList != null && userList.Count > 0)
			{
				Result<ADRawEntry>[] array = adRecipientSession.ReadMultiple(userList.ToArray(), new PropertyDefinition[]
				{
					ADObjectSchema.Guid,
					ADRecipientSchema.ExternalDirectoryObjectId
				});
				if (array != null && array.Length > 0)
				{
					string[] array2 = new string[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = InstalledExtensionTable.GetWireUserId(array[i].Data, array[i].Data.Id);
					}
					return array2;
				}
			}
			return null;
		}

		internal override ExtensionData GetExtensionDataForInstall(IRecipientSession adRecipientSession)
		{
			ExtensionData extensionDataForInstall = base.GetExtensionDataForInstall(adRecipientSession);
			extensionDataForInstall.IsMandatory = (base.DefaultStateForUser == Microsoft.Exchange.Management.Extension.DefaultStateForUser.AlwaysEnabled);
			extensionDataForInstall.IsEnabledByDefault = (base.DefaultStateForUser == Microsoft.Exchange.Management.Extension.DefaultStateForUser.Enabled);
			extensionDataForInstall.ProvidedTo = this.ProvidedTo;
			extensionDataForInstall.SpecificUsers = OrgApp.ConvertPresentationFormatToWireUserList(adRecipientSession, this.UserList);
			return extensionDataForInstall;
		}

		internal static RecipientTypeDetails[] SupportedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.UserMailbox,
			RecipientTypeDetails.LinkedMailbox
		};

		private static OWAOrgExtensionSchema schema = ObjectSchema.GetInstance<OWAOrgExtensionSchema>();
	}
}
