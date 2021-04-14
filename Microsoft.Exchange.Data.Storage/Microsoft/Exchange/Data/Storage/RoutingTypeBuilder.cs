using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RoutingTypeBuilder : ADConsumer
	{
		private RoutingTypeBuilder(ITopologyConfigurationSession configSession, OrganizationCache cache) : base(configSession.GetRoutingGroupId(), configSession, cache)
		{
			ADNotificationAdapter.RegisterChangeNotification<MailGateway>(base.ConfigSession.GetOrgContainerId(), new ADNotificationCallback(base.OnChange), null);
		}

		public static RoutingTypeBuilder Instance
		{
			get
			{
				if (RoutingTypeBuilder.instance != null)
				{
					return RoutingTypeBuilder.instance;
				}
				RoutingTypeBuilder result;
				lock (RoutingTypeBuilder.lockInstance)
				{
					if (RoutingTypeBuilder.instance == null)
					{
						DirectoryHelper.DoAdCallAndTranslateExceptions(delegate
						{
							RoutingTypeBuilder.instance = new RoutingTypeBuilder(ADConsumer.ADSystemConfigurationSessionInstance, new OrganizationCache());
						}, "get_Instance");
					}
					result = RoutingTypeBuilder.instance;
				}
				return result;
			}
		}

		public string[] GetRoutingTypes()
		{
			return base.Cache.Get<string[]>(this, RoutingTypeBuilder.RoutingTypes, new Func<string[]>(this.QueryRoutingTypes));
		}

		private string[] QueryRoutingTypes()
		{
			return (from type in (from addressSpace in base.ConfigSession.FindPaged<MailGateway>(base.ConfigSession.GetOrgContainerId().GetChildId("Administrative Groups"), QueryScope.SubTree, null, null, ADGenericPagedReader<Microsoft.Exchange.Data.Directory.SystemConfiguration.MailGateway>.DefaultPageSize).SelectMany((MailGateway mailGateway) => mailGateway.AddressSpaces)
			select Participant.NormalizeRoutingType(addressSpace.Type)).Concat(RoutingTypeBuilder.DefaultAddressTypes).Distinct<string>()
			orderby type
			select type).ToArray<string>();
		}

		private const string MailGateway = "mailGateway";

		private static readonly object lockInstance = new object();

		private static readonly string RoutingTypes = "RoutingTypes";

		private static readonly string[] DefaultAddressTypes = new string[]
		{
			"EX",
			"SMTP",
			"X400"
		};

		private static RoutingTypeBuilder instance = null;
	}
}
