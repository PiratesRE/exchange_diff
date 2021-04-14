using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class OverlappingBandDefinitionException : MailboxLoadBalancePermanentException
	{
		public OverlappingBandDefinitionException(string newBand, string existingBand) : base(MigrationWorkflowServiceStrings.ErrorOverlappingBandDefinition(newBand, existingBand))
		{
			this.newBand = newBand;
			this.existingBand = existingBand;
		}

		public OverlappingBandDefinitionException(string newBand, string existingBand, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorOverlappingBandDefinition(newBand, existingBand), innerException)
		{
			this.newBand = newBand;
			this.existingBand = existingBand;
		}

		protected OverlappingBandDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.newBand = (string)info.GetValue("newBand", typeof(string));
			this.existingBand = (string)info.GetValue("existingBand", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("newBand", this.newBand);
			info.AddValue("existingBand", this.existingBand);
		}

		public string NewBand
		{
			get
			{
				return this.newBand;
			}
		}

		public string ExistingBand
		{
			get
			{
				return this.existingBand;
			}
		}

		private readonly string newBand;

		private readonly string existingBand;
	}
}
