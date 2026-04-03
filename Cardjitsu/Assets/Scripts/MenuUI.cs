using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private FusionBootstrap fusionBootstrap;
    [SerializeField] private TMP_InputField sessionInput;

    public void OnHostClicked()
    {
        fusionBootstrap.StartHost(sessionInput.text);
    }

    public void OnJoinClicked()
    {
        fusionBootstrap.StartClient(sessionInput.text);
    }
}