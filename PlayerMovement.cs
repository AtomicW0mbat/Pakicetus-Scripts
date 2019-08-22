/* Player movement script written by Landon
 * 
 */
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	[SerializeField] float maxHeight = 30.0f;
    [SerializeField] public float baseMoveSpeed = 2.5f;
	float moveSpeed = 2.5f;
	float maxVerticalSpeed = 0.8f;
	float acceleration = 0.01f;
	float reverseAcceleration = 0.06f; // if trying to change direction
	float maxTurnSpeed = 0.5f;
	float turnAcceleration = 0.02f;
	float turnReverseAcceleration = 0.1f;
	float maxBankAngle = 20.0f;
	float bankSmooth = 0.05f;
    
	Rigidbody rig;

	float cameraHeight = 8.0f;
	float cameraSmooth = 0.1f;
	Vector2 cameraLimit = new Vector2(2, 10); // (Min, Max)
	//public Transform cameraTarget;
	Camera cam;
    [SerializeField] private UnityEngine.UI.Text altimeter = null;
    [SerializeField] private LayerMask altimeterLayer = 1;
    [SerializeField] private AudioSource terrainCollision = null;
    [SerializeField] private AudioSource enemyCollision = null;
    [SerializeField] private AudioSource fishCollision = null;
    [SerializeField] private AudioSource foodFishCollision = null;
    [SerializeField] private AudioSource boostSound = null;
    [SerializeField] private float boostForce = 200f;
    [SerializeField] private float altimeterHeightMultiplier = 10.0f;

    [Header("Collision Damage")]
    [SerializeField] private int environmentCollisionDamage = 10;

	public ParticleSystem forwardMoveParticles;
	public ParticleSystem boostParticles;

	void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Environment")
        {
            terrainCollision.Play();
        }

        if (other.gameObject.tag == "Fish")
        {
            fishCollision.Play();
        }

        if (other.gameObject.tag == "Enemy")
        {
            enemyCollision.Play();
        }

        if (rig.velocity.magnitude > 0.5f)
        {
            MainManager.Player.DamageHull((int)rig.velocity.magnitude * 10);
        }
    }
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FoodFish")
        {
            foodFishCollision.Play();
        }
    }

	void Start() {
		rig = GetComponent<Rigidbody>();
		//rig.constraints = RigidbodyConstraints.FreezeRotationX; // | RigidbodyConstraints.FreezePositionY
		cam = Camera.main;
		
	}
    
    void SetSpeed() // uses tech status to determine crew requirements
    {
        if (MainManager.Player.engineI)
        {
            moveSpeed = baseMoveSpeed * 1.5f;
        }
        else if (!MainManager.Player.engineI)
        {
            moveSpeed = baseMoveSpeed;
        }
    }

	void FixedUpdate()
    {
		Vector3 targetSpeed = Vector3.zero;
		float targetRotationSpeed = 0;
		float targetBankAngle = 0;

        SetSpeed();
        UpdateAltimeter();

        // To move, submarine must be in movment mode with at least one navigator and at least one crew member on engines
        // Movement speed is incresed with number of crew members on engines
        if (Input.GetButtonDown("Jump") && 
            MainManager.Player.GetMovement() && 
            MainManager.Player.boost.activeInHierarchy == true && 
            MainManager.Player.energy > MainManager.Player.boostEnergyCost)
        {
            MainManager.Player.ChangeEnergy(-MainManager.Player.boostEnergyCost);
            StartCoroutine(Boost(0.1f));
        }
        if (Input.GetKey(KeyCode.W) && 
            MainManager.Player.GetMovement())
        {
            targetSpeed += transform.forward * moveSpeed; //forward
        }
		if (Input.GetKey(KeyCode.S) && 
            MainManager.Player.GetMovement())
        {
            targetSpeed -= transform.forward * moveSpeed; //backward
        }
		if (Input.GetKey(KeyCode.E) &&
            MainManager.Player.GetMovement() && 
            transform.position.y < maxHeight)
        {
            targetSpeed += transform.up * maxVerticalSpeed; //up
        }
		if (Input.GetKey(KeyCode.Q) && 
            MainManager.Player.GetMovement())
        {
            targetSpeed -= transform.up * maxVerticalSpeed; //down
        }
		if (Input.GetKey(KeyCode.A) && 
            MainManager.Player.GetMovement())
        {
			targetRotationSpeed -= maxTurnSpeed; //turn left
			targetBankAngle += maxBankAngle;
		}
		if (Input.GetKey(KeyCode.D) && 
            MainManager.Player.GetMovement())
        {
			targetRotationSpeed += maxTurnSpeed; //turn right
			targetBankAngle -= maxBankAngle;
		}
		
		Vector3 moveDif = targetSpeed - rig.velocity;
		float acc = acceleration;
		if (moveDif.magnitude > moveSpeed)
			acc = reverseAcceleration;
		float rotDif = targetRotationSpeed - rig.angularVelocity.y;
		float turnAcc = turnAcceleration;
		if (rotDif > maxTurnSpeed)
			turnAcc = turnReverseAcceleration;
		
		Vector3 velocityChange = Vector3.ClampMagnitude(moveDif, acc);
		Vector3 directionChange = new Vector3(0.0f, Mathf.Clamp(rotDif, -turnAcc, turnAcc), 0.0f);

		rig.AddForce(velocityChange, ForceMode.VelocityChange);
        // Debug.Log(rig.velocity.magnitude);
		rig.AddTorque(directionChange, ForceMode.VelocityChange);

		float currentBank = transform.eulerAngles.z;
		if (currentBank > 180)
			currentBank -= 360;
		float bank = Mathf.Lerp(Mathf.Clamp(currentBank, -maxBankAngle, maxBankAngle), targetBankAngle, bankSmooth);
		transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, bank);


		//particles
		if (Input.GetKeyDown(KeyCode.W) && MainManager.Player.GetMovement())
			forwardMoveParticles.Play();
		if (Input.GetKeyUp(KeyCode.W))
			forwardMoveParticles.Stop();


		//camera
		float keyscroll = 0;
		cameraHeight = Mathf.Clamp(cameraHeight - Input.GetAxis("Mouse ScrollWheel") * cameraHeight, cameraLimit.x, cameraLimit.y);
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Z))
            keyscroll = -.02f;
        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.X))
            keyscroll = .02f;
        cameraHeight = Mathf.Clamp(cameraHeight + keyscroll * cameraHeight, cameraLimit.x, cameraLimit.y);

        Vector3 targetCameraPosition = transform.position - transform.forward * 2 + Vector3.up * cameraHeight;
		cam.transform.position = Vector3.Lerp(cam.transform.position, targetCameraPosition, cameraSmooth);
		cam.transform.LookAt(transform.position + transform.forward * 4.5f - Vector3.up * 3);
	}

    private IEnumerator Boost(float delay)
    {
        if(!boostSound.isPlaying)
        {
            boostSound.Play();
        }
        
        yield return new WaitForSeconds(delay);
        rig.AddForce(transform.forward * boostForce);
		boostParticles.Play();

    }

    [SerializeField] private GameObject altimeter_raycast_fire_transform;
    private void UpdateAltimeter()
    {
        // raycast down to find terrain, change altimeter to display height
        RaycastHit hit;
        if (Physics.Raycast(altimeter_raycast_fire_transform.transform.position, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, altimeterLayer))
        {
            altimeter.text = "0" + Mathf.Round(hit.distance*altimeterHeightMultiplier).ToString();
        }
    }
}