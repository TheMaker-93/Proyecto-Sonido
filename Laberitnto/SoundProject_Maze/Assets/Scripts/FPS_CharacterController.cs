using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CursorLocker))]
public class FPS_CharacterController : MonoBehaviour {

    [SerializeField] private CursorLocker curslorLocker;
    [SerializeField] private Player player;

    // Rotacion
    float m_Yaw;                    // current jaw
    float m_Pitch;                  // current pitch
    public float m_jawRotationSpeed = 160f;         // rotacion en grados por segundo
    public float m_PitchRotationalSpeed = 160f;
    public float m_MinPitch = - 80;
    public float m_MaxPitch = 50;
    public Transform m_PitchControllerTransform;
    public bool invertedYaw = false;
    public bool invertedPitch = true;

    // translacion

    CharacterController m_CharacterController;
    public float m_Speed = 10.0f;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;

    // correr
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public float m_FastSpeedMultiplier = 1.2f;
    public float m_JumpSpeed = 10.0f;

    // gravedad
    float m_VerticalSpeed = 0.0f;
    bool m_OnGround = false;


    void Awake()
    {
        if (curslorLocker == false) curslorLocker = GetComponent<CursorLocker>();
        if (player == null)         player = GetComponent<Player>();

        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;

        // accedemos al componente del character ontroller
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Solo si estamos vivos podremos movernos
  // Rotacion de la camara
        if (curslorLocker.GetAimLocked() == true)
        {
            CameraRotation();
        }

        // movimiento
        Move();
        


    }

    private void CameraRotation ()
    {
        // MOVIMIENTO EN PITCH (EJE X) ------------------------------------------ //
        //… Guardams el input del raton  y se lo aplicamos a la variable pitch que tenemos en nuestro script
        float l_MouseAxisY = Input.GetAxis("Mouse Y");

        // utilizamos la boleana para evitar que el movimiento vaya al reves
        if (invertedPitch) l_MouseAxisY = -l_MouseAxisY;

        // calculamos el pitch y lo limitamos 
        m_Pitch += l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);             // si el valor supera los limites lo steamos al limite que supera


        // MOVIMIENTO EN JAW (EJE Y) ------------------------------------------ //

        float l_MouseAxisX = Input.GetAxis("Mouse X");

        // calculamos la nueva posicion del yaw
        if (invertedYaw) l_MouseAxisX = -l_MouseAxisX;
        // calculamos el yaw
        m_Yaw += l_MouseAxisX * m_jawRotationSpeed * Time.deltaTime;

        // ---------------------------------------------------------------------- //

        //… Aplicamos la rotacion al objeo padre (rotacion en la y) y en la  el jaw
        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);


    }

    private void Move ()
    {

        //…
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;           
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_Movement = new Vector3();

        // aqui aplicamos las for3mulas de v = (seno alfa, 0 , cos alfa) siendo alfa el angulo de la y 
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));

        // Player Inputs
        if (Input.GetKey(m_UpKeyCode))
            l_Movement = l_Forward;
        else if (Input.GetKey(m_DownKeyCode))
            l_Movement = -l_Forward;
        if (Input.GetKey(m_RightKeyCode))
            l_Movement += l_Right;
        else if (Input.GetKey(m_LeftKeyCode))
            l_Movement -= l_Right;

        l_Movement.Normalize();                     // normalizamos para que se trate de n vector director con valores enter 0 y 1
        

        // -- SPRINT ------------------------------------------------------- //
        //… el multiplcicado rde velocdiad ahora mismo auugmenta nestra velocidad en un 50 por ciento
        float l_SpeedMultiplier = 1.0f;
        if (Input.GetKey(m_RunKeyCode))
            l_SpeedMultiplier = m_FastSpeedMultiplier;

        // solo movemos si no estamos bloqueados
        l_Movement *= Time.deltaTime * m_Speed * l_SpeedMultiplier;


        // APLICAMOS LA GRAVEDAD -------------------------------------------- //

        //… Applicamos la gravedad a la variabl ede velocidad verticals
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_OnGround = true;
            m_VerticalSpeed = 0.0f;
        }
        else
            m_OnGround = false;

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;


        //…SALTO ---------------------------------- 77
        if (m_OnGround && Input.GetKeyDown(m_JumpKeyCode))
            m_VerticalSpeed = m_JumpSpeed;




    }



}


