using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmFailedToDeterminePAM : AmServerTransientException
	{
		public AmFailedToDeterminePAM(string dagName) : base(ServerStrings.AmFailedToDeterminePAM(dagName))
		{
			this.dagName = dagName;
		}

		public AmFailedToDeterminePAM(string dagName, Exception innerException) : base(ServerStrings.AmFailedToDeterminePAM(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected AmFailedToDeterminePAM(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string dagName;
	}
}
