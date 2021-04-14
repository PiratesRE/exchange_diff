using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaBitMask : SearchCriteriaBitMask, IJetSearchCriteria
	{
		public JetSearchCriteriaBitMask(Column lhs, Column rhs, SearchCriteriaBitMask.SearchBitMaskOp op) : base(lhs, rhs, op)
		{
		}
	}
}
