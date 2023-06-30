# Random Avatar Animation e State Pattern in Unity

### Introduzione
Nel dominio di uno spazio virtuale che oggi chiamiamo, ad esempio, **Metaverso**, vive un concetto caratteristico: quello di **Avatar**.

Un avatar è una rappresentazione digitale di un essere vivente in uno spazio virtuale.

Ovviamente gli avatar sono animati e le loro animazioni sono gestite, ad esempio in Unity, con macchine a stati finiti implementate in un **Animator Controller**.

Oggi esistono molte tipologie di animazione e, per farsene un'idea, basta andare  a consultare il sito https://www.mixamo.com/ dove è possibile scaricarle gratuitamente per poi gestirle come stati autonomi, con relative transizioni, all'interno dell'Animator Controller in Unity.

Gli stati di animazione, sono stati a "_grana fine_", relativi appunto all'animazione dell'avatar. Possiamo, ad esempio, avere gli stati **Start Walking**, **Stop Walking**, **Walking** ecc. e, giocando con le transizioni di stato, combinare questi stati come vogliamo.

L'avatar, però, è un concetto di dominio, e non sempre è possibile associare con una relazione 1:1 lo stato dell'avatar con uno stato della sua animazione. Ad esempio, può essere per nulla rilevante avere uno stato dell'avatar come **Start Walking** o **Stop Walking** ma potrebbe essere invece rilevante avere uno stato di dominio dell'avatar semplicemente **Walking** e, in quello stato, avere tutte le possibili combinazioni di animazioni di tipo **Start Walking**, **Stop Walking**, **Walking**.

Se devo controllare che il mio avatar non vada a sbattere contro un muro quello che importa è che lo faccia mentre l'avatar è nello stato di dominio **Walking**, poco importa se sta iniziando o finendo di camminare, altrimenti rischierei di dover fare una serie interminabile di if-else sparsi nel codice in tutti gli stati di animazione rilevanti per quel controllo.

A questo proposito, quindi, ho provato a _disaccoppiare_ gli stati di dominio dagli stati di animazione utilizzando il buon vecchio **State Pattern**.

In questo semplice demo in cui un avatar di nome **Bryce** viene fatto muovere in maniera random, cercando il percorso più breve, gli stati di dominio sono gestiti tramite uno **State Pattern** che incapsula al suo interno i **trigger** necessari ai cambiamenti di stato dell'animazione da inviare all'Animator Controller di Unity.

Per questo demo ho utilizzato il **Character** e le **Animation** gratuite dal sito mixamo citato sopra, istruendo opportunamenet Unity a gestire direttamente le animazioni importate per lo spostamento della posizione nello spazio tridimensionale tramite **Apply Root Motion**.

### Codice
Per la macchina a stati ho creato una classe **StateMachine** che incapsula lo stato corrente e permette l'inizializzazione e il cambiamento di stato.

```cs
public class StateMachine
{
    private State actualState = null;

    public State CurrentState
    {
        get
        {
            return actualState;
        }
        private set
        {
            actualState = value;
        }
    }

    public void Init(State state)
    {
        CurrentState = state;
        state.OnEnter();
    }

    public void ChangeState(State state)
    {
        CurrentState.OnExit();
        CurrentState = state;
        state.OnEnter();
    }
}
```

Ogni stato dell'avatar deriva da una classe astratta **State** che incapsula una interfaccia **IAvatar** in modo da evitare dipendenze cicliche tra avatar e stato. In questo modo, ogni stato internamente può riferirsi all'avatar solo tramite il protocollo stabilito dall'interfaccia **IAvatar**.

```cs
public abstract class State
{
    protected IAvatar avatar;
    protected enum AvatarState { IDLE, WALKING, RUNNING, ENDRUNNING, ENDWALKING, TURNING }

    protected State(IAvatar avatar)
    {
        this.avatar = avatar;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnInput()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnFixedUpdate()
    {
        avatar.DrawRays();
    }
}
```

L'interfaccia **IAvatar** definisce il protocollo.

