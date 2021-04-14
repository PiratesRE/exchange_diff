using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedUserConfigurationSchema : IAggregatedUserConfigurationSchema
	{
		public AggregatedUserConfigurationSchema()
		{
			this.all = new List<AggregatedUserConfigurationDescriptor>();
			this.owaUserConfiguration = new AggregatedUserConfigurationDescriptor("Aggregated.OwaUserConfiguration", new UserConfigurationDescriptor[]
			{
				UserConfigurationDescriptor.CreateMailboxDescriptor("OWA.UserOptions", UserConfigurationTypes.Dictionary),
				UserConfigurationDescriptor.CreateDefaultFolderDescriptor("WorkHours", UserConfigurationTypes.XML, DefaultFolderType.Calendar),
				UserConfigurationDescriptor.CreateDefaultFolderDescriptor("Calendar", UserConfigurationTypes.Dictionary, DefaultFolderType.Calendar),
				UserConfigurationDescriptor.CreateMailboxDescriptor("OWA.ViewStateConfiguration", UserConfigurationTypes.Dictionary),
				UserConfigurationDescriptor.CreateDefaultFolderDescriptor("CategoryList", UserConfigurationTypes.XML, DefaultFolderType.Calendar),
				UserConfigurationDescriptor.CreateDefaultFolderDescriptor("MRM", UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, DefaultFolderType.Inbox),
				UserConfigurationDescriptor.CreateDefaultFolderDescriptor("Inference.Settings", UserConfigurationTypes.Dictionary, DefaultFolderType.Inbox),
				UserConfigurationDescriptor.CreateMailboxDescriptor("AggregatedAccountList", UserConfigurationTypes.XML)
			});
			this.all.Add(this.owaUserConfiguration);
		}

		public static AggregatedUserConfigurationSchema Instance
		{
			get
			{
				if (AggregatedUserConfigurationSchema.instance == null)
				{
					lock (AggregatedUserConfigurationSchema.instanceLock)
					{
						if (AggregatedUserConfigurationSchema.instance == null)
						{
							AggregatedUserConfigurationSchema.instance = new AggregatedUserConfigurationSchema();
						}
					}
				}
				return AggregatedUserConfigurationSchema.instance;
			}
		}

		public IEnumerable<AggregatedUserConfigurationDescriptor> All
		{
			get
			{
				return this.all;
			}
		}

		public AggregatedUserConfigurationDescriptor OwaUserConfiguration
		{
			get
			{
				return this.owaUserConfiguration;
			}
		}

		private static object instanceLock = new object();

		private static AggregatedUserConfigurationSchema instance;

		private List<AggregatedUserConfigurationDescriptor> all;

		private AggregatedUserConfigurationDescriptor owaUserConfiguration;
	}
}
