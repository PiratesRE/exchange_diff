using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class PlayOnPhoneAAGreetingRequest : PlayOnPhoneRequest
	{
		public Guid AAIdentity
		{
			get
			{
				return this.aaGuid;
			}
			set
			{
				this.aaGuid = value;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		public string UserRecordingTheGreeting
		{
			get
			{
				return this.user;
			}
			set
			{
				this.user = value;
			}
		}

		internal UMDialPlan DialPlan
		{
			get
			{
				if (this.dp == null)
				{
					this.GetAAAndDialPlan();
				}
				return this.dp;
			}
		}

		internal UMAutoAttendant AutoAttendant
		{
			get
			{
				if (this.aa == null)
				{
					this.GetAAAndDialPlan();
				}
				return this.aa;
			}
		}

		internal override string GetFriendlyName()
		{
			return Strings.PlayOnPhoneAAGreetingRequest;
		}

		private void GetAAAndDialPlan()
		{
			Guid aaidentity = this.AAIdentity;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromTenantGuid(base.TenantGuid);
			UMAutoAttendant autoAttendantFromId = iadsystemConfigurationLookup.GetAutoAttendantFromId(new ADObjectId(this.AAIdentity));
			if (autoAttendantFromId == null)
			{
				throw new InvalidUMAutoAttendantException();
			}
			this.aa = autoAttendantFromId;
			this.dp = iadsystemConfigurationLookup.GetDialPlanFromId(autoAttendantFromId.UMDialPlan);
		}

		private Guid aaGuid;

		private string fileName;

		private UMDialPlan dp;

		private UMAutoAttendant aa;

		private string user;
	}
}
