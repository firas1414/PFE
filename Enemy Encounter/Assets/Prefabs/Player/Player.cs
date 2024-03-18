using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] JoyStick moveStick; //Created an instance of JoyStick class
    [SerializeField] JoyStick aimStick;
    [SerializeField] CharacterController characterController;
    [SerializeField] CameraController cameraController; 
    [SerializeField] float moveSpeed = 20f;
    [SerializeField] float  turnSpeed = 30f;
    [SerializeField] float  AnimturnSpeed = 30f;
    [Header("Inventory")]
    [SerializeField] InventoryComponent inventoryComponent;
    
    Vector2 moveStickUpdated; // MoveStick value
    Vector2 aimInput;
    Camera mainCam;
    Animator animator ;
    float animatorTurnSpeed;


    // Start is called before the first frame update
    void Start()
    {
     moveStick.OnStickValueUpdated += GetmoveStickUpdated; // Each time the stick value gets updated, the GetmoveStickUpdated function will be called
     aimStick.OnStickValueUpdated +=aimStickUpdated; // Each time the stick value gets updated, the aimStickUpdated function will be called
     aimStick.onStickTaped += StartSwitchWeapon; // Each time the aim stick gets tapped, the StartSwitchWeapon function will be called
     mainCam = Camera.main;
     animator=GetComponent<Animator>();
    
     /*
     When you do joystick_1.event_name += function_name, you are subscribing function_name to the event_name.
     This means that when event_name is raised or triggered, function_name will be called.
     "Hey, whenever the OnStickValueUpdated event happens in the joystick_1 instance, call the GetmoveStickUpdated function."
     */
    }


    void GetmoveStickUpdated(Vector2 inputValue)
    {
        moveStickUpdated = inputValue;
    }


    public void AttackPoint(){
        inventoryComponent.GetActiveWeapon().Attack();
    }


    void StartSwitchWeapon(){
        animator.SetTrigger("switchWeapon");
    }
    public void SwitchWeapon(){
        inventoryComponent.NextWeapon();
    }

    //turn the 2D direction to 3D direction (calculation)
    Vector3 StickInputToWorldDirection (Vector2 inputValue) {
        Vector3 x_axis = mainCam.transform.right;
        Vector3 z_axis = Vector3.Cross(x_axis, Vector3.up);
        return x_axis * inputValue.x + z_axis * inputValue.y ; //convert the input to the world direction
        
    }

    void aimStickUpdated(Vector2 inputValue){
        aimInput = inputValue;
        //switch to attacking animations
        if(aimInput.magnitude > 0){
            animator.SetBool("attacking",true);

        }else{
            animator.SetBool("attacking",false);
        }
     }


    // Update is called once per frame
    void Update()
    {
        PerformMoveAndAim();
        UpdateCamera();

    }

// make this function just to clear the code
    private void PerformMoveAndAim(){
        Vector3 MoveDir= StickInputToWorldDirection(moveStickUpdated) ; //control the move direction
        characterController.Move(MoveDir* Time.deltaTime * moveSpeed); //Move the character
      
        UpdateAim(MoveDir);
        
        // change animation based on MoveDirection (MoveDir)
        //when we aim we use the aim direction to set the animation not the move direction
        //how much we are moving forward and right (back= forward -1) (calculatated via DOT product) 
        float forward = Vector3.Dot(MoveDir,transform.forward);
        float right = Vector3.Dot(MoveDir,transform.right);
        animator.SetFloat("forwardSpeed",forward);
        animator.SetFloat("rightSpeed",right);
        

    }

    private void  UpdateAim(Vector3 currentMoveDir){
        Vector3 AimDir= currentMoveDir ; //control the aim direction
        //ckech if the player is trying to aim (if we are not aiming aim is where we are moving)
        if(aimInput.magnitude != 0){
            AimDir= StickInputToWorldDirection(aimInput);

        }
        RotationTowards(AimDir);
    }

    private void UpdateCamera(){
        // don't update camera direction while aiming (if the player move and don't aim)
        if (moveStickUpdated.magnitude != 0 && aimInput.magnitude == 0 && cameraController != null) 
        {
            
            cameraController.AddYawInput(moveStickUpdated.x);
            

        }
    }

    private void RotationTowards(Vector3 AimDir){
        
          // go back to 0 if we are not aiming
           float currentTurnSpeed=0;
        if(AimDir.magnitude != 0){
            
            
           
            //save previous rotation to calculate rotationsped
            Quaternion prevRot=transform.rotation ;
            // we want some animation when the player move from looking up to looking down instantly (rotaion progress => lerp to the rotaion instead of turn to it lerp=turn from one rotation to an other with alpha)
            // we want the player to aim in a direction while moving to another direction when he is attacked by the enemeies
            //when you aim the direction of the player is controlled by the aim not the move (aim independent from the move)
            float turnLerpAlpha = turnSpeed* Time.deltaTime; //calculating alpha
            transform.rotation=Quaternion.Lerp( transform.rotation,Quaternion.LookRotation(AimDir,Vector3.up), turnLerpAlpha);//player aim the direction (vector3.up to direct the head of the player to the direction)
            Quaternion currentRot=transform.rotation ;

            //figure out the direction
            float direction =Vector3.Dot(AimDir,transform.right) > 0 ? 1 : -1; //if >0=1 if not -1
            // diffrence between current rotation and the prev one (use Quaternion with mem variables)
            float rotaionDelta = Quaternion.Angle(prevRot,currentRot) * direction;

           

            currentTurnSpeed=rotaionDelta / Time.deltaTime;
            
             
        }

           // lerp this value because the turn speed is high and the player start chaking
           animatorTurnSpeed = Mathf.Lerp(animatorTurnSpeed,currentTurnSpeed , Time.deltaTime * AnimturnSpeed );
           animator.SetFloat("turnSpeed",animatorTurnSpeed );

    }
    
        
}