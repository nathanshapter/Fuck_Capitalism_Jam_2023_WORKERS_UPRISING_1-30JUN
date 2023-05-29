/**
 * Moving platform that has a editor waypoint system.
 * Movement utilizes DOTween library.
 */

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UniRx;

public class MovingPlatform : BaseTile
{
    public bool StartMovingWhenInViewport = false;
    public bool ReturnToStartingPoint = false;
    public float TotalDuration = 5f;
    public PathType Type = PathType.Linear;
    public PathMode Mode = PathMode.Sidescroller2D;
    public Ease EasingType = Ease.Linear;

    public int Resolution = 10;
    public int Loops = -1;

    [HideInInspector]    
    public BooleanNotifier IsMoving = new BooleanNotifier(false);

    public List<Vector3> MovePoints {
        get { return m_TargetPositions; }
        set { m_TargetPositions = value; }
    }

    [SerializeField]
    private List<Vector3> m_TargetPositions = new List<Vector3>() { new Vector3(-0.1f, 0, 0), new Vector3(0.1f, 0, 0) };
    private bool initStartMovingWhenInViewport;
    private Tween PathTween;

    protected override void Start()
    {
        base.Start();
        initStartMovingWhenInViewport = StartMovingWhenInViewport;

        if (!StartMovingWhenInViewport)
            StartMoving(m_TargetPositions, Loops, ReturnToStartingPoint, TotalDuration, EasingType, Type, Mode, Resolution);
    }

    private void FixedUpdate()
    {
        //Activate moving platform if transform.position point is within camera viewport
        if (!IsMoving.Value && StartMovingWhenInViewport && CameraHelper.IsPointWithinViewport(Camera.main, Camera.main.transform.position, transform.position))
        {
            StartMovingWhenInViewport = false; //This needs to happen only once...
            StartMoving(m_TargetPositions, Loops, ReturnToStartingPoint, TotalDuration, EasingType, Type, Mode, Resolution);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Parent Character to the moving platform to prevent sliding

        if (other.transform.GetComponent<CharacterBase>() != null)        
            other.transform.parent = transform;
        
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        //Remove parenting on collision exit
        if (other.transform.GetComponent<CharacterBase>() != null)
        {
            other.transform.parent = null;
            //TODO: This is completely fucked in Unity. DontDesroyOnLoad actually depends on transform parenting: so if it's a player, call dontdestroyonload again...
            if (other.transform.GetComponent<PlayerCharacter>() != null)
                DontDestroyOnLoad(other.gameObject);
        }    
            
    }

    /**
    * Start platform movement with DOTween's DOPath.
    * @param wayPoints List of Vector3 way points.
    * @param loops Loop n times. -1 = loop infinetely.
    * @param returnToStartingPoint Return to (transform) starting point?
    * @param easingType DOTween Ease easing type.
    * @param type Linear or curved path. 
    * @param mode 2D or 3D path.
    * @param resolution Curved path detail level.
    */

    public void StartMoving(List<Vector3> wayPoints, int loops, bool returnToStartingPoint, float totalDuration, Ease easingType = Ease.Linear, PathType type = PathType.Linear, PathMode mode = PathMode.Sidescroller2D, int resolution = 10)
    {
        IsMoving.Value = true;

        List<Vector3> loopPoints = wayPoints.Select(pos => pos + transform.position).ToList();
        
        if (returnToStartingPoint)
            loopPoints.Add(transform.position); //Add starting point to last waypoint if ReturnToStartingPoint true
        
        //Set and start path
        PathTween = transform.DOPath(loopPoints.ToArray(), totalDuration, type, mode, resolution).SetEase(easingType).SetLoops(loops).OnComplete(() => EndMovingCallBack());
        
    }

    /**
    * Extend reset by setting StartMovingWhenInViewport back to initial value.
    */
    public override void Reset()
    {
        StartMovingWhenInViewport = initStartMovingWhenInViewport;
        base.Reset();
    }

    protected virtual void EndMovingCallBack()
    {
        IsMoving.Value = false;
    }
}
