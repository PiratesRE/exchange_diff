using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	internal struct SensitivityConverter : IConverter<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>, IConverter<Microsoft.Exchange.Entities.DataModel.Items.Sensitivity, Microsoft.Exchange.Data.Storage.Sensitivity>
	{
		public IConverter<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity> StorageToEntitiesConverter
		{
			get
			{
				return this;
			}
		}

		public IConverter<Microsoft.Exchange.Entities.DataModel.Items.Sensitivity, Microsoft.Exchange.Data.Storage.Sensitivity> EntitiesToStorageConverter
		{
			get
			{
				return this;
			}
		}

		Microsoft.Exchange.Entities.DataModel.Items.Sensitivity IConverter<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>.Convert(Microsoft.Exchange.Data.Storage.Sensitivity value)
		{
			return SensitivityConverter.mappingConverter.Convert(value);
		}

		Microsoft.Exchange.Data.Storage.Sensitivity IConverter<Microsoft.Exchange.Entities.DataModel.Items.Sensitivity, Microsoft.Exchange.Data.Storage.Sensitivity>.Convert(Microsoft.Exchange.Entities.DataModel.Items.Sensitivity value)
		{
			return SensitivityConverter.mappingConverter.Reverse(value);
		}

		private static SimpleMappingConverter<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity> mappingConverter = SimpleMappingConverter<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>.CreateStrictConverter(new Tuple<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>[]
		{
			new Tuple<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>(Microsoft.Exchange.Data.Storage.Sensitivity.Normal, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity.Normal),
			new Tuple<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>(Microsoft.Exchange.Data.Storage.Sensitivity.Personal, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity.Personal),
			new Tuple<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>(Microsoft.Exchange.Data.Storage.Sensitivity.Private, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity.Private),
			new Tuple<Microsoft.Exchange.Data.Storage.Sensitivity, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity>(Microsoft.Exchange.Data.Storage.Sensitivity.CompanyConfidential, Microsoft.Exchange.Entities.DataModel.Items.Sensitivity.Confidential)
		});
	}
}
