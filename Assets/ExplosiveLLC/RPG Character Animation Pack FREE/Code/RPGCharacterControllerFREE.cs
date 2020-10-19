using System.Collections;
using UnityEngine;

namespace RPGCharacterAnimsFREE
{
    public class RPGCharacterControllerFREE:MonoBehaviour
    {
        //Components.
        [HideInInspector] public RPGCharacterMovementControllerFREE rpgCharacterMovementController;
        [HideInInspector] public RPGCharacterWeaponControllerFREE rpgCharacterWeaponController;
        [HideInInspector] public RPGCharacterInputControllerFREE rpgCharacterInputController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public IKHandsFREE ikHands;
        public Weapon weapon = Weapon.UNARMED;
        public GameObject target;

        //Strafing/action.
        [HideInInspector] public bool isDead = false;
		[HideInInspector] public bool canBlock = true;
		[HideInInspector] public bool isBlocking = false;
        [HideInInspector] public bool canAction = true;
        [HideInInspector] public bool isStrafing = false;
        [HideInInspector] public bool injured;
        private float idleTimer;
        private float idleTrigger = 0f;

        public float animationSpeed = 1;

        #region Initialization

        private void Awake()
        {
			rpgCharacterMovementController = GetComponent<RPGCharacterMovementControllerFREE>();
            rpgCharacterWeaponController = GetComponent<RPGCharacterWeaponControllerFREE>();
            rpgCharacterInputController = GetComponent<RPGCharacterInputControllerFREE>();
			//Setup Animator, add AnimationEvents script.
			animator = GetComponentInChildren<Animator>();
			if(animator == null)
			{
				Debug.LogError("ERROR: There is no Animator component for character.");
				Destroy(this);
			}
			else
			{
				animator.gameObject.AddComponent<RPGCharacterAnimatorEventsFREE>();
				animator.GetComponent<RPGCharacterAnimatorEventsFREE>().rpgCharacterController = this;
				animator.gameObject.AddComponent<AnimatorParentMoveFREE>();
				animator.GetComponent<AnimatorParentMoveFREE>().anim = animator;
				animator.GetComponent<AnimatorParentMoveFREE>().rpgCharacterMovementController = rpgCharacterMovementController;
			}
            ikHands = GetComponent<IKHandsFREE>();
            //Set for starting Unarmed state.
            weapon = Weapon.UNARMED;
            animator.SetInteger("Weapon", 0);
            animator.SetInteger("WeaponSwitch", -1);
            StartCoroutine(_ResetIdleTimer());
			
		}

        private void Start()
        {
            rpgCharacterMovementController.SwitchCollisionOn();
        }

        #endregion

        #region Updates

