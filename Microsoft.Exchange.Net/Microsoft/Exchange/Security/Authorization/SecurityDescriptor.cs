using System;
using System.Diagnostics;
using System.Security.AccessControl;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Security.Authorization
{
	[DebuggerDisplay("RawSecurityDescriptor = {RawSecurityDescriptor}")]
	public class SecurityDescriptor
	{
		public SecurityDescriptor(byte[] binaryForm)
		{
			this.binaryForm = binaryForm;
		}

		public byte[] BinaryForm
		{
			get
			{
				return this.binaryForm;
			}
		}

		public static SecurityDescriptor FromRawSecurityDescriptor(RawSecurityDescriptor rawSecurityDescriptor)
		{
			if (rawSecurityDescriptor != null)
			{
				byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
				rawSecurityDescriptor.GetBinaryForm(array, 0);
				return new SecurityDescriptor(array);
			}
			return null;
		}

		public RawSecurityDescriptor ToRawSecurityDescriptorThrow()
		{
			if (this.binaryForm == null)
			{
				return null;
			}
			return new RawSecurityDescriptor(this.binaryForm, 0);
		}

		public RawSecurityDescriptor ToRawSecurityDescriptor()
		{
			RawSecurityDescriptor result = null;
			if (this.binaryForm == null)
			{
				return null;
			}
			Exception ex = null;
			try
			{
				result = new RawSecurityDescriptor(this.binaryForm, 0);
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentNullException ex3)
			{
				ex = ex3;
			}
			catch (ArgumentException ex4)
			{
				ex = ex4;
			}
			catch (FormatException ex5)
			{
				ex = ex5;
			}
			catch (OverflowException ex6)
			{
				ex = ex6;
			}
			catch (InvalidOperationException ex7)
			{
				ex = ex7;
			}
			if (ex != null)
			{
				ExTraceGlobals.AuthorizationTracer.TraceWarning<Type, string>(0L, "SecurityDescriptor::RawSecurityDescriptor - failed to create RawSecurityDescriptor with {0}, {1}", ex.GetType(), ex.Message);
				return null;
			}
			return result;
		}

		private byte[] binaryForm;
	}
}
