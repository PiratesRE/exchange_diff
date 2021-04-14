using System;
using System.Security.Util;

namespace System.Security
{
	internal static class BuiltInPermissionSets
	{
		internal static NamedPermissionSet Everything
		{
			get
			{
				return BuiltInPermissionSets.GetOrDeserializeExtendablePermissionSet(ref BuiltInPermissionSets.s_everything, BuiltInPermissionSets.s_everythingXml, "<PermissionSet class = \"System.Security.PermissionSet\"\r\n                             version = \"1\">\r\n                  <IPermission class = \"System.Security.Permissions.MediaPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.WebBrowserPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n              </PermissionSet>");
			}
		}

		internal static NamedPermissionSet Execution
		{
			get
			{
				return BuiltInPermissionSets.GetOrDeserializePermissionSet(ref BuiltInPermissionSets.s_execution, BuiltInPermissionSets.s_executionXml);
			}
		}

		internal static NamedPermissionSet FullTrust
		{
			get
			{
				return BuiltInPermissionSets.GetOrDeserializePermissionSet(ref BuiltInPermissionSets.s_fullTrust, BuiltInPermissionSets.s_fullTrustXml);
			}
		}

		internal static NamedPermissionSet Internet
		{
			get
			{
				return BuiltInPermissionSets.GetOrDeserializeExtendablePermissionSet(ref BuiltInPermissionSets.s_internet, BuiltInPermissionSets.s_internetXml, "<PermissionSet class = \"System.Security.PermissionSet\"\r\n                             version = \"1\">\r\n                  <IPermission class = \"System.Security.Permissions.MediaPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Audio=\"SafeAudio\" Video=\"SafeVideo\" Image=\"SafeImage\" />\r\n                  <IPermission class = \"System.Security.Permissions.WebBrowserPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Level=\"Safe\" />\r\n              </PermissionSet>");
			}
		}

		internal static NamedPermissionSet LocalIntranet
		{
			get
			{
				return BuiltInPermissionSets.GetOrDeserializeExtendablePermissionSet(ref BuiltInPermissionSets.s_localIntranet, BuiltInPermissionSets.s_localIntranetXml, "<PermissionSet class = \"System.Security.PermissionSet\"\r\n                             version = \"1\">\r\n                  <IPermission class = \"System.Security.Permissions.MediaPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Audio=\"SafeAudio\" Video=\"SafeVideo\" Image=\"SafeImage\" />\r\n                  <IPermission class = \"System.Security.Permissions.WebBrowserPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Level=\"Safe\" />\r\n              </PermissionSet>");
			}
		}

		internal static NamedPermissionSet Nothing
		{
			get
			{
				return BuiltInPermissionSets.GetOrDeserializePermissionSet(ref BuiltInPermissionSets.s_nothing, BuiltInPermissionSets.s_nothingXml);
			}
		}

		internal static NamedPermissionSet SkipVerification
		{
			get
			{
				return BuiltInPermissionSets.GetOrDeserializePermissionSet(ref BuiltInPermissionSets.s_skipVerification, BuiltInPermissionSets.s_skipVerificationXml);
			}
		}

		private static NamedPermissionSet GetOrDeserializeExtendablePermissionSet(ref NamedPermissionSet permissionSet, string permissionSetXml, string extensionXml)
		{
			if (permissionSet == null)
			{
				SecurityElement permissionSetXml2 = SecurityElement.FromString(permissionSetXml);
				NamedPermissionSet namedPermissionSet = new NamedPermissionSet(permissionSetXml2);
				PermissionSet permissionSetExtensions = BuiltInPermissionSets.GetPermissionSetExtensions(extensionXml);
				namedPermissionSet.InplaceUnion(permissionSetExtensions);
				permissionSet = namedPermissionSet;
			}
			return permissionSet.Copy() as NamedPermissionSet;
		}

		private static NamedPermissionSet GetOrDeserializePermissionSet(ref NamedPermissionSet permissionSet, string permissionSetXml)
		{
			if (permissionSet == null)
			{
				SecurityElement permissionSetXml2 = SecurityElement.FromString(permissionSetXml);
				NamedPermissionSet namedPermissionSet = new NamedPermissionSet(permissionSetXml2);
				permissionSet = namedPermissionSet;
			}
			return permissionSet.Copy() as NamedPermissionSet;
		}

		private static PermissionSet GetPermissionSetExtensions(string extensionXml)
		{
			SecurityElement securityElement = SecurityElement.FromString(extensionXml);
			SecurityElement el = (SecurityElement)securityElement.Children[0];
			if (XMLUtil.GetClassFromElement(el, true) != null)
			{
				return new NamedPermissionSet(securityElement);
			}
			return null;
		}

		private static readonly string s_everythingXml = "<PermissionSet class = \"System.Security.NamedPermissionSet\"\r\n                             version = \"1\"\r\n                             Name = \"Everything\"\r\n                             Description = \"" + Environment.GetResourceString("Policy_PS_Everything") + "\"\r\n                  <IPermission class = \"System.Data.OleDb.OleDbPermission, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Data.SqlClient.SqlClientPermission, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Diagnostics.PerformanceCounterPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Net.DnsPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Net.SocketPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Net.WebPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.DataProtectionPermission, System.Security, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Diagnostics.EventLogPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.FileDialogPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.FileIOPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" /> \r\n                  <IPermission class = \"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.KeyContainerPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Drawing.Printing.PrintingPermission, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.ReflectionPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.RegistryPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.SecurityPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Flags = \"Assertion, UnmanagedCode, Execution, ControlThread, ControlEvidence, ControlPolicy, ControlAppDomain, SerializationFormatter, ControlDomainPolicy, ControlPrincipal, RemotingConfiguration, Infrastructure, BindingRedirects\" />\r\n                  <IPermission class = \"System.Security.Permissions.UIPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.StorePermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.TypeDescriptorPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n               </PermissionSet>";

