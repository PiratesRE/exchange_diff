using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExtendedEwsStoreObjectSchema : ObjectSchema
	{
		private static readonly Guid DefaultPropertySetId = new Guid("5822CB43-247D-4953-AB15-83AB07C54CE8");

		public static readonly ExtendedPropertyDefinition AlternativeId = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "AlternativeId", 25);

		public static readonly ExtendedPropertyDefinition ExchangeVersion = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "ExchangeVersion", 16);

		public static readonly ExtendedPropertyDefinition ExtendedAttributes = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "ExtendedAttributes", 2);

		public static readonly ExtendedPropertyDefinition Message = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "Message", 2);

		public static readonly ExtendedPropertyDefinition PercentComplete = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "PercentComplete", 14);

		public static readonly ExtendedPropertyDefinition Status = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "Status", 14);

		public static readonly ExtendedPropertyDefinition DisplayName = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "DisplayName", 2);

		public static readonly ExtendedPropertyDefinition IsNotificationEmailFromTaskSent = new ExtendedPropertyDefinition(ExtendedEwsStoreObjectSchema.DefaultPropertySetId, "IsNotificationEmailFromTaskSent", 4);
	}
}
