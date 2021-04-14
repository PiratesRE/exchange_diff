using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AppIdParameter : IIdentityParameter
	{
		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawInput;
			}
		}

		internal AppId InternalOWAExtensionId
		{
			get
			{
				return this.internalOWAExtensionId;
			}
			set
			{
				this.internalOWAExtensionId = value;
			}
		}

		internal string RawExtensionName
		{
			get
			{
				return this.rawExtensionName;
			}
		}

		internal MailboxIdParameter RawMailbox
		{
			get
			{
				return this.rawMailbox;
			}
		}

		public AppIdParameter()
		{
		}

		public AppIdParameter(ConfigurableObject configurableObject)
		{
			if (configurableObject == null)
			{
				throw new ArgumentNullException("configurableObject");
			}
			((IIdentityParameter)this).Initialize(configurableObject.Identity);
			this.rawInput = configurableObject.Identity.ToString();
		}

		public AppIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			if (string.IsNullOrEmpty(this.rawExtensionName))
			{
				this.rawExtensionName = namedIdentity.DisplayName;
			}
		}

		public AppIdParameter(string extensionId)
		{
			if (string.IsNullOrEmpty(extensionId))
			{
				throw new ArgumentNullException("extensionId");
			}
			this.rawInput = extensionId;
			string text = string.Empty;
			int num = extensionId.Length;
			int num2;
			do
			{
				num2 = num;
				num = extensionId.LastIndexOf("\\\\", num2 - 1, num2);
			}
			while (num != -1);
			int num3 = extensionId.LastIndexOf('\\', num2 - 1, num2);
			if (num3 == extensionId.Length - 1)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("extensionId"), "extensionId");
			}
			string text2;
			if (num3 >= 0)
			{
				text2 = extensionId.Substring(0, num3);
				text = extensionId.Substring(1 + num3);
			}
			else
			{
				text = extensionId;
				text2 = null;
			}
			if (num2 != extensionId.Length)
			{
				text = text.Replace("\\\\", '\\'.ToString());
			}
			if (!string.IsNullOrEmpty(text2) && !text2.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				this.rawMailbox = new MailboxIdParameter(text2);
			}
			this.rawExtensionName = text;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (null == this.internalOWAExtensionId)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			IConfigurable[] array = session.Find<T>(null, this.internalOWAExtensionId, false, null);
			if (array == null || array.Length == 0)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				return new T[0];
			}
			notFoundReason = null;
			T[] array2 = new T[array.Length];
			int num = 0;
			foreach (IConfigurable configurable in array)
			{
				array2[num++] = (T)((object)configurable);
			}
			return array2;
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (!(objectId is AppId))
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.internalOWAExtensionId = (AppId)objectId;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.RawExtensionName))
			{
				return this.RawExtensionName;
			}
			if (this.InternalOWAExtensionId != null)
			{
				return this.InternalOWAExtensionId.ToString();
			}
			return this.rawInput;
		}

		private readonly MailboxIdParameter rawMailbox;

		private readonly string rawInput;

		private readonly string rawExtensionName;

		private AppId internalOWAExtensionId;
	}
}
