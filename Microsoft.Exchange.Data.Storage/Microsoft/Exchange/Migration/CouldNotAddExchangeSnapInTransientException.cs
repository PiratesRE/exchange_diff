using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CouldNotAddExchangeSnapInTransientException : MigrationTransientException
	{
		public CouldNotAddExchangeSnapInTransientException(string snapInName) : base(Strings.CouldNotAddExchangeSnapIn(snapInName))
		{
			this.snapInName = snapInName;
		}

		public CouldNotAddExchangeSnapInTransientException(string snapInName, Exception innerException) : base(Strings.CouldNotAddExchangeSnapIn(snapInName), innerException)
		{
			this.snapInName = snapInName;
		}

		protected CouldNotAddExchangeSnapInTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.snapInName = (string)info.GetValue("snapInName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("snapInName", this.snapInName);
		}

		public string SnapInName
		{
			get
			{
				return this.snapInName;
			}
		}

		private readonly string snapInName;
	}
}
