using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct ModifyTableRow
	{
		public ModifyTableRow(ModifyTableFlags modifyTableFlags, PropertyValue[] propertyValues)
		{
			Util.ThrowOnNullArgument(propertyValues, "propertyValues");
			this.modifyTableFlags = modifyTableFlags;
			this.propertyValues = propertyValues;
		}

		public ModifyTableFlags ModifyTableFlags
		{
			get
			{
				return this.modifyTableFlags;
			}
		}

		public PropertyValue[] PropertyValues
		{
			get
			{
				if (this.propertyValues == null)
				{
					throw new ArgumentException("The propertyValues should not be null");
				}
				return this.propertyValues;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("ModifyTableFlags: {0}", this.modifyTableFlags));
			stringBuilder.AppendLine(string.Format("Properties[{0}] {{", this.PropertyValues.Length));
			foreach (PropertyValue propertyValue in this.PropertyValues)
			{
				stringBuilder.AppendLine(string.Format("  {{{0}}}", propertyValue));
			}
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}

		internal static ModifyTableRow Parse(Reader reader)
		{
			ModifyTableFlags modifyTableFlags = (ModifyTableFlags)reader.ReadByte();
			PropertyValue[] array = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
			return new ModifyTableRow(modifyTableFlags, array);
		}

		internal void Serialize(Writer writer, Encoding string8Encoding)
		{
			writer.WriteByte((byte)this.modifyTableFlags);
			writer.WriteCountAndPropertyValueList(this.PropertyValues, string8Encoding, WireFormatStyle.Rop);
		}

		internal void ResolveString8Values(Encoding string8Encoding)
		{
			foreach (PropertyValue propertyValue in this.PropertyValues)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
		}

		private readonly ModifyTableFlags modifyTableFlags;

		private readonly PropertyValue[] propertyValues;
	}
}
