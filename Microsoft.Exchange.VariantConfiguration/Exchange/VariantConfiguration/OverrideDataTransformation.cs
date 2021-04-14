using System;
using System.Linq;
using Microsoft.Search.Platform.Parallax.DataLoad;
using Microsoft.Search.Platform.Parallax.Util.IniFormat.FileModel;

namespace Microsoft.Exchange.VariantConfiguration
{
	internal class OverrideDataTransformation : IDataTransformation
	{
		private OverrideDataTransformation(VariantConfigurationOverride validationOverride)
		{
			if (validationOverride == null)
			{
				throw new ArgumentNullException("validationOverride");
			}
			this.validationOverride = validationOverride;
		}

		private OverrideDataTransformation()
		{
		}

		public static OverrideDataTransformation Get(VariantConfigurationOverride validationOverride)
		{
			if (validationOverride == null)
			{
				return OverrideDataTransformation.instance;
			}
			return new OverrideDataTransformation(validationOverride);
		}

		public string Transform(string dataSourceName, string input)
		{
			string text = input;
			if (text.Contains("_meta.access=public"))
			{
				text = text.Replace("_meta.access=public", string.Empty);
			}
			VariantConfigurationOverride[] array = VariantConfiguration.Overrides;
			if (this.validationOverride != null)
			{
				if (array != null)
				{
					array = array.Concat(new VariantConfigurationOverride[]
					{
						this.validationOverride
					}).ToArray<VariantConfigurationOverride>();
				}
				else
				{
					array = new VariantConfigurationOverride[]
					{
						this.validationOverride
					};
				}
			}
			if (array != null && array.Length > 0)
			{
				IniFileModel iniFileModel = IniFileModel.CreateFromString(dataSourceName, text);
				bool flag = false;
				foreach (VariantConfigurationOverride variantConfigurationOverride in array.Reverse<VariantConfigurationOverride>())
				{
					if (variantConfigurationOverride.FileName.Equals(dataSourceName, StringComparison.InvariantCultureIgnoreCase) && iniFileModel.Sections.ContainsKey(variantConfigurationOverride.SectionName))
					{
						Section section = iniFileModel.Sections[variantConfigurationOverride.SectionName];
						Section section2 = new Section(section.Name);
						foreach (ParameterAssignmentRule parameterAssignmentRule in this.GetParameterAssignmentRules(variantConfigurationOverride))
						{
							section2.AddParameter(parameterAssignmentRule);
						}
						foreach (ParameterAssignmentRule parameterAssignmentRule2 in section.Parameters)
						{
							section2.AddParameter(parameterAssignmentRule2);
						}
						iniFileModel.RemoveSection(section);
						iniFileModel.AddSection(section2);
						flag = true;
					}
				}
				if (flag)
				{
					text = iniFileModel.Serialize();
				}
			}
			return text;
		}

		private ParameterAssignmentRule[] GetParameterAssignmentRules(VariantConfigurationOverride o)
		{
			ParameterAssignmentRule[] array = new ParameterAssignmentRule[o.Parameters.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = o.Parameters[i].Split(OverrideDataTransformation.Assignment, 2);
				string text = array2[1];
				array2 = array2[0].Split(OverrideDataTransformation.Ampersand, 2);
				string text2 = array2[0];
				string text3 = (array2.Length > 1) ? ("&" + array2[1]) : string.Empty;
				array[i] = new ParameterAssignmentRule(o.SectionName, text2, text3, text);
			}
			return array;
		}

		private const string MetaAccess = "_meta.access=public";

		private static readonly char[] Assignment = new char[]
		{
			'='
		};

		private static readonly char[] Ampersand = new char[]
		{
			'&'
		};

		private static OverrideDataTransformation instance = new OverrideDataTransformation();

		private VariantConfigurationOverride validationOverride;
	}
}
