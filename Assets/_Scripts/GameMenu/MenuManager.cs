using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager {

    static MenuManager msMenuManager = null;

    public static MenuManager GetInstance() {
        if (msMenuManager == null) {
            msMenuManager = new MenuManager();
        }
        return msMenuManager;
    }




}
