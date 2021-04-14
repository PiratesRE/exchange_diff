using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TooFewMailboxServersToStartException : LocalizedException
	{
		public TooFewMailboxServersToStartException(string dag, int number) : base(Strings.TooFewMailboxServersToStart(dag, number))
		{
			this.dag = dag;
			this.number = number;
		}

		public TooFewMailboxServersToStartException(string dag, int number, Exception innerException) : base(Strings.TooFewMailboxServersToStart(dag, number), innerException)
		{
			this.dag = dag;
			this.number = number;
		}

		protected TooFewMailboxServersToStartException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dag = (string)info.GetValue("dag", typeof(string));
			this.number = (int)info.GetValue("number", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dag", this.dag);
			info.AddValue("number", this.number);
		}

		public string Dag
		{
			get
			{
				return this.dag;
			}
		}

		public int Number
		{
			get
			{
				return this.number;
			}
		}

		private readonly string dag;

		private readonly int number;
	}
}
