using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatacenterMsiConfigurationInfo : MsiConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "Datacenter.msi";
			}
		}

		public override Guid ProductCode
		{
			get
			{
				return DatacenterMsiConfigurationInfo.productCode;
			}
		}

		protected override string LogFileName
		{
			get
			{
				return "DatacenterSetup.msilog";
			}
		}

		private const string MsiFileName = "Datacenter.msi";

		private const string MsiLogFileName = "DatacenterSetup.msilog";

		protected static readonly Guid productCode = new Guid("{7FD073ED-7852-4798-A223-A0266176B4DA}");
	}
}
