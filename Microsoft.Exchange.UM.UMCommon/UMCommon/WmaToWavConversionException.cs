using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WmaToWavConversionException : AudioConversionException
	{
		public WmaToWavConversionException(string wma, string wav) : base(Strings.WmaToWavConversion(wma, wav))
		{
			this.wma = wma;
			this.wav = wav;
		}

		public WmaToWavConversionException(string wma, string wav, Exception innerException) : base(Strings.WmaToWavConversion(wma, wav), innerException)
		{
			this.wma = wma;
			this.wav = wav;
		}

		protected WmaToWavConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.wma = (string)info.GetValue("wma", typeof(string));
			this.wav = (string)info.GetValue("wav", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("wma", this.wma);
			info.AddValue("wav", this.wav);
		}

		public string Wma
		{
			get
			{
				return this.wma;
			}
		}

		public string Wav
		{
			get
			{
				return this.wav;
			}
		}

		private readonly string wma;

		private readonly string wav;
	}
}
