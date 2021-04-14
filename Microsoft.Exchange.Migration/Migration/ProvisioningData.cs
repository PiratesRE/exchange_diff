using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal abstract class ProvisioningData : IProvisioningData
	{
		public Dictionary<string, object> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public ProvisioningType ProvisioningType
		{
			get
			{
				return this.GetEnumValueOrDefault<ProvisioningType>("ProvisioningType", ProvisioningType.Unknown);
			}
			set
			{
				this["ProvisioningType"] = value;
			}
		}

		public ProvisioningAction Action
		{
			get
			{
				return this.GetEnumValueOrDefault<ProvisioningAction>("ProvisioningAction", ProvisioningAction.CreateNew);
			}
			set
			{
				this["ProvisioningAction"] = value;
			}
		}

		public int Version
		{
			get
			{
				object obj = this["Version"];
				if (obj != null)
				{
					return (int)obj;
				}
				return 1;
			}
			set
			{
				this["Version"] = value;
			}
		}

		public ProvisioningComponent Component
		{
			get
			{
				return this.GetEnumValueOrDefault<ProvisioningComponent>("ProvisioningComponent", ProvisioningComponent.BulkProvision);
			}
			set
			{
				this["ProvisioningComponent"] = value;
			}
		}

		public string Identity
		{
			get
			{
				return (string)this["Identity"];
			}
			set
			{
				this["Identity"] = value;
			}
		}

		public string Organization
		{
			get
			{
				return (string)this["Organization"];
			}
			set
			{
				this["Organization"] = value;
			}
		}

		public bool IsBPOS
		{
			get
			{
				object obj = this["IsBPOS"];
				return obj != null && (bool)obj;
			}
			set
			{
				this["IsBPOS"] = value;
			}
		}

		protected object this[string key]
		{
			get
			{
				object result;
				if (!this.Parameters.TryGetValue(key, out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				this.Parameters[key] = value;
			}
		}

		protected object this[PropertyDefinition cmdletParameterDefinition]
		{
			get
			{
				return this[cmdletParameterDefinition.Name];
			}
			set
			{
				this[cmdletParameterDefinition.Name] = value;
			}
		}

		public static IProvisioningData FromPersistableDictionary(PersistableDictionary dictionary)
		{
			IProvisioningData provisioningData = null;
			string text = (string)((IDictionary)dictionary)["ProvisioningType"];
			try
			{
				ProvisioningType type = (ProvisioningType)Enum.Parse(typeof(ProvisioningType), text);
				provisioningData = ProvisioningDataFactory.Create(type);
				foreach (object obj in dictionary)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					provisioningData.Parameters[(string)dictionaryEntry.Key] = dictionaryEntry.Value;
				}
			}
			catch (ArgumentException)
			{
				throw new MigrationDataCorruptionException("Unable to create from PersistableDictionary ProvisioningData of type " + text);
			}
			return provisioningData;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, object> keyValuePair in this.Parameters)
			{
				stringBuilder.Append(string.Format("{0}:", keyValuePair.Key));
				if (string.Compare(keyValuePair.Key, "Password", StringComparison.OrdinalIgnoreCase) != 0)
				{
					stringBuilder.AppendLine(keyValuePair.Value.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		public PersistableDictionary ToPersistableDictionary()
		{
			PersistableDictionary persistableDictionary = new PersistableDictionary();
			foreach (KeyValuePair<string, object> keyValuePair in this.Parameters)
			{
				object obj = keyValuePair.Value;
				if (obj.GetType().IsEnum)
				{
					obj = obj.ToString();
				}
				persistableDictionary.Add(keyValuePair.Key, obj);
			}
			return persistableDictionary;
		}

		private T GetEnumValueOrDefault<T>(string propertyName, T defaultValue)
		{
			object obj = this[propertyName];
			if (obj == null || !typeof(T).IsEnum)
			{
				return defaultValue;
			}
			if (obj.GetType() == typeof(T))
			{
				return (T)((object)obj);
			}
			T result = defaultValue;
			try
			{
				result = (T)((object)Enum.Parse(typeof(T), (string)obj));
			}
			catch (ArgumentException exception)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception, string.Format("Unable to parse enum value {0} for parameter {1}, using default value {2} instead.", obj, propertyName, defaultValue.ToString()), new object[0]);
			}
			return result;
		}

		public const string IdentityParameterName = "Identity";

		public const string OrganizationParameterName = "Organization";

		public const string ProvisioningTypeParameterName = "ProvisioningType";

		public const string ProvisioningActionParameterName = "ProvisioningAction";

		public const string ProvisioningComponentParameterName = "ProvisioningComponent";

		public const string PasswordParameterName = "Password";

		public const string VersionParameterName = "Version";

		public const int CurrentVersion = 1;

		public const string IsBPOSParameterName = "IsBPOS";

		private Dictionary<string, object> parameters = new Dictionary<string, object>();
	}
}
