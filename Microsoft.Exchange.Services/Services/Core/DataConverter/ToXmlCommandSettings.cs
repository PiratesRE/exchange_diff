using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ToXmlCommandSettings : ToXmlCommandSettingsBase
	{
		public ToXmlCommandSettings()
		{
		}

		public ToXmlCommandSettings(PropertyPath propertyPath) : base(propertyPath)
		{
		}

		public IdAndSession IdAndSession
		{
			get
			{
				return this.idAndSession;
			}
			set
			{
				this.idAndSession = value;
			}
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

		private IdAndSession idAndSession;

		private StoreObject storeObject;

		private ResponseShape responseShape;
	}
}
