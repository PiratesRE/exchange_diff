using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Audio
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedAudioFormat : AudioConversionException
	{
		public UnsupportedAudioFormat(string fileName) : base(NetException.UnsupportedAudioFormat(fileName))
		{
			this.fileName = fileName;
		}

		public UnsupportedAudioFormat(string fileName, Exception innerException) : base(NetException.UnsupportedAudioFormat(fileName), innerException)
		{
			this.fileName = fileName;
		}

		protected UnsupportedAudioFormat(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileName = (string)info.GetValue("fileName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileName", this.fileName);
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		private readonly string fileName;
	}
}
