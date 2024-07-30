using System.Collections;
using System.IO;

#if UNITY_ANDROID || UNITY_IOS
using NativeShareNamespace;
#endif

using UnityEngine;

using Playniax.Ignition;

#pragma warning disable 0414

// SimpleShare settings for sharing a screenshot or banner for organic marketing.
public class SimpleShare : MonoBehaviour
{
    const string TEMP_FILE = "screenshot.png";
    public enum Mode
    {
        Screenshot, // Capture screenshot mode
        Banner // Banner mode
    };

    [Tooltip("The mode for the application: Screenshot or Banner.")]
    public Mode mode;

    [Tooltip("The subject of the message.")]
    public string subject = "My Game";

    [Tooltip("The text content of the message.")]
    public string text = "www.playniax.com";

    [Tooltip("Whether to add a screenshot logo.")]
    public bool addScreenshotLogo = true;

    [Tooltip("The logo to use in the screenshot.")]
    public Texture2D screenshotLogo;

    [Tooltip("The position of the screenshot logo.")]
    public Vector2 screenshotLogoPosition;

    [Tooltip("The image to use as banner.")]
    public Texture2D bannerImage;

    [Tooltip("Whether to allow sharing.")]
    public bool allow = true;

#if UNITY_EDITOR
    [Tooltip("Whether to display the result in Unity Editor Mode.")]
    public bool showResultInEditor = true;
#endif

    public void OnScreenshot()
    {
        if (allow == false) return;

        switch (mode)
        {
            case Mode.Banner:

                if (bannerImage) _Banner();

                break;

            case Mode.Screenshot:

                _Screenshot();

                break;
        }
    }

    public void Share()
    {
        if (allow) StartCoroutine(_Share());
    }

    void _Banner()
    {
        string path = Path.Combine(Application.temporaryCachePath, TEMP_FILE);

        File.WriteAllBytes(path, bannerImage.EncodeToPNG());
    }

    void _Screenshot()
    {
        string path = Path.Combine(Application.temporaryCachePath, TEMP_FILE);

        if (addScreenshotLogo && screenshotLogo)
        {
            Texture2D screenshot = Texture2DHelpers.GetScreenshot();

            screenshot = Texture2DHelpers.Blit(screenshotLogo, screenshot, screenshotLogoPosition);

            File.WriteAllBytes(path, screenshot.EncodeToPNG());

            Destroy(screenshot);
        }
        else
        {
            Texture2D screenshot = Texture2DHelpers.GetScreenshot();

            File.WriteAllBytes(path, screenshot.EncodeToPNG());

            Destroy(screenshot);
        }
    }

    IEnumerator _Share()
    {
        yield return new WaitForEndOfFrame();

        string path = Path.Combine(Application.temporaryCachePath, TEMP_FILE);

#if UNITY_EDITOR
        if (showResultInEditor) Application.OpenURL(path);
#endif

        new NativeShare()
            .AddFile(path)
            .SetSubject(subject)
            .SetText(text)
            .Share();
    }
}

