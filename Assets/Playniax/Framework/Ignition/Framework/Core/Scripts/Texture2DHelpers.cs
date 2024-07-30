using UnityEngine;

namespace Playniax.Ignition
{

    // Collection of Texture2D functions.
    public class Texture2DHelpers
    {
        // Blits texture on top of another one at a specified position.
        public static Texture2D Blit(Texture2D overlayTexture, Texture2D targetTexture, Vector2 position)
        {
            int destX = Mathf.FloorToInt(position.x);
            int destY = Mathf.FloorToInt(targetTexture.height - position.y - overlayTexture.height);

            Color32[] sourcePixels = overlayTexture.GetPixels32();
            Color32[] destPixels = targetTexture.GetPixels32();

            int sourceWidth = overlayTexture.width;
            int sourceHeight = overlayTexture.height;
            int destWidth = targetTexture.width;
            int destHeight = targetTexture.height;

            int startX = Mathf.Max(destX, 0);
            int startY = Mathf.Max(destY, 0);
            int endX = Mathf.Min(destX + sourceWidth, destWidth);
            int endY = Mathf.Min(destY + sourceHeight, destHeight);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    int destIndex = y * destWidth + x;
                    int sourceIndex = (y - destY) * sourceWidth + (x - destX);

                    Color32 sourcePixel = sourcePixels[sourceIndex];
                    Color32 destPixel = destPixels[destIndex];

                    float alpha = sourcePixel.a / 255f;
                    float oneMinusAlpha = 1 - alpha;

                    destPixels[destIndex] = new Color32(
                        (byte)(sourcePixel.r * alpha + destPixel.r * oneMinusAlpha),
                        (byte)(sourcePixel.g * alpha + destPixel.g * oneMinusAlpha),
                        (byte)(sourcePixel.b * alpha + destPixel.b * oneMinusAlpha),
                        (byte)(255 * (alpha + destPixel.a / 255f * oneMinusAlpha))
                    );
                }
            }

            Texture2D result = new Texture2D(destWidth, destHeight);
            result.SetPixels32(destPixels);
            result.Apply();

            return result;
        }

        // Returns the dominant color of a texture by averaging the RGB values of all pixels in the texture.
        public static Color32 GetDominantColor(Texture2D texture)
        {
            Color32[] colors = texture.GetPixels32();

            int count = colors.Length;

            float r = 0f;
            float g = 0f;
            float b = 0f;

            for (int i = 0; i < count; i++)
            {
                r += colors[i].r;
                g += colors[i].g;
                b += colors[i].b;
            }

            return new Color32((byte)(r / count), (byte)(g / count), (byte)(b / count), 255);
        }

        // Returns a clone of the texture with the new width and height.
        public static Texture2D ScaledClone(Texture2D texture, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new(0, 0, width, height);
            _Scale(texture, width, height, mode);

            Texture2D output = new(width, height, TextureFormat.ARGB32, true);
            output.Reinitialize(width, height);
            output.ReadPixels(texR, 0, 0, true);
            output.Apply(true);

            return output;
        }

        // Resizes the texture to the new width and height.
        public static void Scale(Texture2D texture, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new(0, 0, width, height);
            _Scale(texture, width, height, mode);

            texture.Reinitialize(width, height);
            texture.ReadPixels(texR, 0, 0, true);
            texture.Apply(true);
        }

        // Returns a screenshot.
        public static Texture2D GetScreenshot()
        {
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            return screenshot;
        }

        // Returns a sprite.
        public static Sprite TextureToSprite(Texture2D texture, Vector2 pivot, float pixelsPerUnit = 100)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), pivot, pixelsPerUnit);

            return sprite;
        }

        static void _Scale(Texture2D src, int width, int height, FilterMode fmode)
        {
            src.filterMode = fmode;
            src.Apply(true);
            RenderTexture rtt = new(width, height, 32);
            Graphics.SetRenderTarget(rtt);
            GL.LoadPixelMatrix(0, 1, 1, 0);
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
        }
    }
}