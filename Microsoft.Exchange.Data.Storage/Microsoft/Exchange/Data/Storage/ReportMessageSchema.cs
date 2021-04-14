using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReportMessageSchema : MessageItemSchema
	{
		public new static ReportMessageSchema Instance
		{
			get
			{
				if (ReportMessageSchema.instance == null)
				{
					ReportMessageSchema.instance = new ReportMessageSchema();
				}
				return ReportMessageSchema.instance;
			}
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			base.CoreObjectUpdate(coreItem, operation);
			ReportMessage.CoreObjectUpdateSubjectPrefix(coreItem);
		}

		private static ReportMessageSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition OriginalMessageId = InternalSchema.OriginalMessageId;

		[Autoload]
		public new static readonly StorePropertyDefinition ReceivedRepresenting = InternalSchema.ReceivedRepresenting;

		[Autoload]
		internal static readonly StorePropertyDefinition OriginalDisplayBcc = InternalSchema.OriginalDisplayBcc;

		[Autoload]
		internal static readonly StorePropertyDefinition OriginalDisplayCc = InternalSchema.OriginalDisplayCc;

		[Autoload]
		internal static readonly StorePropertyDefinition OriginalDisplayTo = InternalSchema.OriginalDisplayTo;

		[Autoload]
		internal static readonly StorePropertyDefinition OriginalSentTime = InternalSchema.OriginalSentTime;

		[Autoload]
		internal static readonly StorePropertyDefinition OriginalSubject = InternalSchema.OriginalSubject;

		[Autoload]
		internal static readonly StorePropertyDefinition ReportTime = InternalSchema.ReportTime;

		[Autoload]
		internal static readonly StorePropertyDefinition ReportingMta = InternalSchema.ReportingMta;

		[Autoload]
		internal new static readonly StorePropertyDefinition MessageLocaleId = InternalSchema.MessageLocaleId;
	}
}
