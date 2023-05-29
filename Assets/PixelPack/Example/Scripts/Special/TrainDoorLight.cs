/**
 *  Change train's door light sprite when train starts moving or stops. 
 */

using UnityEngine;
using UniRx;
public class TrainDoorLight : BaseTile
{
    public Train Train;
    public Sprite RedLight;
    public Sprite GreenLight;
    private CompositeDisposable disposables = new CompositeDisposable();
    protected override void Start()
    {
        base.Start();    
        Train.MovingTrainObject.IsMoving.Subscribe(b => ChangeLightState(b)).AddTo(disposables);
    }
    private void OnDestroy()
    {
        disposables.Dispose();
    }

    private void ChangeLightState(bool isMoving)
    {
        spriteRenderer.sprite = isMoving ? RedLight : GreenLight;
    }
}
