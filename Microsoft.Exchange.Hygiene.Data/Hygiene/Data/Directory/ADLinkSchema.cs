using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADLinkSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition SourceIdProperty = new HygienePropertyDefinition("SourceId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition DestinationIdProperty = new HygienePropertyDefinition("DestinationId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition SourceTypeProperty = new HygienePropertyDefinition("SourceType", typeof(DirectoryObjectClass));

		public static readonly HygienePropertyDefinition DestinationTypeProperty = new HygienePropertyDefinition("DestinationType", typeof(DirectoryObjectClass));

		public static readonly HygienePropertyDefinition LinkTypeProperty = new HygienePropertyDefinition("LinkType", typeof(LinkType));
	}
}
