using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ProductMsiConfigurationInfo : MsiConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "exchangeserver.msi";
			}
		}

		public override Guid ProductCode
		{
			get
			{
				return ProductMsiConfigurationInfo.productCode;
			}
		}

		protected override string LogFileName
		{
			get
			{
				return "ExchangeSetup.msilog";
			}
		}

		private const string MsiFileName = "exchangeserver.msi";

		private const string MsiLogFileName = "ExchangeSetup.msilog";

		protected static readonly Guid productCode = new Guid("{4934D1EA-BE46-48B1-8847-F1AF20E892C1}");
	}
}
