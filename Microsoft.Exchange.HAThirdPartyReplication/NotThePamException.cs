using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotThePamException : ThirdPartyReplicationException
	{
		public NotThePamException(string apiName) : base(ThirdPartyReplication.OnlyPAMError(apiName))
		{
			this.apiName = apiName;
		}

		public NotThePamException(string apiName, Exception innerException) : base(ThirdPartyReplication.OnlyPAMError(apiName), innerException)
		{
			this.apiName = apiName;
		}

		protected NotThePamException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.apiName = (string)info.GetValue("apiName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("apiName", this.apiName);
		}

		public string ApiName
		{
			get
			{
				return this.apiName;
			}
		}

		private readonly string apiName;
	}
}
