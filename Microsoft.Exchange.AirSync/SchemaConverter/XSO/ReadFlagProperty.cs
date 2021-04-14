using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class ReadFlagProperty : PropertyBase, IBooleanProperty, IProperty
	{
		public ReadFlagProperty(bool isRead)
		{
			this.isRead = isRead;
		}

		public bool BooleanData
		{
			get
			{
				return this.isRead;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"( Base: ",
				base.ToString(),
				", IsRead: ",
				this.isRead,
				", state: ",
				base.State,
				")"
			});
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			throw new InvalidOperationException("ReadFlagProperty is read-only.");
		}

		private bool isRead;
	}
}
