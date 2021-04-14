using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidEndpointTypeException : LocalizedException
	{
		public InvalidEndpointTypeException(string endpointId, string type) : base(Strings.ErrorInvalidEndpointType(endpointId, type))
		{
			this.endpointId = endpointId;
			this.type = type;
		}

		public InvalidEndpointTypeException(string endpointId, string type, Exception innerException) : base(Strings.ErrorInvalidEndpointType(endpointId, type), innerException)
		{
			this.endpointId = endpointId;
			this.type = type;
		}

		protected InvalidEndpointTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpointId = (string)info.GetValue("endpointId", typeof(string));
			this.type = (string)info.GetValue("type", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpointId", this.endpointId);
			info.AddValue("type", this.type);
		}

		public string EndpointId
		{
			get
			{
				return this.endpointId;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		private readonly string endpointId;

		private readonly string type;
	}
}
