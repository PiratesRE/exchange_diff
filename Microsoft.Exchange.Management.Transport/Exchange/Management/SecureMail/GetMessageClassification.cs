using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SecureMail
{
	[Cmdlet("Get", "MessageClassification", DefaultParameterSetName = "Identity")]
	public sealed class GetMessageClassification : GetMultitenancySystemConfigurationObjectTask<MessageClassificationIdParameter, MessageClassification>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeLocales
		{
			get
			{
				return this.includeLocales;
			}
			set
			{
				this.includeLocales = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity != null)
				{
					return null;
				}
				return MessageClassificationIdParameter.DefaultRoot(base.DataSession);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider configDataProvider = base.CreateSession();
			((IConfigurationSession)configDataProvider).SessionSettings.IsSharedConfigChecked = true;
			((IConfigurationSession)configDataProvider).SessionSettings.IsRedirectedToSharedConfig = false;
			return configDataProvider;
		}

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			IConfigurationSession configurationSession = base.CreateConfigurationSession(sessionSettings);
			configurationSession.SessionSettings.IsSharedConfigChecked = true;
			configurationSession.SessionSettings.IsRedirectedToSharedConfig = false;
			return configurationSession;
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = MessageClassificationIdParameter.DefaultsRoot;
			}
			base.InternalValidate();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MessageClassification messageClassification = dataObject as MessageClassification;
			base.WriteResult(dataObject);
			if (this.IncludeLocales && messageClassification != null && messageClassification.IsDefault)
			{
				foreach (MessageClassification messageClassification2 in base.DataSession.FindPaged<MessageClassification>(new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, messageClassification.ClassificationID), MessageClassificationIdParameter.DefaultRoot(base.DataSession).Parent, true, null, 0))
				{
					if (!messageClassification2.IsDefault)
					{
						base.WriteResult(messageClassification2);
					}
				}
			}
		}

		private SwitchParameter includeLocales = new SwitchParameter(false);
	}
}
