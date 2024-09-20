using UnityEngine;
using System;


public class DialogWithScroll : MonoBehaviour {

    Rect m_windowRect;
    Action m_actionOK;
    string m_title;
    string m_preScrollText;
    string m_msg;
	string m_buttonOK;
	static int m_width = 300;
	static int m_height = 100;
	static int m_posX;
	static int m_posY;
    static int m_scrollHeight;
    Vector2 scrollPosition = Vector2.zero;

	static public void MessageBox(string tag, string title, string msg, string button_ok, Action action_ok, string preScrollText = "", int pos_x = int.MaxValue, int pos_y = int.MaxValue, int widthMax = 0, int heightMax = 0, int scrollHeight=200)
    {
        GameObject go = new("DialogWithScroll")
        {
            tag = tag
        };
        DialogWithScroll dlg = go.AddComponent<DialogWithScroll>();

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
               

        dlg.Init(title, preScrollText, msg, button_ok, action_ok, pos_x_int, pos_y_int, width, height, scrollHeight);
    }

	void Init(string title, string preScrollText, string msg, string buttonOK, Action actionOK, int posX, int posY, int width, int height, int scrollHeight)
    {
        m_title = title;
        m_preScrollText = preScrollText;
        m_msg = msg;
		m_buttonOK = buttonOK;
        m_actionOK = actionOK;
		m_width = width;
		m_height = height;
        m_scrollHeight = scrollHeight;
		m_posX = posX;
		m_posY = posY;
    }

    void OnGUI()
    {
		m_windowRect = new Rect(
            m_posX,
            m_posY,
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
        const int preScrollHeight = 50;
        const int scrollbar = 20;

        Rect preScroll = new Rect(
            border,
            border + spacing,
            m_windowRect.width - border * 2,
            preScrollHeight);

        GUI.Label(preScroll, m_preScrollText);

        Rect scrollMask = new Rect(
            border,
            border + spacing + preScrollHeight,
            m_windowRect.width - border * 2,
            m_windowRect.height - border * 2 - height - spacing - preScrollHeight - scrollbar);

        Rect scrollView = new Rect(
            scrollMask.x,
            scrollMask.y,
            scrollMask.width - scrollbar,
            scrollMask.height + m_scrollHeight);        

        scrollPosition = GUI.BeginScrollView(scrollMask, scrollPosition, scrollView);
        //Parameters
        //Position - Position and size to draw the scroll view
        //scrollPosition - Variable that will hold the current scroll location (X.Y)
        //viewRect - Area Inside the scroll view - should always be bigger than the size in Position

        
        GUI.Label(scrollView, m_msg);

        GUI.EndScrollView();

        Rect buttonOK = new Rect(
            m_windowRect.width - width - border,
            m_windowRect.height - height - border,
            //border,
            width,
            height);

        if (GUI.Button(buttonOK, m_buttonOK))
        {
            Destroy(this.gameObject);
            m_actionOK();
        }
    }
}
