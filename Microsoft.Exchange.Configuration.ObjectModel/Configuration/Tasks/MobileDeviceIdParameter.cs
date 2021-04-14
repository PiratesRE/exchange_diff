using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MobileDeviceIdParameter : ADIdParameter
	{
		public MobileDeviceIdParameter()
		{
		}

		public MobileDeviceIdParameter(string identity) : base(identity)
		{
		}

		public MobileDeviceIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public MobileDeviceIdParameter(MobileDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			this.Initialize(device.Id);
		}

		public MobileDeviceIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal string DeviceId
		{
			get
			{
				if (this.deviceId == null)
				{
					this.ComputeIdAndDeviceTypeAndClientType();
				}
				return this.deviceId;
			}
		}

		internal string DeviceType
		{
			get
			{
				if (this.deviceType == null)
				{
					this.ComputeIdAndDeviceTypeAndClientType();
				}
				return this.deviceType;
			}
		}

		internal MobileClientType ClientType
		{
			get
			{
				if (this.clientType == null)
				{
					this.ComputeIdAndDeviceTypeAndClientType();
				}
				return this.clientType.Value;
			}
		}

		public static MobileDeviceIdParameter Parse(string identity)
		{
			return new MobileDeviceIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (!typeof(MobileDevice).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			notFoundReason = new LocalizedString?(Strings.WrongActiveSyncDeviceIdParameter(this.ToString()));
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetExactMatchObjects<T>(rootId, subTreeSession, optionalData));
			if (!wrapper.HasElements() && base.RawIdentity != null)
			{
				string[] array = base.RawIdentity.Split(new char[]
				{
					'\\'
				});
				if (array.Length == 3)
				{
					string text = array[0];
					string text2 = array[2];
					if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
					{
						wrapper = EnumerableWrapper<T>.GetWrapper(this.GetObjectsInOrganization<T>(text2, rootId, session, optionalData), new MobileDeviceIdParameter.MobileDeviceUsernameFilter<T>(text));
					}
				}
			}
			return wrapper;
		}

		internal MailboxIdParameter GetMailboxId()
		{
			ADObjectId internalADObjectId = base.InternalADObjectId;
			if (internalADObjectId == null)
			{
				ADIdParameter.TryResolveCanonicalName(base.RawIdentity, out internalADObjectId);
			}
			if (internalADObjectId == null || internalADObjectId.Parent == null || internalADObjectId.Parent.Parent == null)
			{
				return null;
			}
			return new MailboxIdParameter(internalADObjectId.Parent.Parent);
		}

		private string ParseDeviceType(string part)
		{
			string text = "ExchangeActiveSyncDevices/";
			int num = part.IndexOf(text, StringComparison.OrdinalIgnoreCase);
			string result;
			if (0 <= num)
			{
				result = part.Substring(num + text.Length);
			}
			else
			{
				result = part;
			}
			return result;
		}

		private void ComputeIdAndDeviceTypeAndClientType()
		{
			string text = null;
			if (base.InternalADObjectId != null)
			{
				text = base.InternalADObjectId.Rdn.UnescapedName;
			}
			else if (!string.IsNullOrEmpty(base.RawIdentity))
			{
				int num = base.RawIdentity.LastIndexOf('\\');
				text = ((num != -1 && num < base.RawIdentity.Length - 1) ? base.RawIdentity.Substring(num + 1) : base.RawIdentity);
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException("Id is null or empty");
			}
			string[] array = text.Split(this.TypeIdSeparatorChars, 3);
			if (array.Length == 3)
			{
				this.deviceId = array[2];
				this.deviceType = this.ParseDeviceType(array[1]);
				this.clientType = new MobileClientType?(string.Equals("MOWA", array[0], StringComparison.OrdinalIgnoreCase) ? MobileClientType.MOWA : MobileClientType.EAS);
				return;
			}
			if (array.Length == 2)
			{
				this.deviceId = array[1];
				this.deviceType = this.ParseDeviceType(array[0]);
				this.clientType = new MobileClientType?(MobileClientType.EAS);
				return;
			}
			throw new ArgumentException("Id is invalid.");
		}

		public const char TypeIdSeparatorChar = '§';

		private readonly char[] TypeIdSeparatorChars = new char[]
		{
			'§'
		};

		private string deviceId;

		private string deviceType;

		private MobileClientType? clientType;

		private class MobileDeviceUsernameFilter<T> : IEnumerableFilter<T> where T : IConfigurable
		{
			public MobileDeviceUsernameFilter(string username)
			{
				if (username == null)
				{
					throw new ArgumentNullException("username", username);
				}
				this.username = username;
			}

			public bool AcceptElement(T element)
			{
				if (element == null)
				{
					return false;
				}
				ADObjectId adobjectId = element.Identity as ADObjectId;
				return adobjectId != null && adobjectId.Parent != null && adobjectId.Parent.Parent != null && string.Equals(adobjectId.Parent.Parent.Name, this.username, StringComparison.OrdinalIgnoreCase);
			}

			private readonly string username;
		}
	}
}
