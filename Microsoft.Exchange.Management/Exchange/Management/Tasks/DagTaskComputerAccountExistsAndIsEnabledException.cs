using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskComputerAccountExistsAndIsEnabledException : LocalizedException
	{
		public DagTaskComputerAccountExistsAndIsEnabledException(string cnoName) : base(Strings.DagTaskComputerAccountExistsAndIsEnabledException(cnoName))
		{
			this.cnoName = cnoName;
		}

		public DagTaskComputerAccountExistsAndIsEnabledException(string cnoName, Exception innerException) : base(Strings.DagTaskComputerAccountExistsAndIsEnabledException(cnoName), innerException)
		{
			this.cnoName = cnoName;
		}

		protected DagTaskComputerAccountExistsAndIsEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
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
