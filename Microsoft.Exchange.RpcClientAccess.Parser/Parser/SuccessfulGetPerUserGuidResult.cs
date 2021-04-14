using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SuccessfulGetPerUserGuidResult : RopResult
	{
		internal SuccessfulGetPerUserGuidResult(Guid databaseGuid) : base(RopId.GetPerUserGuid, ErrorCode.None, null)
		{
			this.databaseGuid = databaseGuid;
		}

		internal SuccessfulGetPerUserGuidResult(Reader reader) : base(reader)
		{
			this.databaseGuid = reader.ReadGuid();
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		internal static SuccessfulGetPerUserGuidResult Parse(Reader reader)
		{
			return new SuccessfulGetPerUserGuidResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteGuid(this.DatabaseGuid);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" DatabaseGuid=[").Append(this.DatabaseGuid).Append("]");
		}

		private readonly Guid databaseGuid;
	}
}
