using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PAMCouldNotBeDeterminedException : LocalizedException
	{
		public PAMCouldNotBeDeterminedException(string dagName, string errorMsg) : base(Strings.PAMCouldNotBeDeterminedException(dagName, errorMsg))
		{
			this.dagName = dagName;
			this.errorMsg = errorMsg;
		}

		public PAMCouldNotBeDeterminedException(string dagName, string errorMsg, Exception innerException) : base(Strings.PAMCouldNotBeDeterminedException(dagName, errorMsg), innerException)
		{
			this.dagName = dagName;
			this.errorMsg = errorMsg;
		}

		protected PAMCouldNotBeDeterminedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string dagName;

		private readonly string errorMsg;
	}
}
