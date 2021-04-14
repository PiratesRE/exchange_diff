using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal class ComplianceJobSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition DisplayName = new SimpleProviderPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2007, typeof(ComplianceJobId), PropertyDefinitionFlags.None, new ComplianceJobId(), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ObjectState = new SimpleProviderPropertyDefinition("ObjectState", ExchangeObjectVersion.Exchange2007, typeof(ObjectState), PropertyDefinitionFlags.None, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExchangeVersion = new SimpleProviderPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2007, typeof(ExchangeObjectVersion), PropertyDefinitionFlags.PersistDefaultValue, ExchangeObjectVersion.Exchange2007, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CreatedTime = new SimpleProviderPropertyDefinition("CreatedTime", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LastModifiedTime = new SimpleProviderPropertyDefinition("LastModifiedTime", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobStartTime = new SimpleProviderPropertyDefinition("JobStartTime", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobEndTime = new SimpleProviderPropertyDefinition("JobEndTime", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Description = new SimpleProviderPropertyDefinition("Description", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobType = new SimpleProviderPropertyDefinition("JobType", ExchangeObjectVersion.Exchange2007, typeof(ComplianceJobType), PropertyDefinitionFlags.None, ComplianceJobType.UnknownType, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CreatedBy = new SimpleProviderPropertyDefinition("CreatedBy", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RunBy = new SimpleProviderPropertyDefinition("RunBy", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobObjectVersion = new SimpleProviderPropertyDefinition("JobObjectVersion", ExchangeObjectVersion.Exchange2007, typeof(ComplianceJobObjectVersion), PropertyDefinitionFlags.None, ComplianceJobObjectVersion.VersionUnknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TenantId = new SimpleProviderPropertyDefinition("TenantId", ExchangeObjectVersion.Exchange2007, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NumBindings = new SimpleProviderPropertyDefinition("NumBindings", ExchangeObjectVersion.Exchange2007, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NumBindingsFailed = new SimpleProviderPropertyDefinition("NumBindingsFailed", ExchangeObjectVersion.Exchange2007, typeof(int?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobStatus = new SimpleProviderPropertyDefinition("JobStatus", ExchangeObjectVersion.Exchange2007, typeof(ComplianceJobStatus), PropertyDefinitionFlags.None, ComplianceJobStatus.NotStarted, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExchangeBindings = new SimpleProviderPropertyDefinition("ExchangeBindings", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PublicFolderBindings = new SimpleProviderPropertyDefinition("PublicFolderBindings", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition SharePointBindings = new SimpleProviderPropertyDefinition("SharePointBindings", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AllExchangeBindings = new SimpleProviderPropertyDefinition("AllExchangeBindings", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AllSharePointBindings = new SimpleProviderPropertyDefinition("AllSharePointBindings", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AllPublicFolderBindings = new SimpleProviderPropertyDefinition("AllPublicFolderBindings", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Resume = new SimpleProviderPropertyDefinition("Resume", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition JobRunId = new SimpleProviderPropertyDefinition("JobRunId", ExchangeObjectVersion.Exchange2007, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TenantInfo = new SimpleProviderPropertyDefinition("TenantInfo", ExchangeObjectVersion.Exchange2007, typeof(byte[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
