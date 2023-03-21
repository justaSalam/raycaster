using UnityEngine;

public class Collidable : MonoBehaviour//dont bully me, i dont know what to call the class >:(
{
    public string[] cmds; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

#if !UNITY_EDITOR
            for (int i = 0; i <= cmds.Length; i++)
            {
                Execute.ExecuteCommand(cmds[i]); //allows me to execute multiple commands
            }
#else
            Execute.ExecuteCommand("msg %username% unity editor build, cant execute");
#endif
        }
    }
}
