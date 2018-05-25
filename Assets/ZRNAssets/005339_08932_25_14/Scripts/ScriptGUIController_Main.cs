using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScriptGUIController_Main : MonoBehaviour {

	private float hSliderValue = 0.0f;
	private bool menuVisible = false;

    [SerializeField] private bool testGUI = false;

    [Header("Camera")]
    [SerializeField] private float VR_camera_Xoffset = -0.1f;
    [SerializeField] private float VR_camera_Zoffset = 0.2f;
    [SerializeField] private GameObject CameraContainer;

    [Header("Canvases")]
    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject Canvas_Loading;
    [SerializeField] private GameObject Canvases;
    [SerializeField] private Dropdown DropDownAccelerationType;

    [SerializeField] private MSVehicleController Player_Vehicle;

    private float timer_check_input;

    private bool loading_scene;

    private Vector3 camera_init_position;
    private Quaternion camera_init_rotation;
    private int camera_init_cullingmask;

    [Header("Respawn")]
    [SerializeField] private float respawn_timer = 2.5f;
    [SerializeField] private Color respawn_color = new Color(255, 255, 255, 0);
    private float respawn_alpha = 0.0f;
    private bool respawn = false;
    private Texture2D respawn_texture;

    // Use this for initialization
    void Start()
    {
        loading_scene = false;
        Public_Vars.game_paused = false;
        
        //GetComponent<AmbientController>().changeShadow(true);

        Canvas_Loading.SetActive(false);
        DropDownAccelerationType.value = Public_Vars.instant_acceleration == true ? 1 : 0;

        if (Public_Vars.forced_controller_disabled)
            ForceControllerDisabled(Public_Vars.forced_controller_disabled);
        else
            ChangeCanvasIfController(true);

        timer_check_input = 2.0f;

        camera_init_position = Player_Vehicle._cameras.cameras[0]._camera.transform.localPosition;
        camera_init_rotation = Player_Vehicle._cameras.cameras[0]._camera.transform.localRotation;
        camera_init_cullingmask = Player_Vehicle._cameras.cameras[0]._camera.cullingMask;

        respawn_texture = new Texture2D(1, 1);
        respawn_texture.SetPixel(1, 1, respawn_color);
        respawn_texture.Apply();
    }
	
	// Update is called once per frame
	void Update () {
        if (!loading_scene)
        {
            if (Public_Vars.game_paused)
            {
                if (Public_Vars.is_controller_enabled && !Public_Vars.forced_controller_disabled)
                {
                    if (Input.GetKeyDown(KeyCode.Joystick1Button7))//Start
                        ChangeCanvasVisibility();
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                    ChangeCanvasVisibility();

                
                timer_check_input -= Time.fixedDeltaTime;
                if (timer_check_input <= 0.0f)
                {
                    timer_check_input = 2.0f;
                    if (!Public_Vars.forced_controller_disabled)
                        ChangeCanvasIfController();
                    else
                        ChangeCameraIfVR();
                }
            }
            else
            {
                if (Public_Vars.is_controller_enabled && !Public_Vars.forced_controller_disabled && Input.GetKeyDown(KeyCode.Joystick1Button7) ||//Start
                    Input.GetKeyDown(KeyCode.Escape))
                    ChangeCanvasVisibility();
                else
                    ChangeIfController();
            }
        }
    }

	void InitAICars () {

		/*GameObject[] targetAICars = GameObject.FindGameObjectsWithTag("AICars");
		foreach (GameObject targetAICar in targetAICars)
		{
			targetAICar.GetComponent<AICarMove>().InitAICar();
		}*/
	}

    public void ResumeMainGame()
    {
        ChangeCanvasVisibility();
    }

    void ChangeCanvasIfController(bool clean = false)
    {
        if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].Length > 0)
        {
            if (!Public_Vars.is_controller_enabled || clean)
            {
                Public_Vars.is_controller_enabled = true;
                //Debug.Log("controller connected");

                EventSystem.current.sendNavigationEvents = true;
                Canvas.transform.GetChild(1).GetComponent<Button>().Select();
                Canvas.transform.GetChild(1).GetComponent<Button>().OnSelect(null);//make sure to highlight the button

                SetButtonActive(Canvas.transform.Find("Button_Enable_Controller").gameObject, false);
                SetButtonActive(Canvas.transform.Find("Button_Disable_Controller").gameObject, true);
            }
        }
        else
        {
            if (Public_Vars.is_controller_enabled || clean)
            {
                Public_Vars.is_controller_enabled = false;
                //Debug.Log("controller not connected");

                EventSystem.current.sendNavigationEvents = false;

                SetButtonActive(Canvas.transform.Find("Button_Enable_Controller").gameObject, false);
                SetButtonActive(Canvas.transform.Find("Button_Disable_Controller").gameObject, false);
            }
        }

        ChangeCameraIfVR();
    }

    void ChangeIfController()
    {
        if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].Length > 0)
        {
            if (!Public_Vars.is_controller_enabled)
            {
                Public_Vars.is_controller_enabled = true;
                //Debug.Log("controller connected");
            }
        }
        else
        {
            if (Public_Vars.is_controller_enabled)
            {
                Public_Vars.is_controller_enabled = false;
                //Debug.Log("controller not connected");
            }
        }
    }

    void ChangeCameraIfVR()
    {
        if (XRDevice.isPresent && !Public_Vars.forced_VR_disabled)
        {
            //Debug.Log("VR enabled");

            Canvases.transform.localRotation = Quaternion.identity;

            Vector3 tmp = new Vector3(VR_camera_Xoffset, 0.0f, VR_camera_Zoffset);
            CameraContainer.transform.localPosition = tmp;

            SetButtonActive(Canvas.transform.Find("Button_Enable_VR").gameObject, false);
            SetButtonActive(Canvas.transform.Find("Button_Disable_VR").gameObject, true);

            SetButtonActive(Canvas.transform.Find("Button_Disable_Controller").gameObject, false);

            Player_Vehicle._cameras.cameras[0].rotationType = CameraTypeClass.TipoRotac.VR_Nothing;
        }
        else
        {
            //Debug.Log("VR not enabled");

            Canvases.transform.localEulerAngles = Player_Vehicle._cameras.cameras[0]._camera.transform.localEulerAngles;

            CameraContainer.transform.localPosition = Vector3.zero;

            SetButtonActive(Canvas.transform.Find("Button_Disable_VR").gameObject, false);
            SetButtonActive(Canvas.transform.Find("Button_Enable_VR").gameObject, true);

            Player_Vehicle._cameras.cameras[0].rotationType = CameraTypeClass.TipoRotac.ETS_StyleCamera;
        }
    }

    public void ForceControllerDisabled(bool b)
    {
        Public_Vars.forced_controller_disabled = b;

        if (!b)
        {
            SetButtonActive(Canvas.transform.Find("Button_Enable_Controller").gameObject, false);
            ChangeCanvasIfController(true);
        }
        else
        {
            Public_Vars.is_controller_enabled = false;
            
            SetButtonActive(Canvas.transform.Find("Button_Enable_Controller").gameObject, true);
            SetButtonActive(Canvas.transform.Find("Button_Disable_Controller").gameObject, false);

            EventSystem.current.sendNavigationEvents = false;
            ChangeCameraIfVR();
        }
    }

    public void ForceVRDisabled(bool b)
    {
        Public_Vars.forced_VR_disabled = b;

        if (!b)
        {
            //Debug.Log(XRSettings.supportedDevices[0]);
            StartCoroutine(LoadDevice(XRSettings.supportedDevices[0]));
        }
        else
        {
            Player_Vehicle._cameras.cameras[0].rotationType = CameraTypeClass.TipoRotac.ETS_StyleCamera;
            
            Canvases.transform.localEulerAngles = Player_Vehicle._cameras.cameras[0]._camera.transform.localEulerAngles;

            Player_Vehicle._cameras.cameras[0]._camera.transform.localPosition = camera_init_position;

            XRSettings.LoadDeviceByName("");
            XRSettings.enabled = !b;
            
            if (Public_Vars.forced_controller_disabled)
                ForceControllerDisabled(Public_Vars.forced_controller_disabled);
            else
                ChangeCanvasIfController(true);
        }
    }

    public void QuitMainGame()
    {
        //Debug.Log("Return to Init_Scene");
        loading_scene = true;

        Canvases.SetActive(true);

        Canvas.SetActive(false);

        Canvas_Loading.SetActive(true);

        StartCoroutine("Loading");
    }

    public void ChangeCanvasVisibility()
    {
        Canvases.SetActive(!Canvases.activeSelf);
        Public_Vars.game_paused = Canvases.activeSelf;
        if (Public_Vars.game_paused)
        {
            AudioListener.pause = true;
            //AudioListener.volume = 0;
            Time.timeScale = 0;

            Player_Vehicle._cameras.cameras[0]._camera.cullingMask = 1 << 5; //UI Layer
            
            timer_check_input = 2.0f;
            if (Public_Vars.forced_controller_disabled)
                ForceControllerDisabled(Public_Vars.forced_controller_disabled);
            else
                ChangeCanvasIfController(true);
        }
        else
        {
            AudioListener.pause = false;
            Time.timeScale = 1;

            Player_Vehicle._cameras.cameras[0]._camera.cullingMask = camera_init_cullingmask; //Initial Layer
        }

        if (XRDevice.isPresent && !Public_Vars.forced_VR_disabled)
            Canvases.transform.localRotation = Quaternion.identity;
        else
            Canvases.transform.localEulerAngles = Player_Vehicle._cameras.cameras[0]._camera.transform.localEulerAngles;
    }

    IEnumerator Loading()
    {
        SceneManager.LoadScene("Init_Scene");
        
        yield return true;
    }

    IEnumerator LoadDevice(string str)
    {
        Vector3 camera_old_position = Player_Vehicle._cameras.cameras[0]._camera.transform.localPosition;
        Quaternion camera_old_rotation = Player_Vehicle._cameras.cameras[0]._camera.transform.localRotation;

        XRSettings.LoadDeviceByName(str);
        yield return null;
        if (XRSettings.loadedDeviceName == str)
        {
            XRSettings.enabled = true;
            Player_Vehicle._cameras.cameras[0]._camera.transform.localPosition = camera_init_position;
            Player_Vehicle._cameras.cameras[0]._camera.transform.localRotation = camera_init_rotation;
        }
        else
        {
            Player_Vehicle._cameras.cameras[0]._camera.transform.localPosition = camera_old_position;
            Player_Vehicle._cameras.cameras[0]._camera.transform.localRotation = camera_old_rotation;
        }

        if (Public_Vars.forced_controller_disabled)
            ForceControllerDisabled(Public_Vars.forced_controller_disabled);
        else
            ChangeCanvasIfController(true);
    }

    public void SetRespawn() { respawn = true; }

    private void OnGUI()
    {
        if(respawn)
        {
            if (respawn_alpha < 1.0f)
            {
                respawn_alpha += Time.deltaTime / respawn_timer;

                if (respawn_alpha >= 1.0f)
                    respawn_alpha = 1.0f;
            }
            else
            {
                SceneManager.LoadScene("Init_Scene");
            }

            respawn_color.a = respawn_alpha;
            respawn_texture.SetPixel(0, 0, respawn_color);
            respawn_texture.Apply();

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), respawn_texture);
        }
    }

    private void SetButtonActive(GameObject Button, bool active)
    {
        Button.SetActive(active);
        Button.GetComponent<Button>().interactable = active;
    }

    public void ChangeAccelerationType(int instant_acceleration)
    {
        bool ret = instant_acceleration >= 1 ? true : false;
        Public_Vars.instant_acceleration = ret;

        Player_Vehicle.ChangeAccelerationType(ret);
    }
}