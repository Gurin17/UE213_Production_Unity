using System;
using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PawnFollower : MonoBehaviour
    {
        [Header("Path Manager")]
        public PathCreator pathCreator;
        public FollowBehavior followBehavior;
        public float speed = 5;
        public float heightOffset = 0.0f;
        public Vector3 vehiculeRotation;
        public bool startFromEnd = false;
        public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        

        private float distanceTravelled;
        private float start;
        private float end;
        private float t;
        private bool reverse = false;

        void Start()
        {
            distanceTravelled = 0f;
            start = 0f;
            t = 0f;
            if (pathCreator != null)
            {
                if (startFromEnd)
                {
                    distanceTravelled = pathCreator.path.length;
                }
                end = pathCreator.path.length;
            }

        }

        void Update()
        {

            if (pathCreator != null)
            {
                if (followBehavior == FollowBehavior.reverse)
                {
                    if (!reverse)
                    {
                        t += Time.deltaTime * speed;

                        if (t >= 1f)
                        { 
                            reverse = true;
                        }
                    }
                    else
                    {
                        t -= Time.deltaTime * speed;

                        if (t <= 0f)
                        {
                            reverse = false;
                        }
                    }
                }
                else
                {
                    t += Time.deltaTime * speed;

                    if (t >= 1f)
                    {
                        t = 0f;
                    }
                }
                

                float s = t / 1f;
                distanceTravelled += (Mathf.Lerp(start, end, curve.Evaluate(s)) - distanceTravelled);

                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Reverse) + (transform.up * heightOffset);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Reverse) * Quaternion.Euler(vehiculeRotation.x, vehiculeRotation.y, vehiculeRotation.z);
            }
        }
    }

    public enum FollowBehavior
    {
        reverse,
        loop
    }
}