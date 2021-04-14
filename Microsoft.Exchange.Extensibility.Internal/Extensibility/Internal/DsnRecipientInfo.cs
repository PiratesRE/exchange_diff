using System;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal sealed class DsnRecipientInfo
	{
		public DsnRecipientInfo(string displayName, string address, string addressType, string enhancedStatusCode, string statusText) : this(displayName, address, addressType, enhancedStatusCode, statusText, null, null, null, null, null)
		{
			if (!EnhancedStatusCodeImpl.IsValid(enhancedStatusCode))
			{
				throw new ArgumentException("Invalid enhanced status code");
			}
		}

		internal DsnRecipientInfo(string displayName, string address, string addressType, string enhancedStatusCode, string statusText, string orecip, string explanation, string specificExplanation, string decodedAddress, string[] dsnParamTexts) : this(displayName, address, addressType, enhancedStatusCode, statusText, orecip, explanation, specificExplanation, decodedAddress, dsnParamTexts, false, null)
		{
		}

		internal DsnRecipientInfo(string displayName, string address, string addressType, string enhancedStatusCode, string statusText, string orecip, string explanation, string specificExplanation, string decodedAddress, string[] dsnParamTexts, bool overwriteDefault, string dsnSource) : this(displayName, address, addressType, enhancedStatusCode, statusText, orecip, explanation, specificExplanation, decodedAddress, dsnParamTexts, overwriteDefault, dsnSource, 0, null, null, null)
		{
		}

		internal DsnRecipientInfo(string displayName, string address, string addressType, string enhancedStatusCode, string statusText, string orecip, string explanation, string specificExplanation, string decodedAddress, string[] dsnParamTexts, bool overwriteDefault, string dsnSource, int retryCount, string lastTransientErrorDetails, string lastPermanentErrorDetails, string receivingServerDetails)
		{
			this.displayName = (string.IsNullOrEmpty(displayName) ? string.Empty : displayName);
			this.address = (string.IsNullOrEmpty(address) ? string.Empty : address);
			this.addressType = (string.IsNullOrEmpty(addressType) ? string.Empty : addressType);
			this.enhancedStatusCode = (string.IsNullOrEmpty(enhancedStatusCode) ? string.Empty : enhancedStatusCode);
			this.statusText = (string.IsNullOrEmpty(statusText) ? string.Empty : statusText);
			this.orecip = (string.IsNullOrEmpty(orecip) ? string.Empty : orecip);
			this.dsnHumanReadableExplanation = (string.IsNullOrEmpty(explanation) ? string.Empty : explanation);
			this.specificExplanation = (specificExplanation ?? string.Empty);
			this.decodedAddress = (string.IsNullOrEmpty(decodedAddress) ? string.Empty : decodedAddress);
			this.dsnParamTexts = dsnParamTexts;
			this.overwriteDefault = overwriteDefault;
			this.dsnSource = dsnSource;
			this.retryCount = retryCount;
			this.lastTransientErrorDetails = (string.IsNullOrEmpty(lastTransientErrorDetails) ? string.Empty : lastTransientErrorDetails);
			this.lastPermanentErrorDetails = (string.IsNullOrEmpty(lastPermanentErrorDetails) ? string.Empty : lastPermanentErrorDetails);
			this.receivingServerDetails = (string.IsNullOrEmpty(receivingServerDetails) ? string.Empty : receivingServerDetails);
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		internal string Address
		{
			get
			{
				return this.address;
			}
		}

		internal string AddressType
		{
			get
			{
				return this.addressType;
			}
		}

		internal string EnhancedStatusCode
		{
			get
			{
				return this.enhancedStatusCode;
			}
		}

		internal string StatusText
		{
			get
			{
				return this.statusText;
			}
		}

		internal string DecodedAddress
		{
			get
			{
				return this.decodedAddress;
			}
			set
			{
				this.decodedAddress = value;
			}
		}

		internal string ORecip
		{
			get
			{
				return this.orecip;
			}
		}

		internal string DsnHumanReadableExplanation
		{
			get
			{
				return this.dsnHumanReadableExplanation;
			}
			set
			{
				this.dsnHumanReadableExplanation = value;
			}
		}

		internal string DsnRecipientExplanation
		{
			get
			{
				return this.specificExplanation;
			}
			set
			{
				this.specificExplanation = value;
			}
		}

		internal bool IsCustomizedDsn
		{
			get
			{
				return this.isCustomizedDsn;
			}
			set
			{
				this.isCustomizedDsn = value;
			}
		}

		internal string[] DsnParamTexts
		{
			get
			{
				return this.dsnParamTexts;
			}
		}

		internal bool OverwriteDefault
		{
			get
			{
				return this.overwriteDefault;
			}
		}

		internal string DsnSource
		{
			get
			{
				return this.dsnSource;
			}
		}

		internal int RetryCount
		{
			get
			{
				return this.retryCount;
			}
		}

		internal string LastTransientErrorDetails
		{
			get
			{
				return this.lastTransientErrorDetails;
			}
		}

		internal string LastPermanentErrorDetails
		{
			get
			{
				return this.lastPermanentErrorDetails;
			}
		}

		internal string ReceivingServerDetails
		{
			get
			{
				return this.receivingServerDetails;
			}
		}

		public string NdrEnhancedText
		{
			get
			{
				return this.ndrEnhancedText;
			}
			set
			{
				this.ndrEnhancedText = value;
			}
		}

		public EmailRecipientCollection ModeratedRecipients
		{
			get
			{
				return this.moderatedRecipients;
			}
			set
			{
				this.moderatedRecipients = value;
			}
		}

		internal string GetDiagnostic()
		{
			return this.addressType + "#" + this.orecip;
		}

		public DsnRecipientInfo NewCloneWithDifferentRecipient(string recipientAddress)
		{
			return new DsnRecipientInfo(string.Empty, recipientAddress, this.AddressType, this.EnhancedStatusCode, this.StatusText, this.ORecip, this.DsnHumanReadableExplanation, this.DsnRecipientExplanation, string.Empty, (string[])this.DsnParamTexts.Clone(), this.OverwriteDefault, this.DsnSource);
		}

		private readonly string dsnSource;

		private string displayName;

		private readonly string addressType;

		private readonly string address;

		private string decodedAddress;

		private readonly string enhancedStatusCode;

		private readonly string statusText;

		private readonly string orecip;

		private string dsnHumanReadableExplanation;

		private readonly string[] dsnParamTexts;

		private readonly bool overwriteDefault;

		private readonly int retryCount;

		private readonly string lastTransientErrorDetails;

		private readonly string lastPermanentErrorDetails;

		private readonly string receivingServerDetails;

		private bool isCustomizedDsn;

		private string ndrEnhancedText;

		private EmailRecipientCollection moderatedRecipients;

		private string specificExplanation;
	}
}
