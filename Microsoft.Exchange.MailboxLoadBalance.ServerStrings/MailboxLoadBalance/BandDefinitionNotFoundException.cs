using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BandDefinitionNotFoundException : MailboxLoadBalancePermanentException
	{
		public BandDefinitionNotFoundException(string band) : base(MigrationWorkflowServiceStrings.ErrorBandDefinitionNotFound(band))
		{
			this.band = band;
		}

		public BandDefinitionNotFoundException(string band, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorBandDefinitionNotFound(band), innerException)
		{
			this.band = band;
		}

		protected BandDefinitionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.band = (string)info.GetValue("band", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("band", this.band);
		}

		public string Band
		{
			get
			{
				return this.band;
			}
		}

		private readonly string band;
	}
}
