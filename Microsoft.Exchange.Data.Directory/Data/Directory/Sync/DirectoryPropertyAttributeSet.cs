using System;
using System.Collections;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class DirectoryPropertyAttributeSet : DirectoryProperty
	{
		public AttributeSet[] Value
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

		public override IList GetValues()
		{
			if (this.Value != null)
			{
				return this.Value;
			}
			return DirectoryProperty.EmptyValues;
		}

		public sealed override void SetValues(IList values)
		{
			if (values == DirectoryProperty.EmptyValues)
			{
				this.Value = null;
				return;
			}
			this.Value = new AttributeSet[values.Count];
			values.CopyTo(this.Value, 0);
		}

		private AttributeSet[] valueField;
	}
}
