﻿using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.Text
{
	[Serializable]
	internal sealed class MLangCodePageEncoding : ISerializable, IObjectReference
	{
		internal MLangCodePageEncoding(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.m_codePage = (int)info.GetValue("m_codePage", typeof(int));
			try
			{
				this.m_isReadOnly = (bool)info.GetValue("m_isReadOnly", typeof(bool));
				this.encoderFallback = (EncoderFallback)info.GetValue("encoderFallback", typeof(EncoderFallback));
				this.decoderFallback = (DecoderFallback)info.GetValue("decoderFallback", typeof(DecoderFallback));
			}
			catch (SerializationException)
			{
				this.m_deserializedFromEverett = true;
				this.m_isReadOnly = true;
			}
		}

		[SecurityCritical]
		public object GetRealObject(StreamingContext context)
		{
			this.realEncoding = Encoding.GetEncoding(this.m_codePage);
			if (!this.m_deserializedFromEverett && !this.m_isReadOnly)
			{
				this.realEncoding = (Encoding)this.realEncoding.Clone();
				this.realEncoding.EncoderFallback = this.encoderFallback;
				this.realEncoding.DecoderFallback = this.decoderFallback;
			}
			return this.realEncoding;
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new ArgumentException(Environment.GetResourceString("Arg_ExecutionEngineException"));
		}

		[NonSerialized]
		private int m_codePage;

		[NonSerialized]
		private bool m_isReadOnly;

		[NonSerialized]
		private bool m_deserializedFromEverett;

		[NonSerialized]
		private EncoderFallback encoderFallback;

		[NonSerialized]
		private DecoderFallback decoderFallback;

		[NonSerialized]
		private Encoding realEncoding;

		[Serializable]
		internal sealed class MLangEncoder : ISerializable, IObjectReference
		{
			internal MLangEncoder(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this.realEncoding = (Encoding)info.GetValue("m_encoding", typeof(Encoding));
			}

			[SecurityCritical]
			public object GetRealObject(StreamingContext context)
			{
				return this.realEncoding.GetEncoder();
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_ExecutionEngineException"));
			}

			[NonSerialized]
			private Encoding realEncoding;
		}

		[Serializable]
		internal sealed class MLangDecoder : ISerializable, IObjectReference
		{
			internal MLangDecoder(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this.realEncoding = (Encoding)info.GetValue("m_encoding", typeof(Encoding));
			}

			[SecurityCritical]
			public object GetRealObject(StreamingContext context)
			{
				return this.realEncoding.GetDecoder();
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_ExecutionEngineException"));
			}

			[NonSerialized]
			private Encoding realEncoding;
		}
	}
}
