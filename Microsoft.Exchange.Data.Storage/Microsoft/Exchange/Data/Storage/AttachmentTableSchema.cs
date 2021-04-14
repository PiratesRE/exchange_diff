using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AttachmentTableSchema : Schema
	{
		public new static AttachmentTableSchema Instance
		{
			get
			{
				if (AttachmentTableSchema.instance == null)
				{
					AttachmentTableSchema.instance = new AttachmentTableSchema();
				}
				return AttachmentTableSchema.instance;
			}
		}

		private static AttachmentTableSchema instance = null;

		[Autoload]
		internal static readonly StorePropertyDefinition RecordKey = AttachmentSchema.RecordKey;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachNum = AttachmentSchema.AttachNum;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachFileName = AttachmentSchema.AttachFileName;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachLongFileName = AttachmentSchema.AttachLongFileName;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachSize = AttachmentSchema.AttachSize;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachInConflict = InternalSchema.AttachInConflict;

		[Autoload]
		internal static readonly StorePropertyDefinition DisplayName = AttachmentSchema.DisplayName;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachMimTag = AttachmentSchema.AttachMimeTag;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachMethod = AttachmentSchema.AttachMethod;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachContentId = AttachmentSchema.AttachContentId;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachContentLocation = AttachmentSchema.AttachContentLocation;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachCalendarFlags = AttachmentSchema.AttachCalendarFlags;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachCalendarHidden = AttachmentSchema.AttachCalendarHidden;

		[Autoload]
		internal static readonly StorePropertyDefinition AppointmentExceptionStartTime = AttachmentSchema.AppointmentExceptionStartTime;

		[Autoload]
		internal static readonly StorePropertyDefinition AppointmentExceptionEndTime = AttachmentSchema.AppointmentExceptionEndTime;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachContentBase = AttachmentSchema.AttachContentBase;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachMhtmlFlags = AttachmentSchema.AttachMhtmlFlags;

		[Autoload]
		internal static readonly StorePropertyDefinition IsContactPhoto = AttachmentSchema.IsContactPhoto;

		[Autoload]
		internal static readonly StorePropertyDefinition RenderingPosition = AttachmentSchema.RenderingPosition;

		[Autoload]
		internal static readonly StorePropertyDefinition AttachEncoding = AttachmentSchema.AttachEncoding;

		[Autoload]
		internal static readonly StorePropertyDefinition TextAttachmentCharset = InternalSchema.TextAttachmentCharset;
	}
}
