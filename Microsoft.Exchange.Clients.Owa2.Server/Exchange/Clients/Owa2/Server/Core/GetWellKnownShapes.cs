using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetWellKnownShapes : ServiceCommand<GetWellKnownShapesResponse>
	{
		public GetWellKnownShapes(CallContext callContext) : base(callContext)
		{
		}

		protected override GetWellKnownShapesResponse InternalExecute()
		{
			List<string> list = new List<string>();
			List<ResponseShape> list2 = new List<ResponseShape>();
			foreach (KeyValuePair<WellKnownShapeName, ResponseShape> keyValuePair in WellKnownShapes.ResponseShapes)
			{
				list.Add(keyValuePair.Key.ToString());
				list2.Add(keyValuePair.Value);
			}
			return new GetWellKnownShapesResponse
			{
				ShapeNames = list.ToArray(),
				ResponseShapes = list2.ToArray()
			};
		}
	}
}
