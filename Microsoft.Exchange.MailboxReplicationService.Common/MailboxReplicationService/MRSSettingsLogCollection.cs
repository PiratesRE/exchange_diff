using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class MRSSettingsLogCollection
	{
		public MRSSettingsLogCollection(string settingsString)
		{
			if (string.IsNullOrWhiteSpace(settingsString))
			{
				return;
			}
			string[] array = settingsString.Split(new char[]
			{
				';'
			});
			foreach (string text in array)
			{
				string text2 = text.Trim();
				if (!string.IsNullOrWhiteSpace(text2))
				{
					this.SettingsLogCollection.Add(new MRSSettingsLogCollection.MRSSettingsLogElement(text2));
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (MRSSettingsLogCollection.MRSSettingsLogElement mrssettingsLogElement in this.SettingsLogCollection)
			{
				if (mrssettingsLogElement != null && !string.IsNullOrWhiteSpace(mrssettingsLogElement.SettingName))
				{
					if (!flag)
					{
						stringBuilder.Append(';');
						flag = false;
					}
					stringBuilder.AppendFormat("{0}", mrssettingsLogElement.SettingName);
				}
			}
			return stringBuilder.ToString();
		}

		internal readonly List<MRSSettingsLogCollection.MRSSettingsLogElement> SettingsLogCollection = new List<MRSSettingsLogCollection.MRSSettingsLogElement>();

		public class MRSSettingsLogElement
		{
			public string SettingName { get; set; }

			public MRSSettingsLogElement(string inputSettingName)
			{
				this.SettingName = inputSettingName;
			}
		}

		public class MRSSettingsLogCollectionConverter : TypeConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
			}

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value == null || value is string)
				{
					return new MRSSettingsLogCollection((string)value);
				}
				return base.ConvertFrom(context, culture, value);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(MRSSettingsLogCollection))
				{
					return this.ConvertFrom(context, culture, value);
				}
				MRSSettingsLogCollection mrssettingsLogCollection = value as MRSSettingsLogCollection;
				if (mrssettingsLogCollection == null)
				{
					throw new ArgumentException("Converted value is not of MRSSettingsLogCollection type");
				}
				if (destinationType == typeof(string))
				{
					return mrssettingsLogCollection.ToString();
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
