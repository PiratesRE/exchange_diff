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
	public abstract class ServerBasedIdParameter : ADIdParameter
	{
		protected ServerBasedIdParameter(string identity) : base(identity)
		{
			this.Initialize(identity);
		}

		protected ServerBasedIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected ServerBasedIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		protected ServerBasedIdParameter()
		{
		}

		internal bool AllowLegacy
		{
			get
			{
				return this.allowLegacy;
			}
			set
			{
				this.allowLegacy = value;
			}
		}

		protected abstract ServerRole RoleRestriction { get; }

		protected string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		protected string CommonName { get; set; }

		protected ServerIdParameter ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		private static string LocalServerFQDN
		{
			get
			{
				return NativeHelpers.GetLocalComputerFqdn(true);
			}
		}

		public override string ToString()
		{
			if (base.InternalADObjectId == null)
			{
				string str = "";
				if (!string.IsNullOrEmpty(this.serverName))
				{
					str = this.serverName + '\\';
				}
				return str + this.CommonName;
			}
			return base.ToString();
		}

		internal override void Initialize(ObjectId objectId)
		{
			if (!string.IsNullOrEmpty(this.ServerName))
			{
				throw new InvalidOperationException(Strings.ErrorChangeImmutableType);
			}
			base.Initialize(objectId);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			IEnumerable<T> enumerable = null;
			List<T> list = new List<T>();
			notFoundReason = null;
			int num = 0;
			int num2 = 0;
			if (base.InternalADObjectId != null)
			{
				enumerable = base.GetADObjectIdObjects<T>(base.InternalADObjectId, rootId, subTreeSession, optionalData);
			}
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(enumerable);
			if (wrapper.HasElements())
			{
				using (IEnumerator<T> enumerator = wrapper.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T item = enumerator.Current;
						if (ServerIdParameter.HasRole((ADObjectId)item.Identity, this.RoleRestriction, (IConfigDataProvider)session) || (this.AllowLegacy && !ServerIdParameter.HasRole((ADObjectId)item.Identity, ServerRole.All, (IConfigDataProvider)session)))
						{
							list.Add(item);
						}
						else if (!ServerIdParameter.HasRole((ADObjectId)item.Identity, ServerRole.All, (IConfigDataProvider)session))
						{
							num2++;
						}
						else
						{
							num++;
						}
					}
					goto IL_21B;
				}
			}
			if (!string.IsNullOrEmpty(this.CommonName) && this.ServerId != null)
			{
				ADObjectId[] matchingIdentities = this.ServerId.GetMatchingIdentities((IConfigDataProvider)session);
				foreach (ADObjectId rootId2 in matchingIdentities)
				{
					enumerable = base.GetObjectsInOrganization<T>(this.CommonName, rootId2, session, optionalData);
					foreach (T item2 in enumerable)
					{
						if (ServerIdParameter.HasRole((ADObjectId)item2.Identity, this.RoleRestriction, (IConfigDataProvider)session) || (this.AllowLegacy && !ServerIdParameter.HasRole((ADObjectId)item2.Identity, ServerRole.All, (IConfigDataProvider)session)))
						{
							list.Add(item2);
						}
						else if (!ServerIdParameter.HasRole((ADObjectId)item2.Identity, ServerRole.All, (IConfigDataProvider)session))
						{
							num2++;
						}
						else
						{
							num++;
						}
					}
				}
			}
			IL_21B:
			if (list.Count == 0)
			{
				if (num2 != 0)
				{
					notFoundReason = new LocalizedString?(Strings.ExceptionLegacyObjects(this.ToString()));
				}
				if (num != 0)
				{
					notFoundReason = new LocalizedString?(Strings.ExceptionRoleNotFoundObjects(this.ToString()));
				}
			}
			return list;
		}

		protected virtual void Initialize(string identity)
		{
			if (base.InternalADObjectId != null && base.InternalADObjectId.Rdn != null)
			{
				return;
			}
			string[] array = identity.Split(new char[]
			{
				'\\'
			});
			if (array.Length > 2)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
			if (array.Length == 2)
			{
				this.serverName = array[0];
				this.CommonName = array[1];
			}
			else if (array.Length == 1)
			{
				this.serverName = ServerBasedIdParameter.LocalServerFQDN;
				this.CommonName = array[0];
			}
			if (string.IsNullOrEmpty(this.serverName) || string.IsNullOrEmpty(this.CommonName))
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
			try
			{
				this.serverId = ServerIdParameter.Parse(this.serverName);
			}
			catch (ArgumentException)
			{
			}
		}

		private string serverName;

		private bool allowLegacy;

		private ServerIdParameter serverId;
	}
}
