/**
 *  Train/subway that extends WarpZone and uses MovingPlatform to 
 *  create behaviour that appears like player is entering train,
 *  which then moves from point A to point B and then player exits the
 *  train in point B.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
public class Train : WarpZone
{
    public bool DisableAfterOneRide = true;
    public float TrainTravelTime = 5f;
    public List<Vector3> TrainMovingPath = new List<Vector3>() { new Vector3(10f,0,0) };
    public MovingPlatform MovingTrainObject { get; private set; }

    private CompositeDisposable disposables = new CompositeDisposable();    
    private bool permanentlyDisabled = false;

    private void Awake()
    {
        //TODO: Refactor. Hack for circumvent timing issues with subscription.
        MovingTrainObject = GetComponentInParent<MovingPlatform>();
        if (MovingTrainObject == null)
            Debug.LogError("Train (object: " + gameObject.name + ") requires 'MovingPlatform' in parent object!");
        else
            MovingTrainObject.IsMoving.Subscribe(b => TrainMoving(b)).AddTo(disposables);
    }
    protected override void Start()
    {
        base.Start();
    }

    private void OnDestroy()
    {        
        disposables.Dispose();

        if (currentPlayer != null)
            //TODO: This is completely fucked in Unity. Dontdestroyinload actually depends on transform parenting: so if it's a player, call dontdestroyonload again...
            DontDestroyOnLoad(currentPlayer.gameObject);
    }

    /**
    * Extends WarpZone's PlayEnterAnimation by starting the platform movement in co-routine
    * when player enters the train. Re-parents player to Transform.
    */
    public override void PlayEnterAnimation()
    {
        base.PlayEnterAnimation();


        if (currentPlayer != null)
            currentPlayer.transform.parent = this.transform;

        StartCoroutine(StartMovingDelayed());

        //Disable script if DisableAfterOneRide
        if (DisableAfterOneRide)
        {
            collider2d.enabled = false;
            permanentlyDisabled = true;
        }
    }

    /**
    * Extends WarpZone's PlayExitAnimation. 
    * When player exits the train, she's re-parented to null, 
    * or actually to 'DontDestroyOnLoad'.
    */
    public override void PlayExitAnimation()
    {
        if (currentPlayer != null)
        {
            currentPlayer.transform.parent = null;
            DontDestroyOnLoad(currentPlayer.gameObject);
        }

        base.PlayExitAnimation();
    }

    private void TrainMoving(bool isMoving)
    {
        if (!permanentlyDisabled)
            collider2d.enabled = !isMoving;
    }

    private IEnumerator StartMovingDelayed()
    {
        yield return new WaitForSeconds(2f);
        MovingTrainObject.StartMoving(TrainMovingPath, 1, false, TrainTravelTime, Ease.InOutCubic);
    }

    
}
