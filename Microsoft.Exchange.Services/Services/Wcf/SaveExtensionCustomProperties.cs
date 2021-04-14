using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SaveExtensionCustomProperties : ServiceCommand<SaveExtensionCustomPropertiesResponse>
	{
		public SaveExtensionCustomProperties(CallContext callContext, string extensionId, string itemId, string customProperties) : base(callContext)
		{
			this.extensionId = extensionId;
			this.itemId = itemId;
			this.customProperties = customProperties;
		}

		protected override SaveExtensionCustomPropertiesResponse InternalExecute()
		{
			Item item = null;
			try
			{
				MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
				IdHeaderInformation idHeaderInformation = IdConverter.ConvertFromConcatenatedId(this.itemId, BasicTypes.Item, null, false);
				StoreObjectId storeId = idHeaderInformation.ToStoreObjectId();
				string text = "cecp-" + this.extensionId;
				GuidNamePropertyDefinition guidNamePropertyDefinition = GuidNamePropertyDefinition.CreateCustom(text, typeof(string), LoadExtensionCustomProperties.ClientExtensibilityGuid, text, PropertyFlags.None);
				item = Item.Bind(mailboxIdentityMailboxSession, storeId, new PropertyDefinition[]
				{
					guidNamePropertyDefinition,
					LoadExtensionCustomProperties.CustomPropertyNamesListDefinition
				});
				item.OpenAsReadWrite();
				item.SafeSetProperty(guidNamePropertyDefinition, this.customProperties);
				try
				{
					string valueOrDefault = item.GetValueOrDefault<string>(LoadExtensionCustomProperties.CustomPropertyNamesListDefinition, string.Empty);
					if (valueOrDefault.IndexOf(text, StringComparison.OrdinalIgnoreCase) == -1)
					{
						string text2 = valueOrDefault + text + ";";
						if (text2.Length < 16000)
						{
							item.SafeSetProperty(LoadExtensionCustomProperties.CustomPropertyNamesListDefinition, text2);
						}
					}
				}
				catch (StoragePermanentException)
				{
				}
				item.Save(SaveMode.ResolveConflicts);
			}
			catch (StorageTransientException)
			{
				return new SaveExtensionCustomPropertiesResponse(CoreResources.SaveExtensionCustomPropertiesFailed);
			}
			catch (StoragePermanentException)
			{
				return new SaveExtensionCustomPropertiesResponse(CoreResources.SaveExtensionCustomPropertiesFailed);
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
				}
			}
			return new SaveExtensionCustomPropertiesResponse();
		}

		private readonly string extensionId;

		private readonly string itemId;

		private readonly string customProperties;
	}
}
