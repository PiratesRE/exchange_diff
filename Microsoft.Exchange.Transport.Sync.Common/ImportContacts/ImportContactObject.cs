using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.ImportContacts
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ImportContactObject
	{
		internal ImportContactObject(int index)
		{
			this.index = index;
			this.propertyBag = new Dictionary<ImportContactProperties, object>(ImportContactObject.propertyBagInitialSize);
			this.appendedDataToNotes = false;
		}

		internal int Index
		{
			get
			{
				return this.index;
			}
		}

		internal int PropertyCount
		{
			get
			{
				return this.propertyBag.Count;
			}
		}

		internal Dictionary<ImportContactProperties, object> PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		internal void AddProperty(string propertyName, string value, Dictionary<string, ImportContactProperties> mappingDictionary, CultureInfo culture)
		{
			SyncUtilities.ThrowIfArgumentNull("propertyName", propertyName);
			SyncUtilities.ThrowIfArgumentNull("value", value);
			SyncUtilities.ThrowIfArgumentNull("mappingDictionary", mappingDictionary);
			SyncUtilities.ThrowIfArgumentNull("culture", culture);
			if (value == string.Empty)
			{
				return;
			}
			ImportContactProperties importContactProperties;
			if (!mappingDictionary.TryGetValue(propertyName, out importContactProperties))
			{
				this.AppendToNotes(propertyName, value, null);
				return;
			}
			if (ImportContactObject.ShouldIgnoreProperty(importContactProperties))
			{
				return;
			}
			object value2;
			if (!this.propertyBag.ContainsKey(importContactProperties) && this.GetValueObject(importContactProperties, value, culture, out value2))
			{
				this.propertyBag[importContactProperties] = value2;
				return;
			}
			this.AppendToNotes(propertyName, value, new ImportContactProperties?(importContactProperties));
		}

		private static bool ShouldIgnoreProperty(ImportContactProperties property)
		{
			return property == ImportContactProperties.IgnoredProperty || property == ImportContactProperties.Gender || property == ImportContactProperties.Priority || property == ImportContactProperties.Sensitivity;
		}

		private void AppendToNotes(string propertyName, string value, ImportContactProperties? mappedProperty)
		{
			if (mappedProperty == ImportContactProperties.Anniversary || mappedProperty == ImportContactProperties.Birthday)
			{
				return;
			}
			if (!this.propertyBag.ContainsKey(ImportContactProperties.Notes))
			{
				string value2 = this.BuildNotesString(propertyName, value);
				this.propertyBag[ImportContactProperties.Notes] = value2;
				return;
			}
			string text = (string)this.propertyBag[ImportContactProperties.Notes];
			string value3;
			if (mappedProperty == ImportContactProperties.Notes)
			{
				value3 = value + '\n' + text;
			}
			else
			{
				value3 = text + '\n' + this.BuildNotesString(propertyName, value);
			}
			this.propertyBag[ImportContactProperties.Notes] = value3;
		}

		private string BuildNotesString(string propertyName, string value)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0} : {1}", new object[]
			{
				propertyName,
				value
			});
			if (!this.appendedDataToNotes)
			{
				this.appendedDataToNotes = true;
				return ImportContactObject.FirstAppendToNotes + text;
			}
			return text;
		}

		private bool GetValueObject(ImportContactProperties property, string inputValue, CultureInfo culture, out object outputValue)
		{
			outputValue = null;
			switch (property)
			{
			case ImportContactProperties.Anniversary:
			case ImportContactProperties.Birthday:
				outputValue = this.GetDateTimeValue(inputValue, culture);
				goto IL_47;
			case ImportContactProperties.Categories:
			case ImportContactProperties.Children:
				outputValue = this.GetMultiValuedStrings(inputValue);
				goto IL_47;
			}
			outputValue = inputValue;
			IL_47:
			return outputValue != null && ImportContactXsoMapper.ValidatePropertyValue(property, outputValue);
		}

		private DateTime? GetDateTimeValue(string stringValue, CultureInfo culture)
		{
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-us");
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)culture.DateTimeFormat.Clone();
			dateTimeFormatInfo.Calendar = cultureInfo.DateTimeFormat.Calendar;
			DateTime value;
			if (DateTime.TryParse(stringValue, dateTimeFormatInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out value))
			{
				return new DateTime?(value);
			}
			return null;
		}

		private string[] GetMultiValuedStrings(string stringValue)
		{
			string[] array = stringValue.Split(new char[]
			{
				ImportContactObject.MultiValueSeparator
			});
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		private bool? GetBoolValue(string stringValue)
		{
			bool value;
			if (bool.TryParse(stringValue, out value))
			{
				return new bool?(value);
			}
			return null;
		}

		private const char NewLine = '\n';

		public static readonly string FirstAppendToNotes = string.Concat(new object[]
		{
			"-----------------------------",
			'\n',
			Strings.FirstAppendToNotes,
			":",
			'\n'
		});

		public static readonly char MultiValueSeparator = ';';

		private static readonly int propertyBagInitialSize = 10;

		private Dictionary<ImportContactProperties, object> propertyBag;

		private bool appendedDataToNotes;

		private int index;
	}
}
