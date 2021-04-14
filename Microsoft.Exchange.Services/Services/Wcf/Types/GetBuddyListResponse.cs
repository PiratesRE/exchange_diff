using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class GetBuddyListResponse
	{
		internal GetBuddyListResponse()
		{
			this.Buddies = new Persona[0];
			this.Groups = new BuddyGroup[0];
		}

		[DataMember]
		public Persona[] Buddies { get; internal set; }

		[DataMember]
		public BuddyGroup[] Groups { get; internal set; }
	}
}
