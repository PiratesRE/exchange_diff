using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class AddressComponent
	{
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public string Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		private string value;

		private string label;
	}
}
