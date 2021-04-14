using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal sealed class DataValidationResult : IDataValidationResult
	{
		public PAAValidationResult PAAValidationResult { get; set; }

		public ADRecipient ADRecipient { get; set; }

		public PhoneNumber PhoneNumber { get; set; }

		public PersonalContactInfo PersonalContactInfo { get; set; }

		public PersonaType PersonaContactInfo { get; set; }

		public DataValidationResult()
		{
			this.PAAValidationResult = PAAValidationResult.Valid;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", new object[]
			{
				this.PAAValidationResult,
				(this.ADRecipient != null) ? this.ADRecipient.ToString() : string.Empty,
				(this.PhoneNumber != null) ? this.PhoneNumber.ToString() : string.Empty,
				(this.PersonalContactInfo != null) ? this.PersonalContactInfo.ToString() : string.Empty,
				(this.PersonaContactInfo != null) ? this.PersonaContactInfo.DisplayName : string.Empty
			});
		}
	}
}
