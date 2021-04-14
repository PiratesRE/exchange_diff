using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmRegistryException : AmCommonException
	{
		public AmRegistryException(string apiName) : base(ReplayStrings.AmRegistryException(apiName))
		{
			this.apiName = apiName;
		}

		public AmRegistryException(string apiName, Exception innerException) : base(ReplayStrings.AmRegistryException(apiName), innerException)
		{
			this.apiName = apiName;
		}

		protected AmRegistryException(SerializationInfo info, StreamingContext context) : base(info, context)
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
