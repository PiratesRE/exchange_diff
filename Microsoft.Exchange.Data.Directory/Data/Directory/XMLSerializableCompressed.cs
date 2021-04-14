using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public class XMLSerializableCompressed<T> : XMLSerializableBase where T : class
	{
		public XMLSerializableCompressed()
		{
			this.internalValue = default(T);
			this.internalData = null;
		}

		public XMLSerializableCompressed(T value)
		{
			this.internalValue = value;
			this.internalData = null;
		}

		public static implicit operator T(XMLSerializableCompressed<T> proxy)
		{
			return proxy.Value;
		}

		[XmlIgnore]
		public T Value
		{
			get
			{
				T result;
				lock (this.Locker)
				{
					if (this.internalData != null && this.internalValue == null)
					{
						string serializedXML = this.DecompressString(this.internalData);
						this.internalValue = XMLSerializableBase.Deserialize<T>(serializedXML, this.XmlRawProperty);
					}
					result = this.internalValue;
				}
				return result;
			}
			set
			{
				lock (this.Locker)
				{
					this.internalValue = value;
					this.internalData = null;
				}
			}
		}

		[XmlElement("Data")]
		public byte[] Data
		{
			get
			{
				byte[] result;
				lock (this.Locker)
				{
					if (this.internalValue != null && this.internalData == null)
					{
						this.internalData = StringUtil.CompressString(XMLSerializableBase.Serialize(this.internalValue, false));
					}
					result = this.internalData;
				}
				return result;
			}
			set
			{
				lock (this.Locker)
				{
					this.internalData = value;
					this.internalValue = default(T);
				}
			}
		}

		protected virtual PropertyDefinition XmlRawProperty
		{
			get
			{
				return XMLSerializableBase.ConfigurationXmlRawProperty();
			}
		}

		public string DecompressString(byte[] data)
		{
			string result;
			try
			{
				result = StringUtil.DecompressString(data);
			}
			catch (InvalidDataException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(this.XmlRawProperty.Name, ex.Message), this.XmlRawProperty, Convert.ToBase64String(data)), ex);
			}
			catch (EndOfStreamException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(this.XmlRawProperty.Name, ex2.Message), this.XmlRawProperty, Convert.ToBase64String(data)), ex2);
			}
			return result;
		}

		public override string ToString()
		{
			lock (this.Locker)
			{
				if (this.internalValue != null)
				{
					return this.internalValue.ToString();
				}
			}
			return null;
		}

		private readonly object Locker = new object();

		private T internalValue;

		private byte[] internalData;
	}
}
