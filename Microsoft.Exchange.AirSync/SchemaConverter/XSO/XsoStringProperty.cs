using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoStringProperty : XsoProperty, IStringProperty, IProperty
	{
		public XsoStringProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public XsoStringProperty(StorePropertyDefinition propertyDef, PropertyType type) : base(propertyDef, type)
		{
		}

		public XsoStringProperty(StorePropertyDefinition propertyDef, PropertyType type, params PropertyDefinition[] prefechProperties) : base(propertyDef, type, prefechProperties)
		{
		}

		public virtual string StringData
		{
			get
			{
				if (base.State == PropertyState.Stream)
				{
					Stream stream = null;
					try
					{
						stream = base.XsoItem.OpenPropertyStream(base.PropertyDef, PropertyOpenMode.ReadOnly);
						if (stream.CanSeek)
						{
							int num = (int)((stream.Length < 32768L) ? stream.Length : 32768L);
							byte[] array = new byte[num];
							for (int i = 0; i < num; i += stream.Read(array, i, num - i))
							{
							}
							char[] chars = Encoding.Unicode.GetChars(array);
							return new string(chars);
						}
					}
					catch (Exception innerException)
					{
						throw new ConversionException("Failed to open propertyStream for " + base.PropertyDef.ToString(), innerException);
					}
					finally
					{
						if (stream != null)
						{
							stream.Dispose();
						}
					}
				}
				object obj;
				try
				{
					obj = base.XsoItem.TryGetProperty(base.PropertyDef);
					if (obj is PropertyError && ((PropertyError)obj).PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
					{
						base.XsoItem.Load(new PropertyDefinition[]
						{
							base.PropertyDef
						});
						obj = base.XsoItem.TryGetProperty(base.PropertyDef);
					}
				}
				catch (PropertyErrorException ex)
				{
					if (ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
					{
						base.XsoItem.Load(new PropertyDefinition[]
						{
							base.PropertyDef
						});
					}
					obj = base.XsoItem.TryGetProperty(base.PropertyDef);
				}
				return (string)obj;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = ((IStringProperty)srcProperty).StringData;
		}
	}
}
