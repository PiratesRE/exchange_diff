using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OptionsDataBuilder : ADConsumer
	{
		private OptionsDataBuilder(ITopologyConfigurationSession configSession, OrganizationCache cache) : base(new ADObjectId("CN=Addressing," + configSession.GetOrgContainerId().ToDNString()), configSession, cache)
		{
			ADNotificationAdapter.RegisterChangeNotification<AddressTemplate>(base.ConfigSession.GetOrgContainerId(), new ADNotificationCallback(base.OnChange), null);
		}

		internal static OptionsDataBuilder Instance
		{
			get
			{
				if (OptionsDataBuilder.instance != null)
				{
					return OptionsDataBuilder.instance;
				}
				OptionsDataBuilder result;
				lock (OptionsDataBuilder.lockInstance)
				{
					if (OptionsDataBuilder.instance == null)
					{
						OptionsDataBuilder.instance = new OptionsDataBuilder(ADConsumer.ADSystemConfigurationSessionInstance, new OrganizationCache());
					}
					result = OptionsDataBuilder.instance;
				}
				return result;
			}
		}

		internal RoutingTypeOptionsData GetOptionsData(MailboxSession session, string routingType)
		{
			string attribute = OptionsDataBuilder.MakeCannonicRoutingTypeKey(routingType, session.PreferedCulture.LCID);
			return base.Cache.Get<RoutingTypeOptionsData>(this, attribute, () => this.QueryOptionsData(session, routingType));
		}

		private static string MakeCannonicRoutingTypeKey(string routingType, int localeId)
		{
			return string.Format("{0}:{1}", routingType.ToUpperInvariant(), localeId);
		}

		private RoutingTypeOptionsData QueryOptionsData(MailboxSession session, string routingType)
		{
			Util.ThrowOnNullArgument(routingType, "routingType");
			int num = routingType.IndexOf('=');
			if (num >= 0)
			{
				routingType = routingType.Substring(0, num);
			}
			if (routingType == string.Empty)
			{
				throw new ArgumentException("routingType");
			}
			int lcid = session.PreferedCulture.LCID;
			ADObjectId descendantId;
			if (string.CompareOrdinal(routingType, "EX") == 0)
			{
				descendantId = base.Id.GetDescendantId(new ADObjectId(string.Format("CN=Exchange,CN={0:X},CN={1}", lcid, "Display-Templates")));
			}
			else
			{
				descendantId = base.Id.GetDescendantId(new ADObjectId(string.Format("CN={0},CN={1:X},CN={2}", routingType, lcid, "Address-Templates")));
			}
			AddressTemplate addressTemplate = base.ConfigSession.Read<AddressTemplate>(descendantId);
			RoutingTypeOptionsData result;
			if (addressTemplate != null)
			{
				ADRawEntry adrawEntry = base.ConfigSession.ReadADRawEntry(addressTemplate.Id, new ADPropertyDefinition[]
				{
					AddressTemplateSchema.PerMsgDialogDisplayTable,
					AddressTemplateSchema.PerRecipDialogDisplayTable,
					DetailsTemplateSchema.HelpFileName,
					DetailsTemplateSchema.HelpData32
				});
				byte[] messageData = adrawEntry[AddressTemplateSchema.PerMsgDialogDisplayTable] as byte[];
				byte[] recipientData = adrawEntry[AddressTemplateSchema.PerRecipDialogDisplayTable] as byte[];
				byte[] helpFileName = adrawEntry[DetailsTemplateSchema.HelpFileName] as byte[];
				byte[] helpFileData = adrawEntry[DetailsTemplateSchema.HelpData32] as byte[];
				result = new RoutingTypeOptionsData(messageData, recipientData, helpFileName, helpFileData);
			}
			else
			{
				result = default(RoutingTypeOptionsData);
			}
			return result;
		}

		private const string DisplayTemplates = "Display-Templates";

		private const string AddressTemplates = "Address-Templates";

		private static readonly object lockInstance = new object();

		private static OptionsDataBuilder instance = null;
	}
}
