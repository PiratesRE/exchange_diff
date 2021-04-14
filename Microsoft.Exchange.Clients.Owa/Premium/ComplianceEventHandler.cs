using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("Compliance")]
	internal sealed class ComplianceEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ComplianceEventHandler));
		}

		[OwaEvent("gtCMIB")]
		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEventParameter("cguid", typeof(string))]
		public void GetComplianceInfobar()
		{
			string g = (string)base.GetParameter("cguid");
			Guid empty = Guid.Empty;
			if (GuidHelper.TryParseGuid(g, out empty))
			{
				InfobarMessage compliance = InfobarMessageBuilder.GetCompliance(base.UserContext, empty);
				if (compliance != null)
				{
					Infobar.RenderMessage(this.Writer, compliance, base.UserContext);
				}
			}
		}

		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEventParameter("id", typeof(StoreObjectId), false, true)]
		[OwaEvent("gtCM")]
		public void GetComplianceContextMenu()
		{
			StoreObjectId storeObjectId = base.GetParameter("id") as StoreObjectId;
			string id = "0";
			if (storeObjectId != null)
			{
				using (Item item = Utilities.GetItem<Item>(base.UserContext, storeObjectId, ComplianceEventHandler.prefetchProperties))
				{
					if (Utilities.IrmDecryptIfRestricted(item, base.UserContext, true))
					{
						RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)item;
						id = "1";
						if (rightsManagedMessageItem.Restriction != null && base.UserContext.ComplianceReader.RmsTemplateReader.LookupRmsTemplate(rightsManagedMessageItem.Restriction.Id) != null)
						{
							id = rightsManagedMessageItem.Restriction.Id.ToString();
						}
					}
					else
					{
						bool property = ItemUtility.GetProperty<bool>(item, ItemSchema.IsClassified, false);
						if (property)
						{
							string property2 = ItemUtility.GetProperty<string>(item, ItemSchema.ClassificationGuid, string.Empty);
							id = "1";
							Guid empty = Guid.Empty;
							if (GuidHelper.TryParseGuid(property2, out empty) && base.UserContext.ComplianceReader.MessageClassificationReader.LookupMessageClassification(empty, base.UserContext.UserCulture) != null)
							{
								id = empty.ToString();
							}
						}
					}
				}
			}
			ComplianceContextMenu complianceContextMenu = new ComplianceContextMenu(base.UserContext, id);
			complianceContextMenu.Render(this.Writer);
		}

		public const string EventNamespace = "Compliance";

		public const string Id = "id";

		public const string GuidArrayMap = "cguid";

		public const string MethodGetComplianceMenu = "gtCM";

		public const string MethodGetComplianceInfobar = "gtCMIB";

		private const string UnknownComplianceId = "1";

		private static readonly StorePropertyDefinition[] prefetchProperties = new StorePropertyDefinition[]
		{
			ItemSchema.IsClassified,
			ItemSchema.Classification,
			ItemSchema.ClassificationDescription,
			ItemSchema.ClassificationGuid
		};
	}
}
