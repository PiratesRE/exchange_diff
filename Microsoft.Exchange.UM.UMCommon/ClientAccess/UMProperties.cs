using System;

namespace Microsoft.Exchange.UM.ClientAccess
{
	public class UMProperties
	{
		public UMProperties()
		{
		}

		public UMProperties(UMPropertiesEx properties)
		{
			this.missedCallNotificationEnabled = properties.MissedCallNotificationEnabled;
			this.playOnPhoneDialString = properties.PlayOnPhoneDialString;
			this.telephoneAccessNumbers = properties.TelephoneAccessNumbers;
			this.telephoneAccessFolderEmail = properties.TelephoneAccessFolderEmail;
			this.oofStatus = properties.OofStatus;
		}

		public bool OofStatus
		{
			get
			{
				return this.oofStatus;
			}
			set
			{
				this.oofStatus = value;
			}
		}

		public bool MissedCallNotificationEnabled
		{
			get
			{
				return this.missedCallNotificationEnabled;
			}
			set
			{
				this.missedCallNotificationEnabled = value;
			}
		}

		public string PlayOnPhoneDialString
		{
			get
			{
				return this.playOnPhoneDialString;
			}
			set
			{
				this.playOnPhoneDialString = value;
			}
		}

		public string TelephoneAccessNumbers
		{
			get
			{
				return this.telephoneAccessNumbers;
			}
			set
			{
				this.telephoneAccessNumbers = value;
			}
		}

		public string TelephoneAccessFolderEmail
		{
			get
			{
				return this.telephoneAccessFolderEmail;
			}
			set
			{
				this.telephoneAccessFolderEmail = value;
			}
		}

		private bool oofStatus;

		private bool missedCallNotificationEnabled;

		private string playOnPhoneDialString;

		private string telephoneAccessNumbers;

		private string telephoneAccessFolderEmail;
	}
}
