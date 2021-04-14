using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[OutputType(new Type[]
	{
		typeof(ConsumerMailbox)
	})]
	[Cmdlet("Get", "ConsumerMailbox")]
	public sealed class GetConsumerMailbox : GetObjectWithIdentityTaskBase<ConsumerMailboxIdParameter, ADUser>
	{
		private new Fqdn DomainController
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

		[Parameter(Mandatory = false)]
		public SwitchParameter MservDataOnly { get; set; }

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider configDataProvider = ConsumerMailboxHelper.CreateConsumerOrganizationSession();
			if (this.MservDataOnly.ToBool())
			{
				((IAggregateSession)configDataProvider).MbxReadMode = MbxReadMode.NoMbxRead;
			}
			else
			{
				((IAggregateSession)configDataProvider).MbxReadMode = MbxReadMode.OnlyIfLocatorDataAvailable;
			}
			return configDataProvider;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return ConsumerMailboxHelper.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser dataObject2 = (ADUser)dataObject;
			return new ConsumerMailbox(dataObject2);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (dataObject != null)
			{
				IConfigurable dataObject2 = this.ConvertDataObjectToPresentationObject(dataObject);
				base.WriteResult(dataObject2);
			}
		}
	}
}
