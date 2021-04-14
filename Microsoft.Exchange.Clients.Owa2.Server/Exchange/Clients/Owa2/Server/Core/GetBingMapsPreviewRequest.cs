using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetBingMapsPreviewRequest
	{
		public GetBingMapsPreviewRequest()
		{
			this.Latitude = null;
			this.Longitude = null;
			this.LocationName = null;
			this.MapWidth = 0;
			this.MapHeight = 0;
			this.ParentItemId = null;
			this.MapControlKey = null;
		}

		[DataMember]
		public string Latitude { get; set; }

		[DataMember]
		public string Longitude { get; set; }

		[DataMember]
		public string LocationName { get; set; }

		[DataMember]
		public int MapWidth { get; set; }

		[DataMember]
		public int MapHeight { get; set; }

		[DataMember]
		public ItemId ParentItemId { get; set; }

		[DataMember]
		public string MapControlKey { get; set; }
	}
}
