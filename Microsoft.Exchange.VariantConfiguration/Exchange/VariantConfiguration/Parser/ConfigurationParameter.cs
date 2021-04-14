using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.VariantConfiguration.Parser
{
	internal class ConfigurationParameter
	{
		internal ConfigurationParameter(string name, string value, IEnumerable<KeyValuePair<string, string>> variants)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (variants == null)
			{
				throw new ArgumentNullException("variants");
			}
			this.Variants = variants;
			this.Name = name;
			this.Value = value;
		}

		public string Name { get; private set; }

		public string Value { get; private set; }

		public IEnumerable<KeyValuePair<string, string>> Variants { get; private set; }

		public static ConfigurationParameter Create(string line)
		{
			string name;
			KeyValuePair<string, string>[] variants;
			string value;
			if (ConfigurationParameter.TryParseParameterLine(line, out name, out variants, out value))
			{
				return new ConfigurationParameter(name, value, variants);
			}
			throw new VariantConfigurationSyntaxException(string.Format("Unabled to parse line {0}", line));
		}

		public static bool TryParseParameterLine(string line, out string parameterName, out KeyValuePair<string, string>[] variants, out string parameterValue)
		{
			bool result;
			try
			{
				Match match = ConfigurationParameter.ParameterRegex.Match(line);
				if (!match.Success)
				{
					parameterName = null;
					variants = null;
					parameterValue = null;
					result = false;
				}
				else
				{
					parameterName = match.Groups["PN"].Value;
					Group group = match.Groups["VN"];
					Group group2 = match.Groups["VV"];
					List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
					if (group.Success && group2.Success)
					{
						for (int i = 0; i < group.Captures.Count; i++)
						{
							list.Add(new KeyValuePair<string, string>(group.Captures[i].Value, group2.Captures[i].Value));
						}
					}
					variants = list.ToArray();
					parameterValue = match.Groups["PV"].Value;
					result = true;
				}
			}
			catch (Exception)
			{
				parameterName = null;
				variants = null;
				parameterValue = null;
				result = false;
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("{0} equals {1}", this.Name, this.Value));
			if (this.Variants.Count<KeyValuePair<string, string>>() > 0)
			{
				stringBuilder.Append(" if ");
				stringBuilder.Append(string.Join<KeyValuePair<string, string>>(" and ", this.Variants));
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			ConfigurationParameter configurationParameter = obj as ConfigurationParameter;
			return configurationParameter != null && string.Equals(this.Name, configurationParameter.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Value, configurationParameter.Value, StringComparison.OrdinalIgnoreCase) && this.Variants.SequenceEqual(configurationParameter.Variants);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.Name.ToLowerInvariant().GetHashCode() ^ this.Value.ToLowerInvariant().GetHashCode() ^ this.CalculateVariantsHash();
		}

		private int CalculateVariantsHash()
		{
			int num = 0;
			foreach (KeyValuePair<string, string> keyValuePair in this.Variants)
			{
				num ^= keyValuePair.GetHashCode();
			}
			return num;
		}

		private static readonly string VariantValueUnquoted = "(?<VV>[\\w\\.\\-\\+\\^]+)";

		private static readonly string VariantValueQuoted = "(?:\"(?<VV>(?:\"\"|[^\"])*)\")";

		private static readonly string ParameterName = "[\\w\\.\\-\\+\\\\\\/\\{\\}\\(\\)#\\*\\$\\[\\]:]+";

		private static readonly Regex ParameterRegex = new Regex(string.Format("^\\s*(?<PN>{0})\\s*((&\\s*(?<VN>[\\w\\.\\-\\+\\^]+)\\s*:\\s*(?:{1}|{2})\\s*)+)?=\\s*(?<PV>.*?)\\s*$", ConfigurationParameter.ParameterName, ConfigurationParameter.VariantValueUnquoted, ConfigurationParameter.VariantValueQuoted), RegexOptions.Compiled);
	}
}
