using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagCantBeSetIntoDatacenterActivationModeException : LocalizedException
	{
		public DagCantBeSetIntoDatacenterActivationModeException(string dagName) : base(Strings.DagCantBeSetIntoDatacenterActivationMode(dagName))
		{
			this.dagName = dagName;
		}

		public DagCantBeSetIntoDatacenterActivationModeException(string dagName, Exception innerException) : base(Strings.DagCantBeSetIntoDatacenterActivationMode(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected DagCantBeSetIntoDatacenterActivationModeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string dagName;
	}
}
