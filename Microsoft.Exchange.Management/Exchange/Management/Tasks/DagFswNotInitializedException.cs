using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswNotInitializedException : LocalizedException
	{
		public DagFswNotInitializedException(string ex) : base(Strings.DagFswNotInitializedException(ex))
		{
			this.ex = ex;
		}

		public DagFswNotInitializedException(string ex, Exception innerException) : base(Strings.DagFswNotInitializedException(ex), innerException)
		{
			this.ex = ex;
		}

		protected DagFswNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ex = (string)info.GetValue("ex", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ex", this.ex);
		}

		public string Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly string ex;
	}
}
