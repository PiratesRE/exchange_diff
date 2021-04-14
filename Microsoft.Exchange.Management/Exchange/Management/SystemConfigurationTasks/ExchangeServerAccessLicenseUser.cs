using System;
using System.Text;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class ExchangeServerAccessLicenseUser : ExchangeServerAccessLicenseBase
	{
		public ExchangeServerAccessLicenseUser(string licenseName, string name) : base(licenseName)
		{
			this.Name = name;
		}

		public string Name { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LicenseName: ");
			stringBuilder.AppendLine(base.LicenseName);
			stringBuilder.Append("Name: ");
			stringBuilder.AppendLine(this.Name);
			return stringBuilder.ToString();
		}
	}
}
