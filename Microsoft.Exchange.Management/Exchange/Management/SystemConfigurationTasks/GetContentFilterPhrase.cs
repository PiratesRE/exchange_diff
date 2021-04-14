using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ContentFilterPhrase", DefaultParameterSetName = "Identity")]
	public sealed class GetContentFilterPhrase : GetObjectWithIdentityTaskBase<ContentFilterPhraseIdParameter, ContentFilterPhrase>
	{
		[Parameter]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = false, ParameterSetName = "Phrase")]
		public ContentFilterPhraseIdParameter Phrase
		{
			get
			{
				return this.Identity;
			}
			set
			{
				this.Identity = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.Phrase == null)
				{
					return null;
				}
				return new ContentFilterPhraseQueryFilter(this.Phrase.RawIdentity);
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, this.ConfigurationSession.SessionSettings, 69, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageHygiene\\ContentFilter\\GetContentFilterPhrase.cs");
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new ContentFilterPhraseDataProvider(this.configurationSession);
		}

		private IConfigurationSession configurationSession;
	}
}