```cs
public interface IAvatar
{
    public Transform GetAvatarTransform();
    public Animator GetAvatarAnimator();
    public StateMachine GetAvatarStateMachine();
    public float GetShortDistance();
    public float GetMovementDistance();
    public void CalculateShortDistance();
    public void CalculateMovementDistance();
    public void CalculateForwardDirection();
    public void DrawRays();
    public void ChangeState(int state);   
}
```

Lo stato di dominio **Walking**, ad esempio, implementa **IAvatar** come segue. 

```cs
public class WalkingState : State
{
    public WalkingState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnEnter()
    {
        avatar.GetAvatarAnimator().SetTrigger("StartWalk");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        avatar.CalculateMovementDistance();

        if (avatar.GetShortDistance() > 11f)
        {
            avatar.CalculateForwardDirection();

            avatar.ChangeState((int)AvatarState.RUNNING);
        }
        else
        {
            if (avatar.GetMovementDistance() < 2.8f)
            {
                avatar.ChangeState((int)AvatarState.ENDWALKING);
            }
        }
    }
}
```

Per avere maggior precisione nell'aggiornamento di tempi e distanze ho usato **FixedUpdate**, normalmente usato per le simulazioni di fisica, che permette un frame-rate fisso a 0.02 s.
In questo modo è gestito meglio il tempo delle rotazioni random nello stato **Turning**.

```cs
public class TurningState : State
{
    public float timer = 0.0f;
    public int turnNumber = 0;

    public TurningState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnEnter()
    {
        turnNumber = Random.Range(0, 2);
        timer = 0.0f;

        avatar.GetAvatarAnimator().SetTrigger("LeftTurn90");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        timer += Time.deltaTime;

        if ((turnNumber > 0) && (timer >= 1.8f))
        {
            turnNumber--;
            timer = 0.0f;
            avatar.GetAvatarAnimator().SetTrigger("LeftTurn90");
        }

        if ((turnNumber == 0) && (timer >= 1.8f))
        {
            avatar.CalculateShortDistance();

            if (avatar.GetShortDistance() > 5.0f)
            {
                avatar.CalculateForwardDirection();

                avatar.ChangeState((int)AvatarState.WALKING);
            }
            else
            {
                turnNumber = 1;
                timer = 10.0f;
            }
        }
    }
}
```

La classe **BryceAvatar** implementa l'avatar incapsulando il **Character**, l'**Animator** e la **StateMachine** realizzando l'interfaccia **IAvatar** necessaria alla comunicazione con gli stati.

