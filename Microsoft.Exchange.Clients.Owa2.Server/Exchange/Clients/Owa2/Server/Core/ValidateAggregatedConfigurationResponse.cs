using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ValidateAggregatedConfigurationResponse
	{
		public ValidateAggregatedConfigurationResponse()
		{
			this.FaiUpdates = new List<string>();
			this.TypeUpdates = new List<string>();
		}

		public List<string> FaiUpdates { get; set; }

		public List<string> TypeUpdates { get; set; }

		[DataMember(Name = "IsValidated")]
		public bool IsValidated { get; set; }

		[DataMember(Name = "FaiUpdates")]
		public string[] FaiUpdatesArray
		{
			get
			{
				return this.FaiUpdates.ToArray();
			}
			set
			{
				this.FaiUpdates = new List<string>(value);
			}
		}

		[DataMember(Name = "TypeUpdates")]
		public string[] TypeUpdatesArray
		{
			get
			{
				return this.TypeUpdates.ToArray();
			}
			set
			{
				this.TypeUpdates = new List<string>(value);
			}
		}
	}
}
