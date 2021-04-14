using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct Property
	{
		public Property(PropertyId id, PropertyValue value)
		{
			this.id = id;
			this.value = value;
		}

		public bool IsNull
		{
			get
			{
				return this.id == PropertyId.Null;
			}
		}

		public PropertyId Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public PropertyValue Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public void Set(PropertyId id, PropertyValue value)
		{
			this.id = id;
			this.value = value;
		}

		public override string ToString()
		{
			return this.id.ToString() + " = " + this.value.ToString();
		}

		private PropertyId id;

		private PropertyValue value;

		public static readonly Property Null = default(Property);
	}
}
