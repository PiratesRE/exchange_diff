using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Role : IInstallable
	{
		public virtual bool IsUnpacked
		{
			get
			{
				Version unpackedVersion = RolesUtility.GetUnpackedVersion(this.RoleName);
				return !(unpackedVersion == null);
			}
		}

		public virtual Version UnpackedVersion
		{
			get
			{
				Version unpackedVersion = RolesUtility.GetUnpackedVersion(this.RoleName);
				if (unpackedVersion == null)
				{
					return new Version(0, 0, 0, 0);
				}
				return unpackedVersion;
			}
		}

		public virtual bool IsDatacenterUnpacked
		{
			get
			{
				Version unpackedDatacenterVersion = RolesUtility.GetUnpackedDatacenterVersion(this.RoleName);
				return !(unpackedDatacenterVersion == null);
			}
		}

		public virtual Version UnpackedDatacenterVersion
		{
			get
			{
				Version unpackedDatacenterVersion = RolesUtility.GetUnpackedDatacenterVersion(this.RoleName);
				if (unpackedDatacenterVersion == null)
				{
					return new Version(0, 0, 0, 0);
				}
				return unpackedDatacenterVersion;
			}
		}

		public virtual bool IsInstalled
		{
			get
			{
				Version v = Role.GetRoleConfiguredVersion(this.RoleName);
				return !(v == null);
			}
		}

		public virtual Version InstalledVersion
		{
			get
			{
				Version configuredVersion = RolesUtility.GetConfiguredVersion(this.RoleName);
				if (configuredVersion == null)
				{
					return new Version(0, 0, 0, 0);
				}
				return configuredVersion;
			}
		}

		public virtual bool IsPartiallyInstalled
		{
			get
			{
				ConfigurationStatus configurationStatus = new ConfigurationStatus(this.RoleName);
				RolesUtility.GetConfiguringStatus(ref configurationStatus);
				return (configurationStatus.Action != InstallationModes.Unknown && configurationStatus.Watermark != null) || this.IsMissingPostSetup;
			}
		}

		public virtual bool IsMissingPostSetup
		{
			get
			{
				Version unpackedVersion = RolesUtility.GetUnpackedVersion(this.RoleName);
				Version configuredVersion = RolesUtility.GetConfiguredVersion(this.RoleName);
				if (configuredVersion == null || configuredVersion != unpackedVersion)
				{
					return false;
				}
				Version postSetupVersion = RolesUtility.GetPostSetupVersion(this.RoleName);
				return postSetupVersion != null && postSetupVersion != configuredVersion;
			}
		}

		public virtual bool IsCurrent
		{
			get
			{
				return this.InstalledVersion == ConfigurationContext.Setup.InstalledVersion;
			}
		}

		public virtual bool IsDatacenterOnly
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsDatacenterDedicatedOnly
		{
			get
			{
				return false;
			}
		}

		public virtual string InstalledPath
		{
			get
			{
				if (this.IsInstalled)
				{
					return ConfigurationContext.Setup.InstallPath;
				}
				return "";
			}
		}

		public abstract ServerRole ServerRole { get; }

		public abstract Task InstallTask { get; }

		public abstract Task DisasterRecoveryTask { get; }

		public abstract Task UninstallTask { get; }

		public abstract ValidatingTask ValidateTask { get; }

		public virtual string RoleName
		{
			get
			{
				return this.roleName;
			}
		}

		internal virtual SetupComponentInfoCollection AllComponents
		{
			get
			{
				if (this.allComponents == null)
				{
					this.LoadComponentList();
				}
				return this.allComponents;
			}
		}

		internal static string SetupComponentInfoFilePath
		{
			get
			{
				if (Path.GetDirectoryName(ConfigurationContext.Setup.AssemblyPath).ToLowerInvariant() == Path.GetDirectoryName(ConfigurationContext.Setup.BinPath).ToLowerInvariant())
				{
					return ConfigurationContext.Setup.InstallPath;
				}
				return ConfigurationContext.Setup.AssemblyPath;
			}
		}

		protected void LoadComponentList()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.RoleName
			});
			string fileName = Path.Combine(ConfigurationContext.Setup.AssemblyPath, this.RoleName + "Definition.xml");
			this.allComponents = new SetupComponentInfoCollection();
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RoleDefinition.xsd");
			xmlReaderSettings.Schemas.Add(null, XmlReader.Create(manifestResourceStream));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SetupComponentInfoReferenceCollection));
			string text;
			using (XmlReader xmlReader = RolesUtility.CreateXmlReader(fileName, xmlReaderSettings, out text))
			{
				TaskLogger.Log(Strings.ReadingComponents(this.RoleName, text));
				SetupComponentInfoReferenceCollection setupComponentInfoReferenceCollection = null;
				try
				{
					setupComponentInfoReferenceCollection = (SetupComponentInfoReferenceCollection)xmlSerializer.Deserialize(xmlReader);
				}
				catch (InvalidOperationException ex)
				{
					throw new XmlDeserializationException(text, ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.Message);
				}
				TaskLogger.Log(Strings.FoundComponents(setupComponentInfoReferenceCollection.Count));
				foreach (SetupComponentInfoReference reference in setupComponentInfoReferenceCollection)
				{
					SetupComponentInfo item = this.LoadComponent(reference);
					this.allComponents.Add(item);
				}
			}
			TaskLogger.LogExit();
		}

		protected SetupComponentInfo LoadComponent(SetupComponentInfoReference reference)
		{
			TaskLogger.LogEnter(new object[]
			{
				this.RoleName,
				reference.RelativeFileLocation
			});
			string fileName = Path.Combine(Role.SetupComponentInfoFilePath, reference.RelativeFileLocation);
			SetupComponentInfo result = RolesUtility.ReadSetupComponentInfoFile(fileName);
			TaskLogger.LogExit();
			return result;
		}

		internal void Reset()
		{
			this.allComponents = null;
		}

		protected string roleName;

		internal SetupComponentInfoCollection allComponents;

		internal static Func<string, Version> GetRoleConfiguredVersion = (string currentRoleName) => RolesUtility.GetConfiguredVersion(currentRoleName);
	}
}
