using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] Menu[] menus;

    Menu currentActiveMenu;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        OpenMenu("LoadingMenu");
    }

    public void OpenMenu(string menuName)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == menuName)
            {
                currentActiveMenu = menu;
                menu.Open();
            }
            else
            {
                menu.Close();
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        CloseMenu(currentActiveMenu);
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
