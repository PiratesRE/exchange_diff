using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Text;

namespace Microsoft.Exchange.EdgeSync
{
	internal class ExSearchResultEntry
	{
		public ExSearchResultEntry(string distinguishedName, DirectoryAttributeCollection attributes)
		{
			if (string.IsNullOrEmpty(distinguishedName))
			{
				throw new ArgumentNullException("distinguishedName");
			}
			this.attributes = new Dictionary<string, DirectoryAttribute>(attributes.Count, StringComparer.OrdinalIgnoreCase);
			foreach (object obj in attributes)
			{
				DirectoryAttribute directoryAttribute = (DirectoryAttribute)obj;
				this.attributes.Add(directoryAttribute.Name, directoryAttribute);
			}
			this.distinguishedName = distinguishedName;
		}

		public ExSearchResultEntry(SearchResultEntry baseEntry)
		{
			if (baseEntry == null)
			{
				throw new ArgumentNullException("baseEntry");
			}
			if (string.IsNullOrEmpty(baseEntry.DistinguishedName))
			{
				throw new InvalidOperationException("baseEntry DistinguishedName can't be null or empty");
			}
			this.attributes = new Dictionary<string, DirectoryAttribute>(baseEntry.Attributes.Count);
			foreach (object obj in baseEntry.Attributes)
			{
				DirectoryAttribute directoryAttribute = (DirectoryAttribute)((DictionaryEntry)obj).Value;
				if (!directoryAttribute.Name.Equals("instanceType", StringComparison.OrdinalIgnoreCase))
				{
					this.attributes.Add(directoryAttribute.Name, directoryAttribute);
				}
			}
			this.distinguishedName = baseEntry.DistinguishedName;
		}

		public Dictionary<string, DirectoryAttribute> Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.distinguishedName;
			}
		}

		public string ObjectClass
		{
			get
			{
				DirectoryAttribute attribute = this.GetAttribute("objectClass");
				if (attribute != null)
				{
					return (string)attribute[attribute.Count - 1];
				}
				return null;
			}
		}

		public bool IsDeleted
		{
			get
			{
				return ExSearchResultEntry.IsDeletedDN(this.distinguishedName);
			}
		}

		public bool IsNew
		{
			get
			{
				return this.attributes.ContainsKey("whenCreated");
			}
		}

		public bool IsRenamed
		{
			get
			{
				return this.attributes.ContainsKey("name");
			}
		}

		public static bool IsDeletedDN(string distinguishedName)
		{
			int num = distinguishedName.IndexOf("\\0ADEL", StringComparison.OrdinalIgnoreCase);
			return num != -1;
		}

		public static string GetAsciiStringValueOfAttribute(object attrValue, string attrName)
		{
			string text = attrValue as string;
			if (text == null)
			{
				byte[] array = attrValue as byte[];
				if (array == null)
				{
					throw new ArgumentException("The value of attribute " + attrName + " is neither string nor byte[]", "attrValue");
				}
				text = Encoding.ASCII.GetString(array);
			}
			return text;
		}

		public bool MultiValuedAttributeContain(string attributeName, string valueToFind)
		{
			DirectoryAttribute directoryAttribute;
			if (!this.Attributes.TryGetValue(attributeName, out directoryAttribute))
			{
				throw new ArgumentException("The entry should contain the attribute " + attributeName);
			}
			foreach (object obj in directoryAttribute)
			{
				if (obj != null)
				{
					string asciiStringValueOfAttribute = ExSearchResultEntry.GetAsciiStringValueOfAttribute(obj, attributeName);
					if (asciiStringValueOfAttribute != null && asciiStringValueOfAttribute.Equals(valueToFind, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsCollisionObject(out int index, out int length)
		{
			index = -1;
			length = 0;
			index = this.distinguishedName.IndexOf("\\0ACNF", StringComparison.OrdinalIgnoreCase);
			if (index != -1)
			{
				length = "\\0ACNF".Length;
				return true;
			}
			return false;
		}

		public ExSearchResultEntry Clone()
		{
			return this.Clone(this.distinguishedName);
		}

		public ExSearchResultEntry Clone(string distinguishedName)
		{
			DirectoryAttributeCollection directoryAttributeCollection = new DirectoryAttributeCollection();
			foreach (KeyValuePair<string, DirectoryAttribute> keyValuePair in this.attributes)
			{
				DirectoryAttribute value = keyValuePair.Value;
				DirectoryAttribute directoryAttribute = new DirectoryAttribute();
				directoryAttribute.Name = value.Name;
				foreach (object obj in value)
				{
					if (obj is byte[])
					{
						byte[] array = new byte[((byte[])obj).Length];
						Buffer.BlockCopy((byte[])obj, 0, array, 0, array.Length);
						directoryAttribute.Add(array);
					}
					else if (obj is string)
					{
						string value2 = string.Copy((string)obj);
						directoryAttribute.Add(value2);
					}
					else
					{
						if (!(obj is Uri))
						{
							throw new NotSupportedException("Type " + obj.GetType() + " is not supported");
						}
						Uri value3 = new Uri(((Uri)obj).OriginalString);
						directoryAttribute.Add(value3);
					}
				}
				directoryAttributeCollection.Add(directoryAttribute);
			}
			return new ExSearchResultEntry(distinguishedName, directoryAttributeCollection);
		}

		public DirectoryAttribute GetAttribute(string name)
		{
			DirectoryAttribute directoryAttribute;
			if (!this.attributes.TryGetValue(name, out directoryAttribute) || (directoryAttribute != null && directoryAttribute.Count == 0))
			{
				directoryAttribute = null;
			}
			return directoryAttribute;
		}

		public Guid GetObjectGuid()
		{
			DirectoryAttribute attribute = this.GetAttribute("objectGUID");
			if (attribute == null)
			{
				throw new InvalidOperationException("AD entry does not contain objectGUID");
			}
			return new Guid((byte[])attribute[0]);
		}

		private const string DeletedObjectSig = "\\0ADEL";

		private const string CollisionObjectSig = "\\0ACNF";

		private readonly Dictionary<string, DirectoryAttribute> attributes;

		private readonly string distinguishedName;
	}
}
