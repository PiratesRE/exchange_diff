using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public sealed class MailboxAssociationReplicationStatePresentationObject : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxAssociationReplicationStatePresentationObject.schema;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				this[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? NextReplicationTime
		{
			get
			{
				return (ExDateTime?)this[MailboxAssociationReplicationStatePresentationObject.MailboxAssociationReplicationStatePresentationObjectSchema.NextReplicationTime];
			}
			set
			{
				this[MailboxAssociationReplicationStatePresentationObject.MailboxAssociationReplicationStatePresentationObjectSchema.NextReplicationTime] = value;
			}
		}

		public MailboxAssociationReplicationStatePresentationObject() : base(new SimpleProviderPropertyBag())
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Current);
			base.ResetChangeTracking();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Mailbox=");
			stringBuilder.Append(this.Identity);
			stringBuilder.Append(", NextReplicationTime=");
			stringBuilder.Append(this.NextReplicationTime);
			return stringBuilder.ToString();
		}

		private static MailboxAssociationReplicationStatePresentationObject.MailboxAssociationReplicationStatePresentationObjectSchema schema = ObjectSchema.GetInstance<MailboxAssociationReplicationStatePresentationObject.MailboxAssociationReplicationStatePresentationObjectSchema>();

		private class MailboxAssociationReplicationStatePresentationObjectSchema : SimpleProviderObjectSchema
		{
			public static readonly SimpleProviderPropertyDefinition NextReplicationTime = new SimpleProviderPropertyDefinition("NextReplicationTime", ExchangeObjectVersion.Current, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
