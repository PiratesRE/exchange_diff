using System;

namespace Microsoft.Exchange.Security.Authentication
{
	[Serializable]
	public class LiveIdLoginAttributes
	{
		public LiveIdLoginAttributes(uint loginAttributes)
		{
			this.loginAttributesBitField = (loginAttributes & 255U);
		}

		public uint Value
		{
			get
			{
				return this.loginAttributesBitField;
			}
		}

		public bool IsInsideCorpnetSession
		{
			get
			{
				return (this.loginAttributesBitField & 32U) == 32U;
			}
		}

		private const byte PP_LOGINATTRIBUTE_TRUSTEDDEVICE = 1;

		private const byte PP_LOGINATTRIBUTE_PIN = 2;

		private const byte PP_LOGINATTRIBUTE_STRONGPWD = 4;

		private const byte PP_LOGINATTRIBUTE_STRONGPWDEXPIRY = 8;

		private const byte PP_LOGINATTRIBUTE_CERTIFICATE = 16;

		private const byte PP_LOGINATTRIBUTE_INSIDECORPORATENETWORK = 32;

		private const byte PP_LOGINATTRIBUTE_DC_ISREGISTEREDUSER = 64;

		private const byte PP_LOGINATTRIBUTE_DC_ISMANAGED = 128;

		private readonly uint loginAttributesBitField;
	}
}
