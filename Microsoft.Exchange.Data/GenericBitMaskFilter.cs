using System;
using System.Reflection;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal abstract class GenericBitMaskFilter : SinglePropertyFilter
	{
		public GenericBitMaskFilter(PropertyDefinition property, ulong mask) : base(property)
		{
			if (property.Type != typeof(short) && property.Type != typeof(int) && property.Type != typeof(long) && property.Type != typeof(ushort) && property.Type != typeof(uint) && property.Type != typeof(ulong) && !typeof(Enum).GetTypeInfo().IsAssignableFrom(property.Type.GetTypeInfo()))
			{
				throw new ArgumentOutOfRangeException(DataStrings.ExceptionBitMaskNotSupported(property.Name, property.Type));
			}
			this.mask = mask;
		}

		public ulong Mask
		{
			get
			{
				return this.mask;
			}
		}

		public override bool Equals(object obj)
		{
			GenericBitMaskFilter genericBitMaskFilter = obj as GenericBitMaskFilter;
			return genericBitMaskFilter != null && this.mask == genericBitMaskFilter.mask && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (int)this.mask;
		}

		private readonly ulong mask;
	}
}
