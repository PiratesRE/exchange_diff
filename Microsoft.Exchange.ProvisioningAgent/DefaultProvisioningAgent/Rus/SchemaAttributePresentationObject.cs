using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class SchemaAttributePresentationObject
	{
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public DataSyntax DataSyntax
		{
			get
			{
				return this.dataSyntax;
			}
			set
			{
				this.dataSyntax = value;
			}
		}

		private string displayName;

		private DataSyntax dataSyntax;
	}
}
