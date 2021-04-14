using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ManagementApiException : LocalizedException
	{
		public ManagementApiException(string api) : base(ReplayStrings.ManagementApiError(api))
		{
			this.api = api;
		}

		public ManagementApiException(string api, Exception innerException) : base(ReplayStrings.ManagementApiError(api), innerException)
		{
			this.api = api;
		}

		protected ManagementApiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.api = (string)info.GetValue("api", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("api", this.api);
		}

		public string Api
		{
			get
			{
				return this.api;
			}
		}

		private readonly string api;
	}
}
