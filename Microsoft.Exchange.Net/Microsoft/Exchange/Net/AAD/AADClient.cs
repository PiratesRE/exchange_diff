using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.WindowsAzure.ActiveDirectory;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using Microsoft.WindowsAzure.ActiveDirectoryV122;
using Microsoft.WindowsAzure.ActiveDirectoryV142;

namespace Microsoft.Exchange.Net.AAD
{
	internal sealed class AADClient : IAadClient
	{
		public Dictionary<string, string> Headers { get; set; }

		public AADClient(string graphBaseURL, string tenantContextId, AADJWTToken token, GraphProxyVersions apiVersion = GraphProxyVersions.Version14)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("graphBaseURL", graphBaseURL);
			ArgumentValidator.ThrowIfNullOrEmpty("tenantContextId", tenantContextId);
			ArgumentValidator.ThrowIfNull("token", token);
			this.tenantContextId = tenantContextId;
			if (apiVersion == GraphProxyVersions.Version14)
			{
				this.Service = new Microsoft.WindowsAzure.ActiveDirectory.DirectoryDataService(graphBaseURL, tenantContextId, token);
			}
			else if (apiVersion == GraphProxyVersions.Version142)
			{
				this.ServiceV142 = new Microsoft.WindowsAzure.ActiveDirectoryV142.DirectoryDataService(graphBaseURL, tenantContextId, token);
			}
			else
			{
				this.ServiceV122 = new Microsoft.WindowsAzure.ActiveDirectoryV122.DirectoryDataService(graphBaseURL, tenantContextId, token);
			}
			this.version = apiVersion;
			this.Initialize();
		}

