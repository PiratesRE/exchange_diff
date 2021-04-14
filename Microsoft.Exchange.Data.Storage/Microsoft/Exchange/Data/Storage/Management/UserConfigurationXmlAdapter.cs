using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UserConfigurationXmlAdapter<TObject> : MailboxManagementDataAdapter<TObject> where TObject : UserConfigurationObject, new()
	{
		public UserConfigurationXmlAdapter(MailboxSession session, string configuration, GetUserConfigurationDelegate configurationGetter, SimplePropertyDefinition property) : this(session, configuration, SaveMode.NoConflictResolution, configurationGetter, property)
		{
		}

		public UserConfigurationXmlAdapter(MailboxSession session, string configuration, SaveMode saveMode, GetUserConfigurationDelegate configurationGetter, SimplePropertyDefinition property) : this(session, configuration, saveMode, configurationGetter, null, property)
		{
		}

		public UserConfigurationXmlAdapter(MailboxSession session, string configuration, SaveMode saveMode, GetUserConfigurationDelegate configurationGetter, GetReadableUserConfigurationDelegate readConfigurationGetter, SimplePropertyDefinition property) : base(session, configuration)
		{
			if (string.IsNullOrEmpty(configuration))
			{
				throw new ArgumentNullException("configuration");
			}
			this.ConfigurationGetter = configurationGetter;
			this.ReadableConfigurationGetter = (readConfigurationGetter ?? new GetReadableUserConfigurationDelegate(this.ReadableConfigurationFromWriteableDelegate));
			this.Property = property;
			this.saveMode = saveMode;
		}

		private SimplePropertyDefinition Property { get; set; }

		private GetUserConfigurationDelegate ConfigurationGetter { get; set; }

		private GetReadableUserConfigurationDelegate ReadableConfigurationGetter { get; set; }

		protected override void InternalFill(TObject configObject)
		{
			base.InternalFill(configObject);
			UserConfigurationXmlHelper.Fill(configObject, this.Property, (bool createIfNonexisting) => this.ReadableConfigurationGetter(base.Session, base.Configuration, UserConfigurationTypes.XML, createIfNonexisting));
		}

		protected override void InternalSave(TObject configObject)
		{
			base.InternalSave(configObject);
			UserConfigurationXmlHelper.Save(configObject, this.saveMode, this.Property, (bool createIfNonexisting) => this.ConfigurationGetter(base.Session, base.Configuration, UserConfigurationTypes.XML, createIfNonexisting));
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UserConfigurationXmlAdapter<TObject>>(this);
		}

		private IReadableUserConfiguration ReadableConfigurationFromWriteableDelegate(MailboxSession session, string configuration, UserConfigurationTypes type, bool createIfNonExisting)
		{
			return this.ConfigurationGetter(session, configuration, type, createIfNonExisting);
		}

		private readonly SaveMode saveMode;
	}
}
