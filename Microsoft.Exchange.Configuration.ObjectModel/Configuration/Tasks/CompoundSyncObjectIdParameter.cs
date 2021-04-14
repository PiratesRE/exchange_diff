using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public abstract class CompoundSyncObjectIdParameter : IIdentityParameter
	{
		protected CompoundSyncObjectIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.RawIdentity = identity;
			this.InitializeServiceAndObjectIds();
			this.CheckNoWildcardedServiceInstnaceIdParses();
			this.CheckNoWildcardedSyncObjectIdParses();
			this.CheckOnlyOneWildCardAtTheStartOrEnd();
			this.CheckObjectClassIsValid();
		}

		protected CompoundSyncObjectIdParameter(ObjectId compoundSyncObjectId)
		{
			this.Initialize(compoundSyncObjectId);
		}

		protected CompoundSyncObjectIdParameter()
		{
		}

		public string RawIdentity { get; private set; }

		internal string ServiceInstanceIdentity { get; private set; }

		internal string SyncObjectIdentity { get; private set; }

		internal bool IsServiceInstanceDefinied
		{
			get
			{
				return this.serviceInstance != null;
			}
		}

		internal ServiceInstanceId ServiceInstance
		{
			get
			{
				return this.serviceInstance;
			}
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public abstract IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new();

		public void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			CompoundSyncObjectId compoundSyncObjectId = objectId as CompoundSyncObjectId;
			if (compoundSyncObjectId == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("objectId", typeof(CompoundSyncObjectId).Name), "objectId");
			}
			this.serviceInstance = compoundSyncObjectId.ServiceInstanceId;
			this.ServiceInstanceIdentity = this.serviceInstance.ToString();
			this.syncObjectId = compoundSyncObjectId.SyncObjectId;
			this.SyncObjectIdentity = this.syncObjectId.ToString();
			this.RawIdentity = compoundSyncObjectId.ToString();
		}

		public override string ToString()
		{
			return string.Format("{0}\\{1}", this.ServiceInstanceIdentity, this.SyncObjectIdentity);
		}

		protected void GetSyncObjectIdElements(out string contextIdString, out string objectIdString, out string objectClassString)
		{
			if (this.syncObjectId != null)
			{
				contextIdString = this.syncObjectId.ContextId;
				objectIdString = this.syncObjectId.ObjectId;
				objectClassString = this.syncObjectId.ObjectClass.ToString();
				return;
			}
			contextIdString = null;
			objectIdString = null;
			objectClassString = null;
			string[] identityElements = SyncObjectId.GetIdentityElements(this.SyncObjectIdentity);
			if (this.SyncObjectIdentity.EndsWith("*"))
			{
				switch (identityElements.Length)
				{
				case 1:
					goto IL_8A;
				case 2:
					break;
				case 3:
					objectIdString = identityElements[2];
					break;
				default:
					return;
				}
				objectClassString = identityElements[1];
				IL_8A:
				contextIdString = identityElements[0];
				return;
			}
			switch (identityElements.Length)
			{
			case 1:
				objectIdString = identityElements[0];
				return;
			case 2:
				objectIdString = identityElements[1];
				objectClassString = identityElements[0];
				return;
			case 3:
				objectIdString = identityElements[2];
				objectClassString = identityElements[1];
				contextIdString = identityElements[0];
				return;
			default:
				return;
			}
		}

		private static bool TryParseSyncObjectId(string identity, out SyncObjectId syncObjectId)
		{
			syncObjectId = null;
			try
			{
				syncObjectId = SyncObjectId.Parse(identity);
			}
			catch (ArgumentException)
			{
				return false;
			}
			return true;
		}

		private static bool TryParseServiceInstanceId(string identity, out ServiceInstanceId serviceInstanceId)
		{
			serviceInstanceId = null;
			try
			{
				serviceInstanceId = new ServiceInstanceId(identity);
			}
			catch (InvalidServiceInstanceIdException)
			{
				return false;
			}
			return true;
		}

		private void InitializeServiceAndObjectIds()
		{
			string[] array = this.RawIdentity.Split(CompoundSyncObjectIdParameter.Separators);
			if (array.Length > 2)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
			if (array.Length == 2)
			{
				this.ServiceInstanceIdentity = array[0];
				this.SyncObjectIdentity = array[1];
			}
			else
			{
				this.ServiceInstanceIdentity = "*";
				this.SyncObjectIdentity = array[0];
			}
			if (string.IsNullOrEmpty(this.SyncObjectIdentity))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
		}

		private void CheckNoWildcardedServiceInstnaceIdParses()
		{
			if (!this.ServiceInstanceIdentity.Contains("*") && !CompoundSyncObjectIdParameter.TryParseServiceInstanceId(this.ServiceInstanceIdentity, out this.serviceInstance))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
		}

		private void CheckNoWildcardedSyncObjectIdParses()
		{
			if (!this.SyncObjectIdentity.Contains("*") && !CompoundSyncObjectIdParameter.TryParseSyncObjectId(this.SyncObjectIdentity, out this.syncObjectId))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
		}

		private void CheckOnlyOneWildCardAtTheStartOrEnd()
		{
			if (this.SyncObjectIdentity.Contains("*") && !this.SyncObjectIdentity.StartsWith("*") && !this.SyncObjectIdentity.EndsWith("*"))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
			if (this.SyncObjectIdentity.Contains("*") && this.SyncObjectIdentity.IndexOf("*") != this.SyncObjectIdentity.LastIndexOf("*"))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
		}

		private void CheckObjectClassIsValid()
		{
			string[] identityElements = SyncObjectId.GetIdentityElements(this.SyncObjectIdentity);
			if (this.SyncObjectIdentity.EndsWith("*") && identityElements.Length > 1 && identityElements[1] != "*" && !Enum.IsDefined(typeof(DirectoryObjectClass), identityElements[1]))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
			if (this.SyncObjectIdentity.StartsWith("*") && identityElements.Length > 1 && identityElements[identityElements.Length - 2] != "*" && !Enum.IsDefined(typeof(DirectoryObjectClass), identityElements[identityElements.Length - 2]))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("identity"), "identity");
			}
		}

		private static readonly char[] Separators = new char[]
		{
			'\\'
		};

		private ServiceInstanceId serviceInstance;

		private SyncObjectId syncObjectId;
	}
}
