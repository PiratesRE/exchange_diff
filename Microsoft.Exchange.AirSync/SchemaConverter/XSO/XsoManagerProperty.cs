using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoManagerProperty : XsoStringProperty
	{
		public XsoManagerProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public override string StringData
		{
			get
			{
				string text = base.XsoItem.TryGetProperty(base.PropertyDef) as string;
				if (text == null)
				{
					return null;
				}
				if (text.Trim().StartsWith("/o="))
				{
					string text2 = (string)XsoManagerProperty.GetObjectFromLegacyDN(text, ADRecipientSchema.DisplayName);
					if (!string.IsNullOrEmpty(text2))
					{
						text = text2;
					}
				}
				return text;
			}
		}

		private static object GetObjectFromLegacyDN(string legacyDN, PropertyDefinition propDef)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(Command.CurrentOrganizationId), 84, "GetObjectFromLegacyDN", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\SchemaConverter\\XSO\\XsoManagerProperty.cs");
			ADRecipient adrecipient = tenantOrRootOrgRecipientSession.FindByLegacyExchangeDN(legacyDN);
			Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, tenantOrRootOrgRecipientSession.LastUsedDc);
			if (adrecipient == null)
			{
				return null;
			}
			return adrecipient[propDef];
		}
	}
}
