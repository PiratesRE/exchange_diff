using System;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal class MsgStoragePropertyTypeRule
	{
		internal MsgStoragePropertyTypeRule(MsgStoragePropertyTypeRule.ReadFixedValueDelegate valueReader, MsgStoragePropertyTypeRule.WriteValueDelegate valueWriter)
		{
			this.fixedValueReader = valueReader;
			this.valueWriter = valueWriter;
			this.canOpenStream = false;
		}

		internal MsgStoragePropertyTypeRule(bool canOpenStream, MsgStoragePropertyTypeRule.ReadStreamedValueDelegate valueReader, MsgStoragePropertyTypeRule.WriteValueDelegate valueWriter)
		{
			this.streamedValueReader = valueReader;
			this.valueWriter = valueWriter;
			this.canOpenStream = canOpenStream;
		}

		internal MsgStoragePropertyTypeRule.ReadFixedValueDelegate ReadFixedValue
		{
			get
			{
				return this.fixedValueReader;
			}
		}

		internal MsgStoragePropertyTypeRule.ReadStreamedValueDelegate ReadStreamedValue
		{
			get
			{
				return this.streamedValueReader;
			}
		}

		internal MsgStoragePropertyTypeRule.WriteValueDelegate WriteValue
		{
			get
			{
				return this.valueWriter;
			}
		}

		internal bool IsFixedValue
		{
			get
			{
				return this.fixedValueReader != null;
			}
		}

		internal bool CanOpenStream
		{
			get
			{
				return this.canOpenStream;
			}
		}

		private MsgStoragePropertyTypeRule.WriteValueDelegate valueWriter;

		private MsgStoragePropertyTypeRule.ReadFixedValueDelegate fixedValueReader;

		private MsgStoragePropertyTypeRule.ReadStreamedValueDelegate streamedValueReader;

		private bool canOpenStream;

		internal delegate object ReadFixedValueDelegate(byte[] propertyStreamData, int propertyOffset);

		internal delegate object ReadStreamedValueDelegate(MsgSubStorageReader parser, MsgSubStorageReader.PropertyInfo propertyInfo);

		internal delegate void WriteValueDelegate(MsgSubStorageWriter writer, TnefPropertyTag propertyTag, object propertyValue);
	}
}
