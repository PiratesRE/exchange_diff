using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PropertyExistenceTracker : SmartPropertyDefinition
	{
		public PropertyExistenceTracker(NativeStorePropertyDefinition trackedProperty) : base(trackedProperty.Name + "_Tracker", trackedProperty.Type, PropertyFlags.None, new PropertyDefinitionConstraint[0], new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.PropertyExistenceTracker, PropertyDependencyType.NeedForRead)
		})
		{
			this.bitFlag = PropertyExistenceTracker.GetBitFlag(trackedProperty);
			if (this.bitFlag == -1L)
			{
				throw new ArgumentException(string.Format("{0} not in Tracked property list", trackedProperty));
			}
		}

		public static long GetBitFlag(PropertyDefinition property)
		{
			for (int i = 0; i < PropertyExistenceTracker.TrackedProperties.Length; i++)
			{
				if (property == PropertyExistenceTracker.TrackedProperties[i])
				{
					return 1L << (i & 31);
				}
			}
			return -1L;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			long valueOrDefault = propertyBag.GetValueOrDefault<long>(InternalSchema.PropertyExistenceTracker);
			return (valueOrDefault & this.bitFlag) == this.bitFlag;
		}

		public static readonly NativeStorePropertyDefinition[] TrackedProperties = new NativeStorePropertyDefinition[]
		{
			InternalSchema.XmlExtractedAddresses,
			InternalSchema.XmlExtractedContacts,
			InternalSchema.XmlExtractedEmails,
			InternalSchema.XmlExtractedKeywords,
			InternalSchema.XmlExtractedMeetings,
			InternalSchema.XmlExtractedPhones,
			InternalSchema.XmlExtractedTasks,
			InternalSchema.XmlExtractedUrls,
			InternalSchema.MapiReplyToNames,
			InternalSchema.MapiReplyToBlob
		};

		private readonly long bitFlag;
	}
}
