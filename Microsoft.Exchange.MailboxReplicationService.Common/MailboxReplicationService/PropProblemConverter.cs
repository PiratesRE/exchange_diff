using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PropProblemConverter : IDataConverter<PropProblem, PropProblemData>
	{
		PropProblem IDataConverter<PropProblem, PropProblemData>.GetNativeRepresentation(PropProblemData data)
		{
			return new PropProblem(data.Index, (PropTag)data.PropTag, data.Scode);
		}

		PropProblemData IDataConverter<PropProblem, PropProblemData>.GetDataRepresentation(PropProblem pp)
		{
			return new PropProblemData
			{
				PropTag = (int)pp.PropTag,
				Scode = pp.Scode,
				Index = pp.Index
			};
		}
	}
}
