using System;
using System.Deployment.Internal.Isolation;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(false)]
	[Serializable]
	public sealed class ApplicationIdentity : ISerializable
	{
		private ApplicationIdentity()
		{
		}

		[SecurityCritical]
		private ApplicationIdentity(SerializationInfo info, StreamingContext context)
		{
			string text = (string)info.GetValue("FullName", typeof(string));
			if (text == null)
			{
				throw new ArgumentNullException("fullName");
			}
			this._appId = IsolationInterop.AppIdAuthority.TextToDefinition(0U, text);
		}

		[SecuritySafeCritical]
		public ApplicationIdentity(string applicationIdentityFullName)
		{
			if (applicationIdentityFullName == null)
			{
				throw new ArgumentNullException("applicationIdentityFullName");
			}
			this._appId = IsolationInterop.AppIdAuthority.TextToDefinition(0U, applicationIdentityFullName);
		}

		[SecurityCritical]
		internal ApplicationIdentity(IDefinitionAppId applicationIdentity)
		{
			this._appId = applicationIdentity;
		}

		public string FullName
		{
			[SecuritySafeCritical]
			get
			{
				return IsolationInterop.AppIdAuthority.DefinitionToText(0U, this._appId);
			}
		}

		public string CodeBase
		{
			[SecuritySafeCritical]
			get
			{
				return this._appId.get_Codebase();
			}
		}

		public override string ToString()
		{
			return this.FullName;
		}

		internal IDefinitionAppId Identity
		{
			[SecurityCritical]
			get
			{
				return this._appId;
			}
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("FullName", this.FullName, typeof(string));
		}

		private IDefinitionAppId _appId;
	}
}
