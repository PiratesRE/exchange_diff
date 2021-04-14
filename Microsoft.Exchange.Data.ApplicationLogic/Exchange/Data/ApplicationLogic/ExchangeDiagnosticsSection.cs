using System;
using System.Configuration;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	public class ExchangeDiagnosticsSection : ConfigurationSection
	{
		public static ExchangeDiagnosticsSection GetConfig()
		{
			return ((ExchangeDiagnosticsSection)ConfigurationManager.GetSection("ExchangeDiagnosticsSection")) ?? new ExchangeDiagnosticsSection();
		}

		[ConfigurationProperty("DiagnosticsComponents")]
		public DiagnosticsComponents DiagnosticComponents
		{
			get
			{
				object obj = base["DiagnosticsComponents"];
				return obj as DiagnosticsComponents;
			}
		}
	}
}