```cs
public class BryceAvatar : IAvatar
{
    private Transform bryceTransform = null;
    private Animator animator = null;

    private StateMachine avatarStateMachine = null;

    public enum AvatarState { IDLE, WALKING, RUNNING, ENDRUNNING, ENDWALKING, TURNING }

    private IdleState idleState;
    private WalkingState walkingState;
    private RunningState runningState;
    private EndRunningState endRunningState;
    private EndWalkingState endWalkingState;
    private TurningState turningState;

    private float maxDistance = 100.0f;
    private float shortDist = 1.0f * Mathf.Pow(10, 6);
    private Vector3 shortDistHP = Vector3.zero;
    private bool firstTime = true;
    private float movementDistance = 0.0f;


    public BryceAvatar()
    {
        bryceTransform = GameObject.Find("Bryce").transform;
        animator = bryceTransform.GetComponent<Animator>();

        avatarStateMachine = new StateMachine();

        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        runningState = new RunningState(this);
        endRunningState = new EndRunningState(this);
        endWalkingState = new EndWalkingState(this);
        turningState = new TurningState(this);

        avatarStateMachine.Init(idleState);
    }

    public Transform GetAvatarTransform()
    {
        return bryceTransform;
    }

    public Animator GetAvatarAnimator()
    {
        return animator;
    }

    public StateMachine GetAvatarStateMachine()
    {
        return avatarStateMachine;
    }

    public float GetShortDistance()
    {
        return shortDist;
    }

    public float GetMovementDistance()
    {
        return movementDistance;
    }

    public void CalculateShortDistance()
    {
        shortDist = 0.0f;
        shortDistHP = Vector3.zero;
        firstTime = true;

        ComputeShortDistance();
    }

    public void CalculateMovementDistance()
    {
        ComputeMovementDistance();
    }

    public void CalculateForwardDirection()
    {
        Vector3 hitPoint = shortDistHP;
        hitPoint.y = 0.0f;
        Vector3 direction = hitPoint - bryceTransform.position;

        bryceTransform.forward = direction;

        Quaternion rotation = Quaternion.AngleAxis(1.0f, bryceTransform.up);
        Vector3 forward = rotation * bryceTransform.forward;

        bryceTransform.forward = forward;
    }

    public void DrawRays()
    {
        DrawDistanceRays();
    }

    public void ChangeState(int state)
    {
        switch (state)
        {
            case (int)AvatarState.IDLE:
                avatarStateMachine.ChangeState(idleState);

                return;

            case (int)AvatarState.WALKING:
                avatarStateMachine.ChangeState(walkingState);

                return;

            case (int)AvatarState.RUNNING:
                avatarStateMachine.ChangeState(runningState);

                return;

            case (int)AvatarState.ENDRUNNING:
                avatarStateMachine.ChangeState(endRunningState);

                return;
            case (int)AvatarState.ENDWALKING:
                avatarStateMachine.ChangeState(endWalkingState);

                return;

            case (int)AvatarState.TURNING:
                avatarStateMachine.ChangeState(turningState);

                return;

            default:
                return;
        }
    }

    private void ComputeShortDistance()
    {
        float increment = 0.0f;
        Vector3 rayOrigin = bryceTransform.position;
        rayOrigin.y = 0.2f;

        for (int i = 0; i < 40; i++, increment += 0.5f)
        {
            Quaternion rotation = Quaternion.AngleAxis(-10.0f + increment, bryceTransform.up);
            Vector3 forward = rotation * bryceTransform.forward;
            Ray ray = new Ray(rayOrigin, forward.normalized);

            RaycastHit hitPoint;

            if (Physics.Raycast(ray, out hitPoint, maxDistance))
            {
                Vector3 rayDirection = hitPoint.point - rayOrigin;

                if (firstTime)
                {
                    shortDist = rayDirection.magnitude;
                    shortDistHP = hitPoint.point;
                    shortDistHP.y = 0.4f;
                    firstTime = false;
                }
                else
                {
                    if (rayDirection.magnitude < shortDist)
                    {
                        shortDist = rayDirection.magnitude;
                        shortDistHP = hitPoint.point;
                        shortDistHP.y = 0.4f;
                    }
                }
            }
        }
    }

    public void ComputeMovementDistance()
    {
        Vector3 lineOrigin = bryceTransform.position;
        lineOrigin.y = 0.4f;

        if (shortDistHP != Vector3.zero)
        {
            Debug.DrawLine(lineOrigin, shortDistHP, Color.red);

            movementDistance = Vector3.Distance(lineOrigin, shortDistHP);
        }
    }

    private void DrawDistanceRays()
    {
        float increment = 0.0f;
        Vector3 rayOrigin = bryceTransform.position;
        rayOrigin.y = 0.2f;

        for (int i = 0; i < 40; i++, increment += 0.5f)
        {
            Quaternion rotation = Quaternion.AngleAxis(-10.0f + increment, bryceTransform.up);
            Vector3 forward = rotation * bryceTransform.forward;
            Ray ray = new Ray(rayOrigin, forward.normalized);

            RaycastHit hitPoint1;

            if (Physics.Raycast(ray, out hitPoint1, maxDistance))
            {
                Debug.DrawLine(ray.origin, hitPoint1.point, Color.green);
            }
        }
    }
}
```

Infine, la classe **AvAnimation** che permette di scrivere il client per l'animazione dell'avatar istanziando la classe **BryceAvatar** e chiamando **OnFixedUpdate** dello stato corrente gestito dalla macchina a stati all'interno di FixedUpdate.

```cs
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
```

### Video

[![](https://dl.dropboxusercontent.com/s/shzado7ockbxork/Avatar.png?dl=1)](https://dl.dropboxusercontent.com/s/nqw7ozt68jyjygr/Avatar02.mp4?dl=0)
