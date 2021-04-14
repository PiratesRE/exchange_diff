using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UnifiedPolicyCommonSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition ObjectIdProperty = new HygienePropertyDefinition("ObjectId", typeof(Guid));

		internal static readonly HygienePropertyDefinition DataSourceProperty = new HygienePropertyDefinition("DataSource", typeof(string));

		internal static readonly HygienePropertyDefinition HashBucketProperty = DalHelper.HashBucketProp;

		internal static readonly HygienePropertyDefinition PhysicalInstanceKeyProp = DalHelper.PhysicalInstanceKeyProp;

		internal static readonly HygienePropertyDefinition FssCopyIdProp = DalHelper.FssCopyIdProp;

		internal static readonly HygienePropertyDefinition IntValue1Prop = new HygienePropertyDefinition("IntValue1", typeof(int?));

		internal static readonly HygienePropertyDefinition IntValue2Prop = new HygienePropertyDefinition("IntValue2", typeof(int?));

		internal static readonly HygienePropertyDefinition IntValue3Prop = new HygienePropertyDefinition("IntValue3", typeof(int?));

		internal static readonly HygienePropertyDefinition LongValue1Prop = new HygienePropertyDefinition("LongValue1", typeof(long?));

		internal static readonly HygienePropertyDefinition LongValue2Prop = new HygienePropertyDefinition("LongValue2", typeof(long?));

		internal static readonly HygienePropertyDefinition GuidValue1Prop = new HygienePropertyDefinition("GuidValue1", typeof(Guid?));

		internal static readonly HygienePropertyDefinition GuidValue2Prop = new HygienePropertyDefinition("GuidValue2", typeof(Guid?));

		internal static readonly HygienePropertyDefinition StringValue1Prop = new HygienePropertyDefinition("StringValue1", typeof(string));

		internal static readonly HygienePropertyDefinition StringValue2Prop = new HygienePropertyDefinition("StringValue2", typeof(string));

		internal static readonly HygienePropertyDefinition StringValue3Prop = new HygienePropertyDefinition("StringValue3", typeof(string));

		internal static readonly HygienePropertyDefinition StringValue4Prop = new HygienePropertyDefinition("StringValue4", typeof(string));

		internal static readonly HygienePropertyDefinition StringValue5Prop = new HygienePropertyDefinition("StringValue5", typeof(string));

		internal static readonly HygienePropertyDefinition ByteValue1Prop = new HygienePropertyDefinition("ByteValue1", typeof(byte[]));

		internal static readonly HygienePropertyDefinition ByteValue2Prop = new HygienePropertyDefinition("ByteValue2", typeof(byte[]));
	}
}
