using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ClassificationDefinitions;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class DataClassificationService
	{
		public static void PostGetListAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			List<DataRow> list = new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow["ClassificationType"].ToString() == "Entity")
				{
					list.Add(dataRow);
				}
				DataClassificationObjectId dataClassificationObjectId = (DataClassificationObjectId)dataRow["Identity"];
				dataRow["Identity"] = new Identity(dataClassificationObjectId);
			}
			Array.ForEach<DataRow>(list.ToArray(), delegate(DataRow r)
			{
				dataTable.Rows.Remove(r);
			});
		}

		public static void NewObjectPreAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint[] fingerprints = DataClassificationService.GetFingerprints((object[])inputrow["FingerprintNames"]);
			inputrow["Fingerprints"] = fingerprints;
			store.ModifiedColumns.Add("Fingerprints");
		}

		public static void LanguagesAddedPreAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			DataClassificationService.LanguageSetting languageSetting = (DataClassificationService.LanguageSetting)inputrow["CurrentLanguage"];
			inputrow["Locale"] = languageSetting.Locale;
			inputrow["Name"] = languageSetting.Name;
			inputrow["Description"] = languageSetting.Description;
			store.ModifiedColumns.Add("Locale");
			store.ModifiedColumns.Add("Name");
			store.ModifiedColumns.Add("Description");
		}

		public static void LanguagesRemovedPreAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			DataClassificationService.LanguageSetting languageSetting = (DataClassificationService.LanguageSetting)inputrow["CurrentLanguage"];
			inputrow["Locale"] = languageSetting.Locale;
			inputrow["Name"] = null;
			inputrow["Description"] = null;
			store.ModifiedColumns.Add("Locale");
			store.ModifiedColumns.Add("Name");
			store.ModifiedColumns.Add("Description");
		}

		private static Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint[] GetFingerprints(object[] fingerprints)
		{
			List<Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint> list = new List<Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint>();
			foreach (object obj in fingerprints)
			{
				list.Add(Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint.Parse(obj.ToString()));
			}
			return list.ToArray();
		}

		public static void GetForSDOPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["Fingerprints"]))
			{
				List<string> list = new List<string>();
				foreach (Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint fingerprint in ((MultiValuedProperty<Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint>)dataRow["Fingerprints"]))
				{
					string item = fingerprint.Description.Substring(fingerprint.Description.LastIndexOf("\\") + 1);
					list.Add(item);
				}
				dataRow["Fingerprints"] = list;
				dataTable.Rows[0]["FingerprintFileList"] = list.ToArray();
			}
		}

		public static void GetObjectPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["AllLocalizedNames"]))
			{
				List<DataClassificationService.LanguageSetting> list = new List<DataClassificationService.LanguageSetting>();
				Dictionary<CultureInfo, string> dictionary = (Dictionary<CultureInfo, string>)dataRow["AllLocalizedDescriptions"];
				foreach (KeyValuePair<CultureInfo, string> keyValuePair in ((Dictionary<CultureInfo, string>)dataRow["AllLocalizedNames"]))
				{
					DataClassificationService.LanguageSetting item = new DataClassificationService.LanguageSetting
					{
						Locale = keyValuePair.Key.ToString(),
						Language = keyValuePair.Key.DisplayName,
						Name = keyValuePair.Value,
						Description = dictionary[keyValuePair.Key],
						IsDefault = (keyValuePair.Key.ToString() == dataRow["DefaultCulture"].ToString())
					};
					list.Add(item);
				}
				if (list.Count > 0)
				{
					dataRow["AllLocalizedNamesList"] = list.ToArray();
				}
			}
			List<Microsoft.Exchange.Management.ControlPanel.Fingerprint> list2 = new List<Microsoft.Exchange.Management.ControlPanel.Fingerprint>();
			MultiValuedProperty<Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint> multiValuedProperty = (MultiValuedProperty<Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint>)dataRow["Fingerprints"];
			foreach (Microsoft.Exchange.Management.ClassificationDefinitions.Fingerprint print in multiValuedProperty)
			{
				list2.Add(new Microsoft.Exchange.Management.ControlPanel.Fingerprint(print));
			}
			dataRow["Fingerprints"] = list2.ToArray();
			store.ModifiedColumns.Add("Fingerprints");
		}

		public static DataClassificationService.LanguageSetting ConvertToLanguageSetting(object language)
		{
			return (DataClassificationService.LanguageSetting)language;
		}

		[DataContract]
		public class LanguageSetting
		{
			[DataMember]
			public string Locale { get; set; }

			[DataMember]
			public string Language { get; set; }

			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public string Description { get; set; }

			[DataMember]
			public bool IsDefault { get; set; }
		}
	}
}
