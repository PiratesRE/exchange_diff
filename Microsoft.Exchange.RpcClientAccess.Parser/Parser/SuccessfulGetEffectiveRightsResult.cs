using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetEffectiveRightsResult : RopResult
	{
		internal SuccessfulGetEffectiveRightsResult(Rights effectiveRights) : base(RopId.GetEffectiveRights, ErrorCode.None, null)
		{
			this.effectiveRights = effectiveRights;
		}

		internal SuccessfulGetEffectiveRightsResult(Reader reader) : base(reader)
		{
			this.effectiveRights = (Rights)reader.ReadUInt32();
		}

		internal Rights EffectiveRights
		{
			get
			{
				return this.effectiveRights;
			}
		}

		internal static SuccessfulGetEffectiveRightsResult Parse(Reader reader)
		{
			return new SuccessfulGetEffectiveRightsResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.effectiveRights);
		}

		private Rights effectiveRights;
	}
}
