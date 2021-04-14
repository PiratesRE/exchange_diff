using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class StructuredErrors
	{
		[DataMember]
		public string[] Path { get; set; }

		[DataMember]
		public StructuredError[] Error
		{
			get
			{
				if (this.nestedErrors == null)
				{
					return null;
				}
				return this.nestedErrors.ToArray();
			}
		}

		public void AddError(StructuredError error)
		{
			if (this.nestedErrors == null)
			{
				this.nestedErrors = new List<StructuredError>(1);
			}
			this.nestedErrors.Add(error);
		}

		private List<StructuredError> nestedErrors;
	}
}
