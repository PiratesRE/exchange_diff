using System;
using System.Collections;
using System.Runtime.Serialization;

namespace System.Security
{
	[Serializable]
	public sealed class ReadOnlyPermissionSet : PermissionSet
	{
		public ReadOnlyPermissionSet(SecurityElement permissionSetXml)
		{
			if (permissionSetXml == null)
			{
				throw new ArgumentNullException("permissionSetXml");
			}
			this.m_originXml = permissionSetXml.Copy();
			base.FromXml(this.m_originXml);
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext ctx)
		{
			this.m_deserializing = true;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			this.m_deserializing = false;
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public override PermissionSet Copy()
		{
			return new ReadOnlyPermissionSet(this.m_originXml);
		}

		public override SecurityElement ToXml()
		{
			return this.m_originXml.Copy();
		}

		protected override IEnumerator GetEnumeratorImpl()
		{
			return new ReadOnlyPermissionSetEnumerator(base.GetEnumeratorImpl());
		}

		protected override IPermission GetPermissionImpl(Type permClass)
		{
			IPermission permissionImpl = base.GetPermissionImpl(permClass);
			if (permissionImpl == null)
			{
				return null;
			}
			return permissionImpl.Copy();
		}

		protected override IPermission AddPermissionImpl(IPermission perm)
		{
			throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ModifyROPermSet"));
		}

		public override void FromXml(SecurityElement et)
		{
			if (this.m_deserializing)
			{
				base.FromXml(et);
				return;
			}
			throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ModifyROPermSet"));
		}

		protected override IPermission RemovePermissionImpl(Type permClass)
		{
			throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ModifyROPermSet"));
		}

		protected override IPermission SetPermissionImpl(IPermission perm)
		{
			throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ModifyROPermSet"));
		}

		private SecurityElement m_originXml;

		[NonSerialized]
		private bool m_deserializing;
	}
}
