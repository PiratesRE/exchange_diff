using System;
using System.Net;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class SenderIdValidationBaseContext
	{
		public SenderIdValidationBaseContext(SenderIdValidator senderIdValidator, IPAddress ipAddress, RoutingAddress purportedResponsibleAddress, string helloDomain, SmtpServer server)
		{
			this.senderIdValidator = senderIdValidator;
			this.ipAddress = ipAddress;
			this.purportedResponsibleAddress = purportedResponsibleAddress;
			this.helloDomain = helloDomain;
			this.server = server;
		}

		public SenderIdValidationContext CreateContext(string purportedResponsibleDomain, bool processExpModifier, AsyncCallback asyncCallback, object asyncState)
		{
			SenderIdValidationContext senderIdValidationContext = new SenderIdValidationContext(this, purportedResponsibleDomain, processExpModifier, asyncCallback, asyncState);
			this.numValidations++;
			if (this.numValidations > 10)
			{
				senderIdValidationContext.SetInvalid();
			}
			return senderIdValidationContext;
		}

		public SenderIdValidator SenderIdValidator
		{
			get
			{
				return this.senderIdValidator;
			}
		}

		public IPAddress IPAddress
		{
			get
			{
				return this.ipAddress;
			}
		}

		public RoutingAddress PurportedResponsibleAddress
		{
			get
			{
				return this.purportedResponsibleAddress;
			}
		}

		public string HelloDomain
		{
			get
			{
				return this.helloDomain;
			}
		}

		public SmtpServer Server
		{
			get
			{
				return this.server;
			}
		}

		public bool UsesUncacheableMacro
		{
			get
			{
				return this.usesUncacheableMacro;
			}
		}

		public string ExpandedPMacro
		{
			get
			{
				return this.expandedPMacro;
			}
			set
			{
				this.expandedPMacro = value;
			}
		}

		public void SetUncacheable()
		{
			this.usesUncacheableMacro = true;
		}

		public const int MaxRecursiveValidations = 10;

		private readonly SenderIdValidator senderIdValidator;

		private readonly IPAddress ipAddress;

		private readonly RoutingAddress purportedResponsibleAddress;

		private readonly string helloDomain;

		private readonly SmtpServer server;

		private int numValidations;

		private bool usesUncacheableMacro;

		private string expandedPMacro;
	}
}
