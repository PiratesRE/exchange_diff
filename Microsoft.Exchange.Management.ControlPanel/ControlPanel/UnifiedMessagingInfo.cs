using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[Serializable]
	public sealed class UnifiedMessagingInfo
	{
		public UnifiedMessagingInfo(string enableTemplate, string disableTemplate, string callForwardingType)
		{
			this.enableTemplate = enableTemplate;
			this.disableTemplate = disableTemplate;
			this.callForwardingType = callForwardingType;
		}

		public string EnableTemplate
		{
			get
			{
				return this.enableTemplate;
			}
		}

		public string DisableTemplate
		{
			get
			{
				return this.disableTemplate;
			}
		}

		public string CallForwardingType
		{
			get
			{
				return this.callForwardingType;
			}
		}

		public string RenderEnableSequence(string phoneNumber)
		{
			return string.Format(this.EnableTemplate, phoneNumber);
		}

		public string RenderDisableSequence(string phoneNumber)
		{
			if (!string.IsNullOrEmpty(phoneNumber))
			{
				return string.Format(this.DisableTemplate, phoneNumber);
			}
			return string.Empty;
		}

		public override string ToString()
		{
			return string.Format("{0}|{1}|{2}", this.EnableTemplate, this.DisableTemplate, this.CallForwardingType);
		}

		private string callForwardingType = string.Empty;

		private string enableTemplate = string.Empty;

		private string disableTemplate = string.Empty;
	}
}
