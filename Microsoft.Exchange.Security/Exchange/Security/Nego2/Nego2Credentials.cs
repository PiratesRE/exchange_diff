using System;
using System.Globalization;
using System.Net;

namespace Microsoft.Exchange.Security.Nego2
{
	public class Nego2Credentials : ICredentials
	{
		public string UserName { get; private set; }

		public string Password { get; private set; }

		public bool IsBusinessInstance { get; private set; }

		public Nego2Credentials(string userName, string password) : this(userName, password, false)
		{
		}

		public Nego2Credentials(string userName, string password, bool isBusinessInstance)
		{
			this.UserName = userName;
			this.Password = password;
			this.IsBusinessInstance = isBusinessInstance;
		}

		public NetworkCredential GetCredential(Uri uri, string authType)
		{
			return this.GetAuthBuffer<NetworkCredential>(new Func<IntPtr, NetworkCredential>(Nego2NativeHelper.GetCredential));
		}

		public NetworkCredential GetCredential()
		{
			return this.GetAuthBuffer<NetworkCredential>(new Func<IntPtr, NetworkCredential>(Nego2NativeHelper.GetCredential));
		}

		private TResult GetAuthBuffer<TResult>(Func<IntPtr, TResult> create)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			TResult result;
			try
			{
				Nego2NativeHelper.CreateLiveClientAuthBufferWithPlainPassword(this.UserName, this.Password, 0U, this.IsBusinessInstance, out zero, out zero2);
				result = create(zero);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					int num = Nego2NativeHelper.FreeAuthBuffer(zero);
					if (num < 0)
					{
						throw new Exception(string.Format(CultureInfo.InvariantCulture, "Failure deallocating the authentication buffer. Error: {0}", new object[]
						{
							num
						}));
					}
				}
			}
			return result;
		}
	}
}
