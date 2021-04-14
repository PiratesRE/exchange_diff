using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WavToWmaConversionException : AudioConversionException
	{
		public WavToWmaConversionException(string wav, string wma) : base(Strings.WavToWmaConversion(wav, wma))
		{
			this.wav = wav;
			this.wma = wma;
		}

		public WavToWmaConversionException(string wav, string wma, Exception innerException) : base(Strings.WavToWmaConversion(wav, wma), innerException)
		{
			this.wav = wav;
			this.wma = wma;
		}

		protected WavToWmaConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.wav = (string)info.GetValue("wav", typeof(string));
			this.wma = (string)info.GetValue("wma", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("wav", this.wav);
			info.AddValue("wma", this.wma);
		}

		public string Wav
		{
			get
			{
				return this.wav;
			}
		}

		public string Wma
		{
			get
			{
				return this.wma;
			}
		}

		private readonly string wav;

		private readonly string wma;
	}
}
