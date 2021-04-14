using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagServerCantBeRemovedInDatacenterActivationMode : LocalizedException
	{
		public DagServerCantBeRemovedInDatacenterActivationMode(string mailbox, string dagName) : base(Strings.DagServerCantBeRemovedInDatacenterActivationMode(mailbox, dagName))
		{
			this.mailbox = mailbox;
			this.dagName = dagName;
		}

		public DagServerCantBeRemovedInDatacenterActivationMode(string mailbox, string dagName, Exception innerException) : base(Strings.DagServerCantBeRemovedInDatacenterActivationMode(mailbox, dagName), innerException)
		{
			this.mailbox = mailbox;
			this.dagName = dagName;
		}

		protected DagServerCantBeRemovedInDatacenterActivationMode(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
			info.AddValue("dagName", this.dagName);
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string mailbox;

		private readonly string dagName;
	}
}
