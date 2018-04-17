using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class ScriptGUIController_Main : MonoBehaviour {

	private float hSliderValue = 0.0f;
	private bool menuVisible = false;
	private bool shadowOn;

    public bool testGUI = false;

    public float VR_camera_Xoffset = -0.1f;
    public float VR_camera_Zoffset = 0.2f;
    public GameObject CameraContainer;

    GameObject Canvas_PC;
    GameObject Canvas_Controller;
    GameObject Canvas_Loading;
    private GameObject Canvases;

    public GameObject Player_Vehicle;

    private float timer_check_input;

    private bool loading_scene;

    private Vector3 camera_init_position;
    private Quaternion camera_init_rotation;
    
    // Use this for initialization
    void Start()
    {
        loading_scene = false;
        Public_Vars.game_paused = false;

        //GetComponent<AmbientController>().changeShadow(true);
        shadowOn = true;

        Canvases = GameObject.Find("Canvases");

        Canvas_PC = GameObject.Find("Canvas_PC");
        Canvas_Controller = GameObject.Find("Canvas_Controller");
        Canvas_Loading = GameObject.Find("Canvas_Loading");

        if (!Canvas_PC || !Canvas_Controller || !Canvas_Loading || !Canvases)
        {
            Debug.LogError("can't find one Canvas");

            Application.Quit();
        }

        Canvas_Loading.SetActive(false);

        ChangeCanvasIfController();
        ChangeCameraIfVR();

        ChangeCanvasVisibility();

        timer_check_input = 2.0f;

        camera_init_position = Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localPosition;
        camera_init_rotation = Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (!loading_scene)
        {
            if (Public_Vars.game_paused)
            {
                if (Public_Vars.is_controller_enabled && !Public_Vars.forced_controller_disabled)
                {
                    if (Input.GetKeyDown(KeyCode.Joystick1Button0))//A
                        ResumeMainGame();
                    if (Input.GetKeyDown(KeyCode.Joystick1Button1))//B
                        QuitMainGame();
                    if (Input.GetKeyDown(KeyCode.Joystick1Button2))//X
                        if (XRDevice.isPresent && !Public_Vars.forced_VR_disabled)
                            ForceVRDisabled(true);
                        else
                            ForceVRDisabled(false);
                    if ((!XRDevice.isPresent || Public_Vars.forced_VR_disabled) && Input.GetKeyDown(KeyCode.Joystick1Button3))//Y
                        ForceControllerDisabled(true);
                    if (Input.GetKeyDown(KeyCode.Joystick1Button7))//Start
                        ChangeCanvasVisibility();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                    ChangeCanvasVisibility();
            }
            else
            {
                if (Public_Vars.is_controller_enabled && !Public_Vars.forced_controller_disabled)
                {
                    if (Input.GetKeyDown(KeyCode.Joystick1Button7))//Start
                        ChangeCanvasVisibility();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                    ChangeCanvasVisibility();
            }

            timer_check_input -= Time.fixedDeltaTime;
            if (timer_check_input <= 0.0f)
            {
                timer_check_input = 2.0f;
                if (!Public_Vars.forced_controller_disabled)
                    ChangeCanvasIfController();

                ChangeCameraIfVR();
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

    void ChangeCanvasIfController()
    {
        if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].Length > 0)
        {
            Public_Vars.is_controller_enabled = true;
            //Debug.Log("controller enabled");

            Canvas_PC.SetActive(false);
            Canvas_Controller.SetActive(true);
        }
        else
        {
            Public_Vars.is_controller_enabled = false;
            //Debug.Log("controller not enabled");

            Canvas_PC.SetActive(true);
            Canvas_Controller.SetActive(false);
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

            Canvas_PC.transform.Find("Button_Enable_VR").gameObject.SetActive(false);
            Canvas_PC.transform.Find("Button_Disable_VR").gameObject.SetActive(true);

            Canvas_Controller.transform.Find("Button_Enable_VR").gameObject.SetActive(false);
            Canvas_Controller.transform.Find("Button_Disable_VR").gameObject.SetActive(true);

            Canvas_Controller.transform.Find("Button_Disable_Controller").gameObject.SetActive(false);

            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0].rotationType = CameraTypeClass.TipoRotac.VR_Nothing;
        }
        else
        {
            //Debug.Log("VR not enabled");

            Canvases.transform.localEulerAngles = Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localEulerAngles;

            CameraContainer.transform.localPosition = Vector3.zero;

            Canvas_PC.transform.Find("Button_Disable_VR").gameObject.SetActive(false);
            Canvas_Controller.transform.Find("Button_Disable_VR").gameObject.SetActive(false);

            Canvas_PC.transform.Find("Button_Enable_VR").gameObject.SetActive(true);
            Canvas_Controller.transform.Find("Button_Enable_VR").gameObject.SetActive(true);

            Canvas_Controller.transform.Find("Button_Disable_Controller").gameObject.SetActive(true);

            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0].rotationType = CameraTypeClass.TipoRotac.ETS_StyleCamera;
        }
    }

    public void ForceControllerDisabled(bool b)
    {
        Public_Vars.forced_controller_disabled = b;

        if (!b)
        {
            Canvas_PC.transform.Find("Button_Enable_Controller").gameObject.SetActive(false);
            ChangeCanvasIfController();
        }
        else
        {
            Public_Vars.is_controller_enabled = false;

            Canvas_PC.SetActive(true);
            Canvas_Controller.SetActive(false);
            Canvas_PC.transform.Find("Button_Enable_Controller").gameObject.SetActive(true);
        }

        timer_check_input = 0.0f;
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
            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0].rotationType = CameraTypeClass.TipoRotac.ETS_StyleCamera;
            
            Canvases.transform.localEulerAngles = Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localEulerAngles;

            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localPosition = camera_init_position;

            XRSettings.LoadDeviceByName("");
            XRSettings.enabled = !b;

            timer_check_input = 0.0f;
        }
    }

    public void QuitMainGame()
    {
        //Debug.Log("Return to Init_Scene");
        loading_scene = true;

        Canvases.SetActive(true);

        Canvas_PC.SetActive(false);
        Canvas_Controller.SetActive(false);

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

            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.cullingMask = 1 << 5; //UI Layer
        }
        else
        {
            AudioListener.pause = false;
            Time.timeScale = 1;

            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.cullingMask = -1; //Default Layer
        }

        if (XRDevice.isPresent && !Public_Vars.forced_VR_disabled)
            Canvases.transform.localRotation = Quaternion.identity;
        else
            Canvases.transform.localEulerAngles = Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localEulerAngles;
    }

    IEnumerator Loading()
    {
        SceneManager.LoadScene("Init_Scene");
        
        yield return true;
    }

    IEnumerator LoadDevice(string str)
    {
        Vector3 camera_old_position = Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localPosition;
        Quaternion camera_old_rotation = Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.rotation;

        XRSettings.LoadDeviceByName(str);
        yield return null;
        if (XRSettings.loadedDeviceName == str)
        {
            XRSettings.enabled = true;
            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localPosition = camera_init_position;
            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localRotation = camera_init_rotation;
        }
        else
        {
            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localPosition = camera_old_position;
            Player_Vehicle.GetComponent<MSVehicleController>()._cameras.cameras[0]._camera.transform.localRotation = camera_old_rotation;
        }

        timer_check_input = 0.0f;
    }

    /*void OnGUI () {

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
    }*/
}