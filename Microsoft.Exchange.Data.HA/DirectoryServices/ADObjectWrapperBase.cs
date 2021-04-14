using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ADObjectWrapperBase : IADObjectCommon
	{
		protected ADObjectWrapperBase(ADObject objToWrap)
		{
			this.Identity = objToWrap.Identity;
			this.Id = objToWrap.Id;
			this.Guid = objToWrap.Guid;
			this.Name = objToWrap.Name;
			this.IsValid = objToWrap.IsValid;
		}

		protected ADObjectWrapperBase(IADObjectCommon source)
		{
			this.Identity = source.Identity;
			this.Id = source.Id;
			this.Guid = source.Guid;
			this.Name = source.Name;
			this.IsValid = source.IsValid;
		}

		public ObjectId Identity { get; private set; }

		public ADObjectId Id { get; private set; }

		public Guid Guid { get; private set; }

		public string Name { get; private set; }

		public bool IsValid { get; private set; }

		protected bool IsMinimized { get; set; }

		public virtual void Minimize()
		{
			this.IsMinimized = true;
		}

		protected void HandleMinimizedProperty(string propName)
		{
			ADObjectWrapperBase.DisableMinimization();
			throw new MinimizedPropertyException(propName);
		}

		protected void CheckMinimizedProperty(string propName)
		{
			if (this.IsMinimized)
			{
				this.HandleMinimizedProperty(propName);
			}
		}

		public static bool IsMinimizeEnabled()
		{
			int num = 0;
			Exception ex = null;
			try
			{
				IRegistryReader instance = RegistryReader.Instance;
				num = instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "DisableADObjectMinimize", 0);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			catch (SecurityException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ADObjectWrapperBase.Tracer.TraceError<Exception>(0L, "IsMinimizeEnabled caught {0}", ex);
				num = 1;
			}
			return num == 0;
		}

		public static void DisableMinimization()
		{
			Exception ex = null;
			try
			{
				IRegistryWriter instance = RegistryWriter.Instance;
				instance.SetValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "DisableADObjectMinimize", 1, RegistryValueKind.DWord);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			catch (SecurityException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ADObjectWrapperBase.Tracer.TraceError<Exception>(0L, "DisableMinimization caught {0}", ex);
			}
		}

		private const string DisableKeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters";

		private const string DisableValueName = "DisableADObjectMinimize";

		protected static readonly Trace Tracer = ExTraceGlobals.ActiveManagerClientTracer;
	}
}
