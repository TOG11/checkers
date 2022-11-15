using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject license_panel;
    public GameObject options_panel;

    public void
    open_git_link()
    {
        Application.OpenURL("https://github.com/TOG11/checkers");
    }

    public void
    exit()
    {
        Application.Quit();
    }

    public void
    show_options_page()
    {
            options_panel.SetActive(!options_panel.activeInHierarchy);
    }

    public void
    play()
    {

    }

    public void
    show_license()
    {
        license_panel.SetActive(license_panel.activeInHierarchy);
    }
}
