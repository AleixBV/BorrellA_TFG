using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScriptGUIController : MonoBehaviour {

	private float hSliderValue = 0.0f;
	private bool menuVisible = false;
	private int operateCameraNumber;
	private bool shadowOn;

    [SerializeField] private bool testGUI = false;
    [SerializeField] private bool testVR = false;

    [Header("Canvases without VR")]
    [SerializeField] private GameObject Canvases;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject Canvas_Loading;

    [Header("Canvases with VR")]
    [SerializeField] private GameObject Canvases_VR;
    [SerializeField] private GameObject Canvas_VR;
    [SerializeField] private GameObject Canvas_Loading_VR;
    [SerializeField] private Dropdown DropDownAccelerationType;
    [SerializeField] private Dropdown DropDownAccelerationTypeVR;

    private GameObject Canvas_Ref;
    private GameObject Canvas_Loading_Ref;

    [Space(15)]
    [SerializeField] GameObject Main_Camera;
    
    private Vector3 camera_init_position;
    private Quaternion camera_init_rotation;

    private float timer_check_input;

    private bool loading_scene;

    private float timer_change_camera;

    private void Awake()
    {
        if (!Main_Camera)
        {
            Debug.LogError("can't find Main Camera");

            QuitGame();
        }

        camera_init_position = Main_Camera.transform.localPosition;
        camera_init_rotation = Main_Camera.transform.localRotation;
    }

    // Use this for initialization
    void Start () {
        AudioListener.pause = false;
        Time.timeScale = 1;
        Public_Vars.game_paused = false;

        loading_scene = false;

        DropDownAccelerationType.value = Public_Vars.instant_acceleration == true ? 1 : 0;
        DropDownAccelerationTypeVR.value = Public_Vars.instant_acceleration == true ? 1 : 0;
        GetComponent<CameraController>().ChangeCamera(101);
		operateCameraNumber = 101;

		GetComponent<AmbientController>().changeShadow(true);
		shadowOn = true;

        if((testVR || XRDevice.isPresent) && !Public_Vars.forced_VR_disabled)
        {
            Canvases.SetActive(false);
            Canvases_VR.SetActive(true);

            Canvas_Ref = Canvas_VR;
            Canvas_Loading_Ref = Canvas_Loading_VR;
        }
        else
        {
            Canvases.SetActive(true);
            Canvases_VR.SetActive(false);

            Canvas_Ref = Canvas;
            Canvas_Loading_Ref = Canvas_Loading;
        }

        Canvas_Loading.SetActive(false);
        Canvas_Loading_VR.SetActive(false);

        if (Public_Vars.forced_controller_disabled)
            ForceControllerDisabled(Public_Vars.forced_controller_disabled);
        else
            ChangeCanvasIfController(true);

        timer_check_input = 0.0f;
        timer_change_camera = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (!loading_scene)
        {
            /*if (Public_Vars.is_controller_enabled && !Public_Vars.forced_controller_disabled)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                    StartGame();
                if (Input.GetKeyDown(KeyCode.Joystick1Button1))
                    QuitGame();
                if (Input.GetKeyDown(KeyCode.Joystick1Button2))
                    if (XRDevice.isPresent && !Public_Vars.forced_VR_disabled)
                        ForceVRDisabled(true);
                    else
                        ForceVRDisabled(false);
                if ((!XRDevice.isPresent || Public_Vars.forced_VR_disabled) && Input.GetKeyDown(KeyCode.Joystick1Button3))//Y
                    ForceControllerDisabled(true);
            }*/

            timer_check_input -= Time.deltaTime;
            if (timer_check_input <= 0.0f)
            {
                timer_check_input = 2.0f;
                if (!Public_Vars.forced_controller_disabled)
                    ChangeCanvasIfController();
                else
                    ChangeCameraIfVR();
            }

            if ((!testVR && !XRDevice.isPresent) || Public_Vars.forced_VR_disabled)
            {
                timer_change_camera -= Time.deltaTime;
                if (timer_change_camera <= 0)
                {
                    timer_change_camera = 20.0f;
                    int r = Random.Range(0, 5);
                    //Debug.Log("Change to camera: " + r.ToString());

                    switch (r)
                    {
                        case 0:
                            GetComponent<CameraController>().ChangeCamera(1);
                            break;
                        case 1:
                            GetComponent<CameraController>().ChangeCamera(5);
                            break;
                        case 2:
                            GetComponent<CameraController>().ChangeCamera(6);
                            break;
                        case 3:
                            GetComponent<CameraController>().ChangeCamera(7);
                            break;
                        case 4:
                            GetComponent<CameraController>().ChangeCamera(8);
                            break;
                    }
                }
            }
            else
            {
                timer_change_camera = 0.0f;
            }
        }
    }


	void OnGUI () {

        if (testGUI)
        {
            if (menuVisible == true)
            {
                GUI.BeginGroup(new Rect(50, 50, Screen.width - 100, 270));

                GUI.Box(new Rect(0, 0, Screen.width - 100, 270), "Control Menu");

                if (GUI.Button(new Rect(Screen.width - 100 - 50, 10, 40, 40), "X"))
                {
                    menuVisible = false;
                }

                // ---------- Sky Control ----------
                GUI.Label(new Rect(20, 40, 100, 30), "Sky Control");
                if (GUI.Button(new Rect(20, 60, 80, 40), "Sunny"))
                {
                    GetComponent<AmbientController>().changeSkybox(AmbientController.AmbientType.AMBIENT_SKYBOX_SUNNY);
                }
                if (GUI.Button(new Rect(110, 60, 80, 40), "Cloud"))
                {
                    GetComponent<AmbientController>().changeSkybox(AmbientController.AmbientType.AMBIENT_SKYBOX_CLOUD);
                }
                if (GUI.Button(new Rect(200, 60, 80, 40), "Night"))
                {
                    GetComponent<AmbientController>().changeSkybox(AmbientController.AmbientType.AMBIENT_SKYBOX_NIGHT);
                }

                // ---------- Shadow Control ----------
                GUI.Label(new Rect(20, 110, 100, 30), "Shadow Control");
                if (GUI.Button(new Rect(20, 130, 80, 40), "On / Off"))
                {
                    if (shadowOn == false)
                    {
                        GetComponent<AmbientController>().changeShadow(true);
                        shadowOn = true;
                    }
                    else
                    {
                        GetComponent<AmbientController>().changeShadow(false);
                        shadowOn = false;
                    }
                }
                GUI.Label(new Rect(120, 130, 100, 30), "TIme");
                hSliderValue = GUI.HorizontalSlider(new Rect(120, 155, 150, 30), hSliderValue, 0.0f, 100.0f);
                GetComponent<AmbientController>().rotateAmbientLight(hSliderValue);

                // ---------- Effect Control ----------
                GUI.Label(new Rect(20, 180, 100, 30), "Effect Control");
                if (GUI.Button(new Rect(20, 200, 80, 40), "None"))
                {
                    GetComponent<AmbientController>().changeParticle(AmbientController.ParticleType.PARTICLE_NONE);
                }
                if (GUI.Button(new Rect(110, 200, 80, 40), "Wind"))
                {
                    GetComponent<AmbientController>().changeParticle(AmbientController.ParticleType.PARTICLE_WIND);
                }
                if (GUI.Button(new Rect(200, 200, 80, 40), "Rain"))
                {
                    GetComponent<AmbientController>().changeParticle(AmbientController.ParticleType.PARTICLE_RAIN);
                }

                // ---------- Camera Control ----------
                if (operateCameraNumber < 100)
                {
                    GUI.Label(new Rect(400, 40, 100, 30), "Camera Control");
                    if (GUI.Button(new Rect(400, 60, 50, 40), "<---"))
                    {
                        operateCameraNumber--;
                        if (operateCameraNumber < 0)
                        {
                            operateCameraNumber = GetComponent<CameraController>().targetCameraNames.Count - 1;
                        }
                    }
                    if (GUI.Button(new Rect(600, 60, 50, 40), "--->"))
                    {
                        operateCameraNumber++;
                        if (operateCameraNumber > GetComponent<CameraController>().targetCameraNames.Count - 1)
                        {
                            operateCameraNumber = 0;
                        }
                    }
                }

                GUI.EndGroup();
            }
            else
            {
                // ---------- Menu Button ----------
                if (GUI.Button(new Rect(Screen.width - 120, 20, 100, 40), "Menu"))
                {
                    menuVisible = true;
                }
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

    public void QuitGame()
    {
        //Debug.Log("Quit Application");
        Application.Quit();
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
                Canvas_Ref.transform.GetChild(1).GetComponent<Button>().Select();

                SetButtonActive(Canvas_Ref.transform.Find("Button_Enable_Controller").gameObject, false);
                SetButtonActive(Canvas_Ref.transform.Find("Button_Disable_Controller").gameObject, true);
            }
        }
        else
        {
            if (Public_Vars.is_controller_enabled || clean)
            {
                Public_Vars.is_controller_enabled = false;
                //Debug.Log("controller not connected");

                EventSystem.current.sendNavigationEvents = false;

                SetButtonActive(Canvas_Ref.transform.Find("Button_Enable_Controller").gameObject, false);
                SetButtonActive(Canvas_Ref.transform.Find("Button_Disable_Controller").gameObject, false);
            }
        }

        ChangeCameraIfVR();
    }

    void ChangeCameraIfVR()
    {
        if ((testVR || XRDevice.isPresent) && !Public_Vars.forced_VR_disabled)
        {
            //Debug.Log("VR enabled");

            Canvases.transform.localRotation = Quaternion.identity;

            SetButtonActive(Canvas_Ref.transform.Find("Button_Enable_VR").gameObject, false);
            SetButtonActive(Canvas_Ref.transform.Find("Button_Disable_VR").gameObject, true);

            SetButtonActive(Canvas_Ref.transform.Find("Button_Disable_Controller").gameObject, false);//don't disable controller while VR enabled
        }
        else
        {
            //Debug.Log("VR not enabled");

            Canvases.transform.localEulerAngles = Main_Camera.transform.localEulerAngles;

            SetButtonActive(Canvas_Ref.transform.Find("Button_Disable_VR").gameObject, false);
            SetButtonActive(Canvas_Ref.transform.Find("Button_Enable_VR").gameObject, true);
        }
    }

    public void ForceControllerDisabled(bool b)
    {
        Public_Vars.forced_controller_disabled = b;

        if (!b)
        {
            SetButtonActive(Canvas_Ref.transform.Find("Button_Enable_Controller").gameObject, false);
            ChangeCanvasIfController(true);
        }
        else
        {
            Public_Vars.is_controller_enabled = false;

            SetButtonActive(Canvas_Ref.transform.Find("Button_Enable_Controller").gameObject, true);
            SetButtonActive(Canvas_Ref.transform.Find("Button_Disable_Controller").gameObject, false);

            EventSystem.current.sendNavigationEvents = false;
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
            Canvases.transform.localEulerAngles = Main_Camera.transform.localEulerAngles;

            XRSettings.LoadDeviceByName("");
            XRSettings.enabled = !b;

            Canvases.SetActive(true);
            Canvases_VR.SetActive(false);

            Canvas_Ref = Canvas;
            Canvas_Loading_Ref = Canvas_Loading;

            Canvas_Ref.SetActive(true);

            if (Public_Vars.forced_controller_disabled)
                ForceControllerDisabled(Public_Vars.forced_controller_disabled);
            else
                ChangeCanvasIfController(true);
        }
    }
    
    IEnumerator LoadDevice(string str)
    {
        Vector3 camera_old_position = Main_Camera.transform.localPosition;
        Quaternion camera_old_rotation = Main_Camera.transform.rotation;

        XRSettings.LoadDeviceByName(str);
        yield return null;
        if (XRSettings.loadedDeviceName == str)
        {
            XRSettings.enabled = true;
            Main_Camera.transform.localPosition = camera_init_position;
            Main_Camera.transform.localRotation = camera_init_rotation;

            Canvases.SetActive(false);
            Canvases_VR.SetActive(true);

            Canvas_Ref = Canvas_VR;
            Canvas_Loading_Ref = Canvas_Loading_VR;

            Canvas_Ref.SetActive(true);
        }
        else
        {
            Main_Camera.transform.localPosition = camera_old_position;
            Main_Camera.transform.localRotation = camera_old_rotation;
        }
        
        if (Public_Vars.forced_controller_disabled)
            ForceControllerDisabled(Public_Vars.forced_controller_disabled);
        else
            ChangeCanvasIfController(true);
    }

    public void StartGame()
    {
        Debug.Log("Game Started");
        loading_scene = true;

        Canvas_Ref.SetActive(false);

        Canvas_Loading_Ref.SetActive(true);

        StartCoroutine("Loading");
    }

    IEnumerator Loading()
    {
        SceneManager.LoadScene("Main_Scene");
        yield return true;
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

        DropDownAccelerationType.value = instant_acceleration;
        DropDownAccelerationTypeVR.value = instant_acceleration;
    }
}
