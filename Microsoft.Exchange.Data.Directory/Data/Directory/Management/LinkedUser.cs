using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class LinkedUser : User
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return LinkedUser.schema;
			}
		}

		public LinkedUser()
		{
			base.SetObjectClass("user");
		}

		public LinkedUser(ADUser dataObject) : base(dataObject)
		{
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private new bool RemotePowerShellEnabled
		{
			get
			{
				return base.RemotePowerShellEnabled;
			}
			set
			{
				base.RemotePowerShellEnabled = value;
			}
		}

		private new UpgradeRequestTypes UpgradeRequest
		{
			get
			{
				return base.UpgradeRequest;
			}
			set
			{
				base.UpgradeRequest = value;
			}
		}

		private new UpgradeStatusTypes UpgradeStatus
		{
			get
			{
				return base.UpgradeStatus;
			}
			set
			{
				base.UpgradeStatus = value;
			}
		}

		private new string UpgradeDetails
		{
			get
			{
				return base.UpgradeDetails;
			}
		}

		private new string UpgradeMessage
		{
			get
			{
				return base.UpgradeMessage;
			}
		}

		private new UpgradeStage? UpgradeStage
		{
			get
			{
				return base.UpgradeStage;
			}
		}

		private new DateTime? UpgradeStageTimeStamp
		{
			get
			{
				return base.UpgradeStageTimeStamp;
			}
		}

		private new MailboxRelease MailboxRelease
		{
			get
			{
				return base.MailboxRelease;
			}
		}

		private new MailboxRelease ArchiveRelease
		{
			get
			{
				return base.ArchiveRelease;
			}
		}

		private static LinkedUserSchema schema = ObjectSchema.GetInstance<LinkedUserSchema>();
	}
}
