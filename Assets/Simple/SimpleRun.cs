using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRun : MonoBehaviour
{
    public int objectCount = 8;
    public float radius = 5;
    public float loopTime = 2;
    public float delayTime = 0.11f;
    public AnimationCurve animationCurve;

    [SerializeField] private Transform objectsParentTrans;
    [SerializeField] private Transform helperLineParentTrans;
    
    private MoveLoop[] moveLoops;
    private Coroutine _co;
    
    public class MoveLoop
    {
        public Transform trans;
        public Vector3 pos1;
        public Vector3 pos2;
        public float angle;

        public float currentTime;
        public float loopTime;
        public float delayTime;
        
        public AnimationCurve animationCurve;

        public void Tick(float ct)
        {
            currentTime = ct;
            var t = (currentTime - delayTime) / loopTime;
            var pos = t < 0?
                new Vector3(100000, 0, 0) :
                pos1 + (pos2 - pos1) * animationCurve.Evaluate(t);
            
            trans.localPosition = pos;
        }
    }

    private void Start() => this.StartRun();
    
    public void StartRun()
    {
        this.StopRun();
        DestroyChildren(this.objectsParentTrans);

        if (this.objectCount <= 0)
        {
            Debug.Log("objectCount <= 0");
            return;
        }

        if (this.animationCurve == null || this.animationCurve.keys.Length == 0)
        {
            Debug.Log("animationCurve empty");
            return;
        }

        if (this.objectsParentTrans == null)
        {
            Debug.Log("objectsParentTrans null");
            return;
        }
        
        if (this.helperLineParentTrans == null)
        {
            Debug.Log("helperLineParentTrans null");
            return;
        }

        if (this.helperLineParentTrans.childCount != this.objectCount)
        {
            this.HideHelperLine();
        }

        void CreateMoveLoops()
        {
            var deltaTheta = Mathf.PI / this.objectCount;
            var deltaAngle = 180f / this.objectCount;

            moveLoops = new MoveLoop[this.objectCount];
            for (int i = 0; i < this.objectCount; i++)
            {
                var angle = i * deltaAngle;
                var theta1 = i * deltaTheta;
                var theta2 = theta1 + Mathf.PI;
                var pos1 = new Vector3(radius * Mathf.Cos(theta1), radius * Mathf.Sin(theta1), 0);
                var pos2 = -pos1;

                var gameObj = CreatePrimitive(
                    PrimitiveType.Sphere, 
                    this.objectsParentTrans, 
                    new Vector3(10000, 0, 0), 
                    Quaternion.identity, 
                    Vector3.one);

                moveLoops[i] = new MoveLoop()
                {
                    trans = gameObj.transform,
                    angle = angle,
                    pos1 = pos1,
                    pos2 = pos2,
                    currentTime = 0,
                    loopTime = loopTime,
                    delayTime = delayTime * i,
                    animationCurve = animationCurve,
                };
            }
        }

        CreateMoveLoops();
        _co = this.StartCoroutine(this.Run());
    }

    private IEnumerator Run()
    {
        float currentTime = 0;
        while (true)
        {
            yield return null;
            currentTime += Time.deltaTime;
            foreach (var moveLoop in moveLoops)
            {
                moveLoop.Tick(currentTime);
            }
        }
    }

    public void StopRun()
    {
        if (_co != null)
        {
            this.StopCoroutine(_co);
            _co = null;
        }
    }

    public void ShowHelperLine()
    {
        this.HideHelperLine();
        if (this.moveLoops == null)
        {
            return;
        }

        foreach (var moveLoop in this.moveLoops)
        {
            CreatePrimitive(
                PrimitiveType.Cube, 
                this.helperLineParentTrans, 
                Vector3.zero, 
                Quaternion.Euler(0, 0, moveLoop.angle),
                new Vector3(radius * 2, 0.1f, 0.1f));
        }
    }

    public void HideHelperLine()
    {
        DestroyChildren(this.helperLineParentTrans);
    }

    private static void DestroyChildren(Transform trans)
    {
        if (trans.childCount == 0)
        {
            return;
        }

        var list = new List<GameObject>(trans.childCount);
        foreach (Transform child in trans)
        {
            list.Add(child.gameObject);
        }

        foreach (var child in list)
        {
            Destroy(child);
        }
    }
    
    private static GameObject CreatePrimitive(PrimitiveType primitiveType, Transform parent, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        var go = CreatePrimitive(primitiveType);
        var trans = go.transform;
        trans.parent = parent;
        trans.position = pos;
        trans.rotation = rot;
        trans.localScale = scale;
        return go;
    }
    
    private static GameObject CreatePrimitive(PrimitiveType primitiveType)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (primitiveType == PrimitiveType.Cube)
            {
                return Instantiate(Resources.Load<GameObject>("Cube"));
            }
        
            if (primitiveType == PrimitiveType.Sphere)
            {
                return Instantiate(Resources.Load<GameObject>("Sphere"));
            }

            throw new NotSupportedException($"WebGL not Support CreatePrimitive: {primitiveType}");
        }
        
        return GameObject.CreatePrimitive(primitiveType);
    }
}