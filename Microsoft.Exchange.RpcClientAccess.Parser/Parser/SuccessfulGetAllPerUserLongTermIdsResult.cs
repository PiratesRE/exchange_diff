using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetAllPerUserLongTermIdsResult : RopResult
	{
		internal SuccessfulGetAllPerUserLongTermIdsResult(PerUserDataCollector perUserDataCollector, bool finished) : base(RopId.GetAllPerUserLongTermIds, ErrorCode.None, null)
		{
			this.perUserDataEntries = perUserDataCollector.PerUserDataEntries;
			this.finished = finished;
		}

		internal SuccessfulGetAllPerUserLongTermIdsResult(Reader reader) : base(reader)
		{
			this.perUserDataEntries = new List<PerUserData>();
			this.finished = reader.ReadBool();
			ushort num = reader.ReadUInt16();
			for (ushort num2 = 0; num2 < num; num2 += 1)
			{
				this.perUserDataEntries.Add(new PerUserData(reader));
			}
		}

		internal List<PerUserData> PerUserDataEntries
		{
			get
			{
				return this.perUserDataEntries;
			}
		}

		internal bool HasFinished
		{
			get
			{
				return this.finished;
			}
		}

		public override string ToString()
		{
			return string.Format("SuccessfulGetAllPerUserLongTermIdsResult: [Finished: {0}, PerUserData: {1}]", this.finished, this.perUserDataEntries.Count);
		}

		internal static RopResult Parse(Reader reader)
		{
			return new SuccessfulGetAllPerUserLongTermIdsResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.finished, 1);
			int count = this.perUserDataEntries.Count;
			writer.WriteUInt16((ushort)count);
			foreach (PerUserData perUserData in this.perUserDataEntries)
			{
				perUserData.Serialize(writer);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Finished=").Append(this.finished);
			stringBuilder.Append("\n PerUserDate entries=").Append(this.perUserDataEntries.Count);
		}

		private readonly List<PerUserData> perUserDataEntries;

		private readonly bool finished;
	}
}
