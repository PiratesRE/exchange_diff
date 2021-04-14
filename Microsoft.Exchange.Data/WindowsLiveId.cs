using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class WindowsLiveId
	{
		public NetID NetId
		{
			get
			{
				return this.netId;
			}
			set
			{
				this.netId = value;
			}
		}

		public SmtpAddress SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
			set
			{
				this.smtpAddress = value;
			}
		}

		public WindowsLiveId(string id)
		{
			if (NetID.TryParse(id, out this.netId))
			{
				return;
			}
			if (SmtpAddress.IsValidSmtpAddress(id))
			{
				this.SmtpAddress = new SmtpAddress(id);
				return;
			}
			throw new FormatException(DataStrings.ErrorIncorrectWindowsLiveIdFormat(id));
		}

		public WindowsLiveId(NetID netId, SmtpAddress smtpAddress)
		{
			this.netId = netId;
			this.smtpAddress = smtpAddress;
		}

		public static WindowsLiveId Parse(string id)
		{
			return new WindowsLiveId(id);
		}

		public static bool TryParse(string id, out WindowsLiveId liveId)
		{
			bool result;
			try
			{
				liveId = WindowsLiveId.Parse(id);
				result = true;
			}
			catch (FormatException)
			{
				liveId = null;
				result = false;
			}
			return result;
		}

		public override string ToString()
		{
			if (this.NetId != null)
			{
				return this.NetId.ToString();
			}
			if (this.SmtpAddress != SmtpAddress.Empty)
			{
				return this.SmtpAddress.ToString();
			}
			return string.Empty;
		}

		private NetID netId;

		private SmtpAddress smtpAddress = SmtpAddress.Empty;
	}
}
