using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.VariantConfiguration.DataLoad;
using Microsoft.Exchange.VariantConfiguration.Parser;
using Microsoft.Search.Platform.Parallax.DataLoad;

namespace Microsoft.Exchange.VariantConfiguration
{
	internal class FlightDependencyTransformation : IDataTransformation
	{
		internal FlightDependencyTransformation(IFlightReader flightReader)
		{
			if (flightReader == null)
			{
				throw new ArgumentNullException("flightReader");
			}
			this.flightReader = flightReader;
		}

		public string Transform(string dataSourceName, string input)
		{
			if (!dataSourceName.EndsWith(".settings.ini", StringComparison.OrdinalIgnoreCase))
			{
				return input;
			}
			IEnumerable<string> flightDependencies = FlightDependencyTransformation.GetFlightDependencies(input, dataSourceName);
			return this.AppendFlightDefinitions(input, flightDependencies);
		}

		internal static IEnumerable<string> GetFlightDependencies(string input, string dataSourceName)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			ConfigurationComponent configurationComponent;
			if (ConfigurationComponent.TryCreate(input, dataSourceName, out configurationComponent))
			{
				foreach (ConfigurationSection configurationSection in configurationComponent.Sections)
				{
					IEnumerable<string> flightDependencies = configurationSection.GetFlightDependencies();
					foreach (string item in flightDependencies)
					{
						hashSet.Add(item);
					}
				}
			}
			return hashSet;
		}

		internal static string AddPrefixToFlightSectionName(string flightName, string flightContent)
		{
			flightName = Regex.Escape(flightName);
			return Regex.Replace(flightContent, string.Format("^\\[{0}\\]", flightName), string.Format("[{0}{1}]", "flt.", flightName), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		}

		internal string AppendFlightDefinitions(string input, IEnumerable<string> flightDependencies)
		{
			StringBuilder stringBuilder = new StringBuilder(input);
			stringBuilder.AppendLine();
			foreach (string flightName in flightDependencies)
			{
				stringBuilder.AppendLine(FlightDependencyTransformation.AddPrefixToFlightSectionName(flightName, this.flightReader.GetFlightContent(flightName)));
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		private const string FlightPrefix = "flt.";

		private const string SettingsFileSuffix = ".settings.ini";

		private readonly IFlightReader flightReader;
	}
}
