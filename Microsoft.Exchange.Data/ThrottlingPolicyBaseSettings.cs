using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal abstract class ThrottlingPolicyBaseSettings
	{
		protected ThrottlingPolicyBaseSettings()
		{
			this.settings = new Dictionary<string, string>();
		}

		protected ThrottlingPolicyBaseSettings(string value) : this()
		{
			ThrottlingPolicyBaseSettings.InternalParse(value, this.settings);
		}

		internal T Clone<T>() where T : ThrottlingPolicyBaseSettings, new()
		{
			if (typeof(T) != base.GetType())
			{
				throw new ArgumentException(string.Format("An object of type '{0}' could not be cloned to type '{1}'.", base.GetType(), typeof(T)), "T");
			}
			T t = Activator.CreateInstance<T>();
			t.settings.Clear();
			foreach (KeyValuePair<string, string> keyValuePair in this.settings)
			{
				t.settings.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return t;
		}

		internal static void InternalParse(string stateToParse, Dictionary<string, string> propertyBag)
		{
			if (!string.IsNullOrEmpty(stateToParse))
			{
				string[] array = stateToParse.Split(new char[]
				{
					'~'
				});
				if (array.Length == 0 || array.Length % 2 != 0)
				{
					throw new FormatException(DataStrings.ThrottlingPolicyStateCorrupted(stateToParse));
				}
				for (int i = 0; i < array.Length; i += 2)
				{
					if (propertyBag.ContainsKey(array[i]))
					{
						throw new FormatException(DataStrings.ThrottlingPolicyStateCorrupted(stateToParse));
					}
					propertyBag[array[i]] = array[i + 1];
				}
			}
		}

		internal static Unlimited<uint> ParseValue(string valueToConvert)
		{
			if (valueToConvert.Equals("-1", StringComparison.OrdinalIgnoreCase))
			{
				return Unlimited<uint>.UnlimitedValue;
			}
			Unlimited<uint> result;
			if (!Unlimited<uint>.TryParse(valueToConvert, out result))
			{
				throw new FormatException(DataStrings.ThrottlingPolicyStateCorrupted(valueToConvert));
			}
			return result;
		}

		protected Unlimited<uint>? GetValueFromPropertyBag(string key)
		{
			string valueToConvert;
			if (this.settings.TryGetValue(key, out valueToConvert))
			{
				return new Unlimited<uint>?(ThrottlingPolicyBaseSettings.ParseValue(valueToConvert));
			}
			return null;
		}

		protected void SetValueInPropertyBag(string key, Unlimited<uint>? value)
		{
			if (value != null)
			{
				this.settings[key] = value.ToString();
				return;
			}
			this.settings.Remove(key);
		}

		public override string ToString()
		{
			if (this.settings.Count == 0)
			{
				return null;
			}
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in this.settings)
			{
				string text = keyValuePair.Value;
				if (text.Equals(Unlimited<uint>.UnlimitedString))
				{
					text = "-1";
				}
				if (flag)
				{
					stringBuilder.AppendFormat("{0}~{1}", keyValuePair.Key, text);
					flag = false;
				}
				else
				{
					stringBuilder.AppendFormat("~{0}~{1}", keyValuePair.Key, text);
				}
			}
			return stringBuilder.ToString();
		}

		private const string UnthrottledStringInAD = "-1";

		private Dictionary<string, string> settings;
	}
}
