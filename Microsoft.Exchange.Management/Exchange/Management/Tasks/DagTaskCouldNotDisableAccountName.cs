using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskCouldNotDisableAccountName : LocalizedException
	{
		public DagTaskCouldNotDisableAccountName(string dagName, Exception ex) : base(Strings.DagTaskCouldNotDisableAccountName(dagName, ex))
		{
			this.dagName = dagName;
			this.ex = ex;
		}

		public DagTaskCouldNotDisableAccountName(string dagName, Exception ex, Exception innerException) : base(Strings.DagTaskCouldNotDisableAccountName(dagName, ex), innerException)
		{
			this.dagName = dagName;
			this.ex = ex;
		}

		protected DagTaskCouldNotDisableAccountName(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
			info.AddValue("ex", this.ex);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly string dagName;

		private readonly Exception ex;
	}
}
