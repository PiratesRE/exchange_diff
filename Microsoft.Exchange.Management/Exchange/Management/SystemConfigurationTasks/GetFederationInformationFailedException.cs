using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class GetFederationInformationFailedException : LocalizedException
	{
		public GetFederationInformationFailedException() : base(Strings.GetFederationInformationFailed)
		{
		}

		public GetFederationInformationFailedException(GetFederationInformationResult[] discoveryResults) : base(Strings.GetFederationInformationFailed)
		{
			this.discoveryResults = discoveryResults;
		}

		public GetFederationInformationFailedException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
			this.discoveryResults = (GetFederationInformationResult[])serializationInfo.GetValue("discoveryResults", typeof(GetFederationInformationResult[]));
		}

		public GetFederationInformationResult[] DiscoveryResults
		{
			get
			{
				return this.discoveryResults;
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext context)
		{
			base.GetObjectData(serializationInfo, context);
			serializationInfo.AddValue("discoveryResults", this.discoveryResults);
		}

		private GetFederationInformationResult[] discoveryResults;
	}
}
