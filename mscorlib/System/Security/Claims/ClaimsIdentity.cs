using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.Security.Claims
{
	[ComVisible(true)]
	[Serializable]
	public class ClaimsIdentity : IIdentity
	{
		public ClaimsIdentity() : this(null)
		{
		}

		public ClaimsIdentity(IIdentity identity) : this(identity, null)
		{
		}

		public ClaimsIdentity(IEnumerable<Claim> claims) : this(null, claims, null, null, null)
		{
		}

		public ClaimsIdentity(string authenticationType) : this(null, null, authenticationType, null, null)
		{
		}

		public ClaimsIdentity(IEnumerable<Claim> claims, string authenticationType) : this(null, claims, authenticationType, null, null)
		{
		}

		public ClaimsIdentity(IIdentity identity, IEnumerable<Claim> claims) : this(identity, claims, null, null, null)
		{
		}

		public ClaimsIdentity(string authenticationType, string nameType, string roleType) : this(null, null, authenticationType, nameType, roleType)
		{
		}

		public ClaimsIdentity(IEnumerable<Claim> claims, string authenticationType, string nameType, string roleType) : this(null, claims, authenticationType, nameType, roleType)
		{
		}

		public ClaimsIdentity(IIdentity identity, IEnumerable<Claim> claims, string authenticationType, string nameType, string roleType) : this(identity, claims, authenticationType, nameType, roleType, true)
		{
		}

		internal ClaimsIdentity(IIdentity identity, IEnumerable<Claim> claims, string authenticationType, string nameType, string roleType, bool checkAuthType)
		{
			this.m_instanceClaims = new List<Claim>();
			this.m_externalClaims = new Collection<IEnumerable<Claim>>();
			this.m_nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
			this.m_roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
			this.m_version = "1.0";
			base..ctor();
			bool flag = false;
			bool flag2 = false;
			if (checkAuthType && identity != null && string.IsNullOrEmpty(authenticationType))
			{
				if (identity is WindowsIdentity)
				{
					try
					{
						this.m_authenticationType = identity.AuthenticationType;
						goto IL_85;
					}
					catch (UnauthorizedAccessException)
					{
						this.m_authenticationType = null;
						goto IL_85;
					}
				}
				this.m_authenticationType = identity.AuthenticationType;
			}
			else
			{
				this.m_authenticationType = authenticationType;
			}
			IL_85:
			if (!string.IsNullOrEmpty(nameType))
			{
				this.m_nameType = nameType;
				flag = true;
			}
			if (!string.IsNullOrEmpty(roleType))
			{
				this.m_roleType = roleType;
				flag2 = true;
			}
			ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
			if (claimsIdentity != null)
			{
				this.m_label = claimsIdentity.m_label;
				if (!flag)
				{
					this.m_nameType = claimsIdentity.m_nameType;
				}
				if (!flag2)
				{
					this.m_roleType = claimsIdentity.m_roleType;
				}
				this.m_bootstrapContext = claimsIdentity.m_bootstrapContext;
				if (claimsIdentity.Actor != null)
				{
					if (this.IsCircular(claimsIdentity.Actor))
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperationException_ActorGraphCircular"));
					}
					if (!AppContextSwitches.SetActorAsReferenceWhenCopyingClaimsIdentity)
					{
						this.m_actor = claimsIdentity.Actor.Clone();
					}
					else
					{
						this.m_actor = claimsIdentity.Actor;
					}
				}
				if (claimsIdentity is WindowsIdentity && !(this is WindowsIdentity))
				{
					this.SafeAddClaims(claimsIdentity.Claims);
				}
				else
				{
					this.SafeAddClaims(claimsIdentity.m_instanceClaims);
				}
				if (claimsIdentity.m_userSerializationData != null)
				{
					this.m_userSerializationData = (claimsIdentity.m_userSerializationData.Clone() as byte[]);
				}
			}
			else if (identity != null && !string.IsNullOrEmpty(identity.Name))
			{
				this.SafeAddClaim(new Claim(this.m_nameType, identity.Name, "http://www.w3.org/2001/XMLSchema#string", "LOCAL AUTHORITY", "LOCAL AUTHORITY", this));
			}
			if (claims != null)
			{
				this.SafeAddClaims(claims);
			}
		}

		public ClaimsIdentity(BinaryReader reader)
		{
			this.m_instanceClaims = new List<Claim>();
			this.m_externalClaims = new Collection<IEnumerable<Claim>>();
			this.m_nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
			this.m_roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
			this.m_version = "1.0";
			base..ctor();
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.Initialize(reader);
		}

		protected ClaimsIdentity(ClaimsIdentity other)
		{
			this.m_instanceClaims = new List<Claim>();
			this.m_externalClaims = new Collection<IEnumerable<Claim>>();
			this.m_nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
			this.m_roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
			this.m_version = "1.0";
			base..ctor();
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other.m_actor != null)
			{
				this.m_actor = other.m_actor.Clone();
			}
			this.m_authenticationType = other.m_authenticationType;
			this.m_bootstrapContext = other.m_bootstrapContext;
			this.m_label = other.m_label;
			this.m_nameType = other.m_nameType;
			this.m_roleType = other.m_roleType;
			if (other.m_userSerializationData != null)
			{
				this.m_userSerializationData = (other.m_userSerializationData.Clone() as byte[]);
			}
			this.SafeAddClaims(other.m_instanceClaims);
		}

		[SecurityCritical]
		protected ClaimsIdentity(SerializationInfo info, StreamingContext context)
		{
			this.m_instanceClaims = new List<Claim>();
			this.m_externalClaims = new Collection<IEnumerable<Claim>>();
			this.m_nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
			this.m_roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
			this.m_version = "1.0";
			base..ctor();
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.Deserialize(info, context, true);
		}

		[SecurityCritical]
		protected ClaimsIdentity(SerializationInfo info)
		{
			this.m_instanceClaims = new List<Claim>();
			this.m_externalClaims = new Collection<IEnumerable<Claim>>();
			this.m_nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
			this.m_roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
			this.m_version = "1.0";
			base..ctor();
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.Deserialize(info, default(StreamingContext), false);
		}

		public virtual string AuthenticationType
		{
			get
			{
				return this.m_authenticationType;
			}
		}

		public virtual bool IsAuthenticated
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_authenticationType);
			}
		}

		public ClaimsIdentity Actor
		{
			get
			{
				return this.m_actor;
			}
			set
			{
				if (value != null && this.IsCircular(value))
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperationException_ActorGraphCircular"));
				}
				this.m_actor = value;
			}
		}

		public object BootstrapContext
		{
			get
			{
				return this.m_bootstrapContext;
			}
			[SecurityCritical]
			set
			{
				this.m_bootstrapContext = value;
			}
		}

		public virtual IEnumerable<Claim> Claims
		{
			get
			{
				int num;
				for (int i = 0; i < this.m_instanceClaims.Count; i = num + 1)
				{
					yield return this.m_instanceClaims[i];
					num = i;
				}
				if (this.m_externalClaims != null)
				{
					for (int j = 0; j < this.m_externalClaims.Count; j = num + 1)
					{
						if (this.m_externalClaims[j] != null)
						{
							foreach (Claim claim in this.m_externalClaims[j])
							{
								yield return claim;
							}
							IEnumerator<Claim> enumerator = null;
						}
						num = j;
					}
				}
				yield break;
				yield break;
			}
		}

		protected virtual byte[] CustomSerializationData
		{
			get
			{
				return this.m_userSerializationData;
			}
		}

		internal Collection<IEnumerable<Claim>> ExternalClaims
		{
			[FriendAccessAllowed]
			get
			{
				return this.m_externalClaims;
			}
		}

		public string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		public virtual string Name
		{
			get
			{
				Claim claim = this.FindFirst(this.m_nameType);
				if (claim != null)
				{
					return claim.Value;
				}
				return null;
			}
		}

		public string NameClaimType
		{
			get
			{
				return this.m_nameType;
			}
		}

		public string RoleClaimType
		{
			get
			{
				return this.m_roleType;
			}
		}

		public virtual ClaimsIdentity Clone()
		{
			ClaimsIdentity claimsIdentity = new ClaimsIdentity(this.m_instanceClaims);
			claimsIdentity.m_authenticationType = this.m_authenticationType;
			claimsIdentity.m_bootstrapContext = this.m_bootstrapContext;
			claimsIdentity.m_label = this.m_label;
			claimsIdentity.m_nameType = this.m_nameType;
			claimsIdentity.m_roleType = this.m_roleType;
			if (this.Actor != null)
			{
				if (this.IsCircular(this.Actor))
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperationException_ActorGraphCircular"));
				}
				if (!AppContextSwitches.SetActorAsReferenceWhenCopyingClaimsIdentity)
				{
					claimsIdentity.Actor = this.Actor.Clone();
				}
				else
				{
					claimsIdentity.Actor = this.Actor;
				}
			}
			return claimsIdentity;
		}

		[SecurityCritical]
		public virtual void AddClaim(Claim claim)
		{
			if (claim == null)
			{
				throw new ArgumentNullException("claim");
			}
			if (claim.Subject == this)
			{
				this.m_instanceClaims.Add(claim);
				return;
			}
			this.m_instanceClaims.Add(claim.Clone(this));
		}

		[SecurityCritical]
		public virtual void AddClaims(IEnumerable<Claim> claims)
		{
			if (claims == null)
			{
				throw new ArgumentNullException("claims");
			}
			foreach (Claim claim in claims)
			{
				if (claim != null)
				{
					this.AddClaim(claim);
				}
			}
		}

		[SecurityCritical]
		public virtual bool TryRemoveClaim(Claim claim)
		{
			bool result = false;
			for (int i = 0; i < this.m_instanceClaims.Count; i++)
			{
				if (this.m_instanceClaims[i] == claim)
				{
					this.m_instanceClaims.RemoveAt(i);
					result = true;
					break;
				}
			}
			return result;
		}

		[SecurityCritical]
		public virtual void RemoveClaim(Claim claim)
		{
			if (!this.TryRemoveClaim(claim))
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ClaimCannotBeRemoved", new object[]
				{
					claim
				}));
			}
		}

		[SecuritySafeCritical]
		private void SafeAddClaims(IEnumerable<Claim> claims)
		{
			foreach (Claim claim in claims)
			{
				if (claim.Subject == this)
				{
					this.m_instanceClaims.Add(claim);
				}
				else
				{
					this.m_instanceClaims.Add(claim.Clone(this));
				}
			}
		}

		[SecuritySafeCritical]
		private void SafeAddClaim(Claim claim)
		{
			if (claim.Subject == this)
			{
				this.m_instanceClaims.Add(claim);
				return;
			}
			this.m_instanceClaims.Add(claim.Clone(this));
		}

		public virtual IEnumerable<Claim> FindAll(Predicate<Claim> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			List<Claim> list = new List<Claim>();
			foreach (Claim claim in this.Claims)
			{
				if (match(claim))
				{
					list.Add(claim);
				}
			}
			return list.AsReadOnly();
		}

		public virtual IEnumerable<Claim> FindAll(string type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			List<Claim> list = new List<Claim>();
			foreach (Claim claim in this.Claims)
			{
				if (claim != null && string.Equals(claim.Type, type, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(claim);
				}
			}
			return list.AsReadOnly();
		}

		public virtual bool HasClaim(Predicate<Claim> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (Claim obj in this.Claims)
			{
				if (match(obj))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool HasClaim(string type, string value)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			foreach (Claim claim in this.Claims)
			{
				if (claim != null && claim != null && string.Equals(claim.Type, type, StringComparison.OrdinalIgnoreCase) && string.Equals(claim.Value, value, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		public virtual Claim FindFirst(Predicate<Claim> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (Claim claim in this.Claims)
			{
				if (match(claim))
				{
					return claim;
				}
			}
			return null;
		}

		public virtual Claim FindFirst(string type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			foreach (Claim claim in this.Claims)
			{
				if (claim != null && string.Equals(claim.Type, type, StringComparison.OrdinalIgnoreCase))
				{
					return claim;
				}
			}
			return null;
		}

		[OnSerializing]
		[SecurityCritical]
		private void OnSerializingMethod(StreamingContext context)
		{
			if (this is ISerializable)
			{
				return;
			}
			this.m_serializedClaims = this.SerializeClaims();
			this.m_serializedNameType = this.m_nameType;
			this.m_serializedRoleType = this.m_roleType;
		}

		[OnDeserialized]
		[SecurityCritical]
		private void OnDeserializedMethod(StreamingContext context)
		{
			if (this is ISerializable)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.m_serializedClaims))
			{
				this.DeserializeClaims(this.m_serializedClaims);
				this.m_serializedClaims = null;
			}
			this.m_nameType = (string.IsNullOrEmpty(this.m_serializedNameType) ? "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" : this.m_serializedNameType);
			this.m_roleType = (string.IsNullOrEmpty(this.m_serializedRoleType) ? "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" : this.m_serializedRoleType);
		}

		[OnDeserializing]
		private void OnDeserializingMethod(StreamingContext context)
		{
			if (this is ISerializable)
			{
				return;
			}
			this.m_instanceClaims = new List<Claim>();
			this.m_externalClaims = new Collection<IEnumerable<Claim>>();
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, SerializationFormatter = true)]
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			info.AddValue("System.Security.ClaimsIdentity.version", this.m_version);
			if (!string.IsNullOrEmpty(this.m_authenticationType))
			{
				info.AddValue("System.Security.ClaimsIdentity.authenticationType", this.m_authenticationType);
			}
			info.AddValue("System.Security.ClaimsIdentity.nameClaimType", this.m_nameType);
			info.AddValue("System.Security.ClaimsIdentity.roleClaimType", this.m_roleType);
			if (!string.IsNullOrEmpty(this.m_label))
			{
				info.AddValue("System.Security.ClaimsIdentity.label", this.m_label);
			}
			if (this.m_actor != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					binaryFormatter.Serialize(memoryStream, this.m_actor, null, false);
					info.AddValue("System.Security.ClaimsIdentity.actor", Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
				}
			}
			info.AddValue("System.Security.ClaimsIdentity.claims", this.SerializeClaims());
			if (this.m_bootstrapContext != null)
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					binaryFormatter.Serialize(memoryStream2, this.m_bootstrapContext, null, false);
					info.AddValue("System.Security.ClaimsIdentity.bootstrapContext", Convert.ToBase64String(memoryStream2.GetBuffer(), 0, (int)memoryStream2.Length));
				}
			}
		}

		[SecurityCritical]
		private void DeserializeClaims(string serializedClaims)
		{
			if (!string.IsNullOrEmpty(serializedClaims))
			{
				using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(serializedClaims)))
				{
					this.m_instanceClaims = (List<Claim>)new BinaryFormatter().Deserialize(memoryStream, null, false);
					for (int i = 0; i < this.m_instanceClaims.Count; i++)
					{
						this.m_instanceClaims[i].Subject = this;
					}
				}
			}
			if (this.m_instanceClaims == null)
			{
				this.m_instanceClaims = new List<Claim>();
			}
		}

		[SecurityCritical]
		private string SerializeClaims()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(memoryStream, this.m_instanceClaims, null, false);
				result = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
			return result;
		}

		private bool IsCircular(ClaimsIdentity subject)
		{
			if (this == subject)
			{
				return true;
			}
			ClaimsIdentity claimsIdentity = subject;
			while (claimsIdentity.Actor != null)
			{
				if (this == claimsIdentity.Actor)
				{
					return true;
				}
				claimsIdentity = claimsIdentity.Actor;
			}
			return false;
		}

		private void Initialize(BinaryReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			ClaimsIdentity.SerializationMask serializationMask = (ClaimsIdentity.SerializationMask)reader.ReadInt32();
			if ((serializationMask & ClaimsIdentity.SerializationMask.AuthenticationType) == ClaimsIdentity.SerializationMask.AuthenticationType)
			{
				this.m_authenticationType = reader.ReadString();
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.BootstrapConext) == ClaimsIdentity.SerializationMask.BootstrapConext)
			{
				this.m_bootstrapContext = reader.ReadString();
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.NameClaimType) == ClaimsIdentity.SerializationMask.NameClaimType)
			{
				this.m_nameType = reader.ReadString();
			}
			else
			{
				this.m_nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.RoleClaimType) == ClaimsIdentity.SerializationMask.RoleClaimType)
			{
				this.m_roleType = reader.ReadString();
			}
			else
			{
				this.m_roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.HasClaims) == ClaimsIdentity.SerializationMask.HasClaims)
			{
				int num = reader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					Claim item = new Claim(reader, this);
					this.m_instanceClaims.Add(item);
				}
			}
		}

		protected virtual Claim CreateClaim(BinaryReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return new Claim(reader, this);
		}

		public virtual void WriteTo(BinaryWriter writer)
		{
			this.WriteTo(writer, null);
		}

		protected virtual void WriteTo(BinaryWriter writer, byte[] userData)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			int num = 0;
			ClaimsIdentity.SerializationMask serializationMask = ClaimsIdentity.SerializationMask.None;
			if (this.m_authenticationType != null)
			{
				serializationMask |= ClaimsIdentity.SerializationMask.AuthenticationType;
				num++;
			}
			if (this.m_bootstrapContext != null)
			{
				string text = this.m_bootstrapContext as string;
				if (text != null)
				{
					serializationMask |= ClaimsIdentity.SerializationMask.BootstrapConext;
					num++;
				}
			}
			if (!string.Equals(this.m_nameType, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", StringComparison.Ordinal))
			{
				serializationMask |= ClaimsIdentity.SerializationMask.NameClaimType;
				num++;
			}
			if (!string.Equals(this.m_roleType, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.Ordinal))
			{
				serializationMask |= ClaimsIdentity.SerializationMask.RoleClaimType;
				num++;
			}
			if (!string.IsNullOrWhiteSpace(this.m_label))
			{
				serializationMask |= ClaimsIdentity.SerializationMask.HasLabel;
				num++;
			}
			if (this.m_instanceClaims.Count > 0)
			{
				serializationMask |= ClaimsIdentity.SerializationMask.HasClaims;
				num++;
			}
			if (this.m_actor != null)
			{
				serializationMask |= ClaimsIdentity.SerializationMask.Actor;
				num++;
			}
			if (userData != null && userData.Length != 0)
			{
				num++;
				serializationMask |= ClaimsIdentity.SerializationMask.UserData;
			}
			writer.Write((int)serializationMask);
			writer.Write(num);
			if ((serializationMask & ClaimsIdentity.SerializationMask.AuthenticationType) == ClaimsIdentity.SerializationMask.AuthenticationType)
			{
				writer.Write(this.m_authenticationType);
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.BootstrapConext) == ClaimsIdentity.SerializationMask.BootstrapConext)
			{
				writer.Write(this.m_bootstrapContext as string);
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.NameClaimType) == ClaimsIdentity.SerializationMask.NameClaimType)
			{
				writer.Write(this.m_nameType);
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.RoleClaimType) == ClaimsIdentity.SerializationMask.RoleClaimType)
			{
				writer.Write(this.m_roleType);
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.HasLabel) == ClaimsIdentity.SerializationMask.HasLabel)
			{
				writer.Write(this.m_label);
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.HasClaims) == ClaimsIdentity.SerializationMask.HasClaims)
			{
				writer.Write(this.m_instanceClaims.Count);
				foreach (Claim claim in this.m_instanceClaims)
				{
					claim.WriteTo(writer);
				}
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.Actor) == ClaimsIdentity.SerializationMask.Actor)
			{
				this.m_actor.WriteTo(writer);
			}
			if ((serializationMask & ClaimsIdentity.SerializationMask.UserData) == ClaimsIdentity.SerializationMask.UserData)
			{
				writer.Write(userData.Length);
				writer.Write(userData);
			}
			writer.Flush();
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, SerializationFormatter = true)]
		private void Deserialize(SerializationInfo info, StreamingContext context, bool useContext)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			BinaryFormatter binaryFormatter;
			if (useContext)
			{
				binaryFormatter = new BinaryFormatter(null, context);
			}
			else
			{
				binaryFormatter = new BinaryFormatter();
			}
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string name = enumerator.Name;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num <= 959168042U)
				{
					if (num <= 623923795U)
					{
						if (num != 373632733U)
						{
							if (num == 623923795U)
							{
								if (name == "System.Security.ClaimsIdentity.roleClaimType")
								{
									this.m_roleType = info.GetString("System.Security.ClaimsIdentity.roleClaimType");
								}
							}
						}
						else if (name == "System.Security.ClaimsIdentity.label")
						{
							this.m_label = info.GetString("System.Security.ClaimsIdentity.label");
						}
					}
					else if (num != 656336169U)
					{
						if (num == 959168042U)
						{
							if (name == "System.Security.ClaimsIdentity.nameClaimType")
							{
								this.m_nameType = info.GetString("System.Security.ClaimsIdentity.nameClaimType");
							}
						}
					}
					else if (name == "System.Security.ClaimsIdentity.authenticationType")
					{
						this.m_authenticationType = info.GetString("System.Security.ClaimsIdentity.authenticationType");
					}
				}
				else if (num <= 1476368026U)
				{
					if (num != 1453716852U)
					{
						if (num != 1476368026U)
						{
							continue;
						}
						if (!(name == "System.Security.ClaimsIdentity.actor"))
						{
							continue;
						}
						using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(info.GetString("System.Security.ClaimsIdentity.actor"))))
						{
							this.m_actor = (ClaimsIdentity)binaryFormatter.Deserialize(memoryStream, null, false);
							continue;
						}
					}
					else if (!(name == "System.Security.ClaimsIdentity.claims"))
					{
						continue;
					}
					this.DeserializeClaims(info.GetString("System.Security.ClaimsIdentity.claims"));
				}
				else if (num != 2480284791U)
				{
					if (num == 3659022112U)
					{
						if (name == "System.Security.ClaimsIdentity.bootstrapContext")
						{
							using (MemoryStream memoryStream2 = new MemoryStream(Convert.FromBase64String(info.GetString("System.Security.ClaimsIdentity.bootstrapContext"))))
							{
								this.m_bootstrapContext = binaryFormatter.Deserialize(memoryStream2, null, false);
							}
						}
					}
				}
				else if (name == "System.Security.ClaimsIdentity.version")
				{
					string @string = info.GetString("System.Security.ClaimsIdentity.version");
				}
			}
		}

		[NonSerialized]
		private byte[] m_userSerializationData;

		[NonSerialized]
		private const string PreFix = "System.Security.ClaimsIdentity.";

		[NonSerialized]
		private const string ActorKey = "System.Security.ClaimsIdentity.actor";

		[NonSerialized]
		private const string AuthenticationTypeKey = "System.Security.ClaimsIdentity.authenticationType";

		[NonSerialized]
		private const string BootstrapContextKey = "System.Security.ClaimsIdentity.bootstrapContext";

		[NonSerialized]
		private const string ClaimsKey = "System.Security.ClaimsIdentity.claims";

		[NonSerialized]
		private const string LabelKey = "System.Security.ClaimsIdentity.label";

		[NonSerialized]
		private const string NameClaimTypeKey = "System.Security.ClaimsIdentity.nameClaimType";

		[NonSerialized]
		private const string RoleClaimTypeKey = "System.Security.ClaimsIdentity.roleClaimType";

		[NonSerialized]
		private const string VersionKey = "System.Security.ClaimsIdentity.version";

		[NonSerialized]
		public const string DefaultIssuer = "LOCAL AUTHORITY";

		[NonSerialized]
		public const string DefaultNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

		[NonSerialized]
		public const string DefaultRoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

		[NonSerialized]
		private List<Claim> m_instanceClaims;

		[NonSerialized]
		private Collection<IEnumerable<Claim>> m_externalClaims;

		[NonSerialized]
		private string m_nameType;

		[NonSerialized]
		private string m_roleType;

		[OptionalField(VersionAdded = 2)]
		private string m_version;

		[OptionalField(VersionAdded = 2)]
		private ClaimsIdentity m_actor;

		[OptionalField(VersionAdded = 2)]
		private string m_authenticationType;

		[OptionalField(VersionAdded = 2)]
		private object m_bootstrapContext;

		[OptionalField(VersionAdded = 2)]
		private string m_label;

		[OptionalField(VersionAdded = 2)]
		private string m_serializedNameType;

		[OptionalField(VersionAdded = 2)]
		private string m_serializedRoleType;

		[OptionalField(VersionAdded = 2)]
		private string m_serializedClaims;

		private enum SerializationMask
		{
			None,
			AuthenticationType,
			BootstrapConext,
			NameClaimType = 4,
			RoleClaimType = 8,
			HasClaims = 16,
			HasLabel = 32,
			Actor = 64,
			UserData = 128
		}
	}
}
