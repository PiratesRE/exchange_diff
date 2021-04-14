using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotMapInvalidSmtpAddressToPhotoFileException : LocalizedException
	{
		public CannotMapInvalidSmtpAddressToPhotoFileException(string address) : base(Strings.CannotMapInvalidSmtpAddressToPhotoFile(address))
		{
			this.address = address;
		}

		public CannotMapInvalidSmtpAddressToPhotoFileException(string address, Exception innerException) : base(Strings.CannotMapInvalidSmtpAddressToPhotoFile(address), innerException)
		{
			this.address = address;
		}

		protected CannotMapInvalidSmtpAddressToPhotoFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.address = (string)info.GetValue("address", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("address", this.address);
		}

		public string Address
		{
			get
			{
				return this.address;
			}
		}

		private readonly string address;
	}
}
