using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class MissingMethodException : MissingMemberException, ISerializable
	{
		[__DynamicallyInvokable]
		public MissingMethodException() : base(Environment.GetResourceString("Arg_MissingMethodException"))
		{
			base.SetErrorCode(-2146233069);
		}

		[__DynamicallyInvokable]
		public MissingMethodException(string message) : base(message)
		{
			base.SetErrorCode(-2146233069);
		}

		[__DynamicallyInvokable]
		public MissingMethodException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233069);
		}

		protected MissingMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
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
				return Environment.GetResourceString("MissingMethod_Name", new object[]
				{
					this.ClassName + "." + this.MemberName + ((this.Signature != null) ? (" " + MissingMemberException.FormatSignature(this.Signature)) : "")
				});
			}
		}

		private MissingMethodException(string className, string methodName, byte[] signature)
		{
			this.ClassName = className;
			this.MemberName = methodName;
			this.Signature = signature;
		}

		public MissingMethodException(string className, string methodName)
		{
			this.ClassName = className;
			this.MemberName = methodName;
		}
	}
}
