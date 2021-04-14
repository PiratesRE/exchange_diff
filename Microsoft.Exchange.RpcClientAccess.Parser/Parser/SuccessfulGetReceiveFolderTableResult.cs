using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetReceiveFolderTableResult : RopResult
	{
		internal SuccessfulGetReceiveFolderTableResult(PropertyValue[][] rowValues) : base(RopId.GetReceiveFolderTable, ErrorCode.None, null)
		{
			Util.ThrowOnNullArgument(rowValues, "rowValues");
			this.rows = new PropertyRow[rowValues.Length];
			for (long num = 0L; num < (long)rowValues.Length; num += 1L)
			{
				checked
				{
					this.rows[(int)((IntPtr)num)] = new PropertyRow(SuccessfulGetReceiveFolderTableResult.columns, rowValues[(int)((IntPtr)num)]);
				}
			}
		}

		internal SuccessfulGetReceiveFolderTableResult(Reader reader, Encoding string8Encoding) : base(reader)
		{
			uint num = reader.ReadUInt32();
			if (num == 0U)
			{
				this.rows = Array<PropertyRow>.Empty;
				return;
			}
			uint elementSize = 13U;
			reader.CheckBoundary(num, elementSize);
			this.rows = new PropertyRow[num];
			for (long num2 = 0L; num2 < (long)((ulong)num); num2 += 1L)
			{
				checked
				{
					this.rows[(int)((IntPtr)num2)] = PropertyRow.Parse(reader, SuccessfulGetReceiveFolderTableResult.columns, WireFormatStyle.Rop);
					this.rows[(int)((IntPtr)num2)].ResolveString8Values(string8Encoding);
				}
			}
		}

		internal PropertyRow[] Rows
		{
			get
			{
				return this.rows;
			}
		}

		internal static SuccessfulGetReceiveFolderTableResult Parse(Reader reader, Encoding string8Encoding)
		{
			return new SuccessfulGetReceiveFolderTableResult(reader, string8Encoding);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.rows.Length);
			foreach (PropertyRow propertyRow in this.rows)
			{
				propertyRow.Serialize(writer, base.String8Encoding, WireFormatStyle.Rop);
			}
		}

		private static readonly PropertyTag[] columns = new PropertyTag[]
		{
			PropertyTag.Fid,
			PropertyTag.MessageClass,
			PropertyTag.LastModificationTime
		};

		private readonly PropertyRow[] rows;
	}
}
