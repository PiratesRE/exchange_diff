using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class WellKnownProperties
	{
		public static ExtendedPropertyUri Hidden { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x10f4",
			PropertyType = MapiPropertyType.Boolean
		};

		public static ExtendedPropertyUri VoiceMessageAttachmentOrder { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x6805",
			PropertyType = MapiPropertyType.String
		};

		public static ExtendedPropertyUri PstnCallbackTelephoneNumber { get; private set; } = new ExtendedPropertyUri
		{
			PropertyName = "PstnCallbackTelephoneNumber",
			DistinguishedPropertySetId = DistinguishedPropertySet.UnifiedMessaging,
			PropertyType = MapiPropertyType.String
		};

		public static ExtendedPropertyUri VoiceMessageDuration { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x6801",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri IsClassified { get; private set; } = new ExtendedPropertyUri
		{
			PropertyId = 34229,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.Boolean
		};

		public static ExtendedPropertyUri ClassificationGuid { get; private set; } = new ExtendedPropertyUri
		{
			PropertyId = 34232,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.String
		};

		public static ExtendedPropertyUri Classification { get; private set; } = new ExtendedPropertyUri
		{
			PropertyId = 34230,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.String
		};

		public static ExtendedPropertyUri ClassificationDescription { get; private set; } = new ExtendedPropertyUri
		{
			PropertyId = 34231,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.String
		};

		public static ExtendedPropertyUri ClassificationKeep { get; private set; } = new ExtendedPropertyUri
		{
			PropertyId = 34234,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.Boolean
		};

		public static ExtendedPropertyUri SharingInstanceGuid { get; private set; } = new ExtendedPropertyUri
		{
			PropertyId = 35356,
			DistinguishedPropertySetId = DistinguishedPropertySet.Sharing,
			PropertyType = MapiPropertyType.CLSID
		};

		public static ExtendedPropertyUri MessageBccMe { get; private set; } = new ExtendedPropertyUri
		{
			PropertyName = "MessageBccMe",
			PropertySetId = "41F28F13-83F4-4114-A584-EEDB5A6B0BFF",
			PropertyType = MapiPropertyType.Boolean
		};

		public static ExtendedPropertyUri NormalizedSubject { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0xe1d",
			PropertyType = MapiPropertyType.String
		};

		public static ExtendedPropertyUri RetentionFlags { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x301d",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri RetentionPeriod { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x301a",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri ArchivePeriod { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x301e",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri NativeBodyInfo { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x1016",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri InternetMessageHeaders { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x7d",
			PropertyType = MapiPropertyType.String
		};

		public static ExtendedPropertyUri FlagStatus { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x1090",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri LastVerbExecuted { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x1081",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri LastVerbExecutionTime { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x1082",
			PropertyType = MapiPropertyType.SystemTime
		};

		public static ExtendedPropertyUri DocumentId { get; private set; } = new ExtendedPropertyUri
		{
			PropertyTag = "0x6815",
			PropertyType = MapiPropertyType.Integer
		};

		public static ExtendedPropertyUri WorkingSetSourcePartitionInternal { get; private set; } = new ExtendedPropertyUri
		{
			PropertyName = "WorkingSetSourcePartitionInternal",
			PropertySetId = "95A4668D-CFBE-4D15-B4AE-3E61B9EF078B",
			PropertyType = MapiPropertyType.String
		};

		public static PropertyUri Location
		{
			get
			{
				return WellKnownProperties.locationPropertyUri;
			}
		}

		public const string MessagingNamespaceGuidString = "41F28F13-83F4-4114-A584-EEDB5A6B0BFF";

		public const string WorkingSetNamespaceGuidString = "95A4668D-CFBE-4D15-B4AE-3E61B9EF078B";

		private static readonly PropertyUri locationPropertyUri = new PropertyUri(PropertyUriEnum.EnhancedLocation);
	}
}
