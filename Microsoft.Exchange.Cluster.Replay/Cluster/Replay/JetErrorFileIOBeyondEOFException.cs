using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JetErrorFileIOBeyondEOFException : TransientException
	{
		public JetErrorFileIOBeyondEOFException(string pageno) : base(ReplayStrings.JetErrorFileIOBeyondEOFException(pageno))
		{
			this.pageno = pageno;
		}

		public JetErrorFileIOBeyondEOFException(string pageno, Exception innerException) : base(ReplayStrings.JetErrorFileIOBeyondEOFException(pageno), innerException)
		{
			this.pageno = pageno;
		}

		protected JetErrorFileIOBeyondEOFException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.pageno = (string)info.GetValue("pageno", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("pageno", this.pageno);
		}

		public string Pageno
		{
			get
			{
				return this.pageno;
			}
		}

		private readonly string pageno;
	}
}
