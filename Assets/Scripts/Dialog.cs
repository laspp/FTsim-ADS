using UnityEngine;
using System;

// example:
// Dialog.MessageBox("error", "Sorry but you're S.O.L", () => { Application.Quit() });

public class Dialog : MonoBehaviour {

    Rect m_windowRect;
    Action m_action_ok;
    string m_title;
    string m_msg;
	string m_button_ok;
	static int m_width = 300;
	static int m_height = 100;
	static int m_pos_x;
	static int m_pos_y;

	static public void MessageBox(string tag, string title, string msg, string button_ok, Action action_ok, int pos_x = int.MaxValue, int pos_y = int.MaxValue, int widthMax = 0, int heightMax = 0)
    {
        GameObject go = new("Dialog")
        {
            tag = tag
        };
        Dialog dlg = go.AddComponent<Dialog>();

		int maxWidth = m_width;
		int maxHeight = m_height;
		if (widthMax != 0) {
			maxWidth = (int) widthMax;
		}
		if (heightMax != 0) {
			maxHeight = (int) heightMax;
		}
		int width = (int) Mathf.Min((float) maxWidth, (float) Screen.width - 20);
		int height = (int) Mathf.Min((float) maxHeight, (float)Screen.height - 20);

		if (pos_x == int.MaxValue) {
			pos_x = (Screen.width - width) / 2;
		}
		if (pos_y == int.MaxValue) {
			pos_y = (Screen.height - height) / 2;
		}
		int pos_x_int = (int) pos_x;
		int pos_y_int = (int) pos_y;

		dlg.Init(title, msg, button_ok, action_ok, pos_x_int, pos_y_int, width, height);
    }

	void Init(string title, string msg, string button_ok, Action action_ok, int pos_x, int pos_y, int width, int height)
    {
        m_title = title;
        m_msg = msg;
		m_button_ok = button_ok;
        m_action_ok = action_ok;
		m_width = width;
		m_height = height;
		m_pos_x = pos_x;
		m_pos_y = pos_y;
    }

    void OnGUI()
    {
		m_windowRect = new Rect(
            m_pos_x,
            m_pos_y,
            m_width,
            m_height);

        m_windowRect = GUI.Window(0, m_windowRect, WindowFunc, m_title);
    }

    void WindowFunc(int windowID)
    {
        const int border = 10;
        const int width = 50;
        const int height = 25;
        const int spacing = 10;

        Rect content = new Rect(
            border,
            border + spacing,
            m_windowRect.width - border * 2,
            m_windowRect.height - border * 2 - height - spacing);

                
        GUI.Label(content, m_msg);

        Rect buttonOK = new Rect(
            m_windowRect.width - width - border,
            m_windowRect.height - height - border,
            width,
            height);

        if (GUI.Button(buttonOK, m_button_ok))
        {
            Destroy(this.gameObject);
            m_action_ok();
        }


    }
}
