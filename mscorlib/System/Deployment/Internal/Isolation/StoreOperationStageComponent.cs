using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	internal struct StoreOperationStageComponent
	{
		public void Destroy()
		{
		}

		public StoreOperationStageComponent(IDefinitionAppId app, string Manifest)
		{
			this = new StoreOperationStageComponent(app, null, Manifest);
		}

		public StoreOperationStageComponent(IDefinitionAppId app, IDefinitionIdentity comp, string Manifest)
		{
			this.Size = (uint)Marshal.SizeOf(typeof(StoreOperationStageComponent));
			this.Flags = StoreOperationStageComponent.OpFlags.Nothing;
			this.Application = app;
			this.Component = comp;
			this.ManifestPath = Manifest;
		}

		[MarshalAs(UnmanagedType.U4)]
		public uint Size;

		[MarshalAs(UnmanagedType.U4)]
		public StoreOperationStageComponent.OpFlags Flags;

		[MarshalAs(UnmanagedType.Interface)]
		public IDefinitionAppId Application;

		[MarshalAs(UnmanagedType.Interface)]
		public IDefinitionIdentity Component;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string ManifestPath;

		[Flags]
		public enum OpFlags
		{
			Nothing = 0
		}

		public enum Disposition
		{
			Failed,
			Installed,
			Refreshed,
			AlreadyInstalled
		}
	}
}
