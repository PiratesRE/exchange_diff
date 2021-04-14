using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class SetEnabledAttachmentsProperty : AttachmentsProperty, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public SetEnabledAttachmentsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static SetEnabledAttachmentsProperty CreateCommand(CommandContext commandContext)
		{
			return new SetEnabledAttachmentsProperty(commandContext);
		}

		public override void ToServiceObject()
		{
			bool createItemWithAttachments = EWSSettings.CreateItemWithAttachments;
			base.InternalToServiceObject(createItemWithAttachments);
		}

		public void Set()
		{
			EWSSettings.CreateItemWithAttachments = true;
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			this.SetAttachments(commandSettings.ServiceObject, commandSettings.StoreObject, false);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			this.SetAttachments(setPropertyUpdate.ServiceObject, updateCommandSettings.StoreObject, true);
		}

		private void SetAttachments(ServiceObject serviceObject, StoreObject storeObject, bool isUpdating)
		{
			AttachmentType[] array = serviceObject[this.commandContext.PropertyInformation] as AttachmentType[];
			if (array.Length == 0)
			{
				return;
			}
			if (EWSSettings.AttachmentNestLevel > Global.MaxAttachmentNestLevel)
			{
				throw new AttachmentNestLevelLimitExceededException();
			}
			EWSSettings.AttachmentNestLevel++;
			Item item = (Item)storeObject;
			if (isUpdating && item.ClassName.Contains(".SMIME") && array.Length > 0 && array[0] is FileAttachmentType && array[0].Name == "smime.p7m" && (array[0].ContentType == "application/x-pkcs7-mime" || array[0].ContentType == "application/pkcs7-mime" || array[0].ContentType == "application/x-pkcs7-signature" || array[0].ContentType == "application/pkcs7-signature"))
			{
				item.AttachmentCollection.RemoveAll();
			}
			using (AttachmentBuilder attachmentBuilder = new AttachmentBuilder(new AttachmentHierarchy(item), array, this.commandContext.IdConverter))
			{
				attachmentBuilder.CreateAllAttachments();
			}
			EWSSettings.AttachmentNestLevel--;
		}
	}
}
