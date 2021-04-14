using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data
{
	internal class RegistrySession : IConfigDataProvider
	{
		public RegistrySession() : this(false)
		{
		}

		public RegistrySession(bool ignoreErrorsOnRead) : this(ignoreErrorsOnRead, Registry.LocalMachine)
		{
		}

		public RegistrySession(bool ignoreErrorsOnRead, RegistryKey root)
		{
			ArgumentValidator.ThrowIfNull("root", root);
			this.RootKey = root;
			this.IgnoreReadErrors = ignoreErrorsOnRead;
		}

		public RegistryKey RootKey { get; private set; }

		public bool IgnoreReadErrors { get; private set; }

		public T Read<T>() where T : RegistryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			RegistryObject registryObject = t;
			this.ReadObject(registryObject.RegistrySchema.DefaultRegistryKeyPath, registryObject.RegistrySchema.DefaultName, this.IgnoreReadErrors, ref registryObject);
			return t;
		}

		public T Read<T>(RegistryObjectId identity) where T : RegistryObject, new()
		{
			T t = Activator.CreateInstance<T>();
			RegistryObject registryObject = t;
			this.ReadObject(identity.RegistryKeyPath ?? registryObject.RegistrySchema.DefaultRegistryKeyPath, identity.Name ?? registryObject.RegistrySchema.DefaultName, this.IgnoreReadErrors, ref registryObject);
			return t;
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
			RegistryObject registryObject = t as RegistryObject;
			RegistryObjectId registryObjectId;
			if (identity != null)
			{
				registryObjectId = (identity as RegistryObjectId);
			}
			else
			{
				registryObjectId = new RegistryObjectId(registryObject.RegistrySchema.DefaultRegistryKeyPath, registryObject.RegistrySchema.DefaultName);
			}
			this.ReadObject(registryObjectId.RegistryKeyPath, registryObjectId.Name, this.IgnoreReadErrors, ref registryObject);
			return t;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			if (filter != null || !deepSearch || sortBy != null)
			{
				throw new NotSupportedException();
			}
			RegistryObjectId registryId = rootId as RegistryObjectId;
			List<IConfigurable> list = new List<IConfigurable>();
			foreach (T t in this.Find<T>(registryId))
			{
				list.Add(t);
			}
			return list.ToArray();
		}

		public T[] Find<T>(RegistryObjectId registryId) where T : IConfigurable, new()
		{
			string[] array = null;
			using (RegistryKey registryKey = this.RootKey.OpenSubKey(registryId.RegistryKeyPath, true))
			{
				if (registryKey == null)
				{
					return new T[0];
				}
				array = registryKey.GetSubKeyNames();
			}
			List<T> list = new List<T>();
			if (array != null)
			{
				foreach (string folderName in array)
				{
					T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
					RegistryObject registryObject = t as RegistryObject;
					this.ReadObject(registryId.RegistryKeyPath, folderName, this.IgnoreReadErrors, ref registryObject);
					list.Add(t);
				}
			}
			return list.ToArray();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			throw new NotImplementedException();
		}

		public void Save(IConfigurable instance)
		{
			RegistryObject registryObject = instance as RegistryObject;
			RegistryObjectId registryObjectId = registryObject.Identity as RegistryObjectId;
			string folderPath = (registryObjectId != null) ? registryObjectId.RegistryKeyPath : registryObject.RegistrySchema.DefaultRegistryKeyPath;
			string text = (registryObjectId != null) ? registryObjectId.Name : registryObject.RegistrySchema.DefaultName;
			using (RegistryKey registryKey = this.CreateRegistryPathIfMissing(folderPath))
			{
				if (!registryKey.GetSubKeyNames().Contains(text, StringComparer.OrdinalIgnoreCase))
				{
					RegistryWriter.Instance.CreateSubKey(registryKey, text);
				}
				ObjectState objectState = instance.ObjectState;
				foreach (PropertyDefinition propertyDefinition in registryObject.ObjectSchema.AllProperties)
				{
					SimpleProviderPropertyDefinition simpleProviderPropertyDefinition = (SimpleProviderPropertyDefinition)propertyDefinition;
					if (!simpleProviderPropertyDefinition.IsCalculated && !this.excludedPersistentProperties.Contains(simpleProviderPropertyDefinition))
					{
						if ((registryObject[simpleProviderPropertyDefinition] != null && registryObject[simpleProviderPropertyDefinition].Equals(simpleProviderPropertyDefinition.DefaultValue)) || (registryObject[simpleProviderPropertyDefinition] == null && simpleProviderPropertyDefinition.DefaultValue == null))
						{
							RegistryWriter.Instance.DeleteValue(registryKey, text, simpleProviderPropertyDefinition.Name);
						}
						else
						{
							RegistryWriter.Instance.SetValue(registryKey, text, simpleProviderPropertyDefinition.Name, registryObject[simpleProviderPropertyDefinition], RegistryValueKind.String);
						}
					}
				}
			}
		}

		public void Delete(IConfigurable instance)
		{
			RegistryObject registryObject = instance as RegistryObject;
			RegistryObjectId registryObjectId = registryObject.Identity as RegistryObjectId;
			using (RegistryKey registryKey = this.RootKey.OpenSubKey(registryObjectId.RegistryKeyPath, true))
			{
				if (registryKey != null)
				{
					foreach (PropertyDefinition propertyDefinition in registryObject.ObjectSchema.AllProperties)
					{
						SimpleProviderPropertyDefinition simpleProviderPropertyDefinition = (SimpleProviderPropertyDefinition)propertyDefinition;
						if (!this.excludedPersistentProperties.Contains(simpleProviderPropertyDefinition))
						{
							RegistryWriter.Instance.DeleteValue(registryKey, registryObjectId.Name, simpleProviderPropertyDefinition.Name);
						}
					}
					bool flag = false;
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(registryObjectId.Name, false))
					{
						if (registryKey2.GetValueNames().Length == 0 && registryKey2.GetSubKeyNames().Length == 0)
						{
							flag = true;
						}
					}
					if (flag)
					{
						registryKey.DeleteSubKey(registryObjectId.Name, true);
					}
				}
			}
		}

		public string Source
		{
			get
			{
				return base.GetType().Name;
			}
		}

		private RegistryKey CreateRegistryPathIfMissing(string folderPath)
		{
			RegistryKey registryKey = this.RootKey.OpenSubKey(folderPath, true);
			if (registryKey != null)
			{
				return registryKey;
			}
			return this.RootKey.CreateSubKey(folderPath);
		}

		private void ReadObject(string folderPath, string folderName, ref RegistryObject instance)
		{
			RegistryObjectSchema registrySchema = instance.RegistrySchema;
			using (RegistryKey registryKey = this.RootKey.OpenSubKey(folderPath))
			{
				if (registryKey == null || !registryKey.GetSubKeyNames().Contains(folderName, StringComparer.OrdinalIgnoreCase))
				{
					return;
				}
				foreach (PropertyDefinition propertyDefinition in registrySchema.AllProperties)
				{
					SimpleProviderPropertyDefinition simpleProviderPropertyDefinition = (SimpleProviderPropertyDefinition)propertyDefinition;
					if (!simpleProviderPropertyDefinition.IsCalculated && !this.excludedPersistentProperties.Contains(simpleProviderPropertyDefinition))
					{
						object obj = RegistryReader.Instance.GetValue<object>(registryKey, folderName, simpleProviderPropertyDefinition.Name, simpleProviderPropertyDefinition.DefaultValue);
						try
						{
							obj = ValueConvertor.ConvertValue(obj, simpleProviderPropertyDefinition.Type, null);
						}
						catch (Exception ex)
						{
							instance.AddValidationError(new PropertyValidationError(DataStrings.ErrorCannotConvertFromString(obj as string, simpleProviderPropertyDefinition.Type.Name, ex.Message), simpleProviderPropertyDefinition, obj));
							continue;
						}
						try
						{
							instance[simpleProviderPropertyDefinition] = obj;
						}
						catch (DataValidationException ex2)
						{
							instance.AddValidationError(ex2.Error);
						}
					}
				}
			}
			instance.propertyBag[SimpleProviderObjectSchema.Identity] = new RegistryObjectId(folderPath, folderName);
			instance.ResetChangeTracking(true);
		}

		private void ReadObject(string folderPath, string folderName, bool ignoreErrors, ref RegistryObject instance)
		{
			if (!this.IgnoreReadErrors)
			{
				this.ReadObject(folderPath, folderName, ref instance);
				return;
			}
			try
			{
				this.ReadObject(folderPath, folderName, ref instance);
			}
			catch (IOException)
			{
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		private readonly ProviderPropertyDefinition[] excludedPersistentProperties = new ProviderPropertyDefinition[]
		{
			SimpleProviderObjectSchema.Identity,
			SimpleProviderObjectSchema.ObjectState,
			SimpleProviderObjectSchema.ExchangeVersion
		};
	}
}
