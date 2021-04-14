using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Util;
using System.Text;
using System.Threading;

namespace System.Security
{
	[ComVisible(true)]
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, Name = "mscorlib", PublicKey = "0x00000000000000000400000000000000")]
	[Serializable]
	public class PermissionSet : ISecurityEncodable, ICollection, IEnumerable, IStackWalk, IDeserializationCallback
	{
		[Conditional("_DEBUG")]
		private static void DEBUG_WRITE(string str)
		{
		}

		[Conditional("_DEBUG")]
		private static void DEBUG_COND_WRITE(bool exp, string str)
		{
		}

		[Conditional("_DEBUG")]
		private static void DEBUG_PRINTSTACK(Exception e)
		{
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext ctx)
		{
			this.Reset();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			if (this.m_serializedPermissionSet != null)
			{
				this.FromXml(SecurityElement.FromString(this.m_serializedPermissionSet));
			}
			else if (this.m_normalPermSet != null)
			{
				this.m_permSet = this.m_normalPermSet.SpecialUnion(this.m_unrestrictedPermSet);
			}
			else if (this.m_unrestrictedPermSet != null)
			{
				this.m_permSet = this.m_unrestrictedPermSet.SpecialUnion(this.m_normalPermSet);
			}
			this.m_serializedPermissionSet = null;
			this.m_normalPermSet = null;
			this.m_unrestrictedPermSet = null;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			if ((ctx.State & ~(StreamingContextStates.Clone | StreamingContextStates.CrossAppDomain)) != (StreamingContextStates)0)
			{
				this.m_serializedPermissionSet = this.ToString();
				if (this.m_permSet != null)
				{
					this.m_permSet.SpecialSplit(ref this.m_unrestrictedPermSet, ref this.m_normalPermSet, this.m_ignoreTypeLoadFailures);
				}
				this.m_permSetSaved = this.m_permSet;
				this.m_permSet = null;
			}
		}

		[OnSerialized]
		private void OnSerialized(StreamingContext context)
		{
			if ((context.State & ~(StreamingContextStates.Clone | StreamingContextStates.CrossAppDomain)) != (StreamingContextStates)0)
			{
				this.m_serializedPermissionSet = null;
				this.m_permSet = this.m_permSetSaved;
				this.m_permSetSaved = null;
				this.m_unrestrictedPermSet = null;
				this.m_normalPermSet = null;
			}
		}

		internal PermissionSet()
		{
			this.Reset();
			this.m_Unrestricted = true;
		}

		internal PermissionSet(bool fUnrestricted) : this()
		{
			this.SetUnrestricted(fUnrestricted);
		}

		public PermissionSet(PermissionState state) : this()
		{
			if (state == PermissionState.Unrestricted)
			{
				this.SetUnrestricted(true);
				return;
			}
			if (state == PermissionState.None)
			{
				this.SetUnrestricted(false);
				return;
			}
			throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPermissionState"));
		}

		public PermissionSet(PermissionSet permSet) : this()
		{
			if (permSet == null)
			{
				this.Reset();
				return;
			}
			this.m_Unrestricted = permSet.m_Unrestricted;
			this.m_CheckedForNonCas = permSet.m_CheckedForNonCas;
			this.m_ContainsCas = permSet.m_ContainsCas;
			this.m_ContainsNonCas = permSet.m_ContainsNonCas;
			this.m_ignoreTypeLoadFailures = permSet.m_ignoreTypeLoadFailures;
			if (permSet.m_permSet != null)
			{
				this.m_permSet = new TokenBasedSet(permSet.m_permSet);
				for (int i = this.m_permSet.GetStartingIndex(); i <= this.m_permSet.GetMaxUsedIndex(); i++)
				{
					object item = this.m_permSet.GetItem(i);
					IPermission permission = item as IPermission;
					ISecurityElementFactory securityElementFactory = item as ISecurityElementFactory;
					if (permission != null)
					{
						this.m_permSet.SetItem(i, permission.Copy());
					}
					else if (securityElementFactory != null)
					{
						this.m_permSet.SetItem(i, securityElementFactory.Copy());
					}
				}
			}
		}

		public virtual void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(this);
			while (permissionSetEnumeratorInternal.MoveNext())
			{
				object value = permissionSetEnumeratorInternal.Current;
				array.SetValue(value, index++);
			}
		}

		private PermissionSet(object trash, object junk)
		{
			this.m_Unrestricted = false;
		}

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal void Reset()
		{
			this.m_Unrestricted = false;
			this.m_allPermissionsDecoded = true;
			this.m_permSet = null;
			this.m_ignoreTypeLoadFailures = false;
			this.m_CheckedForNonCas = false;
			this.m_ContainsCas = false;
			this.m_ContainsNonCas = false;
			this.m_permSetSaved = null;
		}

		internal void CheckSet()
		{
			if (this.m_permSet == null)
			{
				this.m_permSet = new TokenBasedSet();
			}
		}

		public bool IsEmpty()
		{
			if (this.m_Unrestricted)
			{
				return false;
			}
			if (this.m_permSet == null || this.m_permSet.FastIsEmpty())
			{
				return true;
			}
			PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(this);
			while (permissionSetEnumeratorInternal.MoveNext())
			{
				object obj = permissionSetEnumeratorInternal.Current;
				IPermission permission = (IPermission)obj;
				if (!permission.IsSubsetOf(null))
				{
					return false;
				}
			}
			return true;
		}

		internal bool FastIsEmpty()
		{
			return !this.m_Unrestricted && (this.m_permSet == null || this.m_permSet.FastIsEmpty());
		}

		public virtual int Count
		{
			get
			{
				int num = 0;
				if (this.m_permSet != null)
				{
					num += this.m_permSet.GetCount();
				}
				return num;
			}
		}

		internal IPermission GetPermission(int index)
		{
			if (this.m_permSet == null)
			{
				return null;
			}
			object item = this.m_permSet.GetItem(index);
			if (item == null)
			{
				return null;
			}
			IPermission permission = item as IPermission;
			if (permission != null)
			{
				return permission;
			}
			permission = this.CreatePermission(item, index);
			if (permission == null)
			{
				return null;
			}
			return permission;
		}

		internal IPermission GetPermission(PermissionToken permToken)
		{
			if (permToken == null)
			{
				return null;
			}
			return this.GetPermission(permToken.m_index);
		}

		internal IPermission GetPermission(IPermission perm)
		{
			if (perm == null)
			{
				return null;
			}
			return this.GetPermission(PermissionToken.GetToken(perm));
		}

		public IPermission GetPermission(Type permClass)
		{
			return this.GetPermissionImpl(permClass);
		}

		protected virtual IPermission GetPermissionImpl(Type permClass)
		{
			if (permClass == null)
			{
				return null;
			}
			return this.GetPermission(PermissionToken.FindToken(permClass));
		}

		public IPermission SetPermission(IPermission perm)
		{
			return this.SetPermissionImpl(perm);
		}

		protected virtual IPermission SetPermissionImpl(IPermission perm)
		{
			if (perm == null)
			{
				return null;
			}
			PermissionToken token = PermissionToken.GetToken(perm);
			if ((token.m_type & PermissionTokenType.IUnrestricted) != (PermissionTokenType)0)
			{
				this.m_Unrestricted = false;
			}
			this.CheckSet();
			IPermission permission = this.GetPermission(token.m_index);
			this.m_CheckedForNonCas = false;
			this.m_permSet.SetItem(token.m_index, perm);
			return perm;
		}

		public IPermission AddPermission(IPermission perm)
		{
			return this.AddPermissionImpl(perm);
		}

		protected virtual IPermission AddPermissionImpl(IPermission perm)
		{
			if (perm == null)
			{
				return null;
			}
			this.m_CheckedForNonCas = false;
			PermissionToken token = PermissionToken.GetToken(perm);
			if (this.IsUnrestricted() && (token.m_type & PermissionTokenType.IUnrestricted) != (PermissionTokenType)0)
			{
				Type type = perm.GetType();
				return (IPermission)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, new object[]
				{
					PermissionState.Unrestricted
				}, null);
			}
			this.CheckSet();
			IPermission permission = this.GetPermission(token.m_index);
			if (permission != null)
			{
				IPermission permission2 = permission.Union(perm);
				this.m_permSet.SetItem(token.m_index, permission2);
				return permission2;
			}
			this.m_permSet.SetItem(token.m_index, perm);
			return perm;
		}

		private IPermission RemovePermission(int index)
		{
			if (this.GetPermission(index) == null)
			{
				return null;
			}
			return (IPermission)this.m_permSet.RemoveItem(index);
		}

		public IPermission RemovePermission(Type permClass)
		{
			return this.RemovePermissionImpl(permClass);
		}

		protected virtual IPermission RemovePermissionImpl(Type permClass)
		{
			if (permClass == null)
			{
				return null;
			}
			PermissionToken permissionToken = PermissionToken.FindToken(permClass);
			if (permissionToken == null)
			{
				return null;
			}
			return this.RemovePermission(permissionToken.m_index);
		}

		internal void SetUnrestricted(bool unrestricted)
		{
			this.m_Unrestricted = unrestricted;
			if (unrestricted)
			{
				this.m_permSet = null;
			}
		}

		public bool IsUnrestricted()
		{
			return this.m_Unrestricted;
		}

		internal bool IsSubsetOfHelper(PermissionSet target, PermissionSet.IsSubsetOfType type, out IPermission firstPermThatFailed, bool ignoreNonCas)
		{
			firstPermThatFailed = null;
			if (target == null || target.FastIsEmpty())
			{
				if (this.IsEmpty())
				{
					return true;
				}
				firstPermThatFailed = this.GetFirstPerm();
				return false;
			}
			else
			{
				if (this.IsUnrestricted() && !target.IsUnrestricted())
				{
					return false;
				}
				if (this.m_permSet == null)
				{
					return true;
				}
				target.CheckSet();
				for (int i = this.m_permSet.GetStartingIndex(); i <= this.m_permSet.GetMaxUsedIndex(); i++)
				{
					IPermission permission = this.GetPermission(i);
					if (permission != null && !permission.IsSubsetOf(null))
					{
						IPermission permission2 = target.GetPermission(i);
						if (!target.m_Unrestricted)
						{
							CodeAccessPermission codeAccessPermission = permission as CodeAccessPermission;
							if (codeAccessPermission == null)
							{
								if (!ignoreNonCas && !permission.IsSubsetOf(permission2))
								{
									firstPermThatFailed = permission;
									return false;
								}
							}
							else
							{
								firstPermThatFailed = permission;
								switch (type)
								{
								case PermissionSet.IsSubsetOfType.Normal:
									if (!permission.IsSubsetOf(permission2))
									{
										return false;
									}
									break;
								case PermissionSet.IsSubsetOfType.CheckDemand:
									if (!codeAccessPermission.CheckDemand((CodeAccessPermission)permission2))
									{
										return false;
									}
									break;
								case PermissionSet.IsSubsetOfType.CheckPermitOnly:
									if (!codeAccessPermission.CheckPermitOnly((CodeAccessPermission)permission2))
									{
										return false;
									}
									break;
								case PermissionSet.IsSubsetOfType.CheckAssertion:
									if (!codeAccessPermission.CheckAssert((CodeAccessPermission)permission2))
									{
										return false;
									}
									break;
								}
								firstPermThatFailed = null;
							}
						}
					}
				}
				return true;
			}
		}

		public bool IsSubsetOf(PermissionSet target)
		{
			IPermission permission;
			return this.IsSubsetOfHelper(target, PermissionSet.IsSubsetOfType.Normal, out permission, false);
		}

		internal bool CheckDemand(PermissionSet target, out IPermission firstPermThatFailed)
		{
			return this.IsSubsetOfHelper(target, PermissionSet.IsSubsetOfType.CheckDemand, out firstPermThatFailed, true);
		}

		internal bool CheckPermitOnly(PermissionSet target, out IPermission firstPermThatFailed)
		{
			return this.IsSubsetOfHelper(target, PermissionSet.IsSubsetOfType.CheckPermitOnly, out firstPermThatFailed, true);
		}

		internal bool CheckAssertion(PermissionSet target)
		{
			IPermission permission;
			return this.IsSubsetOfHelper(target, PermissionSet.IsSubsetOfType.CheckAssertion, out permission, true);
		}

		internal bool CheckDeny(PermissionSet deniedSet, out IPermission firstPermThatFailed)
		{
			firstPermThatFailed = null;
			if (deniedSet == null || deniedSet.FastIsEmpty() || this.FastIsEmpty())
			{
				return true;
			}
			if (this.m_Unrestricted && deniedSet.m_Unrestricted)
			{
				return false;
			}
			PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(this);
			while (permissionSetEnumeratorInternal.MoveNext())
			{
				object obj = permissionSetEnumeratorInternal.Current;
				CodeAccessPermission codeAccessPermission = obj as CodeAccessPermission;
				if (codeAccessPermission != null && !codeAccessPermission.IsSubsetOf(null))
				{
					if (deniedSet.m_Unrestricted)
					{
						firstPermThatFailed = codeAccessPermission;
						return false;
					}
					CodeAccessPermission denied = (CodeAccessPermission)deniedSet.GetPermission(permissionSetEnumeratorInternal.GetCurrentIndex());
					if (!codeAccessPermission.CheckDeny(denied))
					{
						firstPermThatFailed = codeAccessPermission;
						return false;
					}
				}
			}
			if (this.m_Unrestricted)
			{
				PermissionSetEnumeratorInternal permissionSetEnumeratorInternal2 = new PermissionSetEnumeratorInternal(deniedSet);
				while (permissionSetEnumeratorInternal2.MoveNext())
				{
					if (permissionSetEnumeratorInternal2.Current is IPermission)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal void CheckDecoded(CodeAccessPermission demandedPerm, PermissionToken tokenDemandedPerm)
		{
			if (this.m_allPermissionsDecoded || this.m_permSet == null)
			{
				return;
			}
			if (tokenDemandedPerm == null)
			{
				tokenDemandedPerm = PermissionToken.GetToken(demandedPerm);
			}
			this.CheckDecoded(tokenDemandedPerm.m_index);
		}

		internal void CheckDecoded(int index)
		{
			if (this.m_allPermissionsDecoded || this.m_permSet == null)
			{
				return;
			}
			this.GetPermission(index);
		}

		internal void CheckDecoded(PermissionSet demandedSet)
		{
			if (this.m_allPermissionsDecoded || this.m_permSet == null)
			{
				return;
			}
			PermissionSetEnumeratorInternal enumeratorInternal = demandedSet.GetEnumeratorInternal();
			while (enumeratorInternal.MoveNext())
			{
				this.CheckDecoded(enumeratorInternal.GetCurrentIndex());
			}
		}

		internal static void SafeChildAdd(SecurityElement parent, ISecurityElementFactory child, bool copy)
		{
			if (child == parent)
			{
				return;
			}
			if (child.GetTag().Equals("IPermission") || child.GetTag().Equals("Permission"))
			{
				parent.AddChild(child);
				return;
			}
			if (parent.Tag.Equals(child.GetTag()))
			{
				SecurityElement securityElement = (SecurityElement)child;
				for (int i = 0; i < securityElement.InternalChildren.Count; i++)
				{
					ISecurityElementFactory child2 = (ISecurityElementFactory)securityElement.InternalChildren[i];
					parent.AddChildNoDuplicates(child2);
				}
				return;
			}
			parent.AddChild((ISecurityElementFactory)(copy ? child.Copy() : child));
		}

		internal void InplaceIntersect(PermissionSet other)
		{
			Exception ex = null;
			this.m_CheckedForNonCas = false;
			if (this == other)
			{
				return;
			}
			if (other == null || other.FastIsEmpty())
			{
				this.Reset();
				return;
			}
			if (this.FastIsEmpty())
			{
				return;
			}
			int num = (this.m_permSet == null) ? -1 : this.m_permSet.GetMaxUsedIndex();
			int num2 = (other.m_permSet == null) ? -1 : other.m_permSet.GetMaxUsedIndex();
			if (this.IsUnrestricted() && num < num2)
			{
				num = num2;
				this.CheckSet();
			}
			if (other.IsUnrestricted())
			{
				other.CheckSet();
			}
			for (int i = 0; i <= num; i++)
			{
				object item = this.m_permSet.GetItem(i);
				IPermission permission = item as IPermission;
				ISecurityElementFactory securityElementFactory = item as ISecurityElementFactory;
				object item2 = other.m_permSet.GetItem(i);
				IPermission permission2 = item2 as IPermission;
				ISecurityElementFactory securityElementFactory2 = item2 as ISecurityElementFactory;
				if (item != null || item2 != null)
				{
					if (securityElementFactory != null && securityElementFactory2 != null)
					{
						if (securityElementFactory.GetTag().Equals("PermissionIntersection") || securityElementFactory.GetTag().Equals("PermissionUnrestrictedIntersection"))
						{
							PermissionSet.SafeChildAdd((SecurityElement)securityElementFactory, securityElementFactory2, true);
						}
						else
						{
							bool copy = true;
							if (this.IsUnrestricted())
							{
								SecurityElement securityElement = new SecurityElement("PermissionUnrestrictedUnion");
								securityElement.AddAttribute("class", securityElementFactory.Attribute("class"));
								PermissionSet.SafeChildAdd(securityElement, securityElementFactory, false);
								securityElementFactory = securityElement;
							}
							if (other.IsUnrestricted())
							{
								SecurityElement securityElement2 = new SecurityElement("PermissionUnrestrictedUnion");
								securityElement2.AddAttribute("class", securityElementFactory2.Attribute("class"));
								PermissionSet.SafeChildAdd(securityElement2, securityElementFactory2, true);
								securityElementFactory2 = securityElement2;
								copy = false;
							}
							SecurityElement securityElement3 = new SecurityElement("PermissionIntersection");
							securityElement3.AddAttribute("class", securityElementFactory.Attribute("class"));
							PermissionSet.SafeChildAdd(securityElement3, securityElementFactory, false);
							PermissionSet.SafeChildAdd(securityElement3, securityElementFactory2, copy);
							this.m_permSet.SetItem(i, securityElement3);
						}
					}
					else if (item == null)
					{
						if (this.IsUnrestricted())
						{
							if (securityElementFactory2 != null)
							{
								SecurityElement securityElement4 = new SecurityElement("PermissionUnrestrictedIntersection");
								securityElement4.AddAttribute("class", securityElementFactory2.Attribute("class"));
								PermissionSet.SafeChildAdd(securityElement4, securityElementFactory2, true);
								this.m_permSet.SetItem(i, securityElement4);
							}
							else
							{
								PermissionToken permissionToken = (PermissionToken)PermissionToken.s_tokenSet.GetItem(i);
								if ((permissionToken.m_type & PermissionTokenType.IUnrestricted) != (PermissionTokenType)0)
								{
									this.m_permSet.SetItem(i, permission2.Copy());
								}
							}
						}
					}
					else if (item2 == null)
					{
						if (other.IsUnrestricted())
						{
							if (securityElementFactory != null)
							{
								SecurityElement securityElement5 = new SecurityElement("PermissionUnrestrictedIntersection");
								securityElement5.AddAttribute("class", securityElementFactory.Attribute("class"));
								PermissionSet.SafeChildAdd(securityElement5, securityElementFactory, false);
								this.m_permSet.SetItem(i, securityElement5);
							}
							else
							{
								PermissionToken permissionToken2 = (PermissionToken)PermissionToken.s_tokenSet.GetItem(i);
								if ((permissionToken2.m_type & PermissionTokenType.IUnrestricted) == (PermissionTokenType)0)
								{
									this.m_permSet.SetItem(i, null);
								}
							}
						}
						else
						{
							this.m_permSet.SetItem(i, null);
						}
					}
					else
					{
						if (securityElementFactory != null)
						{
							permission = this.CreatePermission(securityElementFactory, i);
						}
						if (securityElementFactory2 != null)
						{
							permission2 = other.CreatePermission(securityElementFactory2, i);
						}
						try
						{
							IPermission item3;
							if (permission == null)
							{
								item3 = permission2;
							}
							else if (permission2 == null)
							{
								item3 = permission;
							}
							else
							{
								item3 = permission.Intersect(permission2);
							}
							this.m_permSet.SetItem(i, item3);
						}
						catch (Exception ex2)
						{
							if (ex == null)
							{
								ex = ex2;
							}
						}
					}
				}
			}
			this.m_Unrestricted = (this.m_Unrestricted && other.m_Unrestricted);
			if (ex != null)
			{
				throw ex;
			}
		}

		public PermissionSet Intersect(PermissionSet other)
		{
			if (other == null || other.FastIsEmpty() || this.FastIsEmpty())
			{
				return null;
			}
			int num = (this.m_permSet == null) ? -1 : this.m_permSet.GetMaxUsedIndex();
			int num2 = (other.m_permSet == null) ? -1 : other.m_permSet.GetMaxUsedIndex();
			int num3 = (num < num2) ? num : num2;
			if (this.IsUnrestricted() && num3 < num2)
			{
				num3 = num2;
				this.CheckSet();
			}
			if (other.IsUnrestricted() && num3 < num)
			{
				num3 = num;
				other.CheckSet();
			}
			PermissionSet permissionSet = new PermissionSet(false);
			if (num3 > -1)
			{
				permissionSet.m_permSet = new TokenBasedSet();
			}
			for (int i = 0; i <= num3; i++)
			{
				object item = this.m_permSet.GetItem(i);
				IPermission permission = item as IPermission;
				ISecurityElementFactory securityElementFactory = item as ISecurityElementFactory;
				object item2 = other.m_permSet.GetItem(i);
				IPermission permission2 = item2 as IPermission;
				ISecurityElementFactory securityElementFactory2 = item2 as ISecurityElementFactory;
				if (item != null || item2 != null)
				{
					if (securityElementFactory != null && securityElementFactory2 != null)
					{
						bool copy = true;
						bool copy2 = true;
						SecurityElement securityElement = new SecurityElement("PermissionIntersection");
						securityElement.AddAttribute("class", securityElementFactory2.Attribute("class"));
						if (this.IsUnrestricted())
						{
							SecurityElement securityElement2 = new SecurityElement("PermissionUnrestrictedUnion");
							securityElement2.AddAttribute("class", securityElementFactory.Attribute("class"));
							PermissionSet.SafeChildAdd(securityElement2, securityElementFactory, true);
							copy2 = false;
							securityElementFactory = securityElement2;
						}
						if (other.IsUnrestricted())
						{
							SecurityElement securityElement3 = new SecurityElement("PermissionUnrestrictedUnion");
							securityElement3.AddAttribute("class", securityElementFactory2.Attribute("class"));
							PermissionSet.SafeChildAdd(securityElement3, securityElementFactory2, true);
							copy = false;
							securityElementFactory2 = securityElement3;
						}
						PermissionSet.SafeChildAdd(securityElement, securityElementFactory2, copy);
						PermissionSet.SafeChildAdd(securityElement, securityElementFactory, copy2);
						permissionSet.m_permSet.SetItem(i, securityElement);
					}
					else if (item == null)
					{
						if (this.m_Unrestricted)
						{
							if (securityElementFactory2 != null)
							{
								SecurityElement securityElement4 = new SecurityElement("PermissionUnrestrictedIntersection");
								securityElement4.AddAttribute("class", securityElementFactory2.Attribute("class"));
								PermissionSet.SafeChildAdd(securityElement4, securityElementFactory2, true);
								permissionSet.m_permSet.SetItem(i, securityElement4);
							}
							else if (permission2 != null)
							{
								PermissionToken permissionToken = (PermissionToken)PermissionToken.s_tokenSet.GetItem(i);
								if ((permissionToken.m_type & PermissionTokenType.IUnrestricted) != (PermissionTokenType)0)
								{
									permissionSet.m_permSet.SetItem(i, permission2.Copy());
								}
							}
						}
					}
					else if (item2 == null)
					{
						if (other.m_Unrestricted)
						{
							if (securityElementFactory != null)
							{
								SecurityElement securityElement5 = new SecurityElement("PermissionUnrestrictedIntersection");
								securityElement5.AddAttribute("class", securityElementFactory.Attribute("class"));
								PermissionSet.SafeChildAdd(securityElement5, securityElementFactory, true);
								permissionSet.m_permSet.SetItem(i, securityElement5);
							}
							else if (permission != null)
							{
								PermissionToken permissionToken2 = (PermissionToken)PermissionToken.s_tokenSet.GetItem(i);
								if ((permissionToken2.m_type & PermissionTokenType.IUnrestricted) != (PermissionTokenType)0)
								{
									permissionSet.m_permSet.SetItem(i, permission.Copy());
								}
							}
						}
					}
					else
					{
						if (securityElementFactory != null)
						{
							permission = this.CreatePermission(securityElementFactory, i);
						}
						if (securityElementFactory2 != null)
						{
							permission2 = other.CreatePermission(securityElementFactory2, i);
						}
						IPermission item3;
						if (permission == null)
						{
							item3 = permission2;
						}
						else if (permission2 == null)
						{
							item3 = permission;
						}
						else
						{
							item3 = permission.Intersect(permission2);
						}
						permissionSet.m_permSet.SetItem(i, item3);
					}
				}
			}
			permissionSet.m_Unrestricted = (this.m_Unrestricted && other.m_Unrestricted);
			if (permissionSet.FastIsEmpty())
			{
				return null;
			}
			return permissionSet;
		}

		internal void InplaceUnion(PermissionSet other)
		{
			if (this == other)
			{
				return;
			}
			if (other == null || other.FastIsEmpty())
			{
				return;
			}
			this.m_CheckedForNonCas = false;
			this.m_Unrestricted = (this.m_Unrestricted || other.m_Unrestricted);
			if (this.m_Unrestricted)
			{
				this.m_permSet = null;
				return;
			}
			int num = -1;
			if (other.m_permSet != null)
			{
				num = other.m_permSet.GetMaxUsedIndex();
				this.CheckSet();
			}
			Exception ex = null;
			for (int i = 0; i <= num; i++)
			{
				object item = this.m_permSet.GetItem(i);
				IPermission permission = item as IPermission;
				ISecurityElementFactory securityElementFactory = item as ISecurityElementFactory;
				object item2 = other.m_permSet.GetItem(i);
				IPermission permission2 = item2 as IPermission;
				ISecurityElementFactory securityElementFactory2 = item2 as ISecurityElementFactory;
				if (item != null || item2 != null)
				{
					if (securityElementFactory != null && securityElementFactory2 != null)
					{
						if (securityElementFactory.GetTag().Equals("PermissionUnion") || securityElementFactory.GetTag().Equals("PermissionUnrestrictedUnion"))
						{
							PermissionSet.SafeChildAdd((SecurityElement)securityElementFactory, securityElementFactory2, true);
						}
						else
						{
							SecurityElement securityElement;
							if (this.IsUnrestricted() || other.IsUnrestricted())
							{
								securityElement = new SecurityElement("PermissionUnrestrictedUnion");
							}
							else
							{
								securityElement = new SecurityElement("PermissionUnion");
							}
							securityElement.AddAttribute("class", securityElementFactory.Attribute("class"));
							PermissionSet.SafeChildAdd(securityElement, securityElementFactory, false);
							PermissionSet.SafeChildAdd(securityElement, securityElementFactory2, true);
							this.m_permSet.SetItem(i, securityElement);
						}
					}
					else if (item == null)
					{
						if (securityElementFactory2 != null)
						{
							this.m_permSet.SetItem(i, securityElementFactory2.Copy());
						}
						else if (permission2 != null)
						{
							PermissionToken permissionToken = (PermissionToken)PermissionToken.s_tokenSet.GetItem(i);
							if ((permissionToken.m_type & PermissionTokenType.IUnrestricted) == (PermissionTokenType)0 || !this.m_Unrestricted)
							{
								this.m_permSet.SetItem(i, permission2.Copy());
							}
						}
					}
					else if (item2 != null)
					{
						if (securityElementFactory != null)
						{
							permission = this.CreatePermission(securityElementFactory, i);
						}
						if (securityElementFactory2 != null)
						{
							permission2 = other.CreatePermission(securityElementFactory2, i);
						}
						try
						{
							IPermission item3;
							if (permission == null)
							{
								item3 = permission2;
							}
							else if (permission2 == null)
							{
								item3 = permission;
							}
							else
							{
								item3 = permission.Union(permission2);
							}
							this.m_permSet.SetItem(i, item3);
						}
						catch (Exception ex2)
						{
							if (ex == null)
							{
								ex = ex2;
							}
						}
					}
				}
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		public PermissionSet Union(PermissionSet other)
		{
			if (other == null || other.FastIsEmpty())
			{
				return this.Copy();
			}
			if (this.FastIsEmpty())
			{
				return other.Copy();
			}
			PermissionSet permissionSet = new PermissionSet();
			permissionSet.m_Unrestricted = (this.m_Unrestricted || other.m_Unrestricted);
			if (permissionSet.m_Unrestricted)
			{
				return permissionSet;
			}
			this.CheckSet();
			other.CheckSet();
			int num = (this.m_permSet.GetMaxUsedIndex() > other.m_permSet.GetMaxUsedIndex()) ? this.m_permSet.GetMaxUsedIndex() : other.m_permSet.GetMaxUsedIndex();
			permissionSet.m_permSet = new TokenBasedSet();
			for (int i = 0; i <= num; i++)
			{
				object item = this.m_permSet.GetItem(i);
				IPermission permission = item as IPermission;
				ISecurityElementFactory securityElementFactory = item as ISecurityElementFactory;
				object item2 = other.m_permSet.GetItem(i);
				IPermission permission2 = item2 as IPermission;
				ISecurityElementFactory securityElementFactory2 = item2 as ISecurityElementFactory;
				if (item != null || item2 != null)
				{
					if (securityElementFactory != null && securityElementFactory2 != null)
					{
						SecurityElement securityElement;
						if (this.IsUnrestricted() || other.IsUnrestricted())
						{
							securityElement = new SecurityElement("PermissionUnrestrictedUnion");
						}
						else
						{
							securityElement = new SecurityElement("PermissionUnion");
						}
						securityElement.AddAttribute("class", securityElementFactory.Attribute("class"));
						PermissionSet.SafeChildAdd(securityElement, securityElementFactory, true);
						PermissionSet.SafeChildAdd(securityElement, securityElementFactory2, true);
						permissionSet.m_permSet.SetItem(i, securityElement);
					}
					else if (item == null)
					{
						if (securityElementFactory2 != null)
						{
							permissionSet.m_permSet.SetItem(i, securityElementFactory2.Copy());
						}
						else if (permission2 != null)
						{
							PermissionToken permissionToken = (PermissionToken)PermissionToken.s_tokenSet.GetItem(i);
							if ((permissionToken.m_type & PermissionTokenType.IUnrestricted) == (PermissionTokenType)0 || !permissionSet.m_Unrestricted)
							{
								permissionSet.m_permSet.SetItem(i, permission2.Copy());
							}
						}
					}
					else if (item2 == null)
					{
						if (securityElementFactory != null)
						{
							permissionSet.m_permSet.SetItem(i, securityElementFactory.Copy());
						}
						else if (permission != null)
						{
							PermissionToken permissionToken2 = (PermissionToken)PermissionToken.s_tokenSet.GetItem(i);
							if ((permissionToken2.m_type & PermissionTokenType.IUnrestricted) == (PermissionTokenType)0 || !permissionSet.m_Unrestricted)
							{
								permissionSet.m_permSet.SetItem(i, permission.Copy());
							}
						}
					}
					else
					{
						if (securityElementFactory != null)
						{
							permission = this.CreatePermission(securityElementFactory, i);
						}
						if (securityElementFactory2 != null)
						{
							permission2 = other.CreatePermission(securityElementFactory2, i);
						}
						IPermission item3;
						if (permission == null)
						{
							item3 = permission2;
						}
						else if (permission2 == null)
						{
							item3 = permission;
						}
						else
						{
							item3 = permission.Union(permission2);
						}
						permissionSet.m_permSet.SetItem(i, item3);
					}
				}
			}
			return permissionSet;
		}

		internal void MergeDeniedSet(PermissionSet denied)
		{
			if (denied == null || denied.FastIsEmpty() || this.FastIsEmpty())
			{
				return;
			}
			this.m_CheckedForNonCas = false;
			if (this.m_permSet == null || denied.m_permSet == null)
			{
				return;
			}
			int num = (denied.m_permSet.GetMaxUsedIndex() > this.m_permSet.GetMaxUsedIndex()) ? this.m_permSet.GetMaxUsedIndex() : denied.m_permSet.GetMaxUsedIndex();
			for (int i = 0; i <= num; i++)
			{
				IPermission permission = denied.m_permSet.GetItem(i) as IPermission;
				if (permission != null)
				{
					IPermission permission2 = this.m_permSet.GetItem(i) as IPermission;
					if (permission2 == null && !this.m_Unrestricted)
					{
						denied.m_permSet.SetItem(i, null);
					}
					else if (permission2 != null && permission != null && permission2.IsSubsetOf(permission))
					{
						this.m_permSet.SetItem(i, null);
						denied.m_permSet.SetItem(i, null);
					}
				}
			}
		}

		internal bool Contains(IPermission perm)
		{
			if (perm == null)
			{
				return true;
			}
			if (this.m_Unrestricted)
			{
				return true;
			}
			if (this.FastIsEmpty())
			{
				return false;
			}
			PermissionToken token = PermissionToken.GetToken(perm);
			if (this.m_permSet.GetItem(token.m_index) == null)
			{
				return perm.IsSubsetOf(null);
			}
			IPermission permission = this.GetPermission(token.m_index);
			if (permission != null)
			{
				return perm.IsSubsetOf(permission);
			}
			return perm.IsSubsetOf(null);
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			PermissionSet permissionSet = obj as PermissionSet;
			if (permissionSet == null)
			{
				return false;
			}
			if (this.m_Unrestricted != permissionSet.m_Unrestricted)
			{
				return false;
			}
			this.CheckSet();
			permissionSet.CheckSet();
			this.DecodeAllPermissions();
			permissionSet.DecodeAllPermissions();
			int num = Math.Max(this.m_permSet.GetMaxUsedIndex(), permissionSet.m_permSet.GetMaxUsedIndex());
			for (int i = 0; i <= num; i++)
			{
				IPermission permission = (IPermission)this.m_permSet.GetItem(i);
				IPermission permission2 = (IPermission)permissionSet.m_permSet.GetItem(i);
				if (permission != null || permission2 != null)
				{
					if (permission == null)
					{
						if (!permission2.IsSubsetOf(null))
						{
							return false;
						}
					}
					else if (permission2 == null)
					{
						if (!permission.IsSubsetOf(null))
						{
							return false;
						}
					}
					else if (!permission.Equals(permission2))
					{
						return false;
					}
				}
			}
			return true;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			int num = this.m_Unrestricted ? -1 : 0;
			if (this.m_permSet != null)
			{
				this.DecodeAllPermissions();
				int maxUsedIndex = this.m_permSet.GetMaxUsedIndex();
				for (int i = this.m_permSet.GetStartingIndex(); i <= maxUsedIndex; i++)
				{
					IPermission permission = (IPermission)this.m_permSet.GetItem(i);
					if (permission != null)
					{
						num ^= permission.GetHashCode();
					}
				}
			}
			return num;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Demand()
		{
			if (this.FastIsEmpty())
			{
				return;
			}
			this.ContainsNonCodeAccessPermissions();
			if (this.m_ContainsCas)
			{
				StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCallersCaller;
				CodeAccessSecurityEngine.Check(this.GetCasOnlySet(), ref stackCrawlMark);
			}
			if (this.m_ContainsNonCas)
			{
				this.DemandNonCAS();
			}
		}

		[SecurityCritical]
		internal void DemandNonCAS()
		{
			this.ContainsNonCodeAccessPermissions();
			if (this.m_ContainsNonCas && this.m_permSet != null)
			{
				this.CheckSet();
				for (int i = this.m_permSet.GetStartingIndex(); i <= this.m_permSet.GetMaxUsedIndex(); i++)
				{
					IPermission permission = this.GetPermission(i);
					if (permission != null && !(permission is CodeAccessPermission))
					{
						permission.Demand();
					}
				}
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Assert()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			SecurityRuntime.Assert(this, ref stackCrawlMark);
		}

		[SecuritySafeCritical]
		[Obsolete("Deny is obsolete and will be removed in a future release of the .NET Framework. See http://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Deny()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			SecurityRuntime.Deny(this, ref stackCrawlMark);
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void PermitOnly()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			SecurityRuntime.PermitOnly(this, ref stackCrawlMark);
		}

		internal IPermission GetFirstPerm()
		{
			IEnumerator enumerator = this.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return null;
			}
			return enumerator.Current as IPermission;
		}

		public virtual PermissionSet Copy()
		{
			return new PermissionSet(this);
		}

		internal PermissionSet CopyWithNoIdentityPermissions()
		{
			PermissionSet permissionSet = new PermissionSet(this);
			permissionSet.RemovePermission(typeof(GacIdentityPermission));
			permissionSet.RemovePermission(typeof(PublisherIdentityPermission));
			permissionSet.RemovePermission(typeof(StrongNameIdentityPermission));
			permissionSet.RemovePermission(typeof(UrlIdentityPermission));
			permissionSet.RemovePermission(typeof(ZoneIdentityPermission));
			return permissionSet;
		}

		public IEnumerator GetEnumerator()
		{
			return this.GetEnumeratorImpl();
		}

		protected virtual IEnumerator GetEnumeratorImpl()
		{
			return new PermissionSetEnumerator(this);
		}

		internal PermissionSetEnumeratorInternal GetEnumeratorInternal()
		{
			return new PermissionSetEnumeratorInternal(this);
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		private void NormalizePermissionSet()
		{
			PermissionSet permissionSet = new PermissionSet(false);
			permissionSet.m_Unrestricted = this.m_Unrestricted;
			if (this.m_permSet != null)
			{
				for (int i = this.m_permSet.GetStartingIndex(); i <= this.m_permSet.GetMaxUsedIndex(); i++)
				{
					object item = this.m_permSet.GetItem(i);
					IPermission permission = item as IPermission;
					ISecurityElementFactory securityElementFactory = item as ISecurityElementFactory;
					if (securityElementFactory != null)
					{
						permission = this.CreatePerm(securityElementFactory);
					}
					if (permission != null)
					{
						permissionSet.SetPermission(permission);
					}
				}
			}
			this.m_permSet = permissionSet.m_permSet;
		}

		private bool DecodeXml(byte[] data, HostProtectionResource fullTrustOnlyResources, HostProtectionResource inaccessibleResources)
		{
			if (data != null && data.Length != 0)
			{
				this.FromXml(new Parser(data, Tokenizer.ByteTokenEncoding.UnicodeTokens).GetTopElement());
			}
			this.FilterHostProtectionPermissions(fullTrustOnlyResources, inaccessibleResources);
			this.DecodeAllPermissions();
			return true;
		}

		private void DecodeAllPermissions()
		{
			if (this.m_permSet == null)
			{
				this.m_allPermissionsDecoded = true;
				return;
			}
			int maxUsedIndex = this.m_permSet.GetMaxUsedIndex();
			for (int i = 0; i <= maxUsedIndex; i++)
			{
				this.GetPermission(i);
			}
			this.m_allPermissionsDecoded = true;
		}

		internal void FilterHostProtectionPermissions(HostProtectionResource fullTrustOnly, HostProtectionResource inaccessible)
		{
			HostProtectionPermission.protectedResources = fullTrustOnly;
			HostProtectionPermission hostProtectionPermission = (HostProtectionPermission)this.GetPermission(HostProtectionPermission.GetTokenIndex());
			if (hostProtectionPermission == null)
			{
				return;
			}
			HostProtectionPermission hostProtectionPermission2 = (HostProtectionPermission)hostProtectionPermission.Intersect(new HostProtectionPermission(fullTrustOnly));
			if (hostProtectionPermission2 == null)
			{
				this.RemovePermission(typeof(HostProtectionPermission));
				return;
			}
			if (hostProtectionPermission2.Resources != hostProtectionPermission.Resources)
			{
				this.SetPermission(hostProtectionPermission2);
			}
		}

		public virtual void FromXml(SecurityElement et)
		{
			this.FromXml(et, false, false);
		}

		internal static bool IsPermissionTag(string tag, bool allowInternalOnly)
		{
			return tag.Equals("Permission") || tag.Equals("IPermission") || (allowInternalOnly && (tag.Equals("PermissionUnion") || tag.Equals("PermissionIntersection") || tag.Equals("PermissionUnrestrictedIntersection") || tag.Equals("PermissionUnrestrictedUnion")));
		}

		internal virtual void FromXml(SecurityElement et, bool allowInternalOnly, bool ignoreTypeLoadFailures)
		{
			if (et == null)
			{
				throw new ArgumentNullException("et");
			}
			if (!et.Tag.Equals("PermissionSet"))
			{
				throw new ArgumentException(string.Format(null, Environment.GetResourceString("Argument_InvalidXMLElement"), "PermissionSet", base.GetType().FullName));
			}
			this.Reset();
			this.m_ignoreTypeLoadFailures = ignoreTypeLoadFailures;
			this.m_allPermissionsDecoded = false;
			this.m_Unrestricted = XMLUtil.IsUnrestricted(et);
			if (et.InternalChildren != null)
			{
				int count = et.InternalChildren.Count;
				for (int i = 0; i < count; i++)
				{
					SecurityElement securityElement = (SecurityElement)et.Children[i];
					if (PermissionSet.IsPermissionTag(securityElement.Tag, allowInternalOnly))
					{
						string text = securityElement.Attribute("class");
						PermissionToken permissionToken;
						object obj;
						if (text != null)
						{
							permissionToken = PermissionToken.GetToken(text);
							if (permissionToken == null)
							{
								obj = this.CreatePerm(securityElement);
								if (obj != null)
								{
									permissionToken = PermissionToken.GetToken((IPermission)obj);
								}
							}
							else
							{
								obj = securityElement;
							}
						}
						else
						{
							IPermission permission = this.CreatePerm(securityElement);
							if (permission == null)
							{
								permissionToken = null;
								obj = null;
							}
							else
							{
								permissionToken = PermissionToken.GetToken(permission);
								obj = permission;
							}
						}
						if (permissionToken != null && obj != null)
						{
							if (this.m_permSet == null)
							{
								this.m_permSet = new TokenBasedSet();
							}
							if (this.m_permSet.GetItem(permissionToken.m_index) != null)
							{
								IPermission target;
								if (this.m_permSet.GetItem(permissionToken.m_index) is IPermission)
								{
									target = (IPermission)this.m_permSet.GetItem(permissionToken.m_index);
								}
								else
								{
									target = this.CreatePerm((SecurityElement)this.m_permSet.GetItem(permissionToken.m_index));
								}
								if (obj is IPermission)
								{
									obj = ((IPermission)obj).Union(target);
								}
								else
								{
									obj = this.CreatePerm((SecurityElement)obj).Union(target);
								}
							}
							if (this.m_Unrestricted && obj is IPermission)
							{
								obj = null;
							}
							this.m_permSet.SetItem(permissionToken.m_index, obj);
						}
					}
				}
			}
		}

		internal virtual void FromXml(SecurityDocument doc, int position, bool allowInternalOnly)
		{
			if (doc == null)
			{
				throw new ArgumentNullException("doc");
			}
			if (!doc.GetTagForElement(position).Equals("PermissionSet"))
			{
				throw new ArgumentException(string.Format(null, Environment.GetResourceString("Argument_InvalidXMLElement"), "PermissionSet", base.GetType().FullName));
			}
			this.Reset();
			this.m_allPermissionsDecoded = false;
			Exception ex = null;
			string attributeForElement = doc.GetAttributeForElement(position, "Unrestricted");
			if (attributeForElement != null)
			{
				this.m_Unrestricted = (attributeForElement.Equals("True") || attributeForElement.Equals("true") || attributeForElement.Equals("TRUE"));
			}
			else
			{
				this.m_Unrestricted = false;
			}
			ArrayList childrenPositionForElement = doc.GetChildrenPositionForElement(position);
			int count = childrenPositionForElement.Count;
			for (int i = 0; i < count; i++)
			{
				int position2 = (int)childrenPositionForElement[i];
				if (PermissionSet.IsPermissionTag(doc.GetTagForElement(position2), allowInternalOnly))
				{
					try
					{
						string attributeForElement2 = doc.GetAttributeForElement(position2, "class");
						PermissionToken permissionToken;
						object obj;
						if (attributeForElement2 != null)
						{
							permissionToken = PermissionToken.GetToken(attributeForElement2);
							if (permissionToken == null)
							{
								obj = this.CreatePerm(doc.GetElement(position2, true));
								if (obj != null)
								{
									permissionToken = PermissionToken.GetToken((IPermission)obj);
								}
							}
							else
							{
								obj = ((ISecurityElementFactory)new SecurityDocumentElement(doc, position2)).CreateSecurityElement();
							}
						}
						else
						{
							IPermission permission = this.CreatePerm(doc.GetElement(position2, true));
							if (permission == null)
							{
								permissionToken = null;
								obj = null;
							}
							else
							{
								permissionToken = PermissionToken.GetToken(permission);
								obj = permission;
							}
						}
						if (permissionToken != null && obj != null)
						{
							if (this.m_permSet == null)
							{
								this.m_permSet = new TokenBasedSet();
							}
							IPermission permission2 = null;
							if (this.m_permSet.GetItem(permissionToken.m_index) != null)
							{
								if (this.m_permSet.GetItem(permissionToken.m_index) is IPermission)
								{
									permission2 = (IPermission)this.m_permSet.GetItem(permissionToken.m_index);
								}
								else
								{
									permission2 = this.CreatePerm(this.m_permSet.GetItem(permissionToken.m_index));
								}
							}
							if (permission2 != null)
							{
								if (obj is IPermission)
								{
									obj = permission2.Union((IPermission)obj);
								}
								else
								{
									obj = permission2.Union(this.CreatePerm(obj));
								}
							}
							if (this.m_Unrestricted && obj is IPermission)
							{
								obj = null;
							}
							this.m_permSet.SetItem(permissionToken.m_index, obj);
						}
					}
					catch (Exception ex2)
					{
						if (ex == null)
						{
							ex = ex2;
						}
					}
				}
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		private IPermission CreatePerm(object obj)
		{
			return PermissionSet.CreatePerm(obj, this.m_ignoreTypeLoadFailures);
		}

		internal static IPermission CreatePerm(object obj, bool ignoreTypeLoadFailures)
		{
			SecurityElement securityElement = obj as SecurityElement;
			ISecurityElementFactory securityElementFactory = obj as ISecurityElementFactory;
			if (securityElement == null && securityElementFactory != null)
			{
				securityElement = securityElementFactory.CreateSecurityElement();
			}
			IPermission permission = null;
			string tag = securityElement.Tag;
			if (!(tag == "PermissionUnion"))
			{
				if (!(tag == "PermissionIntersection"))
				{
					if (!(tag == "PermissionUnrestrictedUnion"))
					{
						if (!(tag == "PermissionUnrestrictedIntersection"))
						{
							if (tag == "IPermission" || tag == "Permission")
							{
								permission = securityElement.ToPermission(ignoreTypeLoadFailures);
							}
						}
						else
						{
							foreach (object obj2 in securityElement.Children)
							{
								IPermission permission2 = PermissionSet.CreatePerm((SecurityElement)obj2, ignoreTypeLoadFailures);
								if (permission2 == null)
								{
									return null;
								}
								PermissionToken token = PermissionToken.GetToken(permission2);
								if ((token.m_type & PermissionTokenType.IUnrestricted) != (PermissionTokenType)0)
								{
									if (permission != null)
									{
										permission = permission2.Intersect(permission);
									}
									else
									{
										permission = permission2;
									}
								}
								else
								{
									permission = null;
								}
								if (permission == null)
								{
									return null;
								}
							}
						}
					}
					else
					{
						IEnumerator enumerator = securityElement.Children.GetEnumerator();
						bool flag = true;
						while (enumerator.MoveNext())
						{
							object obj3 = enumerator.Current;
							IPermission permission3 = PermissionSet.CreatePerm((SecurityElement)obj3, ignoreTypeLoadFailures);
							if (permission3 != null)
							{
								PermissionToken token2 = PermissionToken.GetToken(permission3);
								if ((token2.m_type & PermissionTokenType.IUnrestricted) != (PermissionTokenType)0)
								{
									permission = XMLUtil.CreatePermission(PermissionSet.GetPermissionElement((SecurityElement)enumerator.Current), PermissionState.Unrestricted, ignoreTypeLoadFailures);
									break;
								}
								if (flag)
								{
									permission = permission3;
								}
								else
								{
									permission = permission3.Union(permission);
								}
								flag = false;
							}
						}
					}
				}
				else
				{
					foreach (object obj4 in securityElement.Children)
					{
						IPermission permission4 = PermissionSet.CreatePerm((SecurityElement)obj4, ignoreTypeLoadFailures);
						if (permission != null)
						{
							permission = permission.Intersect(permission4);
						}
						else
						{
							permission = permission4;
						}
						if (permission == null)
						{
							return null;
						}
					}
				}
			}
			else
			{
				foreach (object obj5 in securityElement.Children)
				{
					IPermission permission5 = PermissionSet.CreatePerm((SecurityElement)obj5, ignoreTypeLoadFailures);
					if (permission != null)
					{
						permission = permission.Union(permission5);
					}
					else
					{
						permission = permission5;
					}
				}
			}
			return permission;
		}

		internal IPermission CreatePermission(object obj, int index)
		{
			IPermission permission = this.CreatePerm(obj);
			if (permission == null)
			{
				return null;
			}
			if (this.m_Unrestricted)
			{
				permission = null;
			}
			this.CheckSet();
			this.m_permSet.SetItem(index, permission);
			if (permission != null)
			{
				PermissionToken token = PermissionToken.GetToken(permission);
				if (token != null && token.m_index != index)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_UnableToGeneratePermissionSet"));
				}
			}
			return permission;
		}

		private static SecurityElement GetPermissionElement(SecurityElement el)
		{
			string tag = el.Tag;
			if (tag == "IPermission" || tag == "Permission")
			{
				return el;
			}
			IEnumerator enumerator = el.Children.GetEnumerator();
			if (enumerator.MoveNext())
			{
				return PermissionSet.GetPermissionElement((SecurityElement)enumerator.Current);
			}
			return null;
		}

		internal static SecurityElement CreateEmptyPermissionSetXml()
		{
			SecurityElement securityElement = new SecurityElement("PermissionSet");
			securityElement.AddAttribute("class", "System.Security.PermissionSet");
			securityElement.AddAttribute("version", "1");
			return securityElement;
		}

		internal SecurityElement ToXml(string permName)
		{
			SecurityElement securityElement = new SecurityElement("PermissionSet");
			securityElement.AddAttribute("class", permName);
			securityElement.AddAttribute("version", "1");
			PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(this);
			if (this.m_Unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			while (permissionSetEnumeratorInternal.MoveNext())
			{
				object obj = permissionSetEnumeratorInternal.Current;
				IPermission permission = (IPermission)obj;
				if (!this.m_Unrestricted)
				{
					securityElement.AddChild(permission.ToXml());
				}
			}
			return securityElement;
		}

		internal SecurityElement InternalToXml()
		{
			SecurityElement securityElement = new SecurityElement("PermissionSet");
			securityElement.AddAttribute("class", base.GetType().FullName);
			securityElement.AddAttribute("version", "1");
			if (this.m_Unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			if (this.m_permSet != null)
			{
				int maxUsedIndex = this.m_permSet.GetMaxUsedIndex();
				for (int i = this.m_permSet.GetStartingIndex(); i <= maxUsedIndex; i++)
				{
					object item = this.m_permSet.GetItem(i);
					if (item != null)
					{
						if (item is IPermission)
						{
							if (!this.m_Unrestricted)
							{
								securityElement.AddChild(((IPermission)item).ToXml());
							}
						}
						else
						{
							securityElement.AddChild((SecurityElement)item);
						}
					}
				}
			}
			return securityElement;
		}

		public virtual SecurityElement ToXml()
		{
			return this.ToXml("System.Security.PermissionSet");
		}

		internal byte[] EncodeXml()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode);
			binaryWriter.Write(this.ToXml().ToString());
			binaryWriter.Flush();
			memoryStream.Position = 2L;
			int num = (int)memoryStream.Length - 2;
			byte[] array = new byte[num];
			memoryStream.Read(array, 0, array.Length);
			return array;
		}

		[Obsolete("This method is obsolete and shoud no longer be used.")]
		public static byte[] ConvertPermissionSet(string inFormat, byte[] inData, string outFormat)
		{
			throw new NotImplementedException();
		}

		public bool ContainsNonCodeAccessPermissions()
		{
			if (this.m_CheckedForNonCas)
			{
				return this.m_ContainsNonCas;
			}
			lock (this)
			{
				if (this.m_CheckedForNonCas)
				{
					return this.m_ContainsNonCas;
				}
				this.m_ContainsCas = false;
				this.m_ContainsNonCas = false;
				if (this.IsUnrestricted())
				{
					this.m_ContainsCas = true;
				}
				if (this.m_permSet != null)
				{
					PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(this);
					while (permissionSetEnumeratorInternal.MoveNext() && (!this.m_ContainsCas || !this.m_ContainsNonCas))
					{
						IPermission permission = permissionSetEnumeratorInternal.Current as IPermission;
						if (permission != null)
						{
							if (permission is CodeAccessPermission)
							{
								this.m_ContainsCas = true;
							}
							else
							{
								this.m_ContainsNonCas = true;
							}
						}
					}
				}
				this.m_CheckedForNonCas = true;
			}
			return this.m_ContainsNonCas;
		}

		private PermissionSet GetCasOnlySet()
		{
			if (!this.m_ContainsNonCas)
			{
				return this;
			}
			if (this.IsUnrestricted())
			{
				return this;
			}
			PermissionSet permissionSet = new PermissionSet(false);
			PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(this);
			while (permissionSetEnumeratorInternal.MoveNext())
			{
				object obj = permissionSetEnumeratorInternal.Current;
				IPermission permission = (IPermission)obj;
				if (permission is CodeAccessPermission)
				{
					permissionSet.AddPermission(permission);
				}
			}
			permissionSet.m_CheckedForNonCas = true;
			permissionSet.m_ContainsCas = !permissionSet.IsEmpty();
			permissionSet.m_ContainsNonCas = false;
			return permissionSet;
		}

		[SecurityCritical]
		private static void SetupSecurity()
		{
			PolicyLevel policyLevel = PolicyLevel.CreateAppDomainLevel();
			CodeGroup codeGroup = new UnionCodeGroup(new AllMembershipCondition(), policyLevel.GetNamedPermissionSet("Execution"));
			StrongNamePublicKeyBlob blob = new StrongNamePublicKeyBlob("002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293");
			CodeGroup group = new UnionCodeGroup(new StrongNameMembershipCondition(blob, null, null), policyLevel.GetNamedPermissionSet("FullTrust"));
			StrongNamePublicKeyBlob blob2 = new StrongNamePublicKeyBlob("00000000000000000400000000000000");
			CodeGroup group2 = new UnionCodeGroup(new StrongNameMembershipCondition(blob2, null, null), policyLevel.GetNamedPermissionSet("FullTrust"));
			CodeGroup group3 = new UnionCodeGroup(new GacMembershipCondition(), policyLevel.GetNamedPermissionSet("FullTrust"));
			codeGroup.AddChild(group);
			codeGroup.AddChild(group2);
			codeGroup.AddChild(group3);
			policyLevel.RootCodeGroup = codeGroup;
			try
			{
				AppDomain.CurrentDomain.SetAppDomainPolicy(policyLevel);
			}
			catch (PolicyException)
			{
			}
		}

		private static void MergePermission(IPermission perm, bool separateCasFromNonCas, ref PermissionSet casPset, ref PermissionSet nonCasPset)
		{
			if (perm == null)
			{
				return;
			}
			if (!separateCasFromNonCas || perm is CodeAccessPermission)
			{
				if (casPset == null)
				{
					casPset = new PermissionSet(false);
				}
				IPermission permission = casPset.GetPermission(perm);
				IPermission target = casPset.AddPermission(perm);
				if (permission != null && !permission.IsSubsetOf(target))
				{
					throw new NotSupportedException(Environment.GetResourceString("NotSupported_DeclarativeUnion"));
				}
			}
			else
			{
				if (nonCasPset == null)
				{
					nonCasPset = new PermissionSet(false);
				}
				IPermission permission2 = nonCasPset.GetPermission(perm);
				IPermission target2 = nonCasPset.AddPermission(perm);
				if (permission2 != null && !permission2.IsSubsetOf(target2))
				{
					throw new NotSupportedException(Environment.GetResourceString("NotSupported_DeclarativeUnion"));
				}
			}
		}

		private static byte[] CreateSerialized(object[] attrs, bool serialize, ref byte[] nonCasBlob, out PermissionSet casPset, HostProtectionResource fullTrustOnlyResources, bool allowEmptyPermissionSets)
		{
			casPset = null;
			PermissionSet permissionSet = null;
			for (int i = 0; i < attrs.Length; i++)
			{
				if (attrs[i] is PermissionSetAttribute)
				{
					PermissionSet permissionSet2 = ((PermissionSetAttribute)attrs[i]).CreatePermissionSet();
					if (permissionSet2 == null)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_UnableToGeneratePermissionSet"));
					}
					PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(permissionSet2);
					while (permissionSetEnumeratorInternal.MoveNext())
					{
						object obj = permissionSetEnumeratorInternal.Current;
						IPermission perm = (IPermission)obj;
						PermissionSet.MergePermission(perm, serialize, ref casPset, ref permissionSet);
					}
					if (casPset == null)
					{
						casPset = new PermissionSet(false);
					}
					if (permissionSet2.IsUnrestricted())
					{
						casPset.SetUnrestricted(true);
					}
				}
				else
				{
					IPermission perm2 = ((SecurityAttribute)attrs[i]).CreatePermission();
					PermissionSet.MergePermission(perm2, serialize, ref casPset, ref permissionSet);
				}
			}
			if (casPset != null)
			{
				casPset.FilterHostProtectionPermissions(fullTrustOnlyResources, HostProtectionResource.None);
				casPset.ContainsNonCodeAccessPermissions();
				if (allowEmptyPermissionSets && casPset.IsEmpty())
				{
					casPset = null;
				}
			}
			if (permissionSet != null)
			{
				permissionSet.FilterHostProtectionPermissions(fullTrustOnlyResources, HostProtectionResource.None);
				permissionSet.ContainsNonCodeAccessPermissions();
				if (allowEmptyPermissionSets && permissionSet.IsEmpty())
				{
					permissionSet = null;
				}
			}
			byte[] result = null;
			nonCasBlob = null;
			if (serialize)
			{
				if (casPset != null)
				{
					result = casPset.EncodeXml();
				}
				if (permissionSet != null)
				{
					nonCasBlob = permissionSet.EncodeXml();
				}
			}
			return result;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			this.NormalizePermissionSet();
			this.m_CheckedForNonCas = false;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void RevertAssert()
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			SecurityRuntime.RevertAssert(ref stackCrawlMark);
		}

		internal static PermissionSet RemoveRefusedPermissionSet(PermissionSet assertSet, PermissionSet refusedSet, out bool bFailedToCompress)
		{
			PermissionSet permissionSet = null;
			bFailedToCompress = false;
			if (assertSet == null)
			{
				return null;
			}
			if (refusedSet != null)
			{
				if (refusedSet.IsUnrestricted())
				{
					return null;
				}
				PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(refusedSet);
				while (permissionSetEnumeratorInternal.MoveNext())
				{
					object obj = permissionSetEnumeratorInternal.Current;
					CodeAccessPermission codeAccessPermission = (CodeAccessPermission)obj;
					int currentIndex = permissionSetEnumeratorInternal.GetCurrentIndex();
					if (codeAccessPermission != null)
					{
						CodeAccessPermission codeAccessPermission2 = (CodeAccessPermission)assertSet.GetPermission(currentIndex);
						try
						{
							if (codeAccessPermission.Intersect(codeAccessPermission2) != null)
							{
								if (!codeAccessPermission.Equals(codeAccessPermission2))
								{
									bFailedToCompress = true;
									return assertSet;
								}
								if (permissionSet == null)
								{
									permissionSet = assertSet.Copy();
								}
								permissionSet.RemovePermission(currentIndex);
							}
						}
						catch (ArgumentException)
						{
							if (permissionSet == null)
							{
								permissionSet = assertSet.Copy();
							}
							permissionSet.RemovePermission(currentIndex);
						}
						continue;
					}
				}
			}
			if (permissionSet != null)
			{
				return permissionSet;
			}
			return assertSet;
		}

		internal static void RemoveAssertedPermissionSet(PermissionSet demandSet, PermissionSet assertSet, out PermissionSet alteredDemandSet)
		{
			alteredDemandSet = null;
			PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(demandSet);
			while (permissionSetEnumeratorInternal.MoveNext())
			{
				object obj = permissionSetEnumeratorInternal.Current;
				CodeAccessPermission codeAccessPermission = (CodeAccessPermission)obj;
				int currentIndex = permissionSetEnumeratorInternal.GetCurrentIndex();
				if (codeAccessPermission != null)
				{
					CodeAccessPermission asserted = (CodeAccessPermission)assertSet.GetPermission(currentIndex);
					try
					{
						if (codeAccessPermission.CheckAssert(asserted))
						{
							if (alteredDemandSet == null)
							{
								alteredDemandSet = demandSet.Copy();
							}
							alteredDemandSet.RemovePermission(currentIndex);
						}
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}

		internal static bool IsIntersectingAssertedPermissions(PermissionSet assertSet1, PermissionSet assertSet2)
		{
			bool result = false;
			if (assertSet1 != null && assertSet2 != null)
			{
				PermissionSetEnumeratorInternal permissionSetEnumeratorInternal = new PermissionSetEnumeratorInternal(assertSet2);
				while (permissionSetEnumeratorInternal.MoveNext())
				{
					object obj = permissionSetEnumeratorInternal.Current;
					CodeAccessPermission codeAccessPermission = (CodeAccessPermission)obj;
					int currentIndex = permissionSetEnumeratorInternal.GetCurrentIndex();
					if (codeAccessPermission != null)
					{
						CodeAccessPermission codeAccessPermission2 = (CodeAccessPermission)assertSet1.GetPermission(currentIndex);
						try
						{
							if (codeAccessPermission2 != null && !codeAccessPermission2.Equals(codeAccessPermission))
							{
								result = true;
							}
						}
						catch (ArgumentException)
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		internal bool IgnoreTypeLoadFailures
		{
			set
			{
				this.m_ignoreTypeLoadFailures = value;
			}
		}

		private bool m_Unrestricted;

		[OptionalField(VersionAdded = 2)]
		private bool m_allPermissionsDecoded;

		[OptionalField(VersionAdded = 2)]
		internal TokenBasedSet m_permSet;

		[OptionalField(VersionAdded = 2)]
		private bool m_ignoreTypeLoadFailures;

		[OptionalField(VersionAdded = 2)]
		private string m_serializedPermissionSet;

		[NonSerialized]
		private bool m_CheckedForNonCas;

		[NonSerialized]
		private bool m_ContainsCas;

		[NonSerialized]
		private bool m_ContainsNonCas;

		[NonSerialized]
		private TokenBasedSet m_permSetSaved;

		private bool readableonly;

		private TokenBasedSet m_unrestrictedPermSet;

		private TokenBasedSet m_normalPermSet;

		[OptionalField(VersionAdded = 2)]
		private bool m_canUnrestrictedOverride;

		internal static readonly PermissionSet s_fullTrust = new PermissionSet(true);

		private const string s_str_PermissionSet = "PermissionSet";

		private const string s_str_Permission = "Permission";

		private const string s_str_IPermission = "IPermission";

		private const string s_str_Unrestricted = "Unrestricted";

		private const string s_str_PermissionUnion = "PermissionUnion";

		private const string s_str_PermissionIntersection = "PermissionIntersection";

		private const string s_str_PermissionUnrestrictedUnion = "PermissionUnrestrictedUnion";

		private const string s_str_PermissionUnrestrictedIntersection = "PermissionUnrestrictedIntersection";

		internal enum IsSubsetOfType
		{
			Normal,
			CheckDemand,
			CheckPermitOnly,
			CheckAssertion
		}
	}
}
