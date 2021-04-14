using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class Imap4AdConfiguration : PopImapAdConfiguration
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return Imap4AdConfiguration.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Imap4AdConfiguration.MostDerivedClass;
			}
		}

		public override string ProtocolName
		{
			get
			{
				return "IMAP4";
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Parameter(Mandatory = false)]
		public override int MaxCommandSize
		{
			get
			{
				return (int)this[Imap4AdConfigurationSchema.MaxCommandSize];
			}
			set
			{
				this[Imap4AdConfigurationSchema.MaxCommandSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ShowHiddenFoldersEnabled
		{
			get
			{
				return (bool)this[Imap4AdConfigurationSchema.ShowHiddenFoldersEnabled];
			}
			set
			{
				this[Imap4AdConfigurationSchema.ShowHiddenFoldersEnabled] = value;
			}
		}

		internal static object ShowHiddenFoldersEnabledGetter(IPropertyBag propertyBag)
		{
			int num = 1;
			ADPropertyDefinition popImapFlags = PopImapAdConfigurationSchema.PopImapFlags;
			return (num & (int)propertyBag[popImapFlags]) != 0;
		}

		internal static void ShowHiddenFoldersEnabledSetter(object value, IPropertyBag propertyBag)
		{
			int num = 1;
			ADPropertyDefinition popImapFlags = PopImapAdConfigurationSchema.PopImapFlags;
			if ((bool)value)
			{
				propertyBag[popImapFlags] = ((int)propertyBag[popImapFlags] | num);
				return;
			}
			propertyBag[popImapFlags] = ((int)propertyBag[popImapFlags] & ~num);
		}

		private static Imap4AdConfigurationSchema schema = ObjectSchema.GetInstance<Imap4AdConfigurationSchema>();

		public static string MostDerivedClass = "protocolCfgIMAPServer";
	}
}
