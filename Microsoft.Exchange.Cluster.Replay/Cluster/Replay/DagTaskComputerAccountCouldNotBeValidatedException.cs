using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskComputerAccountCouldNotBeValidatedException : DagTaskServerException
	{
		public DagTaskComputerAccountCouldNotBeValidatedException(string computerAccount, string userAccount, string error) : base(ReplayStrings.DagTaskComputerAccountCouldNotBeValidatedException(computerAccount, userAccount, error))
		{
			this.computerAccount = computerAccount;
			this.userAccount = userAccount;
			this.error = error;
		}

		public DagTaskComputerAccountCouldNotBeValidatedException(string computerAccount, string userAccount, string error, Exception innerException) : base(ReplayStrings.DagTaskComputerAccountCouldNotBeValidatedException(computerAccount, userAccount, error), innerException)
		{
			this.computerAccount = computerAccount;
			this.userAccount = userAccount;
			this.error = error;
		}

		protected DagTaskComputerAccountCouldNotBeValidatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.computerAccount = (string)info.GetValue("computerAccount", typeof(string));
			this.userAccount = (string)info.GetValue("userAccount", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("computerAccount", this.computerAccount);
			info.AddValue("userAccount", this.userAccount);
			info.AddValue("error", this.error);
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

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string computerAccount;

		private readonly string userAccount;

		private readonly string error;
	}
}
