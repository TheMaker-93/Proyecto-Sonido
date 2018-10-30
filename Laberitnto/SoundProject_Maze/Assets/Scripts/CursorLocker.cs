using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLocker : MonoBehaviour {

    public KeyCode m_DebugAimLocking = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;

    private bool m_AimLocked;
    private bool m_CursorLocked;


    public bool GetAimLocked()
    {
        return m_AimLocked;
    }

    private void Awake()
    {

        Cursor.lockState = CursorLockMode.Locked;
        m_CursorLocked = true;
        Cursor.visible = false;
        m_AimLocked = true;

    }


    // para que esto solo funcine en el editor y no se compile en las builds
#if UNITY_EDITOR

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(m_DebugAimLocking))
        {
            //m_AimLocked = !m_AimLocked;
            LockAim(!m_AimLocked);
        }

        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                SetCursorFree();
            }
            else
            {
                SetCursorLocked();
            }

        }

    }
#endif


    public void SetCursorFree()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void SetCursorLocked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_CursorLocked = Cursor.lockState == CursorLockMode.Locked;
    }
    public void LockAim(bool _status)
    {
        m_AimLocked = _status;
    }


}
