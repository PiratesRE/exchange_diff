using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetReadFlagResultFactory : StandardResultFactory
	{
		internal SetReadFlagResultFactory(byte logonIndex, StoreLongTermId longTermId, bool isPublicLogon) : base(RopId.SetReadFlag)
		{
			this.logonIndex = logonIndex;
			this.longTermId = longTermId;
			this.isPublicLogon = isPublicLogon;
		}

		internal StoreLongTermId LongTermId
		{
			get
			{
				return this.longTermId;
			}
		}

		public RopResult CreateSuccessfulResult(bool hasChanged)
		{
			if (!this.isPublicLogon)
			{
				hasChanged = false;
			}
			return new SuccessfulSetReadFlagResult(hasChanged, this.logonIndex, this.longTermId);
		}

		private readonly byte logonIndex;

		private readonly StoreLongTermId longTermId;

		private readonly bool isPublicLogon;
	}
}
