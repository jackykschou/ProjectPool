using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Serializable]
    public class Chain
    {
        public Innocent Innocent;
        public LineRenderer LineRenderer;
    }

    private static PlayerCharacter _instance;
    public static PlayerCharacter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerCharacter>();
            }
            return _instance;
        }
    }

    public AudioSource BmgAudioSource;
    public AudioSource ShakeAudioSource;
    public AudioSource MovementAudioSource;
    public AudioClip LinkForm;
    public AudioClip Zoom;
    public AudioSource ZoomFinalAudioSource;

    public bool GameStarted = false;

    public GameObject PoolGameObject;
    public GameObject ChainFormationParticle;
    public ParticleSystem SpeedParticle;

    public Rigidbody Rigidbody;

    public float AccelerateRate = 1.0f;
    public float DeaccelerateRate = 0.5f;

    public MeshRenderer MeshRenderer;
    public Material OriginalStartMaterial;
    public Material SkyboxMaterial;
    public Color TargetedPoolColor;

    public Rope Rope;

    public List<Chain> Chains;

    public int NextEnlargeThreshold;
    public int CurrentLinkedNumber;
    public int EnlargedCount;

    public GameObject InstructionPanel;
    public GameObject StartPanel;

    public void OpenInstruction()
    {
        InstructionPanel.SetActive(true);
        StartPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        InstructionPanel.SetActive(false);
        StartPanel.SetActive(true);
    }

    public void Accelerate(Vector3 direction)
    {
        Rigidbody.AddForce(direction * AccelerateRate);
    }

    public void Deaccelerate()
    {
        Rigidbody.AddForce(-Rigidbody.velocity * DeaccelerateRate);
    }

	void Start ()
	{
	    MeshRenderer = PoolGameObject.GetComponent<MeshRenderer>();
        MeshRenderer.material.CopyPropertiesFromMaterial(OriginalStartMaterial);
	    Rigidbody = GetComponent<Rigidbody>();
        TargetedPoolColor = MeshRenderer.material.color;
        SkyboxMaterial.SetColor("_Tint", MeshRenderer.material.color);
	    NextEnlargeThreshold = 16;
	    CurrentLinkedNumber = 0;
	    EnlargedCount = 0;
	}

	void Update ()
	{
        if (!GameStarted)
        {
            return;
        }
	    UpdateMoveNoise();
	    EnlargeEffect();
	    UpdateSpeedParticle();
	    UpdateExtraZoomDistance();
	}

   

    public GameObject MainUI;

    public void StartGame()
    {
        MainUI.SetActive(false);
        GameStarted = true;
    }

    public void RestartGame()
    {
        Application.LoadLevel(0);
    }
    
    private float _currentShakeX = 0f;
    private float _currentShakeY = 0f;
    private float _currentShakeZ = 0f;
    private float _shakeAmountMax = 0.5f;
    private float _shakeAmount = 0.1f;

    void EnlargeEffect()
    {
        if (CurrentLinkedNumber < NextEnlargeThreshold)
        {
            return;
        }

        float shakeXAmount = UnityEngine.Random.Range(-_shakeAmount, _shakeAmount);
        float shakeYAmount = UnityEngine.Random.Range(-_shakeAmount, _shakeAmount);
        float shakeZAmount = UnityEngine.Random.Range(-_shakeAmount, _shakeAmount);

        if ((shakeXAmount + _currentShakeX) > _shakeAmountMax ||
            (shakeXAmount + _currentShakeX) < -_shakeAmountMax)
        {
            shakeXAmount *= -1.0f;
        }
        if ((shakeYAmount + _currentShakeY) > _shakeAmountMax ||
            (shakeYAmount + _currentShakeY) < -_shakeAmountMax)
        {
            shakeYAmount *= -1.0f;
        }
        if ((shakeZAmount + _currentShakeZ) > _shakeAmountMax ||
            (shakeZAmount + _currentShakeZ) < -_shakeAmountMax)
        {
            shakeZAmount *= -1.0f;
        }

        _currentShakeX += shakeXAmount;
        _currentShakeY += shakeYAmount;
        _currentShakeZ += shakeZAmount;

        PoolGameObject.transform.position =
            new Vector3(PoolGameObject.transform.position.x + shakeXAmount,
                PoolGameObject.transform.position.y + shakeYAmount,
                PoolGameObject.transform.position.z + shakeZAmount);
    }

    void FixedUpdate()
    {
        Rigidbody.velocity *= 0.95f;
    }

    private float _extraZoomSpeed =10f;

    void UpdateExtraZoomDistance()
    {
        CameraControl.Instance.ExtraZoomDistance = Rigidbody.velocity.magnitude * _extraZoomSpeed;
    }

    void UpdateSpeedParticle()
    {
        SpeedParticle.transform.forward = -Camera.main.transform.forward;
        SpeedParticle.startColor = new Color(MeshRenderer.material.color.r, 
            MeshRenderer.material.color.g, 
            MeshRenderer.material.color.b,
            0.1f);
    }

    void UpdateMoveNoise()
    {
        if ((Mathf.Abs(Rigidbody.velocity.magnitude) < 10f) &&
            MovementAudioSource.isPlaying)
        {
            MovementAudioSource.Stop();
        }
        else if ((Mathf.Abs(Rigidbody.velocity.magnitude) > 10f) 
            && !MovementAudioSource.isPlaying)
        {
            MovementAudioSource.Play();
        }
    }

    public Color PredicateColor(Color newColor)
    {
        Color returnColor = new Color(newColor.r, newColor.g, newColor.b, newColor.a);

        for (int j = 0; j < 2; j++)
        {
            returnColor += Color.white;
        }

        int i = 3;
        foreach (var chain in Chains)
        {
            returnColor += chain.Innocent.MeshRenderer.material.color;
            ++i;
        }
        return (returnColor / i);
    }

    private const float ChainFormationEffectInterval = 0.3f;

    public void LinkNewChain(Innocent innocent)
    {
        if (innocent.Linked)
        {
            return;
        }
        CurrentLinkedNumber++;
        innocent.Linked = true;
        innocent.SpeedMax = 80f;
        innocent.SpeedMax = 100f;
        innocent.ChangeSpeedAndDirection();
        AudioSource.PlayClipAtPoint(LinkForm, Camera.main.transform.position);

        //LineRenderer lineRenderer = innocent.gameObject.AddComponent<LineRenderer>();
        Chains.Add(new Chain()
        {
            Innocent = innocent,
            //LineRenderer = lineRenderer
        });
        SpringJoint spring = innocent.gameObject.AddComponent<SpringJoint>();
        spring.connectedBody = Rigidbody;
        //lineRenderer.material = Rope.LineRenderer.material;
        //lineRenderer.SetWidth(0.05f, 0.05f);
        UpdateNewColor(innocent.MeshRenderer.material.color);
        Vector3 dir = (innocent.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, innocent.transform.position);
        Vector3 spawnPos = transform.position + (dir * (distance - 1f));
        GameObject effect = Instantiate(ChainFormationParticle, spawnPos, Quaternion.identity) as GameObject;
        effect.transform.SetParent(transform);
        effect.GetComponent<ParticleSystem>().startColor = 
            new Color(MeshRenderer.material.color.r, 
                MeshRenderer.material.color.g, 
                MeshRenderer.material.color.b, 
                0.6f);
        effect.GetComponent<ParticleSystem>().Play();
        Destroy(effect, 3.0f);

        if (CurrentLinkedNumber == NextEnlargeThreshold)
        {
            ShakeAudioSource.Play();
        }
    }

    IEnumerator SpawnChainEffect(Innocent innocent)
    {
        Vector3 dir = (innocent.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, innocent.transform.position);
        float spawnedDistance = ChainFormationEffectInterval;
        Vector3 spawnPos = transform.position + (dir * ChainFormationEffectInterval);
        while (spawnedDistance < distance)
        {
            GameObject effect = Instantiate(ChainFormationParticle, spawnPos, Quaternion.identity) as GameObject;
            effect.GetComponent<ParticleSystem>().startColor = MeshRenderer.material.color;
            effect.GetComponent<ParticleSystem>().Play();
            Destroy(effect, 3.0f);
            spawnedDistance += ChainFormationEffectInterval;
            spawnPos += (dir * ChainFormationEffectInterval);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    void UpdateNewColor(Color newColor)
    {
        var cor = PredicateColor(newColor);
        TargetedPoolColor = new Color(cor.r, cor.g, cor.b, 0.4f);
        Rope.LineRenderer.material.color = TargetedPoolColor;
        StopCoroutine("LerpColor");
        StartCoroutine(LerpColor());
    }

    IEnumerator LerpColor()
    {
        float timer = 0f;
        Color originalColor = MeshRenderer.material.color;
        while (timer < 2f)
        {
            MeshRenderer.material.color = Color.Lerp(originalColor, TargetedPoolColor, timer / 2.0f);
            SkyboxMaterial.SetColor("_Tint", MeshRenderer.material.color);
            yield return new WaitForSeconds(Time.deltaTime);
            timer += Time.deltaTime;
        }
    }

    void UpdateChains()
    {
        foreach (var chain in Chains)
        {
           // chain.LineRenderer.SetPosition(0, transform.position);
           // chain.LineRenderer.SetPosition(1, chain.Innocent.transform.position);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Innocent innocent = col.gameObject.GetComponent<Innocent>();
        if (innocent != null && !innocent.Linked)
        {
            LinkNewChain(innocent);
        }
    }

    public void PlayClickSound()
    {
        AudioSource.PlayClipAtPoint(LinkForm, Camera.main.transform.position);
    }

    public void Enlarge()
    {
        if (CurrentLinkedNumber < NextEnlargeThreshold)
        {
            return;
        }

        ShakeAudioSource.Stop();

        CameraControl.Instance.MaxExtraZoomDistance *= 2;
        CameraControl.Instance.MinExtraZoomDistance *= 2;
        CameraControl.Instance.ExtraZoomDistance =
            (CameraControl.Instance.MaxExtraZoomDistance +
            CameraControl.Instance.MinExtraZoomDistance) / 2.0f;
        CameraControl.Instance.MinZoomDistance *= 2;
        CameraControl.Instance.MaxZoomDistance *= 2;
        CameraControl.Instance.ZoomDistance = (CameraControl.Instance.MaxZoomDistance +
                                               CameraControl.Instance.MinZoomDistance)/2.0f;
        if (EnlargedCount == 2)
        {
            gameObject.ScaleTo(transform.lossyScale * 30.0f, 15f, 0f, EaseType.easeInSine);
            BmgAudioSource.Stop();
            ZoomFinalAudioSource.Play();
            StartCoroutine(EndGame());
        }
        else
        {
            AudioSource.PlayClipAtPoint(Zoom, Camera.main.transform.position);
            gameObject.ScaleTo(transform.lossyScale * 2.0f, 1.5f, 0f);
        }
        _extraZoomSpeed *= 2;
        _shakeAmountMax *= 2;
        _shakeAmount *= 2;
        NextEnlargeThreshold *= 4;
        CurrentLinkedNumber = 0;
        EnlargedCount++;
        CameraControl.Instance.ZoomLimit *= 2.0f;
        AccelerateRate *= 1.2f;
    }

    IEnumerator EndGame()
    {
        GameStarted = false;
        yield return new WaitForSeconds(20.0f);
        RestartGame();
    }
}
