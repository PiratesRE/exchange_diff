using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Entities.TypeConversion.Translators
{
	internal interface IGenericItemTranslator
	{
		IItem ConvertToEntity(IItem storageItem);
	}
}
