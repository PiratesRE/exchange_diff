using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Name = RequestJobSchema.Name;

		public static readonly SimpleProviderPropertyDefinition ExchangeGuid = RequestJobSchema.ExchangeGuid;

		public static readonly SimpleProviderPropertyDefinition Flags = RequestJobSchema.Flags;

		public static readonly SimpleProviderPropertyDefinition RemoteHostName = RequestJobSchema.RemoteHostName;

		public static readonly SimpleProviderPropertyDefinition BatchName = RequestJobSchema.BatchName;

		public static readonly SimpleProviderPropertyDefinition Status = RequestJobSchema.Status;

		public static readonly SimpleProviderPropertyDefinition SourceDatabase = RequestJobSchema.SourceDatabase;

		public static readonly SimpleProviderPropertyDefinition TargetDatabase = RequestJobSchema.TargetDatabase;

		public static readonly SimpleProviderPropertyDefinition FilePath = RequestJobSchema.FilePath;

		public static readonly SimpleProviderPropertyDefinition SourceMailbox = RequestJobSchema.SourceUserId;

		public static readonly SimpleProviderPropertyDefinition TargetMailbox = RequestJobSchema.TargetUserId;

		public static readonly SimpleProviderPropertyDefinition RequestGuid = RequestJobSchema.RequestGuid;

		public static readonly SimpleProviderPropertyDefinition RequestQueue = RequestJobSchema.RequestQueue;

		public static readonly SimpleProviderPropertyDefinition OrganizationId = new SimpleProviderPropertyDefinition("OrganizationId", ExchangeObjectVersion.Exchange2010, typeof(OrganizationId), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenChanged = new SimpleProviderPropertyDefinition("WhenChanged", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenCreated = new SimpleProviderPropertyDefinition("WhenCreated", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenChangedUTC = new SimpleProviderPropertyDefinition("WhenChangedUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition WhenCreatedUTC = new SimpleProviderPropertyDefinition("WhenCreatedUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
