using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncompatibleParametersException : MailboxReplicationPermanentException
	{
		public IncompatibleParametersException(string paramName1, string paramName2) : base(Strings.ErrorIncompatibleParameters(paramName1, paramName2))
		{
			this.paramName1 = paramName1;
			this.paramName2 = paramName2;
		}

		public IncompatibleParametersException(string paramName1, string paramName2, Exception innerException) : base(Strings.ErrorIncompatibleParameters(paramName1, paramName2), innerException)
		{
			this.paramName1 = paramName1;
			this.paramName2 = paramName2;
		}

		protected IncompatibleParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.paramName1 = (string)info.GetValue("paramName1", typeof(string));
			this.paramName2 = (string)info.GetValue("paramName2", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("paramName1", this.paramName1);
			info.AddValue("paramName2", this.paramName2);
		}

		public string ParamName1
		{
			get
			{
				return this.paramName1;
			}
		}

		public string ParamName2
		{
			get
			{
				return this.paramName2;
			}
		}

		private readonly string paramName1;

		private readonly string paramName2;
	}
}
