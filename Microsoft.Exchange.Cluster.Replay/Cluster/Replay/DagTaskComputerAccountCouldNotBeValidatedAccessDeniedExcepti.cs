using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException : DagTaskServerException
	{
		public DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(string computerAccount, string userAccount) : base(ReplayStrings.DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(computerAccount, userAccount))
		{
			this.computerAccount = computerAccount;
			this.userAccount = userAccount;
		}

		public DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(string computerAccount, string userAccount, Exception innerException) : base(ReplayStrings.DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(computerAccount, userAccount), innerException)
		{
			this.computerAccount = computerAccount;
			this.userAccount = userAccount;
		}

		protected DagTaskComputerAccountCouldNotBeValidatedAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.computerAccount = (string)info.GetValue("computerAccount", typeof(string));
			this.userAccount = (string)info.GetValue("userAccount", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("computerAccount", this.computerAccount);
			info.AddValue("userAccount", this.userAccount);
		}

		public string ComputerAccount
		{
			get
			{
				return this.computerAccount;
			}
		}

		public string UserAccount
		{
			get
			{
				return this.userAccount;
			}
		}

		private readonly string computerAccount;

		private readonly string userAccount;
	}
}
