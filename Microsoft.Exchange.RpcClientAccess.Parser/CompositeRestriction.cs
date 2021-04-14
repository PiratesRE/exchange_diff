using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal abstract class CompositeRestriction : Restriction
	{
		internal CompositeRestriction(Restriction[] childRestrictions)
		{
			if (childRestrictions == null)
			{
				throw new ArgumentNullException("childRestrictions");
			}
			this.childRestrictions = childRestrictions;
		}

		protected static T InternalParse<T>(Reader reader, CompositeRestriction.Creator<T> creator, WireFormatStyle wireFormatStyle, uint depth) where T : CompositeRestriction
		{
			uint num = reader.ReadCountOrSize(wireFormatStyle);
			reader.CheckBoundary(num, 1U);
			Restriction[] array = new Restriction[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = Restriction.InternalParse(reader, wireFormatStyle, depth);
				num2++;
			}
			return creator(array);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteCountOrSize(this.childRestrictions.Length, wireFormatStyle);
			foreach (Restriction restriction in this.childRestrictions)
			{
				restriction.Serialize(writer, string8Encoding, wireFormatStyle);
			}
		}

		public Restriction[] ChildRestrictions
		{
			get
			{
				return this.childRestrictions;
			}
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			foreach (Restriction restriction in this.childRestrictions)
			{
				restriction.ResolveString8Values(string8Encoding);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [");
			if (this.childRestrictions != null)
			{
				for (int i = 0; i < this.childRestrictions.Length; i++)
				{
					stringBuilder.Append("[");
					if (this.childRestrictions[i] != null)
					{
						this.childRestrictions[i].AppendToString(stringBuilder);
					}
					stringBuilder.Append("]");
					if (i < this.childRestrictions.Length - 1)
					{
						stringBuilder.Append(", ");
					}
				}
			}
			stringBuilder.Append("]");
		}

		private readonly Restriction[] childRestrictions;

		protected delegate T Creator<T>(Restriction[] childRestriction) where T : CompositeRestriction;
	}
}
