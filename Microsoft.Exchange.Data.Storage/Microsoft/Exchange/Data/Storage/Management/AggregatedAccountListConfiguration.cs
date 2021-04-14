using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class AggregatedAccountListConfiguration : UserConfigurationObject
	{
		internal override UserConfigurationObjectSchema Schema
		{
			get
			{
				return AggregatedAccountListConfiguration.schema;
			}
		}

		public AggregatedAccountListConfiguration()
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
		}

		public List<AggregatedAccountInfo> AggregatedAccountList
		{
			get
			{
				return (List<AggregatedAccountInfo>)this[AggregatedAccountListConfigurationSchema.AggregatedAccountList];
			}
			set
			{
				this[AggregatedAccountListConfigurationSchema.AggregatedAccountList] = value;
			}
		}

		public Guid AggregatedMailboxGuid { get; set; }

		public SmtpAddress SmtpAddress { get; set; }

		public Guid RequestGuid { get; set; }

		public override void Delete(MailboxStoreTypeProvider session)
		{
			AggregatedAccountListConfiguration aggregatedAccountListConfiguration = this.Read(session, null) as AggregatedAccountListConfiguration;
			if (aggregatedAccountListConfiguration != null)
			{
				this.AggregatedAccountList = aggregatedAccountListConfiguration.AggregatedAccountList;
			}
			if (this.AggregatedAccountList != null)
			{
				this.AggregatedAccountList = this.AggregatedAccountList.FindAll((AggregatedAccountInfo account) => account.RequestGuid != this.RequestGuid);
			}
			if (this.AggregatedAccountList == null || this.AggregatedAccountList.Count == 0)
			{
				UserConfigurationHelper.DeleteMailboxConfiguration(session.MailboxSession, "AggregatedAccountList");
				return;
			}
			this.UpdateFAI(session);
		}

		public override IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity)
		{
			base.Principal = ExchangePrincipal.FromADUser(session.ADUser, null);
			IConfigurable result;
			using (UserConfigurationXmlAdapter<AggregatedAccountListConfiguration> userConfigurationXmlAdapter = new UserConfigurationXmlAdapter<AggregatedAccountListConfiguration>(session.MailboxSession, "AggregatedAccountList", SaveMode.NoConflictResolution, new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), new GetReadableUserConfigurationDelegate(UserConfigurationHelper.GetReadOnlyMailboxConfiguration), AggregatedAccountListConfiguration.property))
			{
				result = userConfigurationXmlAdapter.Read(base.Principal);
			}
			return result;
		}

		public override void Save(MailboxStoreTypeProvider session)
		{
			if (!(this.RequestGuid == Guid.Empty))
			{
				SmtpAddress smtpAddress = this.SmtpAddress;
				if (!string.IsNullOrEmpty(this.SmtpAddress.ToString()))
				{
					AggregatedAccountListConfiguration aggregatedAccountListConfiguration = this.Read(session, null) as AggregatedAccountListConfiguration;
					if (aggregatedAccountListConfiguration != null)
					{
						this.AggregatedAccountList = aggregatedAccountListConfiguration.AggregatedAccountList;
					}
					if (this.AggregatedAccountList == null)
					{
						this.AggregatedAccountList = new List<AggregatedAccountInfo>();
					}
					AggregatedAccountInfo aggregatedAccountInfo = this.AggregatedAccountList.Find((AggregatedAccountInfo account) => account.RequestGuid == this.RequestGuid);
					if (aggregatedAccountInfo == null)
					{
						this.AggregatedAccountList.Add(new AggregatedAccountInfo(this.AggregatedMailboxGuid, this.SmtpAddress, this.RequestGuid));
					}
					else
					{
						aggregatedAccountInfo.SmtpAddress = aggregatedAccountInfo.SmtpAddress;
					}
					this.UpdateFAI(session);
					return;
				}
			}
		}

		private void UpdateFAI(MailboxStoreTypeProvider session)
		{
			using (UserConfigurationXmlAdapter<AggregatedAccountListConfiguration> userConfigurationXmlAdapter = new UserConfigurationXmlAdapter<AggregatedAccountListConfiguration>(session.MailboxSession, "AggregatedAccountList", SaveMode.ResolveConflicts, new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), AggregatedAccountListConfiguration.property))
			{
				userConfigurationXmlAdapter.Save(this);
			}
			base.ResetChangeTracking();
		}

		private static AggregatedAccountListConfigurationSchema schema = ObjectSchema.GetInstance<AggregatedAccountListConfigurationSchema>();

		private static readonly SimplePropertyDefinition property = AggregatedAccountListConfigurationSchema.AggregatedAccountList;
	}
}
