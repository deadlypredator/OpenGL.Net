﻿
// Copyright (C) 2017 Luca Piccioni
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Mono.Collections.Generic;
using Mono.Cecil;

using OpenGL;

namespace BindingsGen
{
	/// <summary>
	/// Class for stripping symbols from OpenGL.Net assembly.
	/// </summary>
	class RegistryAssembly
	{
		public static void CleanAssembly(string path, RegistryAssemblyConfiguration cfg)
		{
			Console.WriteLine("*** Generate assembly {0}", cfg.Name);

			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(path);

			#region Enums

			foreach (ModuleDefinition module in assembly.Modules) {
				List<TypeDefinition> removedTypes = new List<TypeDefinition>();

				foreach (TypeDefinition type in module.Types) {
					if (type.IsEnum == false)
						continue;

					// Remove fields
					List<FieldDefinition> removedFields = new List<FieldDefinition>();

					foreach (FieldDefinition field in type.Fields) {
						if (IsCompatibleField(cfg, field) == false)
							removedFields.Add(field);
					}

					// Console.WriteLine("- Removing {0} constants", removedFields.Count);
					foreach (FieldDefinition field in removedFields) {
						// Console.WriteLine("  - {0}", field.Name);
						type.Fields.Remove(field);
					}

					if (type.Fields.Count == 1)
						removedTypes.Add(type);
				}

				Console.WriteLine("- Removing {0} enum types", removedTypes.Count);
				foreach (TypeDefinition type in removedTypes) {
					// Console.WriteLine("  - {0}", field.Name);
					module.Types.Remove(type);
				}
			}

			#endregion

			#region KhronosApi (Gl)

			foreach (ModuleDefinition module in assembly.Modules) {
				foreach (TypeDefinition type in module.Types) {
					if (type.BaseType == null || type.BaseType.Name != "KhronosApi")
						continue;
					if (type.Name != "Gl")
						continue;

					CleanTypeDefinition(type, cfg);

					TypeDefinition vbNestedType = type.NestedTypes.GetNestedType("VB");
					if (vbNestedType != null)
						CleanTypeDefinition(vbNestedType, cfg);
				}
			}

			#endregion

			// Export
			string baseDirPath = Path.GetDirectoryName(path);

			assembly.Write(Path.Combine(baseDirPath, String.Format("OpenGL.Net-{0}.dll", cfg.Name)));
		}

		private static void CleanTypeDefinition(TypeDefinition type, RegistryAssemblyConfiguration cfg)
		{
			// Remove fields
			List<FieldDefinition> removedFields = new List<FieldDefinition>();

			foreach (FieldDefinition field in type.Fields) {
				if (IsCompatibleField(cfg, field) == false) {
					// Console.WriteLine("    - Remove {0}", field.Name);
					removedFields.Add(field);
				}
					
			}

			if (removedFields.Count > 0) {
				Console.WriteLine("- Removed {0} constants from {1}", removedFields.Count, type.FullName);
				foreach (FieldDefinition field in removedFields) {
					type.Fields.Remove(field);
				}
			}

			// Remove methods
			List<MethodDefinition> removedMethods = new List<MethodDefinition>();

			foreach (MethodDefinition method in type.Methods) {
				if (IsCompatibleMethod(cfg, method) == false) {
					// Console.WriteLine("    - Remove {0}", method.ToString());
					removedMethods.Add(method);
				}
			}

			if (removedMethods.Count > 0) {
				Console.WriteLine("- Removing {0} methods from {1}", removedMethods.Count, type.FullName);
				foreach (MethodDefinition method in removedMethods)
					RemoveMethod(type, method);
			}
		}

		private static void RemoveMethod(TypeDefinition type, MethodDefinition method)
		{
			string methodName = type.Name.ToLower() + method.Name;

			// Remove method
			type.Methods.Remove(method);

			// Unsafe methods
			List<MethodDefinition> removedUnsafeMethods = new List<MethodDefinition>();

			TypeDefinition unsafeNestedType = type.NestedTypes.GetNestedType("UnsafeNativeMethods");
			if (unsafeNestedType != null) {
				foreach (MethodDefinition unsafeMethod in unsafeNestedType.Methods) {
					if (unsafeMethod.Name == methodName) {
						removedUnsafeMethods.Add(unsafeMethod);
						break;
					}
				}
				foreach (MethodDefinition unsafeMethod in removedUnsafeMethods)
					unsafeNestedType.Methods.Remove(unsafeMethod);
			}

			// Delegates
			List<FieldDefinition> removedDelegateFields = new List<FieldDefinition>();
			List<TypeDefinition> removedDelegateTypes = new List<TypeDefinition>();

			TypeDefinition delegatesNestedType = type.NestedTypes.GetNestedType("Delegates");
			if (delegatesNestedType != null) {
				foreach (FieldDefinition field in delegatesNestedType.Fields) {
					if (field.Name == "p" + methodName) {
						removedDelegateFields.Add(field);
						break;
					}
				}

				foreach (TypeDefinition nestedType in delegatesNestedType.NestedTypes) {
					if (nestedType.Name == methodName) {
						removedDelegateTypes.Add(nestedType);
						break;
					}
				}

				foreach (FieldDefinition field in removedDelegateFields)
					unsafeNestedType.Fields.Remove(field);
				foreach (TypeDefinition nestedType in removedDelegateTypes)
					unsafeNestedType.NestedTypes.Remove(nestedType);
			}
		}

