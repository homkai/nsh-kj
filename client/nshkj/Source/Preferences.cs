using Newtonsoft.Json;
using System;
using System.IO;

namespace nshkj
{
	public class Preferences {
        public string CuptureHotKey
        {
            get;
            set;
        } = "A:Q"; // 默认 alt + q
        

		/// <summary>
		/// Loads preferences
		/// </summary>
		/// <returns>The Preferences object</returns>
		public static Preferences Load() {
			try {
				Preferences defaults = new Preferences();
				Preferences prefs = JsonConvert.DeserializeObject<Preferences>(File.ReadAllText(Path.Combine(App.AppDirectory, "Preferences.json")));
				App.Logger.WriteLine(LogLevel.Verbose, "validating preferences");
                
				// try to save any possible changes
				prefs.Save();

				// return preferenecs
				return prefs;
			} catch {
				App.Logger.WriteLine(LogLevel.Warning, "could not load/deserialize user preferences - falling back to defaults");
				return new Preferences();
			}
		}

		/// <summary>
		/// Saves preferences
		/// </summary>
		/// <returns>Whether or not the operation completed successfully</returns>
		public bool Save() {
			try {
				App.Logger.WriteLine(LogLevel.Verbose, "saving preferences");
				File.WriteAllText(Path.Combine(App.AppDirectory, "Preferences.json"), JsonConvert.SerializeObject(this));
				App.Logger.WriteLine(LogLevel.Informational, "operation completed successfully");
				return true;
			} catch {
				App.Logger.WriteLine(LogLevel.Error, "could not save preferences");
				return false;
			}
		}
	}
}
