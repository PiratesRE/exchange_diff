using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal struct ExtendedAttendeeData
	{
		public ExtendedAttendeeData(string emailAddress, string displayName, int status, int type, bool sendExtendedData)
		{
			this.emailAddress = emailAddress;
			this.displayName = displayName;
			this.status = ((status != -1) ? status : 5);
			this.type = ((type != -1) ? type : 1);
			this.sendExtendedData = sendExtendedData;
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
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
			}
		}

		public bool SendExtendedData
		{
			get
			{
				return this.sendExtendedData;
			}
			set
			{
				this.sendExtendedData = value;
			}
		}

		public int Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public int Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		private string displayName;

		private string emailAddress;

		private bool sendExtendedData;

		private int status;

		private int type;
	}
}
