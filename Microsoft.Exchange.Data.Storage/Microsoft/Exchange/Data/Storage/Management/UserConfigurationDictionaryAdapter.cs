using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserConfigurationDictionaryAdapter<TObject> : MailboxManagementDataAdapter<TObject> where TObject : UserConfigurationObject, new()
	{
		public UserConfigurationDictionaryAdapter(MailboxSession session, string configuration, GetUserConfigurationDelegate configurationGetter, SimplePropertyDefinition[] appliedProperties) : this(session, configuration, SaveMode.NoConflictResolution, configurationGetter, appliedProperties)
		{
		}

		public UserConfigurationDictionaryAdapter(MailboxSession session, string configuration, SaveMode saveMode, GetUserConfigurationDelegate configurationGetter, SimplePropertyDefinition[] appliedProperties) : base(session, configuration)
		{
			if (string.IsNullOrEmpty(configuration))
			{
				throw new ArgumentNullException("configuration");
			}
			this.ConfigurationGetter = configurationGetter;
			this.AppliedProperties = new List<SimplePropertyDefinition>(appliedProperties);
			this.saveMode = saveMode;
		}

		private List<SimplePropertyDefinition> AppliedProperties { get; set; }

		private GetUserConfigurationDelegate ConfigurationGetter { get; set; }

		protected override void InternalFill(TObject configObject)
		{
			base.InternalFill(configObject);
			UserConfigurationDictionaryHelper.Fill(configObject, this.AppliedProperties.ToArray(), (bool createIfNonexisting) => this.ConfigurationGetter(base.Session, base.Configuration, UserConfigurationTypes.Dictionary, createIfNonexisting));
		}

		protected override void InternalSave(TObject configObject)
		{
			base.InternalSave(configObject);
			UserConfigurationDictionaryHelper.Save(configObject, this.saveMode, this.AppliedProperties.ToArray(), (bool createIfNonexisting) => this.ConfigurationGetter(base.Session, base.Configuration, UserConfigurationTypes.Dictionary, createIfNonexisting));
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UserConfigurationDictionaryAdapter<TObject>>(this);
		}

		private readonly SaveMode saveMode;
	}
}
