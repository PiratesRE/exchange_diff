using System;
using System.Collections;
using System.Deployment.Internal.Isolation;
using System.Deployment.Internal.Isolation.Manifest;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(false)]
	[Serializable]
	public sealed class ActivationContext : IDisposable, ISerializable
	{
		private ActivationContext()
		{
		}

		[SecurityCritical]
		private ActivationContext(SerializationInfo info, StreamingContext context)
		{
			string applicationIdentityFullName = (string)info.GetValue("FullName", typeof(string));
			string[] array = (string[])info.GetValue("ManifestPaths", typeof(string[]));
			if (array == null)
			{
				this.CreateFromName(new ApplicationIdentity(applicationIdentityFullName));
				return;
			}
			this.CreateFromNameAndManifests(new ApplicationIdentity(applicationIdentityFullName), array);
		}

		internal ActivationContext(ApplicationIdentity applicationIdentity)
		{
			this.CreateFromName(applicationIdentity);
		}

		internal ActivationContext(ApplicationIdentity applicationIdentity, string[] manifestPaths)
		{
			this.CreateFromNameAndManifests(applicationIdentity, manifestPaths);
		}

		[SecuritySafeCritical]
		private void CreateFromName(ApplicationIdentity applicationIdentity)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			this._applicationIdentity = applicationIdentity;
			IEnumDefinitionIdentity enumDefinitionIdentity = this._applicationIdentity.Identity.EnumAppPath();
			this._definitionIdentities = new ArrayList(2);
			IDefinitionIdentity[] array = new IDefinitionIdentity[1];
			while (enumDefinitionIdentity.Next(1U, array) == 1U)
			{
				this._definitionIdentities.Add(array[0]);
			}
			this._definitionIdentities.TrimToSize();
			if (this._definitionIdentities.Count <= 1)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidAppId"));
			}
			this._manifestPaths = null;
			this._manifests = null;
			this._actContext = IsolationInterop.CreateActContext(this._applicationIdentity.Identity);
			this._form = ActivationContext.ContextForm.StoreBounded;
			this._appRunState = ActivationContext.ApplicationStateDisposition.Undefined;
		}

		[SecuritySafeCritical]
		private void CreateFromNameAndManifests(ApplicationIdentity applicationIdentity, string[] manifestPaths)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			if (manifestPaths == null)
			{
				throw new ArgumentNullException("manifestPaths");
			}
			this._applicationIdentity = applicationIdentity;
			IEnumDefinitionIdentity enumDefinitionIdentity = this._applicationIdentity.Identity.EnumAppPath();
			this._manifests = new ArrayList(2);
			this._manifestPaths = new string[manifestPaths.Length];
			IDefinitionIdentity[] array = new IDefinitionIdentity[1];
			int num = 0;
			while (enumDefinitionIdentity.Next(1U, array) == 1U)
			{
				ICMS icms = (ICMS)IsolationInterop.ParseManifest(manifestPaths[num], null, ref IsolationInterop.IID_ICMS);
				if (!IsolationInterop.IdentityAuthority.AreDefinitionsEqual(0U, icms.Identity, array[0]))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_IllegalAppIdMismatch"));
				}
				this._manifests.Add(icms);
				this._manifestPaths[num] = manifestPaths[num];
				num++;
			}
			if (num != manifestPaths.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_IllegalAppId"));
			}
			this._manifests.TrimToSize();
			if (this._manifests.Count <= 1)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidAppId"));
			}
			this._definitionIdentities = null;
			this._actContext = null;
			this._form = ActivationContext.ContextForm.Loose;
			this._appRunState = ActivationContext.ApplicationStateDisposition.Undefined;
		}

		~ActivationContext()
		{
			this.Dispose(false);
		}

		public static ActivationContext CreatePartialActivationContext(ApplicationIdentity identity)
		{
			return new ActivationContext(identity);
		}

		public static ActivationContext CreatePartialActivationContext(ApplicationIdentity identity, string[] manifestPaths)
		{
			return new ActivationContext(identity, manifestPaths);
		}

		public ApplicationIdentity Identity
		{
			get
			{
				return this._applicationIdentity;
			}
		}

		public ActivationContext.ContextForm Form
		{
			get
			{
				return this._form;
			}
		}

		public byte[] ApplicationManifestBytes
		{
			get
			{
				return this.GetApplicationManifestBytes();
			}
		}

		public byte[] DeploymentManifestBytes
		{
			get
			{
				return this.GetDeploymentManifestBytes();
			}
		}

		internal string[] ManifestPaths
		{
			get
			{
				return this._manifestPaths;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal string ApplicationDirectory
		{
			[SecurityCritical]
			get
			{
				if (this._form == ActivationContext.ContextForm.Loose)
				{
					return Path.GetDirectoryName(this._manifestPaths[this._manifestPaths.Length - 1]);
				}
				string result;
				this._actContext.ApplicationBasePath(0U, out result);
				return result;
			}
		}

		internal string DataDirectory
		{
			[SecurityCritical]
			get
			{
				if (this._form == ActivationContext.ContextForm.Loose)
				{
					return null;
				}
				string result;
				this._actContext.GetApplicationStateFilesystemLocation(1U, UIntPtr.Zero, IntPtr.Zero, out result);
				return result;
			}
		}

		internal ICMS ActivationContextData
		{
			[SecurityCritical]
			get
			{
				return this.ApplicationComponentManifest;
			}
		}

		internal ICMS DeploymentComponentManifest
		{
			[SecurityCritical]
			get
			{
				if (this._form == ActivationContext.ContextForm.Loose)
				{
					return (ICMS)this._manifests[0];
				}
				return this.GetComponentManifest((IDefinitionIdentity)this._definitionIdentities[0]);
			}
		}

		internal ICMS ApplicationComponentManifest
		{
			[SecurityCritical]
			get
			{
				if (this._form == ActivationContext.ContextForm.Loose)
				{
					return (ICMS)this._manifests[this._manifests.Count - 1];
				}
				return this.GetComponentManifest((IDefinitionIdentity)this._definitionIdentities[this._definitionIdentities.Count - 1]);
			}
		}

		internal ActivationContext.ApplicationStateDisposition LastApplicationStateResult
		{
			get
			{
				return this._appRunState;
			}
		}

		[SecurityCritical]
		internal ICMS GetComponentManifest(IDefinitionIdentity component)
		{
			object obj;
			this._actContext.GetComponentManifest(0U, component, ref IsolationInterop.IID_ICMS, out obj);
			return obj as ICMS;
		}

		[SecuritySafeCritical]
		internal byte[] GetDeploymentManifestBytes()
		{
			string manifestPath;
			if (this._form == ActivationContext.ContextForm.Loose)
			{
				manifestPath = this._manifestPaths[0];
			}
			else
			{
				object obj;
				this._actContext.GetComponentManifest(0U, (IDefinitionIdentity)this._definitionIdentities[0], ref IsolationInterop.IID_IManifestInformation, out obj);
				((IManifestInformation)obj).get_FullPath(out manifestPath);
				Marshal.ReleaseComObject(obj);
			}
			return ActivationContext.ReadBytesFromFile(manifestPath);
		}

		[SecuritySafeCritical]
		internal byte[] GetApplicationManifestBytes()
		{
			string manifestPath;
			if (this._form == ActivationContext.ContextForm.Loose)
			{
				manifestPath = this._manifestPaths[this._manifests.Count - 1];
			}
			else
			{
				object obj;
				this._actContext.GetComponentManifest(0U, (IDefinitionIdentity)this._definitionIdentities[1], ref IsolationInterop.IID_IManifestInformation, out obj);
				((IManifestInformation)obj).get_FullPath(out manifestPath);
				Marshal.ReleaseComObject(obj);
			}
			return ActivationContext.ReadBytesFromFile(manifestPath);
		}

		[SecuritySafeCritical]
		internal void PrepareForExecution()
		{
			if (this._form == ActivationContext.ContextForm.Loose)
			{
				return;
			}
			this._actContext.PrepareForExecution(IntPtr.Zero, IntPtr.Zero);
		}

		[SecuritySafeCritical]
		internal ActivationContext.ApplicationStateDisposition SetApplicationState(ActivationContext.ApplicationState s)
		{
			if (this._form == ActivationContext.ContextForm.Loose)
			{
				return ActivationContext.ApplicationStateDisposition.Undefined;
			}
			uint appRunState;
			this._actContext.SetApplicationRunningState(0U, (uint)s, out appRunState);
			this._appRunState = (ActivationContext.ApplicationStateDisposition)appRunState;
			return this._appRunState;
		}

		[SecuritySafeCritical]
		private void Dispose(bool fDisposing)
		{
			this._applicationIdentity = null;
			this._definitionIdentities = null;
			this._manifests = null;
			this._manifestPaths = null;
			if (this._actContext != null)
			{
				Marshal.ReleaseComObject(this._actContext);
			}
		}

		private static byte[] ReadBytesFromFile(string manifestPath)
		{
			byte[] array = null;
			using (FileStream fileStream = new FileStream(manifestPath, FileMode.Open, FileAccess.Read))
			{
				int num = (int)fileStream.Length;
				array = new byte[num];
				if (fileStream.CanSeek)
				{
					fileStream.Seek(0L, SeekOrigin.Begin);
				}
				fileStream.Read(array, 0, num);
			}
			return array;
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (this._applicationIdentity != null)
			{
				info.AddValue("FullName", this._applicationIdentity.FullName, typeof(string));
			}
			if (this._manifestPaths != null)
			{
				info.AddValue("ManifestPaths", this._manifestPaths, typeof(string[]));
			}
		}

		private ApplicationIdentity _applicationIdentity;

		private ArrayList _definitionIdentities;

		private ArrayList _manifests;

		private string[] _manifestPaths;

		private ActivationContext.ContextForm _form;

		private ActivationContext.ApplicationStateDisposition _appRunState;

		private IActContext _actContext;

		private const int DefaultComponentCount = 2;

		public enum ContextForm
		{
			Loose,
			StoreBounded
		}

		internal enum ApplicationState
		{
			Undefined,
			Starting,
			Running
		}

		internal enum ApplicationStateDisposition
		{
			Undefined,
			Starting,
			StartingMigrated = 65537,
			Running = 2,
			RunningFirstTime = 131074
		}
	}
}
