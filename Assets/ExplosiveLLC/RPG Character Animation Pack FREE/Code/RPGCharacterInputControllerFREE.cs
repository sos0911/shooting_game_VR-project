using UnityEngine;

namespace RPGCharacterAnimsFREE
{
    public class RPGCharacterInputControllerFREE:MonoBehaviour
    {
        //Inputs.
        [HideInInspector] public bool inputJump;
        [HideInInspector] public bool inputLightHit;
        [HideInInspector] public bool inputDeath;
        [HideInInspector] public bool inputAttackL;
        [HideInInspector] public bool inputAttackR;
		[HideInInspector] public bool inputKickL;
		[HideInInspector] public bool inputKickR;
		[HideInInspector] public float inputBlock = 0;
		[HideInInspector] public float inputTarget = 0;
		[HideInInspector] public bool inputTargetKey;
		[HideInInspector] public float inputHorizontal = 0;
        [HideInInspector] public float inputVertical = 0;
        [HideInInspector] public bool inputRoll;
        [HideInInspector] public bool inputRelax;

        //Variables.
        [HideInInspector] public bool allowedInput = true;
        [HideInInspector] public Vector3 moveInput;
        [HideInInspector] public Vector2 aimInput;

        /// <summary>
        /// Input abstraction for easier asset updates using outside control schemes.
        /// </summary>
        private void Inputs()
        {
            inputJump = Input.GetButtonDown("Jump");
            inputLightHit = Input.GetButtonDown("LightHit");
            inputDeath = Input.GetButtonDown("Death");
            inputAttackL = Input.GetButtonDown("AttackL");
            inputAttackR = Input.GetButtonDown("AttackR");
			inputKickL = Input.GetButtonDown("KickL");
			inputKickR = Input.GetButtonDown("KickR");
			inputBlock = Input.GetAxisRaw("Block");
			inputTarget = Input.GetAxisRaw("Target");
			inputTargetKey = Input.GetButton("TargetKey");
			inputHorizontal = Input.GetAxisRaw("Horizontal");
            inputVertical = Input.GetAxisRaw("Vertical");
            inputRoll = Input.GetButtonDown("L3");
            inputRelax = Input.GetButtonDown("Relax");
        }

        private void Awake()
        {
            allowedInput = true;
        }

        private void Update()
        {
            Inputs();
            moveInput = CameraRelativeInput(inputHorizontal, inputVertical);
			HasJoystickConnected();
        }

        /// <summary>
        /// Movement based off camera facing.
        /// </summary>
        private Vector3 CameraRelativeInput(float inputX, float inputZ)
        {
            //Forward vector relative to the camera along the x-z plane.
            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            //Right vector relative to the camera always orthogonal to the forward vector.
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            Vector3 relativeVelocity = inputHorizontal * right + inputVertical * forward;
            //Reduce input for diagonal movement.
            if(relativeVelocity.magnitude > 1)
            {
                relativeVelocity.Normalize();
            }
            return relativeVelocity;
        }

        public bool HasAnyInput()
        {
            if(allowedInput && moveInput != Vector3.zero && aimInput != Vector2.zero && inputJump != false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasMoveInput()
        {
            if(allowedInput && moveInput != Vector3.zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasAimInput()
        {
            if(allowedInput && ((aimInput.x < -0.8f || aimInput.x > 0.8f) || (aimInput.y < -0.8f || aimInput.y > 0.8f)))
            {
                return true;
            }
            else
            {
				return false;
            }
        }

		public bool HasJoystickConnected()
		{
			//No joysticks.
			if(Input.GetJoystickNames().Length == 0)
			{
				//Debug.Log("No Joystick Connected");
				return false;
			}
			else
			{
				//Debug.Log("Joystick Connected");
				//If joystick is plugged in.
				for(int i = 0; i < Input.GetJoystickNames().Length; i++)
				{
					//Debug.Log(Input.GetJoystickNames()[i].ToString());
				}
				return true;
			}
		}

		public bool HasBlockInput()
		{
			if(inputBlock > 0.8)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool HasTargetInput()
		{
			if(inputTarget < -0.8 || inputTargetKey)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}