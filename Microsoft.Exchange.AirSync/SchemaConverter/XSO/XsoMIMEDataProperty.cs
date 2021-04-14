using System;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoMIMEDataProperty : XsoProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		public XsoMIMEDataProperty(PropertyType type) : base(null, type)
		{
		}

		public XsoMIMEDataProperty() : base(null)
		{
		}

		public Stream MIMEData
		{
			get
			{
				if (this.mimeData == null)
				{
					this.mimeData = BodyUtility.ConvertItemToMime(base.XsoItem);
				}
				return this.mimeData;
			}
			set
			{
				throw new NotImplementedException("set_MIMEData");
			}
		}

		public long MIMESize { get; set; }

		public bool IsOnSMIMEMessage
		{
			get
			{
				MessageItem messageItem = base.XsoItem as MessageItem;
				return messageItem != null && ObjectClass.IsSmime(messageItem.ClassName);
			}
		}

		public override void Unbind()
		{
			this.mimeData = null;
			base.Unbind();
		}

		private Stream mimeData;
	}
}
