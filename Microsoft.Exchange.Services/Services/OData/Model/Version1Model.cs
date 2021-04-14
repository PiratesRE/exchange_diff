using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class Version1Model : ServiceModel
	{
		protected override EdmModel BuildModel()
		{
			EdmModel edmModel = new EdmModel();
			ExtensionMethods.SetEdmVersion(edmModel, new Version("4.0"));
			SerializationExtensionMethods.SetEdmxVersion(edmModel, new Version("4.0"));
			edmModel.AddElement(EntitySets.EdmEntityContainer);
			EntitySets.EdmEntityContainer.AddElement(EntitySets.Users);
			EntitySets.EdmEntityContainer.AddElement(EntitySets.Me);
			EnumTypes.Register(edmModel);
			edmModel.AddElement(Recipient.EdmComplexType.Member);
			edmModel.AddElement(Attendee.EdmComplexType.Member);
			edmModel.AddElement(ItemBody.EdmComplexType.Member);
			edmModel.AddElement(Location.EdmComplexType.Member);
			edmModel.AddElement(ResponseStatus.EdmComplexType.Member);
			edmModel.AddElement(PhysicalAddress.EdmComplexType.Member);
			edmModel.AddElement(RecurrencePattern.EdmComplexType.Member);
			edmModel.AddElement(RecurrenceRange.EdmComplexType.Member);
			edmModel.AddElement(PatternedRecurrence.EdmComplexType.Member);
			EntitySchema.SchemaInstance.RegisterEdmModel(edmModel);
			UserSchema.SchemaInstance.RegisterEdmModel(edmModel);
			FolderSchema.SchemaInstance.RegisterEdmModel(edmModel);
			ItemSchema.SchemaInstance.RegisterEdmModel(edmModel);
			MessageSchema.SchemaInstance.RegisterEdmModel(edmModel);
			AttachmentSchema.SchemaInstance.RegisterEdmModel(edmModel);
			FileAttachmentSchema.SchemaInstance.RegisterEdmModel(edmModel);
			ItemAttachmentSchema.SchemaInstance.RegisterEdmModel(edmModel);
			CalendarSchema.SchemaInstance.RegisterEdmModel(edmModel);
			CalendarGroupSchema.SchemaInstance.RegisterEdmModel(edmModel);
			EventSchema.SchemaInstance.RegisterEdmModel(edmModel);
			ContactSchema.SchemaInstance.RegisterEdmModel(edmModel);
			ContactFolderSchema.SchemaInstance.RegisterEdmModel(edmModel);
			return edmModel;
		}
	}
}