		private static readonly string s_executionXml = "<PermissionSet class = \"System.Security.NamedPermissionSet\"\r\n                             version = \"1\"\r\n                             Name = \"Execution\"\r\n                             Description = \"" + Environment.GetResourceString("Policy_PS_Execution") + "\">\r\n                  <IPermission class = \"System.Security.Permissions.SecurityPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Flags = \"Execution\" />\r\n               </PermissionSet>";

		private static readonly string s_fullTrustXml = "<PermissionSet class = \"System.Security.NamedPermissionSet\" \r\n                             version = \"1\" \r\n                             Unrestricted = \"true\" \r\n                             Name = \"FullTrust\" \r\n                             Description = \"" + Environment.GetResourceString("Policy_PS_FullTrust") + "\" />";

		private static readonly string s_internetXml = "<PermissionSet class = \"System.Security.NamedPermissionSet\"\r\n                             version = \"1\"\r\n                             Name = \"Internet\"\r\n                             Description = \"" + Environment.GetResourceString("Policy_PS_Internet") + "\">\r\n                  <IPermission class = \"System.Drawing.Printing.PrintingPermission, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n                               version = \"1\"\r\n                               Level = \"SafePrinting\" />\r\n                  <IPermission class = \"System.Security.Permissions.FileDialogPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Access = \"Open\" />\r\n                  <IPermission class = \"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               UserQuota = \"1024000\"\r\n                               Allowed = \"ApplicationIsolationByUser\" />\r\n                  <IPermission class = \"System.Security.Permissions.SecurityPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Flags = \"Execution\" />\r\n                  <IPermission class = \"System.Security.Permissions.UIPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Window = \"SafeTopLevelWindows\"\r\n                               Clipboard = \"OwnClipboard\" />\r\n               </PermissionSet>";

		private static readonly string s_localIntranetXml = "<PermissionSet class = \"System.Security.NamedPermissionSet\"\r\n                             version = \"1\"\r\n                             Name = \"LocalIntranet\"\r\n                             Description = \"" + Environment.GetResourceString("Policy_PS_LocalIntranet") + "\" >\r\n                  <IPermission class = \"System.Drawing.Printing.PrintingPermission, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n                              version = \"1\"\r\n                              Level = \"DefaultPrinting\" />\r\n                  <IPermission class = \"System.Net.DnsPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.EnvironmentPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Read = \"USERNAME\" />\r\n                  <IPermission class = \"System.Security.Permissions.FileDialogPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Allowed = \"AssemblyIsolationByUser\"\r\n                               UserQuota = \"9223372036854775807\"\r\n                               Expiry = \"9223372036854775807\"\r\n                               Permanent = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.ReflectionPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Flags = \"ReflectionEmit, RestrictedMemberAccess\" />\r\n                  <IPermission class = \"System.Security.Permissions.SecurityPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Flags = \"Execution, Assertion, BindingRedirects \" />\r\n                  <IPermission class = \"System.Security.Permissions.TypeDescriptorPermission, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Flags = \"RestrictedRegistrationAccess\" />\r\n                  <IPermission class = \"System.Security.Permissions.UIPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n               </PermissionSet>";

		private static readonly string s_nothingXml = "<PermissionSet class = \"System.Security.NamedPermissionSet\"\r\n                             version = \"1\"\r\n                             Name = \"Nothing\"\r\n                             Description = \"" + Environment.GetResourceString("Policy_PS_Nothing") + "\" />";

		private static readonly string s_skipVerificationXml = "<PermissionSet class = \"System.Security.NamedPermissionSet\"\r\n                             version = \"1\"\r\n                             Name = \"SkipVerification\"\r\n                             Description = \"" + Environment.GetResourceString("Policy_PS_SkipVerification") + "\">\r\n                  <IPermission class = \"System.Security.Permissions.SecurityPermission, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n                               version = \"1\"\r\n                               Flags = \"SkipVerification\" />\r\n               </PermissionSet>";

		private const string s_wpfExtensionXml = "<PermissionSet class = \"System.Security.PermissionSet\"\r\n                             version = \"1\">\r\n                  <IPermission class = \"System.Security.Permissions.MediaPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Audio=\"SafeAudio\" Video=\"SafeVideo\" Image=\"SafeImage\" />\r\n                  <IPermission class = \"System.Security.Permissions.WebBrowserPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Level=\"Safe\" />\r\n              </PermissionSet>";

		private const string s_wpfExtensionUnrestrictedXml = "<PermissionSet class = \"System.Security.PermissionSet\"\r\n                             version = \"1\">\r\n                  <IPermission class = \"System.Security.Permissions.MediaPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n                  <IPermission class = \"System.Security.Permissions.WebBrowserPermission, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\"\r\n                               version = \"1\"\r\n                               Unrestricted = \"true\" />\r\n              </PermissionSet>";

		private static NamedPermissionSet s_everything;

		private static NamedPermissionSet s_execution;

		private static NamedPermissionSet s_fullTrust;

		private static NamedPermissionSet s_internet;

		private static NamedPermissionSet s_localIntranet;

		private static NamedPermissionSet s_nothing;

		private static NamedPermissionSet s_skipVerification;
	}
}
