using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // keep momentum when paused?
    // https://www.youtube.com/watch?v=blO039OzUZc
    //8:14 https://www.youtube.com/watch?v=BBS2nIKzmbw
    // 

    [System.Serializable]
    public class MoveSettings
    {
        public float forwardVel = 12;
        public float strafeVel = 12;
        //???????
        public float rotateVel = 10;
        public float jumpVel = 25;
        public float distToGrounded = 0.1f;
        public LayerMask ground;
    }

    [System.Serializable]
    public class PhysSettings
    {
        public float downAccel = 0.75f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.01f;
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
    }

    [System.Serializable]
    public class LightSettings
    {
		public int maxLightsForLuminance;
        public int luminanceCalcsPerFrame;
        public Transform lightCheckArea;
        public float maxCheckDistance;
        //layerIgnoreNumber is in checkLuminance
    }

    [System.Serializable]
    public class Stats
    {
        public int gold = 0;
        public int health = 10;
        public int mana = 10;
    }
    public GameController gameController;
    private Animator anim;
    public FirstPersonCam camera; 

    public MoveSettings moveSetting = new MoveSettings();
    public PhysSettings physSetting = new PhysSettings();
    public LightSettings lightSetting = new LightSettings();
    public InputSettings inputSetting = new InputSettings();
    public Stats stats = new Stats();
    

    public float sneakMod;
    public float sprintMod;

    public float interactDistance = 10.0f;

    public Transform coverCheck;

    Vector3 velocity = Vector3.zero;
    //Quaternion targetRotation;
    float forwardInput;
    float strafeInput;
    float jumpInput;
    Rigidbody rb = new Rigidbody();
    Vector3 movementVector = Vector3.zero;

    bool sneaking = false;
    bool sprinting = false;

	//Affects diminishing returns of lights' individual contribution to luminance
	//Higher values mean faster diminishing returns
	public float luminancDiminishingFactor = 2f;
	//Overall multiplier to the player's luminance
	public float luminanceMultiplier = 1.0f;
	//The exponent for how much luminance decreases with distance from a light source
	//1 is linear falloff, 2 is quadratic, etc.
	public float luminanceFalloff = 1.5f;
	public float luminance;

    //public Quaternion TargetRotation()
    //{
    //    return targetRotation;
    //} 

    bool Grounded()
    {
        bool grounded = Physics.Raycast(transform.position, Vector3.down, moveSetting.distToGrounded, moveSetting.ground);
        Debug.DrawRay(transform.position, Vector3.down * moveSetting.distToGrounded, Color.green);
        //Debug.Log("Grounded = " + grounded);
        return grounded;
    }
    // Start is called before the first frame update
    void Start()
    {
        //targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
        {
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("The Character needs a rigidbody");
        }
        if (GetComponentInChildren<Animator>())
        {
            anim = GetComponentInChildren<Animator>();
        }
        else
        {
            Debug.LogError("The Character needs an Animation controller");
        }

        forwardInput = 0;
        strafeInput = 0;
        jumpInput = 0;

        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(checkLuminance());
	}

    void GetInput()
    {
		const int ignoreRaycast = 2;
		const int ignoreLightLayer = 10;
		const int layerMask = ~((1 << ignoreLightLayer) | (1 << ignoreRaycast));

		if (gameController.paused == false)
        {
			if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo, interactDistance, layerMask))
			{
				Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
				if (interactable != null)
				{
					interactable.Hover(gameObject);
					if (Input.GetKey(KeyCode.E))
					{
						interactable.Interact(gameObject, Input.GetKeyDown(KeyCode.E));
					}
				}
			}
			if (Input.GetKey(KeyCode.E))
			{
				Debug.DrawRay(camera.transform.position, camera.transform.forward * interactDistance, Color.magenta);
			}



			Cursor.lockState = CursorLockMode.Locked;

            forwardInput = Input.GetAxis(inputSetting.FORWARD_AXIS);//interpolated (-1-1)
            strafeInput = Input.GetAxis(inputSetting.TURN_AXIS);
            jumpInput = Input.GetAxisRaw(inputSetting.JUMP_AXIS);//not interpolated (-1 or 1)
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //switch the pause state
            gameController.TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleSprinting();
            anim.GetBool("CharRunOn").Equals(true);
        }
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			//switch the pause state

			ToggleSneaking();
		}
	}


    // Update is called once per frame
    void Update()
    {
        GetInput();

        if(gameController.paused == false)
        {
            //Turn();
        }
    }

    //can happen more than once in a frame
    void FixedUpdate()
    {
        if(gameController.paused == false)
        {
            Run();
            Jump();
            rb.useGravity = !Grounded();
            rb.velocity = transform.TransformDirection(velocity);
        }
    }

    void Run()
    {
        //check for input deadzone
        if(Mathf.Abs(forwardInput) > inputSetting.inputDelay)
        {
            //move
            velocity.z = moveSetting.forwardVel * forwardInput;
        }
        else
        {
            //zero velocity
            velocity.z = 0;
        }
        if (Mathf.Abs(strafeInput) > inputSetting.inputDelay)
        {
            //move
            velocity.x = moveSetting.strafeVel * strafeInput;
        }
        else
        {
            //zero velocity
            velocity.x = 0;
        }
        if (sprinting == true)
        {
            velocity.x *= sprintMod;
            velocity.z *= sprintMod;
        }
        else if (sneaking == true)
        {
            velocity.x *= sneakMod;
            velocity.z *= sneakMod;
        }
        if (velocity.x == 0 && velocity.z == 0)
        {
            anim.SetBool("CharSneakOn", false);
            anim.SetBool("CharRunOn", false);
            anim.SetBool("CharWalkOn", false);
        }
        else
        {
            if (sprinting)
            {
                anim.SetBool("CharSneakOn", false);
                anim.SetBool("CharRunOn", true);
                anim.SetBool("CharWalkOn", false);
            }
            else if (sneaking)
            {
                anim.SetBool("CharSneakOn", true);
                anim.SetBool("CharWalkOn", false);
                anim.SetBool("CharRunOn", false);
            }
            else
            {
                anim.SetBool("CharSneakOn", false);
                anim.SetBool("CharWalkOn", true);
                anim.SetBool("CharRunOn", false);
            }
        }
    }

    //void Turn()
    //{
    //    if(Mathf.Abs(strafeInput) > inputSetting.inputDelay)
    //    {
    //        targetRotation *= Quaternion.AngleAxis(moveSetting.strafeVel * strafeInput * Time.deltaTime, Vector3.up);
    //    }
        
    //    transform.rotation = targetRotation;
    //}
    
    void Jump()
    {
        if(jumpInput > 0 && Grounded() == true)
        {
            //jump
            velocity.y = moveSetting.jumpVel;
        }
        else if( jumpInput == 0 && Grounded() == true)
        {
            //zero out jump velocity
            velocity.y = 0;
        }
        else
        {
            //decrease y velocity in air, slowing then accelerating down
            velocity.y -= physSetting.downAccel;
        }

    }

    void ToggleSneaking()
    {
        if (!sprinting)
        {
            sneaking = !sneaking;
        }

    }

    void ToggleSprinting()
    {
        if (!sneaking)
        {
            sprinting = !sprinting;
        }
    }

    public float getLuminanceFactor(Light l, float distance)
    {
        return (l.range * l.intensity) / Mathf.Pow(distance, luminanceFalloff);
    }

    public IEnumerator checkLuminance()
    {
        //gets the layer number of the layer we set to ignore light
        const int ignoreLightLayer = 10;
        //this must be in binary.  ~ is bitwise not
        const int layerMask = ~(1<<ignoreLightLayer);

		//Sort luminance values in descending order
		SortedDictionary<float, int> luminanceFactors = new SortedDictionary<float, int>(Comparer<float>.Create((x, y) => y.CompareTo(x)));

		//for each light, determine if its paths are blocked to the player,
		Collider[] objectsInRadius = Physics.OverlapSphere(lightSetting.lightCheckArea.position, lightSetting.maxCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Collide);
		IEnumerable<Light> lightsInRadius = objectsInRadius.Select(collider => collider.GetComponentInChildren<Light>()).Where(light => light != null && light.enabled);
		int lightsProcessed = 0;
		foreach (Light l in lightsInRadius.OrderBy(light => (light.transform.position - lightSetting.lightCheckArea.position).magnitude))
        {
			++lightsProcessed;

			//Stop calculating if we exceed the max number of lights allowed to contribute to luminance
			if(lightsProcessed > lightSetting.maxLightsForLuminance)
			{
				break;
			}

            Vector3 rayDist = l.transform.position - lightSetting.lightCheckArea.position;
            //Debug.DrawRay(lightSetting.lightCheckArea.position, rayDist);
            //draw ray from lightsensor to light in question
            if (Physics.Raycast(lightSetting.lightCheckArea.position, rayDist.normalized, out RaycastHit hitInfo, lightSetting.maxCheckDistance, layerMask))
            {
                //Debug.Log("Ray collided with " + hitInfo.transform.position + ", Light is at " + l.transform.position);
                if (hitInfo.transform.position == l.transform.position)
                {
					float lf = getLuminanceFactor(l, rayDist.magnitude);
                    if (luminanceFactors.ContainsKey(lf))
					{
						luminanceFactors[lf] += 1;
					}
					else
					{
						luminanceFactors[lf] = 1;
					}
                }
            }

			//After luminanceCalcsPerFrame calculations, yield until unity's base loop picks it up on the next frame.
			if (lightsProcessed % lightSetting.luminanceCalcsPerFrame == 0)
			{
				yield return null;
			}
        }
		luminance = 0;
		float diminishVal = 1;
        foreach(KeyValuePair<float, int> lf in luminanceFactors)
		{
			luminance += (lf.Key * lf.Value) / diminishVal;
			diminishVal *= luminancDiminishingFactor;
		}
		luminance *= luminanceMultiplier;
		//Debug.Log("Luminance is " + luminance + " time: " + Time.time);

		yield return checkLuminance();
    }
}
