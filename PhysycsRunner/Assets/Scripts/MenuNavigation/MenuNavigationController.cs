using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigationController {

    public MenuScreen CurrentMenuScreen { get; private set; }
    public Popup CurrentPopup { get; private set; }

    private readonly Transform _menuScreensParent;
    private readonly Transform _popupsParent;
    private readonly CanvasGroup _canvasGroup;
    private readonly Image _overlay;

    private readonly Dictionary<Type, Popup> _popups = new Dictionary<Type, Popup>();
    private readonly Dictionary<Type, MenuScreen> _menuScreens = new Dictionary<Type, MenuScreen>();

    private readonly ShowableFactory _showableFactory;

    public MenuNavigationController(ShowableFactory showableFactory, Transform menuScreensRoot, Transform popupsRoot, CanvasGroup canvasGroup) {
        _showableFactory = showableFactory;
        _menuScreensParent = menuScreensRoot;
        _popupsParent = popupsRoot;
        _canvasGroup = canvasGroup;
    }
        
    public T ShowMenuScreen<T>(Action onFinish = null) where T : MenuScreen{
        if (CurrentMenuScreen != null)
            CurrentMenuScreen.SetActive(false);
        _canvasGroup.interactable = false;
        var menuScreen = _showableFactory.Create<T>();
        menuScreen.transform.SetParent(_menuScreensParent, false);
        menuScreen.transform.SetAsLastSibling();////
        menuScreen.SetActive(true);
        _menuScreens[menuScreen.GetType()] = menuScreen;
        CurrentMenuScreen = menuScreen;
        CurrentMenuScreen.PlayShowAnimation(() => {
            _canvasGroup.interactable = true;
            onFinish?.Invoke();
        });
        return menuScreen;
    }

    public void HideMenuScreen<T>(Action onFinish = null) where T: MenuScreen {
        if (_menuScreens.Values.Count(scr => scr.IsActive) < 1 || _menuScreens.Count < 2) return;
        var screen = GetMenuScreen<T>();
        if (screen == null) {
            Debug.LogError("SCREEN DOES NOT EXIST YET");
            return;
        }
        HideMenuScreen(screen, onFinish);
    }

    public void HideMenuScreen(MenuScreen menuScreen, Action onFinish = null) {
        _canvasGroup.interactable = false;
        HideAllPopups();
        //Searching for last inactive menu screen, deactivating current screen and activating last inactive screen
        var screenToShow = _menuScreens.Values.OrderBy(e => e.RootIndex).LastOrDefault(e => !e.IsActive);
        menuScreen.PlayHideAnimation(() => {
            _canvasGroup.interactable = true;
            menuScreen.SetActive(false);
            menuScreen.transform.SetAsFirstSibling();////            
            /*
            if (screenToShow != null) {
                screenToShow.SetActive(true);
            }         
            */
            CurrentMenuScreen = screenToShow;
            onFinish?.Invoke();
        });
    }

    public T GetPopup<T>() where T : Popup {
        var type = typeof(T);
        return _popups.ContainsKey(type) ? (T)_popups[type] : null;
    }

    public T GetMenuScreen<T>() where T : MenuScreen {
        var type = typeof(T);
        return _menuScreens.ContainsKey(type) ? (T)_menuScreens[type] : null;
    }
    
    public T ShowPopup<T>(Action onFinish = null) where T : Popup {
        _canvasGroup.interactable = false;
        var popup = _showableFactory.Create<T>();
        popup.transform.SetParent(_popupsParent, false);
        popup.transform.SetAsLastSibling();////
        popup.SetActive(true);
        _popups[popup.GetType()] = popup;
        CurrentPopup = popup;
        CurrentPopup.PlayShowAnimation(()=> {
            _canvasGroup.interactable = true;
            onFinish?.Invoke();
        });
        return popup;
    }

    public void HidePopup<T>(Action onFinish = null) where T: Popup {
        if (_popups.Values.Count(pop => pop.IsActive) < 1) return;
        var popup = GetPopup<T>();
        if (popup == null) {
            Debug.LogError("POPUP DOES NOT EXIST YET");
            return;
        }
        HidePopup(popup, onFinish);
    }

    public void HidePopup(Popup popup, Action onFinish = null) {
        _canvasGroup.interactable = false;
        popup.PlayHideAnimation(() => {
            _canvasGroup.interactable = true;
            popup.SetActive(false);
            CurrentPopup = _popups.Values.OrderBy(pop => pop.RootIndex).LastOrDefault(pop => pop.IsActive);
            onFinish?.Invoke();
        });
    }

    public void HideAllPopups() {
        foreach (var popup in _popups.Values) {
            popup.SetActive(false);
        }
        CurrentPopup = null;
    }

    public bool Interactable {
        get => _canvasGroup.interactable;
        set => _canvasGroup.interactable = value;
    }

}
