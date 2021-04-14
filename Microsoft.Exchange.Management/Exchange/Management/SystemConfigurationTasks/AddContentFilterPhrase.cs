using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "ContentFilterPhrase", SupportsShouldProcess = true)]
	public sealed class AddContentFilterPhrase : NewTaskBase<ContentFilterPhrase>
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

		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
		[Alias(new string[]
		{
			"Identity"
		})]
		public string Phrase
		{
			get
			{
				return this.DataObject.Phrase;
			}
			set
			{
				this.DataObject.Phrase = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		public Influence Influence
		{
			get
			{
				return this.DataObject.Influence;
			}
			set
			{
				this.DataObject.Influence = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddContentFilterPhrase(this.Phrase, this.Influence.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, this.ConfigurationSession.SessionSettings, 82, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageHygiene\\ContentFilter\\AddContentFilterPhrase.cs");
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new ContentFilterPhraseDataProvider(this.configurationSession);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.Exists(this.DataObject.Phrase))
			{
				base.WriteError(new ArgumentException(Strings.DuplicateContentFilterPhrase(this.DataObject.Phrase), "Phrase"), ErrorCategory.InvalidArgument, this.DataObject);
			}
			TaskLogger.LogExit();
		}

		private bool Exists(string phrase)
		{
			ContentFilterPhraseQueryFilter filter = new ContentFilterPhraseQueryFilter(this.DataObject.Phrase);
			IConfigurable[] array = base.DataSession.Find<ContentFilterPhrase>(filter, null, false, null);
			return array != null && array.Length > 0;
		}

		private IConfigurationSession configurationSession;
	}
}
