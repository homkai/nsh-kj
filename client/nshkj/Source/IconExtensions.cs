using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace nshkj
{
	public static class IconExtensions {
		/// <summary>
		/// Retrieves an icon of the specified size from the embedded assembly resources.
		/// If no such icon is found, an ArgumentException will be thrown.
		/// </summary>
		/// <param name="name">Resource name without including assembly name or extension.</param>
		/// <param name="size">Icon size.</param>
		/// <returns>The Icon on success. Otherwise, an exception will be thrown.</returns>
		public static Icon ExtractFromAssembly(string name, Size size) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			string[] names = assembly.GetManifestResourceNames();

			foreach (string resourceName in names) {
				if (resourceName.EndsWith("." + name + ".ico")) {
					using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
						return new Icon(stream, size);
					}
				}
			}

			throw new ArgumentException("No icons with the specified name have been found.");
		}
	}
}
