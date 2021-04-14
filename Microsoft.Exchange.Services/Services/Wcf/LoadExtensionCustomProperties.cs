using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class LoadExtensionCustomProperties : ServiceCommand<LoadExtensionCustomPropertiesResponse>
	{
		public LoadExtensionCustomProperties(CallContext callContext, string extensionId, string itemId) : base(callContext)
		{
			this.extensionId = extensionId;
			this.itemId = itemId;
		}

		protected override LoadExtensionCustomPropertiesResponse InternalExecute()
		{
			string text = null;
			string customPropertyNames = null;
			Item item = null;
			try
			{
				MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
				IdHeaderInformation idHeaderInformation = IdConverter.ConvertFromConcatenatedId(this.itemId, BasicTypes.Item, null, false);
				StoreObjectId storeId = idHeaderInformation.ToStoreObjectId();
				string text2 = "cecp-" + this.extensionId;
				GuidNamePropertyDefinition guidNamePropertyDefinition = GuidNamePropertyDefinition.CreateCustom(text2, typeof(string), LoadExtensionCustomProperties.ClientExtensibilityGuid, text2, PropertyFlags.None);
				item = Item.Bind(mailboxIdentityMailboxSession, storeId, new PropertyDefinition[]
				{
					guidNamePropertyDefinition,
					LoadExtensionCustomProperties.CustomPropertyNamesListDefinition
				});
				text = item.GetValueOrDefault<string>(guidNamePropertyDefinition, "{}");
				try
				{
					customPropertyNames = item.GetValueOrDefault<string>(LoadExtensionCustomProperties.CustomPropertyNamesListDefinition, string.Empty);
				}
				catch (StoragePermanentException)
				{
				}
				if (text.Length > 2500)
				{
					return new LoadExtensionCustomPropertiesResponse(null, null, CoreResources.LoadExtensionCustomPropertiesFailed);
				}
			}
			catch (StorageTransientException)
			{
				return new LoadExtensionCustomPropertiesResponse(null, null, CoreResources.LoadExtensionCustomPropertiesFailed);
			}
			catch (StoragePermanentException)
			{
				return new LoadExtensionCustomPropertiesResponse(null, null, CoreResources.LoadExtensionCustomPropertiesFailed);
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
				}
			}
			return new LoadExtensionCustomPropertiesResponse(text, customPropertyNames, null);
		}

		internal const string CustomPropertyNamesPropertyName = "cecp-propertyNames";

		internal const int CustomPropertyNamesCharacterLimit = 16000;

		internal const string CustomPropertyNamesDelimiter = ";";

		internal const string NamePrefix = "cecp-";

		internal static readonly Guid ClientExtensibilityGuid = ExtendedPropertyUri.PSETIDPublicStrings;

		internal static readonly GuidNamePropertyDefinition CustomPropertyNamesListDefinition = GuidNamePropertyDefinition.CreateCustom("cecp-propertyNames", typeof(string), LoadExtensionCustomProperties.ClientExtensibilityGuid, "cecp-propertyNames", PropertyFlags.None);

		private readonly string extensionId;

		private readonly string itemId;
	}
}
