using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CredentialNotSupportedForEndpointTypeException : LocalizedException
	{
		public CredentialNotSupportedForEndpointTypeException(string endpointType) : base(Strings.ErrorCredentialNotSupportedForEndpointType(endpointType))
		{
			this.endpointType = endpointType;
		}

		public CredentialNotSupportedForEndpointTypeException(string endpointType, Exception innerException) : base(Strings.ErrorCredentialNotSupportedForEndpointType(endpointType), innerException)
		{
			this.endpointType = endpointType;
		}

		protected CredentialNotSupportedForEndpointTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpointType = (string)info.GetValue("endpointType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpointType", this.endpointType);
		}

		public string EndpointType
		{
			get
			{
				return this.endpointType;
			}
		}

		private readonly string endpointType;
	}
}
