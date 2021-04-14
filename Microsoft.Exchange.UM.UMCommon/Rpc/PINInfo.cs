using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.UM.Rpc
{
	[XmlType(TypeName = "PINInfoType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PINInfo : UMRpcResponse
	{
		[XmlElement]
		public string PIN
		{
			get
			{
				return this.pin;
			}
			set
			{
				this.pin = value;
			}
		}

		[XmlElement]
		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
			set
			{
				this.isValid = value;
			}
		}

		[XmlElement]
		public bool PinExpired
		{
			get
			{
				return this.pinExpired;
			}
			set
			{
				this.pinExpired = value;
			}
		}

		[XmlElement]
		public bool LockedOut
		{
			get
			{
				return this.lockedOut;
			}
			set
			{
				this.lockedOut = value;
			}
		}

		[XmlElement]
		public bool FirstTimeUser
		{
			get
			{
				return this.firstTimeUser;
			}
			set
			{
				this.firstTimeUser = value;
			}
		}

		private string pin;

		private bool lockedOut;

		private bool isValid;

		private bool pinExpired;

		private bool firstTimeUser;
	}
}
