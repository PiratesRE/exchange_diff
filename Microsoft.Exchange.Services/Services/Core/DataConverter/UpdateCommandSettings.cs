using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class UpdateCommandSettings : CommandSettings
	{
		public UpdateCommandSettings(PropertyUpdate propertyUpdate, StoreObject storeObject, bool suppressReadReceipts, IFeaturesManager featuresManager)
		{
			this.storeObject = storeObject;
			this.propertyUpdate = propertyUpdate;
			this.suppressReadReceipts = suppressReadReceipts;
			this.featuresManager = featuresManager;
		}

		public StoreObject StoreObject
		{
			get
			{
				return this.storeObject;
			}
		}

		public PropertyUpdate PropertyUpdate
		{
			get
			{
				return this.propertyUpdate;
			}
		}

		public bool SuppressReadReceipts
		{
			get
			{
				return this.suppressReadReceipts;
			}
		}

		public IFeaturesManager FeaturesManager
		{
			get
			{
				return this.featuresManager;
			}
		}

		private readonly bool suppressReadReceipts;

		private StoreObject storeObject;

		private PropertyUpdate propertyUpdate;

		private IFeaturesManager featuresManager;
	}
}
