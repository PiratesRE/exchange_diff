using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "LockDevice")]
	internal class LockDevice : Resource
	{
		public LockDevice(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "lockDevice";
	}
}