        private void Update()
        {
            UpdateAnimationSpeed();
            if(rpgCharacterMovementController.MaintainingGround())
            {
                //Revive.
                if(rpgCharacterInputController.inputDeath)
                {
                    if(isDead)
                    {
                        Revive();
                    }
                }
                if(canAction)
                {
                    Blocking();
                    if(!isBlocking)
                    {
                        Strafing();
                        RandomIdle();
                        Rolling();
                        //Hit.
                        if(rpgCharacterInputController.inputLightHit)
                        {
                            GetHit();
                        }
                        //Death.
                        if(rpgCharacterInputController.inputDeath)
                        {
                            if(!isDead)
                            {
                                Death();
                            }
                            else
                            {
                                Revive();
                            }
                        }
                        //Attacks.
                        if(rpgCharacterInputController.inputAttackL)
                        {
                            Attack(1);
                        }
                        if(rpgCharacterInputController.inputAttackR)
                        {
                            Attack(2);
                        }
						if(rpgCharacterInputController.inputKickL)
						{
							AttackKick(1);
						}
						if(rpgCharacterInputController.inputKickR)
						{
							AttackKick(2);
						}
						if(rpgCharacterInputController.inputLightHit)
                        {
                            GetHit();
                        }
                        //Navmesh.
                        if(Input.GetMouseButtonDown(0))
                        {
                            if(rpgCharacterMovementController.useMeshNav)
                            {
                                RaycastHit hit;
                                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                                {
                                    rpgCharacterMovementController.navMeshAgent.destination = hit.point;
                                }
                            }
                        }
                    }
                }
            }
            //Injury toggle.
            if(Input.GetKeyDown(KeyCode.I))
            {
                if(injured == false)
                {
                    injured = true;
                    animator.SetBool("Injured", true);
                }
                else
                {
                    injured = false;
                    animator.SetBool("Injured", false);
                }
            }
            //Slow time toggle.
            if(Input.GetKeyDown(KeyCode.T))
            {
                if(Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
                else
                {
                    Time.timeScale = 0.25f;
                }
            }
            //Pause toggle.
            if(Input.GetKeyDown(KeyCode.P))
            {
                if(Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
                else
                {
                    Time.timeScale = 0f;
                }
            }
        }

        private void UpdateAnimationSpeed()
        {
            animator.SetFloat("AnimationSpeed", animationSpeed);
        }

        #endregion

        #region Combat

        /// <summary>
        /// Dodge the specified direction.
        /// </summary>
        /// <param name="1">Left</param>
        /// <param name="2">Right</param>
        public IEnumerator _Dodge(int direction)
        {
            animator.SetInteger("Action", direction);
            animator.SetTrigger("DodgeTrigger");
            Lock(true, true, true, 0, 0.55f);
            yield return null;
        }

        //0 = No side
        //1 = Left
        //2 = Right
        //3 = Dual
        //weaponNumber 0 = Unarmed
        //weaponNumber 1 = 2H Sword
        public void Attack(int attackSide)
        {
            int attackNumber = 0;
            //Unarmed.
            if(weapon == Weapon.UNARMED)
            {
                int maxAttacks = 3;
                //Left attacks.
                if(attackSide == 1)
                {
                    animator.SetInteger("AttackSide", 1);
					if(rpgCharacterWeaponController != null)
					{
						attackNumber = Random.Range(1, maxAttacks + 1);
					}
                }
                //Right attacks.
                else if(attackSide == 2)
                {
                    animator.SetInteger("AttackSide", 2);
					if(rpgCharacterWeaponController != null)
					{
						attackNumber = Random.Range(4, maxAttacks + 4);
					}
                }
                //Set the Locks.
                if(attackSide != 3)
                {
					if(rpgCharacterWeaponController != null)
					{
						if(rpgCharacterWeaponController.leftWeapon == 8 || rpgCharacterWeaponController.leftWeapon == 10 || rpgCharacterWeaponController.leftWeapon == 16
							|| rpgCharacterWeaponController.rightWeapon == 9 || rpgCharacterWeaponController.rightWeapon == 11 || rpgCharacterWeaponController.rightWeapon == 17)
						{
							Lock(true, true, true, 0, 0.75f);
						}
						else
						{
							//Dagger and Item has longer attack time.
							Lock(true, true, true, 0, 1f);
						}
					}
                }
            }
            else if(weapon == Weapon.TWOHANDSWORD)
            {
                int maxAttacks = 11;
                attackNumber = Random.Range(1, maxAttacks);
                Lock(true, true, true, 0, 1.1f);
            }
            else
            {
                int maxAttacks = 6;
                attackNumber = Random.Range(1, maxAttacks);
                if(weapon == Weapon.TWOHANDSWORD)
                {
                    Lock(true, true, true, 0, 0.85f);
                }
                else
                {
                    Lock(true, true, true, 0, 0.75f);
                }
            }
            //Trigger the animation.
            animator.SetInteger("Action", attackNumber);
            if(attackSide == 3)
            {
                animator.SetTrigger("AttackDualTrigger");
            }
            else
            {
                animator.SetTrigger("AttackTrigger");
            }
        }

        public void AttackKick(int kickSide)
        {
            animator.SetInteger("Action", kickSide);
            animator.SetTrigger("AttackKickTrigger");
            Lock(true, true, true, 0, 0.9f);
        }

        public void Blocking()
        {
			if(canBlock)
			{
				if(!isBlocking)
				{
					if(rpgCharacterInputController.HasBlockInput() )
					{
						isBlocking = true;
						animator.SetBool("Blocking", true);
						rpgCharacterMovementController.canMove = false;
						animator.SetTrigger("BlockTrigger");
					}
				}
				else
				{
					if(!rpgCharacterInputController.HasBlockInput())
					{
						isBlocking = false;
						animator.SetBool("Blocking", false);
						rpgCharacterMovementController.canMove = true;
					}
				}
			}
		}

        private void Strafing()
        {
            if(rpgCharacterInputController.HasTargetInput())
            {
                animator.SetBool("Strafing", true);
                isStrafing = true;
            }
            else
            {
                isStrafing = false;
                animator.SetBool("Strafing", false);
            }
        }

        private void Rolling()
        {
            if(!rpgCharacterMovementController.isRolling)
            {
                if(rpgCharacterInputController.inputRoll)
                {
                    rpgCharacterMovementController.DirectionalRoll();
                }
            }
        }

        public void GetHit()
        {
            int hits = 5;
            if(isBlocking)
            {
                hits = 2;
            }
            int hitNumber = Random.Range(1, hits + 1);
            animator.SetInteger("Action", hitNumber);
            animator.SetTrigger("GetHitTrigger");
            Lock(true, true, true, 0.1f, 0.4f);
            if(isBlocking)
            {
                StartCoroutine(rpgCharacterMovementController._Knockback(-transform.forward, 3, 3));
                return;
            }
            //Apply directional knockback force.
            if(hitNumber <= 1)
            {
                StartCoroutine(rpgCharacterMovementController._Knockback(-transform.forward, 8, 4));
            }
            else if(hitNumber == 2)
            {
                StartCoroutine(rpgCharacterMovementController._Knockback(transform.forward, 8, 4));
            }
            else if(hitNumber == 3)
            {
                StartCoroutine(rpgCharacterMovementController._Knockback(transform.right, 8, 4));
            }
            else if(hitNumber == 4)
            {
                StartCoroutine(rpgCharacterMovementController._Knockback(-transform.right, 8, 4));
            }
        }

        public void Death()
        {
            animator.SetTrigger("DeathTrigger");
            Lock(true, true, false, 0.1f, 0f);
            isDead = true;
        }

        public void Revive()
        {
            animator.SetTrigger("ReviveTrigger");
            Lock(true, true, true, 0f, 1f);
            isDead = false;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Keep character from doing actions.
        /// </summary>
        private void LockAction()
        {
            canAction = false;
        }

        /// <summary>
        /// Let character move and act again.
        /// </summary>
        private void UnLock(bool movement, bool actions)
        {
            StartCoroutine(_ResetIdleTimer());
            if(movement)
            {
                rpgCharacterMovementController.UnlockMovement();
            }
            if(actions)
            {
                canAction = true;
            }
        }

        #endregion

        #region Misc

        /// <summary>
        /// Plays random idle animation. Currently only Alert1 animation.
        /// </summary>
        private void RandomIdle()
        {
			if(rpgCharacterWeaponController != null)
			{
				if(!rpgCharacterMovementController.isMoving && !rpgCharacterWeaponController.isWeaponSwitching && rpgCharacterMovementController.canMove)
				{
					idleTimer += 0.01f;
					if(idleTimer > idleTrigger)
					{
						//Turn off IK Hands.
						if(ikHands != null)
						{
							ikHands.canBeUsed = false;
						}
						animator.SetInteger("Action", 1);
						animator.SetTrigger("IdleTrigger");
						StartCoroutine(_ResetIdleTimer());
						Lock(true, true, true, 0, 1.25f);
						if(ikHands != null)
						{
							ikHands.canBeUsed = true;
						}
					}
				}
			}
        }

        private IEnumerator _ResetIdleTimer()
        {
            idleTrigger = Random.Range(5f, 15f);
            idleTimer = 0;
            yield return new WaitForSeconds(1f);
            animator.ResetTrigger("IdleTrigger");
        }

        private IEnumerator _GetCurrentAnimationLength()
        {
            yield return new WaitForEndOfFrame();
            float f = animator.GetCurrentAnimatorClipInfo(0).Length;
            Debug.Log(f);
        }

        /// <summary>
        /// Lock character movement and/or action, on a delay for a set time.
        /// </summary>
        /// <param name="lockMovement">If set to <c>true</c> lock movement.</param>
        /// <param name="lockAction">If set to <c>true</c> lock action.</param>
        /// <param name="timed">If set to <c>true</c> timed.</param>
        /// <param name="delayTime">Delay time.</param>
        /// <param name="lockTime">Lock time.</param>
        public void Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            StopCoroutine("_Lock");
            StartCoroutine(_Lock(lockMovement, lockAction, timed, delayTime, lockTime));
        }

        //Timed -1 = infinite, 0 = no, 1 = yes.
        public IEnumerator _Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            if(delayTime > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }
            if(lockMovement)
            {
                rpgCharacterMovementController.LockMovement();
            }
            if(lockAction)
            {
                LockAction();
            }
            if(timed)
            {
                if(lockTime > 0)
                {
                    yield return new WaitForSeconds(lockTime);
                }
                UnLock(lockMovement, lockAction);
            }
        }

        /// <summary>
        /// Sets the animator state.
        /// </summary>
        /// <param name="weapon">Weapon.</param>
        /// <param name="weaponSwitch">Weapon switch.</param>
        /// <param name="Lweapon">Lweapon.</param>
        /// <param name="Rweapon">Rweapon.</param>
        /// <param name="weaponSide">Weapon side.</param>
        private void SetAnimator(int weapon, int weaponSwitch, int Lweapon, int Rweapon, int weaponSide)
        {
            Debug.Log("SETANIMATOR: Weapon:" + weapon + " Weaponswitch:" + weaponSwitch + " Lweapon:" + Lweapon + " Rweapon:" + Rweapon + " Weaponside:" + weaponSide);
            //Set Weapon if applicable.
            if(weapon != -2)
            {
                animator.SetInteger("Weapon", weapon);
            }
            //Set WeaponSwitch if applicable.
            if(weaponSwitch != -2)
            {
                animator.SetInteger("WeaponSwitch", weaponSwitch);
            }
            //Set left weapon if applicable.
            if(Lweapon != -1)
            {
				if(rpgCharacterWeaponController != null)
				{
					rpgCharacterWeaponController.leftWeapon = Lweapon;
				}
                animator.SetInteger("LeftWeapon", Lweapon);
            }
            //Set right weapon if applicable.
            if(Rweapon != -1)
            {
				if(rpgCharacterWeaponController != null)
				{
					rpgCharacterWeaponController.rightWeapon = Rweapon;
				}
                animator.SetInteger("RightWeapon", Rweapon);
            }
            //Set weapon side if applicable.
            if(weaponSide != -1)
            {
                animator.SetInteger("LeftRight", weaponSide);
            }
            SetWeaponState(weapon);
        }

        public void SetWeaponState(int weaponNumber)
        {
			if(rpgCharacterWeaponController != null)
			{
				if(weaponNumber == 0)
				{
					weapon = Weapon.UNARMED;
				}
				else if(weaponNumber == 1)
				{
					weapon = Weapon.TWOHANDSWORD;
				}
			}
        }

        public IEnumerator _BlockBreak()
        {
            animator.SetTrigger("BlockBreakTrigger");
			Lock(true, true, true, 0, 1f);
			yield return null;
		}

        public void AnimatorDebug()
        {
            Debug.Log("ANIMATOR SETTINGS---------------------------");
            Debug.Log("Moving: " + animator.GetBool("Moving"));
            Debug.Log("Strafing: " + animator.GetBool("Strafing"));
            Debug.Log("Blocking: " + animator.GetBool("Blocking"));
            Debug.Log("Injured: " + animator.GetBool("Injured"));
            Debug.Log("Weapon: " + animator.GetInteger("Weapon"));
            Debug.Log("WeaponSwitch: " + animator.GetInteger("WeaponSwitch"));
            Debug.Log("LeftRight: " + animator.GetInteger("LeftRight"));
            Debug.Log("LeftWeapon: " + animator.GetInteger("LeftWeapon"));
            Debug.Log("RightWeapon: " + animator.GetInteger("RightWeapon"));
            Debug.Log("AttackSide: " + animator.GetInteger("AttackSide"));
            Debug.Log("Jumping: " + animator.GetInteger("Jumping"));
            Debug.Log("Action: " + animator.GetInteger("Action"));
            Debug.Log("Velocity X: " + animator.GetFloat("Velocity X"));
            Debug.Log("Velocity Z: " + animator.GetFloat("Velocity Z"));
        }

        #endregion

    }
}