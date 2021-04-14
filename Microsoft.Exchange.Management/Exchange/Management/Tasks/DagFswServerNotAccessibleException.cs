using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswServerNotAccessibleException : LocalizedException
	{
		public DagFswServerNotAccessibleException(string fswMachine, Exception ex) : base(Strings.DagFswServerNotAccessibleException(fswMachine, ex))
		{
			this.fswMachine = fswMachine;
			this.ex = ex;
		}

		public DagFswServerNotAccessibleException(string fswMachine, Exception ex, Exception innerException) : base(Strings.DagFswServerNotAccessibleException(fswMachine, ex), innerException)
		{
			this.fswMachine = fswMachine;
			this.ex = ex;
		}

		protected DagFswServerNotAccessibleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fswMachine = (string)info.GetValue("fswMachine", typeof(string));
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fswMachine", this.fswMachine);
			info.AddValue("ex", this.ex);
		}

		public string FswMachine
		{
			get
			{
				return this.fswMachine;
			}
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly string fswMachine;

		private readonly Exception ex;
	}
}