		private static bool IsCompatibleField(RegistryAssemblyConfiguration cfg, FieldDefinition field)
		{
			bool compatible = false;
			int featureAttribCount = 0;

			foreach (CustomAttribute customAttrib in field.CustomAttributes){
				switch (customAttrib.AttributeType.Name) {
					case "RequiredByFeatureAttribute":
						compatible |= IsCompatible_RequiredAttribute(cfg, customAttrib);
						featureAttribCount++;
						break;
					case "RemovedByFeatureAttribute":
						if (IsCompatible_RemovedAttribute(cfg, customAttrib) == false)
							return (false);
						break;
				}
			}

			return (compatible || featureAttribCount == 0);
		}

		private static bool IsCompatibleMethod(RegistryAssemblyConfiguration cfg, MethodDefinition method)
		{
			// Methods required by OpenGL.Net
			// Note: code should test actual method existence, since  they may not loaded for specific profiles
			switch (method.Name) {
				case "GetString":
					return (true);
			}

			bool compatible = false;
			int featureAttribCount = 0;

			foreach (CustomAttribute customAttrib in method.CustomAttributes) {
				switch (customAttrib.AttributeType.Name) {
					case "RequiredByFeatureAttribute":
						compatible |= IsCompatible_RequiredAttribute(cfg, customAttrib);
						featureAttribCount++;
						break;
					case "RemovedByFeatureAttribute":
						if (IsCompatible_RemovedAttribute(cfg, customAttrib) == false)
							return (false);
						featureAttribCount++;
						break;
				}
			}

			return (compatible || featureAttribCount == 0);
		}

		private static bool IsCompatible_RequiredAttribute(RegistryAssemblyConfiguration cfg, CustomAttribute customAttrib)
		{
			string featureName = customAttrib.ConstructorArguments[0].Value as string;
			KhronosVersion attribVersion = KhronosVersion.ParseFeature(featureName, false);

			// Match at least one feature?
			bool compatible = false;

			foreach (RegistryAssemblyConfiguration.VersionRange cfgFeature in cfg.Features) {
				if (cfgFeature.Api != null) {
					CustomAttributeNamedArgument apiArg = customAttrib.Fields.FirstOrDefault(delegate(CustomAttributeNamedArgument item) { return (item.Name == "Api"); });
					string apiRegex = apiArg.Argument.Value != null ? apiArg.Argument.Value as string : "gl";

					if (Regex.IsMatch(cfgFeature.Api, "^(" + apiRegex + ")$"))
						compatible |= true;
				}

				if (cfgFeature.Profile != null) {
					CustomAttributeNamedArgument apiArg = customAttrib.Fields.FirstOrDefault(delegate(CustomAttributeNamedArgument item) { return (item.Name == "Profile"); });
					string apiRegex = apiArg.Argument.Value != null ? apiArg.Argument.Value as string : null;

					if (apiRegex != null)
						compatible &= Regex.IsMatch(cfgFeature.Profile, "^(" + apiRegex + ")$");
				}

				if (attribVersion != null) {
					KhronosVersion minVersion = /*cfgFeature.MinApiVersion != null ? KhronosVersion.Parse(cfgFeature.MinApiVersion) :*/ null;
					KhronosVersion maxVersion = /*cfgFeature.MaxApiVersion != null ? KhronosVersion.Parse(cfgFeature.MaxApiVersion) :*/ null;
					// API version
					if (minVersion != null && attribVersion < minVersion)
						return (false);
					if (maxVersion != null && attribVersion > maxVersion)
						return (false);
				} else {
					// Extension (Api must match)
					if (compatible)
						compatible &= cfg.Extensions.Contains(featureName);
				}
			}

			return (compatible);
		}

		private static bool IsCompatible_RemovedAttribute(RegistryAssemblyConfiguration cfg, CustomAttribute customAttrib)
		{
			string featureName = customAttrib.ConstructorArguments[0].Value as string;
			KhronosVersion attribVersion = KhronosVersion.ParseFeature(featureName, false);

			if (attribVersion != null) {
				foreach (RegistryAssemblyConfiguration.VersionRange cfgFeature in cfg.Features) {
					if (cfgFeature.Api != null && cfgFeature.Api == attribVersion.Api)
						return (false);
				}
			}

			return (true);
		}
	}

	static class TypeDefinitionExtensions
	{
		public static TypeDefinition GetNestedType(this Collection<TypeDefinition> typeCollection, string name)
		{
			foreach (TypeDefinition nestedType in typeCollection) {
				if (nestedType.Name == name)
					return (nestedType);
			}

			return (null);
		}
	}
}
