using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AudioDataIsOversizeException : LocalizedException
	{
		public AudioDataIsOversizeException(int maxAudioDataMegabytes, long maxGreetingSizeMinutes) : base(Strings.AudioDataIsOversizeException(maxAudioDataMegabytes, maxGreetingSizeMinutes))
		{
			this.maxAudioDataMegabytes = maxAudioDataMegabytes;
			this.maxGreetingSizeMinutes = maxGreetingSizeMinutes;
		}

		public AudioDataIsOversizeException(int maxAudioDataMegabytes, long maxGreetingSizeMinutes, Exception innerException) : base(Strings.AudioDataIsOversizeException(maxAudioDataMegabytes, maxGreetingSizeMinutes), innerException)
		{
			this.maxAudioDataMegabytes = maxAudioDataMegabytes;
			this.maxGreetingSizeMinutes = maxGreetingSizeMinutes;
		}

		protected AudioDataIsOversizeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.maxAudioDataMegabytes = (int)info.GetValue("maxAudioDataMegabytes", typeof(int));
			this.maxGreetingSizeMinutes = (long)info.GetValue("maxGreetingSizeMinutes", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("maxAudioDataMegabytes", this.maxAudioDataMegabytes);
			info.AddValue("maxGreetingSizeMinutes", this.maxGreetingSizeMinutes);
		}

		public int MaxAudioDataMegabytes
		{
			get
			{
				return this.maxAudioDataMegabytes;
			}
		}

		public long MaxGreetingSizeMinutes
		{
			get
			{
				return this.maxGreetingSizeMinutes;
			}
		}

		private readonly int maxAudioDataMegabytes;

		private readonly long maxGreetingSizeMinutes;
	}
}
