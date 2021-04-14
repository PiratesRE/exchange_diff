using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreItemSchema : CoreObjectSchema
	{
		private CoreItemSchema()
		{
		}

		public new static CoreItemSchema Instance
		{
			get
			{
				if (CoreItemSchema.instance == null)
				{
					CoreItemSchema.instance = new CoreItemSchema();
				}
				return CoreItemSchema.instance;
			}
		}

		private static CoreItemSchema instance = null;

		public static readonly StorePropertyDefinition ClientSubmittedSecurely = InternalSchema.ClientSubmittedSecurely;

		[Autoload]
		public static readonly StorePropertyDefinition Codepage = InternalSchema.Codepage;

		[Autoload]
		internal static readonly StorePropertyDefinition DavSubmitData = InternalSchema.DavSubmitData;

		[Autoload]
		public static readonly StorePropertyDefinition Flags = InternalSchema.Flags;

		[Autoload]
		internal static readonly StorePropertyDefinition IsResend = InternalSchema.IsResend;

		[Autoload]
		internal static readonly StorePropertyDefinition NeedSpecialRecipientProcessing = InternalSchema.NeedSpecialRecipientProcessing;

		[Autoload]
		public static readonly StorePropertyDefinition Id = InternalSchema.ItemId;

		[Autoload]
		public static readonly StorePropertyDefinition ItemClass = InternalSchema.ItemClass;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedUrl = InternalSchema.LinkedUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedId = InternalSchema.LinkedId;

		[Autoload]
		public static readonly StorePropertyDefinition MapiHasAttachment = InternalSchema.MapiHasAttachment;

		[Autoload]
		public static readonly StorePropertyDefinition MessageInConflict = InternalSchema.MessageInConflict;

		[Autoload]
		public static readonly StorePropertyDefinition MessageStatus = InternalSchema.MessageStatus;

		[Autoload]
		public static readonly StorePropertyDefinition NativeBodyInfo = InternalSchema.NativeBodyInfo;

		[Autoload]
		public static readonly StorePropertyDefinition NormalizedSubject = InternalSchema.NormalizedSubject;

		[Autoload]
		public static readonly StorePropertyDefinition PropertyExistenceTracker = InternalSchema.PropertyExistenceTracker;

		[Autoload]
		public static readonly StorePropertyDefinition FavLevelMask = InternalSchema.FavLevelMask;

		[Autoload]
		public static readonly StorePropertyDefinition MapiSensitivity = InternalSchema.MapiSensitivity;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition ReceivedTime = InternalSchema.ReceivedTime;

		public static readonly StorePropertyDefinition RenewTime = InternalSchema.RenewTime;

		public static readonly StorePropertyDefinition ReceivedOrRenewTime = InternalSchema.ReceivedOrRenewTime;

		public static readonly StorePropertyDefinition RichContent = InternalSchema.RichContent;

		public static readonly StorePropertyDefinition MailboxGuid = InternalSchema.MailboxGuidGuid;

		[Autoload]
		public static readonly StorePropertyDefinition Size = InternalSchema.Size;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition Subject = InternalSchema.Subject;

		[LegalTracking]
		[DetectCodepage]
		public static readonly StorePropertyDefinition SubjectPrefix = InternalSchema.SubjectPrefix;

		public static readonly PropertyTagPropertyDefinition XMsExchOrganizationAVStampMailbox = InternalSchema.XMsExchOrganizationAVStampMailbox;

		internal static readonly StorePropertyDefinition XMsExchOrganizationOriginalClientIPAddress = InternalSchema.XMsExchOrganizationOriginalClientIPAddress;

		internal static readonly StorePropertyDefinition XMsExchOrganizationOriginalServerIPAddress = InternalSchema.XMsExchOrganizationOriginalServerIPAddress;

		internal static readonly StorePropertyDefinition AnnotationToken = InternalSchema.AnnotationToken;

		[Autoload]
		internal static readonly PropertyTagPropertyDefinition ReadCnNew = InternalSchema.ReadCnNew;
	}
}