		public AADClient(string graphBaseURL, string tenantContextId, ICredentials credentials, GraphProxyVersions apiVersion = GraphProxyVersions.Version14)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("graphBaseURL", graphBaseURL);
			ArgumentValidator.ThrowIfNullOrEmpty("tenantContextId", tenantContextId);
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			this.tenantContextId = tenantContextId;
			if (apiVersion == GraphProxyVersions.Version14)
			{
				this.Service = new Microsoft.WindowsAzure.ActiveDirectory.DirectoryDataService(graphBaseURL, tenantContextId, null);
				this.Service.Credentials = credentials;
			}
			else if (apiVersion == GraphProxyVersions.Version142)
			{
				this.ServiceV142 = new Microsoft.WindowsAzure.ActiveDirectoryV142.DirectoryDataService(graphBaseURL, tenantContextId, null);
				this.ServiceV142.Credentials = credentials;
			}
			else
			{
				this.ServiceV122 = new Microsoft.WindowsAzure.ActiveDirectoryV122.DirectoryDataService(graphBaseURL, tenantContextId, null);
				this.ServiceV122.Credentials = credentials;
			}
			this.version = apiVersion;
			this.Initialize();
		}

		public int Timeout
		{
			get
			{
				if (this.version == GraphProxyVersions.Version14)
				{
					return this.Service.Timeout;
				}
				if (this.version == GraphProxyVersions.Version142)
				{
					return this.ServiceV142.Timeout;
				}
				return this.ServiceV122.Timeout;
			}
			set
			{
				if (this.version == GraphProxyVersions.Version14)
				{
					this.Service.Timeout = value;
					return;
				}
				if (this.version == GraphProxyVersions.Version142)
				{
					this.ServiceV142.Timeout = value;
					return;
				}
				this.ServiceV122.Timeout = value;
			}
		}

		internal Microsoft.WindowsAzure.ActiveDirectory.DirectoryDataService Service { get; private set; }

		internal Microsoft.WindowsAzure.ActiveDirectoryV122.DirectoryDataService ServiceV122 { get; private set; }

		internal Microsoft.WindowsAzure.ActiveDirectoryV142.DirectoryDataService ServiceV142 { get; private set; }

		public string CreateGroup(string displayName, string alias, string description, bool isPublic)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("displayName", displayName);
			ArgumentValidator.ThrowIfNullOrEmpty("alias", alias);
			Microsoft.WindowsAzure.ActiveDirectory.Group group = new Microsoft.WindowsAzure.ActiveDirectory.Group();
			group.securityEnabled = new bool?(false);
			group.mailEnabled = new bool?(true);
			group.groupType = "Unified";
			group.mailNickname = alias;
			group.displayName = displayName;
			group.isPublic = new bool?(isPublic);
			if (!string.IsNullOrEmpty(description))
			{
				group.description = description;
			}
			this.Service.AddTogroups(group);
			this.SaveChanges("CreateGroup");
			return group.objectId;
		}

		public void DeleteGroup(string objectId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("objectId", objectId);
			Microsoft.WindowsAzure.ActiveDirectory.Group group = new Microsoft.WindowsAzure.ActiveDirectory.Group();
			group.objectId = objectId;
			this.Service.AttachTo("groups", group);
			try
			{
				this.Service.DeleteObject(group);
				this.SaveChanges("DeleteGroup");
			}
			finally
			{
				this.Service.Detach(group);
			}
		}

		public void UpdateGroup(string objectId, string description = null, string[] exchangeResources = null, string displayName = null, bool? isPublic = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("objectId", objectId);
			Microsoft.WindowsAzure.ActiveDirectory.Group group = this.GetGroup(objectId, true);
			if (group != null)
			{
				if (description != null)
				{
					group.description = ((description == string.Empty) ? null : description);
				}
				if (exchangeResources != null)
				{
					group.exchangeResources = new Collection<string>(exchangeResources);
				}
				if (displayName != null)
				{
					group.displayName = displayName;
				}
				if (isPublic != null)
				{
					group.isPublic = new bool?(isPublic.Value);
				}
				this.Service.UpdateObject(group);
				this.SaveChanges("UpdateGroup", 8);
			}
		}

		public AADClient.LinkResult[] AddMembers(string groupObjectId, params string[] userObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(userObjectIds, delegate(string[] batchUsers)
			{
				this.UpdateLinks("AddMembers", groupObjectId, "members", true, batchUsers);
			});
		}

		public AADClient.LinkResult[] RemoveMembers(string groupObjectId, params string[] userObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(userObjectIds, delegate(string[] batchUsers)
			{
				this.UpdateLinks("RemoveMembers", groupObjectId, "members", false, batchUsers);
			});
		}

		public AADClient.LinkResult[] AddOwners(string groupObjectId, params string[] userObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(userObjectIds, delegate(string[] batchUsers)
			{
				this.UpdateLinks("AddOwners", groupObjectId, "owners", true, batchUsers);
			});
		}

		public AADClient.LinkResult[] RemoveOwners(string groupObjectId, params string[] userObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(userObjectIds, delegate(string[] batchUsers)
			{
				this.UpdateLinks("RemoveOwners", groupObjectId, "owners", false, batchUsers);
			});
		}

		public AADClient.LinkResult[] AddAllowAccessTo(string groupObjectId, params string[] groupObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(groupObjectIds, delegate(string[] batchGroups)
			{
				this.UpdateLinks("AddAllowAccessTo", groupObjectId, "allowAccessTo", true, batchGroups);
			});
		}

		public AADClient.LinkResult[] RemoveAllowAccessTo(string groupObjectId, params string[] groupObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(groupObjectIds, delegate(string[] batchGroups)
			{
				this.UpdateLinks("RemoveAllowAccessTo", groupObjectId, "allowAccessTo", false, batchGroups);
			});
		}

		public AADClient.LinkResult[] AddPendingMembers(string groupObjectId, params string[] userObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(userObjectIds, delegate(string[] batchUsers)
			{
				this.UpdateLinks("AddPendingMembers", groupObjectId, "pendingMembers", true, batchUsers);
			});
		}

		public AADClient.LinkResult[] RemovePendingMembers(string groupObjectId, params string[] userObjectIds)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			return this.BatchLinkOperation(userObjectIds, delegate(string[] batchUsers)
			{
				this.UpdateLinks("RemovePendingMembers", groupObjectId, "pendingMembers", false, batchUsers);
			});
		}

		public string[] GetGroupMembership(string userObjectId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("userObjectId", userObjectId);
			string text = "users/" + userObjectId + "/memberOf";
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group> query = this.Service.CreateQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(text);
			List<string> list = new List<string>(8);
			bool ignoreMissingProperties = this.Service.IgnoreMissingProperties;
			try
			{
				this.Service.IgnoreMissingProperties = true;
				DataServiceQueryContinuation<Microsoft.WindowsAzure.ActiveDirectory.Group> continuation;
				for (QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.Group> queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(query, false); queryOperationResponse != null; queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(continuation))
				{
					list.AddRange(from @group in queryOperationResponse
					where @group.groupType == "Unified"
					select @group.objectId);
					continuation = queryOperationResponse.GetContinuation();
					if (continuation == null)
					{
						break;
					}
				}
			}
			finally
			{
				this.Service.IgnoreMissingProperties = ignoreMissingProperties;
			}
			return list.ToArray();
		}

		public bool IsUserMemberOfGroup(string userObjectId, string groupObjectId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("userObjectId", userObjectId);
			ArgumentValidator.ThrowIfNullOrEmpty("groupObjectId", groupObjectId);
			string text = "users/" + userObjectId + "/memberOf/" + groupObjectId;
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group> query = this.Service.CreateQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(text);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.Group> queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(query, false);
			bool flag = queryOperationResponse != null && queryOperationResponse.Count<Microsoft.WindowsAzure.ActiveDirectory.Group>() != 0;
			AADClient.Tracer.TraceDebug<string, string, bool>((long)this.GetHashCode(), "User: {0}, Group: {1}, isMember: {2}", userObjectId, groupObjectId, flag);
			return flag;
		}

		public string GetUserObjectId(string userPrincipalName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("userPrincipalName", userPrincipalName);
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectoryV122.User> query = (DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectoryV122.User>)(from user in this.ServiceV122.users
			where string.Equals(user.userPrincipalName, userPrincipalName)
			select user);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectoryV122.User> queryOperationResponse = this.ExecuteQuery<Microsoft.WindowsAzure.ActiveDirectoryV122.User>(query, false);
			if (queryOperationResponse == null)
			{
				AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Did not find User with userPrincipalName {0}", userPrincipalName);
				return null;
			}
			List<Microsoft.WindowsAzure.ActiveDirectoryV122.User> list = queryOperationResponse.ToList<Microsoft.WindowsAzure.ActiveDirectoryV122.User>();
			if (list.Count <= 0)
			{
				return string.Empty;
			}
			return list.FirstOrDefault<Microsoft.WindowsAzure.ActiveDirectoryV122.User>().objectId;
		}

		internal string CreateDevice(string displayName, string easID, Guid deviceID, string userPrincipalName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("displayName", displayName);
			ArgumentValidator.ThrowIfNullOrEmpty("easID", easID);
			ArgumentValidator.ThrowIfNullOrEmpty("userPrincipalName", userPrincipalName);
			ArgumentValidator.ThrowIfNull("deviceID", deviceID);
			Microsoft.WindowsAzure.ActiveDirectoryV122.Device device = new Microsoft.WindowsAzure.ActiveDirectoryV122.Device();
			device.accountEnabled = new bool?(true);
			device.alternativeSecurityIds = new Collection<Microsoft.WindowsAzure.ActiveDirectoryV122.AlternativeSecurityId>();
			device.alternativeSecurityIds.Add(new Microsoft.WindowsAzure.ActiveDirectoryV122.AlternativeSecurityId
			{
				key = Guid.NewGuid().ToByteArray(),
				type = new int?(2),
				identityProvider = null
			});
			device.deviceId = new Guid?(deviceID);
			device.deviceOSType = "iOS";
			device.displayName = displayName;
			device.deviceOSVersion = "2.0";
			Microsoft.WindowsAzure.ActiveDirectoryV122.User user = (from x in this.ServiceV122.users
			where x.userPrincipalName == userPrincipalName
			select x).ToList<Microsoft.WindowsAzure.ActiveDirectoryV122.User>().FirstOrDefault<Microsoft.WindowsAzure.ActiveDirectoryV122.User>();
			if (user == null)
			{
				return string.Format("user not found. UserPrincipalName {0}", userPrincipalName);
			}
			device.exchangeActiveSyncId = new Collection<string>();
			device.exchangeActiveSyncId.Add(string.Format("eas:{0}:{1}:{2}", easID, user.objectId, ExDateTime.UtcNow.ToShortDateString()));
			this.ServiceV122.AddObject("devices", device);
			this.SaveChanges("CreateDevice");
			this.ServiceV122.AddLink(device, "registeredOwners", user);
			this.ServiceV122.AddLink(device, "registeredUsers", user);
			this.SaveChanges("UpdateDeviceOwner");
			return device.objectId;
		}

		internal string UpdateDevice(Guid deviceId, Collection<string> easIds, bool? isManaged, bool? isCompliant, bool? accountEnabled)
		{
			ArgumentValidator.ThrowIfNull("deviceId", deviceId);
			Microsoft.WindowsAzure.ActiveDirectoryV122.Device device = (from x in this.ServiceV122.devices
			where x.deviceId == (Guid?)deviceId
			select x).ToList<Microsoft.WindowsAzure.ActiveDirectoryV122.Device>().FirstOrDefault<Microsoft.WindowsAzure.ActiveDirectoryV122.Device>();
			if (device != null)
			{
				if (easIds != null)
				{
					device.exchangeActiveSyncId = easIds;
				}
				if (isCompliant != null)
				{
					device.isCompliant = isCompliant;
				}
				if (isManaged != null)
				{
					device.isManaged = isManaged;
				}
				if (accountEnabled != null)
				{
					device.accountEnabled = accountEnabled;
				}
				this.ServiceV122.UpdateObject(device);
				this.SaveChanges("UpdateDevice");
				return device.objectId;
			}
			return string.Format("Device with id {0} not found", device.ToString());
		}

		public string EvaluateAuthPolicy(string easId, string userObjectId, bool isSupportedPlatform)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("easId", easId);
			ArgumentValidator.ThrowIfNullOrEmpty("userObjectId", userObjectId);
			string text = this.ServiceV142.BaseUri.ToString() + "/users/" + userObjectId + "/evaluateAuthPolicy";
			AADClient.Tracer.TraceDebug<string, string, bool>((long)this.GetHashCode(), "Evaluate Auth Policy for with serviceInfoQUery {0}, easID:{1}, IsSupported:{2}", text, easId, isSupportedPlatform);
			QueryOperationResponse<ICollection<string>> queryOperationResponse = this.ServiceV142.Execute<ICollection<string>>(new Uri(text), "POST", true, new OperationParameter[]
			{
				new BodyOperationParameter("applicationIdentifier", "00000002-0000-0ff1-ce00-000000000000"),
				new BodyOperationParameter("exchangeActiveSyncId", easId),
				new BodyOperationParameter("platform", isSupportedPlatform ? "eas_supported" : "eas_unsupported")
			}) as QueryOperationResponse<ICollection<string>>;
			if (queryOperationResponse == null)
			{
				AADClient.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Could not Evaluate Auth Policy for user {0}, device {1}", userObjectId, easId);
				return null;
			}
			ICollection<string> collection = queryOperationResponse.ToList<ICollection<string>>().FirstOrDefault<ICollection<string>>();
			if (collection == null)
			{
				AADClient.Tracer.TraceDebug((long)this.GetHashCode(), "response List is empty");
				return null;
			}
			AADClient.Tracer.TraceDebug(this.GetHashCode(), 0L, "EvaluateAuthPolicy Response for user {0}, Device {1}. ResponseCount:{3}, Value:{4}", new object[]
			{
				userObjectId,
				easId,
				collection.Count<string>(),
				collection.FirstOrDefault<string>()
			});
			return collection.FirstOrDefault<string>();
		}

		public List<AadDevice> GetUserDevicesWithEasID(string easId, string userObjectId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("easId", easId);
			ArgumentValidator.ThrowIfNullOrEmpty("userObjectId", userObjectId);
			string pattern = string.Format(CultureInfo.InvariantCulture, "eas:{0}:{1}:", new object[]
			{
				easId,
				userObjectId
			});
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectoryV122.Device> query = (DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectoryV122.Device>)(from device in this.ServiceV122.devices
			where device.exchangeActiveSyncId.Any((string id) => id.StartsWith(pattern))
			select device);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectoryV122.Device> queryOperationResponse = this.ExecuteQuery<Microsoft.WindowsAzure.ActiveDirectoryV122.Device>(query, false);
			if (queryOperationResponse == null)
			{
				AADClient.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Did not find Device with easID {0} and userObjectId {1)", easId, userObjectId);
				return null;
			}
			List<AadDevice> aadDevices = new List<AadDevice>();
			queryOperationResponse.ToList<Microsoft.WindowsAzure.ActiveDirectoryV122.Device>().ForEach(delegate(Microsoft.WindowsAzure.ActiveDirectoryV122.Device s)
			{
				aadDevices.Add(this.GetAadDevice(s, easId, userObjectId));
			});
			return aadDevices;
		}

		public string[] GetServiceInfo(string serviceInstance)
		{
			string text = "tenantDetails/" + this.tenantContextId + "/serviceInfo";
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.ServiceInfo> query = this.Service.CreateQuery<Microsoft.WindowsAzure.ActiveDirectory.ServiceInfo>(text);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.ServiceInfo> queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.ServiceInfo>(query, false);
			if (queryOperationResponse == null)
			{
				AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Did not find ServiceInfo for tenant {0}", this.tenantContextId);
				return null;
			}
			foreach (Microsoft.WindowsAzure.ActiveDirectory.ServiceInfo serviceInfo in queryOperationResponse)
			{
				if (serviceInfo.serviceInstance.StartsWith(serviceInstance, StringComparison.OrdinalIgnoreCase) && serviceInfo.serviceElements != null && serviceInfo.serviceElements.Count > 0)
				{
					AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Found serviceInstance: {0}", serviceInstance);
					return serviceInfo.serviceElements.ToArray<string>();
				}
			}
			AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Did not find serviceInstance: {0}", serviceInstance);
			return null;
		}

		public string GetDefaultDomain()
		{
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.TenantDetail> query = (DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.TenantDetail>)(from t in this.Service.tenantDetails
			select t);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.TenantDetail> queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.TenantDetail>(query, false);
			if (queryOperationResponse != null)
			{
				Collection<Microsoft.WindowsAzure.ActiveDirectory.VerifiedDomain> verifiedDomains = queryOperationResponse.ToList<Microsoft.WindowsAzure.ActiveDirectory.TenantDetail>().FirstOrDefault<Microsoft.WindowsAzure.ActiveDirectory.TenantDetail>().verifiedDomains;
				if (verifiedDomains != null && verifiedDomains.Count > 0)
				{
					return (from domain in verifiedDomains
					where domain.@default == true
					select domain.name).FirstOrDefault<string>();
				}
			}
			return null;
		}

		public bool IsAliasUnique(string alias)
		{
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group> query = (DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>)(from g in this.Service.groups
			where g.mailNickname == alias
			select g);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.Group> queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(query, false);
			return queryOperationResponse == null || queryOperationResponse.Count<Microsoft.WindowsAzure.ActiveDirectory.Group>() == 0;
		}

		public bool IsUserEnabled(string userSmtpAddress)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("userSmtpAddress", userSmtpAddress);
			string userProxyAddress = "SMTP:" + userSmtpAddress;
			Microsoft.WindowsAzure.ActiveDirectory.User user;
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.User> query = (DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.User>)(from user in this.Service.users
			where string.Equals(user.userPrincipalName, userSmtpAddress) || user.proxyAddresses.Any((string proxyAddress) => string.Equals(proxyAddress, userProxyAddress))
			select user);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.User> queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.User>(query, false);
			if (queryOperationResponse == null)
			{
				AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Did not find User with userPrincipalName or proxy addresses: {0}", userSmtpAddress);
				return false;
			}
			using (IEnumerator<Microsoft.WindowsAzure.ActiveDirectory.User> enumerator = queryOperationResponse.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					user = enumerator.Current;
					if (user.accountEnabled != null && user.accountEnabled.Value)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal QueryOperationResponse<T> ExecuteSearchQuery<T>(DataServiceQuery<T> query, bool throwIfNotFound = false) where T : Microsoft.WindowsAzure.ActiveDirectory.DirectoryObject
		{
			QueryOperationResponse<T> result;
			try
			{
				AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Executing query: {0}", query.ToString());
				result = (query.Execute() as QueryOperationResponse<T>);
			}
			catch (DataServiceQueryException ex)
			{
				if (throwIfNotFound || !AADClient.IsRecordNotFoundException(ex))
				{
					AADClient.Tracer.TraceError<DataServiceQueryException>((long)this.GetHashCode(), "ExecuteSearchQuery failed: {0}", ex);
					throw new AADDataException(ex);
				}
				AADClient.Tracer.TraceDebug((long)this.GetHashCode(), "Record not found");
				result = null;
			}
			catch (DataServiceTransportException ex2)
			{
				AADClient.Tracer.TraceError<DataServiceTransportException>((long)this.GetHashCode(), "ExecuteSearchQuery failed: {0}", ex2);
				throw new AADTransportException(ex2);
			}
			return result;
		}

		internal QueryOperationResponse<T> ExecuteQuery<T>(DataServiceQuery<T> query, bool throwIfNotFound = false) where T : Microsoft.WindowsAzure.ActiveDirectoryV122.DirectoryObject
		{
			QueryOperationResponse<T> result;
			try
			{
				AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Executing query: {0}", query.ToString());
				result = (query.Execute() as QueryOperationResponse<T>);
			}
			catch (DataServiceQueryException ex)
			{
				if (throwIfNotFound || !AADClient.IsRecordNotFoundException(ex))
				{
					AADClient.Tracer.TraceError<DataServiceQueryException>((long)this.GetHashCode(), "ExecuteQuery failed: {0}", ex);
					throw new AADDataException(ex);
				}
				AADClient.Tracer.TraceDebug((long)this.GetHashCode(), "Record not found");
				result = null;
			}
			catch (DataServiceTransportException ex2)
			{
				AADClient.Tracer.TraceError<DataServiceTransportException>((long)this.GetHashCode(), "ExecuteQuery failed: {0}", ex2);
				throw new AADTransportException(ex2);
			}
			return result;
		}

		internal QueryOperationResponse<T> ExecuteSearchQuery<T>(DataServiceQueryContinuation<T> continuation) where T : Microsoft.WindowsAzure.ActiveDirectory.DirectoryObject
		{
			QueryOperationResponse<T> result;
			try
			{
				AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Executing continuation query: {0}", continuation.ToString());
				result = this.Service.Execute<T>(continuation);
			}
			catch (DataServiceQueryException ex)
			{
				AADClient.Tracer.TraceError<DataServiceQueryException>((long)this.GetHashCode(), "ExecuteSearchQuery failed: {0}", ex);
				throw new AADDataException(ex);
			}
			catch (DataServiceTransportException ex2)
			{
				AADClient.Tracer.TraceError<DataServiceTransportException>((long)this.GetHashCode(), "ExecuteSearchQuery failed: {0}", ex2);
				throw new AADTransportException(ex2);
			}
			return result;
		}

		internal Microsoft.WindowsAzure.ActiveDirectory.Group GetGroup(string objectId, bool throwIfNotFound = true)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("objectId", objectId);
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group> query = (DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>)(from g in this.Service.groups
			where g.objectId == objectId
			select g);
			QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.Group> queryOperationResponse = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(query, throwIfNotFound);
			if (queryOperationResponse != null)
			{
				return queryOperationResponse.ToList<Microsoft.WindowsAzure.ActiveDirectory.Group>().FirstOrDefault<Microsoft.WindowsAzure.ActiveDirectory.Group>();
			}
			return null;
		}

		internal IEnumerable<Microsoft.WindowsAzure.ActiveDirectory.Group> GetGroups()
		{
			DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group> query = (DataServiceQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>)(from g in this.Service.groups
			where g.groupType == "Unified"
			select g);
			DataServiceQueryContinuation<Microsoft.WindowsAzure.ActiveDirectory.Group> continuation;
			for (QueryOperationResponse<Microsoft.WindowsAzure.ActiveDirectory.Group> response = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(query, false); response != null; response = this.ExecuteSearchQuery<Microsoft.WindowsAzure.ActiveDirectory.Group>(continuation))
			{
				foreach (Microsoft.WindowsAzure.ActiveDirectory.Group group in response)
				{
					yield return group;
				}
				continuation = response.GetContinuation();
				if (continuation == null)
				{
					break;
				}
			}
			yield break;
		}

		private static bool IsRecordNotFoundException(Exception e)
		{
			while (e != null)
			{
				DataServiceClientException ex = e as DataServiceClientException;
				if (ex != null)
				{
					return ex.StatusCode == 404;
				}
				e = e.InnerException;
			}
			return false;
		}

		private void Initialize()
		{
			this.Headers = new Dictionary<string, string>();
			if (this.version == GraphProxyVersions.Version14)
			{
				this.Service.SendingRequest2 += this.OnSendingRequest2;
				this.Service.WritingEntity += this.OnWritingEntity;
				this.Service.ReceivingResponse += this.OnReceivingResponse;
				this.Service.ReadingEntity += this.OnReadingEntity;
				return;
			}
			if (this.version == GraphProxyVersions.Version142)
			{
				this.ServiceV142.SendingRequest2 += this.OnSendingRequest2;
				this.ServiceV142.WritingEntity += this.OnWritingEntity;
				this.ServiceV142.ReceivingResponse += this.OnReceivingResponse;
				this.ServiceV142.ReadingEntity += this.OnReadingEntity;
				return;
			}
			this.ServiceV122.SendingRequest2 += this.OnSendingRequest2;
			this.ServiceV122.WritingEntity += this.OnWritingEntity;
			this.ServiceV122.ReceivingResponse += this.OnReceivingResponse;
			this.ServiceV122.ReadingEntity += this.OnReadingEntity;
		}

		private void UpdateLinks(string action, string groupObjectId, string sourceProperty, bool addLink, params string[] objectIds)
		{
			Microsoft.WindowsAzure.ActiveDirectory.Group group = new Microsoft.WindowsAzure.ActiveDirectory.Group();
			group.objectId = groupObjectId;
			this.Service.AttachTo("groups", group);
			Microsoft.WindowsAzure.ActiveDirectory.User[] array = new Microsoft.WindowsAzure.ActiveDirectory.User[objectIds.Length];
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Microsoft.WindowsAzure.ActiveDirectory.User();
					array[i].objectId = objectIds[i];
					this.Service.AttachTo("users", array[i]);
					if (addLink)
					{
						this.Service.AddLink(group, sourceProperty, array[i]);
					}
					else
					{
						this.Service.DeleteLink(group, sourceProperty, array[i]);
					}
				}
				this.Service.UsePermissiveReferenceUpdates = true;
				this.SaveChanges(action, (array.Length > 1) ? 1 : 0);
			}
			finally
			{
				this.Service.UsePermissiveReferenceUpdates = false;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] != null)
					{
						this.Service.Detach(array[j]);
					}
				}
				this.Service.Detach(group);
			}
		}

		private AADClient.LinkResult[] BatchLinkOperation(string[] links, Action<string[]> action)
		{
			List<AADClient.LinkResult> list = new List<AADClient.LinkResult>(2);
			if (links != null && links.Length > 0)
			{
				int num;
				for (int i = 0; i < links.Length; i += num)
				{
					num = Math.Min(20, links.Length - i);
					string[] subArray = AADClient.GetSubArray<string>(links, i, num);
					AADException ex = null;
					try
					{
						action(subArray);
					}
					catch (AADException ex2)
					{
						ex = ex2;
					}
					if (ex != null)
					{
						AADClient.Tracer.TraceError<AADException>((long)this.GetHashCode(), "Batch action failed with exception: {0}", ex);
						if (num == 1)
						{
							list.Add(new AADClient.LinkResult
							{
								FailedLink = subArray[0],
								Exception = ex
							});
						}
						else
						{
							foreach (string text in subArray)
							{
								try
								{
									action(new string[]
									{
										text
									});
								}
								catch (AADException ex3)
								{
									list.Add(new AADClient.LinkResult
									{
										FailedLink = text,
										Exception = ex3
									});
									AADClient.Tracer.TraceError<string, AADException>((long)this.GetHashCode(), "Batch action failed for {0} with exception: {1}", text, ex3);
								}
							}
						}
					}
				}
			}
			if (!list.Any<AADClient.LinkResult>())
			{
				return null;
			}
			return list.ToArray();
		}

		private static T[] GetSubArray<T>(T[] array, int start, int length)
		{
			if (start == 0 && length == array.Length)
			{
				return array;
			}
			T[] array2 = new T[length];
			Array.Copy(array, start, array2, 0, length);
			return array2;
		}

		private void SaveChanges(string action)
		{
			if (this.version == GraphProxyVersions.Version14)
			{
				this.SaveChanges(action, this.Service.SaveChangesDefaultOptions);
				return;
			}
			if (this.version == GraphProxyVersions.Version142)
			{
				this.SaveChanges(action, this.ServiceV142.SaveChangesDefaultOptions);
				return;
			}
			this.SaveChanges(action, this.ServiceV122.SaveChangesDefaultOptions);
		}

		private void SaveChanges(string action, SaveChangesOptions options)
		{
			AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Calling SaveChanges for {0}", action);
			try
			{
				if (this.version == GraphProxyVersions.Version14)
				{
					this.Service.SaveChanges(options);
				}
				else if (this.version == GraphProxyVersions.Version142)
				{
					this.ServiceV142.SaveChanges(options);
				}
				else
				{
					this.ServiceV122.SaveChanges(options);
				}
			}
			catch (DataServiceRequestException ex)
			{
				AADClient.Tracer.TraceError<DataServiceRequestException>((long)this.GetHashCode(), "SaveChanges failed: {0}", ex);
				throw new AADDataException(ex);
			}
			catch (DataServiceTransportException ex2)
			{
				AADClient.Tracer.TraceError<DataServiceTransportException>((long)this.GetHashCode(), "SaveChanges failed: {0}", ex2);
				throw new AADTransportException(ex2);
			}
		}

		private void OnSendingRequest2(object sender, SendingRequest2EventArgs args)
		{
			foreach (KeyValuePair<string, string> keyValuePair in this.Headers)
			{
				args.RequestMessage.SetHeader(keyValuePair.Key, keyValuePair.Value);
			}
			if (!AADClient.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Url: ");
			stringBuilder.AppendLine(args.RequestMessage.Url.ToString());
			stringBuilder.Append("Method: ");
			stringBuilder.AppendLine(args.RequestMessage.Method);
			stringBuilder.AppendLine("Headers:");
			foreach (KeyValuePair<string, string> keyValuePair2 in args.RequestMessage.Headers)
			{
				stringBuilder.Append(keyValuePair2.Key);
				stringBuilder.Append(": ");
				stringBuilder.AppendLine(keyValuePair2.Value);
			}
			AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AADClient.OnSendingRequest2: {0}", stringBuilder.ToString());
		}

		private void OnWritingEntity(object sender, ReadingWritingEntityEventArgs args)
		{
			AADClient.Tracer.TraceDebug<XElement>((long)this.GetHashCode(), "AADClient.OnWritingEntity: {0}", args.Data);
		}

		private void OnReceivingResponse(object sender, ReceivingResponseEventArgs args)
		{
			if (!AADClient.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StatusCode: ");
			stringBuilder.AppendLine(args.ResponseMessage.StatusCode.ToString());
			stringBuilder.AppendLine("Headers:");
			foreach (KeyValuePair<string, string> keyValuePair in args.ResponseMessage.Headers)
			{
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append(": ");
				stringBuilder.AppendLine(keyValuePair.Value);
			}
			AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AADClient.OnReceivingResponse: {0}", stringBuilder.ToString());
		}

		private void OnReadingEntity(object sender, ReadingWritingEntityEventArgs args)
		{
			AADClient.Tracer.TraceDebug<XElement>((long)this.GetHashCode(), "AADClient.OnReadingEntity: {0}", args.Data);
		}

		private AadDevice GetAadDevice(Microsoft.WindowsAzure.ActiveDirectoryV122.Device device, string easDeviceId, string userObjectId)
		{
			AadDevice aadDevice = new AadDevice();
			aadDevice.AccountEnabled = device.accountEnabled;
			aadDevice.DeviceId = device.deviceId;
			aadDevice.DisplayName = device.displayName;
			aadDevice.ExchangeActiveSyncIds = device.exchangeActiveSyncId.ToList<string>();
			aadDevice.IsCompliant = device.isCompliant;
			aadDevice.IsManaged = device.isManaged;
			string text = (from s in device.exchangeActiveSyncId
			where s.StartsWith(string.Format("eas:{0}:{1}:", easDeviceId, userObjectId), StringComparison.OrdinalIgnoreCase)
			select s).First<string>();
			AADClient.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AADClient.GetAadDevice: exchangeActiveSyncId {0}", text);
			aadDevice.LastUpdated = DateTime.ParseExact(text.Substring(text.LastIndexOf(':') + 1), "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
			return aadDevice;
		}

		private const string UnifiedGroupType = "Unified";

		private const string MembersProperty = "members";

		private const string OwnersProperty = "owners";

		private const string AllowAccessToProperty = "allowAccessTo";

		private const string PendingMembersProperty = "pendingMembers";

		private const string AppIdentifierParameter = "applicationIdentifier";

		private const string EasIDParameter = "exchangeActiveSyncId";

		private const string PlatformPamaneter = "platform";

		private const string ApplicationId = "00000002-0000-0ff1-ce00-000000000000";

		private const string SupportedPlatformIdentifier = "eas_supported";

		private const string UnSupportedPlatformIdentifier = "eas_unsupported";

		private const string GroupsEntitySetName = "groups";

		private const string UsersEntitySetName = "users";

		private const string ExchangeActiveSyncIdFormat = "eas:{0}:{1}:";

		private const char AadEasIdSeparator = ':';

		private static readonly Trace Tracer = ExTraceGlobals.AADClientTracer;

		private readonly string tenantContextId;

		private readonly GraphProxyVersions version;

		internal class LinkResult
		{
			public string FailedLink { get; set; }

			public Exception Exception { get; set; }
		}
	}
}
