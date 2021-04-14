using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidItemForOperationException : ServicePermanentException
	{
		static InvalidItemForOperationException()
		{
			InvalidItemForOperationException.errorMappings.Add("CreateItemAttachment", (CoreResources.IDs)4225005690U);
			InvalidItemForOperationException.errorMappings.Add("CreateItem", CoreResources.IDs.ErrorInvalidItemForOperationCreateItem);
			InvalidItemForOperationException.errorMappings.Add("ExpandDL", (CoreResources.IDs)2181052460U);
			InvalidItemForOperationException.errorMappings.Add("SendItem", (CoreResources.IDs)4123291671U);
			InvalidItemForOperationException.errorMappings.Add(typeof(AcceptItemType).Name, CoreResources.IDs.ErrorInvalidItemForOperationAcceptItem);
			InvalidItemForOperationException.errorMappings.Add(typeof(DeclineItemType).Name, CoreResources.IDs.ErrorInvalidItemForOperationDeclineItem);
			InvalidItemForOperationException.errorMappings.Add(typeof(TentativelyAcceptItemType).Name, CoreResources.IDs.ErrorInvalidItemForOperationTentative);
			InvalidItemForOperationException.errorMappings.Add(typeof(CancelCalendarItemType).Name, CoreResources.IDs.ErrorInvalidItemForOperationCancelItem);
			InvalidItemForOperationException.errorMappings.Add(typeof(RemoveItemType).Name, CoreResources.IDs.ErrorInvalidItemForOperationRemoveItem);
			InvalidItemForOperationException.errorMappings.Add(typeof(ArchiveItem).Name, CoreResources.IDs.ErrorInvalidItemForOperationArchiveItem);
		}

		public InvalidItemForOperationException(string operation) : base(InvalidItemForOperationException.errorMappings[operation])
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}

		public const string CreateItemAttachmentOperation = "CreateItemAttachment";

		public const string CreateItemOperation = "CreateItem";

		public const string ExpandDLOperation = "ExpandDL";

		public const string SendItemOperation = "SendItem";

		private static Dictionary<string, Enum> errorMappings = new Dictionary<string, Enum>();
	}
}
