using UnityEngine;

public class InputController : MonoBehaviour
{
    public enum InputMode
    {
        Nagivation = 1,
        Hooking = 2
    }

    private static InputController _instance;
    public static InputController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputController>();
            }
            return _instance;
        }
    }

    public bool HookingModeOn = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        TurnOffHookingMode();
    }

    void TurnOnHookingMode()
    {
        if (HookingModeOn)
        {
            return;
        }
        HookingModeOn = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
        PlayerCharacter.Instance.Rope.LineRenderer.enabled = true;
        PlayerCharacter.Instance.Rope.RopeEnd.gameObject.SetActive(true);
        PlayerCharacter.Instance.Rope.RopeEnd.transform.position =
            PlayerCharacter.Instance.transform.position + new Vector3(0f, 2f, 0f);
        PlayerCharacter.Instance.Rope.SetPosition(
            PlayerCharacter.Instance.transform.position,
            PlayerCharacter.Instance.Rope.RopeEnd.transform.position);
        PlayerCharacter.Instance.Rope.ResetColor();
        PlayerCharacter.Instance.Rope.RopeEnd.Innocent = null;
    }

    void TurnOffHookingMode()
    {
        if (!HookingModeOn)
        {
            return;
        }
        HookingModeOn = false;
        PlayerCharacter.Instance.Rope.LineRenderer.enabled = false;
        PlayerCharacter.Instance.Rope.RopeEnd.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (!PlayerCharacter.Instance.GameStarted)
        {
            return;
        }
        UpdateInputMode();
        UpdateCurrentMode();
    }

    void UpdateInputMode()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            (Input.GetButton("TriggerLeft") && Input.GetButton("TriggerRight")))
        {
            PlayerCharacter.Instance.Enlarge();
        }
    }

    void UpdateCurrentMode()
    {
        UpdateNagivationMode();
        UpdateHookingMode();
    }

    void UpdateNagivationMode()
    {
        if (MoveDown())
        {
            PlayerCharacter.Instance.Accelerate(-Camera.main.transform.up);
        }
        if (MoveLeft())
        {
            PlayerCharacter.Instance.Accelerate(-Camera.main.transform.right);
        }
        if (MoveUp())
        {
            PlayerCharacter.Instance.Accelerate(Camera.main.transform.up);
        }
        if (MoveRight())
        {
            PlayerCharacter.Instance.Accelerate(Camera.main.transform.right);
        }
        if (MoveFoward())
        {
            PlayerCharacter.Instance.Accelerate(Camera.main.transform.forward);
        }
        if (MoveBackward())
        {
            PlayerCharacter.Instance.Accelerate(-Camera.main.transform.forward);
        }
    }

    private const float MaxRopeDistance = 20f;
    private const float MinRopeXSpeed = 0f;
    private const float MinRopeYSpeed = 0f;
    private const float MinRopeZSpeed = 0.05f;
    private const float XSpeed = 10.0f;
    private const float YSpeed = 10.0f;
    private const float ZSpeed = 100.0f;

    void UpdateHookingMode()
    {
        if (!HookingModeOn)
        {
            return;
        }
        float xSpeed = GetXAxis();
        float ySpeed = GetYAxis();
        float zSpeed = GetZAxis();

        Vector3 xDir = Quaternion.AngleAxis(90, Vector3.up) * new Vector3(Camera.main.transform.forward.x, 0f,
            Camera.main.transform.forward.z);
        Vector3 yDir = new Vector3(0f, 1f, 0f);
        Vector3 zDir = new Vector3(Camera.main.transform.forward.x, 0f, 
            Camera.main.transform.forward.z);
        if (Mathf.Abs(xSpeed) > MinRopeXSpeed)
        {
            xDir *= (xSpeed * Time.deltaTime * XSpeed);
        }
        else
        {
            xDir = Vector3.zero;
        }
        if (Mathf.Abs(ySpeed) > MinRopeYSpeed)
        {
            yDir *= (ySpeed * Time.deltaTime * YSpeed);
        }
        else
        {
            yDir = Vector3.zero;
        }
        if (Mathf.Abs(zSpeed) > MinRopeZSpeed)
        {
            zDir *= (zSpeed * Time.deltaTime * ZSpeed);
        }
        else
        {
            zDir = Vector3.zero;
        }

        Vector3 translateVector = xDir + yDir + zDir;

        PlayerCharacter.Instance.Rope.SetPosition(
                    PlayerCharacter.Instance.transform.position,
                    PlayerCharacter.Instance.Rope.RopeEnd.transform.position);

        if (Vector3.Distance(PlayerCharacter.Instance.transform.position + translateVector, 
            PlayerCharacter.Instance.Rope.RopeEnd.gameObject.transform.position) < MaxRopeDistance)
        {
            PlayerCharacter.Instance.Rope.RopeEnd.transform.Translate(translateVector);
        }

        //if (Input.GetMouseButtonDown(0) && PlayerCharacter.Instance.Rope.RopeEnd.Innocent != null &&
        //    !PlayerCharacter.Instance.Rope.RopeEnd.Innocent.Linked)
        //{
        //    PlayerCharacter.Instance.LinkNewChain(PlayerCharacter.Instance.Rope.RopeEnd.Innocent);
        //}
    }

    public float GetXAxis()
    {
        return (Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(Input.GetAxis("Controller X")))
            ? Input.GetAxis("Mouse X")
            : ((Mathf.Abs(Input.GetAxis("Controller X")) < 0.3f) ? 0f : Input.GetAxis("Controller X"));
    }

    public float GetYAxis()
    {
        return (Mathf.Abs(Input.GetAxis("Mouse Y")) > Mathf.Abs(Input.GetAxis("Controller Y"))) 
            ? Input.GetAxis("Mouse Y")
            : ((Mathf.Abs(Input.GetAxis("Controller Y")) < 0.3f) ? 0f : Input.GetAxis("Controller Y"));
    }

    public float GetZAxis()
    {
        return (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > Mathf.Abs(Input.GetAxis("Controller Z")))
            ? (Input.GetAxis("Mouse ScrollWheel") * 10f)
            : ((Mathf.Abs(Input.GetAxis("Controller Z")) < 0.3f) ? 0f : Input.GetAxis("Controller Z") * 0.5f);
    }

    public bool MoveDown()
    {
        return Input.GetKey(KeyCode.S) || Input.GetButton("MoveDown");
    }

    public bool MoveLeft()
    {
        return Input.GetKey(KeyCode.A) || Input.GetButton("MoveLeft");
    }

    public bool MoveUp()
    {
        return Input.GetKey(KeyCode.W) || Input.GetButton("MoveUp");
    }

    public bool MoveRight()
    {
        return Input.GetKey(KeyCode.D) || Input.GetButton("MoveRight");
    }

    public bool MoveFoward()
    {
        return Input.GetMouseButton(0) || (Input.GetAxis("MoveForward") > 0f);
    }

    public bool MoveBackward()
    {
        return Input.GetMouseButton(1) || (Input.GetAxis("MoveBackward") > 0f);
    }
}
