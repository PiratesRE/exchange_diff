using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class PropertyBinaryValueXML : PropertyValueBaseXML
	{
		[XmlText]
		public string StrValue
		{
			get
			{
				return Convert.ToBase64String(this.BinValue);
			}
			set
			{
				this.BinValue = Convert.FromBase64String(value);
			}
		}

		[XmlIgnore]
		public byte[] BinValue { get; set; }

		internal override object RawValue
		{
			get
			{
				return this.BinValue;
			}
		}

		internal Guid? AsGuid
		{
			get
			{
				if (this.BinValue != null && this.BinValue.Length == 16)
				{
					return new Guid?(new Guid(this.BinValue));
				}
				return null;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}", TraceUtils.DumpEntryId(this.BinValue));
		}

		internal override bool TryGetValue(ProviderPropertyDefinition pdef, out object result)
		{
			if (this.BinValue == null)
			{
				result = null;
				return true;
			}
			ADPropertyDefinition adpropertyDefinition = pdef as ADPropertyDefinition;
			IFormatProvider formatProvider = (adpropertyDefinition != null) ? adpropertyDefinition.FormatProvider : null;
			result = null;
			Exception ex = null;
			if (!ADValueConvertor.TryConvertValueFromBinary(this.BinValue, pdef.Type, formatProvider, out result, out ex))
			{
				MrsTracer.Common.Warning("Failed to convert {0} from binary ({1} bytes): {2}", new object[]
				{
					pdef.Name,
					this.BinValue.Length,
					CommonUtils.FullExceptionMessage(ex)
				});
				return false;
			}
			return true;
		}

		internal override bool HasValue()
		{
			return this.BinValue != null && this.BinValue.Length > 0;
		}
	}
}
