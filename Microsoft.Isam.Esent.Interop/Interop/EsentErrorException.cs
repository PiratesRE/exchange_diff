using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class EsentErrorException : EsentException
	{
		internal EsentErrorException(string message, JET_err err) : base(message)
		{
			this.errorCode = err;
		}

		protected EsentErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorCode = (JET_err)info.GetInt32("errorCode");
		}

		public JET_err Error
		{
			get
			{
				return this.errorCode;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("errorCode", this.errorCode, typeof(int));
			}
		}

		private JET_err errorCode;
	}
}
