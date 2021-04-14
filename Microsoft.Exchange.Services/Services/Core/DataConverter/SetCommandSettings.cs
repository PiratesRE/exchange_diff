using System;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class SetCommandSettings : CommandSettings
	{
		public SetCommandSettings(ServiceObject serviceObject, StoreObject storeObject)
		{
			this.serviceObject = serviceObject;
			this.storeObject = storeObject;
		}

		public StoreObject StoreObject
		{
			get
			{
				return this.storeObject;
			}
		}

		public ServiceObject ServiceObject
		{
			get
			{
				return this.serviceObject;
			}
		}

		public SetCommandSettings(XmlElement serviceProperty, StoreObject storeObject)
		{
			this.serviceProperty = serviceProperty;
			this.storeObject = storeObject;
		}

		public XmlElement ServiceProperty
		{
			get
			{
				return this.serviceProperty;
			}
		}

		private StoreObject storeObject;

		private ServiceObject serviceObject;

		private XmlElement serviceProperty;
	}
}
