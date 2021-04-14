using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ArrayPropertyInformation : PropertyInformation
	{
		public ArrayPropertyInformation(string arrayLocalName, ExchangeVersion effectiveVersion, string arrayItemLocalName, PropertyDefinition propertyDefinition, PropertyPath propertyPath, PropertyCommand.CreatePropertyCommand createCommand) : base(arrayLocalName, effectiveVersion, propertyDefinition, propertyPath, createCommand)
		{
			this.arrayItemLocalName = arrayItemLocalName;
			this.arrayLocalName = arrayLocalName;
		}

		public string ArrayItemLocalName
		{
			get
			{
				return this.arrayItemLocalName;
			}
		}

		public string ArrayLocalName
		{
			get
			{
				return this.arrayLocalName;
			}
		}

		private readonly string arrayLocalName;

		private string arrayItemLocalName;
	}
}
