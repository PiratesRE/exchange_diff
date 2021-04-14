using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoteEndpointsCannotBeUsedOnLocalBatchesException : LocalizedException
	{
		public RemoteEndpointsCannotBeUsedOnLocalBatchesException(string endpointName) : base(Strings.ErrorRemoteEndpointsCannotBeUsedOnLocalBatches(endpointName))
		{
			this.endpointName = endpointName;
		}

		public RemoteEndpointsCannotBeUsedOnLocalBatchesException(string endpointName, Exception innerException) : base(Strings.ErrorRemoteEndpointsCannotBeUsedOnLocalBatches(endpointName), innerException)
		{
			this.endpointName = endpointName;
		}

		protected RemoteEndpointsCannotBeUsedOnLocalBatchesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpointName = (string)info.GetValue("endpointName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpointName", this.endpointName);
		}

		public string EndpointName
		{
			get
			{
				return this.endpointName;
			}
		}

		private readonly string endpointName;
	}
}
