using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_InitializeError : LocalizedException
	{
		public TUC_InitializeError(string error) : base(Strings.InitializeError(error))
		{
			this.error = error;
		}

		public TUC_InitializeError(string error, Exception innerException) : base(Strings.InitializeError(error), innerException)
		{
			this.error = error;
		}

		protected TUC_InitializeError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
