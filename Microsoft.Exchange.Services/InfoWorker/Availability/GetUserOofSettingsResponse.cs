using System;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUserOofSettingsResponse : IExchangeWebMethodResponse
	{
		[XmlElement(IsNullable = false)]
		public ResponseMessage ResponseMessage
		{
			get
			{
				return this.responseMessage;
			}
			set
			{
				this.responseMessage = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public UserOofSettings OofSettings
		{
			get
			{
				return this.oofSettings;
			}
			set
			{
				if (value != null)
				{
					this.oofSettings = value;
				}
			}
		}

		public string AllowExternalOof
		{
			get
			{
				if (!this.EmitAllowExternalOof)
				{
					return null;
				}
				return this.allowExternalOof;
			}
			set
			{
				this.allowExternalOof = value;
			}
		}

		[XmlIgnore]
		internal bool EmitAllowExternalOof
		{
			get
			{
				return this.emitAllowExternalOof;
			}
			set
			{
				this.emitAllowExternalOof = value;
			}
		}

		public ResponseType GetResponseType()
		{
			return ResponseType.GetUserOofSettingsResponseMessage;
		}

		public ResponseCodeType GetErrorCodeToLog()
		{
			if (this.ResponseMessage == null)
			{
				return ResponseCodeType.NoError;
			}
			return this.ResponseMessage.ResponseCode;
		}

		private ResponseMessage responseMessage;

		private UserOofSettings oofSettings;

		private string allowExternalOof = ExternalAudience.None.ToString();

		private bool emitAllowExternalOof = true;
	}
}
