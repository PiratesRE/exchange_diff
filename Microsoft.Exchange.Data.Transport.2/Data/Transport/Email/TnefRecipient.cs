using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class TnefRecipient
	{
		internal TnefRecipient(PureTnefMessage tnefMessage, int originalIndex, string displayName, string smtpAddress, string nativeAddress, string nativeAddressType)
		{
			this.tnefMessage = tnefMessage;
			this.originalIndex = originalIndex;
			this.displayName = displayName;
			this.smtpAddress = smtpAddress;
			this.nativeAddress = nativeAddress;
			this.nativeAddressType = nativeAddressType;
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
				this.SetDirty();
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
			set
			{
				this.smtpAddress = value;
				this.SetDirty();
			}
		}

		public string NativeAddress
		{
			get
			{
				return this.nativeAddress;
			}
			set
			{
				this.nativeAddress = value;
				this.SetDirty();
			}
		}

		public string NativeAddressType
		{
			get
			{
				return this.nativeAddressType;
			}
			set
			{
				this.nativeAddressType = value;
				this.SetDirty();
			}
		}

		internal int OriginalIndex
		{
			get
			{
				return this.originalIndex;
			}
			set
			{
				this.originalIndex = value;
			}
		}

		internal PureTnefMessage TnefMessage
		{
			get
			{
				return this.tnefMessage;
			}
			set
			{
				this.tnefMessage = value;
			}
		}

		private void SetDirty()
		{
			if (this.tnefMessage != null)
			{
				this.tnefMessage.SetDirty(this);
			}
		}

		private PureTnefMessage tnefMessage;

		private string displayName;

		private string smtpAddress;

		private string nativeAddress;

		private string nativeAddressType;

		private int originalIndex;
	}
}
