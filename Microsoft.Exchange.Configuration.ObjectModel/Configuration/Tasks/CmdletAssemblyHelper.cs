using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public static class CmdletAssemblyHelper
	{
		public static string[] ManagementCmdletAssemblyNames
		{
			get
			{
				return CmdletAssemblyHelper._managementCmdletAssemblyNames;
			}
		}

		public static bool IsCmdletAssembly(string assemblyNameWithoutExtension)
		{
			return CmdletAssemblyHelper._managementCmdletAssemblyNamesWithoutExtension.Any((string cmdletAssemblyNameWithoutExtension) => string.Equals(assemblyNameWithoutExtension, cmdletAssemblyNameWithoutExtension, StringComparison.OrdinalIgnoreCase));
		}

		public static bool EnsureTargetTypesLoaded(string[] assemblyNames, string[] typeNames)
		{
			if (assemblyNames == null)
			{
				throw new ArgumentNullException("assemblyNames");
			}
			if (typeNames == null)
			{
				throw new ArgumentNullException("typeNames");
			}
			if (assemblyNames.Length == 0)
			{
				assemblyNames = CmdletAssemblyHelper.ManagementCmdletAssemblyNames;
			}
			if (assemblyNames.Length == 0)
			{
				throw new ArgumentException("Cannot find any assembly to load types.");
			}
			TaskHelper.LoadExchangeAssemblyAndReferencesFromSpecificPathForAssemblies(ConfigurationContext.Setup.BinPath, assemblyNames);
			foreach (string valueToConvert in typeNames)
			{
				Type type;
				if (!LanguagePrimitives.TryConvertTo<Type>(valueToConvert, out type))
				{
					return false;
				}
			}
			return true;
		}

		public static string[] AppendCmdletAssemblyNames(params string[] assemblyNames)
		{
			if (assemblyNames == null || assemblyNames.Length == 0)
			{
				return (from x in CmdletAssemblyHelper.ManagementCmdletAssemblyNames
				select x).ToArray<string>();
			}
			string[] array = new string[assemblyNames.Length + CmdletAssemblyHelper.ManagementCmdletAssemblyNames.Length];
			assemblyNames.CopyTo(array, 0);
			CmdletAssemblyHelper.ManagementCmdletAssemblyNames.CopyTo(array, assemblyNames.Length);
			return array;
		}

		public static Assembly[] GetAllCmdletAssemblies(string basePath)
		{
			if (!Directory.Exists(basePath))
			{
				throw new ArgumentException(string.Format("Base path '{0}' doesn't exist, abort the operation.", basePath ?? "null"), "basePath");
			}
			return (from x in CmdletAssemblyHelper.ManagementCmdletAssemblyNames
			select Assembly.LoadFrom(Path.Combine(basePath, x))).ToArray<Assembly>();
		}

		public static Assembly[] LoadingAllCmdletAssembliesAndReference(string basePath, params string[] additionAssemblyNames)
		{
			if (!Directory.Exists(basePath))
			{
				throw new ArgumentException(string.Format("Base path '{0}' doesn't exist, abort the operation.", basePath ?? "null"), "basePath");
			}
			if (additionAssemblyNames == null || additionAssemblyNames.Length == 0)
			{
				return TaskHelper.LoadExchangeAssemblyAndReferencesFromSpecificPathForAssemblies(basePath, CmdletAssemblyHelper.ManagementCmdletAssemblyNames);
			}
			List<string> list = new List<string>(CmdletAssemblyHelper.ManagementCmdletAssemblyNames);
			list.AddRange(additionAssemblyNames);
			return TaskHelper.LoadExchangeAssemblyAndReferencesFromSpecificPathForAssemblies(basePath, list.ToArray());
		}

		public static Type[] GetAllCmdletTypes(string basePath)
		{
			if (!Directory.Exists(basePath))
			{
				throw new ArgumentException(string.Format("Base path '{0}' doesn't exist, abort the operation.", basePath ?? "null"), "basePath");
			}
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in CmdletAssemblyHelper.GetAllCmdletAssemblies(basePath))
			{
				list.AddRange(from type in CmdletAssemblyHelper.GetAssemblyTypes(assembly)
				where !type.IsAbstract && type.GetCustomAttributes(typeof(CmdletAttribute), false).Any<object>()
				select type);
			}
			return list.ToArray();
		}

		public static string GetScriptForAllCmdlets(string followingPipeline)
		{
			string arg = string.Join(" -or ", from x in CmdletAssemblyHelper.ManagementCmdletAssemblyNames
			select string.Format("$_.DLL -like '*{0}'", x));
			string arg2 = string.IsNullOrEmpty(followingPipeline) ? string.Empty : followingPipeline;
			return string.Format("Get-Command | where {{ ( {0} ) -and  {1} }} {2}", arg, "$_.ModuleName -ne 'Microsoft.Exchange.Management.PowerShell.Setup'", arg2);
		}

		private static Type[] GetAssemblyTypes(Assembly assembly)
		{
			Type[] result = null;
			try
			{
				result = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				result = (from type in ex.Types
				where type != null
				select type).ToArray<Type>();
			}
			return result;
		}

		private static readonly string[] _managementCmdletAssemblyNames = new string[]
		{
			"Microsoft.Exchange.Management.dll",
			"Microsoft.Exchange.Management.Recipient.dll",
			"Microsoft.Exchange.Management.Mobility.dll",
			"Microsoft.Exchange.Management.Transport.dll"
		};

		private static readonly string[] _managementCmdletAssemblyNamesWithoutExtension = (from assembly in CmdletAssemblyHelper._managementCmdletAssemblyNames
		select Path.GetFileNameWithoutExtension(assembly)).ToArray<string>();
	}
}
