using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Search.Platform.Parallax.DataLoad;
using Microsoft.Search.Platform.Parallax.Util.IniFormat;
using Microsoft.Search.Platform.Parallax.Util.IniFormat.FileModel;

namespace Microsoft.Exchange.VariantConfiguration.DataLoad
{
	internal class FlightReader : IFlightReader
	{
		internal FlightReader(IDataSourceReader dataSourceReader, IDataTransformation transformation, IEnumerable<string> inputDataSources)
		{
			if (dataSourceReader == null)
			{
				throw new ArgumentNullException("dataSourceReader");
			}
			if (transformation == null)
			{
				throw new ArgumentNullException("transformation");
			}
			if (inputDataSources == null)
			{
				throw new ArgumentNullException("inputDataSources");
			}
			this.dataSourceReader = dataSourceReader;
			this.transformation = transformation;
			this.flightMap = FlightReader.CreateFlightMap(this.dataSourceReader, this.transformation, inputDataSources);
		}

		public string GetFlightContent(string flightName)
		{
			return FlightReader.GetFlightContent(flightName, this.dataSourceReader, this.transformation, this.flightMap);
		}

		internal static Dictionary<string, string> CreateFlightMap(IDataSourceReader dataSourceReader, IDataTransformation transformation, IEnumerable<string> inputDataSources)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (string text in inputDataSources)
			{
				IniFileModel iniFileModel;
				if (FlightReader.TryCreateFileModel(text, dataSourceReader, transformation, out iniFileModel))
				{
					foreach (Section section in iniFileModel.Sections.Values)
					{
						if (section.Parameters.Any((ParameterAssignmentRule param) => string.Equals(param.ParameterName, "_meta.type", StringComparison.OrdinalIgnoreCase) && string.Equals(param.Value, "Microsoft.Exchange.Flighting.IFlight", StringComparison.OrdinalIgnoreCase)))
						{
							dictionary[section.Name] = text;
						}
					}
				}
			}
			return dictionary;
		}

		internal static string GetFlightContent(string flightName, IDataSourceReader dataSourceReader, IDataTransformation transformation, Dictionary<string, string> flightMap)
		{
			if (string.IsNullOrWhiteSpace(flightName))
			{
				throw new ArgumentNullException("dataSource");
			}
			if (flightMap.ContainsKey(flightName))
			{
				string dataSourceName = flightMap[flightName];
				IniFileModel iniFileModel;
				if (FlightReader.TryCreateFileModel(dataSourceName, dataSourceReader, transformation, out iniFileModel) && iniFileModel.Sections.ContainsKey(flightName))
				{
					IniFileModel iniFileModel2 = new IniFileModel();
					iniFileModel2.AddSection(iniFileModel.Sections[flightName]);
					return iniFileModel2.Serialize();
				}
			}
			return string.Join(Environment.NewLine, new string[]
			{
				string.Format("[{0}]", flightName),
				string.Format("{0}={1}", "_meta.type", "Microsoft.Exchange.Flighting.IFlight")
			});
		}

		private static bool TryCreateFileModel(string dataSourceName, IDataSourceReader dataSourceReader, IDataTransformation transformation, out IniFileModel fileModel)
		{
			if (dataSourceReader.CanGetContentReader(dataSourceName))
			{
				using (TextReader textReader = dataSourceReader.GetContentReader(dataSourceName)())
				{
					try
					{
						string text = transformation.Transform(dataSourceName, textReader.ReadToEnd());
						fileModel = IniFileModel.CreateFromString(dataSourceName, text);
						return true;
					}
					catch (IniParseException)
					{
					}
				}
			}
			fileModel = null;
			return false;
		}

		private const string IFlightTypeName = "Microsoft.Exchange.Flighting.IFlight";

		private const string TypeParameterName = "_meta.type";

		private readonly IDataSourceReader dataSourceReader;

		private readonly IDataTransformation transformation;

		private readonly Dictionary<string, string> flightMap;
	}
}
