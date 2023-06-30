using UnityEngine;


public class AvAnimation : MonoBehaviour
{
    IAvatar avatar;

    // Start is called before the first frame update
    void Start()
    {
        avatar = new BryceAvatar();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        avatar.GetAvatarStateMachine().CurrentState.OnFixedUpdate();
    }
}
