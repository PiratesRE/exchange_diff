using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	[Serializable]
	internal class GlsPropertyValidationError : ValidationError
	{
		public GlsPropertyValidationError(LocalizedString description, GlsProperty propertyDefinition, object invalidData) : base(description, propertyDefinition.Name)
		{
			this.invalidData = invalidData;
			this.propertyDefinition = propertyDefinition;
		}

		public object InvalidData
		{
			get
			{
				return this.invalidData;
			}
		}

		public GlsProperty PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		private object invalidData;

		private GlsProperty propertyDefinition;
	}
}
