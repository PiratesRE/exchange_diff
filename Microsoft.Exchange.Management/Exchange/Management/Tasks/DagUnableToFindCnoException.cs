using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagUnableToFindCnoException : LocalizedException
	{
		public DagUnableToFindCnoException(string cnoName) : base(Strings.DagUnableToFindCnoError(cnoName))
		{
			this.cnoName = cnoName;
		}

		public DagUnableToFindCnoException(string cnoName, Exception innerException) : base(Strings.DagUnableToFindCnoError(cnoName), innerException)
		{
			this.cnoName = cnoName;
		}

		protected DagUnableToFindCnoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.cnoName = (string)info.GetValue("cnoName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("cnoName", this.cnoName);
		}

		public string CnoName
		{
			get
			{
				return this.cnoName;
			}
		}

		private readonly string cnoName;
	}
}
