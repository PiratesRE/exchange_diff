using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptLogDetectedException : TransientException
	{
		public CorruptLogDetectedException(string filename, string errorText) : base(ReplayStrings.CorruptLogDetectedError(filename, errorText))
		{
			this.filename = filename;
			this.errorText = errorText;
		}

		public CorruptLogDetectedException(string filename, string errorText, Exception innerException) : base(ReplayStrings.CorruptLogDetectedError(filename, errorText), innerException)
		{
			this.filename = filename;
			this.errorText = errorText;
		}

		protected CorruptLogDetectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filename = (string)info.GetValue("filename", typeof(string));
			this.errorText = (string)info.GetValue("errorText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filename", this.filename);
			info.AddValue("errorText", this.errorText);
		}

		public string Filename
		{
			get
			{
				return this.filename;
			}
		}

		public string ErrorText
		{
			get
			{
				return this.errorText;
			}
		}

		private readonly string filename;

		private readonly string errorText;
	}
}
