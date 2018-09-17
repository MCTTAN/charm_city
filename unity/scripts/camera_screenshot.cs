/*

When running this code in Unity, press Shift + C to take a screenshot.

Pictures will be saved in your project's folder.

*/

using UnityEngine;
using System;

public class camera_screenshot : MonoBehaviour
{
  public KeyCode[] screenshot_keys;
    public KeyCode[] key_modifiers;

    public int min_width = 1684;
    public int min_height = 2384;
    public string directory = "./";

    public Camera target_camera;
    public float view_port_width = 1.0f;
    public float view_port_height = 1.0f;

    public enum DepthBuffer { DEPTH_0 = 0, DEPTH_16 = 16, DEPTH_24 = 24 };
    public DepthBuffer depthBuffer = DepthBuffer.DEPTH_24;

    public enum Format { PNG, JPG };
    public Format format = Format.PNG;

    void Reset()
    {
        screenshot_keys = new KeyCode[] { KeyCode.C };
        key_modifiers = new KeyCode[] { KeyCode.LeftShift, KeyCode.RightShift };
    }

    void Update()
    {
        if (key_modifiers.Length > 0)
        {
            bool is_modifier_pressed = false;
            foreach (KeyCode keyCode in key_modifiers)
            {
                if (Input.GetKey(keyCode))
                {
                    is_modifier_pressed = true;
                    break;
                }
            }
            if (!is_modifier_pressed) { return; }
        }

        foreach (KeyCode keyCode in screenshot_keys)
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (target_camera != null)
                {
                    take_screenshot_with_camera();
                }
                else
                {
                    Debug.Log("target camera was not specified.", this);
                }
            }
        }
    }

    public void take_screenshot()
    {
        float rw = (float)min_width / Screen.width;
        float rh = (float)min_height / Screen.height;
        int scale = (int)Mathf.Ceil(Mathf.Max(rw, rh));

        string path = directory + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png"; // jpeg was not supported

        ScreenCapture.CaptureScreenshot(path, scale);
        Debug.Log(string.Format("screen shot : path = {0}, scale = {1} (screen = {2}, {3})",
            path, scale, Screen.width, Screen.height), this);
    }

    public void take_screenshot_with_camera()
    {
        int vw = (int)(Screen.width * view_port_width);
        int vh = (int)(Screen.height * view_port_height);

        float rw = (float)min_width / vw;
        float rh = (float)min_height / vh;
        float scale = Mathf.Max(rw, rh);

        int tw = (int)Mathf.Ceil(vw * scale);
        int th = (int)Mathf.Ceil(vh * scale);
        RenderTexture renderTexture = RenderTexture.GetTemporary(tw, th, (int)depthBuffer, RenderTextureFormat.ARGB32);
        Texture2D texture = new Texture2D(tw, th, TextureFormat.ARGB32, false);

        RenderTexture oldTargetTexture = target_camera.targetTexture;
        RenderTexture oldActiveTexture = RenderTexture.active;

        target_camera.targetTexture = renderTexture;
        target_camera.Render();

        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, tw, th), 0, 0);
        texture.Apply();

        RenderTexture.active = oldActiveTexture;
        target_camera.targetTexture = oldTargetTexture;

        RenderTexture.ReleaseTemporary(renderTexture);

        string path = directory + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + (format == Format.PNG ? ".png" : ".jpg");

        System.IO.File.WriteAllBytes(path, texture.EncodeToJPG());
        Debug.Log(string.Format("screen shot with camera : path = {0}, scale = {1:F2} (view = {2}, {3})",
            path, scale, vw, vh), this);
    }
}
