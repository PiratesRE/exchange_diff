using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskNoServersAreStartedException : LocalizedException
	{
		public DagTaskNoServersAreStartedException(string dagName) : base(Strings.DagTaskNoServersAreStartedException(dagName))
		{
			this.dagName = dagName;
		}

		public DagTaskNoServersAreStartedException(string dagName, Exception innerException) : base(Strings.DagTaskNoServersAreStartedException(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected DagTaskNoServersAreStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
