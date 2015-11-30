using Assets.Scripts.Utility;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public const float MaxRotationAmount = 50f;
    public const float MaxYRotationAmount = 50f;
    public float MinZoomDistance = 5f;
    public float MaxZoomDistance = 20f;
    public float MinExtraZoomDistance = 0f;
    public float MaxExtraZoomDistance = 10f;

    public GameObject Target;

    public float ZoomDistance;
    public float ExtraZoomDistance;

    public float rotateXAmount = 0f;
    public float rotateYAmount = 0f;

    private static CameraControl _instance;
    public static CameraControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraControl>();
            }
            return _instance;
        }
    }

	void Start ()
	{
        ZoomDistance = Vector3.Distance(Target.transform.position, transform.position);
	}
	
	void Update () 
    {
	    if (!PlayerCharacter.Instance.GameStarted)
	    {
	        return;
	    }

        transform.forward = (Target.transform.position - transform.position).normalized;

	    if (!InputController.Instance.HookingModeOn)
	    {
            UpdateRotationInput();
            UpdateZoomDistanceInput();
	    }
	    UpdateRotation();
        UpdateZoomDistance();
    }

    private const float OutputSpeed = 200f;
    public float ZoomLimit = 0.5f;
    public float ZoomSpeed = 5f;

    void UpdateZoomDistance()
    {
        float distance = Vector3.Distance(Target.transform.position, transform.position);
        ExtraZoomDistance = Mathf.Clamp(ExtraZoomDistance,
            MinExtraZoomDistance, MaxExtraZoomDistance);
        if (Mathf.Abs((ZoomDistance + ExtraZoomDistance) - distance) > ZoomLimit)
        {
            if (distance > (ZoomDistance + ExtraZoomDistance))
            {
                transform.Translate(0f, 0f, 1f * Time.deltaTime * ZoomSpeed * (distance - (ZoomDistance + ExtraZoomDistance)));
            }
            else
            {
                transform.Translate(0f, 0f, -1f * Time.deltaTime * ZoomSpeed * ((ZoomDistance + ExtraZoomDistance) - distance));
            }
        }
    }

    void UpdateRotation()
    {
        float xAmount = 0f;
        float yAmount = 0f;
        if (Mathf.Abs(rotateXAmount) > 1f)
        {
            if (rotateXAmount > 0f)
            {
                xAmount = Easing.EaseInOut(Mathf.Clamp((rotateXAmount / MaxRotationAmount) + 0.3f, 0f, 1f), EasingType.Quintic) 
                    * OutputSpeed * Time.deltaTime;
            }
            else
            {
                xAmount = -Easing.EaseInOut(Mathf.Clamp((Mathf.Abs(rotateXAmount) / MaxRotationAmount) + 0.3f, 0f, 1f), EasingType.Quintic) 
                    * OutputSpeed * Time.deltaTime;
            }
            rotateXAmount -= xAmount;
            transform.RotateAround(Target.transform.position, Vector3.up, xAmount);
        }
        if (Mathf.Abs(rotateYAmount) > 1f)
        {
            if (rotateYAmount > 0f)
            {
                yAmount = Easing.EaseInOut(Mathf.Clamp((Mathf.Abs(rotateYAmount) / MaxRotationAmount) + 0.3f, 0f, 1f), EasingType.Quintic) 
                    * OutputSpeed * Time.deltaTime;
            }
            else
            {
                yAmount = -Easing.EaseInOut(Mathf.Clamp((Mathf.Abs(rotateYAmount) / MaxRotationAmount) + 0.3f, 0f, 1f), EasingType.Quintic) 
                    * OutputSpeed * Time.deltaTime;
            }
            rotateYAmount -= yAmount;
            if ((yAmount > 0f && 
                (Vector3.Angle(transform.forward, new Vector3(transform.forward.x, 0f, transform.forward.z)) < 70.0f || 
                (PlayerCharacter.Instance.PoolGameObject.transform.position.y - transform.position.y) <= 0f)) ||
                (yAmount < 0f && 
                (Vector3.Angle(transform.forward, new Vector3(transform.forward.x, 0f, transform.forward.z)) < 70.0f ||
                (PlayerCharacter.Instance.PoolGameObject.transform.position.y - transform.position.y) >= 0f)))
            {
                transform.RotateAround(Target.transform.position, Quaternion.AngleAxis(90, Vector3.down) *
                    new Vector3(transform.forward.x, 0f, transform.forward.z), yAmount);
            }
        }
    }

    private const float MaxYAxisInput = 3f;
    private const float MaxXAxisInput = 3f;
    private const float XAxisSpeed = 50f;
    private const float YAxisSpeed = 50f;
    private const float InputMinSpeed = 0.1f;

    void UpdateZoomDistanceInput()
    {
        float amount = InputController.Instance.GetZAxis();

        ZoomDistance += amount;
        ZoomDistance = Mathf.Clamp(ZoomDistance, MinZoomDistance, MaxZoomDistance);
    }

    void UpdateRotationInput()
    {
        float rotationX = 0f;
        float rotationY = 0f;
        if (Input.mousePosition.x > (Screen.width * 0.9f))
        {
            rotationX = MaxXAxisInput;
        }
        else if (Input.mousePosition.x < (Screen.width * 0.1f))
        {
            rotationX = -MaxXAxisInput;
        }
        else
        {
            rotationX = InputController.Instance.GetXAxis();
        }

        if (Input.mousePosition.y > (Screen.height * 0.9f))
        {
            rotationY = MaxYAxisInput;
        }
        else if (Input.mousePosition.y < (Screen.height * 0.1f))
        {
            rotationY = -MaxYAxisInput;
        }
        else
        {
            rotationY = InputController.Instance.GetYAxis();
        }

        rotationX = Mathf.Clamp(rotationX, -MaxXAxisInput, MaxXAxisInput);
        rotationY = Mathf.Clamp(rotationY, -MaxYAxisInput, MaxYAxisInput);

        if (Mathf.Abs(rotationX) > InputMinSpeed && Mathf.Abs(rotationY) > InputMinSpeed)
	    {
	        if (Mathf.Abs(rotateXAmount) < MaxRotationAmount)
	        {
                rotateXAmount += rotationX * Time.deltaTime * XAxisSpeed;
	        }
            if (Mathf.Abs(rotateYAmount) < MaxRotationAmount)
            {
                rotateYAmount += rotationY * Time.deltaTime * YAxisSpeed;
            }
	    }
        else if (Mathf.Abs(rotationY) > InputMinSpeed)
        {
            if (Mathf.Abs(rotateYAmount) < MaxRotationAmount)
            {
                rotateYAmount += rotationY * Time.deltaTime * YAxisSpeed;
            }
        }
        else if (Mathf.Abs(rotationX) > InputMinSpeed)
        {
            if (Mathf.Abs(rotateXAmount) < MaxRotationAmount)
            {
                rotateXAmount += rotationX * Time.deltaTime * XAxisSpeed;
            }
        }
    }

}
