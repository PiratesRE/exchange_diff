using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetWellKnownShapesResponse
	{
		[DataMember]
		public string[] ShapeNames { get; set; }

		[DataMember]
		public ResponseShape[] ResponseShapes { get; set; }
	}
}
