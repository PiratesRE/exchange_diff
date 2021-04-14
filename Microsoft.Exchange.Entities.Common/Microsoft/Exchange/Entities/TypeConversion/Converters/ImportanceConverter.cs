using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	internal struct ImportanceConverter : IConverter<Microsoft.Exchange.Data.Storage.Importance, Microsoft.Exchange.Entities.DataModel.Items.Importance>, IConverter<Microsoft.Exchange.Entities.DataModel.Items.Importance, Microsoft.Exchange.Data.Storage.Importance>
	{
		public IConverter<Microsoft.Exchange.Data.Storage.Importance, Microsoft.Exchange.Entities.DataModel.Items.Importance> StorageToEntitiesConverter
		{
			get
			{
				return this;
			}
		}

		public IConverter<Microsoft.Exchange.Entities.DataModel.Items.Importance, Microsoft.Exchange.Data.Storage.Importance> EntitiesToStorageConverter
		{
			get
			{
				return this;
			}
		}

		Microsoft.Exchange.Entities.DataModel.Items.Importance IConverter<Microsoft.Exchange.Data.Storage.Importance, Microsoft.Exchange.Entities.DataModel.Items.Importance>.Convert(Microsoft.Exchange.Data.Storage.Importance value)
		{
			switch (value)
			{
			case Microsoft.Exchange.Data.Storage.Importance.Low:
				return Microsoft.Exchange.Entities.DataModel.Items.Importance.Low;
			case Microsoft.Exchange.Data.Storage.Importance.Normal:
				return Microsoft.Exchange.Entities.DataModel.Items.Importance.Normal;
			case Microsoft.Exchange.Data.Storage.Importance.High:
				return Microsoft.Exchange.Entities.DataModel.Items.Importance.High;
			default:
				throw new ArgumentOutOfRangeException("value");
			}
		}

		Microsoft.Exchange.Data.Storage.Importance IConverter<Microsoft.Exchange.Entities.DataModel.Items.Importance, Microsoft.Exchange.Data.Storage.Importance>.Convert(Microsoft.Exchange.Entities.DataModel.Items.Importance value)
		{
			switch (value)
			{
			case Microsoft.Exchange.Entities.DataModel.Items.Importance.Low:
				return Microsoft.Exchange.Data.Storage.Importance.Low;
			case Microsoft.Exchange.Entities.DataModel.Items.Importance.Normal:
				return Microsoft.Exchange.Data.Storage.Importance.Normal;
			case Microsoft.Exchange.Entities.DataModel.Items.Importance.High:
				return Microsoft.Exchange.Data.Storage.Importance.High;
			default:
				throw new ArgumentOutOfRangeException("value");
			}
		}
	}
}
