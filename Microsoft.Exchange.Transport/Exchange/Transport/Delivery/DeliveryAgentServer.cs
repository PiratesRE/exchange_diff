using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Partner;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class DeliveryAgentServer : ExtendedDeliveryAgentSmtpServer
	{
		private DeliveryAgentServer(IReadOnlyMailItem currentMailItem, AcceptedDomainCollection acceptedDomains)
		{
			ArgumentValidator.ThrowIfNull("acceptedDomains", acceptedDomains);
			this.catServer = CatServer.GetInstance(currentMailItem, acceptedDomains);
		}

		public static DeliveryAgentServer GetInstance(IReadOnlyMailItem currentMailItem, AcceptedDomainCollection acceptedDomains)
		{
			return new DeliveryAgentServer(currentMailItem, acceptedDomains);
		}

		public override string Name
		{
			get
			{
				return this.catServer.Name;
			}
		}

		public override Version Version
		{
			get
			{
				return this.catServer.Version;
			}
		}

		public override IPPermission IPPermission
		{
			get
			{
				return this.catServer.IPPermission;
			}
		}

		public override AddressBook AddressBook
		{
			get
			{
				return this.catServer.AddressBook;
			}
		}

		public override AcceptedDomainCollection AcceptedDomains
		{
			get
			{
				return this.catServer.AcceptedDomains;
			}
		}

		public override RemoteDomainCollection RemoteDomains
		{
			get
			{
				return this.catServer.RemoteDomains;
			}
		}

		public override void SubmitMessage(EmailMessage message)
		{
			this.catServer.SubmitMessage(message);
		}

		private readonly CatServer catServer;
	}
}
