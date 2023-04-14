using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class CameraExtensions {
		public static Sprite TakeSnapshot (this Camera self, int width, int height) {
			bool enabled = self.enabled;
			self.enabled = true;

			Texture2D     texture       = new Texture2D(width, height, TextureFormat.ARGB32, false);
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);

			self.targetTexture = renderTexture;
			self.Render();
			self.targetTexture = null;

			RenderTexture.active = renderTexture;
			texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			texture.Apply();
			RenderTexture.active = null;

			RenderTexture.ReleaseTemporary(renderTexture);

			self.enabled = enabled;

			Rect   rect     = new Rect(0, 0, texture.width, texture.height);
			Sprite snapshot = Sprite.Create(texture, rect, Vector2.zero);

			return snapshot;
		}
	}
}
