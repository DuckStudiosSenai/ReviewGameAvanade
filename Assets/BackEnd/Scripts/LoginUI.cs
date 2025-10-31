using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class LoginUI : MonoBehaviourPun
{
    [Header("LoginUI")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI successText;
    public GameObject loginUi;

    [Header("RegisterUI")]
    public TMP_InputField usernameInputReg;
    public TMP_InputField passwordInputReg;
    public TextMeshProUGUI errorTextReg;
    public TextMeshProUGUI successReg;
    public Button registerButtonReg;

    [Header("Managers")]
    public PlayFabPhotonManager playFabPhotonManager;

    [Header("States")]
    public bool isLoginMenu = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PhotonNetwork.IsConnected)
        {
            loginUi.SetActive(false);
            return;
        }

        if (errorText != null)
            errorText.text = "";

        loginButton.onClick.AddListener(OnLoginClicked);
        registerButtonReg.onClick.AddListener(OnRegisterClicked);
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnected) return;

        if (isLoginMenu && Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnLoginClicked();
        } else if (!isLoginMenu && Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnRegisterClicked();
        }

        if (loginUi.activeSelf)
        {
            isLoginMenu = true;
            passwordInputReg.text = "";
            errorTextReg.text = "";
            usernameInputReg.text = "";
        }
        else
        {
            isLoginMenu = false;
            passwordInput.text = "";
            errorText.text = "";
            usernameInput.text = "";
        }
    }

    private void OnLoginClicked()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            errorText.text = "Preencha usuário e senha!";
            return;
        }

        playFabPhotonManager.Login(user, pass);
    }

    private void OnRegisterClicked()
    {
        string user = usernameInputReg.text.Trim();
        string pass = passwordInputReg.text.Trim();

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            errorTextReg.text = "Preencha usuário e senha!";
            return;
        }

        if (pass.Length < 6)
        {
            errorTextReg.text = "A senha precisa ter pelo menos 6 caracteres!";
            return;
        }

        playFabPhotonManager.Register(user, pass);
    }

    public void ShowError(string message)
    {
        errorText.text = message;
        successText.text = "";
    }

    public void ShowSuccess(string message)
    {
        successText.text = message;
        errorText.text = "";
    }

    public void ShowErrorReg(string message)
    {
        errorTextReg.text = message;
        successReg.text = "";
    }

    public void ShowSuccessReg(string message)
    {
        successReg.text = message;
        errorTextReg.text = "";
    }
}
