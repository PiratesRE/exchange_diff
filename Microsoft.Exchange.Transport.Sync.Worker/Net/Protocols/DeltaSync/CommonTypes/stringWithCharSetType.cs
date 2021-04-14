using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.CommonTypes
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class stringWithCharSetType
	{
		internal stringWithCharSetType()
		{
			this.encodingField = "0";
		}

		internal string charset
		{
			get
			{
				return this.charsetField;
			}
			set
			{
				this.charsetField = value;
			}
		}

		[DefaultValue("0")]
		internal string encoding
		{
			get
			{
				return this.encodingField;
			}
			set
			{
				this.encodingField = value;
			}
		}

		[XmlText]
		internal string Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		private string charsetField;

		private string encodingField;

		private string valueField;
	}
}
