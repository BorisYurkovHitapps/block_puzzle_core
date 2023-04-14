using Newtonsoft.Json;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class ObjectExtensions {
		public static string ToJson (this object self) {
			return JsonConvert.SerializeObject(self, Formatting.Indented);
		}

		public static T FromJson<T> (this string json) {
			return JsonConvert.DeserializeObject <T>(json);
		}

		public static void FromJson (this object self, string json) {
			JsonConvert.PopulateObject(json, self);
		}
	}
}
