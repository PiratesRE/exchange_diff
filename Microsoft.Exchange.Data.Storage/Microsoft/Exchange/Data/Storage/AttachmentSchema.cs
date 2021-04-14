using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentSchema : Schema
	{
		public new static AttachmentSchema Instance
		{
			get
			{
				if (AttachmentSchema.instance == null)
				{
					AttachmentSchema.instance = new AttachmentSchema();
				}
				return AttachmentSchema.instance;
			}
		}

		internal virtual void CoreObjectUpdate(CoreAttachment coreAttachment)
		{
		}

		[Autoload]
		public static readonly StorePropertyDefinition IsInline = InternalSchema.AttachmentIsInline;

		[Autoload]
		internal static readonly StorePropertyDefinition AppointmentExceptionEndTime = InternalSchema.AppointmentExceptionEndTime;

		[Autoload]
		internal static readonly StorePropertyDefinition HasDlpDetectedClassifications = InternalSchema.HasDlpDetectedClassifications;

		[Autoload]
		internal static readonly StorePropertyDefinition AppointmentExceptionStartTime = InternalSchema.AppointmentExceptionStartTime;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachCalendarFlags = InternalSchema.AttachCalendarFlags;

		[Autoload]
		public static readonly StorePropertyDefinition AttachCalendarHidden = InternalSchema.AttachCalendarHidden;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachCalendarLinkId = InternalSchema.AttachCalendarLinkId;

		[Autoload]
		internal static readonly StorePropertyDefinition FailedInboundICalAsAttachment = InternalSchema.FailedInboundICalAsAttachment;

		[Autoload]
		public static readonly StorePropertyDefinition AttachExtension = InternalSchema.AttachExtension;

		[Autoload]
		public static readonly StorePropertyDefinition AttachFileName = InternalSchema.AttachFileName;

		public static readonly StorePropertyDefinition AttachAdditionalInfo = InternalSchema.AttachAdditionalInfo;

		[Autoload]
		public static readonly StorePropertyDefinition AttachLongFileName = InternalSchema.AttachLongFileName;

		[Autoload]
		public static readonly StorePropertyDefinition AttachLongPathName = InternalSchema.AttachLongPathName;

		[Autoload]
		public static readonly StorePropertyDefinition IsContactPhoto = InternalSchema.IsContactPhoto;

		[Autoload]
		public static readonly StorePropertyDefinition AttachMethod = InternalSchema.AttachMethod;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachMhtmlFlags = InternalSchema.AttachMhtmlFlags;

		[Autoload]
		public static readonly StorePropertyDefinition AttachNum = InternalSchema.AttachNum;

		[Autoload]
		public static readonly StorePropertyDefinition CreationTime = InternalSchema.CreationTime;

		[DetectCodepage]
		public static readonly StorePropertyDefinition DisplayName = InternalSchema.DisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition LastModifiedTime = InternalSchema.LastModifiedTime;

		[Autoload]
		internal static readonly StorePropertyDefinition OriginalMimeReadTime = InternalSchema.OriginalMimeReadTime;

		[Autoload]
		internal static readonly StorePropertyDefinition RecordKey = InternalSchema.RecordKey;

		[Autoload]
		public static readonly StorePropertyDefinition AttachContentBase = InternalSchema.AttachContentBase;

		[Autoload]
		public static readonly StorePropertyDefinition AttachContentId = InternalSchema.AttachContentId;

		[Autoload]
		public static readonly StorePropertyDefinition AttachContentLocation = InternalSchema.AttachContentLocation;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachEncoding = InternalSchema.AttachEncoding;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachSize = InternalSchema.AttachSize;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachInConflict = InternalSchema.AttachInConflict;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachMimeTag = InternalSchema.AttachMimeTag;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachmentMacInfo = InternalSchema.AttachmentMacInfo;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachmentMacContentType = InternalSchema.AttachmentMacContentType;

		[Autoload]
		internal static readonly StorePropertyDefinition RenderingPosition = InternalSchema.RenderingPosition;

		[Autoload]
		public static readonly StorePropertyDefinition AttachDataBin = InternalSchema.AttachDataBin;

		[Autoload]
		public static readonly PropertyDefinition TextAttachmentCharset = InternalSchema.TextAttachmentCharset;

		[Autoload]
		public static readonly StorePropertyDefinition AttachRendering = InternalSchema.AttachRendering;

		[Autoload]
		internal static readonly StorePropertyDefinition ItemClass = InternalSchema.ItemClass;

		[Autoload]
		public static readonly StorePropertyDefinition DRMServerLicenseCompressed = InternalSchema.DRMServerLicenseCompressed;

		[Autoload]
		public static readonly StorePropertyDefinition DRMRights = InternalSchema.DRMRights;

		[Autoload]
		public static readonly StorePropertyDefinition DRMExpiryTime = InternalSchema.DRMExpiryTime;

		[Autoload]
		public static readonly StorePropertyDefinition DRMPropsSignature = InternalSchema.DRMPropsSignature;

		[Autoload]
		public static readonly StorePropertyDefinition AttachHash = InternalSchema.AttachHash;

		[Autoload]
		public static readonly StorePropertyDefinition AttachmentProviderEndpointUrl = InternalSchema.AttachmentProviderEndpointUrl;

		[Autoload]
		public static readonly StorePropertyDefinition AttachmentProviderType = InternalSchema.AttachmentProviderType;

		public static readonly StorePropertyDefinition ImageThumbnail = InternalSchema.ImageThumbnail;

		public static readonly StorePropertyDefinition ImageThumbnailSalientRegions = InternalSchema.ImageThumbnailSalientRegions;

		public static readonly StorePropertyDefinition ImageThumbnailHeight = InternalSchema.ImageThumbnailHeight;

		public static readonly StorePropertyDefinition ImageThumbnailWidth = InternalSchema.ImageThumbnailWidth;

		private static AttachmentSchema instance = null;
	}
}
