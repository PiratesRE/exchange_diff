using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MRSRequestSchema : UserConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition RequestGuidRaw = new SimplePropertyDefinition("RequestGuidRaw", ExchangeObjectVersion.Exchange2012, typeof(byte[]), PropertyDefinitionFlags.None, null, null);

		public static readonly SimplePropertyDefinition RequestGuid = new SimplePropertyDefinition("RequestGuid", ExchangeObjectVersion.Exchange2012, typeof(Guid), PropertyDefinitionFlags.Calculated, Guid.Empty, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MRSRequestSchema.RequestGuidRaw
		}, null, delegate(IPropertyBag bag)
		{
			byte[] array = bag[MRSRequestSchema.RequestGuidRaw] as byte[];
			if (array == null)
			{
				throw new FormatException("RequestGuidRaw is null");
			}
			return ValueConvertor.ConvertValueFromBinary(array, typeof(Guid), null);
		}, delegate(object value, IPropertyBag bag)
		{
			bag[MRSRequestSchema.RequestGuidRaw] = ValueConvertor.ConvertValueToBinary(value, null);
		});

		public static readonly SimplePropertyDefinition Name = new SimplePropertyDefinition("Name", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new MandatoryStringLengthConstraint(1, 256)
		});

		public static readonly SimplePropertyDefinition Status = new SimplePropertyDefinition("Status", ExchangeObjectVersion.Exchange2012, typeof(RequestStatus), PropertyDefinitionFlags.None, RequestStatus.None, RequestStatus.None);

		public static readonly SimplePropertyDefinition Flags = new SimplePropertyDefinition("Flags", ExchangeObjectVersion.Exchange2012, typeof(RequestFlags), PropertyDefinitionFlags.None, RequestFlags.None, RequestFlags.None);

		public static readonly SimplePropertyDefinition RemoteHostName = new SimplePropertyDefinition("RemoteHostName", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty);

		public static readonly SimplePropertyDefinition BatchName = new SimplePropertyDefinition("BatchName", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty);

		public static readonly SimplePropertyDefinition SourceMDB = new SimplePropertyDefinition("SourceMDB", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), PropertyDefinitionFlags.None, null, null);

		public static readonly SimplePropertyDefinition TargetMDB = new SimplePropertyDefinition("TargetMDB", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), PropertyDefinitionFlags.None, null, null);

		public static readonly SimplePropertyDefinition StorageMDB = new SimplePropertyDefinition("StorageMDB", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), PropertyDefinitionFlags.None, null, null);

		public static readonly SimplePropertyDefinition FilePath = new SimplePropertyDefinition("FilePath", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty);

		public static readonly SimplePropertyDefinition Type = new SimplePropertyDefinition("Type", ExchangeObjectVersion.Exchange2012, typeof(MRSRequestType), PropertyDefinitionFlags.None, null, null);

		public static readonly SimplePropertyDefinition TargetUserId = new SimplePropertyDefinition("TargetUserId", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), PropertyDefinitionFlags.None, null, null);

		public static readonly SimplePropertyDefinition SourceUserId = new SimplePropertyDefinition("SourceUserId", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), PropertyDefinitionFlags.None, null, null);

		public static readonly SimplePropertyDefinition WhenCreatedRaw = new SimplePropertyDefinition("WhenCreatedRaw", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty, string.Empty);

		public static readonly SimplePropertyDefinition WhenChangedRaw = new SimplePropertyDefinition("WhenChangedRaw", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReadOnly, string.Empty, string.Empty);

		public static readonly SimplePropertyDefinition WhenCreated = new SimplePropertyDefinition("WhenCreated", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.WhenCreatedRaw
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ADObject.WhenCreatedGetter), null);

		public static readonly SimplePropertyDefinition WhenCreatedUTC = new SimplePropertyDefinition("WhenCreatedUTC", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.WhenCreatedRaw
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ADObject.WhenCreatedUTCGetter), null);

		public static readonly SimplePropertyDefinition WhenChanged = new SimplePropertyDefinition("WhenChanged", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.WhenChangedRaw
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ADObject.WhenChangedGetter), null);

		public static readonly SimplePropertyDefinition WhenChangedUTC = new SimplePropertyDefinition("WhenChangedUTC", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.WhenChangedRaw
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(ADObject.WhenChangedUTCGetter), null);

		public static readonly SimplePropertyDefinition[] PersistedProperties = new SimplePropertyDefinition[]
		{
			MRSRequestSchema.RequestGuidRaw,
			MRSRequestSchema.Name,
			MRSRequestSchema.Status,
			MRSRequestSchema.Flags,
			MRSRequestSchema.BatchName,
			MRSRequestSchema.SourceMDB,
			MRSRequestSchema.TargetMDB,
			MRSRequestSchema.StorageMDB,
			MRSRequestSchema.FilePath,
			MRSRequestSchema.Type,
			MRSRequestSchema.TargetUserId,
			MRSRequestSchema.SourceUserId,
			MRSRequestSchema.WhenCreatedRaw,
			MRSRequestSchema.WhenChangedRaw
		};
	}
}
