using System;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyTraceSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = UnifiedPolicyCommonSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ObjectIdProperty = UnifiedPolicyCommonSchema.ObjectIdProperty;

		internal static readonly HygienePropertyDefinition DataSourceProperty = UnifiedPolicyCommonSchema.DataSourceProperty;

		internal static readonly HygienePropertyDefinition FileIdProperty = new HygienePropertyDefinition("FileId", typeof(Guid));

		internal static readonly HygienePropertyDefinition FileNameProperty = new HygienePropertyDefinition("FileName", typeof(string));

		internal static readonly HygienePropertyDefinition SizeProperty = new HygienePropertyDefinition("Size", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SiteIdProperty = new HygienePropertyDefinition("SiteId", typeof(Guid));

		internal static readonly HygienePropertyDefinition FileUrlProperty = new HygienePropertyDefinition("FileUrl", typeof(string));

		internal static readonly HygienePropertyDefinition OwnerProperty = new HygienePropertyDefinition("Owner", typeof(string));

		internal static readonly HygienePropertyDefinition IsViewableByExternalUsersProperty = new HygienePropertyDefinition("IsViewableByExternalUsers", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastModifiedByProperty = new HygienePropertyDefinition("LastModifiedBy", typeof(string));

		internal static readonly HygienePropertyDefinition CreateTimeProperty = new HygienePropertyDefinition("CreateTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastModifiedTimeProperty = new HygienePropertyDefinition("LastModifiedTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PolicyMatchTimeProperty = new HygienePropertyDefinition("PolicyMatchTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
