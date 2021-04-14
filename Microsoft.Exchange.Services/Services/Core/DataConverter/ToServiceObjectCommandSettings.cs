using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ToServiceObjectCommandSettings : ToServiceObjectCommandSettingsBase
	{
		public ToServiceObjectCommandSettings()
		{
		}

		public ToServiceObjectCommandSettings(PropertyPath propertyPath) : base(propertyPath)
		{
		}

		public StoreObject StoreObject
		{
			get
			{
				return this.storeObject;
			}
			set
			{
				this.storeObject = value;
			}
		}

		public ResponseShape ResponseShape
		{
			get
			{
				return this.responseShape;
			}
			set
			{
				this.responseShape = value;
			}
		}

		private StoreObject storeObject;

		private ResponseShape responseShape;
	}
}
