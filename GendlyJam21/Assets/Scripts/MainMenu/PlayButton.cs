using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CGDP2.UtilityComponents;

public class PlayButton : ButtonEventListener
{
    public override void OnClick()
    {
        SceneManager.LoadScene(1);
    }
}
