﻿using UnityEngine;
using System.Collections;

public class HandIconMover : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;

    [SerializeField,Tooltip("目的地までの距離")]
    private Vector3 endPositionDistance;

    [SerializeField, Tooltip("目的地にたどり着くまでの時間"), Range(0.1f, 5.0f)]
    private float moveTime;
    
    //Easingのスタートポジション
    private Vector3 startPosition;
    //Easingのエンドポジション
    private Vector3 endPosition;
    //Easingの起動し始める時間
    private float startTime;
    //Easingを行っていいかどうか
    private bool canEasing = true;

    void Start()
    {
        //初期位置設定
        startPosition = this.transform.position;
        //目的地の設定
        //※あえてendPositionDistanceを作ったのは、
        //子オブジェクトのローカル座標を基準にEasingさせたかったため
        endPosition += new Vector3(this.transform.position.x + endPositionDistance.x,
                                   this.transform.position.y + endPositionDistance.y,
                                   this.transform.position.z + endPositionDistance.z);
        //起動する時間の設定
        startTime = Time.timeSinceLevelLoad;
    }

    void Update()
    {
        //Rayが当たっている場合は、Easingをストップし、
        //当たっていない場合は常にEasingを行う
        if (!RayCast())
        {
            if (!canEasing)
            {
                canEasing = true;
                startTime = Time.timeSinceLevelLoad;
            }
            else if (canEasing)
            {
                RoundTripEasing(startTime);
            }
        }
        if(RayCast())
        {
            canEasing = false;
        }
    }
    //第一引数……動かし始める時間
    //private IEnumerator StartEasing(float startTime_,int delayTime_)
    void RoundTripEasing(float startTime_)
    {
        var diff = Time.timeSinceLevelLoad - startTime_;

        if (diff > moveTime)
        {
            this.transform.position = endPosition;
            
            canEasing = false;
        }

        var rate = diff / moveTime;
        var pos = curve.Evaluate(rate);

        this.transform.position = Vector3.Lerp(startPosition, endPosition, pos);

        if (rate >= 1)
        {
            canEasing = true;
            rate = 0;

            startTime = Time.timeSinceLevelLoad;

            RoundTripEasing(startTime);
        }
    }

    bool RayCast()
    {
        //カメラの場所からポインタの場所に向かってレイを飛ばす
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        //レイが何か当たっているかを調べる
        if (Physics.Raycast(ray, out hit))
        {
            //当たったオブジェクトを格納
            GameObject obj = hit.collider.gameObject;

            if (obj.CompareTag("UI"))
            {
                return true;
            }
        }

        return false;
    }
}
