using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class CategoryType
	{
		public CategoryType(string name, int color)
		{
			this.Name = name;
			this.Color = color;
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = true, Order = 2)]
		public int Color { get; set; }
	}
}
