using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Providers
{
	[Serializable]
	public class AlternateMailboxObject : ConfigurableObject
	{
		[Parameter]
		public string Name
		{
			get
			{
				return (string)this.propertyBag[AlternateMailboxSchema.Name];
			}
			set
			{
				this.propertyBag[AlternateMailboxSchema.Name] = value;
			}
		}

		internal static StringComparer NameComparer
		{
			get
			{
				return StringComparer.CurrentCultureIgnoreCase;
			}
		}

		internal static StringComparison NameComparison
		{
			get
			{
				return StringComparison.CurrentCultureIgnoreCase;
			}
		}

		[Parameter]
		public string UserDisplayName
		{
			get
			{
				return (string)this.propertyBag[AlternateMailboxSchema.UserDisplayName];
			}
			set
			{
				this.propertyBag[AlternateMailboxSchema.UserDisplayName] = value;
			}
		}

		public AlternateMailbox.AlternateMailboxFlags Type
		{
			get
			{
				return (AlternateMailbox.AlternateMailboxFlags)this.propertyBag[AlternateMailboxSchema.Type];
			}
			internal set
			{
				this.propertyBag[AlternateMailboxSchema.Type] = value;
			}
		}

		[Parameter]
		public bool RetentionPolicyEnabled
		{
			get
			{
				return (bool)this.propertyBag[AlternateMailboxSchema.RetentionPolicyEnabled];
			}
			set
			{
				this.propertyBag[AlternateMailboxSchema.RetentionPolicyEnabled] = value;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return (Guid)this.propertyBag[AlternateMailboxSchema.DatabaseGuid];
			}
			internal set
			{
				this.propertyBag[AlternateMailboxSchema.DatabaseGuid] = value;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)this.propertyBag[AlternateMailboxSchema.MailboxGuid];
			}
			internal set
			{
				this.propertyBag[AlternateMailboxSchema.MailboxGuid] = value;
			}
		}

		internal void SetIdentity(AlternateMailboxObjectId id)
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = id;
		}

		public AlternateMailboxObject() : base(new AlternateMailboxPropertyBag())
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AlternateMailboxObject.s_schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static ObjectSchema s_schema = ObjectSchema.GetInstance<AlternateMailboxSchema>();
	}
}
