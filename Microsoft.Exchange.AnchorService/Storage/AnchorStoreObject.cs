using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AnchorStoreObject : DisposeTrackableBase, IAnchorStoreObject, IDisposable, IPropertyBag, IReadOnlyPropertyBag
	{
		public StoreObjectId Id
		{
			get
			{
				return this.StoreObject.StoreObjectId;
			}
		}

		public ExDateTime CreationTime
		{
			get
			{
				return this.StoreObject.CreationTime;
			}
		}

		public abstract string Name { get; }

		protected AnchorContext AnchorContext { get; set; }

		protected abstract StoreObject StoreObject { get; }

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.GetProperty(propertyDefinition);
			}
			set
			{
				this.StoreObject[propertyDefinition] = value;
			}
		}

		public virtual void OpenAsReadWrite()
		{
		}

		public abstract void Save(SaveMode saveMode);

		public virtual void LoadMessageIdProperties()
		{
			this.Load(AnchorStoreObject.IdPropertyDefinition);
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			return this.StoreObject.GetProperties(propertyDefinitionArray);
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			this.StoreObject.SetProperties(propertyDefinitionArray, propertyValuesArray);
		}

		public void Delete(PropertyDefinition propertyDefinition)
		{
			this.StoreObject.Delete(propertyDefinition);
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = this.TryGetProperty(propertyDefinition);
			PropertyError propertyError = obj as PropertyError;
			if (obj == null || (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound))
			{
				return defaultValue;
			}
			if (propertyError != null)
			{
				throw PropertyError.ToException(new PropertyError[]
				{
					(PropertyError)obj
				});
			}
			return (T)((object)obj);
		}

		public void Load(ICollection<PropertyDefinition> properties)
		{
			this.StoreObject.Load(properties);
		}

		private object GetProperty(PropertyDefinition propertyDefinition)
		{
			object obj = this.TryGetProperty(propertyDefinition);
			if (obj == null)
			{
				throw PropertyError.ToException(new PropertyError[]
				{
					new PropertyError(propertyDefinition, PropertyErrorCode.NullValue)
				});
			}
			if (PropertyError.IsPropertyError(obj))
			{
				throw PropertyError.ToException(new PropertyError[]
				{
					(PropertyError)obj
				});
			}
			return obj;
		}

		private object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			object obj = this.StoreObject.TryGetProperty(propertyDefinition);
			if (propertyDefinition.Type == typeof(string) && PropertyError.IsPropertyValueTooBig(obj))
			{
				this.AnchorContext.Logger.Log(MigrationEventType.Verbose, "AnchorStoreObject.TryGetProperty: reading {0} as a stream", new object[]
				{
					propertyDefinition
				});
				using (Stream stream = this.StoreObject.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
				{
					if (stream.Length > 131072L)
					{
						throw new MigrationDataCorruptionException(string.Format("size of property {0} too large {1}", propertyDefinition, stream.Length));
					}
					int num = (int)stream.Length;
					byte[] array = new byte[num];
					int i = 0;
					while (i < num)
					{
						int num2 = stream.Read(array, i, num - i);
						i += num2;
						if (num2 <= 0)
						{
							break;
						}
					}
					if (i != num)
					{
						throw new MigrationDataCorruptionException(string.Format("size of property {0} inconsistent, expected {1}, found {2}", propertyDefinition, num, i));
					}
					UnicodeEncoding unicodeEncoding = new UnicodeEncoding(false, true, true);
					try
					{
						obj = unicodeEncoding.GetString(array);
					}
					catch (ArgumentException innerException)
					{
						throw new MigrationDataCorruptionException(string.Format("couldn't decode bytes to utf16 for property {0}", propertyDefinition), innerException);
					}
				}
			}
			return obj;
		}

		private const int MaxPropertySize = 131072;

		internal static readonly PropertyDefinition[] IdPropertyDefinition = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.CreationTime
		};
	}
}
