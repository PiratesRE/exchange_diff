using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidOrganizationStateException : MigrationTransientException
	{
		public InvalidOrganizationStateException(string org, string servicePlan, ExchangeObjectVersion version, bool isUpgrading, bool isPiloting, bool isUpgradeInProgress) : base(UpgradeHandlerStrings.InvalidOrganizationState(org, servicePlan, version, isUpgrading, isPiloting, isUpgradeInProgress))
		{
			this.org = org;
			this.servicePlan = servicePlan;
			this.version = version;
			this.isUpgrading = isUpgrading;
			this.isPiloting = isPiloting;
			this.isUpgradeInProgress = isUpgradeInProgress;
		}

		public InvalidOrganizationStateException(string org, string servicePlan, ExchangeObjectVersion version, bool isUpgrading, bool isPiloting, bool isUpgradeInProgress, Exception innerException) : base(UpgradeHandlerStrings.InvalidOrganizationState(org, servicePlan, version, isUpgrading, isPiloting, isUpgradeInProgress), innerException)
		{
			this.org = org;
			this.servicePlan = servicePlan;
			this.version = version;
			this.isUpgrading = isUpgrading;
			this.isPiloting = isPiloting;
			this.isUpgradeInProgress = isUpgradeInProgress;
		}

		protected InvalidOrganizationStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.org = (string)info.GetValue("org", typeof(string));
			this.servicePlan = (string)info.GetValue("servicePlan", typeof(string));
			this.version = (ExchangeObjectVersion)info.GetValue("version", typeof(ExchangeObjectVersion));
			this.isUpgrading = (bool)info.GetValue("isUpgrading", typeof(bool));
			this.isPiloting = (bool)info.GetValue("isPiloting", typeof(bool));
			this.isUpgradeInProgress = (bool)info.GetValue("isUpgradeInProgress", typeof(bool));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("org", this.org);
			info.AddValue("servicePlan", this.servicePlan);
			info.AddValue("version", this.version);
			info.AddValue("isUpgrading", this.isUpgrading);
			info.AddValue("isPiloting", this.isPiloting);
			info.AddValue("isUpgradeInProgress", this.isUpgradeInProgress);
		}

		public string Org
		{
			get
			{
				return this.org;
			}
		}

		public string ServicePlan
		{
			get
			{
				return this.servicePlan;
			}
		}

		public ExchangeObjectVersion Version
		{
			get
			{
				return this.version;
			}
		}

		public bool IsUpgrading
		{
			get
			{
				return this.isUpgrading;
			}
		}

		public bool IsPiloting
		{
			get
			{
				return this.isPiloting;
			}
		}

		public bool IsUpgradeInProgress
		{
			get
			{
				return this.isUpgradeInProgress;
			}
		}

		private readonly string org;

		private readonly string servicePlan;

		private readonly ExchangeObjectVersion version;

		private readonly bool isUpgrading;

		private readonly bool isPiloting;

		private readonly bool isUpgradeInProgress;
	}
}
