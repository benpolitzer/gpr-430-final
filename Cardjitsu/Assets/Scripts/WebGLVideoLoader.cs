using UnityEngine;
using UnityEngine.Video;

public class WebGLVideoLoader : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "clubpenguindance.mp4");

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }
}