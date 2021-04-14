using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoMultiValuedStringProperty : XsoProperty, IMultivaluedProperty<string>, IProperty, IEnumerable<string>, IEnumerable
	{
		public XsoMultiValuedStringProperty(StorePropertyDefinition propertyDef) : base(propertyDef)
		{
		}

		public int Count
		{
			get
			{
				return ((string[])base.XsoItem.TryGetProperty(base.PropertyDef)).Length;
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			object obj = base.XsoItem.TryGetProperty(base.PropertyDef);
			if (obj is PropertyError)
			{
				bool flag = false;
				if (flag)
				{
					yield return "Business";
				}
			}
			else
			{
				string[] strs = (string[])obj;
				for (int idx = 0; idx < strs.Length; idx++)
				{
					yield return strs[idx];
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			IMultivaluedProperty<string> multivaluedProperty = srcProperty as IMultivaluedProperty<string>;
			if (multivaluedProperty == null)
			{
				throw new UnexpectedTypeException("IMultivaluedProperty<string>", srcProperty);
			}
			string[] array = new string[multivaluedProperty.Count];
			int num = 0;
			foreach (string text in multivaluedProperty)
			{
				array[num++] = text;
			}
			base.XsoItem[base.PropertyDef] = array;
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			base.XsoItem[base.PropertyDef] = new string[0];
		}
	}
}
