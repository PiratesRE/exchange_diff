using System;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	public class EmailRecipient
	{
		public EmailRecipient(string displayName, string smtpAddress)
		{
			this.mimeRecipient = new MimeRecipient(displayName, smtpAddress);
		}

		internal EmailRecipient(MimeRecipient recipient)
		{
			this.mimeRecipient = recipient;
		}

		internal EmailRecipient(TnefRecipient tnefRecipient)
		{
			this.tnefRecipient = tnefRecipient;
		}

		public string DisplayName
		{
			get
			{
				if (this.mimeRecipient != null)
				{
					string empty = string.Empty;
					DecodingResults decodingResults;
					this.mimeRecipient.TryGetDisplayName(Utility.DecodeOrFallBack, out decodingResults, out empty);
					return empty;
				}
				return this.tnefRecipient.DisplayName;
			}
			set
			{
				if (this.mimeRecipient != null)
				{
					this.mimeRecipient.DisplayName = value;
					return;
				}
				this.tnefRecipient.DisplayName = value;
			}
		}

		public string SmtpAddress
		{
			get
			{
				if (this.mimeRecipient == null)
				{
					return this.tnefRecipient.SmtpAddress;
				}
				return this.mimeRecipient.Email;
			}
			set
			{
				if (this.mimeRecipient != null)
				{
					this.mimeRecipient.Email = value;
					return;
				}
				this.tnefRecipient.SmtpAddress = value;
			}
		}

		public string NativeAddress
		{
			get
			{
				if (this.mimeRecipient == null)
				{
					return this.tnefRecipient.NativeAddress;
				}
				return this.mimeRecipient.Email;
			}
			set
			{
				if (this.mimeRecipient != null)
				{
					throw new InvalidOperationException(EmailMessageStrings.CannotSetNativePropertyForMimeRecipient);
				}
				this.tnefRecipient.NativeAddress = value;
			}
		}

		public string NativeAddressType
		{
			get
			{
				if (this.mimeRecipient == null)
				{
					return this.tnefRecipient.NativeAddressType;
				}
				return "SMTP";
			}
			set
			{
				if (this.mimeRecipient != null)
				{
					throw new InvalidOperationException(EmailMessageStrings.CannotSetNativePropertyForMimeRecipient);
				}
				this.tnefRecipient.NativeAddressType = value;
			}
		}

		internal MimeRecipient MimeRecipient
		{
			get
			{
				if (this.mimeRecipient == null)
				{
					this.mimeRecipient = new MimeRecipient(this.DisplayName, this.SmtpAddress);
				}
				return this.mimeRecipient;
			}
		}

		internal TnefRecipient TnefRecipient
		{
			get
			{
				if (this.tnefRecipient == null)
				{
					this.tnefRecipient = new TnefRecipient(null, int.MinValue, this.DisplayName, this.SmtpAddress, this.NativeAddress, this.NativeAddressType);
				}
				return this.tnefRecipient;
			}
		}

		private MimeRecipient mimeRecipient;

		private TnefRecipient tnefRecipient;
	}
}
