using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswServerNotAccessibleToDeleteFswException : LocalizedException
	{
		public DagFswServerNotAccessibleToDeleteFswException(string fswMachine, string share, Exception ex) : base(Strings.DagFswServerNotAccessibleToDeleteFswException(fswMachine, share, ex))
		{
			this.fswMachine = fswMachine;
			this.share = share;
			this.ex = ex;
		}

		public DagFswServerNotAccessibleToDeleteFswException(string fswMachine, string share, Exception ex, Exception innerException) : base(Strings.DagFswServerNotAccessibleToDeleteFswException(fswMachine, share, ex), innerException)
		{
			this.fswMachine = fswMachine;
			this.share = share;
			this.ex = ex;
		}

		protected DagFswServerNotAccessibleToDeleteFswException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fswMachine = (string)info.GetValue("fswMachine", typeof(string));
			this.share = (string)info.GetValue("share", typeof(string));
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fswMachine", this.fswMachine);
			info.AddValue("share", this.share);
			info.AddValue("ex", this.ex);
		}

		public string FswMachine
		{
			get
			{
				return this.fswMachine;
			}
		}

		public string Share
		{
			get
			{
				return this.share;
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

		private readonly string share;

		private readonly Exception ex;
	}
}
