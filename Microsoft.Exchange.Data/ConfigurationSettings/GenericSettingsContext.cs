using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GenericSettingsContext : SettingsContextBase
	{
		public GenericSettingsContext(string propertyName, string propertyValue, SettingsContextBase nextContext = null) : base(nextContext)
		{
			this.propertyName = propertyName;
			this.propertyValue = propertyValue;
		}

		public override string GetGenericProperty(string propertyName)
		{
			if (StringComparer.InvariantCultureIgnoreCase.Equals(propertyName, this.propertyName))
			{
				return this.propertyValue;
			}
			return null;
		}

		public override XElement GetDiagnosticInfo(string argument)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(argument);
			diagnosticInfo.Add(new XElement(this.propertyName, this.propertyValue));
			return diagnosticInfo;
		}

		private readonly string propertyName;

		private readonly string propertyValue;
	}
}
