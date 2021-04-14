using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class MissingMemberException : MemberAccessException, ISerializable
	{
		[__DynamicallyInvokable]
		public MissingMemberException() : base(Environment.GetResourceString("Arg_MissingMemberException"))
		{
			base.SetErrorCode(-2146233070);
		}

		[__DynamicallyInvokable]
		public MissingMemberException(string message) : base(message)
		{
			base.SetErrorCode(-2146233070);
		}

		[__DynamicallyInvokable]
		public MissingMemberException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233070);
		}

		protected MissingMemberException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ClassName = info.GetString("MMClassName");
			this.MemberName = info.GetString("MMMemberName");
			this.Signature = (byte[])info.GetValue("MMSignature", typeof(byte[]));
		}

		[__DynamicallyInvokable]
		public override string Message
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				if (this.ClassName == null)
				{
					return base.Message;
				}
				return Environment.GetResourceString("MissingMember_Name", new object[]
				{
					this.ClassName + "." + this.MemberName + ((this.Signature != null) ? (" " + MissingMemberException.FormatSignature(this.Signature)) : "")
				});
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string FormatSignature(byte[] signature);

		private MissingMemberException(string className, string memberName, byte[] signature)
		{
			this.ClassName = className;
			this.MemberName = memberName;
			this.Signature = signature;
		}

		public MissingMemberException(string className, string memberName)
		{
			this.ClassName = className;
			this.MemberName = memberName;
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("MMClassName", this.ClassName, typeof(string));
			info.AddValue("MMMemberName", this.MemberName, typeof(string));
			info.AddValue("MMSignature", this.Signature, typeof(byte[]));
		}

		protected string ClassName;

		protected string MemberName;

		protected byte[] Signature;
	}
}
