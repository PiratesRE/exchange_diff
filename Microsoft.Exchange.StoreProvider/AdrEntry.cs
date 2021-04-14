using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AdrEntry
	{
		internal unsafe AdrEntry(_AdrEntry* padrEntry)
		{
			SPropValue* pspva = padrEntry->pspva;
			this.propValues = new PropValue[padrEntry->cValues];
			uint num = 0U;
			while ((ulong)num < (ulong)((long)padrEntry->cValues))
			{
				this.propValues[(int)((UIntPtr)num)] = new PropValue(pspva + (ulong)num * (ulong)((long)sizeof(SPropValue)) / (ulong)sizeof(SPropValue));
				num += 1U;
			}
		}

		public PropValue[] Values
		{
			get
			{
				return this.propValues;
			}
			set
			{
				this.propValues = value;
			}
		}

		public AdrEntry(params PropValue[] propValues)
		{
			this.propValues = propValues;
		}

		public bool IsEqualTo(AdrEntry aeOther)
		{
			if (this.propValues.Length != aeOther.propValues.Length)
			{
				return false;
			}
			Dictionary<PropTag, PropValue> dictionary = new Dictionary<PropTag, PropValue>();
			foreach (PropValue value in aeOther.propValues)
			{
				dictionary.Add(value.PropTag, value);
			}
			foreach (PropValue propValue in this.propValues)
			{
				PropValue propOther = dictionary[propValue.PropTag];
				if (!propValue.IsEqualTo(propOther))
				{
					return false;
				}
			}
			return true;
		}

		internal int GetBytesToMarshal()
		{
			int num = _AdrEntry.SizeOf + 7 & -8;
			for (int i = 0; i < this.propValues.Length; i++)
			{
				num += this.propValues[i].GetBytesToMarshal();
			}
			return num;
		}

		internal unsafe void MarshalToNative(_AdrEntry* padrEntry, ref SPropValue* psprop, ref byte* pbExtra)
		{
			padrEntry->ulReserved1 = 0;
			padrEntry->cValues = this.propValues.Length;
			padrEntry->pspva = psprop;
			for (int i = 0; i < this.propValues.Length; i++)
			{
				this.propValues[i].MarshalToNative(psprop, ref pbExtra);
				psprop += (IntPtr)sizeof(SPropValue);
			}
		}

		internal PropValue[] propValues;
	}
}
