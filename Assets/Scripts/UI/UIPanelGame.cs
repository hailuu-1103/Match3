using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelGame : MonoBehaviour,IMenu
{
    public Text LevelConditionView;

    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnRestart;

    private UIMainManager m_mngr;
    private GameManager m_gameManager;

    private void Awake()
    {
        btnPause.onClick.AddListener(OnClickPause);
        this.btnRestart.onClick.AddListener(OnClickRestart);
        this.m_gameManager = FindObjectOfType<GameManager>();
    }

    private void OnClickRestart()
    {
        this.m_gameManager.Restart();
    }

    private void OnClickPause()
    {
        m_mngr.ShowPauseMenu();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
