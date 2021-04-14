using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswUnableToBindWitnessDirectoryException : LocalizedException
	{
		public DagFswUnableToBindWitnessDirectoryException(string host) : base(Strings.DagFswUnableToBindWitnessDirectoryException(host))
		{
			this.host = host;
		}

		public DagFswUnableToBindWitnessDirectoryException(string host, Exception innerException) : base(Strings.DagFswUnableToBindWitnessDirectoryException(host), innerException)
		{
			this.host = host;
		}

		protected DagFswUnableToBindWitnessDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.host = (string)info.GetValue("host", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("host", this.host);
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		private readonly string host;
	}
}
