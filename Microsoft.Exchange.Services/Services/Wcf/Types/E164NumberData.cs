using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class E164NumberData : OptionsPropertyChangeTracker
	{
		public E164NumberData()
		{
		}

		internal E164NumberData(E164Number number)
		{
			this.CountryCode = number.CountryCode;
			this.SignificantNumber = number.SignificantNumber;
		}

		[DataMember]
		public string CountryCode { get; set; }

		[DataMember]
		public string SignificantNumber { get; set; }
	}
}
