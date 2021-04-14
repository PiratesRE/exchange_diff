using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	internal class PhonebookServicesResponse : IBingResultSet
	{
		[DataMember(Name = "SearchResponse")]
		public PhonebookServicesWebResponse Response { get; set; }

		public IBingResult[] Results
		{
			get
			{
				if (this.Response != null && this.Response.Phonebook != null && this.Response.Phonebook.Results != null)
				{
					return this.Response.Phonebook.Results;
				}
				return null;
			}
		}

		public IBingError[] Errors
		{
			get
			{
				if (this.Response != null && this.Response.Errors != null)
				{
					return this.Response.Errors;
				}
				return null;
			}
		}
	}
}
