using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.CommonTypes
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ApplicationDataTypeResponseFlag
	{
		internal ApplicationDataTypeResponseFlag()
		{
			this.itemsElementNameField = new List<ItemsChoiceType2>();
			this.itemsField = new List<object>();
		}

		[XmlChoiceIdentifier("ItemsElementName")]
		internal List<object> Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlIgnore]
		internal List<ItemsChoiceType2> ItemsElementName
		{
			get
			{
				return this.itemsElementNameField;
			}
			set
			{
				this.itemsElementNameField = value;
			}
		}

		private List<object> itemsField;

		private List<ItemsChoiceType2> itemsElementNameField;
	}
}
