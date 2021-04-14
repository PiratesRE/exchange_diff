using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.CommonHandlers
{
	public class FaultDiagnosticsInfo
	{
		public FaultDiagnosticsInfo(int errorID, string errorText)
		{
			this.errorID = errorID;
			this.errorText = errorText;
		}

		public string ErrorText
		{
			get
			{
				return this.errorText;
			}
		}

		public int ErrorID
		{
			get
			{
				return this.errorID;
			}
		}

		private readonly int errorID;

		private readonly string errorText;
	}
}
